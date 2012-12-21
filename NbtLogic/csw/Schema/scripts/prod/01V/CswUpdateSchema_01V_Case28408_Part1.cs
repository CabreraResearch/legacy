using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28408_Part1
    /// </summary>
    public class CswUpdateSchema_01V_Case28408_Part1 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28408; }
        }

        public override void update()
        {
            // Part 1: Set the sequence for the barcode property
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );

            foreach( CswNbtMetaDataNodeType UserOCNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp barcodeNTP = UserOCNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Barcode );
                if( null != barcodeNTP )
                {
                    if( Int32.MinValue == barcodeNTP.SequenceId )
                    {
                        int userOCBarcodeSequenceId = _CswNbtSchemaModTrnsctn.makeSequence( new CswSequenceName( "User Barcode" ), "U", "", 6, 0 );
                        barcodeNTP.setSequence( userOCBarcodeSequenceId );
                    }
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28408_Part1

}//namespace ChemSW.Nbt.Schema