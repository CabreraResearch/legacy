using System.Collections.Generic;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public interface ICswSchemaScripts
    {

        CswSchemaVersion LatestVersion { get; }
        CswSchemaVersion MinimumVersion { get; }
        CswSchemaVersion CurrentVersion( CswNbtResources CswNbtResources );
        CswSchemaVersion TargetVersion( CswNbtResources CswNbtResources );
        CswSchemaUpdateDriver Next( CswNbtResources CswNbtResources );
        CswSchemaUpdateDriver this[CswSchemaVersion CswSchemaVersion] { get; }
        void stampSchemaVersion( CswNbtResources CswNbtResources, CswSchemaUpdateDriver CswSchemaUpdateDriver );
        Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get; }

        void addUniversalPreProcessDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver );
        void addReleaseDmlDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver );
        void addReleaseDdlDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver );
        void addUniversalPostProcessDriver( CswSchemaUpdateDriver CswSchemaUpdateDriver );


    }//CswScriptCollections

}//ChemSW.Nbt.Schema
