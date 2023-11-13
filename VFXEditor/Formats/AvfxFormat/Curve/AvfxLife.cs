using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxLife : AvfxOptional {
        public bool Enabled = true;
        // Life is kinda strange, can either be -1 (4 bytes = ffffffff) or Val + ValR + RanT

        public readonly AvfxFloat Value = new( "Value", "Val", value: -1 );
        public readonly AvfxFloat ValRandom = new( "Random Value", "ValR" );
        public readonly AvfxEnum<RandomType> ValRandomType = new( "Random Type", "Type" );

        private readonly List<AvfxBase> Parsed;
        private readonly List<IUiItem> Display;

        public AvfxLife() : base( "Life" ) {
            Parsed = new() {
                Value,
                ValRandom,
                ValRandomType
            };

            Display = new() {
                Value,
                ValRandom,
                ValRandomType
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            if( size > 4 ) {
                Enabled = true;
                ReadNested( reader, Parsed, size );
                return;
            }
            Enabled = false;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        public override void WriteContents( BinaryWriter writer ) {
            if( !Enabled ) {
                writer.Write( -1 );
                return;
            }
            WriteNested( writer, Parsed );
        }

        public override void DrawUnassigned() {
            using var _ = ImRaii.PushId( "Life" );

            AssignedCopyPaste( GetDefaultText() );
            DrawAssignButton( GetDefaultText(), true );
        }

        public override void DrawAssigned() {
            using var _ = ImRaii.PushId( "Life" );

            AssignedCopyPaste( GetDefaultText() );
            DrawItems( Display );
        }

        public override string GetDefaultText() => "Life";
    }
}
