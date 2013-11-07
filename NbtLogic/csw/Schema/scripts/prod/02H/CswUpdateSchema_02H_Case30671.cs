using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30671 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30671; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override string Title
        {
            get { return "Adjust Vendor layouts"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            foreach( CswNbtMetaDataNodeType VendorNT in VendorOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp VendorName = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
                VendorName.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 1, DisplayColumn: 1 );
                VendorName.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp Street1 = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Street1 );
                Street1.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 2, DisplayColumn: 1 );
                Street1.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp Street2 = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Street2 );
                Street2.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 3, DisplayColumn: 1 );
                Street2.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp City = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.City );
                City.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 4, DisplayColumn: 1 );
                City.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp State = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.State );
                State.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 5, DisplayColumn: 1 );
                State.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 5, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp Zip = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Zip );
                Zip.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 6, DisplayColumn: 1 );
                Zip.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 6, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp Country = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Country );
                Country.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 7, DisplayColumn: 1 );
                Country.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 7, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp Phone = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Phone );
                Phone.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 1, DisplayColumn: 2 );
                Phone.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 2 );

                CswNbtMetaDataNodeTypeProp Fax = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.Fax );
                Fax.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 2, DisplayColumn: 2 );
                Fax.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 2, DisplayColumn: 2 );

                CswNbtMetaDataNodeTypeProp ContactName = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.ContactName );
                ContactName.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 3, DisplayColumn: 2 );
                ContactName.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 3, DisplayColumn: 2 );

                CswNbtMetaDataNodeTypeProp DeptBillCode = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.DeptBillCode );
                DeptBillCode.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 4, DisplayColumn: 2 );
                DeptBillCode.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 4, DisplayColumn: 2 );

                CswNbtMetaDataNodeTypeProp AccountNo = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.AccountNo );
                AccountNo.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 5, DisplayColumn: 2 );
                AccountNo.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 5, DisplayColumn: 2 );

                CswNbtMetaDataNodeTypeProp VendorTypeName = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                VendorTypeName.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 6, DisplayColumn: 2 );
                VendorTypeName.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 6, DisplayColumn: 2 );

                CswNbtMetaDataNodeTypeProp CorporateEntityName = VendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.CorporateEntityName );
                CorporateEntityName.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: VendorNT.getFirstNodeTypeTab().TabId, DisplayRow: 7, DisplayColumn: 2 );
                CorporateEntityName.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 7, DisplayColumn: 2 );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema