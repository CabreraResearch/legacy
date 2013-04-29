using System.Collections.Generic;
using System.Linq;

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
            // This is where you manually set to the last version of the previous release (the one currently in production)
            _MinimumVersion = new CswSchemaVersion( 1, 'Y', 27 );

            // This is where you add new versions.
            #region ASPEN

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_CaseXXXXX() ) );          // 02A-000
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case27923_Save() ) );             // 02A-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28355() ) );                  // 02A-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28907() ) );                  // 02A-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case27906() ) );                  // 02A-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29108A() ) );                 // 02A-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29108B() ) );                 // 02A-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28950() ) );                  // 02A-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29243A() ) );                 // 02A-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29243B() ) );                 // 02A-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28890() ) );                  // 02A-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29147() ) );                  // 02A-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28927_01() ) );               // 02A-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28927_02() ) );               // 02A-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29245() ) );                  // 02A-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29245B() ) );                 // 02A-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28535() ) );                  // 02A-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case28706() ) );                  // 02A-017
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29367() ) );                 // 02A-018
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29368() ) );                 // 02A-019
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02A_Case29368() ) );                 // 02A-020

            #endregion ASPEN

            #region BUCKEYE

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_CaseXXXXX() ) );            // 02A-000   02B-000
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28160() ) );                    // 02A-021   02B-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case26531() ) );                    // 02A-022   02B-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29270_SubmittedRequests() ) );  // 02A-023   02B-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29145_MultiPrint() ) );         // 02A-024   02B-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28753() ) );                    // 02A-025   02B-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28899_01() ) );                 // 02A-026   02B-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29287() ) );                    // 02A-027   02B-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case25871() ) );                    // 02A-028   02B-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28778() ) );                    // 02A-029   02B-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29401() ) );                    // 02A-030   02B-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29320_PrintJobs() ) );          // 02A-031   02B-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29335_DisableSchedules() ) );   // 02A-032   02B-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29254() ) );                    // 02A-033   02B-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28768() ) );                    // 02A-034   02B-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29211() ) );                    // 02A-035   02B-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28690B() ) );                   // 02A-036   02B-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28690C() ) );                   // 02A-037   02B-017
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28690D() ) );                   // 02A-038   02B-018
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28874() ) );                    // 02A-039   02B-019

            #endregion BUCKEYE

            // This automatically detects the latest version
            _LatestVersion = _MinimumVersion;
            foreach( CswSchemaVersion Version in _UpdateDrivers.Keys.Where( Version => _LatestVersion == _MinimumVersion ||
                                                                                        ( _LatestVersion.CycleIteration == Version.CycleIteration &&
                                                                                            _LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier &&
                                                                                            _LatestVersion.ReleaseIteration < Version.ReleaseIteration ) ) )
            {
                _LatestVersion = Version;
            }

            #region Before Scripts

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01() ), RunBeforeEveryExecutionOfUpdater_01.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01M() ), RunBeforeEveryExecutionOfUpdater_01M.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01OC() ), RunBeforeEveryExecutionOfUpdater_01OC.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02SQL() ), RunBeforeEveryExecutionOfUpdater_02SQL.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02() ), RunBeforeEveryExecutionOfUpdater_02.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_03() ), RunBeforeEveryExecutionOfUpdater_03.Title );

            #endregion Before Scripts

            #region After Scripts

            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_01() ), RunAfterEveryExecutionOfUpdater_01.Title );

            #endregion After Scripts

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
            }
        }

        public void stampSchemaVersion( CswNbtResources CswNbtResources, CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswNbtResources.ConfigVbls.setConfigVariableValue( "schemaversion", CswSchemaUpdateDriver.SchemaVersion.ToString() ); ;
        }

        #endregion

        #region Versioned scripts

        CswSchemaVersion _makeNextSchemaVersion()
        {
            int SuperCycle = _MinimumVersion.CycleIteration;
            char ReleaseIdentifier = _MinimumVersion.ReleaseIdentifier;
            if( 'Y' != ReleaseIdentifier )
            {
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWY".ToCharArray(); //No X or Z
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
        }

        private void _addVersionedScript( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswSchemaUpdateDriver.SchemaVersion = _makeNextSchemaVersion();
            CswSchemaUpdateDriver.Description = CswSchemaUpdateDriver.SchemaVersion.ToString(); //we do this in prod scripts because test scripts have a different dispensation for description
            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );
        }

        #endregion

        #region Run-always scripts

        private List<CswSchemaUpdateDriver> _RunBeforeScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunBeforeScripts
        {
            get
            {
                return ( _RunBeforeScripts );
            }
        }

        private void _addRunBeforeScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', _RunBeforeScripts.Count );
            CswSchemaUpdateDriver.Description = Description;
            if( false == _RunBeforeScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunBeforeScripts.Add( CswSchemaUpdateDriver );
            }
        }

        private List<CswSchemaUpdateDriver> _RunAfterScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunAfterScripts
        {
            get
            {
                return ( _RunAfterScripts );
            }

        }
        private void _addRunAfterScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 99, '#', _RunAfterScripts.Count );
            CswSchemaUpdateDriver.Description = Description;
            if( false == _RunAfterScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunAfterScripts.Add( CswSchemaUpdateDriver );
            }

        }

        #endregion

    }//CswScriptCollections
}//ChemSW.Nbt.Schema
