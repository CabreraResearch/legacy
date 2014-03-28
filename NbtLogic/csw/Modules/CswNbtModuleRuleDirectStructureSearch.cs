namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the ACCL Direct Structure Search Module
    /// </summary>
    /// <remarks>
    /// Enables structure search and image rendering using the Accelrys Direct cartridge
    /// </remarks>
    public class CswNbtModuleRuleDirectStructureSearch : CswNbtModuleRule
    {
        public CswNbtModuleRuleDirectStructureSearch( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.DirectStructureSearch; } }

        protected override void OnEnable()
        {
            
        }

        protected override void OnDisable()
        {
            
        } // OnDisable()

    } // class CswNbtModuleRuleDirectStructureSearch
}// namespace ChemSW.Nbt
