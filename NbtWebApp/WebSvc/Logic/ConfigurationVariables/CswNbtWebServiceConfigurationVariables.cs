using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Csw.WebSvc;
using ChemSW.DB;
using ChemSW.Exceptions;
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

        private const string DATATYPE_INT = "INT";
        private const string DATATYPE_LIST = "LIST";
        private const string DATATYPE_BOOL = "BOOL";

        public static void Initialize( ICswResources CswResources,
                                       CswNbtConfigurationVariablesPageReturn Return,
                                       object Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Data = _getConfigVars( NbtResources );
        }

        public static void Update( ICswResources CswResources,
                                   CswNbtConfigurationVariablesPageReturnUpdateSuccess Return,
                                   CswNbtDataContractConfigurationVariableResponseContainer Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            Return.Data = _updateConfigVars( NbtResources, Request );
        }


        /// <summary>
        ///     returns a collection of config vars, to be displayed on the config var page. Only config vars the currently logged in user can see are included
        /// </summary>
        /// <param name="NbtResources">Instance of NbtResources</param>
        /// <returns>dictionary of config vars, arranged by module</returns>
        private static CswNbtDataContractConfigurationVariablesPage _getConfigVars( CswNbtResources NbtResources )
        {
            CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>> configVarsByModule =
                new CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>>();
            //system vars are grouped manually in order to add them
            //to the end of the collection
            Collection<CswNbtDataContractConfigurationVariable> systemConfigVars =
                new Collection<CswNbtDataContractConfigurationVariable>();
            Collection<CswNbtDataContractConfigurationVariable> commonConfigVars =
                new Collection<CswNbtDataContractConfigurationVariable>();

            CswTableSelect CVTableSelect = NbtResources.makeCswTableSelect( "config_var_nu", "configuration_variables" );
            DataTable CVDataTable = CVTableSelect.getTable();

            //go through each config var
            //if it is associated with a module, select only those 
            //which are attached to modules in use
            foreach( DataRow currentRow in CVDataTable.Rows )
            {
                //check if the config var should be added based on module
                bool includeConfigVar = true;
                int moduleIDForConfigVar = CswConvert.ToInt32(currentRow[COL_MODULEID]); 

                //if this config var is associated with a module
                //and that module is currently disabled
                //hide it
                if( moduleIDForConfigVar != Int32.MinValue)
                {
                    if( false == ( NbtResources.Modules.IsModuleEnabled( moduleIDForConfigVar )) )
                    {
                        includeConfigVar = false;
                    }
                }

                if( includeConfigVar )
                {
                    CswNbtDataContractConfigurationVariable thisConfigVarDataContract =
                        new CswNbtDataContractConfigurationVariable
                            {
                                VariableName = currentRow[COL_VARIABLENAME].ToString(),
                                VariableValue = currentRow[COL_VARIABLEVALUE].ToString(),
                                IsSystem = CswConvert.ToBoolean( currentRow[COL_ISSYSTEM] ),
                                DataType = currentRow[COL_DATATYPE].ToString(),
                                Description = currentRow[COL_DESCRIPTION].ToString()
                            };

                    //set the constraints according to datatype
                    if( DATATYPE_INT == thisConfigVarDataContract.DataType )
                    {
                        thisConfigVarDataContract.Minvalue = currentRow[COL_CONSTRAINT].ToString();
                    }

                    else if( DATATYPE_LIST == thisConfigVarDataContract.DataType )
                    {
                        string listOptionsAsString = currentRow[COL_CONSTRAINT].ToString();
                        //string[] listOptionsAsArray = listOptionsAsString.Split( ',' );
                        thisConfigVarDataContract.ListOptions = new CswCommaDelimitedString(listOptionsAsString);
                    }

                    //if this configVar is a system id, group as system settings
                    //if this configVar has a moduleID, find the module name.
                    //if this configVar is a common module, add it under a blank module name
                    if( CswConvert.ToBoolean( currentRow[COL_ISSYSTEM] ) )
                    {
                        systemConfigVars.Add( thisConfigVarDataContract );
                    }
                    else if( currentRow[COL_MODULEID] == DBNull.Value )
                    {
                        commonConfigVars.Add( thisConfigVarDataContract );
                    }
                    else
                    {
                        string thisConfigVarModuleName =
                            NbtResources.Modules.GetModuleName( CswConvert.ToInt32( currentRow[COL_MODULEID] ) );

                        //add the config var module to the collection
                        //if the module doesn't exist in the collection, create it
                        if( false == configVarsByModule.ContainsKey( thisConfigVarModuleName ) )
                        {
                            configVarsByModule[thisConfigVarModuleName] =
                                new Collection<CswNbtDataContractConfigurationVariable>();
                        }

                        configVarsByModule[thisConfigVarModuleName].Add( thisConfigVarDataContract );
                    }
                } //if configVar is to be included according to _includeConfigVar()
            } //foreach dataRow

            configVarsByModule.Add( "Common", commonConfigVars );

            //only add the system config vars if the current
            //user is chemsw_admin
            if( NbtResources.CurrentUser.Username == CswAuthenticator.ChemSWAdminUsername )
            {
                configVarsByModule.Add( "System", systemConfigVars );
            }

            CswNbtDataContractConfigurationVariablesPage ret = new CswNbtDataContractConfigurationVariablesPage();
            ret.ConfigVarsByModule = configVarsByModule;

            return ret;
        }

        //_getConfigVars
        
        /// <summary>
        /// powers the webservice, which handles incoming updated config vars from server
        /// </summary>
        /// <param name="NbtResources">NbtResources object</param>
        /// <param name="varsFromServer">Encapsulates a collection of config vars form server</param>
        /// <returns></returns>
        private static CswNbtDataContractConfigurationUpdateSuccessResponse _updateConfigVars( CswNbtResources NbtResources,
                                                                                               CswNbtDataContractConfigurationVariableResponseContainer varsFromServer )
        {
            CswNbtDataContractConfigurationUpdateSuccessResponse ret = new CswNbtDataContractConfigurationUpdateSuccessResponse();
            Dictionary<string, string> updatedConfigVarsFromClient = new Dictionary<string, string>();

            //add all the config vars into the dictionary
            //keyed by name
            foreach( CswNbtDataContractConfigurationVariableResponse thisResponse in varsFromServer.responseArray )
            {
                updatedConfigVarsFromClient.Add( thisResponse.VariableName, thisResponse.VariableValue );
            }

            CswTableUpdate CVTableUpdate = NbtResources.makeCswTableUpdate( "update config vars", "configuration_variables" );
            DataTable CVDataTable = CVTableUpdate.getTable();

            foreach( DataRow thisRow in CVDataTable.Rows )
            {
                string configVarName = thisRow[COL_VARIABLENAME].ToString();

                if( updatedConfigVarsFromClient.ContainsKey( configVarName ) )
                {
                    string updatedConfigVarValue = updatedConfigVarsFromClient[configVarName];
                    string currentConfigVarValue = thisRow[COL_VARIABLEVALUE].ToString();

                    //make sure config var has been altered
                    if( updatedConfigVarValue != currentConfigVarValue )
                    {
                        string configVarDataType = thisRow[COL_DATATYPE].ToString();

                        //now do type specific validation
                        switch( configVarDataType )
                        {
                            case DATATYPE_INT:
                                int configValueAsInt = CswConvert.ToInt32( updatedConfigVarValue );
                                int configMinValue = CswConvert.ToInt32( thisRow[COL_CONSTRAINT] );
                                //throw exception if the value is > minvalue
                                if( configValueAsInt < configMinValue )
                                {
                                    throw new CswDniException( CswEnumErrorType.Warning,
                                                               "The value of " + configVarName + " must be greater than "
                                                               + configMinValue,
                                                               "Config varible of INT datatype must be < " + configMinValue );
                                }
                                else
                                {
                                    thisRow[COL_VARIABLEVALUE] = updatedConfigVarValue;
                                }
                                break;

                            case DATATYPE_LIST:
                                CswCommaDelimitedString listOptions = new CswCommaDelimitedString(thisRow[COL_CONSTRAINT].ToString());
                                if( false == listOptions.Contains( updatedConfigVarValue ) )
                                {
                                    string listOptionsString = listOptions.ToString();
                                    throw new CswDniException( CswEnumErrorType.Warning,
                                                               configVarName + " must be set to one of these options: "
                                                               + listOptionsString,
                                                               "Config variable " + configVarName + " cannot be set to " + currentConfigVarValue +
                                                               " since it is not one of the predefined options " + listOptionsString );
                                }
                                else
                                {
                                    thisRow[COL_VARIABLEVALUE] = updatedConfigVarValue;
                                }
                                break;

                            case DATATYPE_BOOL:
                                if( false == ( updatedConfigVarValue == "0" ||
                                               updatedConfigVarValue == "1" ) )
                                {
                                    throw new CswDniException( CswEnumErrorType.Warning,
                                                               configVarName + " must be set to 0 or 1",
                                                               configVarName + " is of type BOOL, therefore it must be set to 0 or 1" );
                                }
                                else
                                {
                                    thisRow[COL_VARIABLEVALUE] = updatedConfigVarValue;
                                }
                                break;

                            //for string type there is no config var specific validation
                            default:
                                thisRow[COL_VARIABLEVALUE] = updatedConfigVarValue;
                                break;
                        } //switch by ConfigVarDatatype
                    } // if config var has been updated
                } //if this config var was returned from client
            } //for each dataRow

            //commit changes
            CVTableUpdate.update( CVDataTable );

            //if any errors are encountered, the exception will throw an error message
            //so there is no execution path where we return false
            ret.success = true;
            return ret;
        }

        //updateConfigVars

    }

    [DataContract, KnownType( typeof( Collection<CswNbtDataContractConfigurationVariable> ) )]
    public class CswNbtDataContractConfigurationVariablesPage
    {
        [DataMember]
        public CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>> ConfigVarsByModule = new CswAjaxDictionary<Collection<CswNbtDataContractConfigurationVariable>>();
    }

    [DataContract, KnownType(typeof(CswCommaDelimitedString))]
    public class CswNbtDataContractConfigurationVariable
    {
        /// <summary>
        ///     if this config var's datatype is INTTYPE this is the minimum permitted value 
        /// </summary>
        [DataMember( Name = "minvalue" )]
        public string Minvalue = string.Empty;

        /// <summary>
        ///     if the datatype is LIST, these are the rendered list options
        /// </summary>
        [DataMember( Name = "listoptions" )]
        public CswCommaDelimitedString ListOptions = new CswCommaDelimitedString();

        /// <summary>
        ///     Used to determine how to apply the constraint. Can be INT, STRING, LIST, BOOL
        /// </summary>
        [DataMember( Name = "dataType" )]
        public string DataType = string.Empty;

        /// <summary>
        ///     a description of the configuration variable
        /// </summary>
        [DataMember( Name = "description" )]
        public string Description = string.Empty;

        /// <summary>
        ///     true if this is a system config var
        /// </summary>
        [DataMember( Name = "isSystem" )]
        public Boolean IsSystem = false;

        /// <summary>
        ///     the name of the config var
        /// </summary>
        [DataMember( Name = "variableName" )]
        public string VariableName = string.Empty;

        /// <summary>
        ///     the current value to which this config var is set
        /// </summary>
        [DataMember( Name = "variableValue" )]
        public string VariableValue = string.Empty;
    }

    [DataContract]
    public class CswNbtDataContractConfigurationVariableResponseContainer
    {
        [DataMember( Name = "Data" )]
        public List<CswNbtDataContractConfigurationVariableResponse> responseArray
            = new List<CswNbtDataContractConfigurationVariableResponse>();
    }

    [DataContract]
    public class CswNbtDataContractConfigurationVariableResponse
    {
        /// <summary>
        /// the name of this config variable
        /// </summary>
        [DataMember( Name = "variableName" )]
        public string VariableName = string.Empty;

        /// <summary>
        /// value the config variable has been set to in the UI
        /// </summary>
        [DataMember( Name = "variableValue" )]
        public string VariableValue = string.Empty;
    }

    [DataContract]
    public class CswNbtDataContractConfigurationUpdateSuccessResponse
    {
        /// <summary>
        /// whether or not the config vars posted back to the server were succesfully applied
        /// </summary>
        [DataMember( Name = "success" )]
        public Boolean success = false;
    }
}

// namespace ChemSW.Nbt.WebServices