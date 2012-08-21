using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Mobile Module
    /// </summary>
    public class CswNbtModuleRuleMobile : CswNbtModuleRule
    {
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.Mobile; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleMobile
}// namespace ChemSW.Nbt
