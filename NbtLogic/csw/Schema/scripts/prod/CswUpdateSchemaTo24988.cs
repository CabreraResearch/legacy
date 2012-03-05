using System;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to case 24988
    /// </summary>
    public class CswUpdateSchemaTo24988 : CswUpdateSchemaTo
    {
        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 12 ); } }
        //public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

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

    }//class CswUpdateSchemaTo24988

}//namespace ChemSW.Nbt.Schema