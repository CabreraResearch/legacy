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
    public class CswSchemaUpdater
    {
        private CswNbtResources _CswNbtResources;
        private CswTableUpdate _UpdateHistoryTableUpdate;
        private DataTable _UpdateHistoryTable;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = null;
        private CswSchemaScriptsProd _CswScriptCollections = null;


        public enum HamletNodeTypes
        {
            Fire_Extinguisher,
            Mount_Point,
            Mount_Point_Group,
            Physical_Inspection,
            Physical_Inspection_Schedule,
            Physical_Inspection_Route,
            Notification,
            Floor,
            FE_Inspection_Point,
            Inspection_Group
        }
        public static string HamletNodeTypesAsString( HamletNodeTypes NodeType )
        {
            return ( NodeType.ToString().Replace( '_', ' ' ) );
        }

        ICswSchemaScripts _CswSchemaScripts = null; 
        /// <summary>
        /// Constructor
        /// </summary>
        public CswSchemaUpdater( CswNbtResources CswNbtResources, ICswSchemaScripts CswSchemaScripts )
        {
            _CswSchemaScripts = CswSchemaScripts; 
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            // This is where you manually set to the last version of the previous release
            MinimumVersion = new CswSchemaVersion( 1, 'G', 32 );

            _UpdateDrivers = CswSchemaScripts.Scripts;

            // This automatically detects the latest version
            foreach( CswSchemaVersion Version in _UpdateDrivers.Keys )
            {
                if( LatestVersion == null ||
                    ( LatestVersion.CycleIteration == Version.CycleIteration &&
                      LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier &&
                      LatestVersion.ReleaseIteration < Version.ReleaseIteration ) )
                {
                    LatestVersion = Version;
                }
            }
        }

        /// <summary>
        /// The highest schema version number defined in the updater
        /// </summary>
        public CswSchemaVersion LatestVersion = null;
        /// <summary>
        /// The minimum version required to use this updater
        /// </summary>
        public CswSchemaVersion MinimumVersion = null;

        public CswSchemaVersion CurrentVersion
        {
            get { return ( _CswSchemaScripts.CurrentVersion ); }
        }

        /// <summary>
        /// Schema version of the currently targeted schema
        /// </summary>
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

        private string _ErrorMessage = string.Empty;
        public string ErrorMessage { get { return ( _ErrorMessage ); } }



        /// <summary>
        /// Update the schema to the next version
        /// </summary>
        public bool Update()
        {
            _UpdateHistoryTableUpdate = _CswNbtResources.makeCswTableUpdate( "schemaupdater_updatehistory_update", "update_history" );
            _UpdateHistoryTable = _UpdateHistoryTableUpdate.getTable();

            bool UpdateSuccessful = true;
            if( CurrentVersion == MinimumVersion ||
                ( LatestVersion.CycleIteration == CurrentVersion.CycleIteration &&
                  LatestVersion.ReleaseIdentifier == CurrentVersion.ReleaseIdentifier &&
                  LatestVersion.ReleaseIteration > CurrentVersion.ReleaseIteration ) )
            {

                CswSchemaUpdateDriver CurrentUpdateDriver = _UpdateDrivers[TargetVersion] as CswSchemaUpdateDriver;
                CurrentUpdateDriver.update();
                UpdateSuccessful = CurrentUpdateDriver.UpdateSucceeded;

                if( !UpdateSuccessful )
                {
                    // Belt and suspenders.
                    _CswNbtResources.logError( new CswDniException( "Schema Updater encountered a problem: " + CurrentUpdateDriver.Message ) );
                    _ErrorMessage = "Error updating to schema version " + CurrentUpdateDriver.SchemaVersion.ToString() + ": " + CurrentUpdateDriver.Message;
                }
                else
                {
                    _CswNbtResources.setConfigVariableValue( "schemaversion", CurrentUpdateDriver.SchemaVersion.ToString() );
                }

                DataRow NewUpdateHistoryRow = _UpdateHistoryTable.NewRow();
                NewUpdateHistoryRow["updatedate"] = DateTime.Now.ToString();
                NewUpdateHistoryRow["version"] = CurrentUpdateDriver.SchemaVersion.ToString();
                if( UpdateSuccessful )
                {
                    NewUpdateHistoryRow["log"] = CurrentUpdateDriver.Message;

                }
                else if( CurrentUpdateDriver.RollbackSucceeded )
                {
                    NewUpdateHistoryRow["log"] = "Schema rolled back to previous version due to failure: " + CurrentUpdateDriver.Message;
                }
                else
                {
                    NewUpdateHistoryRow["log"] = "Schema rollback failed; current schema state undefined: " + CurrentUpdateDriver.Message;
                }

                _UpdateHistoryTable.Rows.Add( NewUpdateHistoryRow );
                _UpdateHistoryTableUpdate.update( _UpdateHistoryTable );

                _CswNbtResources.finalize();

            } // if update is valid

            return UpdateSuccessful;

        }//Update()

    }//CswSchemaUpdater

}//ChemSW.Nbt.Schema
