using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Csw.WebSvc;
using ChemSW.DB;
using ChemSW.Nbt.PropTypes;
using ChemSW.Security;
using NbtWebApp.Services;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    ///     Webservice to manage configuration variables
    /// </summary>
    public class CswNbtWebServiceConfigurationVariables
    {
        private const string COL_DESCRIPTION = "description";
        private const string COL_ISSYSTEM = "issystem";
        private const string COL_VARIABLENAME = "variablename";
        private const string COL_VARIABLEVALUE = "variablevalue";
        private const string COL_MODULEID = "moduleid";
        private const string COL_DATATYPE = "datatype";
        private const string COL_CONSTRAINT = "constraint";

        public static void Initialize( ICswResources CswResources, CswNbtConfigurationVariablesPageReturn Return, object Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Data = _getConfigVars( NbtResources );
        }

        /// <summary>
        ///     returns a collection of config vars, to be displayed on the config var page. Only config vars the currently logged in user can see are included
        /// </summary>
        /// <param name="NbtResources">Instance of NbtResources</param>
        /// <returns>dictionary of config vars, arranged by module</returns>
        private static CswNbtDataContractConfigurationVariablesPage _getConfigVars( CswNbtResources NbtResources )
        {
            CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>> configVarsByModule = new CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>>();
            HashSet<int> enabledModuleIDs = new HashSet<int>();
            //system vars are grouped manually in order to add them
            //to the end of the collection
            Collection<CswNbtDataContractConfigurationVariable> systemConfigVars = new Collection<CswNbtDataContractConfigurationVariable>();

            //a set of seen modules will help check which modules
            //can be seen
            foreach( CswEnumNbtModuleName moduleName in CswEnumNbtModuleName.All )
            {
                if( CswEnumNbtModuleName.Unknown != moduleName &&
                    NbtResources.Modules.IsModuleEnabled( moduleName ) )
                {
                    int thisModuleID = NbtResources.Modules.GetModuleId( moduleName );
                    enabledModuleIDs.Add( thisModuleID );
                }
            }

            CswTableSelect CVTableSelect = NbtResources.makeCswTableSelect( "config_var_nu", "configuration_variables" );
            DataTable CVDataTable = CVTableSelect.getTable();

            //go through each config var
            //if it is associated with a module, select only those 
            //which are attached to modules in use
            foreach( DataRow currentRow in CVDataTable.Rows )
            {
                //check if the config var should be added based on module
                if( _includeConfigVar( enabledModuleIDs, currentRow[COL_MODULEID] ) )
                {
                    CswNbtDataContractConfigurationVariable thisConfigVarDataContract = new CswNbtDataContractConfigurationVariable
                        {
                            VariableName = currentRow[COL_VARIABLENAME].ToString(),
                            VariableValue = currentRow[COL_VARIABLEVALUE].ToString(),
                            IsSystem = CswConvert.ToBoolean( currentRow[COL_ISSYSTEM] ),
                            Constraint = currentRow[COL_CONSTRAINT].ToString(),
                            DataType = currentRow[COL_DATATYPE].ToString(),
                            Description = currentRow[COL_DESCRIPTION].ToString()
                        };
                    //if this configVar is a system id, group as system settings
                    //if this configVar has a moduleID, find the module name.
                    //if this configVar is a common module, add it under a blank module name
                    if( CswConvert.ToBoolean( currentRow[COL_ISSYSTEM] ))
                    {
                        systemConfigVars.Add( thisConfigVarDataContract );
                    }

                    else
                    {
                        string thisConfigVarModuleName = NbtResources.Modules.GetModuleName( CswConvert.ToInt32( currentRow[COL_MODULEID] ) );

                        if( thisConfigVarModuleName == "" )
                        {
                            thisConfigVarModuleName = "Common";
                        }

                        //add the config var module to the collection
                        //if the module doesn't exist in the collection, create it
                        if( false == configVarsByModule.ContainsKey( thisConfigVarModuleName ) )
                        {
                            configVarsByModule[thisConfigVarModuleName] = new Collection<CswNbtDataContractConfigurationVariable>();
                        }

                        configVarsByModule[thisConfigVarModuleName].Add( thisConfigVarDataContract );
                    }
                }
            }

            //only add the system config vars if the current
            //user is chemsw_admin
            if( NbtResources.CurrentUser.Username == CswAuthenticator.ChemSWAdminUsername )
            {
                configVarsByModule.Add( "System", systemConfigVars);
            }

            CswNbtDataContractConfigurationVariablesPage ret = new CswNbtDataContractConfigurationVariablesPage();
            ret.ConfigVarsByModule = configVarsByModule;

            return ret;
        }

        /// <summary>
        ///     checks whether or not whether a given config var should be included in the list of config vars in the config vars page, based on whether the module it is associated to is enabled
        /// </summary>
        /// <param name="nbtResources">CswNbtResources object</param>
        /// <param name="enabledModuleIds">HashSet containing ids of modules that are enabled</param>
        /// <param name="moduleIDForConfigVar">The module id of this configVar. </param>
        /// <param name="configVarIsSystem">True if this config var is a system config var</param>
        /// <returns></returns>
        private static bool _includeConfigVar( HashSet<int> enabledModuleIds, object moduleIDForConfigVar )
        {
            Boolean ret = true;

            //if this config var is associated with a module
            //and that module is currently disabled
            //hide it
            if( moduleIDForConfigVar != DBNull.Value )
            {
                int moduleIDForConfigVarInt = CswConvert.ToInt32( moduleIDForConfigVar );
                if( false == ( enabledModuleIds.Contains( moduleIDForConfigVarInt ) ) )
                {
                    ret = false;
                }
            }

            return ret;
        }

//_includeConfigVar
    }

    [DataContract, KnownType( typeof( Collection<CswNbtDataContractConfigurationVariable> ) )]
    public class CswNbtDataContractConfigurationVariablesPage
    {
        [DataMember] public CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>> ConfigVarsByModule = new CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>>();
    }

    [DataContract]
    public class CswNbtDataContractConfigurationVariable
    {
        /// <summary>
        ///     constraints on the permitted input to this configvariable. The way in which it is applied depends on this configvar's datatype
        /// </summary>
        [DataMember( Name = "constraint" )] public string Constraint = string.Empty;

        /// <summary>
        ///     Used to determine how to apply the constraint. Can be INT, STRING, LIST, BOOL
        /// </summary>
        [DataMember( Name = "dataType" )] public string DataType = string.Empty;

        /// <summary>
        ///     a description of the configuration variable
        /// </summary>
        [DataMember( Name = "description" )] public string Description = string.Empty;

        /// <summary>
        ///     true if this is a system config var
        /// </summary>
        [DataMember( Name = "isSystem" )] public Boolean IsSystem = false;

        /// <summary>
        ///     the name of the config var
        /// </summary>
        [DataMember( Name = "variableName" )] public string VariableName = string.Empty;

        /// <summary>
        ///     the current value to which this config var is set
        /// </summary>
        [DataMember( Name = "variableValue" )] public string VariableValue = string.Empty;
    }
}

// namespace ChemSW.Nbt.WebServices