
using System.Linq;
using ChemSW.Config;
using ChemSW.Log;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514RequestButton : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.RequestPropertyName,
                    Extended = CswNbtNodePropButton.ButtonMode.menu,
                    ListOptions = CswNbtObjClassContainer.RequestMenu.Options.ToString(),
                    StaticText = CswNbtObjClassContainer.RequestMenu.Dispense
                } );

            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab RequestsTab = ContainerNt.getNodeTypeTab( "Requests" );
                if( null == RequestsTab )
                {
                    RequestsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ContainerNt, "Requests", ContainerNt.getNodeTypeTabIds().Count );
                }
                CswNbtMetaDataNodeTypeProp DispenseNtp = ContainerNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.RequestPropertyName );
                DispenseNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, RequestsTab.TabId, 1, 1 );
                DispenseNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, RequestsTab.TabId, 1, 1 );

                CswNbtMetaDataNodeTypeProp RequestsGridNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ContainerNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Submitted Requests", RequestsTab.TabId );
                CswNbtView GridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( RequestsGridNtp.ViewId );
                makeRequestGridView( GridView, ContainerNt );
            }

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp MaterialRequestOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                MaterialOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassMaterial.RequestPropertyName
                } );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab IdentityTab = MaterialNt.getNodeTypeTab( "Identity" );
                if( null == IdentityTab )
                {
                    IdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab(MaterialNt, "Identity", MaterialNt.getNodeTypeTabIds().Count);
                }
                CswNbtMetaDataNodeTypeProp RequestBtnNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.RequestPropertyName );
                RequestBtnNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, IdentityTab.TabId );
            }

        }//Update()


        private void makeRequestGridView( CswNbtView GridView, CswNbtMetaDataNodeType RootNt )
        {
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            CswNbtMetaDataNodeType RequestNt = RequestOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null == RequestNt )
            {
                CswStatusMessage Msg = new CswStatusMessage
                {
                    AppType = AppType.SchemUpdt,
                    ContentType = ContentType.Error
                };
                Msg.Attributes.Add( ChemSW.Log.LegalAttribute.exoteric_message, "Could not get a Request NodeType" );
                _CswNbtSchemaModTrnsctn.CswLogger.send( Msg );
            }
            else
            {

                CswNbtMetaDataNodeTypeProp NameNtp =
                    RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Name.ToString() );
                CswNbtMetaDataNodeTypeProp SubmittedDateNtp =
                    RequestNt.getNodeTypePropByObjectClassProp(
                        CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString() );
                CswNbtMetaDataNodeTypeProp CompletedDateNtpNtp =
                    RequestNt.getNodeTypePropByObjectClassProp(
                        CswNbtObjClassRequest.PropertyName.CompletedDate.ToString() );
                CswNbtMetaDataNodeTypeProp RequestorNtp =
                    RequestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor.ToString() );

                CswNbtMetaDataObjectClass RequestItemOc =
                    _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(
                        CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
                CswNbtMetaDataNodeType RequestItemNt = RequestItemOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null == RequestItemNt )
                {
                    CswStatusMessage Msg = new CswStatusMessage
                    {
                        AppType = AppType.SchemUpdt,
                        ContentType = ContentType.Error
                    };
                    Msg.Attributes.Add( ChemSW.Log.LegalAttribute.exoteric_message, "Could not get a Request Item NodeType" );
                    _CswNbtSchemaModTrnsctn.CswLogger.send( Msg );

                }
                else
                {
                    CswNbtMetaDataNodeTypeProp TypeNtp =
                        RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
                    CswNbtMetaDataNodeTypeProp StatusNtp =
                        RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status );
                    CswNbtMetaDataNodeTypeProp ExternalOrderNoNtp =
                        RequestItemNt.getNodeTypePropByObjectClassProp(
                            CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
                    CswNbtMetaDataNodeTypeProp NumberNtp =
                        RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Number );

                    GridView.Root.ChildRelationships.Clear();
                    GridView.ViewName = RootNt.NodeTypeName + " Requested Items";
                    GridView.Visibility = NbtViewVisibility.Property;
                    GridView.ViewMode = NbtViewRenderingMode.Grid;
                    GridView.Category = "Requests";

                    CswNbtViewRelationship RootRel = GridView.AddViewRelationship( RootNt, false );

                    CswNbtMetaDataNodeTypeProp RelationshipToRootNtp = null;
                    switch( RootNt.getObjectClass().ObjectClass )
                    {
                        case CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass:
                            RelationshipToRootNtp =
                                RequestItemNt.getNodeTypePropByObjectClassProp(
                                    CswNbtObjClassRequestItem.PropertyName.Material );
                            break;

                        case CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass:
                            RelationshipToRootNtp =
                                RequestItemNt.getNodeTypePropByObjectClassProp(
                                    CswNbtObjClassRequestItem.PropertyName.Container );
                            break;
                        default:
                            CswStatusMessage Msg = new CswStatusMessage
                                                       {
                                                           AppType = AppType.SchemUpdt,
                                                           ContentType = ContentType.Error
                                                       };
                            Msg.Attributes.Add( ChemSW.Log.LegalAttribute.exoteric_message, "Request grids of this type are not supported." );
                            _CswNbtSchemaModTrnsctn.CswLogger.send( Msg );
                            break;
                    }
                    if( null != RelationshipToRootNtp )
                    {
                        CswNbtViewRelationship RequestItemRel = GridView.AddViewRelationship( RootRel,
                                                                                             NbtViewPropOwnerType.Second,
                                                                                             RelationshipToRootNtp,
                                                                                             false );
                        CswNbtMetaDataNodeTypeProp RiRequestNtp =
                            RequestItemNt.getNodeTypePropByObjectClassProp(
                                CswNbtObjClassRequestItem.PropertyName.Request );
                        CswNbtViewRelationship RequestRel = GridView.AddViewRelationship( RequestItemRel,
                                                                                         NbtViewPropOwnerType.First,
                                                                                         RiRequestNtp, false );

                        CswNbtViewProperty CompletedVp = GridView.AddViewProperty( RequestRel, CompletedDateNtpNtp );
                        CompletedVp.Order = 3;
                        CompletedVp.SortBy = true;
                        CompletedVp.SortMethod = NbtViewPropertySortMethod.Descending;

                        CswNbtViewProperty SubmittedVp = GridView.AddViewProperty( RequestRel, SubmittedDateNtp );
                        SubmittedVp.Order = 2;
                        SubmittedVp.SortMethod = NbtViewPropertySortMethod.Descending;

                        CswNbtViewProperty NameVp = GridView.AddViewProperty( RequestRel, NameNtp );
                        NameVp.Order = 1;
                        NameVp.SortMethod = NbtViewPropertySortMethod.Descending;

                        CswNbtViewProperty RequestorVp = GridView.AddViewProperty( RequestRel, RequestorNtp );
                        RequestorVp.Order = 4;

                        CswNbtViewProperty TypeVp = GridView.AddViewProperty( RequestItemRel, TypeNtp );
                        TypeVp.Order = 5;
                        CswNbtViewProperty NumberVp = GridView.AddViewProperty( RequestItemRel, NumberNtp );
                        NumberVp.Order = 6;
                        CswNbtViewProperty OrderVp = GridView.AddViewProperty( RequestItemRel, ExternalOrderNoNtp );
                        OrderVp.Order = 7;
                        CswNbtViewProperty StatusVp = GridView.AddViewProperty( RequestItemRel, StatusNtp );
                        StatusVp.Order = 8;

                        GridView.save();
                    }
                }
            }
        }


    }//class CswUpdateSchemaCase24514RequestButton

}//namespace ChemSW.Nbt.Schema