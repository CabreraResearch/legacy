using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24485
    /// </summary>
    public class CswUpdateSchemaCase24485 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // cispro grid views: category=Containers , scope=global
            // name=Expiring Containers (expiration<=today+30) mode=grid, struct=container>size>material
            CswNbtMetaDataObjectClass contOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp contSizeProp = contOC.getObjectClassProp( CswNbtObjClassContainer.SizePropertyName );
            CswNbtMetaDataObjectClassProp contBarcodeProp = contOC.getObjectClassProp( CswNbtObjClassContainer.BarcodePropertyName );
            CswNbtMetaDataObjectClassProp contQuantityProp = contOC.getObjectClassProp( CswNbtObjClassContainer.QuantityPropertyName );
            CswNbtMetaDataObjectClassProp contLocationProp = contOC.getObjectClassProp( CswNbtObjClassContainer.LocationPropertyName );
            CswNbtMetaDataObjectClassProp contExpiresProp = contOC.getObjectClassProp( CswNbtObjClassContainer.ExpirationDatePropertyName );
            CswNbtMetaDataObjectClassProp contMissingProp = contOC.getObjectClassProp( CswNbtObjClassContainer.MissingPropertyName );
            CswNbtMetaDataObjectClass sizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp sizeMatProp = sizeOC.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
            CswNbtMetaDataObjectClass matOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp matTradenameProp = matOC.getObjectClassProp( CswNbtObjClassMaterial.TradenamePropertyName );
            CswNbtMetaDataObjectClassProp matSupplierProp = matOC.getObjectClassProp( CswNbtObjClassMaterial.SupplierPropertyName );
            CswNbtMetaDataObjectClassProp matPartNumberProp = matOC.getObjectClassProp( CswNbtObjClassMaterial.PartNumberPropertyName );

            CswNbtView expView = _CswNbtSchemaModTrnsctn.makeView();
            expView.makeNew( "Expiring Containers", NbtViewVisibility.Global );
            expView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtViewRelationship contRelationship = expView.AddViewRelationship( contOC, true );
            //add size
            CswNbtViewRelationship sizeRelationship = expView.AddViewRelationship( contRelationship, NbtViewPropOwnerType.First, contSizeProp, true );
            //add material
            CswNbtViewRelationship matRelationship = expView.AddViewRelationship( sizeRelationship, NbtViewPropOwnerType.First, sizeMatProp, true );
            //add props
            CswNbtViewProperty bcViewProp = expView.AddViewProperty( contRelationship, contBarcodeProp );
            bcViewProp.Order = 1;
            CswNbtViewProperty expViewProp = expView.AddViewProperty( contRelationship, contExpiresProp );
            expViewProp.Order = 2;
            CswNbtViewProperty tnameViewProp = expView.AddViewProperty( matRelationship, matTradenameProp );
            tnameViewProp.Order = 3;
            CswNbtViewProperty suppViewProp = expView.AddViewProperty( matRelationship, matSupplierProp );
            suppViewProp.Order = 4;
            CswNbtViewProperty partViewProp = expView.AddViewProperty( matRelationship, matPartNumberProp );
            partViewProp.Order = 5;
            CswNbtViewProperty qtyViewProp = expView.AddViewProperty( contRelationship, contQuantityProp );
            qtyViewProp.Order = 6;
            CswNbtViewProperty locViewProp = expView.AddViewProperty( contRelationship, contLocationProp );
            locViewProp.Order = 7;
            expView.save();


            // Missing Containers (missing==true)
            CswNbtView missingView = _CswNbtSchemaModTrnsctn.makeView();
            missingView.makeNew( "Missing Containers", NbtViewVisibility.Global, null, null, expView );
            missingView.save();


            //add filters
            expView.AddViewPropertyFilter(
                                             expViewProp,
                                             CswNbtSubField.SubFieldName.Value,
                                             CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals,
                                             "today+30",
                                             false );
            expView.save();


            CswNbtViewProperty missingViewProp = expView.AddViewProperty( contRelationship, contMissingProp );
            missingViewProp.Order = 8;
            missingViewProp.ShowInGrid = false;
            missingView.AddViewPropertyFilter(
                                               missingViewProp,
                                               CswNbtSubField.SubFieldName.Value,
                                               CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                               true.ToString(),
                                               false );
            missingView.save();


        }//Update()

    }//class CswUpdateSchemaCase24485

}//namespace ChemSW.Nbt.Schema