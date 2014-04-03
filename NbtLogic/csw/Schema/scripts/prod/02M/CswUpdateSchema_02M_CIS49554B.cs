using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateSchema_02M_CIS49554B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 49554; }
        }

        public override string Title
        {
            get { return "populate moduleID, type, and minvar columns in configuration_variables table" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            //constants
            const string INTTYPE = "INT";
            const string STRINGTYPE = "STRING";
            const string BOOLTYPE = "BOOL";
            const string LISTTYPE = "LIST";

            const string COL_MODULEID = "moduleid";
            const string COL_DATATYPE = "datatype";
            const string COL_CONSTRAINT = "constraint";

            string MODULEID_CONTAINERS = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.Containers ).ToString();
            string MODULEID_C3 = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.C3 ).ToString();
            string MODULEID_SI = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.SI ).ToString();
            string MODULEID_CHEMWATCH = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.ChemWatch ).ToString();

            //Dictionary whose key is the name of the config var to add
            //and whose value is a 3-tuple, whose values are
            //moduleid, datatype, minvalue
            Dictionary<string,Tuple<string, string, string>> rowsToUpdate = new Dictionary<string, Tuple<string, string, string>>();

            //populate the rowsToUpdateDict
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.passwordexpiry_days.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.failedloginlimit.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.password_length.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "5") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.password_complexity.ToString(), Tuple.Create<string, string, string>(null, LISTTYPE, "0,1,2" ) );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.treeview_resultlimit.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "10") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.is_demo.ToString(), Tuple.Create<string, string, string>(null, BOOLTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.auditing.ToString(), Tuple.Create<string, string, string>(null,BOOLTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.mobileview_resultlim.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.relationshipoptionlimit.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "10") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.batchthreshold.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.generatorlimit.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.generatortargetlimit.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.container_receipt_limit.ToString(), Tuple.Create(MODULEID_CONTAINERS, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.total_comments_lines.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.custom_barcodes.ToString(), Tuple.Create(MODULEID_CONTAINERS, BOOLTYPE, "") );
            rowsToUpdate.Add( CswEnumConfigurationVariableNames.Logging_Level.ToString().ToLower(), Tuple.Create<string, string, string>(null, STRINGTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.netquantity_enforced.ToString(), Tuple.Create(MODULEID_CONTAINERS, BOOLTYPE, "") );
            rowsToUpdate.Add( CswEnumConfigurationVariableNames.NodesProcessedPerCycle.ToString().ToLower(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumConfigurationVariableNames.container_max_depth.ToString(), Tuple.Create(MODULEID_CONTAINERS, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumConfigurationVariableNames.C3_Username.ToString().ToLower(), Tuple.Create(MODULEID_C3, STRINGTYPE, "") );
            rowsToUpdate.Add( CswEnumConfigurationVariableNames.C3_Password.ToString().ToLower(), Tuple.Create(MODULEID_C3, STRINGTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.sql_report_resultlimit.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.miss_outdated_inspections.ToString(), Tuple.Create(MODULEID_SI, BOOLTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.lock_inspection_answer.ToString(), Tuple.Create(MODULEID_SI, BOOLTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.chemwatchusername.ToString(), Tuple.Create(MODULEID_CHEMWATCH, STRINGTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.chemwatchpassword.ToString(), Tuple.Create(MODULEID_CHEMWATCH, STRINGTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.arielmodules.ToString(), Tuple.Create<string, string, string>(null, STRINGTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.chemwatchdomain.ToString(), Tuple.Create(MODULEID_CHEMWATCH, STRINGTYPE, "") );
            rowsToUpdate.Add( CswEnumNbtConfigurationVariables.password_reuse_count.ToString(), Tuple.Create<string, string, string>(null, INTTYPE, "0") );

            CswTableUpdate UpdateConfigVarsTable =  _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "populate moduleID, type and minvar columns in config_variables table CIS:49554B", "configuration_variables" );

            //get entire config vars table
            DataTable ConfigVarsDataTable = UpdateConfigVarsTable.getTable();

            //update the configVarsTable datatable according to the 
            //data in the rowsToUpdate dict
            foreach( DataRow thisRow in ConfigVarsDataTable.Rows)
            {
                string thisConfigVarName = thisRow["variablename"].ToString();

                if( rowsToUpdate.ContainsKey(thisConfigVarName) )
                {
                    Tuple<string,string,string> rowOfDataToInsert = rowsToUpdate[thisConfigVarName];
                    if( rowOfDataToInsert.Item1 != null )
                    {
                        thisRow[COL_MODULEID] = rowOfDataToInsert.Item1;
                    }

                    if( rowOfDataToInsert.Item2 != null )
                    {
                        thisRow[COL_DATATYPE] = rowOfDataToInsert.Item2;
                    }
                    
                    if( rowOfDataToInsert.Item3 != "")
                    {
                        thisRow[COL_CONSTRAINT] = rowOfDataToInsert.Item3;
                    }

                }

            }
                
            UpdateConfigVarsTable.update( ConfigVarsDataTable );

            _CswNbtSchemaModTrnsctn.commitTransaction();
        }
    }
}


