using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25960
    /// </summary>
    public class CswUpdateSchemaCase25960 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //1. Add new properties to Customer Object Class:
            
            //Schema Name (fieldtype: Static)
            _CswNbtSchemaModTrnsctn.createObjectClassProp( 
                CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass,
                CswNbtObjClassCustomer.SchemaNamePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Static );

            //Schema Version (fieldtype: Static)
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass,
                CswNbtObjClassCustomer.SchemaVersionPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Static );

        }//Update()

    }//class CswUpdateSchemaCase25960

}//namespace ChemSW.Nbt.Schema