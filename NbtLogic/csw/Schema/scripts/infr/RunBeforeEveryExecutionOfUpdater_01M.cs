using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for Modules
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01M : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Modules";

        #region Blame Logic

        private void _acceptBlame( CswEnumDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswEnumDeveloper.NBT;
            _CaseNo = 0;
        }

        private CswEnumDeveloper _Author = CswEnumDeveloper.NBT;

        public override CswEnumDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        #endregion Blame Logic

        public override void update()
        {
            // This script is for adding Modules, which often become required by other business logic and can cause prior scripts to fail.

            #region CEDAR

            _createPCIDSyncModule( CswEnumDeveloper.CM, 29566 );
            _createCofAModule( CswEnumDeveloper.BV, 29563 );

            #endregion CEDAR

            #region DOGWOOD

            #endregion DOGWOOD

        }//Update()

        #region CEDAR Methods

        private void _createPCIDSyncModule( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            int ModuleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.PCIDSync );
            if( Int32.MinValue == ModuleId )
            {
                // Create the PCID Sync module
                _CswNbtSchemaModTrnsctn.createModule( "When enabled, PCID data is synced with ChemCatCentral", CswEnumNbtModuleName.PCIDSync.ToString(), false );
            }

            _resetBlame();
        }
        
        private void _createCofAModule( CswEnumDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            Int32 CofAModuleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.CofA );
            if( Int32.MinValue == CofAModuleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Certificate of Analysis", CswEnumNbtModuleName.CofA.ToString(), false );
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.Containers, CswEnumNbtModuleName.CofA );
            }

            _resetBlame();
        }


        #endregion CEDAR Methods

        #region DOGWOOD Methods

        #endregion DOGWOOD Methods


    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


