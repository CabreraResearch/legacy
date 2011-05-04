using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-34
	/// </summary>
	public class CswUpdateSchemaTo01H34 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 34 ); } }
		public CswUpdateSchemaTo01H34( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
			// case 21250
			_CswNbtSchemaModTrnsctn.setConfigVariableValue( "treeview_resultlimit", "1000" );

		} // update()

	}//class CswUpdateSchemaTo01H34

}//namespace ChemSW.Nbt.Schema

