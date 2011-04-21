using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-31
    /// </summary>
    public class CswUpdateSchemaTo01H31 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 31 ); } }
        public CswUpdateSchemaTo01H31( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
       
        {

            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + ": test script", "configuration_variables" );
            DataTable DataTable = CswTableUpdate.getTable( " where variablevalue='cmdlinetest'" );
            DataRow DataRow = DataTable.NewRow();
            DataRow["variablename"] = "cmdlinetest";
            DataRow["variablename"] = "iteration 2";
            DataTable.Rows.Add( DataRow );
            CswTableUpdate.update( DataTable );

            System.Threading.Thread.Sleep( 3000 ); 



        } // update()

    }//class CswUpdateSchemaTo01H31

}//namespace ChemSW.Nbt.Schema

