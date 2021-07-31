using AVFXLib.AVFX;
using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class LiteralEnum<T> : LiteralBase {
        public T Value { get; set; }
        public string[] Options = Enum.GetNames( typeof( T ) );

        public LiteralEnum( string avfxName, int size = 4 ) : base( avfxName, size ) {
        }

        public override void Read( AVFXNode node ) {
        }

        public override void Read( AVFXLeaf leaf ) {
            var intValue = Util.Bytes4ToInt( leaf.Contents );
            //if (intValue != -1) // means none
            //{
            Value = ( T )( object )intValue;
            //}

            Size = leaf.Size;
            Assigned = true;
        }

        public void GiveValue( string value ) {
            var enumValue = ( T )Enum.Parse( typeof( T ), value, true );
            GiveValue( enumValue );
        }
        public void GiveValue( T value ) {
            Value = value;
            Assigned = true;
        }

        public override void ToDefault() {
            GiveValue( ( T )( object )0 );
        }


        public override AVFXNode ToAVFX() {
            var enumValue = -1;
            if( Value != null ) {
                enumValue = ( int )( object )Value;
            }
            return new AVFXLeaf( AVFXName, Size, Util.IntTo4Bytes( enumValue ) );
        }

        public override string StringValue() {
            return Value.ToString();
        }
    }
}
