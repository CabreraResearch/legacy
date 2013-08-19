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
        Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDriversToRun { get; }
        void addVersionedScriptsToRun( CswNbtResources CswNbtResources );

    }//CswScriptCollections

}//ChemSW.Nbt.Schema
