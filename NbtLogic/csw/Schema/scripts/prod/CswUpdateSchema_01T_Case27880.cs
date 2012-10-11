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