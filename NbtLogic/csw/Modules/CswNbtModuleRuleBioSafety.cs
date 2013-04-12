using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the BioSafety Module
    /// </summary>
    public class CswNbtModuleRuleBioSafety : CswNbtModuleRule
    {
        public CswNbtModuleRuleBioSafety( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.BioSafety; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleBioSafety
}// namespace ChemSW.Nbt
