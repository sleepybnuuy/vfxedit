using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace VfxEditor.Select2.Vfx.JournalCutscene {
    public class JournalCutsceneRow {
        public string Name;
        public int RowId;
        public readonly List<string> Paths = new();

        public JournalCutsceneRow( CompleteJournal journal ) {
            RowId = ( int )journal.RowId;
            Name = journal.Name.ToString();

            foreach( var cutscene in journal.Cutscene ) {
                var path = cutscene.Value?.Path.ToString();
                if( !string.IsNullOrEmpty( path ) ) Paths.Add( $"cut/{path}.cutb" );
            }
        }
    }
}
