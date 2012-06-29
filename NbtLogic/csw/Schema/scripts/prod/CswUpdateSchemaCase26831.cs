using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26831
    /// </summary>
    public class CswUpdateSchemaCase26831 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //get the containerOC and the props we're going to add to chemical, biological and supply
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.BarcodePropertyName );
            CswNbtMetaDataObjectClassProp quantityOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.QuantityPropertyName );
            CswNbtMetaDataObjectClassProp statusOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.StatusPropertyName );
            CswNbtMetaDataObjectClassProp expirationDateOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.ExpirationDatePropertyName );
            CswNbtMetaDataObjectClassProp ownerOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.OwnerPropertyName );
            CswNbtMetaDataObjectClassProp locationOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.LocationPropertyName );

            //get the sizeOC and the props we're going to add to the material 
            CswNbtMetaDataObjectClass sizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp sizeCapacityOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName );
            CswNbtMetaDataObjectClassProp sizeQuantityEditableOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName );
            CswNbtMetaDataObjectClassProp sizeCatalogNoOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.CatalogNoPropertyName );

            //Update Chemical
            CswNbtMetaDataNodeType chemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != chemicalNT )
            {
                CswNbtMetaDataNodeTypeTab chemContainerTab = chemicalNT.getNodeTypeTab( "Containers" );
                if( null != chemContainerTab )
                {
                    //create the chemical containers grid
                    CswNbtMetaDataNodeTypeProp chemContainersGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( chemicalNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Chemical Containers", chemContainerTab.TabId );

                    CswNbtView chemContainersView = _CswNbtSchemaModTrnsctn.restoreView( chemContainersGridNTP.ViewId );
                    if( null == chemContainersView )
                    {
                        chemContainersView = _CswNbtSchemaModTrnsctn.makeView();
                        chemContainersGridNTP.ViewId = chemContainersView.ViewId;
                    }

                    chemContainersView.Root.ChildRelationships.Clear();
                    chemContainersView.ViewMode = NbtViewRenderingMode.Grid;
                    chemContainersView.Visibility = NbtViewVisibility.Property;

                    /*
                     * Chemical
                     *  |-> Container (by Material) THIS IS THE OWNER, so ownertype = 2
                     *      |-> barcode
                     *      |-> quantity
                     *      |-> ect, ect...
                     */

                    CswNbtViewRelationship ParentRelationship = chemContainersView.AddViewRelationship( chemicalNT, true );
                    CswNbtViewRelationship ChildRelationship = chemContainersView.AddViewRelationship( ParentRelationship, NbtViewPropOwnerType.Second, containerOC.getObjectClassProp( CswNbtObjClassContainer.MaterialPropertyName ), true );

                    //add properties
                    CswNbtViewProperty barcodeVP = chemContainersView.AddViewProperty( ChildRelationship, barcodeOCP );
                    CswNbtViewProperty quantityVP = chemContainersView.AddViewProperty( ChildRelationship, quantityOCP );
                    CswNbtViewProperty statusVP = chemContainersView.AddViewProperty( ChildRelationship, statusOCP );
                    CswNbtViewProperty expireDateVP = chemContainersView.AddViewProperty( ChildRelationship, expirationDateOCP );
                    CswNbtViewProperty ownerVP = chemContainersView.AddViewProperty( ChildRelationship, ownerOCP );
                    CswNbtViewProperty locationVP = chemContainersView.AddViewProperty( ChildRelationship, locationOCP );

                    barcodeVP.Order = 1;
                    quantityVP.Order = 2;
                    statusVP.Order = 3;
                    expireDateVP.Order = 4;
                    ownerVP.Order = 5;
                    locationVP.Order = 6;

                    chemContainersView.save();
                    // end making chemical containers grid

                    //create the chemical sizes grid
                    CswNbtMetaDataNodeTypeProp chemSizesGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( chemicalNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Chemical Sizes", chemContainerTab.TabId );
                    CswNbtView chemSizesView = _CswNbtSchemaModTrnsctn.restoreView( chemSizesGridNTP.ViewId );
                    if( null == chemSizesView )
                    {
                        chemSizesView = _CswNbtSchemaModTrnsctn.makeView();
                        chemSizesGridNTP.ViewId = chemSizesView.ViewId;
                    }
                    chemSizesGridNTP.Extended = CswNbtNodePropGrid.GridPropMode.Link.ToString(); //make it a link grid

                    chemSizesView.Root.ChildRelationships.Clear();
                    chemSizesView.ViewMode = NbtViewRenderingMode.Grid;
                    chemSizesView.Visibility = NbtViewVisibility.Property;

                    CswNbtViewRelationship chemSizesPR = chemSizesView.AddViewRelationship( chemicalNT, true ); //PR = "parent relationship", CR = "child relationship
                    CswNbtViewRelationship chemSizesCR = chemSizesView.AddViewRelationship( chemSizesPR, NbtViewPropOwnerType.Second, sizeOC.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName ), true );

                    CswNbtViewProperty chemCapacityVP = chemSizesView.AddViewProperty( chemSizesCR, sizeCapacityOCP );
                    CswNbtViewProperty chemQuantityEditableVP = chemSizesView.AddViewProperty( chemSizesCR, sizeQuantityEditableOCP );
                    CswNbtViewProperty chemCatalogNoVP = chemSizesView.AddViewProperty( chemSizesCR, sizeCatalogNoOCP );

                    chemCapacityVP.Order = 1;
                    chemQuantityEditableVP.Order = 2;
                    chemCatalogNoVP.Order = 3;

                    chemSizesView.save();
                    //end making chemical sizes link grid
                }
            }

            //Update Biological
            CswNbtMetaDataNodeType biologicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );
            if( null != biologicalNT )
            {
                CswNbtMetaDataNodeTypeTab bioContainerTab = biologicalNT.getNodeTypeTab( "Containers" );
                if( null != bioContainerTab )
                {
                    CswNbtMetaDataNodeTypeProp bioContainersGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( biologicalNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Biological Containers", bioContainerTab.TabId );

                    CswNbtView bioContainersView = _CswNbtSchemaModTrnsctn.restoreView( bioContainersGridNTP.ViewId );
                    if( null == bioContainersView )
                    {
                        bioContainersView = _CswNbtSchemaModTrnsctn.makeView();
                        bioContainersGridNTP.ViewId = bioContainersView.ViewId;
                    }

                    bioContainersView.Root.ChildRelationships.Clear();
                    bioContainersView.ViewMode = NbtViewRenderingMode.Grid;
                    bioContainersView.Visibility = NbtViewVisibility.Property;

                    /*
                     * biological
                     *  |-> Container (by Material) THIS IS THE OWNER, so ownertype = 2
                     *      |-> barcode
                     *      |-> quantity
                     *      |-> ect, ect...
                     */

                    CswNbtViewRelationship bioParentRelationship = bioContainersView.AddViewRelationship( biologicalNT, true );
                    CswNbtViewRelationship bioChildRelationship = bioContainersView.AddViewRelationship( bioParentRelationship, NbtViewPropOwnerType.Second, containerOC.getObjectClassProp( CswNbtObjClassContainer.MaterialPropertyName ), true );

                    //add properties
                    CswNbtViewProperty bioBarcodeVP = bioContainersView.AddViewProperty( bioChildRelationship, barcodeOCP );
                    CswNbtViewProperty bioQuantityVP = bioContainersView.AddViewProperty( bioChildRelationship, quantityOCP );
                    CswNbtViewProperty bioStatusVP = bioContainersView.AddViewProperty( bioChildRelationship, statusOCP );
                    CswNbtViewProperty bioOwnerVP = bioContainersView.AddViewProperty( bioChildRelationship, ownerOCP );
                    CswNbtViewProperty bioLocationVP = bioContainersView.AddViewProperty( bioChildRelationship, locationOCP );

                    bioBarcodeVP.Order = 1;
                    bioQuantityVP.Order = 2;
                    bioStatusVP.Order = 3;
                    bioOwnerVP.Order = 4;
                    bioLocationVP.Order = 5;

                    bioContainersView.save();

                }
            }

            //Update Supply
            CswNbtMetaDataNodeType supplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "supply" );
            if( null != supplyNT )
            {
                CswNbtMetaDataNodeTypeTab supplyContainerTab = supplyNT.getNodeTypeTab( "Containers" );
                if( null != supplyContainerTab )
                {
                    CswNbtMetaDataNodeTypeProp supplyContainersGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( supplyNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Supply Containers", supplyContainerTab.TabId );

                    CswNbtView supplyContainersView = _CswNbtSchemaModTrnsctn.restoreView( supplyContainersGridNTP.ViewId );
                    if( null == supplyContainersView )
                    {
                        supplyContainersView = _CswNbtSchemaModTrnsctn.makeView();
                        supplyContainersGridNTP.ViewId = supplyContainersView.ViewId;
                    }

                    supplyContainersView.Root.ChildRelationships.Clear();
                    supplyContainersView.ViewMode = NbtViewRenderingMode.Grid;
                    supplyContainersView.Visibility = NbtViewVisibility.Property;

                    /*
                     * supply
                     *  |-> Container (by Material) THIS IS THE OWNER, so ownertype = 2
                     *      |-> barcode
                     *      |-> quantity
                     *      |-> ect, ect...
                     */

                    CswNbtViewRelationship supplyParentRelationship = supplyContainersView.AddViewRelationship( supplyNT, true );
                    CswNbtViewRelationship supplyChildRelationship = supplyContainersView.AddViewRelationship( supplyParentRelationship, NbtViewPropOwnerType.Second, containerOC.getObjectClassProp( CswNbtObjClassContainer.MaterialPropertyName ), true );

                    //add properties
                    CswNbtViewProperty supplyBarcodeVP = supplyContainersView.AddViewProperty( supplyChildRelationship, barcodeOCP );
                    CswNbtViewProperty supplyQuantityVP = supplyContainersView.AddViewProperty( supplyChildRelationship, quantityOCP );
                    CswNbtViewProperty supplyStatusVP = supplyContainersView.AddViewProperty( supplyChildRelationship, statusOCP );
                    CswNbtViewProperty supplyOwnerVP = supplyContainersView.AddViewProperty( supplyChildRelationship, ownerOCP );
                    CswNbtViewProperty supplyLocationVP = supplyContainersView.AddViewProperty( supplyChildRelationship, locationOCP );

                    supplyBarcodeVP.Order = 1;
                    supplyQuantityVP.Order = 2;
                    supplyStatusVP.Order = 3;
                    supplyOwnerVP.Order = 4;
                    supplyLocationVP.Order = 5;

                    supplyContainersView.save();

                }
            }

        }//Update()

    }//class CswUpdateSchemaCase26831

}//namespace ChemSW.Nbt.Schema