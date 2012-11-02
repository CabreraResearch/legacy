using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27867
    /// </summary>
    public class CswUpdateSchema_01T_Case27867 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass receiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );

            CswNbtMetaDataNodeType receiptLotNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Receipt Lot" );
            if( null == receiptLotNT )
            {
                receiptLotNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( receiptLotOC.ObjectClassId, "Receipt Lot", "MLM" );

                /* Name template text should be Receipt Lot No - waiting on 27877 */

            }

            //create receipt lots view
            CswNbtView receiptLotsView = _CswNbtSchemaModTrnsctn.restoreView( "Receipt Lots" );
            if( null == receiptLotsView )
            {
                receiptLotsView = _CswNbtSchemaModTrnsctn.makeNewView( "Receipt Lots", NbtViewVisibility.Global );
                receiptLotsView.Category = "MLM (demo)";
                receiptLotsView.ViewMode = NbtViewRenderingMode.Tree;
                receiptLotsView.AddViewRelationship( receiptLotOC, true );
                receiptLotsView.IsDemo = false;
                receiptLotsView.save();
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27867; }
        }

        //Update()

    }//class CswUpdateSchema_01T_Case27867

}//namespace ChemSW.Nbt.Schema