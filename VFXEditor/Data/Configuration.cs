using Dalamud.Configuration;
using Dalamud.Interface.Utility;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Library;
using VfxEditor.Select;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Ui;

namespace VfxEditor {
    [Serializable]
    public unsafe class ManagerConfiguration {
        public List<SelectResult> RecentItems = new();
        public List<SelectResult> Favorites = new();
        public bool UseCustomWindowColor = false;
        public Vector4 TitleBg = *ImGui.GetStyleColorVec4( ImGuiCol.TitleBg );
        public Vector4 TitleBgActive = *ImGui.GetStyleColorVec4( ImGuiCol.TitleBgActive );
        public Vector4 TitleBgCollapsed = *ImGui.GetStyleColorVec4( ImGuiCol.TitleBgCollapsed );
    }

    [Serializable]
    public class Configuration : DalamudWindow, IPluginConfiguration {
        public int Version { get; set; } = 0;
        public bool IsEnabled { get; set; } = true;

        public bool LogAllFiles = false;
        public bool LogDebug = false;
        public bool LogVfxDebug = false;
        public bool LogVfxTriggers = false;

        public bool UpdateWriteLocation = true;
        public bool AutosaveEnabled = false;
        public int AutosaveSeconds = 300;
        public int SaveRecentLimit = 10;

        public bool HideWithUI = true;
        public bool ShowTabBar = true;
        public bool DocumentPopoutShowSource = false;
        public bool SelectDialogLogAllFilesHidden = true;
        public bool UseDegreesForAngles = false;

        public bool OverlayLimit = true;
        public float OverlayRemoveDelay = 1;

        public bool BlockGameInputsWhenFocused = false;
        public string WriteLocation = Path.Combine( new[] {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XIVLauncher",
            "pluginConfigs",
            "VFXEditor",
        } );

        public List<(string, string)> RecentWorkspaces = new();

        public bool VfxSpawnLoop = false;
        public float VfxSpawnDelay = 0.1f;
        public bool VfxSpawnSplit = false;

        // ===== [ OBSOLETE ] =======
        public List<SelectResult> RecentSelects = new();
        public List<SelectResult> RecentSelectsTMB = new();
        public List<SelectResult> RecentSelectsPAP = new();
        public List<SelectResult> RecentSelectsScd = new();
        public List<SelectResult> FavoriteVfx = new();
        public List<SelectResult> FavoriteTmb = new();
        public List<SelectResult> FavoritePap = new();
        public List<SelectResult> FavoriteScd = new();
        // ===========================

        public Dictionary<string, ManagerConfiguration> ManagerConfigs = new();

        public KeybindConfiguration SaveKeybind = new();
        public KeybindConfiguration SaveAsKeybind = new();
        public KeybindConfiguration OpenKeybind = new();
        public KeybindConfiguration UpdateKeybind = new();
        public KeybindConfiguration CopyKeybind = new();
        public KeybindConfiguration PasteKeybind = new();
        public KeybindConfiguration UndoKeybind = new();
        public KeybindConfiguration RedoKeybind = new();
        public KeybindConfiguration SpawnOnSelfKeybind = new();
        public KeybindConfiguration SpawnOnGroundKeybind = new();
        public KeybindConfiguration SpawnOnTargetKeybind = new();

        public List<LibraryProps> VFXNodeLibraryItems = new();
        public List<LibraryProps> VfxTextureLibraryItems = new();
        public bool VfxTextureDefaultLoaded = false;

        public bool LoopMusic = true;
        public bool LoopSoundEffects = false;
        public float ScdVolume = 1f;
        public bool SimulateScdLoop = false;

        public float CurveEditorHeight = 300;
        public Vector4 CurveEditorLineColor = new( 0, 0.1f, 1, 1 );
        public Vector4 CurveEditorPointColor = new( 1 );
        public Vector4 CurveEditorSelectedColor = new( 1.000f, 0.884f, 0.561f, 1f );
        public Vector4 CurveEditorPrimarySelectedColor = new( 0.984f, 0.726f, 0.011f, 1.0f );
        public List<Vector4> CurveEditorPalette = new();
        public int CurveEditorLineWidth = 2;
        public int CurveEditorColorRingSize = 3;
        public int CurveEditorGrabbingDistance = 25;
        public int CurveEditorPointSize = 7;
        public int CurveEditorSelectedSize = 10;
        public int CurveEditorPrimarySelectedSize = 12;
        public Vector4 TimelineSelectedColor = new( 1f, 0.532f, 0f, 1f );
        public Vector4 TimelineBarColor = new( 0.44f, 0.457f, 0.492f, 1f );

