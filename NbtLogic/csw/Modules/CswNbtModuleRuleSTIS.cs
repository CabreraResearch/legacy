using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the STIS Module
    /// </summary>
    public class CswNbtModuleRuleSTIS : CswNbtModuleRule
    {
        public CswNbtModuleRuleSTIS( CswNbtResources CswNbtResources ) : 
            base( CswNbtResources ) 
        { 
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.STIS; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleSTIS
}// namespace ChemSW.Nbt
