using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataDecalRing : AvfxData {
        public readonly AvfxCurve Width = new( "Width", "WID" );
        public readonly AvfxFloat ScalingScale = new( "Scaling Scale", "SS" );
        public readonly AvfxFloat RingFan = new( "Ring Fan", "RF" );

        public readonly UiParameters Display;

        public AvfxParticleDataDecalRing() : base() {
            Parsed = new() {
                Width,
                ScalingScale,
                RingFan
            };

            DisplayTabs.Add( Display = new UiParameters( "Parameters" ) );
            Display.Add( ScalingScale );
            Display.Add( RingFan );
            DisplayTabs.Add( Width );
        }
    }
}