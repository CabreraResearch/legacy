using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28690
    /// </summary>
    public class CswUpdateSchema_02B_Case28690D : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28690; }
        }

        private CswNbtMetaDataPropertySet MaterialPS;
        private CswNbtMetaDataObjectClass ChemicalOC;

        public override void update()
        {
            MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass );

            //Set FK Type on all material-related props to Material PropertySet
            setPropFK( CswEnumNbtObjectClass.SizeClass, CswNbtObjClassSize.PropertyName.Material, "Size" );
            setPropFK( CswEnumNbtObjectClass.DocumentClass, CswNbtObjClassDocument.PropertyName.Owner, "Material Document" );
            setPropFK( CswEnumNbtObjectClass.InventoryLevelClass, CswNbtObjClassInventoryLevel.PropertyName.Material, "Inventory Level" );
            setPropFK( CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Material, "Container" );
            setPropFK( CswEnumNbtObjectClass.MaterialSynonymClass, CswNbtObjClassMaterialSynonym.PropertyName.Material, "Material Synonym" );
            setPropFK( CswEnumNbtObjectClass.RequestContainerDispenseClass, CswNbtPropertySetRequestItem.PropertyName.Material, "Request Container Dispense" );
            setPropFK( CswEnumNbtObjectClass.RequestContainerUpdateClass, CswNbtPropertySetRequestItem.PropertyName.Material, "Request Container Update" );
            setPropFK( CswEnumNbtObjectClass.RequestMaterialCreateClass, CswNbtPropertySetRequestItem.PropertyName.Material, "Request Material Create" );
            setPropFK( CswEnumNbtObjectClass.RequestMaterialDispenseClass, CswNbtPropertySetRequestItem.PropertyName.Material, "Request Material Dispense" );
            //Set FK Type on all chemical-related props to Chemical ObjectClass
            setPropFK( CswEnumNbtObjectClass.DocumentClass, CswNbtObjClassDocument.PropertyName.Owner, "SDS Document", true );
            setPropFK( CswEnumNbtObjectClass.GHSClass, CswNbtObjClassGHS.PropertyName.Material, "GHS", true );
            setPropFK( CswEnumNbtObjectClass.ReceiptLotClass, CswNbtObjClassReceiptLot.PropertyName.Material, "Receipt Lot", true );
            setPropFK( CswEnumNbtObjectClass.ManufacturerEquivalentPartClass, CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material, "Manufacturing Equivalent Part", true );
            setPropFK( CswEnumNbtObjectClass.MaterialComponentClass, CswNbtObjClassMaterialComponent.PropertyName.Constituent, "Material Component", true );
            setPropFK( CswEnumNbtObjectClass.MaterialComponentClass, CswNbtObjClassMaterialComponent.PropertyName.Mixture, "Material Component", true );

            //Update all Material grid prop views to use MaterialSet as root relationship
            foreach( CswNbtMetaDataObjectClass MatOC in MaterialPS.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType MaterialNT in MatOC.getNodeTypes() )
                {
                    if( null != MaterialNT )
                    {
                        _updateSynonymsView( MaterialNT );
                        _updateDocumentsView( MaterialNT );
                        _updateSizesView( MaterialNT );
                        _updateInventoryLevelsView( MaterialNT );
                        _updateContainersView( MaterialNT );
                    }
                }
            }

            //Update ViewSelect views to use proper Material root relationship
            _updateUnapprovedMaterialsView();
            _updateMissingHazardClassesView();

        } // update()

        private void setPropFK( String RelatedClassName, String RelatedPropName, String RelatedNodeTypeName, bool isChemicalOnly = false )
        {
            String RelatedIdType = isChemicalOnly ? CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() : CswEnumNbtViewRelatedIdType.PropertySetId.ToString();
            Int32 RelatedId = isChemicalOnly ? ChemicalOC.ObjectClassId : MaterialPS.PropertySetId;

            CswNbtMetaDataObjectClass RelatedOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( RelatedClassName );
            CswNbtMetaDataObjectClassProp MaterialOCP = RelatedOC.getObjectClassProp( RelatedPropName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOCP, CswEnumNbtObjectClassPropAttributes.fktype, RelatedIdType );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOCP, CswEnumNbtObjectClassPropAttributes.fkvalue, RelatedId );
            CswNbtMetaDataNodeType RelatedNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RelatedNodeTypeName );
            if( null != RelatedNT )
            {
                CswNbtMetaDataNodeTypeProp MaterialNTP = RelatedNT.getNodeTypePropByObjectClassProp( RelatedPropName );
                MaterialNTP.SetFK( RelatedIdType, RelatedId );
            }
        }

        #region Material Grid Property Views

        private void _updateSynonymsView( CswNbtMetaDataNodeType MaterialNT )
        {
            CswNbtMetaDataNodeTypeProp SynonymsNTP = MaterialNT.getNodeTypeProp( "Synonyms" );
            if( null != SynonymsNTP )
            {
                CswNbtMetaDataObjectClass MatSynOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialSynonymClass );
                CswNbtMetaDataObjectClassProp MatOCP = MatSynOC.getObjectClassProp( CswNbtObjClassMaterialSynonym.PropertyName.Material );
                CswNbtMetaDataObjectClassProp NameOCP = MatSynOC.getObjectClassProp( CswNbtObjClassMaterialSynonym.PropertyName.Name );

                CswNbtView SynonymsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( SynonymsNTP.ViewId );
                SynonymsView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship RootRel = SynonymsView.AddViewRelationship( MaterialPS, false );
                CswNbtViewRelationship MatRel = SynonymsView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, MatOCP, true );
                SynonymsView.AddViewProperty( MatRel, NameOCP, 1 );
                SynonymsView.save();
            }
        }

        private void _updateDocumentsView( CswNbtMetaDataNodeType MaterialNT )
        {
            CswNbtMetaDataNodeTypeProp DocumentsNTP = MaterialNT.getNodeTypeProp( "Documents" );
            if( null != DocumentsNTP )
            {
                CswNbtMetaDataNodeType DocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
                if( null != DocumentNT )
                {
                    CswNbtMetaDataNodeTypeProp MatOCP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
                    CswNbtMetaDataNodeTypeProp AcquiredDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.AcquiredDate );
                    CswNbtMetaDataNodeTypeProp ExpDateNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.ExpirationDate );
                    CswNbtMetaDataNodeTypeProp FileNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
                    CswNbtMetaDataNodeTypeProp LinkNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
                    CswNbtMetaDataNodeTypeProp ArchivedNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );

                    CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( DocumentsNTP.ViewId );
                    DocumentsView.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship RootRel = DocumentsView.AddViewRelationship( MaterialPS, false );
                    CswNbtViewRelationship MatRel = DocumentsView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, MatOCP, true );
                    DocumentsView.AddViewPropertyAndFilter( MatRel, ArchivedNTP, CswEnumTristate.False.ToString(),
                                                             FilterMode: CswEnumNbtFilterMode.Equals,
                                                             ShowAtRuntime: true,
                                                             ShowInGrid: false );
                    DocumentsView.AddViewProperty( MatRel, AcquiredDateNTP, 1 );
                    DocumentsView.AddViewProperty( MatRel, ExpDateNTP, 2 );
                    DocumentsView.AddViewProperty( MatRel, FileNTP, 3 );
                    DocumentsView.AddViewProperty( MatRel, LinkNTP, 4 );
                    DocumentsView.save();
                }
            }
        }

        private void _updateSizesView( CswNbtMetaDataNodeType MaterialNT )
        {
            CswNbtMetaDataNodeTypeProp SizesNTP = MaterialNT.getNodeTypeProp( MaterialNT.NodeTypeName + " Sizes" );
            if( null != SizesNTP )
            {
                CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp MatOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                CswNbtMetaDataObjectClassProp InitQtyOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );
                CswNbtMetaDataObjectClassProp CatalogNoOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );
                CswNbtMetaDataObjectClassProp QtyEditOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.QuantityEditable );

                CswNbtView SizesView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( SizesNTP.ViewId );
                SizesView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship RootRel = SizesView.AddViewRelationship( MaterialPS, false );
                CswNbtViewRelationship MatRel = SizesView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, MatOCP, true );
                SizesView.AddViewProperty( MatRel, InitQtyOCP, 1 );
                SizesView.AddViewProperty( MatRel, CatalogNoOCP, 2 );
                SizesView.AddViewProperty( MatRel, QtyEditOCP, 3 );
                SizesView.save();
            }
        }

        private void _updateInventoryLevelsView( CswNbtMetaDataNodeType MaterialNT )
        {
            CswNbtMetaDataNodeTypeProp InventoryLevelsNTP = MaterialNT.getNodeTypeProp( "Inventory Levels" );
            if( null != InventoryLevelsNTP )
            {
                CswNbtMetaDataObjectClass InventoryLevelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryLevelClass );
                CswNbtMetaDataObjectClassProp MatOCP = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material );
                CswNbtMetaDataObjectClassProp TypeOCP = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Type );
                CswNbtMetaDataObjectClassProp LevelOCP = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Level );
                CswNbtMetaDataObjectClassProp LocationOCP = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Location );
                CswNbtMetaDataObjectClassProp StatusOCP = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Status );

                CswNbtView InventoryLevelsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( InventoryLevelsNTP.ViewId );
                InventoryLevelsView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship RootRel = InventoryLevelsView.AddViewRelationship( MaterialPS, false );
                CswNbtViewRelationship MatRel = InventoryLevelsView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, MatOCP, true );
                InventoryLevelsView.AddViewProperty( MatRel, TypeOCP, 1 );
                InventoryLevelsView.AddViewProperty( MatRel, LevelOCP, 2 );
                InventoryLevelsView.AddViewProperty( MatRel, LocationOCP, 3 );
                InventoryLevelsView.AddViewProperty( MatRel, StatusOCP, 4 );
                InventoryLevelsView.save();
            }
        }

        private void _updateContainersView( CswNbtMetaDataNodeType MaterialNT )
        {
            CswNbtMetaDataNodeTypeProp ContainersNTP = MaterialNT.getNodeTypeProp( MaterialNT.NodeTypeName + " Containers" );
            if( null != ContainersNTP )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                CswNbtMetaDataObjectClassProp MatOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataObjectClassProp DisposedOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed );
                CswNbtMetaDataObjectClassProp BarcodeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );
                CswNbtMetaDataObjectClassProp QuantityOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );
                CswNbtMetaDataObjectClassProp StatusOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status );
                CswNbtMetaDataObjectClassProp OwnerOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner );
                CswNbtMetaDataObjectClassProp LocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );

                CswNbtView ContainersView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( ContainersNTP.ViewId );
                ContainersView.Root.ChildRelationships.Clear();
                CswNbtViewRelationship RootRel = ContainersView.AddViewRelationship( MaterialPS, false );
                CswNbtViewRelationship MatRel = ContainersView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, MatOCP, true );
                ContainersView.AddViewPropertyAndFilter( MatRel, DisposedOCP, CswEnumTristate.False.ToString(),
                                                             FilterMode: CswEnumNbtFilterMode.Equals,
                                                             ShowAtRuntime: false,
                                                             ShowInGrid: false );
                ContainersView.AddViewProperty( MatRel, BarcodeOCP, 1 );
                ContainersView.AddViewProperty( MatRel, QuantityOCP, 2 );
                ContainersView.AddViewProperty( MatRel, StatusOCP, 3 );
                ContainersView.AddViewProperty( MatRel, OwnerOCP, 4 );
                ContainersView.AddViewProperty( MatRel, LocationOCP, 5 );
                ContainersView.save();
            }
        }

        #endregion Material Grid Property Views

        #region ViewSelect Views

        private void _updateUnapprovedMaterialsView()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp MaterialIdProp = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.MaterialId );
            CswNbtMetaDataObjectClassProp TradeNameProp = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.TradeName );
            CswNbtMetaDataObjectClassProp SupplierProp = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.Supplier );
            CswNbtMetaDataObjectClassProp PartNoProp = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.PartNumber );
            CswNbtMetaDataObjectClassProp CASNoProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.CasNo );
            CswNbtMetaDataObjectClassProp PhysicalStateProp = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.PhysicalState );

            CswNbtView UnapprovedMaterialsView = _CswNbtSchemaModTrnsctn.restoreView( "Unapproved Materials", CswEnumNbtViewVisibility.Global );
            if( null == UnapprovedMaterialsView )
            {
                UnapprovedMaterialsView = _CswNbtSchemaModTrnsctn.makeNewView( "Unapproved Materials", CswEnumNbtViewVisibility.Global );
                UnapprovedMaterialsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                UnapprovedMaterialsView.Category = "Materials";
            }
            else
            {
                UnapprovedMaterialsView.Root.ChildRelationships.Clear();
            }

            CswNbtViewRelationship MatRel = UnapprovedMaterialsView.AddViewRelationship( MaterialPS, true );
            CswNbtViewProperty ApprovedForReceivingPropVP = 
                MaterialOC.getNodeTypes()
                .Select( MaterialNT => MaterialNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving ) )
                .Select( ApprovedForReceivingProp => UnapprovedMaterialsView.AddViewProperty( MatRel, ApprovedForReceivingProp ) ).FirstOrDefault();
            UnapprovedMaterialsView.AddViewPropertyFilter( ApprovedForReceivingPropVP,
                                          CswEnumNbtFilterConjunction.And,
                                          CswEnumNbtFilterResultMode.Hide,
                                          CswEnumNbtSubFieldName.Checked,
                                          CswEnumNbtFilterMode.Equals,
                                          "false");
            UnapprovedMaterialsView.AddViewProperty( MatRel, MaterialIdProp, 1 );
            UnapprovedMaterialsView.AddViewProperty( MatRel, TradeNameProp, 2 );
            UnapprovedMaterialsView.AddViewProperty( MatRel, SupplierProp, 3 );
            UnapprovedMaterialsView.AddViewProperty( MatRel, PartNoProp, 4 );
            UnapprovedMaterialsView.AddViewProperty( MatRel, CASNoProp, 5 );
            UnapprovedMaterialsView.AddViewProperty( MatRel, PhysicalStateProp, 6 );
            UnapprovedMaterialsView.save();
        }

        private void _updateMissingHazardClassesView()
        {
            CswNbtView MHCView = _CswNbtSchemaModTrnsctn.restoreView( "Missing Hazard Classes" );
            if( null == MHCView )
            {
                MHCView = _CswNbtSchemaModTrnsctn.makeNewView( "Missing Hazard Classes", CswEnumNbtViewVisibility.Global );
                MHCView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                MHCView.Category = "Materials";
            }
            else
            {
                MHCView.Root.ChildRelationships.Clear();
            }

            CswNbtViewRelationship RootRel = MHCView.AddViewRelationship( ChemicalOC, true );

            CswNbtMetaDataObjectClassProp SpecialFlagsOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.SpecialFlags );
            CswNbtViewProperty SpecialFlagsVP = MHCView.AddViewProperty( RootRel, SpecialFlagsOCP );
            MHCView.AddViewPropertyFilter( SpecialFlagsVP,
                                            CswEnumNbtFilterConjunction.And,
                                            CswEnumNbtFilterResultMode.Hide,
                                            CswEnumNbtSubFieldName.Value,
                                            CswEnumNbtFilterMode.NotContains,
                                            "Not Reportable" );

            CswNbtMetaDataObjectClassProp HazardClassesOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.HazardClasses );
            CswNbtViewProperty HazardClassesVP = MHCView.AddViewProperty( RootRel, HazardClassesOCP );
            MHCView.AddViewPropertyFilter( HazardClassesVP,
                                            CswEnumNbtFilterConjunction.And,
                                            CswEnumNbtFilterResultMode.Hide,
                                            CswEnumNbtSubFieldName.Value,
                                            CswEnumNbtFilterMode.Null );
            MHCView.save();
        }

        #endregion ViewSelect Views

    }//class CswUpdateSchema_02B_Case28690D
}//namespace ChemSW.Nbt.Schema