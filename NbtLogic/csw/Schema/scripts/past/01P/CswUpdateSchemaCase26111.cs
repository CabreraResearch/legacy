using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26111
    /// </summary>
    public class CswUpdateSchemaCase26111 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //// Insert new parameters for CswScheduleLogicNbtGenNode
            //CswTableSelect RulesSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "26111_rules_select", "scheduledrules" );
            //DataTable RulesTable = RulesSelect.getTable("where rulename = '"+NbtScheduleRuleNames.GenNode.ToString()+"'");
            //if( RulesTable.Rows.Count > 0 )
            //{
            //    Int32 ScheduledRuleId = CswConvert.ToInt32( RulesTable.Rows[0]["scheduledruleid"] );
            //    CswTableUpdate ParamsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "26111_params_update", "scheduledruleparams" );
            //    DataTable ParamsTable = ParamsUpdate.getEmptyTable();

            //    DataRow NewRow1 = ParamsTable.NewRow();
            //    NewRow1["scheduledruleid"] = ScheduledRuleId;
            //    NewRow1["paramname"] = CswScheduleLogicNbtGenNode._ParamName_GeneratorLimit;
            //    NewRow1["paramval"] = "1";
            //    ParamsTable.Rows.Add( NewRow1 );

            //    DataRow NewRow2 = ParamsTable.NewRow();
            //    NewRow2["scheduledruleid"] = ScheduledRuleId;
            //    NewRow2["paramname"] = CswScheduleLogicNbtGenNode._ParamName_GeneratorTargetLimit;
            //    NewRow2["paramval"] = "5";
            //    ParamsTable.Rows.Add( NewRow2 );

            //    ParamsUpdate.update( ParamsTable );
            //} // if( RulesTable.Rows.Count > 0 )


            
            // Config Vars for Generators
            _CswNbtSchemaModTrnsctn.createConfigurationVariable(
                CswNbtResources.ConfigurationVariables.generatorlimit.ToString(),
                "Number of Generators to process in each scheduler cycle",
                "1",
                false );

            _CswNbtSchemaModTrnsctn.createConfigurationVariable( 
                CswNbtResources.ConfigurationVariables.generatortargetlimit.ToString(), 
                "Number of Targets to generate from a Generator in each scheduler cycle", 
                "5", 
                false);

        }//Update()

    }//class CswUpdateSchemaCase26111

}//namespace ChemSW.Nbt.Schema