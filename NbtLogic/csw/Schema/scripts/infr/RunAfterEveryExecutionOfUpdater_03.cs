using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Post-schema update script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_03 : CswUpdateSchemaTo
    {
        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.DH; }
        }

        public override int CaseNo
        {
            get { return 30252; }
        }

        #endregion Blame Logic

        public override string AppendToScriptName()
        {
            return "RunAfter_PostScript_Create_All_NT_Views";
        }

        public override bool AlwaysRun
        {
            get { return true; }
        }

        public override string Title { get { return "Post-Script: Create All NT Views"; } }
        public override void update()
        {
            List<CswStoredProcParam> Params = new List<CswStoredProcParam>();
            _CswNbtSchemaModTrnsctn.execStoredProc( "CREATEALLNTVIEWS", Params );
        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


