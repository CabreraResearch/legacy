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
            _MinimumVersion = new CswSchemaVersion( 1, 'R', 34 );

            // This is where you add new versions.
            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );

            #region SEBASTIAN

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27578() ) );                              //01S-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27648() ) );                              //01S-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27647() ) );                              //01S-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27646() ) );                              //01S-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case26760() ) );                              //01S-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27629() ) );                              //01S-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_DevModule_Case27759() ) );                    //01S-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27632() ) );                              //01S-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27691() ) );                              //01S-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_FieldTypes_Case25352() ) );                   //01S-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_PrintLabel_Case21701() ) );                   //01S-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27703() ) );                              //01S-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27780() ) );                              //01S-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27665() ) );                              //01S-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27670() ) );                              //01S-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27769() ) );                              //01S-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );                                   //01S-017
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_MobileModule_Case27859() ) );                 //01S-018
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27779_part1() ) );                        //01S-019
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27779_part2() ) );                        //01S-020
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27551_part2() ) );                        //01S-021 
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27751() ) );                              //01S-022 
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27858() ) );                              //01S-023
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27904() ) );                              //01S-024
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27800() ) );                              //01S-025
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27720() ) );                              //01S-026

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27578_part2() ) );                        //01S-027
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27720_part2() ) );                        //01S-028
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27720_part3() ) );                        //01S-029
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27720_part4() ) );                        //01S-030
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case28002() ) );                              //01S-031
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27742() ) );                              //01S-032
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case28016() ) );                              //01S-033
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27709() ) );                              //01S-034
            #endregion SEBASTIAN

            #region TITANIA

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27869() ) );                         //01S-035    01T-01
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27873() ) );                         //01S-036    01T-02
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27872() ) );                         //01S-037    01T-03
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_CertMethod_Case27868() ) );              //01S-038    01T-04
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_IdentityTab_Case27965() ) );             //01S-039    01T-05
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27694() ) );                         //01S-040    01T-06
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27865_part1() ) );                   //01S-041    01T-07
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27865_part2() ) );                   //01S-042    01T-08
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27867() ) );                         //01S-043    01T-09
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27947() ) );                         //01S-044    01T-10
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27880() ) );                         //01S-045    01T-11
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27740() ) );                         //01S-046    01T-12
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27801() ) );                         //01S-047    01T-13
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_0T1_Case27884() ) );                         //01S-048    01T-14
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27920() ) );                         //01S-049    01T-15
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27551() ) );                         //01S-050    01T-16
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27881() ) );                         //01S-051    01T-17
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01S_Case27528() ) );                         //01S-052    01T-18
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01T_Case27912() ) );                         //01S-053    01T-19

            #endregion TITANIA


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
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01M() ), RunBeforeEveryExecutionOfUpdater_01M.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01OC() ), RunBeforeEveryExecutionOfUpdater_01OC.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02() ), RunBeforeEveryExecutionOfUpdater_02.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_03() ), RunBeforeEveryExecutionOfUpdater_03.Title );

            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_01() ), RunAfterEveryExecutionOfUpdater_01.Title );

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
