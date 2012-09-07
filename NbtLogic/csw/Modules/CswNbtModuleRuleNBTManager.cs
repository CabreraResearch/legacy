using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the NBTManager Module
    /// </summary>
    public class CswNbtModuleRuleNBTManager : CswNbtModuleRule
    {
        public CswNbtModuleRuleNBTManager( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.NBTManager; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleNBTManager
}// namespace ChemSW.Nbt
