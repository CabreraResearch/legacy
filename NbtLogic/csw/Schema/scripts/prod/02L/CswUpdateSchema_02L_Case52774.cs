using ChemSW.Nbt.csw.Dev;
using ChemSW.Config;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52774: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52774; }
        }

        public override string Title
        {
            get { return "Set Config Var Defaults on Master"; }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.auditing.ToString(), "1" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue("batchthreshold", "15" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue(CswEnumConfigurationVariableNames.container_max_depth.ToString(), "5");
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.container_receipt_limit.ToString(), "250" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.custom_barcodes.ToString(), "0" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.loc_max_depth.ToString(), "7" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.LocationViewRootName.ToString(), "Root" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.netquantity_enforced.ToString(), "1" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.passwordexpiry_days.ToString(), "90" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.relationshipoptionlimit.ToString(), "50" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.sql_report_resultlimit.ToString(), "50000" );
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumNbtConfigurationVariables.treeview_resultlimit.ToString(), "2000" );
                _CswNbtSchemaModTrnsctn.commitTransaction();
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema