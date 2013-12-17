using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31264B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31264; }
        }

        public override string Title
        {
            get { return "Replace Size UPC Barcode with UPC"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            foreach( CswNbtMetaDataNodeType SizeNT in SizeOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UPCNTP = SizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.UPC );
                UPCNTP.removeFromAllLayouts();
            }
            CswNbtMetaDataObjectClassProp UPCBarcodeOCP = SizeOC.getObjectClassProp( "UPC Barcode" );
            if( null != UPCBarcodeOCP )
            {
                foreach( CswNbtObjClassSize SizeNode in SizeOC.getNodes( false, false, false ) )
                {
                    if( null != SizeNode.Node.Properties["UPC Barcode"] )
                    {
                        SizeNode.UPC.Text = SizeNode.Node.Properties["UPC Barcode"].AsBarcode.Barcode;
                        SizeNode.postChanges(false);
                    }
                }
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( UPCBarcodeOCP, true );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema