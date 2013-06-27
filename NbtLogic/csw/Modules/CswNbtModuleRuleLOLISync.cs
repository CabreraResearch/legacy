namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the LOLI SYnc Module
    /// </summary>
    public class CswNbtModuleRuleLOLISync : CswNbtModuleRule
    {
        public CswNbtModuleRuleLOLISync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.LOLISync; } }
        protected override void OnEnable()
        {
        }// OnEnabled

        protected override void OnDisable()
        {
        } // OnDisable()

    } // class CswNbtModuleLOLISync
}// namespace ChemSW.Nbt
