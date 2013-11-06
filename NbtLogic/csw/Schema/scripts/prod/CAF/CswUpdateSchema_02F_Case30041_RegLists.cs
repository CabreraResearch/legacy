﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_RegLists : CswUpdateSchemaTo
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
            return "RegLists";
        }

        public override void update()
        {
            {
                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "regulatory_lists", "Regulatory List", "reglists_view" );

                ImpMgr.importBinding( "displayname", CswNbtObjClassRegulatoryList.PropertyName.Name, "" );
                ImpMgr.importBinding( "listmode", CswNbtObjClassRegulatoryList.PropertyName.ListMode, "" );

                ImpMgr.finalize();
            }

            {
                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "regulated_casnos", "Regulatory List CAS" );

                ImpMgr.importBinding( "casno", CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo, "" );
                //TODO: Add reg list relationship
                ImpMgr.finalize();
            }

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema