using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27942
    /// </summary>
    public class CswUpdateSchema_RequestItems_Case27942 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 27942; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClass RequestContainerDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerDispenseClass );
            CswNbtMetaDataObjectClass RequestContainerUpdateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerUpdateClass );
            CswNbtMetaDataObjectClass RequestMaterialDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );

            #region Delete the old Request NTs

            foreach( CswNbtMetaDataNodeType NodeType in RequestOc.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( NodeType );
            }

            #endregion Delete the old Request NTs

            #region Rebuild the Request NT

            string RequestName = "Request";
            CswNbtMetaDataNodeType RequestNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RequestName );
            if( null != RequestNt )
            {
                RequestName = "CISPro Request";
                RequestNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RequestName );
            }
            if( null != RequestNt )
            {
                _CswNbtSchemaModTrnsctn.logError( "Could not create a Node Type for the Request Object Class." );
            }
            else
            {
                RequestNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( RequestOc )
                {
                    NodeTypeName = RequestName,
                    Category = "Requests",
                    IconFileName = NbtIcon.cart

                } );

                CswNbtMetaDataNodeTypeProp RequestNameNtp = RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Name );
                CswNbtMetaDataNodeTypeProp RequestRequestorNtp = RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
                CswNbtMetaDataNodeTypeProp RequestSubmittedNtp = RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                RequestNt.addNameTemplateText( RequestNameNtp.PropName );
                RequestNt.addNameTemplateText( RequestRequestorNtp.PropName );
                RequestNt.addNameTemplateText( RequestSubmittedNtp.PropName );

                CswNbtMetaDataNodeTypeTab RequestItemsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( RequestNt, "Request Items", Int32.MinValue );

                CswNbtMetaDataFieldType GridFt = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid );
                CswNbtMetaDataNodeTypeProp RequestItemsGridNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( RequestNt,
                                                                                                         GridFt,
                                                                                                         "Request Items" )
                    {
                        TabId = RequestItemsTab.TabId
                    } );
                CswNbtView GridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( RequestItemsGridNtp.ViewId );
                GridView.ViewName = "Requested Items";
                GridView.Visibility = NbtViewVisibility.Property;
                GridView.ViewMode = NbtViewRenderingMode.Grid;
                GridView.Category = "Requests";
                GridView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship RequestRel = GridView.AddViewRelationship( RequestNt, false );

                CswNbtMetaDataObjectClassProp RcdRequestOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Request );
                CswNbtMetaDataObjectClassProp RcuRequestOcp = RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Request );
                CswNbtMetaDataObjectClassProp RmdRequestOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Request );

                CswNbtViewRelationship RequestItemRel = GridView.AddViewRelationship( RequestRel,
                                                                                      NbtViewPropOwnerType.Second,
                                                                                      RcdRequestOcp, false );
                GridView.AddViewRelationship( RequestRel,
                                              NbtViewPropOwnerType.Second,
                                              RcuRequestOcp, false );
                GridView.AddViewRelationship( RequestRel,
                                              NbtViewPropOwnerType.Second,
                                              RmdRequestOcp, false );



                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Number ) );
                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Name ) );
                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.AssignedTo ) );
                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.NeededBy ) );
                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.RequestedFor ) );
                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.InventoryGroup ) );
                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Location ) );

                GridView.save();

                string MyRequestViewName = "My Request History";
                bool UniqueView = _CswNbtSchemaModTrnsctn.restoreViews( MyRequestViewName ).Count == 0;
                if( false == UniqueView )
                {
                    MyRequestViewName = "My CISPro Request History";
                    UniqueView = _CswNbtSchemaModTrnsctn.restoreViews( MyRequestViewName ).Count == 0;
                }
                if( false == UniqueView )
                {
                    _CswNbtSchemaModTrnsctn.logError( "Could not create a unique Request History view." );
                }
                else
                {
                    CswNbtView MyRequestsView = _CswNbtSchemaModTrnsctn.makeNewView( MyRequestViewName, NbtViewVisibility.Global );
                    MyRequestsView.Category = "Requests";
                    MyRequestsView.ViewMode = NbtViewRenderingMode.Tree;
                    CswNbtViewRelationship RequestVr = MyRequestsView.AddViewRelationship( RequestNt, true );
                    MyRequestsView.AddViewPropertyAndFilter( RequestVr, RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate ), FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotNull );
                    MyRequestsView.AddViewPropertyAndFilter( RequestVr, RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ), FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, Value: "me" );
                    MyRequestsView.save();
                }

            }

            #endregion Rebuild the Request NT

            #region Build the new Request Item NTs

            CswNbtMetaDataNodeType RequestCdNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( NbtObjectClass.RequestContainerDispenseClass, "Request Container Dispense", "Requests" );
            CswNbtMetaDataNodeType RequestCuNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( NbtObjectClass.RequestContainerUpdateClass, "Request Container Update", "Requests" );
            CswNbtMetaDataNodeType RequestMdNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( NbtObjectClass.RequestMaterialDispenseClass, "Request Material Dispense", "Requests" );

            #endregion Build the new Request Item NTs

            #region Permissions

            //Grab permissions from Case 25815

            CswNbtObjClassRole CisproAdmin = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            if( null != CisproAdmin )
            {
                _setCISProPermissions( CisproAdmin, FullPermissions );
            }

            CswNbtObjClassRole CisproReceive = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Receiver" );
            if( null != CisproReceive )
            {
                _setCISProPermissions( CisproReceive, FullPermissions );
            }

            CswNbtObjClassRole CisproGeneral = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_General" );
            if( null != CisproGeneral )
            {
                _setCISProPermissions( CisproGeneral, ViewPermissions );
            }

            CswNbtObjClassRole CisproFulfiller = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Request_Fulfiller" );
            if( null != CisproFulfiller )
            {
                _setCISProPermissions( CisproFulfiller, ViewPermissions );
            }

            CswNbtObjClassRole CisproView = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_View_Only" );
            if( null != CisproView )
            {
                _setCISProPermissions( CisproView, ViewPermissions );
            }

            #endregion Permissions
        }

        private static CswNbtPermit.NodeTypePermission[] ViewPermissions = { CswNbtPermit.NodeTypePermission.View };
        private static CswNbtPermit.NodeTypePermission[] FullPermissions = { 
            CswNbtPermit.NodeTypePermission.View, 
            CswNbtPermit.NodeTypePermission.Create, 
            CswNbtPermit.NodeTypePermission.Edit, 
            CswNbtPermit.NodeTypePermission.Delete 
        };

        private void _setCISProPermissions( CswNbtObjClassRole Role, CswNbtPermit.NodeTypePermission[] Permissions )
        {
            _setNodeTypePermissions( Role, NbtObjectClass.RequestClass, Permissions );
            _setNodeTypePermissions( Role, NbtObjectClass.RequestContainerDispenseClass, Permissions );
            _setNodeTypePermissions( Role, NbtObjectClass.RequestContainerUpdateClass, Permissions );
            _setNodeTypePermissions( Role, NbtObjectClass.RequestMaterialDispenseClass, Permissions );
        }

        private void _setNodeTypePermissions( CswNbtObjClassRole Role, NbtObjectClass ObjClassName, CswNbtPermit.NodeTypePermission[] Permissions )
        {
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClassName );
            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( Permissions, NodeType, Role, true );
            }
        }

        //Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema