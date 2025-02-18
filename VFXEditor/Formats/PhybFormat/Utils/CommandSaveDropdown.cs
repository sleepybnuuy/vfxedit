using System;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;
using VfxEditor.PhybFormat.Simulator;
using VfxEditor.PhybFormat.Simulator.Chain;
using ImGuiNET;

namespace VfxEditor.Ui.Components {
    public class CommandSaveDropdown<T> : CommandDropdown<T> where T : class, IUiItem {

        public CommandSaveDropdown(
            string id,
            List<T> items,
            Func<T, int, string> getTextAction,
            Func<T> newAction,
            Action<T, bool> onChangeAction = null
        ) : base( id, items, getTextAction, newAction, onChangeAction ) {

        }

        protected override void DrawNewDeleteControls( Action onNew, Action<T> onDelete ) {
            if( onNew != null ) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) onNew();
            }

            using var disabled = ImRaii.Disabled( Selected == null );

            if( typeof(T) == typeof(PhybSimulator) || typeof(T) == typeof(PhybChain) ) {
                // coerce selected to export type
                // TODO add onto IPhysicsObject instead?
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) && Items.Contains( Selected ) ) {
                    var coerced = Selected as PhybSimulator;
                    if (coerced != null) {
                        coerced.SaveDialog();
                    } else {
                        var coerced_chain = Selected as PhybChain;
                        coerced_chain.SaveDialog();
                    }
                }
            }

            if( onDelete != null ) {
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 4 );
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) && Items.Contains( Selected ) ) {
                    onDelete( Selected );
                    Selected = null;
                }
            }
        }
    }
}
