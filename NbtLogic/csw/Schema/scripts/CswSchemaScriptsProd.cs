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
            _MinimumVersion = new CswSchemaVersion( 2, 'D', 23 );

            // This is where you add new versions.
            #region EUCALYPTUS

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30014() ) );                    //02E-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30222() ) );                    //02E-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case29847() ) );                    //02E-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30123() ) );                    //02E-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30370() ) );                    //02E-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30360() ) );                    //02E-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30339_UserProfilex2() ) );      //02E-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30300() ) );                    //02E-008 
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30440() ) );                    //02E-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30445() ) );                    //02E-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30360() ) );                    //02E-011 
            
            #endregion EUCALYPTUS

            #region FOXGLOVE
            

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_CaseXXXXX() ) );            //02E-011 //02F-000 
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case30281() ) );                    //02E-012 //02F-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case28998() ) );                    //02E-013 //02F-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29973() ) );                    //02E-014 //02F-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29191() ) );                    //02E-015 //02F-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29542() ) );                    //02E-016 //02F-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29438() ) );                    //02E-017 //02F-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case30082_UserCache() ) );          //02E-018 //02F-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case30197() ) );                    //02E-019 //02F-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case27883() ) );                    //02E-020 //02F-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case27495() ) );                    //02E-021 //02F-010


            #endregion FOXGLOVE

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

                #region EUCALYPTUS Run Before Scripts

                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case29700() ), RunBeforeEveryExecutionOfUpdater_02E_Case29700.Title );
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case30123() ), RunBeforeEveryExecutionOfUpdater_02E_Case30123.Title );
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case29701() ), RunBeforeEveryExecutionOfUpdater_02E_Case29701.Title );
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case30347() ), RunBeforeEveryExecutionOfUpdater_02E_Case30347.Title );

                #endregion EUCALYPTUS Run Before Scripts

                #region FOXGLOVE Run Before Scripts

                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30281() ), RunBeforeEveryExecutionOfUpdater_02F_Case30281.Title );
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30251() ), RunBeforeEveryExecutionOfUpdater_02F_Case30251.Title );
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30251B() ), RunBeforeEveryExecutionOfUpdater_02F_Case30251B.Title );
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30082_UserCache() ), RunBeforeEveryExecutionOfUpdater_02F_Case30082_UserCache.Title );
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case27883() ), RunBeforeEveryExecutionOfUpdater_02F_Case27883.Title );
                
                #endregion FOXGLOVE Run Before Scripts

                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_MakeMissingNodeTypeProps() ), RunBeforeEveryExecutionOfUpdater_MakeMissingNodeTypeProps.Title );
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
