using System.IO;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class ServerPathInstanceObject : SgbObject {
        public ServerPathInstanceObject( LayerEntryType type ) : base( type ) { }

        public ServerPathInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {

        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {

        }
    }
}
