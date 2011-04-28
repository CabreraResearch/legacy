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


        public CswSchemaScriptsTest( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
        }//ctor


        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> Scripts
        {
            get
            {

                Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> ReturnVal = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();

                CswSchemaUpdateDriver Schema_001t01Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchema_001_T_01( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema_001t01Driver.SchemaVersion, Schema_001t01Driver );

                return ( ReturnVal );
            }//get 
        }//Test


        public CswSchemaVersion LatestVersion
        {
            get { throw new NotImplementedException(); }
        }

        public CswSchemaVersion MinimumVersion
        {
            get { throw new NotImplementedException(); }
        }

        public CswSchemaVersion CurrentVersion
        {
            get { throw new NotImplementedException(); }
        }

        public CswSchemaVersion TargetVersion
        {
            get { throw new NotImplementedException(); }
        }

        public CswSchemaUpdateDriver Next
        {
            get { throw new NotImplementedException(); }
        }



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
