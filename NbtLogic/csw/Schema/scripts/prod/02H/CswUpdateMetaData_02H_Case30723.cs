using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30723 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30723; }
        }

        public override string Title
        {
            get { return "Receipt Lot NTP no longer Hidden"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ReceiptLotNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReceiptLot );
                if( null != ReceiptLotNTP )
                {
                    ReceiptLotNTP.Hidden = false;
                    if( _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswEnumNbtModuleName.MLM ) )
                    {
                        CswNbtMetaDataNodeTypeProp DateCreatedNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.DateCreated );
                        ReceiptLotNTP.updateLayout( CswEnumNbtLayoutType.Edit, DateCreatedNTP, true );
                    }
                    else
                    {
                        ReceiptLotNTP.removeFromAllLayouts();
                    }
                }
            }
        }

    }

}//namespace ChemSW.Nbt.Schema