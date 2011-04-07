using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-25
	/// </summary>
	public class CswUpdateSchemaTo01H25 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 25 ); } }
		public CswUpdateSchemaTo01H25( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
			// case 21347
			CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
			CswTableUpdate OCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H25_OC_Update", "object_class" );
			DataTable OCTable = OCUpdate.getTable( "objectclassid", UserOC.ObjectClassId );
			if( OCTable.Rows.Count > 0 )
			{
				OCTable.Rows[0]["iconfilename"] = "user.gif";
				OCUpdate.update( OCTable );
			}

			foreach( CswNbtMetaDataNodeType UserNT in UserOC.NodeTypes )
			{
				UserNT.IconFileName = "user.gif";
			}

		} // update()

	}//class CswUpdateSchemaTo01H25

}//namespace ChemSW.Nbt.Schema

