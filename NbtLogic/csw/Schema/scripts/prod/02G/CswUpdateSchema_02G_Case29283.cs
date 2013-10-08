using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case29283 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29283; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Row Limits on SQL Reports"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswEnumNbtConfigurationVariables.sql_report_resultlimit, "Result Limit for SQL Reports", "500", IsSystem: false );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema