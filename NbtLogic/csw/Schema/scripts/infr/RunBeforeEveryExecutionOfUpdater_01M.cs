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

        public override void update()
        {
            // This script is for adding Modules, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region SEBASTIAN


            #endregion SEBASTIAN

            #region TITANIA

            Int32 MlmId = _CswNbtSchemaModTrnsctn.getModuleId( CswNbtModuleName.MLM );
            if( Int32.MinValue == MlmId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Material Life-Cycle Management", CswNbtModuleName.MLM.ToString(), false );
            }
            else if( false == _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                _CswNbtSchemaModTrnsctn.Modules.EnableModule( CswNbtModuleName.MLM );
            }

            #endregion TITANIA


        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01M

}//namespace ChemSW.Nbt.Schema


