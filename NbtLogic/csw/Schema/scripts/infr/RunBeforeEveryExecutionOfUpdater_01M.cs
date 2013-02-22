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

            #endregion YORICK

        }//Update()

        private void _makeContainersModule( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( CswDeveloper.MB, 28902 );
            int moduleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswNbtModuleName.Containers );
            if( Int32.MinValue == moduleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Containers add-on for CISPro", CswNbtModuleName.Containers.ToString(), false );
            }
            _resetBlame();
        }

    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


