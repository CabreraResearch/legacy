using System;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DBinding
    {
        public string SourceColumnName;
        
        public string ImportDataColumnName
        {
            get { return SafeColName( SourceColumnName ); }
        }

        public static string SafeColName( string ColName )
        {
            string ret = ColName;
            ret = ret.Replace( "'", "" );
            ret = ret.Replace( " ", "" );
            return ret;
        }

        public CswNbtSubField DestSubfield;
        public CswNbtMetaDataNodeTypeProp DestProperty;
        public CswNbtMetaDataNodeType DestNodeType;
        public string Instance;
    }
}
