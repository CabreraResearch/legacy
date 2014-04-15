using System;
using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Post-schema update script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_DBViews : CswUpdateSchemaTo
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

        public override string AppendToScriptName()
        {
            return "RunAfter_PostScript_Create_Relational_Views";
        }

        public override bool AlwaysRun
        {
            get { return true; }
        }

        public override string Title { get { return "Post-Script: Create Relational Views"; } }
        public override void update()
        {
            _acceptBlame( CswEnumDeveloper.DH, 30252 );
            List<CswStoredProcParam> Params = new List<CswStoredProcParam>();
            _CswNbtSchemaModTrnsctn.execStoredProc( "CREATEALLNTVIEWS", Params );
            _resetBlame();

            _acceptBlame( CswEnumDeveloper.BV, 52432 );
            _CswNbtSchemaModTrnsctn.execStoredProc( "NBT_VIEW_BUILDER.CREATE_ALL_OC_VIEWS", new List<CswStoredProcParam>() );
            _resetBlame();
        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


