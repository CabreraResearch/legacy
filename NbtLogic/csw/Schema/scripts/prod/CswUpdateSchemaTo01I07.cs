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
	/// Updates the schema to version 01I-07
	/// </summary>
	public class CswUpdateSchemaTo01I07 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 07 ); } }

		public override void update()
		{
			// case 21450 - Remove 'For Reports' views from master data

			CswNbtView View1 = _CswNbtSchemaModTrnsctn.restoreView( "Task Work Order for Reports" );
			if( View1 != null )
				View1.Delete();

			CswNbtView View2 = _CswNbtSchemaModTrnsctn.restoreView( "Open Problems for Report" );
			if( View2 != null )
				View2.Delete();

		} // Update()


	}//class CswUpdateSchemaTo01I07

}//namespace ChemSW.Nbt.Schema


