using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02M_CIS49554A : CswUpdateSchemaTo
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
            get { return "Add moduleid, datatype, and constraint column to configvariables table"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.addNumberColumn( "configuration_variables", "moduleid", "id of the module this config var is related to. Leave blank if common or system config var", false, "", false, "", false);
            _CswNbtSchemaModTrnsctn.addStringColumn( "configuration_variables", "datatype", "type of the data contained. values can be INT, BOOL, LIST or STRING", true, 20);
            _CswNbtSchemaModTrnsctn.addStringColumn( "configuration_variables", "constraint", "Constraints on the config value, which are applied depending on the datatype",true, 20);
        }
    }
}


