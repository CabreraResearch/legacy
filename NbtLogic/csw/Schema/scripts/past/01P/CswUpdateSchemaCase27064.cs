using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27064
    /// </summary>
    public class CswUpdateSchemaCase27064 : CswUpdateSchemaTo
    {
        public override void update()
        {
            HashSet<string> barcodesInSystem = new HashSet<string>();

            CswNbtMetaDataNodeType containerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != containerNT )
            {
                //check for any duplicate barcodes and make them unique
                foreach( CswNbtNode curContainer in containerNT.getNodes( false, true ) )
                {
                    CswNbtNodePropBarcode barCode = curContainer.Properties[CswNbtMetaDataFieldType.NbtFieldType.Barcode].AsBarcode;
                    while( barcodesInSystem.Contains( barCode.Barcode ) )
                    {
                        barCode.setBarcodeValueOverride( "", false ); //clear the barcode
                        barCode.setBarcodeValue(); //then let it go to the next sequence so it's %100 unique
                    }
                    barcodesInSystem.Add( barCode.Barcode );
                    curContainer.postChanges( false );
                }

                //make the barcode prop unique
                CswNbtMetaDataNodeTypeProp barcodeNTP = containerNT.getNodeTypeProp( CswNbtObjClassContainer.BarcodePropertyName );
                if( null != barcodeNTP )
                {
                    barcodeNTP.setIsUnique( true );
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase27064

}//namespace ChemSW.Nbt.Schema