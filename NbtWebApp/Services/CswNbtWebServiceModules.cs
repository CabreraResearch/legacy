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
                foreach( CswNbtResources.CswNbtModule Module in Enum.GetValues( typeof( CswNbtResources.CswNbtModule ) ) )
                {
                    ret[Module.ToString()] = _CswNbtResources.IsModuleEnabled( Module );
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
                Collection<CswNbtResources.CswNbtModule> ModulesToEnable = new Collection<CswNbtResources.CswNbtModule>();
                Collection<CswNbtResources.CswNbtModule> ModulesToDisable = new Collection<CswNbtResources.CswNbtModule>();

                foreach( JProperty ModulesJProp in inModulesJson.Properties() )
                {
                    CswNbtResources.CswNbtModule Module;
                    Enum.TryParse( ModulesJProp.Name, true, out Module );
                    if( CswConvert.ToBoolean( ModulesJProp.Value ) )
                    {
                        ModulesToEnable.Add( Module );
                    }
                    else
                    {
                        ModulesToDisable.Add( Module );
                    }
                }

                _CswNbtResources.UpdateModules( ModulesToEnable, ModulesToDisable );

            } // if( _CanEditModules )
            return ret;
        } // SaveModules()

    } // class CswNbtWebServiceModules
} // namespace ChemSW.Nbt.WebServices

