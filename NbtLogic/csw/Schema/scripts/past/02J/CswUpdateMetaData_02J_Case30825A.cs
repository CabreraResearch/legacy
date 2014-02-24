using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02J_Case30825A : CswUpdateSchemaTo
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
            get { return "Create Ariel Sync module"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            if( Int32.MinValue == _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.ArielSync ) )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Add-on for Regulatory Lists thats syncs with the regulation database 'Ariel.'", CswEnumNbtModuleName.ArielSync, false );
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.RegulatoryLists, CswEnumNbtModuleName.ArielSync );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema