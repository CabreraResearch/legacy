using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01I-08
	/// </summary>
	public class CswUpdateSchemaTo01I08 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 08 ); } }
		public CswUpdateSchemaTo01I08( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}


		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


		public void update()
		{

			// case 9943
			// Add "Time Format" property to User

			CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

			CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I-08_OCP_Update", "object_class_props" );
			DataTable OCPTable = OCPUpdate.getEmptyTable();
			_CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, UserOC, CswNbtObjClassUser.TimeFormatPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
														   false, false, false, string.Empty, Int32.MinValue, false, false, false, false,
														   "h:mm:ss tt, H:mm:ss", Int32.MinValue, Int32.MinValue );
			OCPUpdate.update( OCPTable );

			_CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

		} // Update()


	}//class CswUpdateSchemaTo01I08

}//namespace ChemSW.Nbt.Schema