        public Vector4 FileBrowserSelectedColor = new( 1.000f, 0.884f, 0.561f, 1f );
        public Vector4 FileBrowserFolderColor = new( 0.516f, 0.859f, 1f, 1f );
        public Vector4 FileBrowserCodeColor = new( 0.229f, 1f, 0.832f, 1f );
        public Vector4 FileBrowserMiscColor = new( 1f, 0.789f, 0.233f, 1f );
        public Vector4 FileBrowserImageColor = new( 0.321f, 1f, 0.310f, 1f );
        public Vector4 FileBrowserFfxivColor = new( 1f, 0.543f, 0.508f, 1.0f );
        public Vector4 FileBrowserArchiveColor = new( 1f, 0.475f, 0.805f, 1.0f );
        public bool FileBrowserPreviewOpen = true;
        public bool FileBrowserImagePreview = true;
        public bool FileBrowserOverwriteDontAsk = false;

        public bool PhybSkeletonSplit = true;
        public bool EidSkeletonSplit = true;
        public bool ShowBoneNames = true;
        public BoneDisplay SklbBoneDisplay = BoneDisplay.Blender_Style_Perpendicular;
        public bool SklbMappingIndexDisplay = false;
        public bool SklbMappingUpdateExisting = true;

        public bool ModelWireframe = false;
        public bool ModelShowEdges = true;
        public bool ModelShowEmitters = true;

        public Vector4 LuaParensColor = new( 0.5f, 0.5f, 0.5f, 1f );
        public Vector4 LuaFunctionColor = new( 0f, 0.439f, 1f, 1f );
        public Vector4 LuaLiteralColor = new( 0.639f, 0.207f, 0.933f, 1f );
        public Vector4 LuaVariableColor = new( 0.125f, 0.67058f, 0.45098f, 1f );

        public int PngMips = 9;
        public TextureFormat PngFormat = TextureFormat.DXT5;

        public string CustomPathPrefix = "vfx/custom/";

        [NonSerialized]
        public bool WriteLocationError = false;

        public Configuration() : base( "Settings", false, new( 300, 200 ), Plugin.WindowSystem ) { }

        public void Setup() {
            Dalamud.PluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;

            // Move old configurations over to new
            ProcessOldManagerConfigs( RecentSelects, FavoriteVfx, "Vfx" );
            ProcessOldManagerConfigs( RecentSelectsTMB, FavoriteTmb, "Tmb" );
            ProcessOldManagerConfigs( RecentSelectsPAP, FavoritePap, "Pap" );
            ProcessOldManagerConfigs( RecentSelectsScd, FavoriteScd, "Scd" );

            try { Directory.CreateDirectory( WriteLocation ); }
            catch( Exception ) { WriteLocationError = true; }

            Dalamud.Log( "Write location: " + WriteLocation );

            if( CurveEditorPalette.Count == 0 ) {
                CurveEditorPalette.AddRange( ImGuiHelpers.DefaultColorPalette( 56 ) );
            }
        }

        public ManagerConfiguration GetManagerConfig( string id ) {
            if( ManagerConfigs.TryGetValue( id, out var config ) ) return config;
            var newConfig = new ManagerConfiguration();
            ManagerConfigs.Add( id, newConfig );
            return newConfig;
        }

        private void ProcessOldManagerConfigs( List<SelectResult> recent, List<SelectResult> favorites, string key ) {
            if( recent.Count == 0 && favorites.Count == 0 ) return;

            if( !ManagerConfigs.ContainsKey( key ) ) ManagerConfigs[key] = new();
            ManagerConfigs[key].RecentItems.AddRange( recent );
            ManagerConfigs[key].Favorites.AddRange( favorites );

            recent.Clear();
            favorites.Clear();
        }

        public void AddRecent( List<SelectResult> recentList, SelectResult result ) {
            if( result == null ) return;
            recentList.RemoveAll( result.CompareTo );

            recentList.Add( result );
            if( recentList.Count > SaveRecentLimit ) recentList.RemoveRange( 0, recentList.Count - SaveRecentLimit );
            Save();
        }

