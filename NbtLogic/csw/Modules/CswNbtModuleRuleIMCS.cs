using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the IMCS Module
    /// </summary>
    public class CswNbtModuleRuleIMCS : CswNbtModuleRule
    {
        public CswNbtModuleRuleIMCS( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.IMCS; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleIMCS
}// namespace ChemSW.Nbt
