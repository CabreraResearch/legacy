using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27942
    /// </summary>
    public class CswUpdateSchema_01U_RequestItems_Case27942 : CswUpdateSchemaTo
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
            CswNbtMetaDataObjectClass RequestMaterialCreateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialCreateClass );
            CswNbtMetaDataObjectClass RequestMaterialDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, RequestOc.ObjectClassId );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, RequestContainerDispenseOc.ObjectClassId );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, RequestContainerUpdateOc.ObjectClassId );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, RequestMaterialCreateOc.ObjectClassId );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, RequestMaterialDispenseOc.ObjectClassId );

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

                foreach( NbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
                {
                    CswNbtMetaDataObjectClass MemberOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( Member );
                    CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                    CswNbtViewRelationship RequestItemRel = GridView.AddViewRelationship( RequestRel,
                                                                                      NbtViewPropOwnerType.Second,
                                                                                      RequestOcp, false );

                    CswNbtViewProperty Vp0 = GridView.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ) );
                    Vp0.Order = 0;
                    CswNbtViewProperty Vp1 = GridView.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ) );
                    Vp1.Order = 1;
                    CswNbtViewProperty Vp2 = GridView.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                    Vp2.Order = 2;
                    CswNbtViewProperty Vp3 = GridView.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.NeededBy ) );
                    Vp3.Order = 3;
                    CswNbtViewProperty Vp4 = GridView.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Location ) );
                    Vp4.Order = 4;
                    CswNbtViewProperty Vp5 = GridView.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.InventoryGroup ) );
                    Vp5.Order = 5;
                    CswNbtViewProperty Vp6 = GridView.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.RequestedFor ) );
                    Vp6.Order = 6;
                }

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
            CswNbtMetaDataNodeType RequestMcNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( NbtObjectClass.RequestMaterialCreateClass, "Request Material Create", "Requests" );
            CswNbtMetaDataNodeType RequestMdNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( NbtObjectClass.RequestMaterialDispenseClass, "Request Material Dispense", "Requests" );

            //Grab name template from Case 27703
            RequestCdNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Number );
            RequestCdNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Description );

            RequestCuNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Number );
            RequestCuNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Description );

            RequestMcNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Number );
            RequestMcNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Description );

            RequestMdNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Number );
            RequestMdNt.addNameTemplateText( CswNbtPropertySetRequestItem.PropertyName.Description );

            //Grab Table/Preview layouts from Case 27071

            #region Request Container Dispense Layout

            //Add Layout: Case 27263
            //Nothing to add to this Layout

            CswNbtMetaDataNodeTypeProp RcdQuantityNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Quantity );
            CswNbtMetaDataNodeTypeProp RcdNeededByNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.NeededBy );
            CswNbtMetaDataNodeTypeProp RcdStatusNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp RcdNumberNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Number );
            CswNbtMetaDataNodeTypeProp RcdOrderNoNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp RcdFulfillNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Fulfill );
            CswNbtMetaDataNodeTypeProp RcdTotalDispNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.TotalDispensed );
            CswNbtMetaDataNodeTypeProp RcdIGroupNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.InventoryGroup );
            CswNbtMetaDataNodeTypeProp RcdMaterialNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp RcdContainerNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Container );
            CswNbtMetaDataNodeTypeProp RcdRequestNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Request );
            CswNbtMetaDataNodeTypeProp RcdDescriptionNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Description );
            CswNbtMetaDataNodeTypeProp RcdRequestedForNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.RequestedFor );
            CswNbtMetaDataNodeTypeProp RcdCommentsNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Comments );
            CswNbtMetaDataNodeTypeProp RcdLocationNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Location );

            //Edit Layout
            CswNbtMetaDataNodeTypeTab RcdNodeTypeTab = RequestCdNt.getFirstNodeTypeTab();
            RcdNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, DisplayRow: 1, DisplayColumn: 1, TabId: RcdNodeTypeTab.TabId );
            RcdDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdNumberNtp, DoMove: true );
            RcdQuantityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdDescriptionNtp, DoMove: true );
            RcdContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdQuantityNtp, DoMove: true );
            RcdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdContainerNtp, DoMove: true );
            RcdNeededByNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdMaterialNtp, DoMove: true );
            RcdLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdNeededByNtp, DoMove: true );
            RcdIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdLocationNtp, DoMove: true );
            RcdRequestedForNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdIGroupNtp, DoMove: true );
            RcdCommentsNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcdRequestedForNtp, DoMove: true );

            //Table Layout
            RcdNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            RcdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 2 );
            RcdRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
            RcdOrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 2 );
            RcdIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 1 );
            RcdStatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
            RcdTotalDispNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 2 );
            RcdFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 1 );

            //Preview Layout
            RcdDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
            RcdContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, RcdDescriptionNtp, true );
            RcdQuantityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, RcdContainerNtp, true );

            #endregion Request Container Dispense Layout

            #region Request Container Update Layout

            //Add Layout: Case 27263
            //Nothing to add to this Layout

            CswNbtMetaDataNodeTypeProp RcuMaterialNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp RcuContainerNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Container );
            CswNbtMetaDataNodeTypeProp RcuTypeNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Type );
            CswNbtMetaDataNodeTypeProp RcuLocationNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp RcuRequestNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Request );
            CswNbtMetaDataNodeTypeProp RcuStatusNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp RcuNumberNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Number );
            CswNbtMetaDataNodeTypeProp RcuOrderNoNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp RcuFulfillNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Fulfill );
            CswNbtMetaDataNodeTypeProp RcuIGroupNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.InventoryGroup );
            CswNbtMetaDataNodeTypeProp RcuDescriptionNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Description );
            CswNbtMetaDataNodeTypeProp RcuNeededByNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.NeededBy );
            CswNbtMetaDataNodeTypeProp RcuRequestedForNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.RequestedFor );
            CswNbtMetaDataNodeTypeProp RcuCommentsNtp = RequestCuNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Comments );

            //Edit Layout
            CswNbtMetaDataNodeTypeTab RcuNodeTypeTab = RequestCuNt.getFirstNodeTypeTab();
            RcuNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, DisplayRow: 1, DisplayColumn: 1, TabId: RcuNodeTypeTab.TabId );
            RcuDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcuNumberNtp, DoMove: true );
            RcuContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcuDescriptionNtp, DoMove: true );
            RcuMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcuContainerNtp, DoMove: true );
            RcuLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcuMaterialNtp, DoMove: true );
            RcuNeededByNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcuLocationNtp, DoMove: true );
            RcuRequestedForNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcuNeededByNtp, DoMove: true );
            RcuCommentsNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, RcuRequestedForNtp, DoMove: true );

            //Table Layout
            RcuTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            RcuNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
            RcuMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 2 );
            RcuRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
            RcuOrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 2 );
            RcuStatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 7, DisplayColumn: 1 );
            RcuLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 8, DisplayColumn: 2 );
            RcuFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 9, DisplayColumn: 1 );

            //Preview Layout
            RcuDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
            RcuContainerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );
            RcuLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 4, DisplayColumn: 2 );

            RcuTypeNtp.removeFromAllLayouts();
            RcuIGroupNtp.removeFromAllLayouts();

            #endregion Request Container Update Layout

            #region Request Material Dispense Layout

            CswNbtMetaDataNodeTypeProp RmdMaterialNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp RmdTypeNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Type );
            CswNbtMetaDataNodeTypeProp RmdRequestNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Request );
            CswNbtMetaDataNodeTypeProp RmdStatusNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp RmdNumberNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Number );
            CswNbtMetaDataNodeTypeProp RmdOrderNoNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp RmdFulfillNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Fulfill );
            CswNbtMetaDataNodeTypeProp RmdIGroupNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.InventoryGroup );
            CswNbtMetaDataNodeTypeProp RmdTotalDispNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.TotalDispensed );
            CswNbtMetaDataNodeTypeProp RmdTotalMoveNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.TotalMoved );
            CswNbtMetaDataNodeTypeProp RmdLevelNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Level );
            CswNbtMetaDataNodeTypeProp RmdIsBatchNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsBatch );
            CswNbtMetaDataNodeTypeProp RmdBatchNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Batch );
            CswNbtMetaDataNodeTypeProp RmdReceiptLotsReceivedNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.ReceiptLotsReceived );
            CswNbtMetaDataNodeTypeProp RmdGoodsReceivedNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.GoodsReceived );
            CswNbtMetaDataNodeTypeProp RmdReceiptLotsToDispenseNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.ReceiptLotToDispense );
            CswNbtMetaDataNodeTypeProp RmdReorderFreqNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.ReorderFrequency );
            CswNbtMetaDataNodeTypeProp RmdNextReorderDateNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate );
            CswNbtMetaDataNodeTypeProp RmdDescriptionNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Description );
            CswNbtMetaDataNodeTypeProp RmdSizeNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Size );
            CswNbtMetaDataNodeTypeProp RmdCountNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Count );
            CswNbtMetaDataNodeTypeProp RmdQuantityNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Quantity );
            CswNbtMetaDataNodeTypeProp RmdPriorityNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Priority );
            CswNbtMetaDataNodeTypeProp RmdNeededByNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NeededBy );
            CswNbtMetaDataNodeTypeProp RmdCommentsNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Comments );
            CswNbtMetaDataNodeTypeProp RmdLocationNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp RmdRequestedForNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RequestedFor );
            CswNbtMetaDataNodeTypeProp RmdReorderNtp = RequestMdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Reorder );

            //Add Layout: Case 27263
            //Nothing to add to this Layout

            //Edit Layout
            CswNbtMetaDataNodeTypeTab RmdNodeTypeTab = RequestMdNt.getFirstNodeTypeTab();
            RmdNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, DisplayRow: 1, DisplayColumn: 1, TabId: RmdNodeTypeTab.TabId );
            RmdDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdNumberNtp );
            RmdCountNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdDescriptionNtp );
            RmdSizeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdCountNtp );
            RmdQuantityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdSizeNtp );
            RmdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdQuantityNtp );
            RmdNeededByNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdMaterialNtp );
            RmdLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdNeededByNtp );
            RmdIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdLocationNtp );
            RmdRequestedForNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdIGroupNtp );
            RmdReorderNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdRequestedForNtp );
            RmdCommentsNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmdReorderNtp );

            //Table layout
            RmdTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            RmdNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
            RmdMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 2 );
            RmdRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
            RmdOrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 2 );
            RmdIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 6, DisplayColumn: 1 );
            RmdStatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 7, DisplayColumn: 1 );
            RmdTotalDispNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 8, DisplayColumn: 2 );
            RmdTotalMoveNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 9, DisplayColumn: 2 );
            RmdFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 9, DisplayColumn: 1 );

            //Preview Layout

            RmdDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
            RmdTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
            RmdCountNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );
            RmdSizeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 4, DisplayColumn: 1 );
            RmdQuantityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 5, DisplayColumn: 1 );

            //Case 27871
            RmdReorderFreqNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            RmdNextReorderDateNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            if( _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                //Identity Tab
                CswNbtMetaDataNodeTypeTab IdentityTab = RequestMdNt.getIdentityTab();
                RmdDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DisplayRow: 1, DisplayColumn: 1, DoMove: false, TabId: IdentityTab.TabId );
                RmdFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DisplayRow: 2, DisplayColumn: 1, DoMove: false, TabId: IdentityTab.TabId );

                //CMG Tab
                CswNbtMetaDataNodeTypeTab CmgTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( RequestMdNt, "Central Material Group" );
                RmdReorderNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId );
                RmdReorderFreqNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId );
                RmdNextReorderDateNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId );
                RmdLevelNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId );
                RmdIsBatchNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId );
                RmdBatchNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId );
                RmdPriorityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId );
                RmdFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, CmgTab.TabId );

                //Receive Tab
                CswNbtMetaDataNodeTypeTab ReceiveTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( RequestMdNt, "Receive" );
                RmdReceiptLotsReceivedNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ReceiveTab.TabId );
                RmdGoodsReceivedNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ReceiveTab.TabId );
                RmdReceiptLotsToDispenseNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ReceiveTab.TabId );
            }
            else
            {
                RmdLevelNtp.removeFromAllLayouts();
                RmdIsBatchNtp.removeFromAllLayouts();
                RmdBatchNtp.removeFromAllLayouts();
                RmdReceiptLotsReceivedNtp.removeFromAllLayouts();
                RmdGoodsReceivedNtp.removeFromAllLayouts();
                RmdReceiptLotsToDispenseNtp.removeFromAllLayouts();
            }

            CswNbtView ReceiptLotGridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( RmdReceiptLotsReceivedNtp.ViewId );
            ReceiptLotGridView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship RootVr = ReceiptLotGridView.AddViewRelationship( RequestMaterialDispenseOc, IncludeDefaultFilters: true );

            CswNbtMetaDataObjectClass ReceiptLotOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );
            CswNbtMetaDataObjectClassProp ReceiptLotRequestOcp = ReceiptLotOc.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.RequestItem );
            CswNbtViewRelationship LotVr = ReceiptLotGridView.AddViewRelationship( RootVr, NbtViewPropOwnerType.Second, ReceiptLotRequestOcp, IncludeDefaultFilters: true );

            ReceiptLotGridView.AddViewProperty( LotVr, ReceiptLotOc.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.ExpirationDate ) );
            ReceiptLotGridView.AddViewProperty( LotVr, ReceiptLotOc.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.InvestigationNotes ) );
            ReceiptLotGridView.AddViewProperty( LotVr, ReceiptLotOc.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.Manufacturer ) );
            ReceiptLotGridView.AddViewProperty( LotVr, ReceiptLotOc.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.UnderInvestigation ) );

            ReceiptLotGridView.save();

            //end Case 27871

            #endregion Request Material Dispense Layout

            #region Request Material Create Layout Case 27871
            CswNbtMetaDataNodeTypeProp RmcMaterialTypeNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialType );
            CswNbtMetaDataNodeType ChemicalNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNt && ChemicalNt.getObjectClass().ObjectClass == NbtObjectClass.MaterialClass )
            {
                RmcMaterialTypeNtp.DefaultValue.AsNodeTypeSelect.SelectedNodeTypeIds.Add( ChemicalNt.NodeTypeId.ToString() );
            }
            CswNbtMetaDataNodeTypeProp RmcDescriptionNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Description );
            CswNbtMetaDataNodeTypeProp RmcMaterialNameNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialTradename );
            CswNbtMetaDataNodeTypeProp RmcSupplierNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialSupplier );
            CswNbtMetaDataNodeTypeProp RmcPartNoNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialPartNo );
            CswNbtMetaDataNodeTypeProp RmcLocationNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp RmcStatusNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp RmcNumberNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Number );
            CswNbtMetaDataNodeTypeProp RmcOrderNoNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp RmcFulfillNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Fulfill );
            CswNbtMetaDataNodeTypeProp RmcIGroupNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.InventoryGroup );
            CswNbtMetaDataNodeTypeProp RmcRequestNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Request );
            CswNbtMetaDataNodeTypeProp RmcRequestorNTP = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Requestor );
            CswNbtMetaDataNodeTypeProp RmcMaterialNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp RmcNeededByNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.NeededBy );
            CswNbtMetaDataNodeTypeProp RmcRequestedForNtp = RequestMcNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.RequestedFor );

            //Add Layout: Case 27263
            RmcLocationNtp.removeFromAllLayouts();

            //Edit Layout
            CswNbtMetaDataNodeTypeTab RmcNodeTypeTab = RequestMcNt.getFirstNodeTypeTab();
            RmcNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, DisplayRow: 1, DisplayColumn: 1, TabId: RmcNodeTypeTab.TabId );
            RmcDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcNumberNtp );
            RmcMaterialTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcNumberNtp );
            RmcMaterialNameNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcMaterialTypeNtp );
            RmcSupplierNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcMaterialNameNtp );
            RmcPartNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcSupplierNtp );
            RmcNeededByNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcPartNoNtp );
            RmcLocationNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcNeededByNtp );
            RmcIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcLocationNtp );
            RmcRequestedForNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, DoMove: true, InsertAfterProp: RmcIGroupNtp );

            //Table Layout
            RmcNumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
            RmcMaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 2 );
            RmcRequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
            RmcOrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 2 );
            RmcIGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 1 );
            RmcStatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
            RmcFulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 1 );

            //Preview Layout
            RmcDescriptionNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
            RmcMaterialNameNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
            RmcSupplierNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );
            RmcPartNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 4, DisplayColumn: 1 );

            #endregion Request Material Create Layout Case 27871

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
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestContainerDispense.Statuses.Completed, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestContainerDispense.Statuses.Cancelled, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestContainerDispense.Statuses.Dispensed, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
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

                Collection<CswNbtMetaDataObjectClass> OcsInThisView = new Collection<CswNbtMetaDataObjectClass>
                {
                    RequestContainerDispenseOc, RequestContainerUpdateOc
                };

                foreach( CswNbtMetaDataObjectClass Oc in OcsInThisView )
                {
                    CswNbtViewRelationship RequestItemRel = GridView.AddViewRelationship( RootRel,
                                                                                    NbtViewPropOwnerType.Second,
                                                                                    Oc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Container ),
                                                                                    false );

                    CswNbtViewRelationship RequestRel = GridView.AddViewRelationship( RequestItemRel,
                                                                                      NbtViewPropOwnerType.First,
                                                                                      Oc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request ), false );

                    CswNbtViewProperty CompletedVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate ) );
                    CompletedVp.Order = 3;
                    CompletedVp.SortBy = true;
                    CompletedVp.SortMethod = NbtViewPropertySortMethod.Descending;

                    CswNbtViewProperty SubmittedVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate ) );
                    SubmittedVp.Order = 2;
                    SubmittedVp.SortMethod = NbtViewPropertySortMethod.Descending;

                    CswNbtViewProperty NameVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Name ) );
                    NameVp.Order = 1;
                    NameVp.SortMethod = NbtViewPropertySortMethod.Descending;

                    CswNbtViewProperty RequestorVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ) );
                    RequestorVp.Order = 4;

                    CswNbtViewProperty TypeVp = GridView.AddViewProperty( RequestItemRel, RequestContainerUpdateOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ) );
                    TypeVp.Order = 5;
                    CswNbtViewProperty NumberVp = GridView.AddViewProperty( RequestItemRel, RequestContainerUpdateOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                    NumberVp.Order = 6;
                    CswNbtViewProperty OrderVp = GridView.AddViewProperty( RequestItemRel, RequestContainerUpdateOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.ExternalOrderNumber ) );
                    OrderVp.Order = 7;
                    CswNbtViewProperty StatusVp = GridView.AddViewProperty( RequestItemRel, RequestContainerUpdateOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ) );
                    StatusVp.Order = 8;
                }

                GridView.save();
            }

            #endregion Container Request Tabs

            #region Size Relationship

            //Grab Size View mutator from Case 27542
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );
            CswNbtMetaDataNodeTypeProp RcdSizeNtp = RequestCdNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Size );

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

            //Nuke the System Views from the DB
            foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( SystemViewName.CISProRequestCart.ToString() ) )
            {
                View.Delete();
            }
            foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( SystemViewName.CISProRequestHistory.ToString() ) )
            {
                View.Delete();
            }

            //Case 27871: Fix ContainerDispenseTransaction's RequestItem relationship view
            CswNbtMetaDataObjectClass CdtOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerDispenseTransactionClass );
            foreach( CswNbtMetaDataNodeType NodeType in CdtOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp RequestItemNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.RequestItem );
                CswNbtView RelationshipView = _CswNbtSchemaModTrnsctn.restoreView( RequestItemNtp.ViewId );
                RelationshipView.Root.ChildRelationships.Clear();
                //It would probably be better to cache the Request on the ContainerDispenseTransaction and scope the relationship view down
                //however, we're going to be assigning the relatednodeid directly and won't need to use this view to generate a picklist (which would be a performance nightmare)
                foreach( NbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
                {
                    CswNbtMetaDataObjectClass MemberOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( Member );
                    RelationshipView.AddViewRelationship( MemberOc, IncludeDefaultFilters: false );
                }
                RelationshipView.save();
            }

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
            _setNodeTypePermissions( Role, NbtObjectClass.RequestMaterialCreateClass, Permissions );
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