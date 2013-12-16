using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_RegLists: CswUpdateNbtMasterSchemaTo
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

        public override void doUpdate()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
            ImpMgr.CAFimportOrder( "Regulatory List", "regulatory_lists", "reglists_view", "regulatorylistid" );
            ImpMgr.importBinding( "displayname", CswNbtObjClassRegulatoryList.PropertyName.Name, "" );
            ImpMgr.importBinding( "listmode", CswNbtObjClassRegulatoryList.PropertyName.ListMode, "" );


            ImpMgr.CAFimportOrder( "Regulatory List CAS", "regulated_casnos", PkColumnName : "regulatedcasnoid" );
            ImpMgr.importBinding( "casno", CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo, "" );

            ImpMgr.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema