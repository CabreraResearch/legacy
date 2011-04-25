using System.Data;

using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-32
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
            
            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate(SchemaVersion.ToString() + ": test script","configuration_variables"); 
            DataTable DataTable = CswTableUpdate.getEmptyTable();
            DataRow DataRow = DataTable.NewRow();
            DataRow["variablename"] = "cmdlinetest";
            DataRow["variablename"] = "iteration 3";
            DataTable.Rows.Add(DataRow);
            CswTableUpdate.update( DataTable );

            System.Threading.Thread.Sleep( 3000 ); 

        } // update()

    }//class CswUpdateSchemaTo01H32

}//namespace ChemSW.Nbt.Schema

