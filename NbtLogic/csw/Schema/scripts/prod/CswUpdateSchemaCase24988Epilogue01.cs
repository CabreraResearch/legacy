using System;
using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 24988
    /// </summary>
    public class CswUpdateSchemaCase24988Epilogue01 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //string SqlDelete = "delete from sessionlist";
            //_CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( SqlDelete );



            if( string.Empty == _CswNbtSchemaModTrnsctn.getUniqueConstraintName( "sessionlist", "sessionid" ) )
            {

                CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "nukesessionlistrecords", "sessionlist" );
                DataTable DataTable = CswTableUpdate.getTable();
                foreach( DataRow CurrentRow in DataTable.Rows )
                {
                    CurrentRow.Delete();
                }

                CswTableUpdate.update( DataTable );
            }

        }//Update()

    }//class CswUpdateSchemaCase24988Epilogue01

}//namespace ChemSW.Nbt.Schema