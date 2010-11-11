using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-08
    /// </summary>
    public class CswUpdateSchemaTo01H08 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 08 ); } }
        public CswUpdateSchemaTo01H08( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 20062
            // promote Location 'Name' and Mount Point 'Barcode' to object class props
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass MountPointOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MountPointClass );

            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H08_ocp_update", "object_class_props" );
            DataTable OCPTable = OCPUpdate.getEmptyTable();
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, LocationOC.ObjectClassId, CswNbtObjClassLocation.NamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, MountPointOC.ObjectClassId, CswNbtObjClassMountPoint.BarcodePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Barcode, Int32.MinValue, Int32.MinValue );
            OCPUpdate.update( OCPTable );

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

        } // update()

    }//class CswUpdateSchemaTo01H08

}//namespace ChemSW.Nbt.Schema


