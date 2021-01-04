using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataPolygon : UIBase
    {
        public AVFXParticleDataPolygon Data;
        //==========================

        public UIParticleDataPolygon(AVFXParticleDataPolygon data)
        {
            Data = data;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //=======================
            Attributes.Add(new UICurve(Data.Count, "count"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawAttrs( id );
        }
    }
}