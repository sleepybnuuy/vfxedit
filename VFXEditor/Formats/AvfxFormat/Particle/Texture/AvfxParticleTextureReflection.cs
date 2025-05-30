using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat.Particle.Texture;
using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleTextureReflection : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxBool UseScreenCopy = new( "Use Screen Copy", "bUSC" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureCalculateColor> TextureCalculateColorType = new( "Calculate Color", "TCCT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo", value: -1 );
        public readonly AvfxCurve1Axis Rate = new( "Rate", "Rate" );
        public readonly AvfxCurve1Axis RPow = new( "Power", "RPow" );

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTextureReflection( AvfxParticle particle ) : base( "TR", particle, locked: true ) {
            InitNodeSelects();
            Display.Add( new TextureNodeSelectDraw( NodeSelects ) );

            Parsed = [
                Enabled,
                UseScreenCopy,
                TextureFilter,
                TextureCalculateColorType,
                TextureIdx,
                Rate,
                RPow
            ];

            Display.Add( Enabled );
            Display.Add( UseScreenCopy );
            Display.Add( TextureFilter );
            Display.Add( TextureCalculateColorType );

            DisplayTabs.Add( Rate );
            DisplayTabs.Add( RPow );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            EnableAllSelectors();
        }

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => "Texture Reflection";

        public override List<AvfxNodeSelect> GetNodeSelects() => [
            new AvfxNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        ];
    }
}
