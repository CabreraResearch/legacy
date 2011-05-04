using System.Data;
using System.Threading; 
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-34
	/// </summary>
	public class CswUpdateSchemaTo01H34 : ICswUpdateSchemaTo
	{

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 


        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 34 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H34( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

		public void update()
		{
            Thread.Sleep( 5000 );

		} // update()

	}//class CswUpdateSchemaTo01H34

}//namespace ChemSW.Nbt.Schema

