using System.Data;
using System.Threading;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-31
	/// </summary>
	public class CswUpdateSchemaTo01H32 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 32 ); } }
		public CswUpdateSchemaTo01H32( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
            Thread.Sleep( 5000 ); 


		} // update()

	}//class CswUpdateSchemaTo01H32

}//namespace ChemSW.Nbt.Schema

