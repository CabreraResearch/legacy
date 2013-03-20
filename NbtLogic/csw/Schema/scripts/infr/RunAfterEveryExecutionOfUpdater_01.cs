
using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Post-schema update script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        #region Blame Logic

        private CswDeveloper _Author = CswDeveloper.NBT;
        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;
        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public static string Title = "Post-Script";

        public override void update()
        {
            _acceptBlame( CswDeveloper.SS, 26029 );
            // This should always be run after schema updates in order to synchronize enabled nodetypes
            _CswNbtSchemaModTrnsctn.MetaData.ResetEnabledNodeTypes();
            _resetBlame();

            _acceptBlame( CswDeveloper.PG, 23784 );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update scheduledrules set reprobate=0,totalroguecount=0,failedcount=0" );
            _resetBlame();

            _CswNbtSchemaModTrnsctn.Modules.TriggerModuleEventHandlers();
        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


