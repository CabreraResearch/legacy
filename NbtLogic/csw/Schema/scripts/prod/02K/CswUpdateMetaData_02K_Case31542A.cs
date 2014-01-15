using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31542A : CswUpdateSchemaTo
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
            return "A";
        }

        public override string Title
        {
            get { return "Add new C3 ACD module"; }
        }

        public override void update()
        {
            if( Int32.MinValue == _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.C3ACD ) )
            {
                _CswNbtSchemaModTrnsctn.createModule( "C3 ACD Module", CswEnumNbtModuleName.C3ACD, false ); // C3ACD disabled by default
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.C3, CswEnumNbtModuleName.C3ACD );
            }
        } // update()

    }//class CswUpdateMetaData_02K_Case31542A

}//namespace ChemSW.Nbt.Schema