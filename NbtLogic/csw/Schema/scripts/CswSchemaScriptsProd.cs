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
        private CswNbtResources _CswNbtResources;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();


        public CswSchemaScriptsProd( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

			// This is where you manually set to the last version of the previous release
			_MinimumVersion = new CswSchemaVersion( 1, 'G', 32 );

			// This is where you add new versions.
            CswSchemaUpdateDriver Schema01H01Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H01( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H01Driver.SchemaVersion, Schema01H01Driver );
            CswSchemaUpdateDriver Schema01H02Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H02( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H02Driver.SchemaVersion, Schema01H02Driver );
            CswSchemaUpdateDriver Schema01H03Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H03( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H03Driver.SchemaVersion, Schema01H03Driver );
            CswSchemaUpdateDriver Schema01H04Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H04( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H04Driver.SchemaVersion, Schema01H04Driver );
            CswSchemaUpdateDriver Schema01H05Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H05( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H05Driver.SchemaVersion, Schema01H05Driver );
            CswSchemaUpdateDriver Schema01H06Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H06( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H06Driver.SchemaVersion, Schema01H06Driver );
            CswSchemaUpdateDriver Schema01H07Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H07( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H07Driver.SchemaVersion, Schema01H07Driver );
            CswSchemaUpdateDriver Schema01H08Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H08( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H08Driver.SchemaVersion, Schema01H08Driver );
            CswSchemaUpdateDriver Schema01H09Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H09( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H09Driver.SchemaVersion, Schema01H09Driver );
            CswSchemaUpdateDriver Schema01H10Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H10( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H10Driver.SchemaVersion, Schema01H10Driver );
            CswSchemaUpdateDriver Schema01H11Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H11( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H11Driver.SchemaVersion, Schema01H11Driver );
            CswSchemaUpdateDriver Schema01H12Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H12( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H12Driver.SchemaVersion, Schema01H12Driver );
            CswSchemaUpdateDriver Schema01H13Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H13( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H13Driver.SchemaVersion, Schema01H13Driver );
            CswSchemaUpdateDriver Schema01H14Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H14( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H14Driver.SchemaVersion, Schema01H14Driver );
            CswSchemaUpdateDriver Schema01H15Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H15( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H15Driver.SchemaVersion, Schema01H15Driver );
            CswSchemaUpdateDriver Schema01H16Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H16( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H16Driver.SchemaVersion, Schema01H16Driver );
            CswSchemaUpdateDriver Schema01H17Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H17( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H17Driver.SchemaVersion, Schema01H17Driver );
            CswSchemaUpdateDriver Schema01H18Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H18( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H18Driver.SchemaVersion, Schema01H18Driver );
            CswSchemaUpdateDriver Schema01H19Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H19( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H19Driver.SchemaVersion, Schema01H19Driver );
            CswSchemaUpdateDriver Schema01H20Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H20( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H20Driver.SchemaVersion, Schema01H20Driver );
            CswSchemaUpdateDriver Schema01H21Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H21( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H21Driver.SchemaVersion, Schema01H21Driver );
            CswSchemaUpdateDriver Schema01H22Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H22( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H22Driver.SchemaVersion, Schema01H22Driver );
            CswSchemaUpdateDriver Schema01H23Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H23( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H23Driver.SchemaVersion, Schema01H23Driver );
            CswSchemaUpdateDriver Schema01H24Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H24( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H24Driver.SchemaVersion, Schema01H24Driver );
            CswSchemaUpdateDriver Schema01H25Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H25( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H25Driver.SchemaVersion, Schema01H25Driver );
            CswSchemaUpdateDriver Schema01H26Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H26( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H26Driver.SchemaVersion, Schema01H26Driver );
            CswSchemaUpdateDriver Schema01H27Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H27( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H27Driver.SchemaVersion, Schema01H27Driver );
            CswSchemaUpdateDriver Schema01H28Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H28( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H28Driver.SchemaVersion, Schema01H28Driver );
            CswSchemaUpdateDriver Schema01H29Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H29( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H29Driver.SchemaVersion, Schema01H29Driver );
            CswSchemaUpdateDriver Schema01H30Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H30( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H30Driver.SchemaVersion, Schema01H30Driver );
            CswSchemaUpdateDriver Schema01H31Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H31( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H31Driver.SchemaVersion, Schema01H31Driver );
            CswSchemaUpdateDriver Schema01H32Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H32( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H32Driver.SchemaVersion, Schema01H32Driver );
            CswSchemaUpdateDriver Schema01H33Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H33( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H33Driver.SchemaVersion, Schema01H33Driver );
            CswSchemaUpdateDriver Schema01H34Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H34( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H34Driver.SchemaVersion, Schema01H34Driver );
            CswSchemaUpdateDriver Schema01H35Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H35( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H35Driver.SchemaVersion, Schema01H35Driver );
			CswSchemaUpdateDriver Schema01H36Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H36( _CswNbtSchemaModTrnsctn ) );
			_UpdateDrivers.Add( Schema01H36Driver.SchemaVersion, Schema01H36Driver );
			CswSchemaUpdateDriver Schema01H37Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H37( _CswNbtSchemaModTrnsctn ) );
			_UpdateDrivers.Add( Schema01H37Driver.SchemaVersion, Schema01H37Driver );
            CswSchemaUpdateDriver Schema01H38Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H38( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H38Driver.SchemaVersion, Schema01H38Driver );
            CswSchemaUpdateDriver Schema01H39Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H39( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H39Driver.SchemaVersion, Schema01H39Driver );
            CswSchemaUpdateDriver Schema01H40Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H40( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H40Driver.SchemaVersion, Schema01H40Driver );

            // This automatically detects the latest version
            foreach( CswSchemaVersion Version in _UpdateDrivers.Keys.Where( Version => _LatestVersion == null ||
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

        public CswSchemaVersion CurrentVersion
        {
            get
            {
                return ( new CswSchemaVersion( _CswNbtResources.getConfigVariableValue( "schemaversion" ) ) );
            }
        }


        public CswSchemaVersion TargetVersion
        {
            get
            {
                CswSchemaVersion ret = null;
                if( CurrentVersion == MinimumVersion )
                    ret = new CswSchemaVersion( LatestVersion.CycleIteration, LatestVersion.ReleaseIdentifier, 1 );
                else
                    ret = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration + 1 );
                return ret;
            }
        }


        public CswSchemaUpdateDriver Next
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;


                if( CurrentVersion == MinimumVersion ||
                    ( LatestVersion.CycleIteration == CurrentVersion.CycleIteration &&
                      LatestVersion.ReleaseIdentifier == CurrentVersion.ReleaseIdentifier &&
                      LatestVersion.ReleaseIteration > CurrentVersion.ReleaseIteration ) )
                {
                    ReturnVal = _UpdateDrivers[TargetVersion];
                }



                return ( ReturnVal );
            }
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

        public void stampSchemaVersion( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            _CswNbtResources.setConfigVariableValue( "schemaversion", CswSchemaUpdateDriver.SchemaVersion.ToString() ); ;
        }//stampSchemaVersion()


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
