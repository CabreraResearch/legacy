using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01K-05
    /// </summary>
    public class CswUpdateSchemaTo01K05 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 05 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            // case 24294
            // Remove deprecated Inspection Actions

            CswTableUpdate ActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01K05_actions_update", "actions" );
            DataTable ActionTable = ActionUpdate.getTable( "where actionname = 'Assign Inspection' or actionname = 'Inspection Design' " );
            foreach( DataRow ActionRow in ActionTable.Rows )
            {
                string ActionId = CswConvert.ToString( ActionRow["actionid"] );
                CswTableUpdate JctModulesActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01K05_jctmodulesactions_update", "jct_modules_actions" );
                DataTable JmaTable = JctModulesActionUpdate.getTable( "where actionid = ' " + ActionId + "' " );
                foreach( DataRow JctRow in JmaTable.Rows )
                {
                    JctRow.Delete();
                }
                JctModulesActionUpdate.update( JmaTable );
                ActionRow.Delete();
            }
            ActionUpdate.update( ActionTable );



        }//Update()

    }//class CswUpdateSchemaTo01K05

}//namespace ChemSW.Nbt.Schema


