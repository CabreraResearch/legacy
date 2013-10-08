using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30879 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30879; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Create and populate receiptLotNo"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            if( null != ReceiptLotOC )
            {

                CswNbtMetaDataObjectClassProp ReceiptLotNoOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ReceiptLotOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassReceiptLot.PropertyName.ReceiptLotNo,
                        FieldType = CswEnumNbtFieldType.Sequence,
                        SetValOnAdd = true,
                        ServerManaged = true,
                    } );

                _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps(); //if we don't do this, .setSequence( SequenceId ) throws an ORNI


                if( null != ReceiptLotNoOCP )
                {
                    int SequenceId = _CswNbtSchemaModTrnsctn.makeSequence( new CswSequenceName( "ReceiptLotNo" ), "RL", "", 6, 1 );

                    foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
                    {
                        ReceiptLotNT.addNameTemplateText( CswNbtObjClassReceiptLot.PropertyName.ReceiptLotNo );
                        ReceiptLotNT.getNodeTypePropByObjectClassProp( ReceiptLotNoOCP ).setSequence( SequenceId );
                        foreach( CswNbtObjClassReceiptLot ReceiptLot in ReceiptLotNT.getNodes( true, true ) )
                        {
                            ReceiptLot.ReceiptLotNo.setSequenceValue();
                            ReceiptLot.postChanges( false );
                        }
                    }
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema