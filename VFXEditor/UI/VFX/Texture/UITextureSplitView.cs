using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureSplitView : UISplitView<UITexture>
    {
        public UITextureView TextureView;
        public UITextureSplitView( List<UITexture> items, UITextureView texView ) : base( items, true )
        {
            TextureView = texView;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Texture" + id ) )
            {
                OnNew( new UITexture( TextureView.AVFX.addTexture(), TextureView, TextureView._plugin ) );
            }
        }
    }
}
