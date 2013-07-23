using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.Services;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for modules management
    /// </summary>
    public class CswNbtWebServiceModules
    {
        private static bool _CanEditModules( CswNbtResources NbtResources )
        {
            return ( NbtResources.CurrentNbtUser.Rolename == CswNbtObjClassRole.ChemSWAdminRoleName );
        }

        private static Collection<CswNbtDataContractModule> _getModules( CswNbtResources NbtResources )
        {
            HashSet<string> seenModules = new HashSet<string>();
            Collection<CswNbtDataContractModule> ret = new Collection<CswNbtDataContractModule>();

            //This is the "root" module - this exists solely for rendering the tree
            CswNbtDataContractModule RootModule = new CswNbtDataContractModule
                {
                    Name = "Modules",
                    Enabled = false //this doesn't really matter because we don't show the root
                };
            ret.Add( RootModule );

            foreach( CswEnumNbtModuleName ModuleName in CswEnumNbtModuleName.All )
            {
                if( CswEnumNbtModuleName.Unknown != ModuleName && false == seenModules.Contains( ModuleName.ToString() ) && false == NbtResources.Modules.ModuleHasPrereq( ModuleName ) )
                {
                    seenModules.Add( ModuleName.ToString() );
                    CswNbtDataContractModule module = _getModuleData( NbtResources, ModuleName );
                    _recurse( NbtResources, module, seenModules );
                    RootModule.ChildModules.Add( module );
                }
            }
            return ret;
        }

        private static void _recurse( CswNbtResources NbtResources, CswNbtDataContractModule Module, HashSet<string> seenModules )
        {
            foreach( CswEnumNbtModuleName childModule in NbtResources.Modules.GetChildModules( Module.Name ) )
            {
                CswNbtDataContractModule childModuleData = _getModuleData( NbtResources, childModule );
                seenModules.Add( childModule.Value );
                _recurse( NbtResources, childModuleData, seenModules );
                Module.ChildModules.Add( childModuleData );
            }
        }

        private static CswNbtDataContractModule _getModuleData( CswNbtResources NbtResources, CswEnumNbtModuleName Module )
        {
            bool isEnabled = NbtResources.Modules.IsModuleEnabled( Module );
            CswNbtDataContractModule module = new CswNbtDataContractModule()
            {
                Name = Module.Value,
                Enabled = isEnabled
            };
            return module;
        }

        public static void Initialize( ICswResources CswResources, CswNbtModulesPageReturn Return, object Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Data.Modules = _getModules( NbtResources );
        }

        public static void HandleModule( ICswResources CswResources, CswNbtModulesPageReturn Return, CswNbtDataContractModule Module )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswEnumNbtModuleName ThisModule = Module.Name;
            if( Module.Enabled )
            {
                NbtResources.Modules.EnableModule( ThisModule );
            }
            else
            {
                NbtResources.Modules.DisableModule( ThisModule );
            }

            Return.Data.Modules = _getModules( NbtResources );
        }

    } // class CswNbtWebServiceModules

    [DataContract]
    public class CswNbtDataContractModulePage
    {
        [DataMember]
        public Collection<CswNbtDataContractModule> Modules = new Collection<CswNbtDataContractModule>();
    }

    [DataContract]
    public class CswNbtDataContractModule
    {
        [DataMember( Name = "text" )]
        public string Name = string.Empty;

        [DataMember( Name = "checked" )]
        public bool Enabled = false;

        [DataMember( Name = "children" )]
        public Collection<CswNbtDataContractModule> ChildModules = new Collection<CswNbtDataContractModule>();
    }

} // namespace ChemSW.Nbt.WebServices

