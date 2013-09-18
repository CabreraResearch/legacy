using System;
using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Post-schema update script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
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

        public override string ScriptName
        {
            get { return "RunAfter_PostScript"; }
        }

        public override bool AlwaysRun
        {
            get { return true; }
        }

        public override string Title { get { return "Post-Script: Reset Enabled NodeTypes | Re-enable Scheduled Rules | Clear Session Data | Trigger all Module Events"; } }
        public override void update()
        {
            _acceptBlame( CswEnumDeveloper.SS, 26029 );
            // This should always be run after schema updates in order to synchronize enabled nodetypes
            _CswNbtSchemaModTrnsctn.MetaData.ResetEnabledNodeTypes();
            _resetBlame();

            _acceptBlame( CswEnumDeveloper.PG, 23784 );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update scheduledrules set reprobate=0,totalroguecount=0,failedcount=0" );
            _resetBlame();

            _acceptBlame( CswEnumDeveloper.DH, 30252 );

            List<CswStoredProcParam> Params = new List<CswStoredProcParam>();
            _CswNbtSchemaModTrnsctn.execStoredProc( "CREATEALLNTVIEWS", Params );
            _resetBlame();

            _acceptBlame(CswEnumDeveloper.MB, 30700);
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "truncate table sessionlist" );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "truncate table session_data" );
            _resetBlame();

            _CswNbtSchemaModTrnsctn.Modules.TriggerModuleEventHandlers();


        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


