using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25580
    /// </summary>
    public class CswUpdateSchemaCase25580 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // admin and chemsw_admin should have permissions to all nodetypes in master

            CswNbtNode AdminNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            CswNbtNode ChemSWAdminNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            CswNbtObjClassRole AdminNodeAsRole = null;
            CswNbtObjClassRole ChemSWAdminNodeAsRole = null;
            if( AdminNode != null )
            {
                AdminNodeAsRole = CswNbtNodeCaster.AsRole( AdminNode );
            }
            if( ChemSWAdminNode != null )
            {
                ChemSWAdminNodeAsRole = CswNbtNodeCaster.AsRole( ChemSWAdminNode );
            }

            CswNbtPermit.NodeTypePermission[] Permissions = new CswNbtPermit.NodeTypePermission[] {
                    CswNbtPermit.NodeTypePermission.Create,
                    CswNbtPermit.NodeTypePermission.Delete,
                    CswNbtPermit.NodeTypePermission.Edit,
                    CswNbtPermit.NodeTypePermission.View 
                };

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
            {
                if( AdminNodeAsRole != null )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( Permissions, NodeType, AdminNodeAsRole, true );
                }
                if( ChemSWAdminNodeAsRole != null )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( Permissions, NodeType, ChemSWAdminNodeAsRole, true );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase25580

}//namespace ChemSW.Nbt.Schema