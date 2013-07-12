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

            #region DOGWOOD

            _createLOLISyncModule( CswEnumDeveloper.CM, 30090 );

            #endregion DOGWOOD

        }//Update()


        #region DOGWOOD Methods

        #endregion DOGWOOD Methods

        #region DOGWOOD Methods

        private void _createLOLISyncModule( CswEnumDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            Int32 LOLISyncModuleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.LOLISync );
            if( Int32.MinValue == LOLISyncModuleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "LOLI Sync", CswEnumNbtModuleName.LOLISync.ToString(), false );
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.RegulatoryLists, CswEnumNbtModuleName.LOLISync );
            }

            _resetBlame();
        }

        #endregion DOGWOOD Methods

    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


