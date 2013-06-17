
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CCPro Module
    /// </summary>
    public class CswNbtModuleRuleCCPro: CswNbtModuleRule
    {
        public CswNbtModuleRuleCCPro( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.CCPro; } }
        protected override void OnEnable()
        {
        }
        protected override void OnDisable()
        {
        }

    } // class CswNbtModuleCCPro
}// namespace ChemSW.Nbt
