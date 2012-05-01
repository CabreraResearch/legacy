using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25987
    /// </summary>
    public class CswUpdateSchemaCase25987 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswCommaDelimitedString LegacyTables = new CswCommaDelimitedString
                                                       {
                                                           "containers",
                                                           "containers_audit",
                                                           "inventory_groups",
                                                           "inventory_groups_audit",
                                                           "locations",
                                                           "locations_audit",
                                                           "materials",
                                                           "materials_audit",
                                                           "materials_subclass",
                                                           "materials_subclass_audit",
                                                           "materials_synonyms",
                                                           "materials_synonyms_audit",
                                                           "units_of_measure",
                                                           "units_of_measure_audit",
                                                           "users",
                                                           "users_audit",
                                                           "vendors",
                                                           "vendors_audit"
                                                       };
            CswCommaDelimitedString DoomedTables = new CswCommaDelimitedString();
            DataTable AllTablesDt = _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSqlSelect( "25987_all_tables", "select table_name from all_tables where lower(owner)='" + _CswNbtSchemaModTrnsctn.UserName.ToLower() + "'" );
            foreach( DataRow Row in AllTablesDt.Rows )
            {
                string TableName = CswConvert.ToString( Row["table_name"] ).ToLower();
                if( TableName.StartsWith( "copy_" ) ||
                    LegacyTables.Contains( TableName ) )
                {
                    DoomedTables.Add( TableName );
                }
            }
            foreach( string DoomedTable in DoomedTables )
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "drop table " + DoomedTable );
            }
        }//Update()

    }//class CswUpdateSchemaCase25987

}//namespace ChemSW.Nbt.Schema