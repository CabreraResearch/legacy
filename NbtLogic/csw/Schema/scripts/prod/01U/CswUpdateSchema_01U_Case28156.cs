using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28156
    /// </summary>
    public class CswUpdateSchema_01U_Case28156 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28156; }
        }

        public override void update()
        {
            #region remove the receipt lot objclass from jct_modules_objectclass

            CswNbtMetaDataObjectClass receiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );
            _CswNbtSchemaModTrnsctn.deleteAllModuleObjectClassJunctions( receiptLotOC );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, receiptLotOC.ObjectClassId );

            #endregion

            #region make the receipt lot NTP on containers hidden by default

            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp receiptLotOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ReceiptLot );

            foreach( CswNbtMetaDataNodeTypeProp receiptLotNTP in receiptLotOCP.getNodeTypeProps() )
            {
                receiptLotNTP.removeFromAllLayouts();
            }

            #endregion
        }

        //Update()

    }//class CswUpdateSchemaCase28156

}//namespace ChemSW.Nbt.Schema