        public void AddRecentWorkspace( string path ) {
            RecentWorkspaces.RemoveAll( x => x.Item2 == path );
            var name = Path.GetFileName( path );
            RecentWorkspaces.Insert( 0, (name, path) );
            if( RecentWorkspaces.Count > 10 ) RecentWorkspaces.RemoveRange( 10, RecentWorkspaces.Count - 10 );
            Save();
        }

        public void Save() {
            Dalamud.PluginInterface.SavePluginConfig( this );
            Dalamud.PluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
        }

        // =================

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "Settings" );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Configuration" ) ) {
                DrawConfiguration();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Keybinds" ) ) {
                DrawKeybinds();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "File Browser" ) ) {
                DrawFileBrowser();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Vfx" ) ) {
                DrawVfx();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Tmb" ) ) {
                DrawTmb();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "File Editors" ) ) {
                DrawEditorSpecific();
                ImGui.EndTabItem();
            }
        }

        private void DrawConfiguration() {
            using var child = ImRaii.Child( "Config" );

            if( ImGui.CollapsingHeader( "Saving", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                ImGui.TextDisabled( "Changes to the temp file location may require a restart to take effect" );
                if( ImGui.InputText( "Write Location", ref WriteLocation, 255 ) ) Save();
                if( ImGui.Checkbox( "Refresh on Update", ref UpdateWriteLocation ) ) Save();
                if( ImGui.Checkbox( "Autosave Workspace", ref AutosaveEnabled ) ) Save();
                using var autosaveDim = ImRaii.PushStyle( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f, !AutosaveEnabled );
                using var indent = ImRaii.PushIndent();
                ImGui.SetNextItemWidth( 120 );
                if( ImGui.InputInt( "Autosave Time (seconds)", ref AutosaveSeconds ) ) Save();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.CollapsingHeader( "Logging", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                if( ImGui.Checkbox( "All Files", ref LogAllFiles ) ) Save();
                if( ImGui.Checkbox( "Debug Information", ref LogDebug ) ) Save();
                if( ImGui.Checkbox( "Vfx Debug Information", ref LogVfxDebug ) ) Save();
                if( ImGui.Checkbox( "Vfx Triggers", ref LogVfxTriggers ) ) Save();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.CollapsingHeader( "UI", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                if( ImGui.Checkbox( "Hide with Game UI", ref HideWithUI ) ) Save();
                if( ImGui.Checkbox( "Show Tab Bar", ref ShowTabBar ) ) Save();
                ImGui.SetNextItemWidth( 135 );
                if( ImGui.InputInt( "Recent File Limit", ref SaveRecentLimit ) ) {
                    SaveRecentLimit = Math.Max( SaveRecentLimit, 0 );
                    Save();
                }
                ImGui.SetNextItemWidth( 135 );
                if( ImGui.InputFloat( "Overlay Remove Delay", ref OverlayRemoveDelay ) ) Save();
                if( ImGui.Checkbox( "Limit Overlay by Distance", ref OverlayLimit ) ) Save();
                if( ImGui.Checkbox( "Split Spawn in Select Dialog", ref VfxSpawnSplit ) ) Save();
            }
        }

        private void DrawKeybinds() {
            if( ImGui.Checkbox( "Block Game Inputs When VFXEditor is Focused", ref BlockGameInputsWhenFocused ) ) Save();

            using var child = ImRaii.Child( "Keybinds", new Vector2( -1 ), false );

            if( SaveKeybind.Draw( "Save" ) ) Save();
            if( SaveAsKeybind.Draw( "Save As" ) ) Save();
            if( OpenKeybind.Draw( "Open" ) ) Save();
            if( CopyKeybind.Draw( "Copy" ) ) Save();
            if( PasteKeybind.Draw( "Paste" ) ) Save();
            if( UndoKeybind.Draw( "Undo" ) ) Save();
            if( RedoKeybind.Draw( "Redo " ) ) Save();
            if( UpdateKeybind.Draw( "Update" ) ) Save();
            if( SpawnOnSelfKeybind.Draw( "Spawn on Self (Vfx only)" ) ) Save();
            if( SpawnOnGroundKeybind.Draw( "Spawn on Ground (Vfx only)" ) ) Save();
            if( SpawnOnTargetKeybind.Draw( "Spawn on Target (Vfx only)" ) ) Save();
        }

        private void DrawVfx() {
            using var child = ImRaii.Child( "Vfx" );

            if( ImGui.CollapsingHeader( "Curve Editor", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                if( ImGui.InputFloat( "Editor Height", ref CurveEditorHeight ) ) Save();

                if( ImGui.ColorEdit4( "Line Color", ref CurveEditorLineColor ) ) Save();
                if( ImGui.ColorEdit4( "Point Color", ref CurveEditorPointColor ) ) Save();
                if( ImGui.ColorEdit4( "Primary Selected Color", ref CurveEditorPrimarySelectedColor ) ) Save();
                if( ImGui.ColorEdit4( "Selected Color", ref CurveEditorSelectedColor ) ) Save();

                if( ImGui.InputInt( "Line Width", ref CurveEditorLineWidth ) ) Save();
                if( ImGui.InputInt( "Color Ring Width", ref CurveEditorColorRingSize ) ) Save();
                if( ImGui.InputInt( "Point Size", ref CurveEditorPointSize ) ) Save();
                if( ImGui.InputInt( "Primary Selected Size", ref CurveEditorPrimarySelectedSize ) ) Save();
                if( ImGui.InputInt( "Selected Size", ref CurveEditorSelectedSize ) ) Save();
                if( ImGui.InputInt( "Grab Distance", ref CurveEditorGrabbingDistance ) ) Save();
            }

            if( ImGui.CollapsingHeader( "Timeline Editor", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                if( ImGui.ColorEdit4( "Selected Color", ref TimelineSelectedColor ) ) Save();
                if( ImGui.ColorEdit4( "Bar Color", ref TimelineBarColor ) ) Save();
            }
        }

        private void DrawTmb() {
            using var child = ImRaii.Child( "Tmb" );

            if( ImGui.CollapsingHeader( "Lua", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                if( ImGui.ColorEdit4( "Parentheses Color", ref LuaParensColor ) ) Save();
                if( ImGui.ColorEdit4( "Function Color", ref LuaFunctionColor ) ) Save();
                if( ImGui.ColorEdit4( "Literal Color", ref LuaLiteralColor ) ) Save();
                if( ImGui.ColorEdit4( "Variable Color", ref LuaVariableColor ) ) Save();
            }
        }

        private void DrawFileBrowser() {
            using var child = ImRaii.Child( "FileBrowser" );

            if( ImGui.Checkbox( "Preview Images", ref FileBrowserImagePreview ) ) Save();
            if( ImGui.Checkbox( "Skip File Overwriting Confirmation", ref FileBrowserOverwriteDontAsk ) ) Save();

            if( ImGui.ColorEdit4( "Selected Color", ref FileBrowserSelectedColor ) ) Save();
            if( ImGui.ColorEdit4( "Folder Color", ref FileBrowserFolderColor ) ) Save();
            if( ImGui.ColorEdit4( "Code File Color", ref FileBrowserCodeColor ) ) Save();
            if( ImGui.ColorEdit4( "Misc File Color", ref FileBrowserMiscColor ) ) Save();
            if( ImGui.ColorEdit4( "Image File Color", ref FileBrowserImageColor ) ) Save();
            if( ImGui.ColorEdit4( "Archive File Color", ref FileBrowserArchiveColor ) ) Save();
            if( ImGui.ColorEdit4( "FFXIV File Color", ref FileBrowserFfxivColor ) ) Save();
        }

        private void DrawEditorSpecific() {
            using var child = ImRaii.Child( "EditorSpecific" );

            foreach( var config in ManagerConfigs ) {
                using var _ = ImRaii.PushId( config.Key );

                if( ImGui.CollapsingHeader( config.Key ) ) {
                    using var indent = ImRaii.PushIndent( 5f );

                    ImGui.Checkbox( "Use Custom Window Color", ref config.Value.UseCustomWindowColor );
                    if( config.Value.UseCustomWindowColor ) {
                        if( ImGui.ColorEdit4( "Background", ref config.Value.TitleBg ) ) Save();
                        if( ImGui.ColorEdit4( "Active", ref config.Value.TitleBgActive ) ) Save();
                        if( ImGui.ColorEdit4( "Collapsed", ref config.Value.TitleBgCollapsed ) ) Save();
                    }
                }
            }
        }
    }
}