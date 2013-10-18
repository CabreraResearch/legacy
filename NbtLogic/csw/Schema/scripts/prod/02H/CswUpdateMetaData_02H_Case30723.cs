using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

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

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
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
                        ReceiptLotNTP.updateLayout( CswEnumNbtLayoutType.Edit, true );
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