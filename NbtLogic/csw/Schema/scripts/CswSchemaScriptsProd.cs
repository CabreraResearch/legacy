using System.Collections.Generic;
using System.Linq;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsProd: ICswSchemaScripts
    {
        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        public CswSchemaScriptsProd()
        {
            // This is where you manually set to the last version of the previous release (the one currently in production)
            _MinimumVersion = new CswSchemaVersion( 2, 'C', 23 );

            // This is where you add new versions.

            #region DOGWOOD

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29407() ) );                    //02D-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30008() ) );                    //02D-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30010() ) );                    //02D-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29499() ) );                    //02D-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29570() ) );                    //02D-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29570B() ) );                   //02D-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29499B() ) );                   //02D-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29760() ) );                    //02D-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case28758() ) );                    //02D-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29846() ) );                    //02D-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30194() ) );                    //02D-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29965() ) );                    //02D-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29863_UserProfile() ) );        //02D-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30126() ) );                    //02D-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30254() ) );                    //02D-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30246() ) );                    //02D-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30239() ) );                    //02D-017

            #endregion DOGWOOD

            #region EUCALYPTUS

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_CaseXXXXX() ) );            //02D-017 //02E-000
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30014() ) );                    //02D-018 //02E-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30222() ) );                    //02D-019 //02E-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case29847() ) );                    //02D-020 //02E-003


            #endregion EUCALYPTUS

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

            //TODO: Remove this generic script
            _addRunBeforeScript( new CswSchemaUpdateDriver( new Deprecated_RunBeforeEveryExecutionOfUpdater_01OC() ), Deprecated_RunBeforeEveryExecutionOfUpdater_01OC.Title );

            #region Dogwood Run Before Scripts

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29833() ), "Case 29833: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29833B() ), "Case 29833B: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29499A() ), "Case 29499A: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29499B() ), "Case 29499B: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29570() ), RunBeforeEveryExecutionOfUpdater_02D_Case29570.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case30194_Pre() ), RunBeforeEveryExecutionOfUpdater_02D_Case30194_Pre.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case30194() ), RunBeforeEveryExecutionOfUpdater_02D_Case30194.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29863_UserPhone() ), "Case 29863: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case30008() ), "Case 30008: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case30010() ), "Case 30010: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case30090() ), "Case 30090: OC Script" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case30126() ), "Case 30126: OC Script" );

            #endregion

            #region EUCALYPTUS Run Before Scripts

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case29701() ), "Case 29701: OC Script" );

            #endregion EUCALYPTUS Run Before Scripts

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_MakeMissingNodeTypeProps() ), "MakeMissingNodeTypeProps" );

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02SQL() ), RunBeforeEveryExecutionOfUpdater_02SQL.Title );
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
