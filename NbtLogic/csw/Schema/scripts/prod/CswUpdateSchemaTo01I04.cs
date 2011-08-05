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
    /// Updates the schema to version 01I-04
    /// </summary>
    public class CswUpdateSchemaTo01I04 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 04 ); } }
        public CswUpdateSchemaTo01I04( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
			// case 8635

			CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I04_FieldTypes_Update", "field_types" );
			DataTable FieldTypeTable = FieldTypesUpdate.getEmptyTable();
			DataRow NewFTRow = FieldTypeTable.NewRow();
			NewFTRow["auditflag"] = "0";
			NewFTRow["datatype"] = "text";
			NewFTRow["deleted"] = CswConvert.ToDbVal(false);
			NewFTRow["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.MultiList.ToString();
			FieldTypeTable.Rows.Add( NewFTRow );
			FieldTypesUpdate.update( FieldTypeTable );
			
			

        }//Update()

    }//class CswUpdateSchemaTo01I02

}//namespace ChemSW.Nbt.Schema


