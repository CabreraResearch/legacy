using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.DB;
using ChemSW.Exceptions;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsTest : ICswSchemaScripts
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        private List<CswSchemaUpdateDriver> _UpdateDriverList = new List<CswSchemaUpdateDriver>();
        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();

        public CswSchemaScriptsTest( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            //***************************************************************************************
            //******************************  Test Case 001
            CswSchemaUpdateDriver Schema_01_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_001_01_001( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_01_Driver.SchemaVersion, Schema_01_Driver );

            CswSchemaUpdateDriver Schema_02_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_001_02_002( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_02_Driver.SchemaVersion, Schema_02_Driver );

            CswSchemaUpdateDriver Schema_03_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_001_03_003( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_03_Driver.SchemaVersion, Schema_03_Driver );

            CswSchemaUpdateDriver Schema_04_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_001_04_004( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_04_Driver.SchemaVersion, Schema_04_Driver );

            //***************************************************************************************
            //******************************  Test Case 002
            CswSchemaUpdateDriver Schema_05_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_002_01_005( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_05_Driver.SchemaVersion, Schema_05_Driver );

            CswSchemaUpdateDriver Schema_06_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_002_02_006( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_06_Driver.SchemaVersion, Schema_06_Driver );


            //***************************************************************************************
            //******************************  Test Case 003
            CswSchemaUpdateDriver Schema_07_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_003_01_007( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_07_Driver.SchemaVersion, Schema_07_Driver );

            CswSchemaUpdateDriver Schema_08_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_003_02_008( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_08_Driver.SchemaVersion, Schema_08_Driver );

            CswSchemaUpdateDriver Schema_09_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_003_03_009( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_09_Driver.SchemaVersion, Schema_09_Driver );

            CswSchemaUpdateDriver Schema_10_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_003_04_010( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_10_Driver.SchemaVersion, Schema_10_Driver );

            CswSchemaUpdateDriver Schema_11_Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTestCase_003_05_011( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema_11_Driver.SchemaVersion, Schema_11_Driver );


            foreach( CswSchemaUpdateDriver CurrentDriver in _UpdateDrivers.Values )
            {
                _UpdateDriverList.Add( CurrentDriver );

            }

            //_UpdateDriverList.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_01( _CswNbtSchemaModTrnsctn ) ) );
            //_UpdateDriverList.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_02( _CswNbtSchemaModTrnsctn ) ) );
            //_UpdateDriverList.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_03( _CswNbtSchemaModTrnsctn ) ) );
            //_UpdateDriverList.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_04( _CswNbtSchemaModTrnsctn ) ) );

        }//ctor


        public CswSchemaVersion LatestVersion
        {
            get { return ( _UpdateDriverList[_UpdateDriverList.Count - 1].SchemaVersion ); }
        }

        public CswSchemaVersion MinimumVersion
        {
            get { return ( new CswSchemaVersion( 1, 'T', 00 ) ); }
        }

        public CswSchemaVersion CurrentVersion
        {
            get
            {
                CswSchemaVersion ReturnVal = MinimumVersion;
                if( _CurrentIdx >= 0 )
                {
                    ReturnVal = _UpdateDriverList[_CurrentIdx].SchemaVersion;
                }

                return ( ReturnVal );
            }//get

        }//CurrentVersion

        public CswSchemaVersion TargetVersion
        {
            get
            {
                CswSchemaVersion ret = null;
                if( CurrentVersion == MinimumVersion )
                    ret = new CswSchemaVersion( LatestVersion.CycleIteration, LatestVersion.ReleaseIdentifier, 1 );
                else
                    ret = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration );
                return ret;
            }
        }//TargetVersion

        private Int32 _CurrentIdx = Int32.MinValue;
        public CswSchemaUpdateDriver Next
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;


                if( _UpdateDriverList.Count > _CurrentIdx )
                {
                    if( Int32.MinValue == _CurrentIdx )
                    {
                        _CurrentIdx = 0;
                    }
                    else
                    {
                        _CurrentIdx++;
                    }

                    ReturnVal = _UpdateDriverList[_CurrentIdx];
                }

                return ( ReturnVal );

            }//get

        }//Next

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
            _CswNbtResources.CswLogger.reportAppState( "Succesfully ran schema updater test " + CswSchemaUpdateDriver.Description );
        }//stampSchemaVersion()


        //#region IEnumerable
        //public IEnumerator<CswSchemaUpdateDriver> GetEnumerator()
        //{
        //    return ( new CswSchemaScriptsTestEnumerator( this ) );
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ( new CswSchemaScriptsTestEnumerator( this ) );
        //}


        //private class CswSchemaScriptsTestEnumerator : IEnumerator<CswSchemaUpdateDriver>
        //{
        //    private CswSchemaScriptsTest _CswSchemaScriptsTest = null;
        //    public CswSchemaScriptsTestEnumerator( CswSchemaScriptsTest CswSchemaScriptsTest )
        //    {
        //        _CswSchemaScriptsTest = CswSchemaScriptsTest;
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
        //}//CswSchemaScriptsTestEnumerator

        //#endregion
    }//CswScriptCollections

}//ChemSW.Nbt.Schema
