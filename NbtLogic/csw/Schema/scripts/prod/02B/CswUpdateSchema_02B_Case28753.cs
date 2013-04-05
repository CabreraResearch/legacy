using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case28753 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28753; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update nodes set searchable='1' where nodeid in (select nodeid from nbtdata where propname = 'Disposed' and field1='0')" );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update nodes set searchable='0' where nodeid in (select nodeid from nbtdata where propname = 'Disposed' and field1='1')" );


        
        } // update()

    }//class CswUpdateSchema_02B_Case28753

}//namespace ChemSW.Nbt.Schema