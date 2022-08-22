using VFXEditor.FileManager;
using VFXEditor.Select.TMB;

namespace VFXEditor.TMB {
    public partial class TMBManager : FileManager<TMBDocument, WorkspaceMetaTmb, TMBFile> {
        public static TMBSelectDialog SourceSelect { get; private set; }
        public static TMBSelectDialog ReplaceSelect { get; private set; }

        public static void Setup() {
            SourceSelect = new TMBSelectDialog(
                "Tmb Select [SOURCE]",
                Plugin.Configuration.RecentSelectsTMB,
                true,
                SetSourceGlobal
            );

            ReplaceSelect = new TMBSelectDialog(
                "Tmb Select [TARGET]",
                Plugin.Configuration.RecentSelectsTMB,
                false,
                SetReplaceGlobal
            );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetSource( result );
            Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsTMB, result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetReplace( result );
            Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsTMB, result );
        }

        public static readonly string PenumbraPath = "Tmb";

        public TMBManager() : base( title: "Tmb Editor", id: "Tmb", tempFilePrefix: "TmbTemp", extension: "tmb", penumbaPath: PenumbraPath ) { }

        protected override TMBDocument GetNewDocument() => new( LocalPath );

        protected override TMBDocument GetImportedDocument( string localPath, WorkspaceMetaTmb data ) => new( LocalPath, localPath, data.Source, data.Replace );

        protected override void DrawMenu() {
        }

        public override void Dispose() {
            base.Dispose();
            SourceSelect.Hide();
            ReplaceSelect.Hide();
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            base.DrawBody();
        }
    }
}