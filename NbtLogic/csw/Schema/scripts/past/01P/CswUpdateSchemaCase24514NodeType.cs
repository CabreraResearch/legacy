using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514NodeType : CswUpdateSchemaTo
    {
        public override void update()
        {

            #region NodeTypes

            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            string RequestNodeTypeName = "Request";
            CswNbtMetaDataNodeType RequestNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RequestNodeTypeName );

            if( null != RequestNt && RequestNt.ObjectClassId != RequestOc.ObjectClassId )
            {
                RequestNodeTypeName = "CISPro Request";
                RequestNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RequestNodeTypeName );
            }
            if( null != RequestNt && RequestNt.ObjectClassId != RequestOc.ObjectClassId )
            {
                throw new CswDniException( ErrorType.Error, "Could not create a unique Request NodeType", "Request nodetypes named 'Request' and 'CISPro Request' already exist." );
            }
            if( null == RequestNt )
            {
                RequestNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( RequestOc )
                                                                     {
                                                                         Category = "Requests",
                                                                         NodeTypeName = RequestNodeTypeName
                                                                     } );

            }
            CswNbtMetaDataNodeTypeProp NameNtp = RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Name.ToString() );
            CswNbtMetaDataNodeTypeProp UserNtp = RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor.ToString() );
            CswNbtMetaDataNodeTypeProp DateNtp = RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString() );
            RequestNt.NameTemplateValue = "";
            RequestNt.addNameTemplateText( NameNtp.PropName );
            RequestNt.addNameTemplateText( UserNtp.PropName );
            RequestNt.addNameTemplateText( DateNtp.PropName );

            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            string RequestItemNodeTypeName = "Request Item";
            CswNbtMetaDataNodeType RequestItemNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RequestItemNodeTypeName );

            if( null != RequestItemNt && RequestItemNt.ObjectClassId != RequestItemOc.ObjectClassId )
            {
                RequestItemNodeTypeName = "CISPro Request Item";
                RequestNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RequestItemNodeTypeName );
            }
            if( null != RequestItemNt && RequestItemNt.ObjectClassId != RequestItemOc.ObjectClassId )
            {
                throw new CswDniException( ErrorType.Error, "Could not create a unique Request Item NodeType", "Request nodetypes named 'Request Item' and 'CISPro Request Item' already exist." );
            }
            if( null == RequestItemNt )
            {
                RequestItemNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType(new CswNbtWcfMetaDataModel.NodeType(RequestItemOc)
                    {
                        Category = "Requests",
                        NodeTypeName = RequestItemNodeTypeName
                    });
            }
            CswNbtMetaDataNodeTypeProp RequestItemRequestNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
            RequestItemRequestNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), RequestNt.NodeTypeId );

            CswNbtMetaDataNodeTypeProp TypeNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
            CswNbtMetaDataNodeTypeProp StatusNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status );
            CswNbtMetaDataNodeTypeProp ExternalOrderNoNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            CswNbtMetaDataNodeTypeProp QuantityNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Quantity );
            CswNbtMetaDataNodeTypeProp CountNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Count );
            CswNbtMetaDataNodeTypeProp SizeNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Size );
            CswNbtMetaDataNodeTypeProp MaterialNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Material );
            CswNbtMetaDataNodeTypeProp ContainerNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Container );
            CswNbtMetaDataNodeTypeProp LocationNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp NumberNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Number );

            Int32 SequenceId = _CswNbtSchemaModTrnsctn.makeSequence( new CswSequenceName( "Request Items" ), "R", "", 6, 1 );
            NumberNtp.setSequence( SequenceId );

            RequestItemNt.addNameTemplateText( RequestItemRequestNtp.PropName );
            RequestItemNt.addNameTemplateText( NumberNtp.PropName );
            RequestItemNt.addNameTemplateText( TypeNtp.PropName );

            #endregion NodeTypes

            #region Tabs

            CswNbtMetaDataNodeTypeTab RequestItemsNtt = RequestNt.getNodeTypeTab( "Request Items" );
            if( null == RequestItemsNtt )
            {
                RequestItemsNtt = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( RequestNt, "Request Items",
                                                                              RequestNt.getNodeTypeTabIds().Count );
            }
            CswNbtMetaDataFieldType GridFt =
                _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid );
            CswNbtMetaDataNodeTypeProp RequestItemsGridNtp =
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( RequestNt,
                                                                                                     GridFt,
                                                                                                     "Request Items" )
                                                                 {
                                                                     TabId = RequestItemsNtt.TabId
                                                                 } );
            CswNbtView GridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( RequestItemsGridNtp.ViewId );
            GridView.ViewName = "Requested Items";
            GridView.Visibility = NbtViewVisibility.Property;
            GridView.ViewMode = NbtViewRenderingMode.Grid;
            GridView.Category = "Requests";
            GridView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship RequestRel = GridView.AddViewRelationship( RequestNt, false );
            CswNbtViewRelationship RequestItemRel = GridView.AddViewRelationship( RequestRel,
                                                                                 NbtViewPropOwnerType.Second,
                                                                                 RequestItemRequestNtp, false );
            GridView.AddViewProperty( RequestItemRel, NumberNtp );
            GridView.AddViewProperty( RequestItemRel, TypeNtp );
            GridView.AddViewProperty( RequestItemRel, ExternalOrderNoNtp );
            GridView.AddViewProperty( RequestItemRel, StatusNtp );
            GridView.AddViewProperty( RequestItemRel, QuantityNtp );
            GridView.AddViewProperty( RequestItemRel, CountNtp );
            GridView.AddViewProperty( RequestItemRel, SizeNtp );
            GridView.AddViewProperty( RequestItemRel, MaterialNtp );
            GridView.AddViewProperty( RequestItemRel, ContainerNtp );
            GridView.AddViewProperty( RequestItemRel, LocationNtp );
            GridView.save();


            #endregion Tabs

            #region Views

            string MyRequestViewName = "My Request History";
            bool UniqueView = _CswNbtSchemaModTrnsctn.restoreViews( MyRequestViewName ).Count == 0;
            if( false == UniqueView )
            {
                MyRequestViewName = "My CISPro Request History";
                UniqueView = _CswNbtSchemaModTrnsctn.restoreViews( MyRequestViewName ).Count == 0;
            }
            if( UniqueView )
            {
                CswNbtView MyRequestsView = _CswNbtSchemaModTrnsctn.makeView();
                MyRequestsView.makeNew( MyRequestViewName, NbtViewVisibility.Global );
                MyRequestsView.Category = "Requests";
                MyRequestsView.ViewMode = NbtViewRenderingMode.Tree;
                CswNbtViewRelationship RequestVr = MyRequestsView.AddViewRelationship( RequestNt, true );
                MyRequestsView.AddViewPropertyAndFilter( RequestVr, RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString() ), FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotNull );
                MyRequestsView.save();
            }
            #endregion Views

        }//Update()

    }//class CswUpdateSchemaCase24514NodeType

}//namespace ChemSW.Nbt.Schema