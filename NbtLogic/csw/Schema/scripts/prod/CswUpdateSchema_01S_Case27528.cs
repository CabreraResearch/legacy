using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27528
    /// </summary>
    public class CswUpdateSchema_01S_Case27528 : CswUpdateSchemaTo
    {
        public override void update()
        {
            bool MakeAdminNode = true;
            bool MakeChemSWAdminNode = true;

            CswNbtObjClassWorkUnit DefaultWorkUnit = _getDefaultWorkUnit();
            CswNbtObjClassInventoryGroup CISProInventoryGroup = _getInventoryGroup( "CISPro" );
            CswNbtObjClassInventoryGroup DefaultInventoryGroup = _getInventoryGroup( "Default Inventory Group" );

            CswNbtObjClassRole AdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            CswNbtObjClassRole ChemSWAdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "chemsw_admin_role" );

            CswNbtMetaDataObjectClass InvGrpPermOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass );
            CswNbtMetaDataNodeType InvGrpPermNt = InvGrpPermOc.FirstNodeType;

            if( null != DefaultInventoryGroup )
            {
                if( null != InvGrpPermNt && null != DefaultWorkUnit )
                {
                    foreach( CswNbtObjClassInventoryGroupPermission InvGrpPermNode in InvGrpPermOc.getNodes( false, false ) )
                    {
                        if( ( null != CISProInventoryGroup && InvGrpPermNode.InventoryGroup.RelatedNodeId == CISProInventoryGroup.NodeId ) ||
                            InvGrpPermNode.InventoryGroup.Gestalt == "CISPro" )
                        {
                            InvGrpPermNode.InventoryGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                            InvGrpPermNode.WorkUnit.RelatedNodeId = DefaultWorkUnit.NodeId;
                            InvGrpPermNode.postChanges( false );
                        }
                        if( null != AdminRole && InvGrpPermNode.Role.RelatedNodeId == AdminRole.NodeId )
                        {
                            MakeAdminNode = false;
                        }
                        else if( null != ChemSWAdminRole && InvGrpPermNode.Role.RelatedNodeId == ChemSWAdminRole.NodeId )
                        {
                            MakeChemSWAdminNode = false;
                        }
                    }

                    if( MakeAdminNode )
                    {
                        _createInventoryGroupPermission( InvGrpPermNt.NodeTypeId, AdminRole.NodeId, DefaultInventoryGroup.NodeId, DefaultWorkUnit.NodeId );
                    }
                    if( MakeChemSWAdminNode )
                    {
                        _createInventoryGroupPermission( InvGrpPermNt.NodeTypeId, ChemSWAdminRole.NodeId, DefaultInventoryGroup.NodeId, DefaultWorkUnit.NodeId );
                    }
                }

                CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                CswNbtMetaDataNodeType LocationNt = LocationOc.FirstNodeType;

                if( null != CISProInventoryGroup && null != LocationNt )
                {
                    foreach( CswNbtObjClassLocation LocationNode in InvGrpPermOc.getNodes( false, false ) )
                    {
                        if( LocationNode.InventoryGroup.RelatedNodeId == CISProInventoryGroup.NodeId )
                        {
                            LocationNode.InventoryGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                            LocationNode.postChanges( false );
                        }
                    }
                    CISProInventoryGroup.Node.delete();
                }
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27528; }
        }

        //Update()

        private CswNbtObjClassWorkUnit _getDefaultWorkUnit()
        {
            CswNbtObjClassWorkUnit DefaultWorkUnit = null;
            CswNbtMetaDataObjectClass WorkUnitOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass );
            CswNbtMetaDataNodeType WorkUnitNt = WorkUnitOc.FirstNodeType;
            if( null != WorkUnitNt )
            {
                foreach( CswNbtObjClassWorkUnit WorkUnitNode in WorkUnitNt.getNodes( false, false ) )
                {
                    if( "Default Work Unit" == WorkUnitNode.Name.Text )
                    {
                        DefaultWorkUnit = WorkUnitNode;
                    }
                }
            }
            return DefaultWorkUnit;
        }

        private CswNbtObjClassInventoryGroup _getInventoryGroup( string Name )
        {
            CswNbtObjClassInventoryGroup DefaultInventoryGroup = null;
            CswNbtMetaDataObjectClass InvGrpOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataNodeType InvGrpNt = InvGrpOc.FirstNodeType;
            if( null != InvGrpNt )
            {
                foreach( CswNbtObjClassInventoryGroup InventoryGroupNode in InvGrpNt.getNodes( false, false ) )
                {
                    if( Name == InventoryGroupNode.Name.Text )
                    {
                        DefaultInventoryGroup = InventoryGroupNode;
                    }
                }
            }
            return DefaultInventoryGroup;
        }

        private void _createInventoryGroupPermission( int NodeTypeId, CswPrimaryKey RoleId, CswPrimaryKey InvGroupId, CswPrimaryKey WorkUnitId )
        {
            CswNbtObjClassInventoryGroupPermission InvGrpPermNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            InvGrpPermNode.IsDemo = true;
            InvGrpPermNode.InventoryGroup.RelatedNodeId = InvGroupId;
            InvGrpPermNode.Role.RelatedNodeId = RoleId;
            InvGrpPermNode.WorkUnit.RelatedNodeId = WorkUnitId;
            InvGrpPermNode.View.Checked = Tristate.True;
            InvGrpPermNode.Edit.Checked = Tristate.True;
            InvGrpPermNode.Dispense.Checked = Tristate.True;
            InvGrpPermNode.Request.Checked = Tristate.True;
            InvGrpPermNode.Dispose.Checked = Tristate.True;
            InvGrpPermNode.Undispose.Checked = Tristate.True;

            InvGrpPermNode.postChanges( false );
        }

    }//class CswUpdateSchemaCase27528

}//namespace ChemSW.Nbt.Schema