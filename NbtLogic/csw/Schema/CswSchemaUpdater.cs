using System;
using System.Collections.Generic;
using System.Data;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Core;
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
        public delegate void ResourcesInitHandler( string AccessId, ref CswNbtResources CswNbtResourcesOut );
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


        public CswSchemaVersion CurrentVersion( CswNbtResources CswNbtResources )
        {
            return ( _CswSchemaScripts.CurrentVersion( CswNbtResources ) );
        }

        /// <summary>
        /// Schema version of the currently targeted schema
        /// </summary>
        public CswSchemaVersion TargetVersion( CswNbtResources CswNbtResources )
        {
            return ( _CswSchemaScripts.TargetVersion( CswNbtResources ) );
        }

        private string _ErrorMessage = string.Empty;
        public string ErrorMessage { get { return ( _ErrorMessage ); } }

        public CswSchemaUpdateDriver getDriver( CswSchemaVersion CswSchemaVersion )
        {
            return ( _CswSchemaScripts[CswSchemaVersion] );
        }//getDriver()

        private CswSchemaUpdateDriver _CurrentUpdateDriver;
        public CswSchemaUpdateDriver getDriver()
        {
            return ( _CurrentUpdateDriver );
        }


        //case 26617: removing otiose instance of CswNbtResources resolved catastrophic memory leak
        private bool _runScript( CswNbtResources CswNbtResources, CswSchemaUpdateDriver CswSchemaUpdateDriver, bool StampVersion )
        {

            bool ReturnVal = true;

            // CswNbtResources CswNbtResources = _ResourcesInitHandler( _AccessId );
            CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( CswNbtResources );

            CswTableUpdate _UpdateHistoryTableUpdate = CswNbtResources.makeCswTableUpdate( "schemaupdater_updatehistory_update", "update_history" );
            DataTable _UpdateHistoryTable = _UpdateHistoryTableUpdate.getTable();

            CswSchemaUpdateDriver.CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            CswSchemaUpdateDriver.update();
            ReturnVal = CswSchemaUpdateDriver.UpdateSucceeded;

            if( false == ReturnVal )
            {
                // Belt and suspenders.
                CswNbtResources.logError( new CswDniException( "Schema Updater encountered a problem: " + CswSchemaUpdateDriver.Message ) );
                _ErrorMessage = "Error updating to schema version " + CswSchemaUpdateDriver.SchemaVersion.ToString() + ": " + CswSchemaUpdateDriver.Message;
            }
            else if( StampVersion )
            {
                _CswSchemaScripts.stampSchemaVersion( CswNbtResources, CswSchemaUpdateDriver );
            }

            DataRow NewUpdateHistoryRow = _UpdateHistoryTable.NewRow();
            NewUpdateHistoryRow["updatedate"] = DateTime.Now.ToString();
            NewUpdateHistoryRow["version"] = CswSchemaUpdateDriver.SchemaVersion.ToString();

            // TODO: Remove this conditional if after FOXGLOVE is in production
            if( _UpdateHistoryTable.Columns.Contains( "scriptname" ) )
            {
                NewUpdateHistoryRow["scriptname"] = CswSchemaUpdateDriver.ScriptName;
            }
            // TODO: Remove this conditional if after FOXGLOVE is in production
            if( _UpdateHistoryTable.Columns.Contains( "succeeded" ) )
            {
                NewUpdateHistoryRow["succeeded"] = CswConvert.ToDbVal( CswSchemaUpdateDriver.UpdateSucceeded );
            }

            if( ReturnVal )
            {
                NewUpdateHistoryRow["log"] = CswSchemaUpdateDriver.Message;

            }
            else
            {
                NewUpdateHistoryRow["log"] = "Failed update: " + CswSchemaUpdateDriver.Message;
            }
            _UpdateHistoryTable.Rows.Add( NewUpdateHistoryRow );

            _UpdateHistoryTableUpdate.update( _UpdateHistoryTable );

            CswNbtResources.finalize();
            CswNbtResources.release();


            CswSchemaUpdateDriver.CswNbtSchemaModTrnsctn = null;

            GC.Collect();

            return ( ReturnVal );

        }//_runScript()



        public bool runArbitraryScript( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {

            //_ResourcesInitHandler( _AccessId )
            CswNbtResources CswNbtResources = null;
            _ResourcesInitHandler( _AccessId, ref CswNbtResources );
            return ( _runScript( CswNbtResources, CswSchemaUpdateDriver, false ) );
        }//UpdateArbitraryScript

        /// <summary>
        /// Update the schema to the next version
        /// </summary>
        public bool runNextVersionedScript()
        {
            CswNbtResources CswNbtResources = null;
            _ResourcesInitHandler( _AccessId, ref CswNbtResources );

            CswSchemaUpdateDriver CurrentUpdateDriver = null;
            bool UpdateSuccessful = true;
            bool StampVersion = true;
            if( null != ( CurrentUpdateDriver = _CswSchemaScripts.Next( CswNbtResources ) ) )
            {
                _CurrentUpdateDriver = CurrentUpdateDriver;
                if( CurrentUpdateDriver.AlwaysRun ) { StampVersion = false; }
                UpdateSuccessful = _runScript( CswNbtResources, CurrentUpdateDriver, StampVersion );

            } // if update is valid

            return UpdateSuccessful;

        }//runNextVersionedScript()

        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers
        {
            get
            {
                return _CswSchemaScripts.UpdateDrivers;
            }
        }

        public void addVersionedScriptsToRun()
        {
            CswNbtResources CswNbtResources = null;
            _ResourcesInitHandler( _AccessId, ref CswNbtResources );

            _CswSchemaScripts.addVersionedScriptsToRun( CswNbtResources );
        }



    }//CswSchemaUpdater

}//ChemSW.Nbt.Schema
