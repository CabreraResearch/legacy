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
            _MinimumVersion = new CswSchemaVersion( 1, 'U', 18 );

            // This is where you add new versions.
            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );

            #region VIOLA

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case26827() ) );              //01V-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28283Part1() ) );         //01V-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28283Part2() ) );         //01V-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28281() ) );              //01V-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28282() ) );              //01V-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28339() ) );              //01V-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28252() ) );              //01V-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case5083() ) );               //01V-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28247() ) );              //01V-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28393() ) );              //01V-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28399() ) );              //01V-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28399B() ) );             //01V-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28331() ) );              //01V-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28267() ) );              //01V-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28374() ) );              //01V-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28374B() ) );             //01V-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28293() ) );              //01V-017
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28408_Part1() ) );        //01V-018
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28408_Part2() ) );        //01V-019
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28295() ) );              //01V-020
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case27649() ) );              //01V-021
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case27436() ) );              //01V-022
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case27436B() ) );             //01V-023
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case27436C() ) );             //01V-024
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case27436D() ) );             //01V-025
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28498() ) );              //01V-026
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01V_Case28570() ) );              //01V-027


            #endregion VIOLA

            #region WILLIAM

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );           //01V-000   01W-000
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28363() ) );              //01V-028   01W-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case24647() ) );              //01V-029   01W-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28246() ) );              //01V-030   01W-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28277() ) );              //01V-031   01W-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28053() ) );              //01V-032   01W-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_TreeGrouping_Case27882() ) ); //01V-033   01W-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28393B() ) );             //01V-034   01W-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28345() ) );              //01V-035   01W-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28507() ) );              //01V-036   01W-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28508() ) );              //01V-037   01W-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28560() ) );              //01V-038   01W-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28409() ) );              //01V-039   01W-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28395() ) );              //01V-040   01W-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28258() ) );              //01V-041   01W-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01W_Case28513() ) );              //01V-042   01W-015


            #endregion WILLIAM


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
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02SQL() ), RunBeforeEveryExecutionOfUpdater_02SQL.Title );
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
