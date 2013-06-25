
using System.Collections.Generic;

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

        public virtual CswEnumNbtModuleName ModuleName
        {
            get { return CswEnumNbtModuleName.Unknown; }
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();

        public void Enable()
        {
            if( _CswNbtResources.Modules.ModuleHasPrereq( this.ModuleName ) )
            {
                CswEnumNbtModuleName modulePrereq = _CswNbtResources.Modules.GetModulePrereq( this.ModuleName );
                _CswNbtResources.Modules.EnableModule( modulePrereq );
            }
            OnEnable();
        }

        public void Disable()
        {
            IEnumerable<CswEnumNbtModuleName> childModules = _CswNbtResources.Modules.GetChildModules( this.ModuleName );
            foreach( CswEnumNbtModuleName childModule in childModules )
            {
                _CswNbtResources.Modules.DisableModule( childModule );
            }
            OnDisable();
        }

    } // interface ICswNbtModuleRule
}// namespace ChemSW.Nbt
