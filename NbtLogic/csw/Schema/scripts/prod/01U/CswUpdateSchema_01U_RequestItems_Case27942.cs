using System;
using System.Linq;
using ChemSW.Core;
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



                GridView.AddViewProperty( RequestItemRel, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Type ) );
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

            //Grab name template from Case 27703
            RequestCdNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Name );
            RequestCuNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Name );
            RequestMdNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Name );

            //Grab Table/Preview layouts from Case 27071

            #region RequestCdNt Layout

            //Add Layout: Case 27263
            CswNbtMetaDataNodeTypeProp RcdMaterialNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp RcdContainerNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Container );
            CswNbtMetaDataNodeTypeProp RcdLocationNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp RcdRequestNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Request );

            RcdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
            RcdContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RcdMaterialNtp, true );
            RcdLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RcdContainerNtp, true );
            RcdRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RcdLocationNtp, true );

            //Case 27800
            CswNbtMetaDataNodeTypeProp RcdRequestorNTP = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Requestor );
            RcdRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            //Table Layout
            CswNbtMetaDataNodeTypeProp RcdStatusNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp RcdNumberNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Number );
            CswNbtMetaDataNodeTypeProp RcdOrderNoNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp RcdFulfillNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Fulfill );
            CswNbtMetaDataNodeTypeProp RcdIGroupNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.InventoryGroup );
            CswNbtMetaDataNodeTypeProp RcdTotalDispNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.TotalDispensed );

            RcdNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            RcdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 2 );
            RcdRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
            RcdOrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 2 );
            RcdIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 1 );
            RcdStatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
            RcdTotalDispNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 2 );
            RcdFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 1 );

            //Preview Layout
            CswNbtMetaDataNodeTypeProp RcdSizeNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Size );
            CswNbtMetaDataNodeTypeProp RcdQuantityNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Quantity );

            RcdSizeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 2 );
            RcdQuantityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );

            #endregion RequestCdNt Layout

            #region RequestCuNt Layout

            //Add Layout: Case 27263
            CswNbtMetaDataNodeTypeProp RcuMaterialNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp RcuContainerNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Container );
            CswNbtMetaDataNodeTypeProp RcuTypeNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Type );
            CswNbtMetaDataNodeTypeProp RcuLocationNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp RcuRequestNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Request );

            RcuMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
            RcuContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RcuMaterialNtp, true );
            RcuTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RcuContainerNtp, true );
            RcuLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RcuTypeNtp, true );
            RcuRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RcuLocationNtp, true );

            //Case 27800
            CswNbtMetaDataNodeTypeProp RcuRequestorNTP = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Requestor );
            RcuRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            //Table Layout

            CswNbtMetaDataNodeTypeProp RcuStatusNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp RcuNumberNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Number );
            CswNbtMetaDataNodeTypeProp RcuOrderNoNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp RcuFulfillNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Fulfill );
            CswNbtMetaDataNodeTypeProp RcuIGroupNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.InventoryGroup );

            RcuTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            RcuNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
            RcuMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 2 );
            RcuRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
            RcuOrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 2 );
            RcuIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 6, DisplayColumn: 1 );
            RcuStatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 7, DisplayColumn: 1 );
            RcuLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 8, DisplayColumn: 2 );
            RcuFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 9, DisplayColumn: 1 );

            //Preview Layout
            RcuTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
            RcuContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
            RcuLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 2 );

            #endregion RequestCuNt Layout

            #region RequestMdNt Layout

            //Add Layout: Case 27263
            CswNbtMetaDataNodeTypeProp RmdMaterialNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp RmdTypeNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Type );
            CswNbtMetaDataNodeTypeProp RmdLocationNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp RmdRequestNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Request );

            RmdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
            RmdTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RmdMaterialNtp, true );
            RmdLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RmdTypeNtp, true );
            RmdRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, RmdLocationNtp, true );

            //Case 27800
            CswNbtMetaDataNodeTypeProp RmdRequestorNTP = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Requestor );
            RmdRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            //Table layout
            CswNbtMetaDataNodeTypeProp RmdStatusNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp RmdNumberNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Number );
            CswNbtMetaDataNodeTypeProp RmdOrderNoNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp RmdFulfillNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Fulfill );
            CswNbtMetaDataNodeTypeProp RmdIGroupNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.InventoryGroup );
            CswNbtMetaDataNodeTypeProp RmdTotalDispNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.TotalDispensed );

            RmdTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            RmdNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
            RmdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 2 );
            RmdRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
            RmdOrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 2 );
            RmdIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 6, DisplayColumn: 1 );
            RmdStatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 7, DisplayColumn: 1 );
            RmdTotalDispNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 8, DisplayColumn: 2 );
            RmdFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 9, DisplayColumn: 1 );

            //Preview Layout
            CswNbtMetaDataNodeTypeProp RmdSizeNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Size );
            CswNbtMetaDataNodeTypeProp RmdCountNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Count );
            CswNbtMetaDataNodeTypeProp RmdQuantityNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Quantity );

            RmdTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
            RmdCountNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
            RmdSizeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 2 );
            RmdQuantityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );

            #endregion RequestCdNt Layout


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

            #region Views

            //Grab Dispense Requests View from Case 27071

            CswNbtMetaDataObjectClassProp StatusOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Status );
            CswNbtView DispenseRequestsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Dispense Requests: Open" ).FirstOrDefault() ?? _CswNbtSchemaModTrnsctn.makeNewView( "Dispense Requests: Open", NbtViewVisibility.Global );
            DispenseRequestsView.Root.ChildRelationships.Clear();

            DispenseRequestsView.ViewMode = NbtViewRenderingMode.Tree;
            DispenseRequestsView.Category = "Requests";
            CswNbtViewRelationship RequestItemsVr = DispenseRequestsView.AddViewRelationship( RequestContainerDispenseOc, true );

            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.AssignedTo ), "me" );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestItem.Statuses.Completed, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestItem.Statuses.Cancelled, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestItem.Statuses.Dispensed, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.save();

            #endregion Views

            #region Container Request Tabs

            //Grab Tab Request grids from Case 24514

            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab RequestsTab = ContainerNt.getNodeTypeTab( "Requests" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ContainerNt, "Requests", ContainerNt.getNodeTypeTabIds().Count );

                CswNbtMetaDataNodeTypeProp RequestsGridNtp = ContainerNt.getNodeTypeProp( "Submitted Requests" );
                if( null != RequestsGridNtp )
                {
                    if( CswNbtMetaDataFieldType.NbtFieldType.Grid != RequestsGridNtp.getFieldType().FieldType )
                    {
                        RequestsGridNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ContainerNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, "CISPro Submitted Requests", RequestsTab.TabId );
                    }
                }
                else
                {
                    RequestsGridNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ContainerNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Submitted Requests", RequestsTab.TabId );
                }
                CswNbtView GridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( RequestsGridNtp.ViewId );
                GridView.Root.ChildRelationships.Clear();
                GridView.ViewName = ContainerNt.NodeTypeName + " Requested Items";
                GridView.Visibility = NbtViewVisibility.Property;
                GridView.ViewMode = NbtViewRenderingMode.Grid;
                GridView.Category = "Requests";

                CswNbtViewRelationship RootRel = GridView.AddViewRelationship( ContainerNt, false );

                CswNbtViewRelationship RequestItemRel1 = GridView.AddViewRelationship( RootRel,
                                                                                    NbtViewPropOwnerType.Second,
                                                                                    RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Container ),
                                                                                    false );

                CswNbtViewRelationship RequestItemRel2 = GridView.AddViewRelationship( RootRel,
                                              NbtViewPropOwnerType.Second,
                                              RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Container ),
                                              false );

                CswNbtViewRelationship RequestRel1 = GridView.AddViewRelationship( RequestItemRel1,
                                                                                  NbtViewPropOwnerType.First,
                                                                                  RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Request ), false );

                CswNbtViewRelationship RequestRel2 = GridView.AddViewRelationship( RequestItemRel2,
                                                                  NbtViewPropOwnerType.First,
                                                                  RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Request ), false );

                CswNbtViewProperty CompletedVp = GridView.AddViewProperty( RequestRel1, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate ) );
                CompletedVp.Order = 3;
                CompletedVp.SortBy = true;
                CompletedVp.SortMethod = NbtViewPropertySortMethod.Descending;

                CswNbtViewProperty SubmittedVp = GridView.AddViewProperty( RequestRel1, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate ) );
                SubmittedVp.Order = 2;
                SubmittedVp.SortMethod = NbtViewPropertySortMethod.Descending;

                CswNbtViewProperty NameVp = GridView.AddViewProperty( RequestRel1, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Name ) );
                NameVp.Order = 1;
                NameVp.SortMethod = NbtViewPropertySortMethod.Descending;

                CswNbtViewProperty RequestorVp = GridView.AddViewProperty( RequestRel1, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ) );
                RequestorVp.Order = 4;

                CswNbtViewProperty TypeVp = GridView.AddViewProperty( RequestItemRel1, RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Type ) );
                TypeVp.Order = 5;
                CswNbtViewProperty NumberVp = GridView.AddViewProperty( RequestItemRel1, RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Number ) );
                NumberVp.Order = 6;
                CswNbtViewProperty OrderVp = GridView.AddViewProperty( RequestItemRel1, RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.ExternalOrderNumber ) );
                OrderVp.Order = 7;
                CswNbtViewProperty StatusVp = GridView.AddViewProperty( RequestItemRel1, RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Status ) );
                StatusVp.Order = 8;

                GridView.save();

            }

            #endregion Container Request Tabs

            #region Size Relationship

            //Grab Size View mutator from Case 27542
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );

            CswNbtView RcdSizeView = _CswNbtSchemaModTrnsctn.restoreView( RcdSizeNtp.ViewId );
            RcdSizeView.Root.ChildRelationships.Clear();

            CswNbtViewRelationship RcdRequestItemViewRel = RcdSizeView.AddViewRelationship( RequestContainerDispenseOc, false );
            CswNbtViewRelationship RcdMaterialViewRel = RcdSizeView.AddViewRelationship( RcdRequestItemViewRel, NbtViewPropOwnerType.First, RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Material ), true );
            CswNbtViewRelationship RcdSizeViewRel = RcdSizeView.AddViewRelationship( RcdMaterialViewRel, NbtViewPropOwnerType.Second, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material ), true );

            //Case 27438 
            RcdSizeView.AddViewPropertyAndFilter(
                    ParentViewRelationship: RcdSizeViewRel,
                    MetaDataProp: SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable ),
                    Value: Tristate.False.ToString(),
                    SubFieldName: CswNbtSubField.SubFieldName.Checked,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals
                    );

            RcdSizeView.save();

            CswNbtView RmdSizeView = _CswNbtSchemaModTrnsctn.restoreView( RcdSizeNtp.ViewId );
            RmdSizeView.Root.ChildRelationships.Clear();

            CswNbtViewRelationship RmdRequestItemViewRel = RmdSizeView.AddViewRelationship( RequestMaterialDispenseOc, false );
            CswNbtViewRelationship RmdMaterialViewRel = RmdSizeView.AddViewRelationship( RmdRequestItemViewRel, NbtViewPropOwnerType.First, RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Material ), true );
            CswNbtViewRelationship RmdSizeViewRel = RmdSizeView.AddViewRelationship( RmdMaterialViewRel, NbtViewPropOwnerType.Second, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material ), true );

            //Case 27438 
            RmdSizeView.AddViewPropertyAndFilter(
                    ParentViewRelationship: RmdSizeViewRel,
                    MetaDataProp: SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable ),
                    Value: Tristate.False.ToString(),
                    SubFieldName: CswNbtSubField.SubFieldName.Checked,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals
                    );

            RmdSizeView.save();


            #endregion Size Relationship

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