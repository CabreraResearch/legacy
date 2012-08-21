using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the SI Module
    /// </summary>
    public class CswNbtModuleRuleSI : CswNbtModuleRule
    {
        public CswNbtModuleRuleSI( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.SI; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleSI
}// namespace ChemSW.Nbt
