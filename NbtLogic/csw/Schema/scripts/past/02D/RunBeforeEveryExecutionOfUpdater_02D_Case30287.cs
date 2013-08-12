using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30287
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30287 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30287; }
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                _CswNbtSchemaModTrnsctn.Modules.DisableModule( CswEnumNbtModuleName.FireDbSync );
            }

        } //Update()

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30287
}//namespace ChemSW.Nbt.Schema


