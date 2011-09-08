using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.Config;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.Config;
using ChemSW.Security;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.Security;

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

        private CswDbCfgInfoNbt _CswDbCfgInfoNbt = null;
        private CswSetupVblsNbt _CswSetupVblsNbt = null;

        private string _ConfigurationFilesFQPN { get { return ( Application.StartupPath + "\\..\\etc" ); } }
        private ICswLogger _CswLogger = null;
        //private CswNbtResources _CswNbtResources = null;
        private CswSchemaUpdater _CswSchemaUpdater;
        //private CswNbtObjClassFactory _CswNbtObjClassFactory;
        //private CswNbtMetaDataEvents _CswNbtMetaDataEvents = null;


        private CswSchemaScriptsProd _CswSchemaScriptsProd = null;
        private CswNbtResources _InitSessionResources(string AccessId)
        {
			CswNbtResources CswNbtResources = null;
            try
            {
                _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe );
                _CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode.NbtExe );

				CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, _CswSetupVblsNbt, _CswDbCfgInfoNbt, CswTools.getConfigurationFilePath( SetupMode.NbtExe ), false, false );
				CswNbtResources.AccessId = AccessId;
				CswNbtResources.InitCurrentUser = InitUser;

				_CswSchemaScriptsProd = new CswSchemaScriptsProd();
				_CswLogger = CswNbtResources.CswLogger;
            }
            catch( Exception ex )
            {
                SetStatus( "ERROR: " + ex.Message );
            }
			return CswNbtResources;
        }//_InitSessionResources()

		public ICswUser InitUser(ICswResources Resources)
		{
			return new CswNbtSystemUser( Resources, "_SchemaUpdaterUser" );
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

                if( _CswDbCfgInfoNbt.TotalDbInstances > 0 )
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
                    foreach( string CurrentAccessId in _CswDbCfgInfoNbt.AccessIds )
                    {
                        _CswDbCfgInfoNbt.makeConfigurationCurrent( CurrentAccessId );
                        DataRow CurrentRow = DbInstances.NewRow();
                        CurrentRow[_ColName_AccessId] = CurrentAccessId.ToString();
                        CurrentRow[_ColName_ServerType] = _CswDbCfgInfoNbt.CurrentServerType;
                        CurrentRow[_ColName_ServerName] = _CswDbCfgInfoNbt.CurrentServerName;
                        CurrentRow[_ColName_UserName] = _CswDbCfgInfoNbt.CurrentUserName;
                        CurrentRow[_ColName_UserCount] = _CswDbCfgInfoNbt.CurrentUserCount;
                        CurrentRow[_ColName_Deactivated] = _CswDbCfgInfoNbt.CurrentDeactivated;
                        CurrentRow[_ColName_Display] = CurrentAccessId + " (" + _CswDbCfgInfoNbt.CurrentUserName + "@" + _CswDbCfgInfoNbt.CurrentServerName + ")";
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
                } // if( _CswDbCfgInfoNbt.TotalDbInstances > 0 )
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

				CswSchemaVersion CurrentVersion = new CswSchemaVersion( CswNbtResources.getConfigVariableValue( "schemaversion" ).ToString() );
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

        public void DoUpdate( string AccessId )
        {
            try
            {
                SetStatus( "Updating Selected Schema" );

				CswNbtResources CswNbtResources = _InitSessionResources( AccessId );

                _CswSchemaUpdater = new CswSchemaUpdater( AccessId, new CswSchemaUpdater.ResourcesInitHandler(_InitSessionResources), _CswSchemaScriptsProd ); //wait to create updater until resource initiation is thoroughly done

                bool UpdateSucceeded = true;
                SchemaInfoEventArgs e = new SchemaInfoEventArgs();
				CswSchemaVersion CurrentVersion = _CswSchemaUpdater.CurrentVersion( CswNbtResources );
                while( UpdateSucceeded && !Cancel && CurrentVersion != _CswSchemaUpdater.LatestVersion )
                {
					SetStatus( "Updating to " + _CswSchemaUpdater.TargetVersion( CswNbtResources ).ToString() );

                    UpdateSucceeded = _CswSchemaUpdater.Update();

                    e.MinimumSchemaVersion = _CswSchemaUpdater.MinimumVersion;
                    e.LatestSchemaVersion = _CswSchemaUpdater.LatestVersion;

					CurrentVersion = _CswSchemaUpdater.CurrentVersion( CswNbtResources );
                    e.CurrentSchemaVersion = CurrentVersion;

					CswTableSelect UpdateHistorySelect = CswNbtResources.makeCswTableSelect( "SchemaUpdater_updatehistory_select", "update_history" );
                    DataTable UpdateHistoryTable = UpdateHistorySelect.getTable( string.Empty, new Collection<OrderByClause> { new OrderByClause( "updatehistoryid", OrderByType.Descending ) } );
                    e.UpdateHistoryTable = UpdateHistoryTable;

                    if( UpdateSucceeded )
                        SetStatus( "Update successful" );

                    if( OnGetSchemaInfo != null )
                        OnGetSchemaInfo( e );
                }

                if( OnUpdateDone != null )
                    OnUpdateDone( e );

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
        }

        #endregion DoUpdate

    } // class UpdateThread

} // namespace ChemSW.Nbt.Schema

