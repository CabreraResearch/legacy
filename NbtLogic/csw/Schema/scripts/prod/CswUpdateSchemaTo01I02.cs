using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-02
    /// </summary>
    public class CswUpdateSchemaTo01I02 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 02 ); } }
        public CswUpdateSchemaTo01I02( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
			// case 9943
			// Add "Date Format" property to User
			
			CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass);
			
			CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("01I-02_OCP_Update", "object_class_props");
			DataTable OCPTable = OCPUpdate.getEmptyTable();
			_CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, UserOC, CswNbtObjClassUser.DateFormatPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List, 
														   false, false, false, string.Empty, Int32.MinValue, false, false, false, false, 
														   "MM/dd/yyyy,dd-MM-yyyy,yyyy/MM/dd,dd MMM yyyy", Int32.MinValue, Int32.MinValue );
			OCPUpdate.update( OCPTable );

			_CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

        }//Update()

    }//class CswUpdateSchemaTo01I02

}//namespace ChemSW.Nbt.Schema


