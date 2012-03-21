using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 24988
    /// </summary>
    public class CswUpdateSchemaCase24988 : CswUpdateSchemaTo
    {
        public override void update()
        {
            string SqlConstraint = "alter table sessionlist add constraint sessionlist_sessionid unique (sessionid);";
            string SqlDelete = "delete from sessionlist;";
            try
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( SqlDelete );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( SqlConstraint );
            }
            catch( Exception ) { }

        }//Update()

    }//class CswUpdateSchemaCase24988

}//namespace ChemSW.Nbt.Schema