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
        //private CswNbtResources _CswNbtResources;

        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        public CswSchemaScriptsProd() //CswNbtResources CswNbtResources )
        {
            //_CswNbtResources = CswNbtResources;

            // This is where you manually set to the last version of the previous release
            _MinimumVersion = new CswSchemaVersion( 1, 'I', 14 );

            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J01() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J02() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J03() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J04() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J05() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J06() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J07() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J08() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J09() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J010() ) );
            addReleaseDmlDriver( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01J011() ) );



            // This is where you add new versions.
                      // This automatically detects the latest version
            _LatestVersion = _MinimumVersion;
            foreach( CswSchemaVersion Version in _UpdateDrivers.Keys.Where( Version => _LatestVersion == _MinimumVersion ||
                                                                                        ( _LatestVersion.CycleIteration == Version.CycleIteration &&
                                                                                            _LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier &&
                                                                                            _LatestVersion.ReleaseIteration < Version.ReleaseIteration ) ) )
            {
                _LatestVersion = Version;
            }

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
            return ( new CswSchemaVersion( CswNbtResources.getConfigVariableValue( "schemaversion" ) ) );
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
            CswNbtResources.setConfigVariableValue( "schemaversion", CswSchemaUpdateDriver.SchemaVersion.ToString() ); ;
        }//stampSchemaVersion()


        #endregion



        #region #script order control
        //These methods enable us to implemetn the Schema Updater Reform Act of 2011 (case 23787) 
        //whilst allowing all the script iteration mechanisms to work business as usual
        public void addUniversalPreProcessDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            if( ( 0 != CswSchemaUpdateDriver.SchemaVersion.CycleIteration ) ||
                ( "A" != CswSchemaUpdateDriver.SchemaVersion.ReleaseIdentifier.ToString().ToUpper() ) )
            {
                throw ( new CswDniException( "Schema version " + CswSchemaUpdateDriver.SchemaVersion.ToString() + " cannot be a universal preprocess script; it's version must be in format 00-A-xx" ) );
            }

            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );

        }//addPreProcessDriver


        public void addReleaseDdlDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            if( ( 99 == CswSchemaUpdateDriver.SchemaVersion.CycleIteration ) ||
                  ( 0 == CswSchemaUpdateDriver.SchemaVersion.CycleIteration ) ||
                  ( "A" == CswSchemaUpdateDriver.SchemaVersion.ReleaseIdentifier.ToString().ToUpper() ) ||
                  ( "Z" == CswSchemaUpdateDriver.SchemaVersion.ReleaseIdentifier.ToString().ToUpper() ) ||
                  ( 0 != CswSchemaUpdateDriver.SchemaVersion.ReleaseIteration )
                )
            {
                throw ( new CswDniException( "Schema version " + CswSchemaUpdateDriver.SchemaVersion.ToString() + " cannot be a release-specific ddl script; it's version must be: CycleIteration > 0 and < 99; ReleaseIdentifier > A and < Z; ReleaseIteration == 0" ) );
            }

            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );


        }//addReleaseDdlDriver() 

        public void addReleaseDmlDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            if( ( 99 == CswSchemaUpdateDriver.SchemaVersion.CycleIteration ) ||
                  ( 0 == CswSchemaUpdateDriver.SchemaVersion.CycleIteration ) ||
                  ( "A" == CswSchemaUpdateDriver.SchemaVersion.ReleaseIdentifier.ToString().ToUpper() ) ||
                  ( "Z" == CswSchemaUpdateDriver.SchemaVersion.ReleaseIdentifier.ToString().ToUpper() ) ||
                  ( 0 == CswSchemaUpdateDriver.SchemaVersion.ReleaseIteration )
                )
            {
                throw ( new CswDniException( "Schema version " + CswSchemaUpdateDriver.SchemaVersion.ToString() + " cannot be a release-specific dml script; it's version must be: CycleIteration > 0 and < 99; ReleaseIdentifier > A and < Z; ReleaseIteration > 0" ) );
            }

            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );

        }//addReleaseDmlDriver() 


        public void addUniversalPostProcessDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            if( ( 99 != CswSchemaUpdateDriver.SchemaVersion.CycleIteration ) ||
                ( "Z" != CswSchemaUpdateDriver.SchemaVersion.ReleaseIdentifier.ToString().ToUpper() ) )
            {
                throw ( new CswDniException( "Schema version " + CswSchemaUpdateDriver.SchemaVersion.ToString() + " cannot be a universal post-process script; it's version must be in format 99-Z-xx" ) );
            }

            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );

        }//addPreProcessDriver        
        #endregion



        //#region IEnumerable
        //public IEnumerator<CswSchemaUpdateDriver> GetEnumerator()
        //{
        //    return ( new CswSchemaScriptsProdEnumerator( this ) );
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ( new CswSchemaScriptsProdEnumerator( this ) );
        //}


        //private class CswSchemaScriptsProdEnumerator : IEnumerator<CswSchemaUpdateDriver>
        //{
        //    private CswSchemaScriptsProd _CswSchemaScriptsProd = null;
        //    public CswSchemaScriptsProdEnumerator( CswSchemaScriptsProd CswSchemaScriptsProd )
        //    {
        //        _CswSchemaScriptsProd = CswSchemaScriptsProd;
        //    }

        //    public CswSchemaUpdateDriver Current
        //    {
        //        get { throw new NotImplementedException(); }
        //    }

        //    public void Dispose()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    object IEnumerator.Current
        //    {
        //        get { throw new NotImplementedException(); }
        //    }

        //    public bool MoveNext()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void Reset()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}//CswSchemaScriptsProdEnumerator

        //#endregion



    }//CswScriptCollections

}//ChemSW.Nbt.Schema
