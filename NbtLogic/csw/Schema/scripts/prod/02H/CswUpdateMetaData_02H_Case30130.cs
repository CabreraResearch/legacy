using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30130 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {                
            get { return 30130; }
        }

        public override string Title
        {
            get { return "Receipt Lot Required"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp ReceiptLotOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ReceiptLot );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReceiptLotOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema