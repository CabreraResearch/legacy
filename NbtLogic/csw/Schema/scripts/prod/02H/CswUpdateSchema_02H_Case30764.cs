using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30764 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30764; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Set vendorname and vendortype to compound unique"; }
        }

        public override void update()
        {
            // Vendorname and VendorType should be compound unique
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );

            // VendorName
            CswNbtMetaDataObjectClassProp VendorNameOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( VendorNameOCP, CswEnumNbtObjectClassPropAttributes.isunique, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( VendorNameOCP, CswEnumNbtObjectClassPropAttributes.iscompoundunique, true );

            //Vendortype
            CswNbtMetaDataObjectClassProp VendorTypeOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( VendorTypeOCP, CswEnumNbtObjectClassPropAttributes.iscompoundunique, true );

        } // update()
    }

}//namespace ChemSW.Nbt.Schema