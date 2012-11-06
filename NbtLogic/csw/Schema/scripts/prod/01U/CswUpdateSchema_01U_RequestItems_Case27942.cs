using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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
        }

        //Update()

    }//class CswUpdateSchemaCaseXXXXX

}//namespace ChemSW.Nbt.Schema