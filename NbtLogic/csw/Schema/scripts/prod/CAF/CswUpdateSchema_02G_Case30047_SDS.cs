using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30047_SDS: CswUpdateSchemaTo
    {
        public override string Title { get { return "Setup SDS import bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30047; }
        }

        public override string ScriptName
        {
            get { return "Case30743_SDS"; }
        }

        public override void update()
        {
            // CAF bindings definitions for Vendors
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "documents", "SDS Document", ViewName : "sds_view" ); //PACKAGES not MATERIALS (intentional)

            ImpMgr.importBinding( "acquisitiondate", CswNbtObjClassSDSDocument.PropertyName.AcquiredDate, "" );

            ImpMgr.finalize( UseView : true );

        }
    }
}