using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class UITextureNormal : UIItem {
        public AVFXTextureNormal Tex;
        public UIParticle Particle;
        public string Name;
        //============================
        public UINodeSelect<UITexture> TextureSelect;
        public List<UIItem> Tabs;
        public UIParameters Parameters;

        public UITextureNormal( AVFXTextureNormal tex, UIParticle particle ) {
            Tex = tex;
            Particle = particle;
            Init();
        }
        public override void Init() {
            base.Init();
            if( !Tex.Assigned ) { Assigned = false; return; }
            //====================
            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) )
            };

            Parameters.Add( TextureSelect = new UINodeSelect<UITexture>( Particle, "Texture", Particle.Main.Textures, Tex.TextureIdx ) );
            Parameters.Add( new UICheckbox( "Enabled", Tex.Enabled ) );
            Parameters.Add( new UIInt( "UV Set Index", Tex.UvSetIdx ) );
            Parameters.Add( new UICombo<TextureFilterType>( "Texture Filter", Tex.TextureFilter ) );
            Parameters.Add( new UICombo<TextureBorderType>( "Texture Border U", Tex.TextureBorderU ) );
            Parameters.Add( new UICombo<TextureBorderType>( "Texture Border V", Tex.TextureBorderV ) );

            Tabs.Add( new UICurve( Tex.NPow, "Power" ) );
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Normal" + parentId ) ) {
                Tex.ToDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId ) {
            var id = parentId + "/TN";
            if( UiHelper.RemoveButton( "Delete Texture Normal" + id, small: true ) ) {
                Tex.Assigned = false;
                TextureSelect.DeleteSelect();
                Init();
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() {
            return "Texture Normal";
        }
    }
}