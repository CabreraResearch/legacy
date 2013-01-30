using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.Schema
{
    class WorkerThread
    {
        private ICswLogger _CswLogger = null;
        private CswNbtResources _CswNbtResources = null;
        private CswNbtImportStatus _CswNbtImportStatus = null;

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

                if( false == _ResourcesAreInitted )
                {
                    _InitSessionResources();
                }

                _CswNbtResources.AccessId = value;
                //_CswNbtResources.refreshDataDictionary();

            }
        }

        public WorkerThread( string ConfigurationPath )
        {
            _ConfigurationPath = ConfigurationPath;

            _InitSessionResources();

        }



        /// <summary>
        /// Sigh. 
        /// What makes this import thread status object threadsafe is that the worker thread does _not_ hold on to a reference to it; 
        /// The reason we need this is because the worker thread object can't touch window controls until it has been invoked by one of those
        /// frickin' asynch invoker thingies. And we want nbt resources creation to be encapsulated in the worker thread. 
        /// So here we are then: the frickin' form will get its own copy of import status and presumably will not touch it once it 
        /// has called the asych invoker thingy anyway. 
        /// </summary>
        /// <returns></returns>
        public CswNbtImportStatus getThreadSafeImportStatus()
        {
            return ( new CswNbtImportStatus( _CswNbtResources ) );
        }



        private void _InitSessionResources()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.SchemInit, SetupMode.NbtExe, false, false );
            _CswNbtImportStatus = new CswNbtImportStatus( _CswNbtResources );
            _CswNbtImportExport = new CswNbtImportExport( _CswNbtResources, _CswNbtImportStatus );
            _CswLogger = _CswNbtResources.CswLogger;
            _CswNbtResources.InitCurrentUser = InitUser;

        }//constructor

        private bool _ResourcesAreInitted
        {
            get
            {
                return ( ( null != _CswNbtResources ) && ( null != _CswLogger ) && ( null != _CswNbtResources.InitCurrentUser ) );
            }//get
        }//_ResourcesAreInitted

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, SystemUserNames.SysUsr__SchemaImport );
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
            foreach( string CurrentAccessId in _CswNbtResources.CswDbCfgInfo.AccessIds )
            {
                _CswNbtResources.CswDbCfgInfo.makeConfigurationCurrent( CurrentAccessId );
                DataRow CurrentRow = DbInstances.NewRow();
                CurrentRow[ColName_AccessId] = CurrentAccessId.ToString();
                CurrentRow[ColName_ServerType] = _CswNbtResources.CswDbCfgInfo.CurrentServerType;
                CurrentRow[ColName_ServerName] = _CswNbtResources.CswDbCfgInfo.CurrentServerName;
                CurrentRow[ColName_UserName] = _CswNbtResources.CswDbCfgInfo.CurrentUserName;
                CurrentRow[ColName_UserCount] = _CswNbtResources.CswDbCfgInfo.CurrentUserCount;
                CurrentRow[ColName_Deactivated] = _CswNbtResources.CswDbCfgInfo.CurrentDeactivated;
                CurrentRow[ColName_Display] = CurrentAccessId + " (" + _CswNbtResources.CswDbCfgInfo.CurrentUserName + "@" + _CswNbtResources.CswDbCfgInfo.CurrentServerName + ")";
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


        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes()
        {
            return _CswNbtResources.MetaData.getNodeTypes();
        }

        public void writeNodeTypesAsXml()
        {
            CswArbitrarySelect CswArbitrarySelect = _CswNbtResources.makeCswArbitrarySelect( "getnodetypesforimport", "select t.nodetypename \"NodeType\",p.propname \"PropName\",f.fieldtype \"FieldType\" from nodetypes t join nodetype_props p on (t.nodetypeid=p.nodetypeid) join field_types f on (p.fieldtypeid=f.fieldtypeid)  order by t.nodetypename,p.propname" );
            DataTable DataTable = CswArbitrarySelect.getTable();
            DataTable.TableName = "NodeTypes";
            DataTable.WriteXml( "NbtNodeTypes.xml" );

        }

        public delegate void StatusMessageHandler( string Msg );
        public event StatusMessageHandler OnStatusChange;
        public void SetStatusMessage( string Msg )
        {
            if( OnStatusChange != null )
                OnStatusChange( Msg );
        }



        public delegate void ImportPhaseMessageHandler( CswNbtImportStatus CswNbtImportStatus );
        public event ImportPhaseMessageHandler OnImportPhaseChange;
        public void setImportPhase( CswNbtImportStatus CswNbtImportStatus )
        {
            if( null != OnImportPhaseChange )
            {
                OnImportPhaseChange( CswNbtImportStatus );
            }
        }//setImportPhase() 

        public void stopImport()
        {
            if( null != _CswNbtImportExport )
            {
                _CswNbtImportExport.stopImport();
            }

        }//stopImport()


        //public void reset()
        //{
        //    _CswNbtImportExport.reset();
        //}



        CswNbtImportExport _CswNbtImportExport = null;
        private string _FilePath = string.Empty;
        public delegate void ImportHandler( string FilePath, ImportMode ImportMode, ImportTablePopulationMode ImportTablePopulationMode ); //, bool ClearExisting );
        public void DoImport( string FilePath, ImportMode ImportMode, ImportTablePopulationMode ImportTablePopulationMode ) //, bool ClearExisting )
        {

            _FilePath = FilePath;
            _CswNbtImportStatus.Mode = ImportMode;

            try
            {

                _CswNbtImportExport.OnStatusUpdate += new StatusUpdateHandler( SetStatusMessage );
                _CswNbtImportExport.OnImportPhaseChange += new ImportPhaseHandler( setImportPhase );

                string ViewXml = string.Empty;
                string ResultXml = string.Empty;
                string ErrorLog = string.Empty;
                string FileContents = string.Empty;
                CswNbtImportExportFrame CswNbtImportExportFrame = null;



                if( _FilePath != string.Empty )
                {

                    if( File.Exists( _FilePath ) )
                    {
                        Stream FileStream = File.OpenRead( _FilePath );
                        StreamReader FileSR = new StreamReader( FileStream );
                        FileContents = FileSR.ReadToEnd();
                        FileStream.Close();

                        CswNbtImportExportFrame = new CswNbtImportExportFrame( _CswNbtResources, FileContents );

                        _CswNbtImportExport.ImportXml( _CswNbtImportStatus.Mode, ImportTablePopulationMode, CswNbtImportExportFrame, ref ViewXml, ref ResultXml, ref ErrorLog );

                        if( ErrorLog != string.Empty )
                        {
                            _CswNbtResources.logMessage( ErrorLog );
                            SetStatusMessage( "Errors Occurred.  Check Log." );
                        }


                        _CswNbtResources.finalize();

                    }
                    else
                    {
                        //ErrorLabel.Text = "File does not exist: " + DataFileName; // openFileDialog1.FileName;
                        SetStatusMessage( "File does not exist: " + _FilePath );
                    }


                }
                else
                {
                    SetStatusMessage( "No file name provided" );
                }// if(FileName != string.Empty)


            }
            catch( Exception ex )
            {
                SetStatusMessage( "Error: " + ex.Message + "\r\n" + ex.StackTrace );
            }
        } // DoImport()


        public delegate void ExportHandler( string FilePath, ICollection SelectedNodeTypes, bool ExportViews, bool ExportNodes );
        public void DoExport( string FilePath, IEnumerable<CswNbtMetaDataNodeType> SelectedNodeTypes, bool ExportViews, bool ExportNodes )
        {
            try
            {
                CswNbtImportExport Exporter = new CswNbtImportExport( _CswNbtResources, new CswNbtImportStatus( _CswNbtResources ) );
                Exporter.OnStatusUpdate += new StatusUpdateHandler( SetStatusMessage );

                XmlDocument ExportXml = Exporter.ExportAll( SelectedNodeTypes, ExportViews, ExportNodes );

                FileInfo DestinationFile = new FileInfo( FilePath );
                StreamWriter DestinationWriter = DestinationFile.CreateText();
                DestinationWriter.Write( ExportXml.InnerXml );
                DestinationWriter.Close();
            }
            catch( Exception ex )
            {
                SetStatusMessage( "Error: " + ex.Message + "\r\n" + ex.StackTrace );
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
        //    //CswNbtMetaDataObjectClass RoleObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.RoleClass );
        //    //CswNbtObjClassRuleRole RoleRule = (CswNbtObjClassRuleRole) CswNbtObjClassRuleFactory.MakeRule( CswNbtMetaDataObjectClassName.NbtObjectClass.RoleClass );
        //    //CswNbtMetaDataNodeType RoleNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( RoleObjectClass.ObjectClassId, "Role", "" );

        //    //CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( RoleNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
        //    //CswNbtObjClassRole RoleNodeAsRole = (CswNbtObjClassRole) RoleNode;
        //    //RoleNodeAsRole.Administrator.Checked = Tristate.True;
        //    //RoleNodeAsRole.Name.Text = "Administrator";
        //    //RoleNodeAsRole.Timeout.Value = 30;
        //    //RoleNodeAsRole.postChanges( false );

        //    //// Create an admin user nodetype and node
        //    //CswNbtMetaDataObjectClass UserObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass );
        //    //CswNbtObjClassRuleUser UserRule = (CswNbtObjClassRuleUser) CswNbtObjClassRuleFactory.MakeRule( CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass );
        //    //CswNbtMetaDataNodeType UserNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( UserObjectClass.ObjectClassId, "User", "" );

        //    //CswNbtNode UserNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UserNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
        //    //CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) UserNode;
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
