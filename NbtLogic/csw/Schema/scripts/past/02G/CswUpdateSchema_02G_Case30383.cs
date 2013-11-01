using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30383 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Move Reconciliations to Containers Category"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30383; }
        }

        public override string ScriptName
        {
            get { return "Case30383"; }
        }

        public override void update()
        {
            CswNbtAction Action = _CswNbtSchemaModTrnsctn.Actions[ CswEnumNbtActionName.Reconciliation ];
            Action.SetCategory( CswEnumNbtCategory.Containers );
            
        }
    }
}