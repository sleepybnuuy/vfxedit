using System.Numerics;
using ImGuiNET;
using VFXEditor.Data;
using VFXEditor.UI.VFX;

namespace VFXEditor.UI {
    public class DocDialog : GenericDialog {
        public DocDialog() : base( "Documents" ) { }

        public ReplaceDoc SelectedDoc = null;
        public override void OnDraw() {
            var id = "##Doc";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            ImGui.Columns( 2, id + "/Columns", false );

            var idx = 0;
            foreach( var doc in DocumentManager.CurrentDocs ) {
                if( ImGui.Selectable( doc.Source.DisplayString + id + idx, doc == SelectedDoc, ImGuiSelectableFlags.SpanAllColumns ) ) {
                    SelectedDoc = doc;
                }
                if( ImGui.IsItemHovered() ) {
                    ImGui.BeginTooltip();
                    ImGui.Text( "Replace path: " + doc.Replace.Path );
                    ImGui.Text( "Write path: " + doc.WriteLocation );
                    ImGui.EndTooltip();
                }
                idx++;
            }
            ImGui.NextColumn();

            foreach( var doc in DocumentManager.CurrentDocs ) {
                ImGui.Text( doc.Replace.DisplayString );
            }

            ImGui.Columns( 1 );
            ImGui.EndChild();

            if( ImGui.Button( "+ NEW" + id ) ) {
                DocumentManager.Manager.NewDoc();
            }

            if( SelectedDoc != null ) {
                var deleteDisabled = ( DocumentManager.CurrentDocs.Count == 1 );

                ImGui.SameLine( ImGui.GetWindowWidth() - 105 );
                if( ImGui.Button( "Select" + id ) ) {
                    DocumentManager.Manager.SelectDoc( SelectedDoc );
                }
                if( !deleteDisabled ) {
                    ImGui.SameLine( ImGui.GetWindowWidth() - 55 );
                    if( UIUtils.RemoveButton( "Delete" + id ) ) {
                        DocumentManager.Manager.RemoveDoc( SelectedDoc );
                        SelectedDoc = DocumentManager.CurrentActiveDoc;
                    }
                }
            }
        }
    }
}