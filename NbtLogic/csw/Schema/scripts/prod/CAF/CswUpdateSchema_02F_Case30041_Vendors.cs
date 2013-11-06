﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_Vendors : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        public override string AppendToScriptName()
        {
            return "02F_Case30041_Vendors";
        }

        public override void update()
        {
            // CAF bindings definitions for Vendors
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "vendors", "Vendor" );

            // Binding
            ImpMgr.importBinding( "accountno", CswNbtObjClassVendor.PropertyName.AccountNo, "" );
            ImpMgr.importBinding( "city", CswNbtObjClassVendor.PropertyName.City, "" );
            ImpMgr.importBinding( "contactname", CswNbtObjClassVendor.PropertyName.ContactName, "" );
            ImpMgr.importBinding( "fax", CswNbtObjClassVendor.PropertyName.Fax, "" );
            ImpMgr.importBinding( "phone", CswNbtObjClassVendor.PropertyName.Phone, "" );
            ImpMgr.importBinding( "state", CswNbtObjClassVendor.PropertyName.State, "" );
            ImpMgr.importBinding( "street1", CswNbtObjClassVendor.PropertyName.Street1, "" );
            ImpMgr.importBinding( "street2", CswNbtObjClassVendor.PropertyName.Street2, "" );
            ImpMgr.importBinding( "vendorname", CswNbtObjClassVendor.PropertyName.VendorName, "" );
            ImpMgr.importBinding( "zip", CswNbtObjClassVendor.PropertyName.Zip, "" );

            // Relationship
            // Vendor has no relationships

            ImpMgr.finalize();
        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema