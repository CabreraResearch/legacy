using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30913 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {                
            get { return 30913; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Disable CAFImport On All Schema"; }
        }

        public override void update()
        {
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "DisableCAFImportRule_30913", "scheduledrules" );
            DataTable ScheduleRules = TableUpdate.getTable( "where rulename='" + CswEnumNbtScheduleRuleNames.CAFImport + "'" );
            if( ScheduleRules.Rows.Count > 0 )
            {
                ScheduleRules.Rows[0]["disabled"] = CswConvert.ToDbVal( true );
                ScheduleRules.Rows[0]["reprobate"] = CswConvert.ToDbVal( false );
                ScheduleRules.Rows[0]["StatusMessage"] = CswConvert.ToDbVal( "" );
                TableUpdate.update( ScheduleRules );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema