
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
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.C3; } }
        protected override void OnEnable()
        {
            // When C3 is enabled, dislay the following
            //   C3 Search option in the search menu
            //   Link to the C3 search on universal search results page

        }// OnEnabled

        protected override void OnDisable()
        {
            // When C3 is disabled, don't display the following
            //   C3 Search option in the search menu
            //   Link to the C3 search on universal search results page

        } // OnDisable()

    } // class CswNbtModuleC3
}// namespace ChemSW.Nbt
