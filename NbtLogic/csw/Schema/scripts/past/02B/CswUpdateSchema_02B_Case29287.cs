using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for Case 29287
    /// </summary>
    public class CswUpdateSchema_02B_Case29287 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29287; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update scheduledrules set recurrence = 'NMinutes', interval = 15 where recurrence = 'NSeconds'" );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update scheduledrules set threadid = null, statusmessage = null, runstarttime = null, runendtime = null" );
        } // update()

    }//class CswUpdateSchema_02B_Case29287

}//namespace ChemSW.Nbt.Schema