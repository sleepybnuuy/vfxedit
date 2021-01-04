using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIUtils
    {
        public static bool ViewSelect(string id, string defaultText, ref int Selected, string[] Options)
        {
            bool validSelect = ( Selected >= 0 && Selected < Options.Length );
            var selectedString = validSelect ? Options[Selected] : defaultText;
            if( ImGui.BeginCombo( "Select" + id, selectedString ) )
            {
                for( int i = 0; i < Options.Length; i++ )
                {
                    bool isSelected = ( Selected == i );
                    if( ImGui.Selectable( Options[i] + id, isSelected ) )
                    {
                        Selected = i;
                    }
                    if( isSelected )
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            return validSelect;
        }
        // =======================
        public static bool EnumComboBox(string label, string[] options, ref int choiceIdx)
        {
            bool ret = false;
            if (ImGui.BeginCombo(label, options[choiceIdx]))
            {
                for (int idx = 0; idx < options.Length; idx++)
                {
                    bool is_selected = (choiceIdx == idx);
                    if(ImGui.Selectable(options[idx], is_selected))
                    {
                        choiceIdx = idx;
                        ret = true;
                    }

                    if (is_selected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            return ret;
        }
        // ================================
        public static bool RemoveButton(string label, bool small = false)
        {
            bool ret = false;
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.25f, 0.25f, 1));
            if( small )
            {
                if( ImGui.SmallButton( label ) )
                {
                    ret = true;
                }
            }
            else
            {
                if( ImGui.Button( label ) )
                {
                    ret = true;
                }
            }
            ImGui.PopStyleColor();
            return ret;
        }
    }
}