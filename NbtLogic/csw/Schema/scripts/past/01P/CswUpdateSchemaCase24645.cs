using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24645
    /// </summary>
    public class CswUpdateSchemaCase24645 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                UnitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.ConversionFactorPropertyName ),
                CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberminvalue,
                0
            );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.SpecificGravityPropertyName ),
                CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberminvalue,
                0
            );
        }//Update()

    }//class CswUpdateSchemaCase24645

}//namespace ChemSW.Nbt.Schema