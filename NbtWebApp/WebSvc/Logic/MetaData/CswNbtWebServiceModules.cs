using System;
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
            Collection<CswNbtDataContractModule> ret = new Collection<CswNbtDataContractModule>();
            foreach( CswEnumNbtModuleName ModuleName in CswEnumNbtModuleName.All )
            {
                if( CswEnumNbtModuleName.Unknown != ModuleName )
                {
                    bool isEnabled = NbtResources.Modules.IsModuleEnabled( ModuleName );
                    CswNbtDataContractModule module = new CswNbtDataContractModule()
                        {
                            Name = ModuleName.Value,
                            Id = NbtResources.Modules.GetModuleId( ModuleName ),
                            Enabled = isEnabled
                        };
                    ret.Add( module );
                }
            }
            return ret;
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
        [DataMember]
        public string Name = string.Empty;

        [DataMember]
        public int Id = Int32.MinValue;

        [DataMember]
        public bool Enabled = false;
    }

} // namespace ChemSW.Nbt.WebServices

