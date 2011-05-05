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
    public interface ICswSchemaScripts
    {

        CswSchemaVersion LatestVersion { get; }
        CswSchemaVersion MinimumVersion { get; }
        CswSchemaVersion CurrentVersion { get; }
        CswSchemaVersion TargetVersion { get; }
        CswSchemaUpdateDriver Next { get; }
        CswSchemaUpdateDriver this[CswSchemaVersion CswSchemaVersion] { get; }
        void stampSchemaVersion( CswSchemaUpdateDriver CswSchemaUpdateDriver );


    }//CswScriptCollections

}//ChemSW.Nbt.Schema
