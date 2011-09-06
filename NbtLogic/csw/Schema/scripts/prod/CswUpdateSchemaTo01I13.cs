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
	/// Updates the schema to version 01I-13
	/// </summary>
	public class CswUpdateSchemaTo01I13 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 13 ); } }
		public CswUpdateSchemaTo01I13( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}


		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


		public void update()
		{

		} // Update()


	}//class CswUpdateSchemaTo01I13

}//namespace ChemSW.Nbt.Schema


