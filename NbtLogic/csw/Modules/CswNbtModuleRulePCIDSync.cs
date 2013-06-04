
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the PCID Sync Module
    /// </summary>
    public class CswNbtModuleRulePCIDSync : CswNbtModuleRule
    {
        public CswNbtModuleRulePCIDSync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.PCIDSync; } }
        public override void OnEnable()
        {

        }// OnEnabled

        public override void OnDisable()
        {

        } // OnDisable()

    } // class CswNbtModulePCIDSync
}// namespace ChemSW.Nbt
