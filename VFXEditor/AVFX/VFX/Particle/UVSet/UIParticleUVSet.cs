using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UIParticleUVSet : UIItem {
        public AVFXParticleUVSet UVSet;
        public UIParticle Particle;
        public UICurve2Axis Scale;
        public UICurve2Axis Scroll;
        public UICurve Rotation;
        private readonly List<UIItem> Curves = new();
        private readonly List<IUIBase> Parameters;

        public UIParticleUVSet( AVFXParticleUVSet uvSet, UIParticle particle ) {
            UVSet = uvSet;
            Particle = particle;

            Parameters = new List<IUIBase> {
                new UICombo<TextureCalculateUV>( "Calculate UV", UVSet.CalculateUVType )
            };

            Curves.Add( Scale = new UICurve2Axis( UVSet.Scale, "Scale" ) );
            Curves.Add( Scroll = new UICurve2Axis( UVSet.Scroll, "Scroll" ) );
            Curves.Add( Rotation = new UICurve( UVSet.Rot, "Rotation" ) );
            Curves.Add( new UICurve( UVSet.RotRandom, "Rotation Random" ) );
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/UV";
            IUIBase.DrawList( Parameters, id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Curves, parentId );
        }

        public override string GetDefaultText() => $"UV {Idx}";
    }
}