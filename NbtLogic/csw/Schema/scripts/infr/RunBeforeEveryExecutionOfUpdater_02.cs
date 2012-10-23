using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Runs the initialize script
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {
        public static string Title = "RunBeforeEveryExecutionOfUpdater_02";

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execStoredProc( "CreateAllNtViews", new List<CswStoredProcParam>() );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //update()

    }//class RunBeforeEveryExecutionOfUpdater_02

}//namespace ChemSW.Nbt.Schema
