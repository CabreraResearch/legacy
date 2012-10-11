using ChemSW.Nbt.csw.Dev;
using ChemSW.DB;
using System.Data;
using ChemSW.Nbt.Sched;
using ChemSW.Core;
using ChemSW.Config;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27880
    /// </summary>
    public class CswUpdateSchema_01T_Case27880 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //add the expired containers scheduled rule to the scheduledrules table
            _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.ExpiredContainers, MtSched.Core.Recurrence.Daily, 1 );

            //create the "NodesProcessedPerIteration" config variable
            _CswNbtSchemaModTrnsctn.createConfigurationVariable(
                Name: CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle,
                Description: "How many nodes are processed at a time in chunking procedures",
                VariableValue: "25",
                IsSystem: false );

            //remove the "NodesPerCycle" row from the scheduledparams table
            CswTableUpdate tu = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "removeNodesPerCycle_27880", "scheduledruleparams" );
            DataTable scheduledparams = tu.getTable( "where paramname = 'NodesPerCycle'" );
            if( 1 == scheduledparams.Rows.Count ) //we should only get one row!
            {
                scheduledparams.Rows[0].Delete();
            }
            tu.update( scheduledparams );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27880; }
        }

        //Update()

    }//class CswUpdateSchema_01T_Case27880

}//namespace ChemSW.Nbt.Schema