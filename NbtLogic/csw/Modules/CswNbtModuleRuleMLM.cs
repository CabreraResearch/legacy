
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CISPro Module
    /// </summary>
    public class CswNbtModuleRuleMLM : CswNbtModuleRule
    {
        public CswNbtModuleRuleMLM( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.CISPro; } }
        public override void OnEnable()
        {
            _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
        }

        public override void OnDisable()
        {

        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
