
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the C3 Module
    /// </summary>
    public class CswNbtModuleRuleC3 : CswNbtModuleRule
    {
        public CswNbtModuleRuleC3( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.C3; } }
        public override void OnEnable()
        {
            // The C3 module can only be enabled if the CISPro module is enabled.
            if( false == ( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
            }

            // When C3 is enabled, dislay the following
            //   C3 Search option in the search menu
            //   Link to the C3 search on universal search results page

        }// OnEnabled

        public override void OnDisable()
        {
            // When C3 is disabled, don't display the following
            //   C3 Search option in the search menu
            //   Link to the C3 search on universal search results page

        } // OnDisable()

    } // class CswNbtModuleC3
}// namespace ChemSW.Nbt
