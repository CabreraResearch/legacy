using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31236B: CswUpdateSchemaTo
    {
        public override string Title { get { return "Add Fire Reporting bindings to Chemical"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31236; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.importBinding( "classes", CswNbtObjClassChemical.PropertyName.HazardClasses, "", DestNodeTypeName : "Chemical" );
            ImpMgr.importBinding( "categories", CswNbtObjClassChemical.PropertyName.HazardClasses, "", DestNodeTypeName : "Chemical" );
            ImpMgr.importBinding( "chemtype", CswNbtObjClassChemical.PropertyName.MaterialType, "", DestNodeTypeName : "Chemical" );
            ImpMgr.importBinding( "special_flags", CswNbtObjClassChemical.PropertyName.SpecialFlags, "", DestNodeTypeName : "Chemical" );
            
            ImpMgr.finalize();

        }

    }
}