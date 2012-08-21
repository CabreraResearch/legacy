using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CCPro Module
    /// </summary>
    public class CswNbtModuleRuleCCPro : CswNbtModuleRule
    {
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.CCPro; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleCCPro
}// namespace ChemSW.Nbt
