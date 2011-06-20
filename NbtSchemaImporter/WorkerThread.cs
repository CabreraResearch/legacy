using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using ChemSW.Core;
using ChemSW.Nbt.Config;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.Config;
using ChemSW.Security;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    class WorkerThread
    {
        private CswDbCfgInfoNbt _CswDbCfgInfoNbt = null;
        private CswSetupVblsNbt _CswSetupVblsNbt = null;
        private ICswLogger _CswLogger = null;
        private CswNbtResources _CswNbtResources = null;
        private CswNbtObjClassFactory _CswNbtObjClassFactory;
        private CswNbtMetaDataEvents _CswNbtMetaDataEvents = null;

        public static string ColName_AccessId = "AccessId";
        public static string ColName_ServerType = "Server Type";
        public static string ColName_ServerName = "ServerName";
        public static string ColName_UserName = "UserName";
        public static string ColName_UserCount = "UserCount";
        public static string ColName_Deactivated = "Deactivated";
        public static string ColName_Display = "Display";
        public static string ColName_DataFileName = "FileName";
        public static string ColName_DataFileFullName = "Path";

        private string _ConfigurationPath;
        private string _AccessId;
        public string AccessId
        {
            get { return _AccessId; }
            set
            {
                _AccessId = value;
                _InitSessionResources();
                _CswNbtResources.AccessId = value;
                _CswNbtResources.refreshDataDictionary();
            }
        }

        public WorkerThread( string ConfigurationPath )
        {
            _ConfigurationPath = ConfigurationPath;

            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.Executable );
            _CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode.Executable );

            _InitSessionResources();
        }

        private void _InitSessionResources()
        {
            //_CswNbtObjClassFactory = new CswNbtObjClassFactory();

            //_CswNbtResources = new CswNbtResources( AppType.SchemInit, _CswSetupVblsNbt, _CswDbCfgInfoNbt, false, false );
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.SchemInit, _CswSetupVblsNbt, _CswDbCfgInfoNbt, CswTools.getConfigurationFilePath( SetupMode.Executable ), false, false  );
            //_CswNbtResources.SetDbResources( new CswNbtTreeFactory( _ConfigurationPath ) );

            //_CswNbtMetaDataEvents = new CswNbtMetaDataEvents( _CswNbtResources );
            //_CswNbtResources.OnMakeNewNodeType += new CswNbtResources.NewNodeTypeEventHandler( _CswNbtMetaDataEvents.OnMakeNewNodeType );
            //_CswNbtResources.OnCopyNodeType += new CswNbtResources.CopyNodeTypeEventHandler( _CswNbtMetaDataEvents.OnCopyNodeType );
            //_CswNbtResources.OnMakeNewNodeTypeProp += new CswNbtResources.NewNodeTypePropEventHandler( _CswNbtMetaDataEvents.OnMakeNewNodeTypeProp );
            //_CswNbtResources.OnEditNodeTypePropName += new CswNbtResources.EditPropNameEventHandler( _CswNbtMetaDataEvents.OnEditNodeTypePropName );
            //_CswNbtResources.OnDeleteNodeTypeProp += new CswNbtResources.DeletePropEventHandler( _CswNbtMetaDataEvents.OnDeleteNodeTypeProp );
            //_CswNbtResources.OnEditNodeTypeName += new CswNbtResources.EditNodeTypeNameEventHandler( _CswNbtMetaDataEvents.OnEditNodeTypeName );

            _CswLogger = _CswNbtResources.CswLogger;

            //_CswNbtResources.CurrentUser = new CswNbtSystemUser( _CswNbtResources, "_SchemaImporterUser" );
			_CswNbtResources.InitCurrentUser = InitUser;
        }//constructor

		public ICswUser InitUser( ICswResources Resources )
		{
			return new CswNbtSystemUser( Resources, "_SchemaImporterUser" );
		}

        public CswDataTable getDbInstances()
        {
            CswDataTable DbInstances = new CswDataTable( "SchemaImporterSchemataDataTable", "" );
            DbInstances.Columns.Add( ColName_AccessId, typeof( string ) );
            DbInstances.Columns.Add( ColName_ServerType, typeof( string ) );
            DbInstances.Columns.Add( ColName_ServerName, typeof( string ) );
            DbInstances.Columns.Add( ColName_UserName, typeof( string ) );
            DbInstances.Columns.Add( ColName_UserCount, typeof( string ) );
            DbInstances.Columns.Add( ColName_Deactivated, typeof( bool ) );
            DbInstances.Columns.Add( ColName_Display, typeof( string ) );
            DbInstances.Rows.Clear();
            foreach( string CurrentAccessId in _CswDbCfgInfoNbt.AccessIds )
            {
                _CswDbCfgInfoNbt.makeConfigurationCurrent( CurrentAccessId );
                DataRow CurrentRow = DbInstances.NewRow();
                CurrentRow[ColName_AccessId] = CurrentAccessId.ToString();
                CurrentRow[ColName_ServerType] = _CswDbCfgInfoNbt.CurrentServerType;
                CurrentRow[ColName_ServerName] = _CswDbCfgInfoNbt.CurrentServerName;
                CurrentRow[ColName_UserName] = _CswDbCfgInfoNbt.CurrentUserName;
                CurrentRow[ColName_UserCount] = _CswDbCfgInfoNbt.CurrentUserCount;
                CurrentRow[ColName_Deactivated] = _CswDbCfgInfoNbt.CurrentDeactivated;
                CurrentRow[ColName_Display] = CurrentAccessId + " (" + _CswDbCfgInfoNbt.CurrentUserName + "@" + _CswDbCfgInfoNbt.CurrentServerName + ")";
                DbInstances.Rows.Add( CurrentRow );
            }
            return DbInstances;
        }

        //public CswDataTable getDataFilesTable()
        //{
        //    CswDataTable DataFilesTable = new CswDataTable( "SchemaInitializerDataFileTable", "" );
        //    DataFilesTable.Columns.Add( WorkerThread.ColName_DataFileName, typeof( string ) );
        //    DataFilesTable.Columns.Add( WorkerThread.ColName_DataFileFullName, typeof( string ) );

        //    string DataFileDirectory = Application.StartupPath + "\\..\\etc\\datafiles";
        //    if( !Directory.Exists( DataFileDirectory ) )
        //        Directory.CreateDirectory( DataFileDirectory );
        //    foreach( string FullFileName in FileSystem.GetFiles( DataFileDirectory ) )
        //    {
        //        string FileName = FullFileName.Substring( FullFileName.LastIndexOf( '\\' ) + 1 );
        //        DataRow CurrentRow = DataFilesTable.NewRow();
        //        CurrentRow[WorkerThread.ColName_DataFileName] = FileName;
        //        CurrentRow[WorkerThread.ColName_DataFileFullName] = FullFileName;
        //        DataFilesTable.Rows.Add( CurrentRow );
        //    }
        //    return DataFilesTable;
        //}


        public ICollection getNodeTypes()
        {
            return _CswNbtResources.MetaData.NodeTypes;
        }

        public delegate void StatusHandler( string Msg );
        public event StatusHandler OnStatusChange;
        public void SetStatus( string Msg )
        {
            if( OnStatusChange != null )
                OnStatusChange( Msg );
        }


        public delegate void ImportHandler( string FileName, CswNbtImportExport.ImportMode Mode ); //, bool ClearExisting );
        public void DoImport( string FilePath, CswNbtImportExport.ImportMode Mode ) //, bool ClearExisting )
        {
            try
            {
                if( FilePath != string.Empty )
                {
                    //string FilePath = Application.StartupPath + "\\..\\etc\\datafiles\\" + FileName;
                    // verify the file exists
                    if( File.Exists( FilePath ) )
                    {
                        Stream FileStream = File.OpenRead( FilePath );
                        StreamReader FileSR = new StreamReader( FileStream );
                        string FileContents = FileSR.ReadToEnd();
                        FileStream.Close();

                        //// Clear data?
                        //if( ClearExisting )
                        //    ClearSchema();

                        // Restore selected data
                        CswNbtImportExport Importer = new CswNbtImportExport( _CswNbtResources );
                        Importer.OnStatusUpdate += new CswNbtImportExport.StatusUpdateHandler( SetStatus );

                        string ViewXml = string.Empty;
                        string ResultXml = string.Empty;
                        string ErrorLog = string.Empty;

                        Importer.ImportXml( Mode, FileContents, ref ViewXml, ref ResultXml, ref ErrorLog );

                        if( ErrorLog != string.Empty )
                        {
                            _CswNbtResources.logMessage( ErrorLog );
                            SetStatus( "Errors Occurred.  Check Log." );
                        }
                    }
                    else
                    {
                        //ErrorLabel.Text = "File does not exist: " + DataFileName; // openFileDialog1.FileName;
                        SetStatus( "File does not exist: " + FilePath );
                    }

                    _CswNbtResources.finalize();

                } // if(FileName != string.Empty)
            }
            catch( Exception ex )
            {
                SetStatus( "Error: " + ex.Message + "\r\n" + ex.StackTrace );
            }
        } // DoImport()


        public delegate void ExportHandler( string FilePath, ICollection SelectedNodeTypes, bool ExportViews, bool ExportNodes );
        public void DoExport( string FilePath, ICollection SelectedNodeTypes, bool ExportViews, bool ExportNodes )
        {
            try
            {
                CswNbtImportExport Exporter = new CswNbtImportExport( _CswNbtResources );
                Exporter.OnStatusUpdate += new CswNbtImportExport.StatusUpdateHandler( SetStatus );

                XmlDocument ExportXml = Exporter.ExportAll( SelectedNodeTypes, ExportViews, ExportNodes );

                FileInfo DestinationFile = new FileInfo( FilePath );
                StreamWriter DestinationWriter = DestinationFile.CreateText();
                DestinationWriter.Write( ExportXml.InnerXml );
                DestinationWriter.Close();
            }
            catch( Exception ex )
            {
                SetStatus( "Error: " + ex.Message + "\r\n" + ex.StackTrace );
            }
        }

        //public void ClearSchema()
        //{
        //    SetStatus( "Clearing Existing Data" );

        //    CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

        //    // ORDER MATTERS, because of constraints

        //    //// Delete all properties with views
        //    //Collection<CswNbtMetaDataNodeTypeProp> ViewProps = new Collection<CswNbtMetaDataNodeTypeProp>();
        //    //foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )
        //    //{
        //    //    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps )
        //    //    {
        //    //        if( Prop.ViewId != Int32.MinValue )
        //    //            ViewProps.Add( Prop );
        //    //    }
        //    //}
        //    //foreach( CswNbtMetaDataNodeTypeProp Prop in ViewProps)
        //    //    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( Prop );

        //    SetStatus( "Clearing Views" );

        //    // Delete all views
        //    DataTable ViewsTable = _CswNbtSchemaModTrnsctn.getAllViews();
        //    foreach( DataRow ViewRow in ViewsTable.Rows )
        //    {
        //        CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( CswConvert.ToInt32( ViewRow["nodeviewid"] ) );
        //        View.Delete();
        //    }

        //    SetStatus( "Clearing NodeTypes, Nodes, and Properties" );

        //    // Delete all nodetypes
        //    // (which will delete all nodes, properties, and property values)
        //    Collection<CswNbtMetaDataNodeType> AllNodeTypes = new Collection<CswNbtMetaDataNodeType>();
        //    foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )
        //    {
        //        if( NodeType.IsLatestVersion )
        //            AllNodeTypes.Add( NodeType );
        //    }
        //    foreach( CswNbtMetaDataNodeType NodeType in AllNodeTypes )
        //        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeAllVersions( NodeType );

        //    // Delete property relational table mapping
        //    CswTableUpdate JctDdNtpUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "clearschema_JctDdNtp_Update", "jct_dd_ntp" );
        //    DataTable JctTable = JctDdNtpUpdate.getTable();
        //    foreach( DataRow JctRow in JctTable.Rows )
        //    {
        //        JctRow.Delete();
        //    }
        //    JctDdNtpUpdate.update( JctTable );

        //    SetStatus( "Clearing Sequences" );

        //    // Delete custom sequences
        //    DataTable SequenceTable = _CswNbtSchemaModTrnsctn.getAllSequences();
        //    foreach( DataRow SequenceRow in SequenceTable.Rows )
        //    {
        //        _CswNbtSchemaModTrnsctn.removeSequence( SequenceRow["sequencename"].ToString() );
        //    }

        //    // Disable all modules
        //    CswTableUpdate ModulesTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "clearschema_modules_update", "modules" );
        //    DataTable ModulesTable = ModulesTableUpdate.getTable();
        //    foreach( DataRow ModuleRow in ModulesTable.Rows )
        //    {
        //        ModuleRow["enabled"] = "0";
        //    }
        //    ModulesTableUpdate.update( ModulesTable );

        //    // Reset sequences
        //    //_CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "exec update_sequences;" );  // this is probably not MSSQL compatible

        //    //// Create Administrator role nodetype and node
        //    //CswNbtMetaDataObjectClass RoleObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
        //    //CswNbtObjClassRuleRole RoleRule = (CswNbtObjClassRuleRole) CswNbtObjClassRuleFactory.MakeRule( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
        //    //CswNbtMetaDataNodeType RoleNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( RoleObjectClass.ObjectClassId, "Role", "" );

        //    //CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( RoleNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
        //    //CswNbtObjClassRole RoleNodeAsRole = CswNbtNodeCaster.AsRole( RoleNode );
        //    //RoleNodeAsRole.Administrator.Checked = Tristate.True;
        //    //RoleNodeAsRole.Name.Text = "Administrator";
        //    //RoleNodeAsRole.Timeout.Value = 30;
        //    //RoleNodeAsRole.postChanges( false );

        //    //// Create an admin user nodetype and node
        //    //CswNbtMetaDataObjectClass UserObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
        //    //CswNbtObjClassRuleUser UserRule = (CswNbtObjClassRuleUser) CswNbtObjClassRuleFactory.MakeRule( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
        //    //CswNbtMetaDataNodeType UserNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( UserObjectClass.ObjectClassId, "User", "" );

        //    //CswNbtNode UserNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UserNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
        //    //CswNbtObjClassUser UserNodeAsUser = CswNbtNodeCaster.AsUser( UserNode );
        //    //UserNodeAsUser.AccountLocked.Checked = Tristate.False;
        //    //UserNodeAsUser.FirstNameProperty.Text = "Admin";
        //    //UserNodeAsUser.LastNameProperty.Text = "User";
        //    //UserNodeAsUser.UsernameProperty.Text = "admin";
        //    //UserNodeAsUser.PasswordProperty.Password = "admin";
        //    //UserNodeAsUser.Role.RelatedNodeId = RoleNode.NodeId;
        //    //UserNodeAsUser.Role.CachedNodeName = RoleNode.NodeName;
        //    //UserNodeAsUser.postChanges( false );

        //    //// Set permissions
        //    //CswNbtNodeTypePermissions RolePermissions = _CswNbtSchemaModTrnsctn.getNodeTypePermissions( RoleNodeAsRole, RoleNodeType );
        //    //RolePermissions.Create = true;
        //    //RolePermissions.Delete = true;
        //    //RolePermissions.Edit = true;
        //    //RolePermissions.View = true;
        //    //RolePermissions.Save();

        //    //CswNbtNodeTypePermissions UserPermissions = _CswNbtSchemaModTrnsctn.getNodeTypePermissions( RoleNodeAsRole, UserNodeType );
        //    //UserPermissions.Create = true;
        //    //UserPermissions.Delete = true;
        //    //UserPermissions.Edit = true;
        //    //UserPermissions.View = true;
        //    //UserPermissions.Save();

        //} // ClearSchema()

    } // class ImportThread
} // namespace
