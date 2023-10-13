using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerDocument<R, S> : IFileDocument where R : FileManagerFile {
        public R CurrentFile { get; protected set; }
        protected VerifiedStatus Verified => CurrentFile == null ? VerifiedStatus.UNKNOWN : CurrentFile.Verified;

        public string DisplayName => string.IsNullOrEmpty( Name ) ? ReplaceDisplay : Name;
        protected string Name = "";

        protected SelectResult Source;
        public string SourceDisplay => Source == null ? "[NONE]" : Source.DisplayString;
        public string SourcePath => Source == null ? "" : Source.Path;

        protected SelectResult Replace;
        public string ReplaceDisplay => Replace == null ? "[NONE]" : Replace.DisplayString;
        public string ReplacePath => ( Disabled || Replace == null ) ? "" : Replace.Path;
        protected bool Disabled = false;

        public string WriteLocation { get; protected set; }

        public abstract string Id { get; }
        public abstract string Extension { get; }

        protected readonly FileManagerBase Manager;

        public bool Unsaved = false;
        protected DateTime LastUpdate = DateTime.Now;

        public FileManagerDocument( FileManagerBase manager, string writeLocation ) {
            Manager = manager;
            WriteLocation = writeLocation;
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = null;
            if( CurrentFile == null ) return false;

            replacePath = ReplacePath.ToLower().Equals( path.ToLower() ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        protected abstract R FileFromReader( BinaryReader reader, bool verify );

        protected void LoadLocal( string path, bool verify ) {
            if( !File.Exists( path ) ) {
                Dalamud.Error( $"Local file: [{path}] does not exist" );
                return;
            }

            if( !path.EndsWith( $".{Extension}" ) ) {
                Dalamud.Error( $"{path} is the wrong file type" );
                return;
            }

            try {
                using var reader = new BinaryReader( File.Open( path, FileMode.Open ) );
                CurrentFile?.Dispose();
                CurrentFile = FileFromReader( reader, verify );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error Reading File" );
                UiUtils.ErrorNotification( "Error reading file" );
            }
        }

        protected void LoadGame( string path, bool verify ) {
            if( !Dalamud.DataManager.FileExists( path ) ) {
                Dalamud.Error( $"Game file: [{path}] does not exist" );
                return;
            }

            if( !path.EndsWith( $".{Extension}" ) ) {
                Dalamud.Error( $"{path} is the wrong file type" );
                return;
            }

            try {
                var file = Dalamud.DataManager.GetFile( path );
                using var ms = new MemoryStream( file.Data );
                using var reader = new BinaryReader( ms );
                CurrentFile?.Dispose();
                CurrentFile = FileFromReader( reader, verify );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error Reading File" );
                UiUtils.ErrorNotification( "Error reading file" );
            }
        }

        // =================

        public void SetSource( SelectResult result ) {
            if( result == null ) return;
            Source = result;

            if( result.Type == SelectResultType.Local ) LoadLocal( result.Path, true );
            else LoadGame( result.Path, true );

            if( CurrentFile != null ) {
                WriteFile( WriteLocation );
            }
        }

        protected void RemoveSource() {
            CurrentFile?.Dispose();
            CurrentFile = null;
            Source = null;
            Unsaved = false;
        }

        public void SetReplace( SelectResult result ) { Replace = result; }

        protected void RemoveReplace() { Replace = null; }

        // =====================

        protected void WriteFile( string path ) {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) Dalamud.Log( $"Wrote {Id} file to {path}" );
            File.WriteAllBytes( path, CurrentFile.ToBytes() );
        }

        protected void ExportRaw() => UiUtils.WriteBytesDialog( $".{Extension}", CurrentFile.ToBytes(), Extension, "ExportedFile" );

        public void Update() {
            if( ( DateTime.Now - LastUpdate ).TotalSeconds <= 0.2 ) return;
            LastUpdate = DateTime.Now;
            Unsaved = false;

            CurrentFile?.Update();

            if( Plugin.Configuration.UpdateWriteLocation ) {
                var newWriteLocation = Manager.NewWriteLocation;
                WriteFile( newWriteLocation );
                WriteLocation = newWriteLocation;
            }
            else {
                WriteFile( WriteLocation );
            }

            if( CurrentFile != null && !ReplacePath.Contains( ".sklb" ) ) {
                Plugin.ResourceLoader.ReloadPath( ReplacePath, WriteLocation, CurrentFile.GetPapIds(), CurrentFile.GetPapTypes() );
            }

            Plugin.ResourceLoader.ReRender();
        }

        // =======================

        protected void LoadWorkspace( string localPath, string relativeLocation, string name, SelectResult source, SelectResult replace, bool disabled ) {
            Name = name ?? "";
            Source = source;
            Replace = replace;
            Disabled = disabled;
            LoadLocal( WorkspaceUtils.ResolveWorkspacePath( relativeLocation, localPath ), false );
            if( CurrentFile != null ) CurrentFile.Verified = VerifiedStatus.WORKSPACE;
            WriteFile( WriteLocation );
        }

        public string GetExportSource() => SourceDisplay;

        public string GetExportReplace() => DisplayName;

        public bool CanExport() => CurrentFile != null && !string.IsNullOrEmpty( ReplacePath );

        public void PenumbraExport( string modFolder, Dictionary<string, string> filesOut ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;
            var data = CurrentFile.ToBytes();

            PenumbraUtils.WriteBytes( data, modFolder, path, filesOut );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;

            var modData = TexToolsUtils.CreateType2Data( CurrentFile.ToBytes() );
            simplePartsOut.Add( TexToolsUtils.CreateModResource( path, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        public abstract S GetWorkspaceMeta( string newPath );

        public void WorkspaceExport( List<S> meta, string rootPath, string newPath ) {
            if( CurrentFile == null ) return;

            var newFullPath = Path.Combine( rootPath, newPath );
            File.WriteAllBytes( newFullPath, CurrentFile.ToBytes() );
            meta.Add( GetWorkspaceMeta( newPath ) );
        }

        // ====== DRAWING ==========

        public virtual void CheckKeybinds() {
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) Manager.GetCopyManager()?.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) Manager.GetCopyManager()?.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) Manager.GetCommandManager()?.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) Manager.GetCommandManager()?.Redo();
        }

        public void Draw() {
            if( Plugin.Configuration.WriteLocationError ) {
                ImGui.TextWrapped( $"VFXEditor does not have access to {Plugin.Configuration.WriteLocation}. Please go to [File > Settings] and change it, then restart your game" );
                return;
            }

            var searchWidth = ImGui.GetContentRegionAvail().X - 160 - 125;

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0 ) ) )
            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.Columns( 3, "Columns", false );
                ImGui.SetColumnWidth( 0, 160 );
            }
            DrawInputTextColumn();

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.NextColumn();
                ImGui.SetColumnWidth( 1, searchWidth );
            }
            DrawSearchBarsColumn();

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.NextColumn();
                ImGui.SetColumnWidth( 2, 126 );
            }
            DrawExtraColumn();

            ImGui.Columns( 1 );

            DrawBody();
        }

        protected virtual void DrawInputTextColumn() {
            var pos = ImGui.GetCursorScreenPos() + new Vector2( 5, 0 );
            var height = ImGui.GetFrameHeight();
            var spacing = ImGui.GetStyle().ItemSpacing.Y;

            var radius = 5f;
            var width = 15f;
            var segmentResolution = 10;
            var thickness = 2;

            var arrowHeight = 8;
            var arrowWidth = 8;

            var drawList = ImGui.GetWindowDrawList();
            var topLeft = pos + new Vector2( 0, height * 0.5f );
            var topRight = topLeft + new Vector2( width, 0 );
            var bottomRight = pos + new Vector2( width, height * 1.5f + spacing - 1 );
            var bottomLeft = new Vector2( topLeft.X, bottomRight.Y );

            var hovered = ImGui.IsWindowFocused( ImGuiFocusedFlags.RootWindow ) && ImGui.IsMouseHoveringRect( topLeft - new Vector2( 5, 5 ), bottomRight + new Vector2( 5, 5 ) );

            var color = hovered ?
                ImGui.ColorConvertFloat4ToU32( UiUtils.YELLOW_COLOR ) :
                ( Disabled ?
                    ImGui.ColorConvertFloat4ToU32( UiUtils.RED_COLOR ) :
                    ImGui.GetColorU32( ImGuiCol.TextDisabled )
               );

            if( hovered && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) Disabled = !Disabled;

            var topLeftCurveCenter = new Vector2( topLeft.X + radius, topLeft.Y + radius );
            var bottomLeftCurveCenter = new Vector2( bottomLeft.X + radius, bottomLeft.Y - radius );

            drawList.PathArcTo( topLeftCurveCenter, radius, DegreesToRadians( 180 ), DegreesToRadians( 270 ), segmentResolution );
            drawList.PathStroke( color, ImDrawFlags.None, thickness );

            drawList.PathArcTo( bottomLeftCurveCenter, radius, DegreesToRadians( 90 ), DegreesToRadians( 180 ), segmentResolution );
            drawList.PathStroke( color, ImDrawFlags.None, thickness );

            drawList.AddLine( topLeft + new Vector2( -0.5f, radius - 0.5f ), bottomLeft + new Vector2( -0.5f, -radius + 0.5f ), color, thickness );
            drawList.AddLine( topLeft + new Vector2( radius - 0.5f, -0.5f ), topRight + new Vector2( 0, -0.5f ), color, thickness );
            drawList.AddLine( bottomLeft + new Vector2( radius - 0.5f, -0.5f ), bottomRight + new Vector2( -4, -0.5f ), color, thickness );

            if( Disabled ) {
                var crossCenter = bottomRight + new Vector2( -4, 0 );
                var crossHeight = arrowHeight / 2;

                drawList.AddLine( crossCenter + new Vector2( crossHeight, crossHeight ), crossCenter + new Vector2( -crossHeight, -crossHeight ), color, thickness );
                drawList.AddLine( crossCenter + new Vector2( -crossHeight, crossHeight ), crossCenter + new Vector2( crossHeight, -crossHeight ), color, thickness );
            }
            else {
                drawList.AddTriangleFilled( bottomRight, bottomRight + new Vector2( -arrowWidth, arrowHeight / 2 ), bottomRight + new Vector2( -arrowWidth, -arrowHeight / 2 ), color );
            }

            if( hovered ) UiUtils.Tooltip( "Toggle replacement", true );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 25 );
            ImGui.Text( $"Loaded {Id}" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 25 );
            ImGui.Text( $"{Id} Being Replaced" );
        }

        private static float DegreesToRadians( float degrees ) => MathF.PI / 180 * degrees;

        protected void DrawSearchBarsColumn() {
            var timesWidth = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Times );
            var searchWidth = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Search );
            // 3 * 2 for spacing, 25 for some more padding
            var inputWidth = ImGui.GetColumnWidth() - timesWidth - searchWidth - ( 3 * 2 ) - 20;

            DisplaySourceBar( inputWidth );
            DisplayReplaceBar( inputWidth );
        }

        protected virtual void DrawExtraColumn() { }

        protected void DisplaySourceBar( float inputSize ) {
            using var _ = ImRaii.PushId( "Source" );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );
            var sourceString = Source == null ? "" : Source.DisplayString;

            // Remove
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), UiUtils.RED_COLOR ) ) RemoveSource();
            }

            // Input
            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputTextWithHint( "", "[NONE]", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );

            // Search
            ImGui.SameLine();

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) Manager.ShowSource();
            }
        }

        protected void DisplayReplaceBar( float inputSize ) {
            using var _ = ImRaii.PushId( "Replace" );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );
            var previewString = Replace == null ? "" : Replace.DisplayString;

            // Remove
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), UiUtils.RED_COLOR ) ) RemoveReplace();
            }

            // Input
            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputTextWithHint( "", "[NONE]", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );
            if( Replace != null && ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "CopyPopup" );

            if( Replace != null && ImGui.BeginPopup( "CopyPopup" ) ) {
                ImGui.Text( Replace.Path );
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 2 );
                if( ImGui.SmallButton( "Copy" ) ) ImGui.SetClipboardText( Replace.Path );
                ImGui.EndPopup();
            }

            // Search
            ImGui.SameLine();

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) Manager.ShowReplace();
            }
        }

        protected virtual void DisplayFileControls() {
            if( UiUtils.OkButton( "UPDATE" ) ) Update();

            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, ImGui.GetStyle().FramePadding + new Vector2( 0, 1 ) ) )
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Download.ToIconString() ) ) ExportRaw();
            }
            UiUtils.Tooltip( "Export as a raw file" );

            ImGui.SameLine();
            UiUtils.ShowVerifiedStatus( Verified );

            var warnings = GetWarningText();
            if( !string.IsNullOrEmpty( warnings ) ) {
                using var _ = ImRaii.PushColor( ImGuiCol.Text, UiUtils.RED_COLOR );
                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.Text( FontAwesomeIcon.ExclamationCircle.ToIconString() );
                }
                UiUtils.Tooltip( warnings );
            }
        }

        protected virtual string GetWarningText() => "";

        protected virtual void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                using var _ = ImRaii.PushId( "Body" );
                CurrentFile.Draw();
            }
        }

        public void DrawRename() {
            Name ??= "";
            using var _ = ImRaii.PushId( "Rename" );
            ImGui.InputTextWithHint( "", ReplaceDisplay, ref Name, 64, ImGuiInputTextFlags.AutoSelectAll );
        }

        public virtual void Dispose() {
            CurrentFile?.Dispose();
            CurrentFile = null;
            File.Delete( WriteLocation );
        }

        // ========================

        protected static void DisplayBeginHelpText() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 15 );

            var availWidth = ImGui.GetContentRegionMax().X;
            var width = availWidth > 500 ? 500 : availWidth; // cap out at 300
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( availWidth - width ) / 2 );
            using var child = ImRaii.Child( "HelpTextChild", new Vector2( width, -1 ) );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 30 );

            var buttonWidth = ImGui.GetContentRegionMax().X - ImGui.GetStyle().FramePadding.X * 2;

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.41764705882f, 0.41764705882f, 0.41764705882f, 1 ) );
            if( ImGui.Button( "Wiki + Guides", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
            }
            ImGui.PopStyleColor();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.21764705882f, 0.21764705882f, 0.21764705882f, 1 ) );
            if( ImGui.Button( "Github", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
            }
            ImGui.PopStyleColor();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.21764705882f, 0.21764705882f, 0.21764705882f, 1 ) );
            if( ImGui.Button( "Report an Issue", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
            }
            ImGui.PopStyleColor();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.33725490196f, 0.38431372549f, 0.96470588235f, 1 ) );
            if( ImGui.Button( "XIVLauncher Discord", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://discord.gg/3NMcUV5" );
            }
            ImGui.PopStyleColor();
        }

        private static readonly string WarningText = "DO NOT modify movement abilities (dashes, backflips). Please read a guide before attempting to modify a .tmb or .pap file";

        protected static void DrawAnimationWarning() {
            using var color = ImRaii.PushColor( ImGuiCol.Border, new Vector4( 1, 0, 0, 0.3f ) );
            color.Push( ImGuiCol.ChildBg, new Vector4( 1, 0, 0, 0.1f ) );

            var style = ImGui.GetStyle();
            var iconSize = UiUtils.GetIconSize( FontAwesomeIcon.Globe ) + 2 * style.FramePadding;
            var textWidth = ImGui.GetContentRegionAvail().X - ( 2 * style.WindowPadding.X ) - ( 2 * style.ItemSpacing.X ) - iconSize.X;
            var textSize = ImGui.CalcTextSize( WarningText, textWidth );

            using var child = ImRaii.Child( "Warning", new Vector2( -1, Math.Max( textSize.Y, iconSize.Y ) + ( 2 * style.WindowPadding.Y ) ), true, ImGuiWindowFlags.NoScrollbar );
            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.Columns( 2, "##WarningColumns", false );
                ImGui.SetColumnWidth( 0, textWidth );
            }

            using( var textColor = ImRaii.PushColor( ImGuiCol.Text, 0xFF4A67FF ) ) {
                ImGui.TextWrapped( WarningText );
            }

            ImGui.NextColumn();
            ImGui.SetColumnWidth( 1, iconSize.X + ( 2 * style.ItemSpacing.X ) );

            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( FontAwesomeIcon.Globe.ToIconString() ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );

            ImGui.Columns( 1 );
        }
    }
}
