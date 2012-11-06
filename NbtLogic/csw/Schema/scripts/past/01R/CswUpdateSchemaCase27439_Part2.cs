using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27439
    /// </summary>
    public class CswUpdateSchemaCase27439_Part2 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            /* Get the Vendor NT and hide the newly created properties */
            CswNbtMetaDataNodeType vendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
            if( null != vendorNT )
            {
                CswNbtMetaDataNodeTypeProp corporateEntityNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.CorporateEntityName );
                corporateEntityNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp vendorTypeNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                vendorTypeNTP.removeFromAllLayouts();
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema