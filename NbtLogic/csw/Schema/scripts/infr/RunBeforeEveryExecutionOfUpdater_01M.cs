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

        private Int32 _CaseNo = 0;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        public override void update()
        {
            // This script is for adding Modules, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region TITANIA

            Int32 MlmId = _CswNbtSchemaModTrnsctn.getModuleId( CswNbtModuleName.MLM );
            if( Int32.MinValue == MlmId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Material Life-Cycle Management", CswNbtModuleName.MLM.ToString(), false );
            }
            else if( _CswNbtSchemaModTrnsctn.isMaster() && false == _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                _CswNbtSchemaModTrnsctn.Modules.EnableModule( CswNbtModuleName.MLM );
            }

            #endregion TITANIA

            #region URSULA

            _CswNbtSchemaModTrnsctn.deleteModule( "mobile" );

            #endregion URSULA

        }



        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


