using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for modules management
    /// </summary>
    public class CswNbtWebServiceModules
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceModules( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private bool _CanEditModules
        {
            get
            {
                return ( _CswNbtResources.CurrentNbtUser.Rolename == CswNbtObjClassRole.ChemSWAdminRoleName );
            }
        }

        public JObject GetModules()
        {
            JObject ret = new JObject();
            if( _CanEditModules )
            {
                foreach( CswEnumNbtModuleName ModuleName in CswEnumNbtModuleName._All )
                {
                    if( CswEnumNbtModuleName.Unknown != ModuleName )
                    {
                        JObject moduleInfo = new JObject();
                        moduleInfo["enabled"] = _CswNbtResources.Modules.IsModuleEnabled( ModuleName );

                        if( _CswNbtResources.Modules.ModuleHasPrereq( ModuleName ) )
                        {
                            moduleInfo["prereq"] = _CswNbtResources.Modules.GetModulePrereq( ModuleName )._Name;
                        }
                        else
                        {
                            moduleInfo["prereq"] = "";
                        }

                        ret[ModuleName.ToString()] = moduleInfo;
                    }
                }
            } // if(_CanEditModules)

            return ret;
        } // GetModules()


        public bool SaveModules( string inModules )
        {
            bool ret = false;
            JObject inModulesJson = JObject.Parse( inModules );
            if( _CanEditModules )
            {
                Collection<CswEnumNbtModuleName> ModulesToEnable = new Collection<CswEnumNbtModuleName>();
                Collection<CswEnumNbtModuleName> ModulesToDisable = new Collection<CswEnumNbtModuleName>();

                foreach( JProperty ModulesJProp in inModulesJson.Properties() )
                {
                    CswEnumNbtModuleName Module = ModulesJProp.Name;
                    //Enum.TryParse( ModulesJProp.Name, true, out Module );
                    if( CswConvert.ToBoolean( ModulesJProp.Value ) )
                    {
                        ModulesToEnable.Add( Module );
                    }
                    else
                    {
                        ModulesToDisable.Add( Module );
                    }
                }

                _CswNbtResources.Modules.UpdateModules( ModulesToEnable, ModulesToDisable );

            } // if( _CanEditModules )
            return ret;
        } // SaveModules()

    } // class CswNbtWebServiceModules
} // namespace ChemSW.Nbt.WebServices

