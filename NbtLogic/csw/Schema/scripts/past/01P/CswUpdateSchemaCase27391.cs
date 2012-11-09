using System;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27391
    /// </summary>
    public class CswUpdateSchemaCase27391 : CswUpdateSchemaTo
    {
        public override void update()
        {
            Int32 ContainerBarcodeId = _CswNbtSchemaModTrnsctn.makeSequence( new CswSequenceName( "Container Barcode" ), "", "", 6, 1 );
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp BarcodeNtp = ContainerNt.getBarcodeProperty();
                BarcodeNtp.setSequence( ContainerBarcodeId );
            }

        }//Update()

    }//class CswUpdateSchemaCase27391

}//namespace ChemSW.Nbt.Schema