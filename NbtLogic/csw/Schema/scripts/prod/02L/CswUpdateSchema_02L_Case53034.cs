using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case53034 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 53034; }
        }

        public override void update()
        {
            // If present, clean up old action from 31611
            _CswNbtSchemaModTrnsctn.deleteAction( "Batch_Edit" );

            // Add 'Bulk Edit' action
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Bulk_Edit, true, string.Empty, "System" );

        } // update()

    } // class CswUpdateSchema_02L_Case31611

}//namespace ChemSW.Nbt.Schema