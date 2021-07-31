using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXTexturePalette : Base {
        public LiteralBool Enabled = new( "bEna" );
        public LiteralEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public LiteralEnum<TextureBorderType> TextureBorder = new( "TBT" );
        public LiteralInt TextureIdx = new( "TxNo" );
        public AVFXCurve Offset = new( "POff" );
        private readonly List<Base> Attributes;

        public AVFXTexturePalette() : base( "TP" ) {
            Attributes = new List<Base>( new Base[]{
                Enabled,
                TextureFilter,
                TextureBorder,
                TextureIdx,
                Offset
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            TextureIdx.GiveValue( -1 );
        }

        public override AVFXNode ToAVFX() {
            var dataAvfx = new AVFXNode( "TP" );
            PutAVFX( dataAvfx, Attributes );
            return dataAvfx;
        }
    }
}
