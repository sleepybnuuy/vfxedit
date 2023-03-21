using System;
using System.Collections.Generic;

namespace VfxEditor.Select2.Vfx.Item {
    public struct ItemIds {
        public ulong Raw;

        public int Id1;
        public int Id2;
        public int Id3;
        public int Id4;

        public int Id => Id1;
        public int GearVariant => Id2;
        public int WeaponBody => Id2;
        public int WeaponVariant => Id3;

        public ItemIds( ulong modelDataRaw ) {
            Raw = modelDataRaw;
            /*
             * Gear: [Id, Var, -, -] / [-,-,-,-]
             * Weapon: [Id, Body, Var, -] / [Id, Body, Var, -]
             */
            var bytes = BitConverter.GetBytes( modelDataRaw );
            Id1 = BitConverter.ToInt16( bytes, 0 );
            Id2 = BitConverter.ToInt16( bytes, 2 );
            Id3 = BitConverter.ToInt16( bytes, 4 );
            Id4 = BitConverter.ToInt16( bytes, 6 );
        }


        public static ulong ToLong( int id1, int id2, int id3, int id4 ) {
            List<byte> bytes = new();
            bytes.AddRange( BitConverter.GetBytes( ( short )id1 ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )id2 ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )id3 ) );
            bytes.AddRange( BitConverter.GetBytes( ( short )id4 ) );
            return ( ulong )BitConverter.ToInt64( bytes.ToArray() );
        }
    }

    public class ItemRow {
        public string Name;
        public ItemIds Ids;
        public ItemIds SecondaryIds;
        public bool HasModel;
        public int Variant;

        public string RootPath;
        public string VfxRootPath;
        public string ImcPath;

        public int RowId;
        public ushort Icon;

        public ItemRow( Lumina.Excel.GeneratedSheets.Item item ) {
            Name = item.Name.ToString();
            RowId = ( int )item.RowId;
            Icon = item.Icon;

            Ids = new ItemIds( item.ModelMain );
            SecondaryIds = new ItemIds( item.ModelSub );
            HasModel = ( Ids.Id1 != 0 );
        }

        public string GetVfxPath( int idx ) => $"{VfxRootPath}{idx.ToString().PadLeft( 4, '0' )}.avfx";
    }
}
