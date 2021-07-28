using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public abstract class UIDropdownView<T> : UIBase, UINodeView<T> where T : UINode {
        public string Id;
        public string DefaultText;
        public UIMain Main;
        public AVFXBase AVFX;
        public UINodeGroup<T> Group;
        public T Selected = null;
        public string DefaultPath;

        public bool AllowNew;
        public bool AllowDelete;

        public UIDropdownView( UIMain main, AVFXBase avfx, string id, string defaultText, bool allowNew = true, bool allowDelete = true, string defaultPath = "" ) {
            Main = main;
            AVFX = avfx;
            Id = id;
            DefaultText = defaultText;
            AllowNew = allowNew;
            AllowDelete = allowDelete;
            DefaultPath = Path.Combine( Plugin.TemplateLocation, "Files", defaultPath );
        }

        public abstract void OnDelete( T item );
        public abstract byte[] OnExport( T item );
        public virtual void OnSelect( T item ) { }
        public abstract T OnImport( AVFXNode node, bool has_dependencies = false );

        public override void Draw( string parentId = "" ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ViewSelect();

            if(AllowNew ) ImGui.SameLine();
            UINodeView<T>.DrawControls(this, Main, Selected, Group, AllowNew, AllowDelete, Id);

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );

            if( Selected != null ) {
                Selected.DrawBody( Id );
            }
        }

        public void ViewSelect() {
            var selectedString = (Selected != null) ? Selected.GetText() : DefaultText;
            if( ImGui.BeginCombo( Id + "-Select", selectedString ) ) {
                foreach(var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText() + Id, Selected == item ) ) {
                        Selected = item;
                        OnSelect( item );
                    }
                }
                ImGui.EndCombo();
            }
        }

        public void ControlDelete() {
            Selected = null;
        }

        public void ControlCreate() {
            Main.ImportData( DefaultPath );
        }
    }
}
