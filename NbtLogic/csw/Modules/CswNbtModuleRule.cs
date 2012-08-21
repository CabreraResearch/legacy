using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Module class interface
    /// </summary>
    public abstract class CswNbtModuleRule
    {
        public abstract CswNbtModuleName ModuleName { get; }
        
        public bool Enabled = false;

        public abstract void OnEnable();
        public abstract void OnDisable();

    } // interface ICswNbtModuleRule
}// namespace ChemSW.Nbt
