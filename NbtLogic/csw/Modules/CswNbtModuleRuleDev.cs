
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Dev Module
    /// </summary>
    public class CswNbtModuleRuleDev : CswNbtModuleRule
    {
        public CswNbtModuleRuleDev( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.Dev; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleRuleDev
}// namespace ChemSW.Nbt
