using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case30825 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30825; }
        }

        public override string Title
        {
            get { return "Create the Ariel Sync module"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createModule( "Add-on for Regulatory Lists thats syncs with the regulation database 'Ariel.'", CswEnumNbtModuleName.ArielSync, false );
            // Make it dependent on the Regulatory Lists module
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.RegulatoryLists, CswEnumNbtModuleName.ArielSync );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema