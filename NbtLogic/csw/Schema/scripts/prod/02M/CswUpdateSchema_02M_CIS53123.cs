using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53123 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 53123; }
        }

        public override string Title
        {
            get { return "Set NodesProcessedPerCycle back to 25"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.setConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle.ToString(), "25" );
        }
    }
}