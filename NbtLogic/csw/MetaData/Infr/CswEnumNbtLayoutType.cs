using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtLayoutType : CswEnum<CswEnumNbtLayoutType>
    {
        private CswEnumNbtLayoutType( String Name ) : base( Name )
        {
        }

        public static IEnumerable<CswEnumNbtLayoutType> _All
        {
            get { return All; }
        }

        public static implicit operator CswEnumNbtLayoutType( string str )
        {
            CswEnumNbtLayoutType ret = Parse( str );
            return ret ?? Unknown;
        }

        public static readonly CswEnumNbtLayoutType Unknown = new CswEnumNbtLayoutType( "Unknown" );
        public static readonly CswEnumNbtLayoutType Add = new CswEnumNbtLayoutType( "Add" );
        public static readonly CswEnumNbtLayoutType Edit = new CswEnumNbtLayoutType( "Edit" );
        public static readonly CswEnumNbtLayoutType Preview = new CswEnumNbtLayoutType( "Preview" );
        public static readonly CswEnumNbtLayoutType Table = new CswEnumNbtLayoutType( "Table" );


        public static CswEnumNbtLayoutType LayoutTypeForEditMode( CswEnumNbtNodeEditMode EditMode )
        {
            CswEnumNbtLayoutType LType = CswEnumNbtLayoutType.Unknown;
            switch( EditMode )
            {
                case CswEnumNbtNodeEditMode.Add:
                    LType = CswEnumNbtLayoutType.Add;
                    break;
                case CswEnumNbtNodeEditMode.Temp:
                    LType = CswEnumNbtLayoutType.Add;
                    break;
                case CswEnumNbtNodeEditMode.Preview:
                    LType = CswEnumNbtLayoutType.Preview;
                    break;
                case CswEnumNbtNodeEditMode.Table:
                    LType = CswEnumNbtLayoutType.Table;
                    break;
                default:
                    LType = CswEnumNbtLayoutType.Edit;
                    break;
            }
            return LType;
        } // LayoutTypeForEditMode()
    
    } // class CswEnumNbtLayoutType
} // namespace ChemSW.Nbt.MetaData
