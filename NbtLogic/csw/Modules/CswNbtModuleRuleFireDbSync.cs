
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the FireDb Sync Module
    /// </summary>
    public class CswNbtModuleRuleFireDbSync: CswNbtModuleRule
    {
        public CswNbtModuleRuleFireDbSync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.FireDbSync; } }
        protected override void OnEnable()
        {
        }// OnEnabled

        protected override void OnDisable()
        {
        } // OnDisable()

    } // class CswNbtModuleFireDbSync
}// namespace ChemSW.Nbt
