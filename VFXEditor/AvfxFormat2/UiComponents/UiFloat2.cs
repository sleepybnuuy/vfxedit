using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class UiFloat2 : IUiBase {
        public readonly UiParsedFloat2 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiFloat2( string name, AvfxFloat l1, AvfxFloat l2 ) {
            Literals = new() { l1, l2 };
            Parsed = new( name, l1.Parsed, l2.Parsed );
        }

        public void Draw( string id ) {
            // Unassigned
            if( AvfxBase.DrawAddButton( Literals, Parsed.Name, id ) ) return;

            Parsed.Draw( id, CommandManager.Avfx );

            AvfxBase.DrawRemoveContextMenu( Literals, Parsed.Name, id );
        }
    }
}