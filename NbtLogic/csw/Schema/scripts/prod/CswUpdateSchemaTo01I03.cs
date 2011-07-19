using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-03
    /// </summary>
    public class CswUpdateSchemaTo01I03 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 03 ); } }
        public CswUpdateSchemaTo01I03( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
			// case 21877
			
			CswTableUpdate WelcomeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("01I-03_Welcome_Update", "welcome");
			DataTable WelcomeTable = WelcomeUpdate.getTable();
			foreach( DataRow WelcomeRow in WelcomeTable.Rows )
			{
				// Remove 'Find Mount Point' from welcome page
				if( WelcomeRow["displaytext"].ToString() == "Find Mount Point" )
				{
					WelcomeRow.Delete();
				}

				// Fix typo: 'Find Inpsection Point' to 'Find Inspection Point'
				else if( WelcomeRow["displaytext"].ToString() == "Find Inpsection Point" )
				{
					WelcomeRow["displaytext"] = "Find Inspection Point";
				}
			} // foreach( DataRow WelcomeRow in WelcomeTable.Rows )

			WelcomeUpdate.update( WelcomeTable );

        } // Update()

    }//class CswUpdateSchemaTo01I03

}//namespace ChemSW.Nbt.Schema


