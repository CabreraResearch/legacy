using System;
using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 24988
    /// </summary>
    public class CswUpdateSchemaCase24988Epilogue02 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //string SqlDelete = "delete from sessionlist";
            //_CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( SqlDelete );



            if( string.Empty == _CswNbtSchemaModTrnsctn.getUniqueConstraintName( "sessionlist", "sessionid" ) )
            {

                _CswNbtSchemaModTrnsctn.makeUniqueConstraint( "sessionlist", "sessionid" );
            }

        }//Update()

    }//class CswUpdateSchemaCase24988Epilogue02

}//namespace ChemSW.Nbt.Schema