
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the BioSafety Module
    /// </summary>
    public class CswNbtModuleRuleBioSafety: CswNbtModuleRule
    {
        public CswNbtModuleRuleBioSafety( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.BioSafety; } }
        protected override void OnEnable()
        {

        }

        protected override void OnDisable()
        {

        }

    } // class CswNbtModuleBioSafety
}// namespace ChemSW.Nbt
