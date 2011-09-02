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

		private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
		public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }
		
		public CswSchemaScriptsProd( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;

			// This is where you manually set to the last version of the previous release
			_MinimumVersion = new CswSchemaVersion( 1, 'H', 60 );

			// This is where you add new versions.
			CswSchemaUpdateDriver Schema01I01Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I01() );
			_UpdateDrivers.Add( Schema01I01Driver.SchemaVersion, Schema01I01Driver );
			CswSchemaUpdateDriver Schema01I02Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I02() );
			_UpdateDrivers.Add( Schema01I02Driver.SchemaVersion, Schema01I02Driver );
			CswSchemaUpdateDriver Schema01I03Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I03() );
			_UpdateDrivers.Add( Schema01I03Driver.SchemaVersion, Schema01I03Driver );
			CswSchemaUpdateDriver Schema01I04Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I04() );
			_UpdateDrivers.Add( Schema01I04Driver.SchemaVersion, Schema01I04Driver );
			CswSchemaUpdateDriver Schema01I05Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I05() );
			_UpdateDrivers.Add( Schema01I05Driver.SchemaVersion, Schema01I05Driver );
			CswSchemaUpdateDriver Schema01I06Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I06() );
			_UpdateDrivers.Add( Schema01I06Driver.SchemaVersion, Schema01I06Driver );
			CswSchemaUpdateDriver Schema01I07Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I07() );
			_UpdateDrivers.Add( Schema01I07Driver.SchemaVersion, Schema01I07Driver );
			CswSchemaUpdateDriver Schema01I08Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I08() );
			_UpdateDrivers.Add( Schema01I08Driver.SchemaVersion, Schema01I08Driver );
			CswSchemaUpdateDriver Schema01I09Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I09() );
			_UpdateDrivers.Add( Schema01I09Driver.SchemaVersion, Schema01I09Driver );
			CswSchemaUpdateDriver Schema01I10Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I10() );
			_UpdateDrivers.Add( Schema01I10Driver.SchemaVersion, Schema01I10Driver );
			CswSchemaUpdateDriver Schema01I11Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I11() );
			_UpdateDrivers.Add( Schema01I11Driver.SchemaVersion, Schema01I11Driver );
			CswSchemaUpdateDriver Schema01I12Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I12() );
			_UpdateDrivers.Add( Schema01I12Driver.SchemaVersion, Schema01I12Driver );
			CswSchemaUpdateDriver Schema01I13Driver = new CswSchemaUpdateDriver( new CswUpdateSchemaTo01I13() );
			_UpdateDrivers.Add( Schema01I13Driver.SchemaVersion, Schema01I13Driver );

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
