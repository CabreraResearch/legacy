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
            _MinimumVersion = new CswSchemaVersion( 2, 'E', 11 );

            #region FOXGLOVE

            _addVersionedScript( new CswUpdateSchema_02F_Case30281() );                    //02E-012 //02F-001
            _addVersionedScript( new CswUpdateSchema_02F_Case28998() );                    //02E-013 //02F-002
            _addVersionedScript( new CswUpdateSchema_02F_Case29973() );                    //02E-014 //02F-003
            _addVersionedScript( new CswUpdateSchema_02F_Case29191() );                    //02E-015 //02F-004
            _addVersionedScript( new CswUpdateSchema_02F_Case29542() );                    //02E-016 //02F-005
            _addVersionedScript( new CswUpdateSchema_02F_Case29438() );                    //02E-017 //02F-006
            _addVersionedScript( new CswUpdateSchema_02F_Case30082_UserCache() );          //02E-018 //02F-007
            _addVersionedScript( new CswUpdateSchema_02F_Case30197() );                    //02E-019 //02F-008
            _addVersionedScript( new CswUpdateSchema_02F_Case30417() );                    //02E-020 //02F-009
            _addVersionedScript( new CswUpdateSchema_02F_Case27883() );                    //02E-021 //02F-010
            _addVersionedScript( new CswUpdateSchema_02F_Case27495() );                    //02E-022 //02F-011
            _addVersionedScript( new CswUpdateSchema_02F_Case30228() );                    //02E-023 //02F-012
            _addVersionedScript( new CswUpdateSchema_02F_Case30040() );                    //02E-024 //02F-013
            _addVersionedScript( new CswUpdateSchema_02F_Case30041_Vendors() );            //02E-025 //02F-014
            _addVersionedScript( new CswUpdateSchema_02F_Case29992() );                    //02E-026 //02F-015
            _addVersionedScript( new CswUpdateSchema_02F_Case29402() );                    //02E-027 //02F-016
            _addVersionedScript( new CswUpdateSchema_02F_Case30041_UnitsOfMeasure() );     //02E-028 //02F-017
            _addVersionedScript( new CswUpdateSchema_02F_Case30041_RolesUsers() );         //02E-029 //02F-018
            _addVersionedScript( new CswUpdateSchema_02F_Case30252() );                    //02E-019 //02F-008
            _addVersionedScript( new CswUpdateSchema_02F_Case30041_ScheduledRuleImport() );//02E-030 //02F-019
            _addVersionedScript( new CswUpdateSchema_02F_Case30043_ControlZones() );       //02E-030 //02F-019
            _addVersionedScript( new CswUpdateSchema_02F_Case30043_Sites() );              //02E-031 //02F-020
            _addVersionedScript( new CswUpdateSchema_02F_Case29984()  );                   //02E-031 //02F-020
            _addVersionedScript( new CswUpdateSchema_02F_Case30577() );                    //02E-032 //02F-021
            _addVersionedScript( new CswUpdateSchema_02F_Case30043_Locations() );          //02E-032 //02F-021


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

            //dch 30252 FOXGLOVE, but metadata changes so before EUC changes
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30252() );
            //This script needs to go first
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30228() );

            #region FOXGLOVE Run Before Scripts

            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30041_NbtImportQueue() ); //Validate the Nbt Import Queue table first
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30281() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30251() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30251B() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30082_UserCache() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case27883() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30040() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case29992() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02F_Case30529() );

            #endregion FOXGLOVE Run Before Scripts

            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_MakeMissingNodeTypeProps() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02SQL() );
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_03() );

            #endregion Before Scripts

            #region After Scripts

            _addRunAfterScript( new RunAfterEveryExecutionOfUpdater_01() );

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

        private void _addVersionedScript( CswUpdateSchemaTo UpdateTo )
        {
            CswSchemaUpdateDriver CswSchemaUpdateDriver = new CswSchemaUpdateDriver( UpdateTo );
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

        private List<CswSchemaUpdateDriver> _RunAfterScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunAfterScripts
        {
            get
            {
                return ( _RunAfterScripts );
            }

        }

        private void _addRunBeforeScript( CswUpdateSchemaTo UpdateTo, string Description = null )
        {
            CswSchemaUpdateDriver CswSchemaUpdateDriver = new CswSchemaUpdateDriver( UpdateTo );
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', _RunBeforeScripts.Count );
            CswSchemaUpdateDriver.Description += Description ?? string.Empty;
            if( false == _RunBeforeScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunBeforeScripts.Add( CswSchemaUpdateDriver );
            }
        }

        private void _addRunAfterScript( CswUpdateSchemaTo UpdateTo, string Description = null )
        {
            CswSchemaUpdateDriver CswSchemaUpdateDriver = new CswSchemaUpdateDriver( UpdateTo );
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 99, '#', _RunAfterScripts.Count );
            CswSchemaUpdateDriver.Description += Description ?? string.Empty;
            if( false == _RunAfterScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunAfterScripts.Add( CswSchemaUpdateDriver );
            }
        }

        #endregion

    }//CswScriptCollections
}//ChemSW.Nbt.Schema
