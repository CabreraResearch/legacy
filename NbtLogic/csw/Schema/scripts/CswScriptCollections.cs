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
    public class CswScriptCollections
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;


        public CswScriptCollections( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
        }//ctor


        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> Prod
        {
            get
            {

                Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> ReturnVal = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();

                // This is where you add new versions.
                CswSchemaUpdateDriver Schema01H01Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H01( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H01Driver.SchemaVersion, Schema01H01Driver );
                CswSchemaUpdateDriver Schema01H02Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H02( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H02Driver.SchemaVersion, Schema01H02Driver );
                CswSchemaUpdateDriver Schema01H03Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H03( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H03Driver.SchemaVersion, Schema01H03Driver );
                CswSchemaUpdateDriver Schema01H04Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H04( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H04Driver.SchemaVersion, Schema01H04Driver );
                CswSchemaUpdateDriver Schema01H05Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H05( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H05Driver.SchemaVersion, Schema01H05Driver );
                CswSchemaUpdateDriver Schema01H06Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H06( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H06Driver.SchemaVersion, Schema01H06Driver );
                CswSchemaUpdateDriver Schema01H07Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H07( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H07Driver.SchemaVersion, Schema01H07Driver );
                CswSchemaUpdateDriver Schema01H08Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H08( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H08Driver.SchemaVersion, Schema01H08Driver );
                CswSchemaUpdateDriver Schema01H09Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H09( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H09Driver.SchemaVersion, Schema01H09Driver );
                CswSchemaUpdateDriver Schema01H10Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H10( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H10Driver.SchemaVersion, Schema01H10Driver );
                CswSchemaUpdateDriver Schema01H11Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H11( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H11Driver.SchemaVersion, Schema01H11Driver );
                CswSchemaUpdateDriver Schema01H12Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H12( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H12Driver.SchemaVersion, Schema01H12Driver );
                CswSchemaUpdateDriver Schema01H13Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H13( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H13Driver.SchemaVersion, Schema01H13Driver );
                CswSchemaUpdateDriver Schema01H14Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H14( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H14Driver.SchemaVersion, Schema01H14Driver );
                CswSchemaUpdateDriver Schema01H15Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H15( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H15Driver.SchemaVersion, Schema01H15Driver );
                CswSchemaUpdateDriver Schema01H16Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H16( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H16Driver.SchemaVersion, Schema01H16Driver );
                CswSchemaUpdateDriver Schema01H17Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H17( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H17Driver.SchemaVersion, Schema01H17Driver );
                CswSchemaUpdateDriver Schema01H18Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H18( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H18Driver.SchemaVersion, Schema01H18Driver );
                CswSchemaUpdateDriver Schema01H19Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H19( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H19Driver.SchemaVersion, Schema01H19Driver );
                CswSchemaUpdateDriver Schema01H20Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H20( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H20Driver.SchemaVersion, Schema01H20Driver );
                CswSchemaUpdateDriver Schema01H21Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H21( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H21Driver.SchemaVersion, Schema01H21Driver );
                CswSchemaUpdateDriver Schema01H22Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H22( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H22Driver.SchemaVersion, Schema01H22Driver );
                CswSchemaUpdateDriver Schema01H23Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H23( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H23Driver.SchemaVersion, Schema01H23Driver );
                CswSchemaUpdateDriver Schema01H24Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H24( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H24Driver.SchemaVersion, Schema01H24Driver );
                CswSchemaUpdateDriver Schema01H25Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H25( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H25Driver.SchemaVersion, Schema01H25Driver );
                CswSchemaUpdateDriver Schema01H26Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H26( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H26Driver.SchemaVersion, Schema01H26Driver );
                CswSchemaUpdateDriver Schema01H27Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H27( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H27Driver.SchemaVersion, Schema01H27Driver );
                CswSchemaUpdateDriver Schema01H28Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H28( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H28Driver.SchemaVersion, Schema01H28Driver );
                CswSchemaUpdateDriver Schema01H29Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H29( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H29Driver.SchemaVersion, Schema01H29Driver );
                CswSchemaUpdateDriver Schema01H30Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H30( _CswNbtSchemaModTrnsctn ) );
                ReturnVal.Add( Schema01H30Driver.SchemaVersion, Schema01H30Driver );
				CswSchemaUpdateDriver Schema01H31Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H31( _CswNbtSchemaModTrnsctn ) );
				ReturnVal.Add( Schema01H31Driver.SchemaVersion, Schema01H31Driver );
				CswSchemaUpdateDriver Schema01H32Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H32( _CswNbtSchemaModTrnsctn ) );
				ReturnVal.Add( Schema01H32Driver.SchemaVersion, Schema01H32Driver );

                return ( ReturnVal );

            }//get

        }//Prod


        /// <summary>
        /// The highest schema version number defined in the updater
        /// </summary>

    }//CswScriptCollections

}//ChemSW.Nbt.Schema
