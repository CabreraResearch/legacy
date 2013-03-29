using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28535
    /// </summary>
    public class CswUpdateSchema_02A_Case28535 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 28535; }
        }

        public override void update()
        {
            // Make the VendorName property unique
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            if( null != VendorOC )
            {
                CswNbtMetaDataObjectClassProp VendorNameOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
                if( null != VendorNameOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( VendorNameOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isunique, true );
                }
            }
        } // update()

    }//class CswUpdateSchema_02A_Case28535

}//namespace ChemSW.Nbt.Schema