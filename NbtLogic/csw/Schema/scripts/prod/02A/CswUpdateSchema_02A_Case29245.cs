using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29245
    /// </summary>
    public class CswUpdateSchema_02A_Case29245 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29245; }
        }

        public override void update()
        {
            // Create the FireDb Sync module
            _CswNbtSchemaModTrnsctn.createModule( "Add-on for Fire Code that syncs FireDb data with ChemCatCentral", CswNbtModuleName.FireDbSync.ToString() );

            // Create the module dependency
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.FireCode, CswNbtModuleName.FireDbSync );

        } // update()

    }//class CswUpdateSchema_02A_Case29245

}//namespace ChemSW.Nbt.Schema