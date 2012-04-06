using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24488
    /// </summary>
    public class CswUpdateSchemaCase24488 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            // Add junction from Container to CISPro
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, ContainerOC.ObjectClassId );

            // Add object class props to Container class
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.StatusPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.List );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.QuantityPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Quantity );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.LocationPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Location );

            CswNbtMetaDataObjectClassProp LocationVerifiedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( 
                                                           CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.LocationVerifiedPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.DateTime );
            
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.SourceContainerPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Relationship );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.MissingPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Logical );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                                                           CswNbtObjClassContainer.DisposedPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Logical );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationVerifiedOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );
            

            // Add default Container nodetype to master data
            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( ContainerOC.ObjectClassId, "Container", "Materials" );


        }//Update()

    }//class CswUpdateSchemaCase24488

}//namespace ChemSW.Nbt.Schema