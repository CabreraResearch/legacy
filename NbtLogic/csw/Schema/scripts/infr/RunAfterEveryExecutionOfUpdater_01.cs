
using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Post-schema update script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_01: CswUpdateSchemaTo
    {
        #region Blame Logic

        private CswEnumDeveloper _Author = CswEnumDeveloper.NBT;
        public override CswEnumDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;
        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswEnumDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswEnumDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public static string Title = "Post-Script";

        public override void update()
        {
            _acceptBlame( CswEnumDeveloper.SS, 26029 );
            // This should always be run after schema updates in order to synchronize enabled nodetypes
            _CswNbtSchemaModTrnsctn.MetaData.ResetEnabledNodeTypes();
            _resetBlame();

            _acceptBlame( CswEnumDeveloper.PG, 23784 );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update scheduledrules set reprobate=0,totalroguecount=0,failedcount=0" );
            _resetBlame();

            #region BUCKEYE

            #endregion


            #region CEDAR

            #endregion CEDAR


            _CswNbtSchemaModTrnsctn.Modules.TriggerModuleEventHandlers();
        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


