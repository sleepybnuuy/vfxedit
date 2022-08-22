using System;
using System.Collections.Generic;
using System.Text;

namespace VFXEditor.Select.Rows {
    public class XivArmor : XivItem {
        public XivArmor( Lumina.Excel.GeneratedSheets.Item item ) : base(item) {
            RootPath = "chara/equipment/e" + Ids.PrimaryId.ToString().PadLeft( 4, '0' ) + "/";
            VfxRootPath = RootPath + "vfx/eff/ve";
            ImcPath = RootPath + "e" + Ids.PrimaryId.ToString().PadLeft( 4, '0' ) + ".imc";
            Variant = Ids.PrimaryVar;
        }
    }
}