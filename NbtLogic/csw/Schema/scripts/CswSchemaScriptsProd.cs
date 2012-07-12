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
            _MinimumVersion = new CswSchemaVersion( 1, 'O', 21 );

            // This is where you add new versions.
            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );


            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25978() ) );                     //01P-01
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase7608() ) );                      //01P-02
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24645() ) );                     //01P-03
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24514ObjectClass() ) );          //01P-04
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26111() ) );                     //01P-05
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24514NodeType() ) );             //01P-06
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24514Action() ) );               //01P-07
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );                     //01P-08
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25449() ) );                     //01P-09
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24508() ) );                     //01P-10
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24441() ) );                     //01P-11
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26572() ) );                     //01P-12
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26579() ) );                     //01P-13
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24485() ) );                     //01P-14
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24514RequestButton() ) );        //01P-15
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26471() ) );                     //01P-16
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26692SetPreferred() ) );         //01P-17
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24517InventoryLevels() ) );      //01P-18
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24508B() ) );                    //01P-19
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26410() ) );                     //01P-20
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25118() ) );                     //01P-21
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25759() ) );                     //01P-22
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25814() ) );                     //01P-23
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26665() ) );                     //01P-24
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24507() ) );                     //01P-25
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24517CurrentQuantity() ) );      //01P-26
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26575() ) );                     //01P-27
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26584() ) );                     //01P-28
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26763() ) );                     //01P-29
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24517InventoryLevelsViews() ) ); //01P-30
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase9111() ) );                      //01P-31
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26758RequestButton() ) );        //01P-32
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24508C() ) );                    //01P-33
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26609() ) );                     //01P-34
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25898() ) );                     //01P-35
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26834() ) );                     //01P-36
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24446Action() ) );               //01P-37
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26731() ) );                     //01P-38
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26757() ) );                     //01P-39
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26835() ) );                     //01P-40
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26806() ) );                     //01P-41
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24523() ) );                     //01P-42
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26833() ) );                     //01P-43
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26887() ) );                     //01P-44
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26837() ) );                     //01P-45
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24446Container() ) );            //01P-46
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26965() ) );                     //01P-47
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24508D() ) );                    //01P-48
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );                     //01P-49 -- case 27091
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );                     //01P-50 -- case 27091
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCaseXXXXX() ) );                     //01P-51 -- case 27091
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26714() ) );                     //01P-52
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26758Fixed() ) );                //01P-53
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25291GHS() ) );                  //01P-54
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26697() ) );                     //01P-55
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26831() ) );                     //01P-56
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26896() ) );                     //01P-57
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26772() ) );                     //01P-58
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26829() ) );                     //01P-59
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26974() ) );                     //01P-60
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24486() ) );                     //01P-61
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase26683() ) );                     //01P-62
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27088() ) );                     //01P-63 
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase27071RequestFulfillment() ) );   //01P-64
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24446Action2() ) );              //01P-65

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
