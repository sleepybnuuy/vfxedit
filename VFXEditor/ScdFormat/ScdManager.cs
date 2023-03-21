using ImGuiNET;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select.ScdSelect;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdManager : FileManagerWindow<ScdDocument, ScdFile, WorkspaceMetaBasic> {
        public static string ConvertWav => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_out.wav" ).Replace( '\\', '/' );
        public static string ConvertOgg => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_out.ogg" ).Replace( '\\', '/' );

        public ScdManager() : base( "Scd Editor", "Scd", "scd", "Scd", "Scd" ) {
            SourceSelect = new ScdSelectDialog( "Scd Select [LOADED]", this, true );
            ReplaceSelect = new ScdSelectDialog( "Scd Select [REPLACED]", this, false );
        }

        protected override ScdDocument GetNewDocument() => new( this, LocalPath );

        protected override ScdDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => 
            new( this, LocalPath, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Source, data.Replace );

        public override void Dispose() {
            base.Dispose();
            CurrentFile?.Dispose();
            ScdUtils.Cleanup();
        }
    }
}
