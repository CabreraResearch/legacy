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

            #region WILLIAM

            //WILLIAM modules go here.

            #endregion WILLIAM

            #region YORICK

            _makeContainersModule( CswDeveloper.MB, 28902 );
            _makeRegulatoryListsModule( CswDeveloper.MB, 28904 );
            _makeMultiSiteModule( CswDeveloper.MB, 28899 );
            _makeMultiInventoryGroupModule( CswDeveloper.MB, 28901 );

            #endregion YORICK

        }//Update()

        #region Private Methods

        private void _makeContainersModule( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( CswDeveloper.MB, 28902 );
            int moduleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswNbtModuleName.Containers );
            if( Int32.MinValue == moduleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Containers add-on for CISPro", CswNbtModuleName.Containers.ToString(), true );
            }
            _resetBlame();
        }

        private void _makeRegulatoryListsModule( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( CswDeveloper.MB, 28904 );
            int moduleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswNbtModuleName.RegulatoryLists );
            if( Int32.MinValue == moduleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Regulatory lists add-on for CISPro", CswNbtModuleName.RegulatoryLists.ToString(), true );
            }
            _resetBlame();
        }

        private void _makeMultiSiteModule( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            int moduleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswNbtModuleName.MultiSite );
            if( Int32.MinValue == moduleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Allow multiple Sites", CswNbtModuleName.MultiSite.ToString(), false );
            }
            _resetBlame();
        }

        private void _makeMultiInventoryGroupModule( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            int moduleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswNbtModuleName.MultiInventoryGroup );
            if( Int32.MinValue == moduleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Allow multiple Inventory Groups", CswNbtModuleName.MultiInventoryGroup.ToString(), false );
            }
            _resetBlame();
        }

        #endregion

    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


