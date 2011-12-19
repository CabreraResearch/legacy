using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-07
    /// </summary>
    public class CswUpdateSchemaTo01L07 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 07 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 23687

            CswTableUpdate ActionsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_actions_update", "actions" );
            CswCommaDelimitedString DoomedActions = new CswCommaDelimitedString()
                                                        {
                                                            "Import Fire Extinguisher Data",
                                                            "Receiving",
                                                            "Load Mobile Data",
                                                            "View By Location",
                                                            "Assign Tests",
                                                            "Enter Results",
                                                            "Split Samples"
                                                        };
            CswCommaDelimitedString DoomedActionIds = new CswCommaDelimitedString();

            //Collect and delete doomed actions
            DataTable ActionsTable = ActionsUpdate.getTable();
            foreach( DataRow ActionRow in ActionsTable.Rows )
            {
                string Name = CswConvert.ToString( ActionRow["actionname"] );
                if( DoomedActions.Contains( Name ) )
                {
                    DoomedActionIds.Add( CswConvert.ToString( ActionRow["actionid"] ) );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswConvert.ToInt32( ActionRow["actionid"] ), false );
                    ActionRow.Delete();
                }
                else
                {
                    //These aspx pages are doomed but the Actions aren't, dereference them
                    if( Name == "Create Inspection" || Name == "Edit View" )
                    {
                        ActionRow["url"] = "";
                    }
                }
            }

            //Remove permissions on doomed actions
            CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            foreach( CswNbtNode Node in RoleOc.getNodes( true, false ) )
            {
                CswNbtObjClassRole NodeAsRole = CswNbtNodeCaster.AsRole( Node );
                foreach( string DoomedActionId in DoomedActionIds )
                {
                    NodeAsRole.ActionPermissions.RemoveValue( "act_" + DoomedActionId );
                }
                Node.postChanges( true );
            }

            //Delete jct rows
            CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_jct_modules_actions_update", "jct_modules_actions" );
            DataTable JctTable = JctUpdate.getTable( "where actionid in (" + DoomedActionIds.ToString() + ")" );
            foreach( DataRow Row in JctTable.Rows )
            {
                Row.Delete();
            }
            JctUpdate.update( JctTable );

            //Commit action delete
            ActionsUpdate.update( ActionsTable );

            #endregion Case 23687

        }//Update()

    }//class CswUpdateSchemaTo01L07

}//namespace ChemSW.Nbt.Schema


