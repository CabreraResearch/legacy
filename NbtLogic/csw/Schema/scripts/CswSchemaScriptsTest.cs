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
        List<CswSchemaUpdateDriver> _UpdateDrivers = new List<CswSchemaUpdateDriver>();


        public CswSchemaScriptsTest( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );


            _UpdateDrivers.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_001_01( _CswNbtSchemaModTrnsctn ) ) );
            _UpdateDrivers.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_001_02( _CswNbtSchemaModTrnsctn ) ) );
            _UpdateDrivers.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_001_03( _CswNbtSchemaModTrnsctn ) ) );
            _UpdateDrivers.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswTstUpdtSchema_001_04( _CswNbtSchemaModTrnsctn ) ) );

        }//ctor


        public CswSchemaVersion LatestVersion
        {
            get { return ( _UpdateDrivers[_UpdateDrivers.Count - 1].SchemaVersion ); }
        }

        public CswSchemaVersion MinimumVersion
        {
            get { return ( new CswSchemaVersion( 001, 'T', 00 ) ); }
        }

        public CswSchemaVersion CurrentVersion
        {
            get
            {
                CswSchemaVersion ReturnVal = MinimumVersion;
                if( _CurrentIdx >= 0 )
                {
                    ReturnVal = _UpdateDrivers[_CurrentIdx].SchemaVersion;
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


                if( _UpdateDrivers.Count > _CurrentIdx )
                {
                    if( Int32.MinValue == _CurrentIdx )
                    {
                        _CurrentIdx = 0;
                    }
                    else
                    {
                        _CurrentIdx++;
                    }

                    ReturnVal = _UpdateDrivers[_CurrentIdx];
                }

                return ( ReturnVal );

            }//get

        }//Next



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
