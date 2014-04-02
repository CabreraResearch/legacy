using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52735: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52735; }
        }

        public override string Title
        {
            get { return "Create Direct Structure Search Module"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createModule( "Accelrys Direct Structure Search", CswEnumNbtModuleName.DirectStructureSearch, false );
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.CISPro, CswEnumNbtModuleName.DirectStructureSearch );
        }
    }
}