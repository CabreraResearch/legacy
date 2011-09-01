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
        private ICswSchemaScripts _CswSchemaScripts = null;
		private string _AccessId = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
		public CswSchemaUpdater( string AccessId, ResourcesInitHandler ResourcesInitHandler, ICswSchemaScripts CswSchemaScripts )
        {
            _CswSchemaScripts = CswSchemaScripts;
			_ResourcesInitHandler = ResourcesInitHandler;
			_AccessId = AccessId;
			//_ReinitCswNbtResources( AccessId );
        }

		#region Resources Handling

		// This allows us to use a new Resources per update script
		public delegate CswNbtResources ResourcesInitHandler( string AccessId );
		private ResourcesInitHandler _ResourcesInitHandler = null;

		//private CswNbtSchemaModTrnsctn _ReinitCswNbtResources( string AccessId )
		//{
		//}

		//private CswNbtSchemaModTrnsctn __CswNbtSchemaModTrnsctn = null;
		//public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
		//{
		//    get { return __CswNbtSchemaModTrnsctn; }
		//}

		//private CswNbtResources __CswNbtResources;
		//public CswNbtResources CswNbtResources
		//{
		//    get { return __CswNbtResources; }
		//}

		#endregion Resources Handling

		/// <summary>
        /// The highest schema version number defined in the updater
        /// </summary>
        public CswSchemaVersion LatestVersion { get { return ( _CswSchemaScripts.LatestVersion ); } }
        /// <summary>
        /// The minimum version required to use this updater
        /// </summary>
        public CswSchemaVersion MinimumVersion { get { return ( _CswSchemaScripts.MinimumVersion ); } }


        public CswSchemaVersion CurrentVersion { get { return ( _CswSchemaScripts.CurrentVersion ); } }

        /// <summary>
        /// Schema version of the currently targeted schema
        /// </summary>
        public CswSchemaVersion TargetVersion { get { return ( _CswSchemaScripts.TargetVersion ); } }

        private string _ErrorMessage = string.Empty;
        public string ErrorMessage { get { return ( _ErrorMessage ); } }

        public CswSchemaUpdateDriver getDriver( CswSchemaVersion CswSchemaVersion )
        {
            return ( _CswSchemaScripts[CswSchemaVersion] );
        }//getDriver()

        /// <summary>
        /// Update the schema to the next version
        /// </summary>
        public bool Update()
        {
			CswNbtResources CswNbtResources = _ResourcesInitHandler( _AccessId );
			CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( CswNbtResources );
			
			CswTableUpdate _UpdateHistoryTableUpdate = CswNbtResources.makeCswTableUpdate( "schemaupdater_updatehistory_update", "update_history" );
            DataTable _UpdateHistoryTable = _UpdateHistoryTableUpdate.getTable();

            CswSchemaUpdateDriver CurrentUpdateDriver = null;
            bool UpdateSuccessful = true;
            if( null != ( CurrentUpdateDriver = _CswSchemaScripts.Next ) )
            {
				CurrentUpdateDriver.CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
                CurrentUpdateDriver.update();
                UpdateSuccessful = CurrentUpdateDriver.UpdateSucceeded;

                if( !UpdateSuccessful )
                {
                    // Belt and suspenders.
                    CswNbtResources.logError( new CswDniException( "Schema Updater encountered a problem: " + CurrentUpdateDriver.Message ) );
                    _ErrorMessage = "Error updating to schema version " + CurrentUpdateDriver.SchemaVersion.ToString() + ": " + CurrentUpdateDriver.Message;
                }
                else
                {
                    _CswSchemaScripts.stampSchemaVersion( CurrentUpdateDriver );

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

                CswNbtResources.finalize();

            } // if update is valid

            return UpdateSuccessful;

        }//Update()

        public bool Next() { return ( null != _CswSchemaScripts.Next ); }

    }//CswSchemaUpdater

}//ChemSW.Nbt.Schema
