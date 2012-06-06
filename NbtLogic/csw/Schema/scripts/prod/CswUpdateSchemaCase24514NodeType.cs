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
            RequestNt.addNameTemplateText( NameNtp.PropName );

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
                RequestItemNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( RequestItemOc )
                                                                     {
                                                                         Category = "Requests",
                                                                         NodeTypeName = RequestItemNodeTypeName
                                                                     } );
            }
            CswNbtMetaDataNodeTypeProp RequestItemRequestNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request.ToString() );
            RequestItemRequestNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), RequestNt.NodeTypeId );

            CswNbtMetaDataNodeTypeProp TypeNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type.ToString() );
            CswNbtMetaDataNodeTypeProp StatusNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status.ToString() );
            CswNbtMetaDataNodeTypeProp ExternalOrderNoNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber.ToString() );

            CswNbtMetaDataNodeTypeProp NumberNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Number.ToString() );
            Int32 SequenceId = _CswNbtSchemaModTrnsctn.makeSequence( new CswSequenceName( "Request Items" ), "RI", "", 0, 1 );
            NumberNtp.setSequence( SequenceId );

            RequestItemNt.addNameTemplateText( NumberNtp.PropName );
            RequestItemNt.addNameTemplateText( " " );
            RequestItemNt.addNameTemplateText( RequestItemRequestNtp.PropName );
            RequestItemNt.addNameTemplateText( " " );
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
            GridView.ViewName = "Request Items Grid Property View";
            GridView.Visibility = NbtViewVisibility.Property;
            GridView.ViewMode = NbtViewRenderingMode.Grid;
            GridView.Category = "Requests";
            GridView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship RequestRel = GridView.AddViewRelationship( RequestNt, false );
            CswNbtViewRelationship RequestItemRel = GridView.AddViewRelationship( RequestRel,
                                                                                 NbtViewPropOwnerType.Second,
                                                                                 RequestItemRequestNtp, false );
            GridView.AddViewProperty( RequestItemRel, TypeNtp );
            GridView.AddViewProperty( RequestItemRel, NumberNtp );
            GridView.AddViewProperty( RequestItemRel, ExternalOrderNoNtp );
            GridView.AddViewProperty( RequestItemRel, StatusNtp );
            GridView.save();


            #endregion Tabs

            #region Views

            string MyRequestViewName = "My Requests";
            bool UniqueView = _CswNbtSchemaModTrnsctn.restoreViews( MyRequestViewName ).Count == 0;
            if( false == UniqueView )
            {
                MyRequestViewName = "My CISPro Requests";
                UniqueView = _CswNbtSchemaModTrnsctn.restoreViews( MyRequestViewName ).Count == 0;
            }
            if( UniqueView )
            {
                CswNbtView MyRequestsView = _CswNbtSchemaModTrnsctn.makeView();
                MyRequestsView.makeNew( MyRequestViewName, NbtViewVisibility.Global );
                MyRequestsView.Category = "Requests";
                MyRequestsView.ViewMode = NbtViewRenderingMode.Tree;
                MyRequestsView.AddViewRelationship( RequestNt, true );
                MyRequestsView.save();
            }
            #endregion Views

        }//Update()

    }//class CswUpdateSchemaCase24514NodeType

}//namespace ChemSW.Nbt.Schema