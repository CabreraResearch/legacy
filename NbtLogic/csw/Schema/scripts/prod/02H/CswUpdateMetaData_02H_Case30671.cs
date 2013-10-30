using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30671 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30671; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Add Country prop to Vendors"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp( VendorOC )
                {
                    PropName = CswNbtObjClassVendor.PropertyName.Country,
                    FieldType = CswEnumNbtFieldType.Text
                } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema