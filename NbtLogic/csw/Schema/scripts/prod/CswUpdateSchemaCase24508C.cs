using System.Linq;
using ChemSW.Config;
using ChemSW.Log;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24508C
    /// </summary>
    public class CswUpdateSchemaCase24508C : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab DispenseTransactionTab = ContainerNt.getNodeTypeTab( "Dispenses" );
                if( null == DispenseTransactionTab )
                {
                    DispenseTransactionTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ContainerNt, "Dispenses", ContainerNt.getNodeTypeTabIds().Count );
                }

                CswNbtMetaDataNodeTypeProp DispenseGridNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ContainerNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Container Dispense Transactions", DispenseTransactionTab.TabId );
                CswNbtView GridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( DispenseGridNtp.ViewId );
                makeDispenseTransactionGridView( GridView, ContainerNt );
            }
        }//Update()

        private void makeDispenseTransactionGridView( CswNbtView GridView, CswNbtMetaDataNodeType RootNt )
        {
            CswNbtMetaDataObjectClass ContDispTransOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataNodeType ContDispTransNt = ContDispTransOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null == ContDispTransNt )
            {
                CswStatusMessage Msg = new CswStatusMessage
                {
                    AppType = AppType.SchemUpdt,
                    ContentType = ContentType.Error
                };
                Msg.Attributes.Add( Log.LegalAttribute.escoteric_message, "Could not get a Container Dispense Transaction NodeType" );
                _CswNbtSchemaModTrnsctn.CswLogger.send( Msg );
            }
            else
            {
                CswNbtMetaDataNodeTypeProp SourceContainerNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.SourceContainerPropertyName );
                CswNbtMetaDataNodeTypeProp DestinationContainerNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.DestinationContainerPropertyName );
                CswNbtMetaDataNodeTypeProp QuantityDispensedNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.QuantityDispensedPropertyName );
                CswNbtMetaDataNodeTypeProp TypeNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.TypePropertyName );
                CswNbtMetaDataNodeTypeProp DispensedDateNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.DispensedDatePropertyName );
                CswNbtMetaDataNodeTypeProp RemainingSourceContainerQuantityNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.RemainingSourceContainerQuantityPropertyName );
                CswNbtMetaDataNodeTypeProp RequestNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.RequestItemPropertyName );

                GridView.Root.ChildRelationships.Clear();
                GridView.ViewName = RootNt.NodeTypeName + " Container Dispense Transactions Grid Property View";
                GridView.Visibility = NbtViewVisibility.Property;
                GridView.ViewMode = NbtViewRenderingMode.Grid;
                GridView.Category = "Dispenses";

                CswNbtViewRelationship RootRel = GridView.AddViewRelationship( RootNt, false );

                if( null == SourceContainerNtp )
                {
                    CswStatusMessage Msg = new CswStatusMessage
                                                {
                                                    AppType = AppType.SchemUpdt,
                                                    ContentType = ContentType.Error
                                                };
                    Msg.Attributes.Add( Log.LegalAttribute.escoteric_message, "Container Dispense Transaction grids of this type are not supported." );
                    _CswNbtSchemaModTrnsctn.CswLogger.send( Msg );
                }
                else
                {
                    CswNbtViewRelationship SrcContDispTransRel = GridView.AddViewRelationship( RootRel, NbtViewPropOwnerType.Second, SourceContainerNtp, false );

                    CswNbtViewProperty SourceContainerVp = GridView.AddViewProperty( SrcContDispTransRel, SourceContainerNtp );
                    SourceContainerVp.Order = 1;
                    SourceContainerVp.SortBy = true;
                    SourceContainerVp.SortMethod = NbtViewPropertySortMethod.Descending;

                    CswNbtViewProperty DispensedDateVp = GridView.AddViewProperty( SrcContDispTransRel, DispensedDateNtp );
                    DispensedDateVp.Order = 2;
                    DispensedDateVp.SortBy = true;
                    DispensedDateVp.SortMethod = NbtViewPropertySortMethod.Descending;

                    CswNbtViewProperty DestinationContainerVp = GridView.AddViewProperty( SrcContDispTransRel, DestinationContainerNtp );
                    DestinationContainerVp.Order = 3;
                    DestinationContainerVp.SortBy = true;
                    DestinationContainerVp.SortMethod = NbtViewPropertySortMethod.Descending;

                    CswNbtViewProperty TypeVp = GridView.AddViewProperty( SrcContDispTransRel, TypeNtp );
                    TypeVp.Order = 4;

                    CswNbtViewProperty QuantityDispensedVp = GridView.AddViewProperty( SrcContDispTransRel, QuantityDispensedNtp );
                    QuantityDispensedVp.Order = 5;

                    CswNbtViewProperty RemainingSourceContainerQuantityVp = GridView.AddViewProperty( SrcContDispTransRel, RemainingSourceContainerQuantityNtp );
                    RemainingSourceContainerQuantityVp.Order = 6;

                    CswNbtViewProperty RequestVp = GridView.AddViewProperty( SrcContDispTransRel, RequestNtp );
                    RequestVp.Order = 7;

                    GridView.save();
                }
            }
        }

    }//class CswUpdateSchemaCase24508C

}//namespace ChemSW.Nbt.Schema