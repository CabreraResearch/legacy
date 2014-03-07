using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case31893_Constituents : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31893; }
        }

        public override string Title
        {
            get { return "CAF: Constituent Bindings"; }
        }

        public override string AppendToScriptName()
        {
            return "D";
        }

        public override void update()
        {
            // CAF bindings definitions for Biologicals
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
            ImpMgr.CAFimportOrder( "Constituent", "materials", "constituents_view", "legacyid" );

            //Simple Props
            ImpMgr.importBinding( "name", CswNbtObjClassChemical.PropertyName.TradeName, "" );
            ImpMgr.importBinding( "casno", CswNbtObjClassChemical.PropertyName.CasNo, "" );
            ImpMgr.importBinding( "einecs", CswNbtObjClassChemical.PropertyName.EINECS, "" );

            ImpMgr.finalize();
        } // update()

    }

}//namespace ChemSW.Nbt.Schema