using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31098: CswUpdateSchemaTo
    {
        public override string Title { get { return "Add Approved For Receiving to Chemical import bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31098; }
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            //RegList CAS No
            ImpMgr.importBinding( "approved_trans", CswNbtObjClassChemical.PropertyName.ApprovedForReceiving, "", DestNodeTypeName : "Chemical" );

            ImpMgr.finalize();

        }

    }
}