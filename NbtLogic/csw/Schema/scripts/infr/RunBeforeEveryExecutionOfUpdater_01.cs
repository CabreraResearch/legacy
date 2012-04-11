using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!


            #region case 21203
            
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "sessionlist", "originaluserid" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "sessionlist", "originaluserid", "If admin is impersonating, original admin user pk", false, false, 50 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "sessionlist", "originalusername" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "sessionlist", "originalusername", "If admin is impersonating, original admin username", false, false, 50 );
            }
            
            #endregion case 21203


            #region case 25780

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "jct_nodes_props", "gestaltsearch" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "jct_nodes_props", "gestaltsearch", "Searchable indexable copy of gestalt", false, false, 512 );
            }

            #endregion case 25780










            // this should always be here, and always be last
            // see case 21989
            _CswNbtSchemaModTrnsctn.makeMissingAuditTablesAndColumns();

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


