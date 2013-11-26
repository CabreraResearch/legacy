using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31056 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31056; }
        }

        public override string Title
        {
            get { return "Merge Action"; }
        }

        public override void update()
        {
            // Create new action 'Merge'
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Merge, true, "", "System" );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema