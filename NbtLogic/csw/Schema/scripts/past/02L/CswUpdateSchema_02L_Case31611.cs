using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case31611: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31611; }
        }

        public override void update()
        {
            // Add 'Batch Edit' action
            //_CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Batch_Edit, true, string.Empty, "System" );

            // see CIS-53034

        } // update()

    } // class CswUpdateSchema_02L_Case31611

}//namespace ChemSW.Nbt.Schema