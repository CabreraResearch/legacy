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
            _MinimumVersion = new CswSchemaVersion( 2, 'A', 20 );

            // This is where you add new versions.

            #region BUCKEYE

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_CaseXXXXX() ) );                  // 02B-000
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28160() ) );                          // 02B-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case26531() ) );                          // 02B-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29270_SubmittedRequests() ) );        // 02B-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29145_MultiPrint() ) );               // 02B-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28753() ) );                          // 02B-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28899_01() ) );                       // 02B-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29287() ) );                          // 02B-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case25871() ) );                          // 02B-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28778() ) );                          // 02B-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29401() ) );                          // 02B-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29320_PrintJobs() ) );                // 02B-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29335_DisableSchedules() ) );         // 02B-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29254() ) );                          // 02B-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29211() ) );                          // 02B-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28690B() ) );                         // 02B-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28690C() ) );                         // 02B-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28690D() ) );                         // 02B-017
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28874() ) );                          // 02B-018
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case28690E() ) );                         // 02B-019
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29431() ) );                          // 02B-020
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29512() ) );                          // 02B-021
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29599() ) );                          // 02B-022
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29630() ) );                          // 02B-023
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29703() ) );                          // 02B-024
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29254_InspectionDeficiencies() ) );   // 02B-025
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29691() ) );                          // 02B-026
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_CaseXXXXX() ) );                          // 02B-027
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29684() ) );                          // 02B-028
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29773() ) );                          // 02B-029
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29798() ) );                          // 02B-030
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02B_Case29707() ) );                          // 02B-031

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
