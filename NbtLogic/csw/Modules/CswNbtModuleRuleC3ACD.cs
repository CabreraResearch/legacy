namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the C3 ACD Module -- a child of C3
    /// </summary>
    public class CswNbtModuleRuleC3ACD : CswNbtModuleRule
    {
        public CswNbtModuleRuleC3ACD( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.C3ACD; } }
        protected override void OnEnable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3Products ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.C3Products );
            }

            // When enabled:
            //      a. C3 search dialog shows 'ANY' and 'Preferred' for list of data sources
            //      b. C3 search dialog allows user to set preferred suppliers
            //      c. Web service to get list of suppliers from ACD can be used

        }// OnEnable()

        protected override void OnDisable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3Products ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.C3 );
            }

        }// OnDisable()

    } // class CswNbtModuleC3ACD
}// namespace ChemSW.Nbt