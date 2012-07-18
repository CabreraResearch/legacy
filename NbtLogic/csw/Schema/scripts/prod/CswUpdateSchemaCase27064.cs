using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27064
    /// </summary>
    public class CswUpdateSchemaCase27064 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataNodeType containerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != containerNT )
            {
                CswNbtMetaDataNodeTypeProp barcodeNTP = containerNT.getNodeTypeProp( CswNbtObjClassContainer.BarcodePropertyName );
                if( null != barcodeNTP )
                {
                    barcodeNTP.setIsUnique( true );
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase27064

}//namespace ChemSW.Nbt.Schema