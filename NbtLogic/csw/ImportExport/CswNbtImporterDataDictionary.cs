using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.ImportExport
{
    public partial class CswNbtImporter
    {
        public string getRemoteDataDictionaryPkColumnName( string TableName, string DataDictionaryLinkName = null )
        {
            string Ret = "";
            DataDictionaryLinkName = DataDictionaryLinkName ?? "";
            if( false == string.IsNullOrEmpty( DataDictionaryLinkName ) && false == DataDictionaryLinkName.StartsWith("@") )
            {
                DataDictionaryLinkName = "@" + DataDictionaryLinkName;
            }
            string PkColumnSql = "select columnname from data_dictionary" + DataDictionaryLinkName + " where tablename = '" + TableName + "' and columntype = 'pk'";
            CswArbitrarySelect PkColumnSelect = _CswNbtResources.makeCswArbitrarySelect( "cafimport_pkcolumn_select", PkColumnSql );
            DataTable PkColumnTable = PkColumnSelect.getTable();
            if( PkColumnTable.Rows.Count > 0 )
            {
                Ret = PkColumnTable.Rows[0]["columnname"].ToString();
            }

            return Ret;
        }

    } // class CswNbtImporter
} // namespace ChemSW.Nbt.ImportExport
