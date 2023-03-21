using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VfxEditor.Select2.Shared;

namespace VfxEditor.Select2.Vfx.Gimmick {
    public class GimmickTab : SelectTab<GimmickRow, ParseAvfxFromFile> {
        public GimmickTab( SelectDialog dialog, string name ) : base( dialog, name ) { }

        // ===== LOADING =====

        public override void OnLoad() {
            var territories = Plugin.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) ).ToList();
            var suffixToName = new Dictionary<string, string>();
            foreach( var zone in territories ) {
                suffixToName[zone.Name.ToString()] = zone.PlaceName.Value?.Name.ToString();
            }

            var sheet = Plugin.DataManager.GetExcelSheet<ActionTimeline>().Where( x => x.Key.ToString().Contains( "gimmick" ) );
            foreach( var item in sheet ) {
                Items.Add( new GimmickRow( item, suffixToName ) );
            }
        }

        public override void SelectItem( GimmickRow item, out ParseAvfxFromFile loaded ) => ParseAvfxFromFile.ReadFile( item.TmbPath, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            if( Loaded.VfxExists ) {
                ImGui.Text( "TMB:" );
                ImGui.SameLine();
                SelectTabUtils.DisplayPath( Selected.TmbPath );
                SelectTabUtils.Copy( Selected.TmbPath, id: $"{parentId}/CopyTmb" );

                Dialog.DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameGimmick, Selected.Name, true );
            }
        }

        protected override string GetName( GimmickRow item ) => item.Name;
    }
}
