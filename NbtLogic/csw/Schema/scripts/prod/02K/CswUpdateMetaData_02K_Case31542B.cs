using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31542B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31542; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override string Title
        {
            get { return "Add new C3 Products module; enable it if C3 is already enabled"; }
        }

        public override void update()
        {
            if( Int32.MinValue == _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.C3Products ) )
            {
                _CswNbtSchemaModTrnsctn.createModule( "C3 Products Module", CswEnumNbtModuleName.C3Products, false ); // C3Products disabled by default
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.C3, CswEnumNbtModuleName.C3Products );
            }

            // For customers who already have C3 enabled, we enable C3 Products automatically
            if( _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3 ) )
            {
                _CswNbtSchemaModTrnsctn.Modules.EnableModule( CswEnumNbtModuleName.C3Products );
            }

        } // update()

    }//class CswUpdateMetaData_02K_Case31542B

}//namespace ChemSW.Nbt.Schema