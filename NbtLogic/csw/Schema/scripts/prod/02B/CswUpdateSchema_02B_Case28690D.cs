using System;
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

        public override void update()
        {
            MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );

            setPropFK( CswEnumNbtObjectClass.SizeClass, CswNbtObjClassSize.PropertyName.Material, "Size" );
            setPropFK( CswEnumNbtObjectClass.DocumentClass, CswNbtObjClassDocument.PropertyName.Owner, "Material Document" );
            setPropFK( CswEnumNbtObjectClass.InventoryLevelClass, CswNbtObjClassInventoryLevel.PropertyName.Material, "Inventory Level" );
            setPropFK( CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Material, "Container" );
            setPropFK( CswEnumNbtObjectClass.MaterialSynonymClass, CswNbtObjClassMaterialSynonym.PropertyName.Material, "Material Synonym" );

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
        } // update()

        private void setPropFK( String RelatedClassName, String RelatedPropName, String RelatedNodeTypeName )
        {
            CswNbtMetaDataObjectClass RelatedOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( RelatedClassName );
            CswNbtMetaDataObjectClassProp MaterialOCP = RelatedOC.getObjectClassProp( RelatedPropName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOCP, CswEnumNbtObjectClassPropAttributes.fktype, CswEnumNbtViewRelatedIdType.PropertySetId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOCP, CswEnumNbtObjectClassPropAttributes.fkvalue, MaterialPS.PropertySetId );
            CswNbtMetaDataNodeType RelatedNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( RelatedNodeTypeName );
            if( null != RelatedNT )
            {
                CswNbtMetaDataNodeTypeProp MaterialNTP = RelatedNT.getNodeTypePropByObjectClassProp( RelatedPropName );
                MaterialNTP.SetFK( CswEnumNbtViewRelatedIdType.PropertySetId.ToString(), MaterialPS.PropertySetId );
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

    }//class CswUpdateSchema_02B_Case28690D
}//namespace ChemSW.Nbt.Schema