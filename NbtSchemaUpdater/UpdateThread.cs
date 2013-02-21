using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Forms;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.Schema
{
    class UpdateThread
    {

        #region Thread Interface

        public bool Cancel = false;

        public event FetchSchemataEventHandler OnFetchSchemata = null;
        public event GetSchemaInfoEventHandler OnGetSchemaInfo = null;
        public event StatusChangeEventHandler OnStatusChange = null;
        public event UpdateDoneEventHandler OnUpdateDone = null;
        public event UpdateFailedEventHandler OnUpdateFailed = null;

        public static string _ColName_AccessId = "AccessId";
        public static string _ColName_ServerType = "Server Type";
        public static string _ColName_ServerName = "ServerName";
        public static string _ColName_UserName = "UserName";
        public static string _ColName_UserCount = "UserCount";
        public static string _ColName_Deactivated = "Deactivated";
        public static string _ColName_Display = "Display";





        #endregion Thread Interface

        #region Session and Database

        private string _ConfigurationFilesFQPN { get { return ( Application.StartupPath + "\\..\\etc" ); } }
        private ICswLogger _CswLogger = null;
        private CswSchemaUpdater _CswSchemaUpdater;


        private CswSchemaScriptsProd _CswSchemaScriptsProd = null;

        private CswNbtResources _CswNbtResources = null;
        private CswNbtResources _InitSessionResources( string AccessId )
        {
            //CswNbtResources CswNbtResources = null;
            try
            {
                _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, false, false, null, ChemSW.RscAdo.PooledConnectionState.Closed );

                _CswNbtResources.AccessId = AccessId;
                _CswNbtResources.InitCurrentUser = InitUser;

                _CswSchemaScriptsProd = new CswSchemaScriptsProd();
                _CswLogger = _CswNbtResources.CswLogger;

            }
            catch( Exception ex )
            {
                SetStatus( "ERROR: " + ex.Message );
            }

            return _CswNbtResources;

        }//_InitSessionResources()

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswSystemUserNames.SysUsr_SchemaUpdt );
        }

        private void _CloseSessionResources( CswNbtResources CswNbtResources )
        {
            // BZ 8756 - close the transaction even if we have no updates
            CswNbtResources.finalize();
        }

        #endregion Session and Database

        #region Status Change
        public class StatusChangeEventArgs
        {
            public string Message;
            public StatusChangeEventArgs( string InMessage )
            {
                Message = InMessage;
            }
        }
        public delegate void StatusChangeEventHandler( StatusChangeEventArgs e );

        private void SetStatus( string NewStatus )
        {
            if( OnStatusChange != null )
                OnStatusChange( new StatusChangeEventArgs( NewStatus ) );
        }

        #endregion Status Change


        #region FetchSchemata

        public class FetchSchemataEventArgs
        {
            public bool Succeeded;
            public string Message;
            public CswDataTable DbInstances;
        }
        public delegate void FetchSchemataEventHandler( FetchSchemataEventArgs e );

        public delegate void FetchInvoker();

        public void FetchSchemata()
        {
            try
            {
                SetStatus( "Fetching Available Schemata" );

                FetchSchemataEventArgs e = new FetchSchemataEventArgs();

                CswNbtResources CswNbtResources = _InitSessionResources( string.Empty );

                if( CswNbtResources.CswDbCfgInfo.TotalDbInstances > 0 )
                {
                    CswDataTable DbInstances = new CswDataTable( "SchemaUpdaterForm1DataTable", "" );
                    DbInstances.Columns.Add( _ColName_AccessId, typeof( string ) );
                    DbInstances.Columns.Add( _ColName_ServerType, typeof( string ) );
                    DbInstances.Columns.Add( _ColName_ServerName, typeof( string ) );
                    DbInstances.Columns.Add( _ColName_UserName, typeof( string ) );
                    DbInstances.Columns.Add( _ColName_UserCount, typeof( string ) );
                    DbInstances.Columns.Add( _ColName_Deactivated, typeof( bool ) );
                    DbInstances.Columns.Add( _ColName_Display, typeof( string ) );
                    DbInstances.Rows.Clear();
                    foreach( string CurrentAccessId in CswNbtResources.CswDbCfgInfo.ActiveAccessIds )
                    {
                        CswNbtResources.CswDbCfgInfo.makeConfigurationCurrent( CurrentAccessId );
                        DataRow CurrentRow = DbInstances.NewRow();
                        CurrentRow[_ColName_AccessId] = CurrentAccessId.ToString();
                        CurrentRow[_ColName_ServerType] = CswNbtResources.CswDbCfgInfo.CurrentServerType;
                        CurrentRow[_ColName_ServerName] = CswNbtResources.CswDbCfgInfo.CurrentServerName;
                        CurrentRow[_ColName_UserName] = CswNbtResources.CswDbCfgInfo.CurrentUserName;
                        CurrentRow[_ColName_UserCount] = CswNbtResources.CswDbCfgInfo.CurrentUserCount;
                        CurrentRow[_ColName_Deactivated] = CswNbtResources.CswDbCfgInfo.CurrentDeactivated;
                        CurrentRow[_ColName_Display] = CurrentAccessId + " (" + CswNbtResources.CswDbCfgInfo.CurrentUserName + "@" + CswNbtResources.CswDbCfgInfo.CurrentServerName + ")";
                        DbInstances.Rows.Add( CurrentRow );
                    }

                    if( DbInstances.Rows.Count > 0 )
                    {
                        e.Succeeded = true;
                        e.DbInstances = DbInstances;
                    }
                    else
                    {
                        e.Succeeded = false;
                        e.Message = "Database configuration file does not contain any instances.";
                    }
                } // if( CswNbtResources.CswDbCfgInfo.TotalDbInstances > 0 )
                else
                {
                    e.Succeeded = false;
                    e.Message = "There is no database configuration data.";
                }

                if( OnFetchSchemata != null )
                    OnFetchSchemata( e );

                _CloseSessionResources( CswNbtResources );

                SetStatus( "Fetching Available Schemata: Done" );
            }
            catch( Exception ex )
            {
                SetStatus( "ERROR: " + ex.Message );
            }

        } // FetchSchemata

        #endregion FetchSchemata

        #region GetAccessIdInfo

        public class SchemaInfoEventArgs
        {
            public CswSchemaVersion MinimumSchemaVersion;
            public CswSchemaVersion CurrentSchemaVersion;
            public CswSchemaVersion LatestSchemaVersion;
            public DataTable UpdateHistoryTable;
        }
        public delegate void GetSchemaInfoEventHandler( SchemaInfoEventArgs e );
        public delegate void UpdateDoneEventHandler( SchemaInfoEventArgs e );
        public delegate void UpdateFailedEventHandler();
        public delegate void GetAccessIdInfoInvoker( string AccessId );

        public void GetAccessIdInfo( string AccessId )
        {
            try
            {
                SetStatus( "Initializing Selected Schema" );

                CswNbtResources CswNbtResources = _InitSessionResources( AccessId );

                SchemaInfoEventArgs e = new SchemaInfoEventArgs();

                _CswSchemaUpdater = new CswSchemaUpdater( AccessId, new CswSchemaUpdater.ResourcesInitHandler( _InitSessionResources ), _CswSchemaScriptsProd );
                e.MinimumSchemaVersion = _CswSchemaUpdater.MinimumVersion;
                e.LatestSchemaVersion = _CswSchemaUpdater.LatestVersion;

                CswSchemaVersion CurrentVersion = new CswSchemaVersion( CswNbtResources.ConfigVbls.getConfigVariableValue( "schemaversion" ).ToString() );
                e.CurrentSchemaVersion = CurrentVersion;

                CswTableSelect UpdateHistorySelect = CswNbtResources.makeCswTableSelect( "SchemaUpdater_updatehistory_select", "update_history" );
                DataTable UpdateHistoryTable = UpdateHistorySelect.getTable( string.Empty, new Collection<OrderByClause> { new OrderByClause( "updatehistoryid", OrderByType.Descending ) } );
                e.UpdateHistoryTable = UpdateHistoryTable;

                if( OnGetSchemaInfo != null )
                    OnGetSchemaInfo( e );

                _CloseSessionResources( CswNbtResources );

                SetStatus( "Initializing Selected Schema: Done" );
            }
            catch( Exception ex )
            {
                SetStatus( "ERROR: " + ex.Message );
            }
        }

        #endregion GetAccessIdInfo

        #region DoUpdate

        public delegate void DoUpdateInvoker( string AccessId );

        private bool _runNonVersionScripts( List<CswSchemaUpdateDriver> ScriptCollection, CswNbtResources CswNbtResources, SchemaInfoEventArgs SchemaInfoEventArgs )
        {
            bool ReturnVal = true;

            for( int idx = 0; ReturnVal && ( idx < ScriptCollection.Count ); idx++ )
            {
                CswSchemaUpdateDriver CurrentUpdateDriver = ScriptCollection[idx];

                string ScriptDescription = CurrentUpdateDriver.SchemaVersion.ToString() + ": " + CurrentUpdateDriver.Description;
                ReturnVal = _CswSchemaUpdater.runArbitraryScript( CurrentUpdateDriver );
                if( ReturnVal )
                {
                    SetStatus( "Update successful: " + ScriptDescription );
                }
                else
                {
                    SetStatus( "Update failed: " + ScriptDescription + ": " + CurrentUpdateDriver.Message );
                }

                _updateHistoryTable( CswNbtResources, SchemaInfoEventArgs );
            }

            return ( ReturnVal );

        }//_runNonVersionScripts

        private void _updateHistoryTable( CswNbtResources CswNbtResources, SchemaInfoEventArgs SchemaInfoEventArgs )
        {
            CswTableSelect UpdateHistorySelect = CswNbtResources.makeCswTableSelect( "SchemaUpdater_updatehistory_select", "update_history" );
            DataTable UpdateHistoryTable = UpdateHistorySelect.getTable( string.Empty, new Collection<OrderByClause> { new OrderByClause( "updatehistoryid", OrderByType.Descending ) } );
            SchemaInfoEventArgs.UpdateHistoryTable = UpdateHistoryTable;


            if( OnGetSchemaInfo != null )
                OnGetSchemaInfo( SchemaInfoEventArgs );

            if( OnUpdateDone != null )
                OnUpdateDone( SchemaInfoEventArgs );


        }//_updateHistoryTable() 


        public void DoUpdate( string AccessId )
        {
            try
            {
                SetStatus( "Updating Selected Schema" );

                CswNbtResources CswNbtResources = _InitSessionResources( AccessId );
                SchemaInfoEventArgs SchemaInfoEventArgs = new SchemaInfoEventArgs();


                _CswSchemaUpdater = new CswSchemaUpdater( AccessId, new CswSchemaUpdater.ResourcesInitHandler( _InitSessionResources ), _CswSchemaScriptsProd ); //wait to create updater until resource initiation is thoroughly done

                bool UpdateSucceeded = _runNonVersionScripts( _CswSchemaScriptsProd.RunBeforeScripts, CswNbtResources, SchemaInfoEventArgs );


                if( UpdateSucceeded )
                {
                    CswSchemaVersion CurrentVersion = _CswSchemaUpdater.CurrentVersion( CswNbtResources );
                    while( UpdateSucceeded && !Cancel && CurrentVersion != _CswSchemaUpdater.LatestVersion )
                    {
                        SetStatus( "Updating to " + _CswSchemaUpdater.TargetVersion( CswNbtResources ).ToString() );

                        UpdateSucceeded = _CswSchemaUpdater.runNextVersionedScript();

                        CswNbtResources.AccessId = AccessId; //cases 23787,9751: you have to re-init after the release() that is done in the Updater
                        CswNbtResources.ClearCache();

                        SchemaInfoEventArgs.MinimumSchemaVersion = _CswSchemaUpdater.MinimumVersion;
                        SchemaInfoEventArgs.LatestSchemaVersion = _CswSchemaUpdater.LatestVersion;

                        CurrentVersion = _CswSchemaUpdater.CurrentVersion( CswNbtResources );
                        SchemaInfoEventArgs.CurrentSchemaVersion = CurrentVersion;

                        _updateHistoryTable( CswNbtResources, SchemaInfoEventArgs );

                        if( UpdateSucceeded )
                            SetStatus( "Update successful" );



                    }//iterate veresions

                }//if pre-process scripts succeded

                if( UpdateSucceeded )
                {
                    UpdateSucceeded = _runNonVersionScripts( _CswSchemaScriptsProd.RunAfterScripts, CswNbtResources, SchemaInfoEventArgs );
                }

                if( Cancel )
                {
                    SetStatus( "Update process stopped" );
                }
                else if( !UpdateSucceeded )
                {
                    if( OnUpdateFailed != null )
                        OnUpdateFailed();
                    SetStatus( "Update failed" );
                }
                else
                {
                    SetStatus( "Update process completed" );
                }

                _CloseSessionResources( CswNbtResources );
            }
            catch( Exception ex )
            {
                SetStatus( "ERROR: " + ex.Message );
            }
        }//iterate versions





        #endregion DoUpdate

    } // class UpdateThread

} // namespace ChemSW.Nbt.Schema

