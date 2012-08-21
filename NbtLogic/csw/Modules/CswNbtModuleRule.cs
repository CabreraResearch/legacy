using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Module class interface
    /// </summary>
    public abstract class CswNbtModuleRule
    {
        protected CswNbtResources _CswNbtResources;
        public CswNbtModuleRule( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public bool Enabled = false;

        public abstract CswNbtModuleName ModuleName { get; }
        public abstract void OnEnable();
        public abstract void OnDisable();

    } // interface ICswNbtModuleRule
}// namespace ChemSW.Nbt
