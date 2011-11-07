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

namespace ChemSW.Nbt.ImportExport
{

    public class CswNbtPersistedImportState
    {

        CswNbtResources _CswNbtResources = null;
        private enum _ConfigVarNames { CompletedProcessPhase }
        private string _ConfigVarStem = "ImportStatus";
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
                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.CompletedProcessPhase.ToString() );
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

                string ConfigVarName = _prefixConfigVarName( _ConfigVarNames.CompletedProcessPhase.ToString() );
                if( false == _CswNbtResources.ConfigVbls.doesConfigVarExist( ConfigVarName ) )
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


    }//class CswNbtPersistedImportState

}//namespace ChemSW.Nbt.ImportExport
