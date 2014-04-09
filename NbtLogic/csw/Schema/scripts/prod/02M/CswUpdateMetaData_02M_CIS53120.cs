using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS53120 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 53120; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo + ":  Remove SetMaterialObsolete schedule rule"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            // Remove the SetMaterialObsolete schedule rule
            CswTableUpdate SchedRulesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "SchemaModTrnsctn_ScheduledRuleDelete", "scheduledrules" );
            DataTable RuleTable = SchedRulesUpdate.getTable( " where lower(rulename)='" + "setmaterialobsolete" + "' " );
            if( RuleTable.Rows.Count == 1 )
            {
                RuleTable.Rows[0].Delete();
            }
            SchedRulesUpdate.update( RuleTable );
        }
    }
}