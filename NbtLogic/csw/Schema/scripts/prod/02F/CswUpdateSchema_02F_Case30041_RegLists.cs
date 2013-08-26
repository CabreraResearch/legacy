using ChemSW.MtSched.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

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

        public override void update()
        {
            // Scheduled rule for CAFImports
            _CswNbtSchemaModTrnsctn.createScheduledRule( CswEnumNbtScheduleRuleNames.CAFImport, CswEnumRecurrence.NHours, 1 );

            {
                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, 1, "regulatory_lists", "Regulatory List" );

                ImpMgr.importBinding( "displayname", CswNbtObjClassRegulatoryList.PropertyName.Name, "" );
                ImpMgr.importBinding( "listmode", CswNbtObjClassRegulatoryList.PropertyName.ListMode, "" );

                ImpMgr.finalize( WhereClause: " lower(listmode)='cispro' " );
            }

            {
                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, 1, "regulated_casnos", "Regulatory List CAS" );

                ImpMgr.importBinding( "casno", CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo, "" );
                //TODO: Add reg list relationship
                ImpMgr.finalize();
            }

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema