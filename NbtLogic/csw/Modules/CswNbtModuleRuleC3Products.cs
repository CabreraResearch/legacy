namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the C3 Products Module -- a child of C3
    /// </summary>
    public class CswNbtModuleRuleC3Products : CswNbtModuleRule
    {
        public CswNbtModuleRuleC3Products( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.C3Products; } }
        protected override void OnEnable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3ACD ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.C3ACD );
            }

            // When enabled:
            //      a.C3 search dialog show data sources per customer
            //      b. C3 search dialog DOESN't allow user to set preferred suppliers
            //      c. Web service to get list of suppliers from ACD CAN'T be used
            //      d. C3ACDPreferredSuppliers can't be accessed
        }// OnEnable()

        protected override void OnDisable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3ACD ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.C3 );
            }

        }// OnDisable()

    } // class CswNbtModuleC3Products
}// namespace ChemSW.Nbt