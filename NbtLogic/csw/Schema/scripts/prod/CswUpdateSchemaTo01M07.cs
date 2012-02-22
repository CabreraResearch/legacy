using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-06
    /// </summary>
    public class CswUpdateSchemaTo01M07 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 07 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 24981
            //Rebirth of vendor object class
            CswTableUpdate ObjectClassUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "createnewvendorclass", "object_class" );
            DataTable ObjectClasstable = ObjectClassUpdate.getEmptyTable();
            DataRow NewObjectClassRow = ObjectClasstable.NewRow();
            ObjectClasstable.Rows.Add( NewObjectClassRow );
            NewObjectClassRow["objectclass"] = CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass.ToString();
            ObjectClassUpdate.update( ObjectClasstable );


            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Approval Status", CswNbtMetaDataFieldType.NbtFieldType.Logical );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Specific Gravity", CswNbtMetaDataFieldType.NbtFieldType.Scientific );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Physcial State", CswNbtMetaDataFieldType.NbtFieldType.List, false, false, false, string.Empty, Int32.MinValue, false, false, false, false, "solid, liquid, gas" );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "CAS No", CswNbtMetaDataFieldType.NbtFieldType.Text );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Regulatory Lists", CswNbtMetaDataFieldType.NbtFieldType.Static );


            CswNbtMetaDataObjectClassProp PartNoOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Part Number", CswNbtMetaDataFieldType.NbtFieldType.Text );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PartNoOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );

            CswNbtMetaDataObjectClassProp TradenameOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Tradename", CswNbtMetaDataFieldType.NbtFieldType.Text );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TradenameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );


            CswNbtMetaDataObjectClassProp StorageClassOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Storage Capacity", CswNbtMetaDataFieldType.NbtFieldType.ImageList );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StorageClassOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.multi, false );

            CswNbtMetaDataObjectClassProp SupplierOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass, "Supplier", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false, false, true, CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SupplierOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SupplierOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SupplierOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass ) );



            #endregion case 24981



        }//Update()

    }//class CswUpdateSchemaTo01M07

}//namespace ChemSW.Nbt.Schema