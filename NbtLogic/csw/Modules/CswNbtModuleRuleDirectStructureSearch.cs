using ChemSW.Exceptions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the ACCL Direct Structure Search Module
    /// </summary>
    /// <remarks>
    /// Enables structure search and image rendering using the Accelrys Direct cartridge
    /// </remarks>
    public class CswNbtModuleRuleDirectStructureSearch: CswNbtModuleRule
    {
        public CswNbtModuleRuleDirectStructureSearch( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.DirectStructureSearch; } }

        protected override void OnEnable()
        {
            //Prevent users from enabling the module if Direct is not installed
            if( false == _isDirectInstalled() )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot enable the Direct Structure Search module because Direct is not installed",
                    "An error was thrown when attempted to get the version of Direct indicating that Direct is not installed" );
            }
        }

        protected override void OnDisable()
        {

        } // OnDisable()

        /// <summary>
        /// Checks if direct is installed. Selects the Direct version from dual and if any exceptions are thrown, returns false
        /// </summary>
        private bool _isDirectInstalled()
        {
            bool ret = true;
            try
            {
                _CswNbtResources.execArbitraryPlatformNeutralSql( "SELECT MDLAUX.VERSION FROM DUAL" );
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

    } // class CswNbtModuleRuleDirectStructureSearch
}// namespace ChemSW.Nbt
