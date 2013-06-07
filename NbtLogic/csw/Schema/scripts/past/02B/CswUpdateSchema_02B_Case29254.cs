using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29254 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 29254; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update node_views set isdemo='0'" );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update node_views set isdemo='1' where visibility = 'Global' and viewname like '%(demo)'" );
        } // update()

    }//class CswUpdateSchema_02B_Case29254

}//namespace ChemSW.Nbt.Schema