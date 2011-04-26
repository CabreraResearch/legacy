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


        public CswSchemaVersion CurrentVersion { get { return new CswSchemaVersion( 000, 'T', 00 ); } }



        /// <summary>
        /// The highest schema version number defined in the updater
        /// </summary>

    }//CswScriptCollections

}//ChemSW.Nbt.Schema
