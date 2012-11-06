using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26579
    /// </summary>
    public class CswUpdateSchemaCase26579 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // materialobjclassprops
            CswNbtMetaDataObjectClass matOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp tradeNameOcp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( matOC.ObjectClassId, CswNbtObjClassMaterial.TradenamePropertyName );
            CswNbtMetaDataObjectClassProp supplierOcp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( matOC.ObjectClassId, CswNbtObjClassMaterial.SupplierPropertyName );
            CswNbtMetaDataObjectClassProp partOcp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( matOC.ObjectClassId, CswNbtObjClassMaterial.PartNumberPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( tradeNameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( supplierOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( tradeNameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( supplierOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( partOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );


        }//Update()

    }//class CswUpdateSchemaCase26579

}//namespace ChemSW.Nbt.Schema