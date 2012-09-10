using System;
using System.Collections.Generic;
using System.Linq;

//using ChemSW.RscAdo;
//using ChemSW.TblDn;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsProd : ICswSchemaScripts
    {
        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        public CswSchemaScriptsProd()
        {
            // This is where you manually set to the last version of the previous release
            _MinimumVersion = new CswSchemaVersion( 1, 'P', 103 );

            // This is where you add new versions.
            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );

            #region QUINCE

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_MaterialComponents_Case27462() ) );          //01Q-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_Requesting_Case27470() ) );                  //01Q-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27401() ) );                              //01Q-003            
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase_GridsAndButtons_27479() ) );             //01Q-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27513() ) );                              //01Q-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27523() ) );                              //01Q-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_PrintLabels_Case26704() ) );                 //01Q-007 Quince_2012.8.17.1
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27612() ) );                              //01Q-008

            #endregion QUINCE

            #region ROMEO

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27145() ) );                              //01Q-009  //01R-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_Requesting_Case27542() ) );                  //01Q-010  //01R-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_Logging_Case27504() ) );                     //01Q-011  //01R-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_Size_Case27438() ) );                        //01Q-012  //01R-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27548() ) );                              //01Q-013  //01R-005                  
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_Documents_Case27435() ) );                   //01Q-014  //01R-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27493() ) );                              //01Q-015  //01R-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_RequestItem_Case27488() ) );                 //01Q-016  //01R-008       
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27500() ) );                              //01Q-017  //01R-009      
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_MSDS_Case27435() ) );                        //01Q-018  //01R-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27432() ) );                              //01Q-019  //01R-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27431() ) );                              //01Q-020  //01R-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27274() ) );                              //01Q-021  //01R-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24525() ) );                              //01Q-022  //01R-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24632() ) );                              //01Q-023  //01R-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27551() ) );                              //01Q-024  //01R-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27549() ) );                              //01Q-025  //01R-017
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27401_part2() ) );                        //01Q-026  //01R-018
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27278() ) );                              //01Q-027  //01R-019
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27622() ) );                              //01Q-028  //01R-020
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26924() ) );                              //01Q-029  //01R-021
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27439() ) );                              //01Q-030  //01R-022
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27628() ) );                              //01Q-031  //01R-023
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27439_Part2() ) );                        //01Q-032  //01R-024

            #endregion ROMEO
            
            #region SEBASTIAN

            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );                            //01Q-033  //01R-025  //01S-001

            #endregion SEBASTIAN


            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27647() ) );                                    //01S-001 //031


            // This automatically detects the latest version
            _LatestVersion = _MinimumVersion;
            foreach( CswSchemaVersion Version in _UpdateDrivers.Keys.Where( Version => _LatestVersion == _MinimumVersion ||
                                                                                        ( _LatestVersion.CycleIteration == Version.CycleIteration &&
                                                                                            _LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier &&
                                                                                            _LatestVersion.ReleaseIteration < Version.ReleaseIteration ) ) )
            {
                _LatestVersion = Version;
            }

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01() ), RunBeforeEveryExecutionOfUpdater_01.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01b() ), RunBeforeEveryExecutionOfUpdater_01b.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02() ), RunBeforeEveryExecutionOfUpdater_02.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_03() ), RunBeforeEveryExecutionOfUpdater_03.Title );

            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_01() ), RunAfterEveryExecutionOfUpdater_01.Title );
            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_02() ), RunAfterEveryExecutionOfUpdater_02.Title );
            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_03() ), RunAfterEveryExecutionOfUpdater_03.Title );

        }//ctor

        #region ICswSchemaScripts




        private CswSchemaVersion _LatestVersion = null;
        public CswSchemaVersion LatestVersion
        {
            get { return ( _LatestVersion ); }
        }

        private CswSchemaVersion _MinimumVersion = null;
        public CswSchemaVersion MinimumVersion
        {
            get { return ( _MinimumVersion ); }
        }

        public CswSchemaVersion CurrentVersion( CswNbtResources CswNbtResources )
        {
            return ( new CswSchemaVersion( CswNbtResources.ConfigVbls.getConfigVariableValue( "schemaversion" ) ) );
        }


        public CswSchemaVersion TargetVersion( CswNbtResources CswNbtResources )
        {
            CswSchemaVersion ret = null;
            CswSchemaVersion myCurrentVersion = CurrentVersion( CswNbtResources );
            if( myCurrentVersion == MinimumVersion )
                ret = new CswSchemaVersion( LatestVersion.CycleIteration, LatestVersion.ReleaseIdentifier, 1 );
            else
                ret = new CswSchemaVersion( myCurrentVersion.CycleIteration, myCurrentVersion.ReleaseIdentifier, myCurrentVersion.ReleaseIteration + 1 );
            return ret;
        }


        public CswSchemaUpdateDriver Next( CswNbtResources CswNbtResources )
        {
            CswSchemaUpdateDriver ReturnVal = null;

            CswSchemaVersion myCurrentVersion = CurrentVersion( CswNbtResources );
            if( myCurrentVersion == MinimumVersion ||
                ( LatestVersion.CycleIteration == myCurrentVersion.CycleIteration &&
                    LatestVersion.ReleaseIdentifier == myCurrentVersion.ReleaseIdentifier &&
                    LatestVersion.ReleaseIteration > myCurrentVersion.ReleaseIteration ) )
            {
                ReturnVal = _UpdateDrivers[TargetVersion( CswNbtResources )];
            }



            return ( ReturnVal );
        }


        public CswSchemaUpdateDriver this[CswSchemaVersion CswSchemaVersion]
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;

                if( _UpdateDrivers.ContainsKey( CswSchemaVersion ) )
                {
                    ReturnVal = _UpdateDrivers[CswSchemaVersion];
                }

                return ( ReturnVal );

            }//get

        }//indexer

        public void stampSchemaVersion( CswNbtResources CswNbtResources, CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswNbtResources.ConfigVbls.setConfigVariableValue( "schemaversion", CswSchemaUpdateDriver.SchemaVersion.ToString() ); ;
        }//stampSchemaVersion()


        #endregion



        #region Versioned scripts

        CswSchemaVersion _makeNextSchemaVersion()
        {
            int SuperCycle = _MinimumVersion.CycleIteration;
            char ReleaseIdentifier = _MinimumVersion.ReleaseIdentifier;
            if( 'Z' != ReleaseIdentifier )
            {
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                List<char> Chars = new List<char>( alpha );
                int ReleaseIdInt = Chars.IndexOf( ReleaseIdentifier );
                ReleaseIdInt++;
                ReleaseIdentifier = Chars[ReleaseIdInt];
            }
            else
            {
                SuperCycle = _MinimumVersion.CycleIteration + 1;
                ReleaseIdentifier = 'A';
            }


            return ( new CswSchemaVersion( SuperCycle, ReleaseIdentifier, _UpdateDrivers.Keys.Count + 1 ) );

        }//_makeNextSchemaVersion()


        private void _addVersionedScript( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswSchemaUpdateDriver.SchemaVersion = _makeNextSchemaVersion();
            CswSchemaUpdateDriver.Description = CswSchemaUpdateDriver.SchemaVersion.ToString(); //we do this in prod scripts because test scripts have a different dispensation for description
            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );

        }//addReleaseDmlDriver() 


        #endregion

        #region Run-always scripts

        //Run before
        private List<CswSchemaUpdateDriver> _RunBeforeScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunBeforeScripts
        {
            get
            {
                return ( _RunBeforeScripts );
            }
        }//RunBeforeScripts


        private void _addRunBeforeScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', _RunBeforeScripts.Count );
            CswSchemaUpdateDriver.Description = Description;
            if( false == _RunBeforeScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunBeforeScripts.Add( CswSchemaUpdateDriver );
            }
        }//


        //Run after
        private List<CswSchemaUpdateDriver> _RunAfterScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunAfterScripts
        {
            get
            {
                return ( _RunAfterScripts );
            }

        }//RunBeforeScripts
        private void _addRunAfterScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 99, '#', _RunAfterScripts.Count );
            CswSchemaUpdateDriver.Description = Description;
            if( false == _RunAfterScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunAfterScripts.Add( CswSchemaUpdateDriver );
            }

        }//
        #endregion
    }//CswScriptCollections

}//ChemSW.Nbt.Schema
