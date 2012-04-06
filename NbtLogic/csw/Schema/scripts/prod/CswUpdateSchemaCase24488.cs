using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24488
    /// </summary>
    public class CswUpdateSchemaCase24488 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            // Add junction from Container to CISPro
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, ContainerOC.ObjectClassId );

            // Add object class props to Container class
            CswNbtMetaDataObjectClassProp MaterialOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                                                            CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                            CswNbtObjClassContainer.MaterialPropertyName,
                                                            CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                            false,
                                                            false,
                                                            true,
                                                            NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                            MaterialOC.ObjectClassId, 
                                                            true );

            CswNbtMetaDataObjectClassProp BarcodeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                                                            CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                            CswNbtObjClassContainer.BarcodePropertyName,
                                                            CswNbtMetaDataFieldType.NbtFieldType.Barcode );

            CswNbtMetaDataObjectClassProp StatusOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.StatusPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.List );

            CswNbtMetaDataObjectClassProp QuantityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.QuantityPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Quantity );

            CswNbtMetaDataObjectClassProp LocationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.LocationPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Location );

            CswNbtMetaDataObjectClassProp LocationVerifiedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                                                           CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.LocationVerifiedPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.DateTime );

            CswNbtMetaDataObjectClassProp SourceContainerOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.SourceContainerPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false,
                                                           false,
                                                           true,
                                                           NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                           ContainerOC.ObjectClassId,
                                                           false );

            CswNbtMetaDataObjectClassProp MissingOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.MissingPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Logical );

            CswNbtMetaDataObjectClassProp DisposedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.DisposedPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Logical );

            CswNbtMetaDataObjectClassProp ExpirationDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                                                           CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.ExpirationDatePropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.DateTime );

            // Location Verified - servermanaged
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationVerifiedOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            // Disposed required, default is false
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisposedOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( DisposedOCP, CswNbtSubField.SubFieldName.Checked, false );

            // Add default Container nodetype to master
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( ContainerOC.ObjectClassId, "Container", "Materials" );
            ContainerNT.IconFileName = "container.gif";
            ContainerNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "Barcode" ) );

            // Add default view to master
            CswNbtView ContainerView = _CswNbtSchemaModTrnsctn.makeView();
            ContainerView.makeNew( "Containers", NbtViewVisibility.Global, null, null, null );
            ContainerView.Category = "Materials";
            ContainerView.ViewMode = NbtViewRenderingMode.Grid;

            CswNbtViewRelationship ContainerRel = ContainerView.AddViewRelationship( ContainerOC, true );

            CswNbtViewProperty BarcodeVP = ContainerView.AddViewProperty( ContainerRel, BarcodeOCP );
            CswNbtViewProperty StatusVP = ContainerView.AddViewProperty( ContainerRel, StatusOCP );
            CswNbtViewProperty QuantityVP = ContainerView.AddViewProperty( ContainerRel, QuantityOCP );
            CswNbtViewProperty LocationVP = ContainerView.AddViewProperty( ContainerRel, LocationOCP );

            BarcodeVP.Order = 2;
            StatusVP.Order = 4;
            QuantityVP.Order = 6;
            LocationVP.Order = 8;

            ContainerView.save();

        }//Update()

    }//class CswUpdateSchemaCase24488

}//namespace ChemSW.Nbt.Schema
