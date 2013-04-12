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

            #region ASPEN

            _makeFireDbSyncModule( CswEnumDeveloper.CM, 29245 );

            #endregion ASPEN

            #region BUCKEYE

            #endregion BUCKEYE

        }//Update()

        #region ASPEN Methods

        private void _makeFireDbSyncModule( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            int ModuleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.FireDbSync );
            if( Int32.MinValue == ModuleId )
            {
                // Create the FireDb Sync module
                _CswNbtSchemaModTrnsctn.createModule( "Add-on for Fire Code that syncs FireDb data with ChemCatCentral", CswEnumNbtModuleName.FireDbSync.ToString() );

                // Create the module dependency
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.FireCode, CswEnumNbtModuleName.FireDbSync );
            }

            _resetBlame();
        }

	#endregion ASPEN Methods

	#region BUCKEYE Methods

        #endregion BUCKEYE Methods

    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


