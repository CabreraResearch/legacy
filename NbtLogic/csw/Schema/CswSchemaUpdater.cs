using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.DB;

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
        private Hashtable _UpdateDrivers = new Hashtable();

        /// <summary>
        /// Constructor
        /// </summary>
        public CswSchemaUpdater( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            // This is where you manually set to the last version of the previous release
            MinimumVersion = new CswSchemaVersion( 1, 'G', 32 ); 

            // This is where you add new versions.
            CswSchemaUpdateDriver Schema01H01Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H01( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H01Driver.SchemaVersion, Schema01H01Driver );
            CswSchemaUpdateDriver Schema01H02Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H02( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H02Driver.SchemaVersion, Schema01H02Driver );
            CswSchemaUpdateDriver Schema01H03Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H03( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H03Driver.SchemaVersion, Schema01H03Driver );
            CswSchemaUpdateDriver Schema01H04Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H04( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H04Driver.SchemaVersion, Schema01H04Driver );
            CswSchemaUpdateDriver Schema01H05Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H05( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H05Driver.SchemaVersion, Schema01H05Driver );
            CswSchemaUpdateDriver Schema01H06Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H06( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H06Driver.SchemaVersion, Schema01H06Driver );
            CswSchemaUpdateDriver Schema01H07Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H07( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H07Driver.SchemaVersion, Schema01H07Driver );
            CswSchemaUpdateDriver Schema01H08Driver = new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo01H08( _CswNbtSchemaModTrnsctn ) );
            _UpdateDrivers.Add( Schema01H08Driver.SchemaVersion, Schema01H08Driver );

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
        /// The latest schema version
        /// </summary>
        public CswSchemaVersion LatestVersion = null;
        /// <summary>
        /// The minimum version required to use this updater
        /// </summary>
        public CswSchemaVersion MinimumVersion = null;

        private CswSchemaVersion CurrentVersion
        {
            get { return new CswSchemaVersion( _CswNbtResources.getConfigVariableValue( "schemaversion" ) ); }
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
                //CswSchemaVersion TargetVersion = null;
                //if( CurrentVersion == MinimumVersion )
                //    TargetVersion = new CswSchemaVersion( LatestVersion.CycleIteration, LatestVersion.ReleaseIdentifier, 1 );
                //else
                //    TargetVersion = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration + 1 );

                CswSchemaUpdateDriver CurrentUpdateDriver = _UpdateDrivers[TargetVersion] as CswSchemaUpdateDriver;
                CurrentUpdateDriver.update();
                UpdateSuccessful = CurrentUpdateDriver.UpdateSucceeded;

                if( !UpdateSuccessful )
                {
                    // Belt and suspenders.
                    _CswNbtResources.logError( new CswDniException( "Schema Updater encountered a problem: " + CurrentUpdateDriver.Message ) );
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
