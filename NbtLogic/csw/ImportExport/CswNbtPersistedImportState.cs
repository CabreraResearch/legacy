using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ImportExport
{

    public class CswNbtPersistedImportState
    {

        CswNbtResources _CswNbtResources = null;
        private enum _ConfigVarNames { Unknown, PhaseComplete, FilePath, ImportMode }
        private string _ConfigVarStem = "imprt";

        public CswNbtPersistedImportState( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        private string _prefixConfigVarName( string VariableName )
        {

            return ( _ConfigVarStem + "_" + VariableName );
        }

        public ImportProcessPhase CompletedProcessPhase
        {
            set
            {
                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.PhaseComplete.ToString() );
                if( false == _CswNbtResources.ConfigVbls.doesConfigVarExist( ConfigVarName ) )
                {
                    _CswNbtResources.ConfigVbls.addNewConfigurationValue( ConfigVarName, value.ToString(), "Last known successful import phase", true );
                }
                else
                {
                    _CswNbtResources.ConfigVbls.setConfigVariableValue( ConfigVarName, value.ToString() );
                }
            }//set

            get
            {
                ImportProcessPhase ReturnVal = ImportProcessPhase.NothingDoneYet;

                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.PhaseComplete.ToString() );
                if( _CswNbtResources.ConfigVbls.doesConfigVarExist( ConfigVarName ) )
                {
                    ImportProcessPhase CandidateValue;
                    if( true == Enum.TryParse<ImportProcessPhase>( _CswNbtResources.ConfigVbls.getConfigVariableValue( ConfigVarName ), true, out CandidateValue ) )
                    {
                        ReturnVal = CandidateValue;
                    }
                }//if the variable exists

                return ( ReturnVal );
            }

        }//CompletedProcessPhase



        public string FilePath
        {
            set
            {

                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.FilePath.ToString() );
                if( false == _CswNbtResources.ConfigVbls.doesConfigVarExist( ConfigVarName ) )
                {
                    _CswNbtResources.ConfigVbls.addNewConfigurationValue( ConfigVarName, value, "Import file path", true );
                }
                else
                {
                    _CswNbtResources.ConfigVbls.setConfigVariableValue( ConfigVarName, value );
                }
            }
            get
            {
                string ReturnVal = string.Empty;

                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.FilePath.ToString() );
                if( _CswNbtResources.ConfigVbls.doesConfigVarExist( ConfigVarName ) )
                {
                    ReturnVal = _CswNbtResources.ConfigVbls.getConfigVariableValue( ConfigVarName );
                }//if the variable exists

                return ( ReturnVal );
            }
        }

        public ImportMode Mode
        {

            set
            {
                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.ImportMode.ToString() );
                if( false == _CswNbtResources.ConfigVbls.doesConfigVarExist( ConfigVarName ) )
                {
                    _CswNbtResources.ConfigVbls.addNewConfigurationValue( ConfigVarName, value.ToString(), "Import mode", true );
                }
                else
                {
                    _CswNbtResources.ConfigVbls.setConfigVariableValue( ConfigVarName, value.ToString() );
                }
            }//set

            get
            {
                ImportMode ReturnVal = ImportMode.Unknown;

                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.ImportMode.ToString() );
                if( _CswNbtResources.ConfigVbls.doesConfigVarExist( ConfigVarName ) )
                {
                    ImportMode CandidateValue;
                    if( true == Enum.TryParse<ImportMode>( _CswNbtResources.ConfigVbls.getConfigVariableValue( ConfigVarName ), true, out CandidateValue ) )
                    {
                        ReturnVal = CandidateValue;
                    }
                }//if the variable exists

                return ( ReturnVal );
            }
        }


    }//class CswNbtPersistedImportState

}//namespace ChemSW.Nbt.ImportExport
