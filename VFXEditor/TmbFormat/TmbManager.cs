using Dalamud.Logging;
using ImGuiNET;
using VfxEditor.Animation;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Select.TmbSelect;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public partial class TmbManager : FileManagerWindow<TmbDocument, TmbFile, WorkspaceMetaBasic> {
        public TmbManager() : base( "Tmb Editor", "Tmb", "tmb", "Tmb", "Tmb" ) {
            SourceSelect = new TmbSelectDialog( "Tmb Select [LOADED]", this, true );
            ReplaceSelect = new TmbSelectDialog( "Tmb Select [REPLACED]", this, false );
        }

        public override void SetReplace( SelectResult result ) {
            base.SetReplace( result );
            if( ActiveDocument != null ) {
                ActiveDocument.AnimationId = ActorAnimationManager.GetIdFromTmbPath( result.Path );
            }
        }

        protected override TmbDocument GetNewDocument() => new( this, LocalPath );

        protected override TmbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => 
            new( this, LocalPath, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Source, data.Replace );
    }
}
