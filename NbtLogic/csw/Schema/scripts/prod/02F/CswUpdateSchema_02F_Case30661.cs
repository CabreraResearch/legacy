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
    public class CswUpdateSchema_02F_Case30661 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30661; }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "DisableCAFImportRule_30661", "scheduledrules" );
                DataTable ScheduleRules = TableUpdate.getTable( "where rulename='" + CswEnumNbtScheduleRuleNames.CAFImport + "'" );
                if( ScheduleRules.Rows.Count > 0 )
                {
                    ScheduleRules.Rows[0]["disabled"] = CswConvert.ToDbVal( true );
                    TableUpdate.update( ScheduleRules );
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema