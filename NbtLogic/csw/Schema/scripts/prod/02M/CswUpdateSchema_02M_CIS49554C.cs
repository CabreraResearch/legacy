using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateSchema_02M_CIS49554C : CswUpdateSchemaTo
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
            get { return "delete unused config vars brand_pagetitle and brand_pageicon" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            //constants
            const string brand_pagetitle = "brand_pagetitle";
            const string brand_pageicon = "brand_pageicon";

            CswTableUpdate UpdateConfigVarsTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "populate moduleID, type and minvar columns in config_variables table CIS:49554B", "configuration_variables" );

            //get entire config vars table
            DataTable ConfigVarsDataTable = UpdateConfigVarsTable.getTable();

            //update the configVarsTable datatable according to the 
            //data in the rowsToUpdate dict
            foreach( DataRow thisRow in ConfigVarsDataTable.Rows )
            {
                string thisConfigVarName = thisRow["variablename"].ToString();

                if( thisConfigVarName == brand_pageicon ||
                    thisConfigVarName == brand_pagetitle )
                {
                    thisRow["DELETED"] = "1";
                }
            }

            UpdateConfigVarsTable.update( ConfigVarsDataTable );

            _CswNbtSchemaModTrnsctn.commitTransaction();
        }
    }
}