using System.Data;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31040 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31040; }
        }

        public override string Title
        {
            get { return "CAFImport rule timeout"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle.ToString(), "10" );
            CswTableUpdate SchedServiceUpdt = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "case31040", "scheduledrules" );
            DataTable SchedServiceTable = SchedServiceUpdt.getTable( "where rulename = 'CAFImport'" );
            if( SchedServiceTable.Rows.Count > 0 )
            {
                SchedServiceTable.Rows[0]["maxruntimems"] = 3600000;
            }
            SchedServiceUpdt.update( SchedServiceTable );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema