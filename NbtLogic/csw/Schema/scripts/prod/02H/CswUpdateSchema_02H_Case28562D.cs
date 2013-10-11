using System.Data;
using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case28562D : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28562; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "D"; }
        }

        public override string Title
        {
            get { return "Remove old HMIS action"; }
        }

        public override void update()
        {
            string HMISActionName = "HMIS_Reporting";

            // Remove from jct_modules_actions
            {
                CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28562D_Jct_Update", "jct_modules_actions" );
                DataTable JctTable = JctUpdate.getTable( "where actionid in (select actionid from actions where actionname = '" + HMISActionName + "')" );
                if( JctTable.Rows.Count > 0 )
                {
                    foreach( DataRow JctRow in JctTable.Rows )
                    {
                        JctRow.Delete();
                    }
                }
            }

            // Remove from actions
            {
                CswTableUpdate ActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28562D_Action_Update", "actions" );
                DataTable ActionTable = ActionUpdate.getTable( "where actionname = '" + HMISActionName + "'" );
                if( ActionTable.Rows.Count > 0 )
                {
                    foreach( DataRow ActionRow in ActionTable.Rows )
                    {
                        ActionRow.Delete();
                    }
                }
            }

            // Fix action permissions
            {
                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                foreach( CswNbtObjClassRole Role in RoleOC.getNodes( false, true ) )
                {
                    if( Role.ActionPermissions.CheckValue( HMISActionName ) )
                    {
                        Role.ActionPermissions.RemoveValue( HMISActionName );
                    }
                    Role.postChanges( false );
                }
            }
        } // update()
    }

}//namespace ChemSW.Nbt.Schema