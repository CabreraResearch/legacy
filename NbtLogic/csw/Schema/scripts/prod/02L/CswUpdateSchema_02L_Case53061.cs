using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case53061 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 53061; }
        }

        public override string Title
        {
            get { return "Update Control Zone Name CAF binding"; }
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.removeImportBinding( "CAF", "controlzonename", "Control Zone", "Name", "Text" );
            ImpMgr.importBinding( "controlzonename", CswNbtObjClassControlZone.PropertyName.ControlZoneName, "", "CAF", "Control Zone" );

            ImpMgr.finalize();

        } // update()

    }

}//namespace ChemSW.Nbt.Schema