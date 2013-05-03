using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
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
            foreach( CswEnumNbtModuleName ModuleName in CswEnumNbtModuleName._All )
            {
                if( CswEnumNbtModuleName.Unknown != ModuleName )
                {
                    bool isEnabled = NbtResources.Modules.IsModuleEnabled( ModuleName );
                    string msg = "";
                    Collection<CswEnumNbtModuleName> ChildModules = NbtResources.Modules.GetChildModules( ModuleName );
                    CswCommaDelimitedString childrenAsString = new CswCommaDelimitedString();
                    if( isEnabled && ChildModules.Count > 0 )
                    {
                        foreach( CswEnumNbtModuleName child in ChildModules )
                        {
                            if( NbtResources.Modules.IsModuleEnabled( child ) )
                            {
                                childrenAsString.Add( child._Name );
                            }
                        }
                        if( childrenAsString.Count > 0 )
                        {
                            msg = "Disable " + childrenAsString.ToString() + " first.";
                        }
                    }
                    else
                    {
                        CswEnumNbtModuleName prereq = NbtResources.Modules.GetModulePrereq( ModuleName );
                        if( prereq != CswEnumNbtModuleName.Unknown && false == NbtResources.Modules.IsModuleEnabled( prereq ) )
                        {
                            msg = "Enable " + prereq._Name + " first.";
                        }
                    }

                    CswNbtDataContractModule module = new CswNbtDataContractModule()
                        {
                            Name = ModuleName._Name,
                            Id = NbtResources.Modules.GetModuleId( ModuleName ),
                            Enabled = isEnabled,
                            StatusMsg = msg
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
                CswEnumNbtModuleName prereq = NbtResources.Modules.GetModulePrereq( ThisModule );
                if( CswEnumNbtModuleName.Unknown != prereq && false == NbtResources.Modules.IsModuleEnabled( prereq ) )
                {
                    Return.Data.ErrorMessage = "Error: cannot enable " + Module.Name + ". " + prereq._Name + " must be enabled first.";
                }
                else
                {
                    NbtResources.Modules.EnableModule( ThisModule );
                }
            }
            else
            {
                Collection<CswEnumNbtModuleName> ChildModules = NbtResources.Modules.GetChildModules( ThisModule );
                CswCommaDelimitedString enabledChildren = new CswCommaDelimitedString();
                if( ChildModules.Count > 0 )
                {
                    foreach( CswEnumNbtModuleName childModule in ChildModules )
                    {
                        if( NbtResources.Modules.IsModuleEnabled( childModule ) )
                        {
                            enabledChildren.Add( childModule._Name );
                        }
                    }
                }

                if( enabledChildren.Count > 0 )
                {
                    Return.Data.ErrorMessage = "Error: cannot disable " + Module.Name + ". " + enabledChildren + " must be disabled first.";
                }
                else
                {
                    NbtResources.Modules.DisableModule( ThisModule );
                }
            }

            Return.Data.Modules = _getModules( NbtResources );
        }

    } // class CswNbtWebServiceModules

    [DataContract]
    public class CswNbtDataContractModulePage
    {
        [DataMember]
        public Collection<CswNbtDataContractModule> Modules = new Collection<CswNbtDataContractModule>();

        [DataMember]
        public string ErrorMessage = string.Empty;
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

        [DataMember]
        public string StatusMsg = string.Empty;
    }

} // namespace ChemSW.Nbt.WebServices

