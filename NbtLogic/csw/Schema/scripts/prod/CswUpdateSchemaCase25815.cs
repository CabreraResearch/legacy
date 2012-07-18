using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25815
    /// </summary>
    public class CswUpdateSchemaCase25815 : CswUpdateSchemaTo
    {
        private static CswNbtPermit.NodeTypePermission[] ViewPermissions = { CswNbtPermit.NodeTypePermission.View };
        private static CswNbtPermit.NodeTypePermission[] FullPermissions = { 
            CswNbtPermit.NodeTypePermission.View, 
            CswNbtPermit.NodeTypePermission.Create, 
            CswNbtPermit.NodeTypePermission.Edit, 
            CswNbtPermit.NodeTypePermission.Delete 
        };

        private CswNbtObjClassInventoryGroup InventoryGroupNode = null;

        public override void update()
        {
            _createInventoryGroupNode();
            CswNbtMetaDataNodeType RoleNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" );
            if( RoleNodeType != null )
            {
                _createAdminRole( RoleNodeType );
                _createReceiverRole( RoleNodeType );
                _createGeneralRole( RoleNodeType );
                _createFulfillerRole( RoleNodeType );
                _createViewOnlyRole( RoleNodeType );
            }

        }//Update()

        private void _createInventoryGroupNode()
        {
            CswNbtMetaDataObjectClass InvGrpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataNodeType InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group" );
            if( InventoryGroupNT != null && InventoryGroupNT.ObjectClassId == InvGrpOC.ObjectClassId )
            {
                InventoryGroupNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( InventoryGroupNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                InventoryGroupNode.Name.Text = "CISPro";
                InventoryGroupNode.postChanges( false );
            }
        }

        private void _createAdminRole( CswNbtMetaDataNodeType RoleNodeType )
        {
            CswNbtObjClassRole AdminNode = _createRole( RoleNodeType, "CISPro_Admin" );
            AdminNode.Administrator.Checked = Tristate.True;

            _setGenericPermissions( AdminNode, FullPermissions );
            _setCISProPermissions( AdminNode, FullPermissions );

            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Create_Material, AdminNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DispenseContainer, AdminNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DisposeContainer, AdminNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Receiving, AdminNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Submit_Request, AdminNode, true );
            //_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Fulfill_Request, AdminNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.UndisposeContainer, AdminNode, true );

            AdminNode.postChanges( false );
            _createInventoryGroupPermission( AdminNode, Tristate.True, Tristate.True );
        }

        private void _createReceiverRole( CswNbtMetaDataNodeType RoleNodeType )
        {
            CswNbtObjClassRole ReceiverNode = _createRole( RoleNodeType, "CISPro_Receiver" );

            _setGenericPermissions( ReceiverNode, ViewPermissions );
            _setCISProPermissions( ReceiverNode, ViewPermissions );
            _setNodeTypePermissions( ReceiverNode, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, FullPermissions );
            _setNodeTypePermissions( ReceiverNode, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass, FullPermissions );
            _setNodeTypePermissions( ReceiverNode, CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass, FullPermissions );
            _setNodeTypePermissions( ReceiverNode, CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass, FullPermissions );

            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DispenseContainer, ReceiverNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DisposeContainer, ReceiverNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Receiving, ReceiverNode, true );

            ReceiverNode.postChanges( false );
            _createInventoryGroupPermission( ReceiverNode, Tristate.False, Tristate.True );
        }

        private void _createGeneralRole( CswNbtMetaDataNodeType RoleNodeType )
        {
            CswNbtObjClassRole GeneralNode = _createRole( RoleNodeType, "CISPro_General" );

            _setGenericPermissions( GeneralNode, ViewPermissions );
            _setCISProPermissions( GeneralNode, ViewPermissions );
            _setNodeTypePermissions( GeneralNode, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass, FullPermissions );

            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DispenseContainer, GeneralNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DisposeContainer, GeneralNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Submit_Request, GeneralNode, true );

            GeneralNode.postChanges( false );
            _createInventoryGroupPermission( GeneralNode, Tristate.True, Tristate.False );
        }

        private void _createFulfillerRole( CswNbtMetaDataNodeType RoleNodeType )
        {
            CswNbtObjClassRole FulfillerNode = _createRole( RoleNodeType, "CISPro_Request_Fulfiller" );

            _setGenericPermissions( FulfillerNode, ViewPermissions );
            _setCISProPermissions( FulfillerNode, ViewPermissions );
            _setNodeTypePermissions( FulfillerNode, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass, FullPermissions );

            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DispenseContainer, FulfillerNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DisposeContainer, FulfillerNode, true );
            _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Submit_Request, FulfillerNode, true );
            //_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Fulfill_Request, FulfillerNode, true );

            FulfillerNode.postChanges( false );
            _createInventoryGroupPermission( FulfillerNode, Tristate.True, Tristate.False );
        }

        private void _createViewOnlyRole( CswNbtMetaDataNodeType RoleNodeType )
        {
            CswNbtObjClassRole ViewOnlyNode = _createRole( RoleNodeType, "CISPro_View_Only" );

            _setGenericPermissions( ViewOnlyNode, ViewPermissions );
            _setCISProPermissions( ViewOnlyNode, ViewPermissions );

            ViewOnlyNode.postChanges( false );
            _createInventoryGroupPermission( ViewOnlyNode, Tristate.False, Tristate.False );
        }

        private CswNbtObjClassRole _createRole( CswNbtMetaDataNodeType RoleNodeType, string RoleName )
        {
            CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( RoleNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassRole NodeAsRole = RoleNode;
            NodeAsRole.Name.Text = RoleName;
            NodeAsRole.Timeout.Value = 30;
            return NodeAsRole;
        }

        private void _setGenericPermissions( CswNbtObjClassRole Role, CswNbtPermit.NodeTypePermission[] Permissions )
        {
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.FeedbackClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.UserClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass, Permissions );
        }

        private void _setCISProPermissions( CswNbtObjClassRole Role, CswNbtPermit.NodeTypePermission[] Permissions )
        {
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.InventoryLevelClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass, Permissions );
            _setNodeTypePermissions( Role, CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass, Permissions );
        }

        private void _setNodeTypePermissions( CswNbtObjClassRole Role, CswNbtMetaDataObjectClass.NbtObjectClass ObjClassName, CswNbtPermit.NodeTypePermission[] Permissions )
        {
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClassName );
            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( Permissions, NodeType, Role, true );
            }
        }

        private void _createInventoryGroupPermission( CswNbtObjClassRole Role, Tristate Request, Tristate Receive )
        {
            CswNbtMetaDataNodeType InventoryGroupPermissionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group Permission" );
            if( InventoryGroupPermissionNT != null && InventoryGroupNode != null )
            {
                CswNbtObjClassInventoryGroupPermission InvGrpPermNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( InventoryGroupPermissionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

                InvGrpPermNode.InventoryGroup.RelatedNodeId = InventoryGroupNode.NodeId;
                InvGrpPermNode.Role.RelatedNodeId = Role.NodeId;
                //WorkUnit?
                InvGrpPermNode.View.Checked = Tristate.True;
                InvGrpPermNode.Edit.Checked = CswConvert.ToTristate( Role.Name.Text != "CISPro_View_Only" );
                InvGrpPermNode.Dispense.Checked = CswConvert.ToTristate( Role.Name.Text != "CISPro_View_Only" );
                InvGrpPermNode.Request.Checked = Request;
                InvGrpPermNode.Dispose.Checked = CswConvert.ToTristate( Role.Name.Text != "CISPro_View_Only" );
                InvGrpPermNode.Undispose.Checked = CswConvert.ToTristate( Role.Name.Text == "CISPro_Admin" );
                //InvGrpPermNode.Receive.Checked = Receive;

                InvGrpPermNode.postChanges( false );
            }
        }

    }//class CswUpdateSchemaCase25815

}//namespace ChemSW.Nbt.Schema