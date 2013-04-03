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

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
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

            #region ASPEN

            _makeFireDbSyncModule( CswDeveloper.CM, 29245 );

            #endregion ASPEN

        }//Update()

        #region ASPEN Methods

        private void _makeFireDbSyncModule( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            int ModuleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswNbtModuleName.FireDbSync );
            if( Int32.MinValue == ModuleId )
            {
                // Create the FireDb Sync module
                _CswNbtSchemaModTrnsctn.createModule( "Add-on for Fire Code that syncs FireDb data with ChemCatCentral", CswNbtModuleName.FireDbSync.ToString() );

                // Create the module dependency
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.FireCode, CswNbtModuleName.FireDbSync );
            }

            _resetBlame();
        }


        #endregion ASPEN Methods

    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


