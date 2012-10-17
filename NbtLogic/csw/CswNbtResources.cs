using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Audit;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.Mail;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.RscAdo;
using ChemSW.Security;
using ChemSW.Session;
using ChemSW.TblDn;

namespace ChemSW.Nbt
{
    /// <summary>
    /// A collection of useful resources for NBT business logic.
    /// </summary>
    public class CswNbtResources : ICswResources
    {
        /// <summary>
        /// The MD5 seed used for NBT
        /// </summary>
        public string MD5Seed { get { return "52978"; } }
        public enum ConfigurationVariables
        {
            unknown,
            /// <summary>
            /// 1 = auditing is on; 0 = auditing is off
            /// </summary>
            auditing,
            /// <summary>
            /// Image to display on every page
            /// </summary>
            brand_pageicon,
            /// <summary>
            /// Title to display on every page
            /// </summary>
            brand_pagetitle,
            /// <summary>
            /// Records the last time Nbt Meta Data was altered
            /// </summary>
            cache_lastupdated,
            /// <summary>
            /// If set to 1, users can define their own barcodes on new containers.
            /// </summary>
            custom_barcodes,
            /// <summary>
            /// Format of database (oracle, mysql, mssql)
            /// </summary>
            dbformat,
            /// <summary>
            /// If 1, display error messages in the web interface.
            /// </summary>
            displayerrorsinui,
            /// <summary>
            /// Number of failed login attempts before a user's account is locked.
            /// </summary>
            failedloginlimit,
            /// <summary>
            /// Number of Generators to process in each scheduler cycle
            /// </summary>
            generatorlimit,
            /// <summary>
            /// Number of Targets to generate from a Generator in each scheduler cycle
            /// </summary>
            generatortargetlimit,
            /// <summary>
            /// If 1, Schema is in Demo mode
            /// </summary>
            is_demo,
            /// <summary>
            /// Enforce license agreement on all users
            /// </summary>
            license_type,
            /// <summary>
            /// Maximum depth of location controls
            /// </summary>
            loc_max_depth,
            /// <summary>
            /// If 1, use image-based location controls
            /// </summary>
            loc_use_images,
            /// <summary>
            /// Number of results to display for views on Mobile
            /// </summary>
            mobileview_resultlim,
            /// <summary>
            /// When set to 1, total quantity to deduct in DispenseContainer cannot exceed container netquantity.
            /// </summary>
            netquantity_enforced,
            /// <summary>
            /// Number of days before a password expires
            /// </summary>
            passwordexpiry_days,
            /// <summary>
            /// User password complexity level (0 - none; 1 - letters, numbers; 2 - letters, numbers, and symbols)
            /// </summary>
            password_complexity,
            /// <summary>
            /// User password minimum length (between 0 and 20)
            /// </summary>
            password_length,
            /// <summary>
            /// Unique identifier for the schema structure
            /// </summary>
            schemaid,
            /// <summary>
            /// Version of this Schema
            /// </summary>
            schemaversion,
            /// <summary>
            /// Show the Loading box on postback
            /// </summary>
            showloadbox,
            /// <summary>
            /// Maximum number of results per tree level
            /// </summary>
            treeview_resultlimit,
            /// <summary>
            /// Limit at which relationship values must be searched for
            /// </summary>
            relationshipoptionlimit,
            /// <summary>
            /// Limit the number of containers allowed to receive in a single operation
            /// </summary>
            container_receipt_limit,

            /// <summary>
            /// The maximum number of lines in comments fields
            /// </summary>
            total_comments_lines
        }

        /// <summary>
        /// Returns whether or not the resource's database connection is still working
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public bool IsDbConnectionHealthy( ref string ErrorMessage ) { return ( _CswResources.IsDbConnectionHealthy( ref ErrorMessage ) ); }

        private CswResources _CswResources;
        private CswNbtNodeCollection _CswNbtNodeCollection = null;
        private CswNbtActionCollection _ActionCollection;
        public CswNbtPermit Permit = null;
        private ICswNbtTreeFactory _CswNbtTreeFactory;
        private bool _ExcludeDisabledModules = true;
        public bool ExcludeDisabledModules { get { return _ExcludeDisabledModules; } }

        public double ServerInitTime = 0;
        public double TotalServerTime = 0;

        public PooledConnectionState PooledConnectionState { get { return ( _CswResources.PooledConnectionState ); } }


        /// <summary>
        /// Provides a means to get lists of views
        /// </summary>
        public CswNbtViewSelect ViewSelect;

        /// <summary>
        /// Provides a means to get session data
        /// </summary>
        public CswNbtSessionDataMgr SessionDataMgr;

        public CswSessionManager CswSessionManager = null;


        ///// <summary>
        ///// Stores all Views used in this session, indexed by SessionViewId
        ///// </summary>
        //public CswNbtViewCache ViewCache;

        /// <summary>
        /// This is for a select set of DB-aware classes ONLY.  Do not use for business logic.
        /// </summary>
        public CswResources CswResources
        {
            get { return ( _CswResources ); }
        }

        /// <summary>
        /// Event Linker to bind events without repeats
        /// This should NOT be cached
        /// </summary>
        public CswEventLinker CswEventLinker = new CswEventLinker();

        /// <summary>
        /// For unique naming and tracking
        /// </summary>
        public string _DebugID;

        public AppType AppType { get { return _CswResources.AppType; } }
        public bool IsDeleteModeLogical { get { return _CswResources.IsDeleteModeLogical(); } }
        public const string UnknownEnum = CswResources.UnknownEnum;
        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtResources( AppType AppType, ICswSetupVbls SetupVbls, ICswDbCfgInfo DbCfgInfo, bool ExcludeDisabledModules, bool IsDeleteModeLogical, ICswSuperCycleCache CswSuperCycleCache, ICswResources CswResourcesMaster = null, ICswLogger CswLogger = null )
        {

            _CswResources = new CswResources( AppType, SetupVbls, DbCfgInfo, IsDeleteModeLogical, CswSuperCycleCache, CswResourcesMaster, CswLogger );
            _DebugID = Guid.NewGuid().ToString(); // DateTime.Now.ToString();
            logMessage( "CswNbtResources CREATED GUID: " + _DebugID );

            _ExcludeDisabledModules = ExcludeDisabledModules;
            //ViewCache = new CswNbtViewCache( this );
            ViewSelect = new CswNbtViewSelect( this );
            SessionDataMgr = new CswNbtSessionDataMgr( this );
            Permit = new CswNbtPermit( this );
        }

        public ICswSuperCycleCache CswSuperCycleCache { get { return ( _CswResources.CswSuperCycleCache ); } }

        /// <summary>
        /// Information related to the set of workflows registered in the database
        /// </summary>
        public CswNbtActionCollection Actions
        {
            get
            {
                if( _ActionCollection == null )
                    _ActionCollection = new CswNbtActionCollection( this, _ExcludeDisabledModules );
                return _ActionCollection;
            }
        }


        #region Nodes and Trees

        public NodeEditMode EditMode = NodeEditMode.Edit;

        /// <summary>
        /// Access to the node factory.  Consider using Nodes instead.
        /// </summary>
        public CswNbtNodeFactory CswNbtNodeFactory
        {
            get
            {
                CswNbtNodeFactory ret = null;
                if( _CswNbtNodeCollection != null )
                    ret = _CswNbtNodeCollection.CswNbtNodeFactory;
                return ret;
            }
        }
        //private CswNbtTreeCache _CswNbtTreeCache = null;
        ///// <summary>
        ///// Access to all trees loaded during this session
        ///// </summary>
        //public CswNbtTreeCache Trees
        //{
        //    get
        //    {
        //        return ( _CswNbtTreeCache );
        //    }
        //}

        private CswNbtTreeBuilder _CswNbtTreeBuilder = null;
        public CswNbtTreeBuilder Trees
        {
            get
            {
                return _CswNbtTreeBuilder;
            }
        }

        /// <summary>
        /// Access to all nodes loaded during this session, and to create new nodes without trees or views
        /// </summary>
        public CswNbtNodeCollection Nodes
        {
            get { return _CswNbtNodeCollection; }
        }

        public CswNbtNode getNode( CswNbtNodeKey NodeKey, DateTime Date )
        {
            return _CswNbtNodeCollection.GetNode( NodeKey.NodeId, Date );
        }
        public CswNbtNode getNode( CswPrimaryKey NodePk, DateTime Date )
        {
            return _CswNbtNodeCollection.GetNode( NodePk, Date );
        }
        public CswNbtNode getNode( string NodeId, string NodeKey, CswDateTime Date )
        {
            return _CswNbtNodeCollection.GetNode( NodeId, NodeKey, Date );
        }

        #endregion Nodes and Trees


        #region CswNbtMetaData

        private CswNbtMetaData _CswNbtMetaData = null;
        /// <summary>
        /// A collection that provides information about (and allows editing of) object classes, nodetypes, properties, and field types.
        /// </summary>
        public CswNbtMetaData MetaData
        {
            get
            {
                if( _CswNbtMetaData == null && IsInitializedForDbAccess )
                {
                    _CswNbtMetaData = new CswNbtMetaData( this, _ExcludeDisabledModules );
                    // These events are cached, so we only need to assign them when we make a New CswNbtMetaData
                    assignMetaDataEvents( _CswNbtMetaData );
                }
                return ( _CswNbtMetaData );
            }
        }

        public void assignMetaDataEvents( CswNbtMetaData CswNbtMetaData )
        {
            CswNbtMetaData.OnMakeNewNodeType += new CswNbtMetaData.NewNodeTypeEventHandler( _CswNbtMetaData_OnMakeNewNodeType );
            CswNbtMetaData.OnCopyNodeType += new CswNbtMetaData.CopyNodeTypeEventHandler( _CswNbtMetaData_OnCopyNodeType );
            CswNbtMetaData.OnMakeNewNodeTypeProp += new CswNbtMetaData.NewNodeTypePropEventHandler( _CswNbtMetaData_OnMakeNewNodeTypeProp );
            CswNbtMetaData.OnDeleteNodeTypeProp += new CswNbtMetaData.DeletePropEventHandler( _CswNbtMetaData_OnDeleteNodeTypeProp );
            CswNbtMetaData.OnEditNodeTypePropName += new CswNbtMetaData.EditPropNameEventHandler( _CswNbtMetaData_OnEditNodeTypePropName );
            CswNbtMetaData.OnEditNodeTypeName += new CswNbtMetaData.EditNodeTypeNameEventHandler( _CswNbtMetaData_OnEditNodeTypeName );

        }//assignMetaDataEvents()

        public bool isTableDefinedInDataBase( string TableName ) { return ( _CswResources.isTableDefinedInDataBase( TableName ) ); }

        /// <summary>
        /// Delegate, when a nodetype name changes
        /// </summary>
        public delegate void EditNodeTypeNameEventHandler( CswNbtMetaDataNodeType EditedNodeType );
        /// <summary>
        /// Event, when a nodetype name changes
        /// </summary>
        public event EditNodeTypeNameEventHandler OnEditNodeTypeName = null;
        void _CswNbtMetaData_OnEditNodeTypeName( CswNbtMetaDataNodeType EditedNodeType )
        {
            if( OnEditNodeTypeName != null )
                OnEditNodeTypeName( EditedNodeType );
        }

        /// <summary>
        /// Delegate, when a nodetype property is added
        /// </summary>
        public delegate void NewNodeTypePropEventHandler( CswNbtMetaDataNodeTypeProp NewProp );
        /// <summary>
        /// Event, when a nodetype property is added
        /// </summary>
        public event NewNodeTypePropEventHandler OnMakeNewNodeTypeProp = null;
        void _CswNbtMetaData_OnMakeNewNodeTypeProp( CswNbtMetaDataNodeTypeProp NewProp )
        {
            if( OnMakeNewNodeTypeProp != null )
                OnMakeNewNodeTypeProp( NewProp );
        }

        /// <summary>
        /// Delegate, when a nodetype property name is edited
        /// </summary>
        public delegate void EditPropNameEventHandler( CswNbtMetaDataNodeTypeProp EditedProp );
        /// <summary>
        /// Event, when a nodetype property name is edited
        /// </summary>
        public event EditPropNameEventHandler OnEditNodeTypePropName = null;
        void _CswNbtMetaData_OnEditNodeTypePropName( CswNbtMetaDataNodeTypeProp EditedProp )
        {
            if( OnEditNodeTypePropName != null )
                OnEditNodeTypePropName( EditedProp );
        }

        /// <summary>
        /// Delegate, when a nodetype property is deleted
        /// </summary>
        public delegate void DeletePropEventHandler( CswNbtMetaDataNodeTypeProp DeletedProp );
        /// <summary>
        /// Event, when a nodetype property is deleted
        /// </summary>
        public event DeletePropEventHandler OnDeleteNodeTypeProp = null;
        void _CswNbtMetaData_OnDeleteNodeTypeProp( CswNbtMetaDataNodeTypeProp DeletedProp )
        {
            if( OnDeleteNodeTypeProp != null )
                OnDeleteNodeTypeProp( DeletedProp );
        }

        /// <summary>
        /// Delegate, when a new nodetype is created
        /// </summary>
        public delegate void NewNodeTypeEventHandler( CswNbtMetaDataNodeType NewNodeType, bool IsCopy );
        /// <summary>
        /// Event, when a new nodetype is created
        /// </summary>
        public event NewNodeTypeEventHandler OnMakeNewNodeType = null;
        void _CswNbtMetaData_OnMakeNewNodeType( CswNbtMetaDataNodeType NewNodeType, bool IsCopy )
        {
            if( OnMakeNewNodeType != null )
                OnMakeNewNodeType( NewNodeType, IsCopy );
        }

        /// <summary>
        /// Delegate, when a nodetype is copied
        /// </summary>
        public delegate void CopyNodeTypeEventHandler( CswNbtMetaDataNodeType OriginalNodeType, CswNbtMetaDataNodeType CopyNodeType );
        /// <summary>
        /// Event, when a nodetype is copied
        /// </summary>
        public event CopyNodeTypeEventHandler OnCopyNodeType = null;
        void _CswNbtMetaData_OnCopyNodeType( CswNbtMetaDataNodeType OriginalNodeType, CswNbtMetaDataNodeType CopyNodeType )
        {
            if( OnCopyNodeType != null )
                OnCopyNodeType( OriginalNodeType, CopyNodeType );
        }

        #endregion CswNbtMetaData


        #region Modules

        private CswNbtModuleManager _CswNbtModuleManager = null;
        public CswNbtModuleManager Modules
        {
            get
            {
                if( null == _CswNbtModuleManager )
                {
                    _CswNbtModuleManager = new CswNbtModuleManager( this );
                }
                return _CswNbtModuleManager;
            }
        }

        #endregion Modules


        #region Caching
        /*
        /// <summary>
        /// Prepare this class for storage in the cache
        /// </summary>
        ///        
        public void BeforeStoreInCache()
        {
            this.CswEventLinker = null;
            _CswNbtNodeCollection = null;        // case 21246
            //_CswResources.BeforeStoreInCache();
        }
         
        /// <summary>
        /// Clean-up after storing this class in the cache
        /// </summary>
        ///         
        public void AfterRestoreFromCache()
        {
            _CswResources.AfterRestoreFromCache();
            CswEventLinker = new CswEventLinker();
            _CswNbtNodeCollection = new CswNbtNodeCollection( this );  // case 21246
        }
         */


        public bool CacheInitialized
        {
            get
            {
                return ( _CswResources.CacheInitialized );
            }
        }

        /// <summary>
        /// Clear any cached values from any child classes
        /// </summary>
        public void ClearCache()
        {
            _clear();
            Modules.ClearModulesCache();
            ClearActionsCache();
        }

        /// <summary>
        /// Refresh the Actions Collection
        /// </summary>
        public void ClearActionsCache()
        {
            _ActionCollection = new CswNbtActionCollection( this, _ExcludeDisabledModules );
        }

        /// <summary>
        /// Stores the datetime that this class was cached
        /// </summary>
        public DateTime CachedDateTime
        {
            get { return _CswResources.CachedDateTime; }
        }

        #endregion Caching


        #region Database Interaction


        public void clearUpdates() { _CswResources.clearUpdates(); }


        /// <summary>
        /// AccessId for database connectivity
        /// </summary>
        public string AccessId
        {
            get
            {
                return ( _CswResources.AccessId );
            }
            set
            {
                if( _CswResources.AccessId != value )
                {
                    _CswResources.AccessId = value;  // This starts the transaction


                    //_CswNbtMetaData = _makeNewMetaData(); //bz # 9932,10241,10283 (quite a saga really)
                }
            }
        } // AccessId


        public void SetDbResources( PooledConnectionState PooledConnectionState )
        {
            _CswResources.SetDbResources( PooledConnectionState );
        }//SetDbResources


        /// <summary>
        /// During initialization, allows setting database resources
        /// </summary>
        public void SetDbResources( ICswNbtTreeFactory CswNbtTreeFactory, PooledConnectionState PooledConnectionState )
        {
            _CswNbtNodeCollection = new CswNbtNodeCollection( this ); //, _ICswNbtObjClassFactory );
            _CswNbtTreeFactory = CswNbtTreeFactory;
            _CswNbtTreeFactory.CswNbtResources = this;
            _CswNbtTreeFactory.CswNbtNodeCollection = _CswNbtNodeCollection;
            //_CswNbtTreeCache = new CswNbtTreeCache( this, _CswNbtTreeFactory );

            _CswNbtTreeBuilder = new CswNbtTreeBuilder( this, _CswNbtTreeFactory );
            _CswResources.SetDbResources( PooledConnectionState );

            _CswResources.OnGetAuditLevel = new Audit.GetAuditLevelHandler( handleGetAuditLevel );
        }

        private void handleGetAuditLevel( DataRow DataRow, ref string ReturnVal )
        {

            ReturnVal = AuditLevel.NoAudit;

            // case 22542
            // Override jct_nodes_props audit level with level set on nodetype prop
            if( DataRow.Table.TableName == "jct_nodes_props" )
            {
                Int32 NodeTypePropId = Int32.MinValue;
                if( DataRowState.Deleted != DataRow.RowState )
                {
                    NodeTypePropId = CswConvert.ToInt32( DataRow["nodetypepropid"] );
                }
                else
                {
                    NodeTypePropId = CswConvert.ToInt32( DataRow["nodetypepropid", DataRowVersion.Original] );
                }

                if( NodeTypePropId != Int32.MinValue )
                {
                    CswNbtMetaDataNodeTypeProp NodeTypeProp = MetaData.getNodeTypeProp( NodeTypePropId );


                    if( ( null != NodeTypeProp ) && ( AuditLevel.IsLevel1HigherThanLevel2( NodeTypeProp.AuditLevel, AuditLevel.NoAudit ) ) )
                    {
                        CswNbtMetaDataNodeType NodeType = NodeTypeProp.getNodeType();

                        if( ( null != NodeType ) && ( AuditLevel.IsLevel1HigherThanLevel2( NodeType.AuditLevel, AuditLevel.NoAudit ) ) ) //NodeType overrides NodeTypeProp (per order TDU)
                        {
                            ReturnVal = NodeTypeProp.AuditLevel;
                        }

                    }//if the prop says to audit

                } // if( NodeTypePropId != Int32.MinValue )

            } // if( DataRow.Table.TableName == "jct_nodes_props" )

        } // handleGetAuditLevel()

        /// <summary>
        /// Commits all posted changes and closes out the transaction
        /// </summary>
        public void finalize()
        {
            finalize( true );
        }

        /// <summary>
        /// Commits all posted changes and closes out the transaction
        /// </summary>
        public void finalize( bool Commit )
        {
            if( null != _CswNbtNodeCollection )
                _CswNbtNodeCollection.finalize();

            if( null != _CswNbtMetaData )
                _CswNbtMetaData.finalize();

            _CswResources.finalize( Commit );

            if( null != _CswNbtMetaData )
                _CswNbtMetaData.afterFinalize();
        }//finalize()

        /// <summary>
        /// Undoes all posted changes in the current transaction, to all CswTableUpdate datatables and in the database
        /// </summary>
        public void Rollback() { _CswResources.Rollback(); }
        /// <summary>
        /// Start an atomistic transaction
        /// </summary>
        public void beginTransaction() { _CswResources.beginTransaction(); }
        /// <summary>
        /// Commit atomistic transaction
        /// </summary>
        public void commitTransaction() { _CswResources.commitTransaction(); }


        /// <summary>
        /// Puts the database resources back into the resource pool
        /// </summary>
        public void releaseDbResources()
        {
            _clear();
            _CswResources.releaseDbResources();
        }


        private void _clear()
        {
            if( Nodes != null )
            {
                Nodes.Clear();
            }

            _CswNbtMetaData = null;

            _CswResources.ClearCache();

        }//clear() 

        /// <summary>
        /// Sets whether the transaction is atomistic
        /// </summary>
        public TransactionMode TransactionMode { get { return _CswResources.TransactionMode; } set { _CswResources.TransactionMode = value; } }

        /// <summary>
        /// Releases all database resources
        /// </summary>
        public void release()
        {
            _CswResources.release();
        }//release()

        #endregion Database Interaction


        #region Mail Report Events

        /// <summary>
        /// Store a nodeid on a mail report for emails later, based on node events
        /// </summary>
        public void runMailReportEvents( CswNbtMetaDataNodeType TargetNodeType, CswNbtObjClassMailReport.EventOption EventOpt, CswNbtNode TargetNode, Collection<CswNbtNodePropWrapper> ModifiedProperties )
        {
            // Find any matching mail reports
            CswNbtMetaDataObjectClass MailReportOC = MetaData.getObjectClass( NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp TargetTypeOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.TargetType );
            CswNbtMetaDataObjectClassProp EventOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.Event );
            CswNbtMetaDataObjectClassProp EnabledOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.Enabled );
            CswNbtMetaDataObjectClassProp NodesToReportOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.NodesToReport );

            CswNbtView MailReportsView = new CswNbtView( this );
            MailReportsView.ViewName = "runMailReportEventsView";
            CswNbtViewRelationship Rel1 = MailReportsView.AddViewRelationship( MailReportOC, false );
            // Nodetype matches
            MailReportsView.AddViewPropertyAndFilter( ParentViewRelationship: Rel1,
                                                      MetaDataProp: TargetTypeOCP,
                                                      FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains,
                                                      Value: TargetNodeType.FirstVersionNodeTypeId.ToString() );
            // Event matches
            MailReportsView.AddViewPropertyAndFilter( ParentViewRelationship: Rel1,
                                                      MetaDataProp: EventOCP,
                                                      FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                      Value: EventOpt.ToString() );
            // Enabled
            MailReportsView.AddViewPropertyAndFilter( ParentViewRelationship: Rel1,
                                                      MetaDataProp: EnabledOCP,
                                                      FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                      Value: Tristate.True.ToString() );
            // Can't check the view, because it depends on the user
            // But check for a matching property value being altered
            ICswNbtTree MailReportsTree = Trees.getTreeFromView( MailReportsView, RequireViewPermissions: false, IncludeSystemNodes: true );
            for( Int32 i = 0; i < MailReportsTree.getChildNodeCount(); i++ )
            {
                MailReportsTree.goToNthChild( i );
                
                CswNbtObjClassMailReport ThisMailReport = MailReportsTree.getNodeForCurrentPosition();
                CswNbtView MailReportView = this.ViewSelect.restoreView( ThisMailReport.ReportView.ViewId );
                bool IncludeNode = false;
                foreach( CswNbtNodePropWrapper PropWrapper in ModifiedProperties )
                {
                    CswNbtViewProperty ViewProp = MailReportView.findPropertyByName( PropWrapper.PropName );
                    if( null != ViewProp )
                    {
                        IncludeNode = true;
                        break; // if one property matches, that's enough
                    }
                } // foreach( CswNbtNodePropWrapper PropWrapper in Properties )
                if( IncludeNode )
                {
                    ThisMailReport.AddNodeToReport( TargetNode );
                }
                ThisMailReport.postChanges( false );

                MailReportsTree.goToParentNode();
            } // for( Int32 i = 0; i < MailReportsTree.getChildNodeCount(); i++ )
        } // runMailReportEvents()

        #endregion Mail Report Events


        #region Pass-thru to CswResources

        /// <summary>
        /// Builds a CswStaticSelect, for use with S4s
        /// </summary>
        /// <param name="UniqueName">Any arbitrary unique name, used to identify this query for debugging</param>
        /// <param name="S4Name">Name of static_sql_select statement</param>
        public CswStaticSelect makeCswStaticSelect( string UniqueName, string S4Name ) { return _CswResources.makeCswStaticSelect( UniqueName, S4Name ); }

        /// <summary>
        /// Builds a CswArbitrarySelect, which allows you to write your own SQL.  Some platform-neutrality-massaging is done, but make it as neutral as possible.
        /// </summary>
        /// <param name="UniqueName">Any arbitrary unique name, used to identify this query for debugging</param>
        /// <param name="SqlText">Arbitrary SQL, Oracle-compatible or platform neutral</param>
        public CswArbitrarySelect makeCswArbitrarySelect( string UniqueName, string SqlText ) { return _CswResources.makeCswArbitrarySelect( UniqueName, SqlText ); }

        /// <summary>
        /// Builds a CswTableSelect, for selects against a single table
        /// </summary>
        /// <param name="UniqueName">Any arbitrary unique name, used to identify this query for debugging</param>
        /// <param name="TableName">Name of table in schema</param>
        public CswTableSelect makeCswTableSelect( string UniqueName, string TableName ) { return _CswResources.makeCswTableSelect( UniqueName, TableName ); }


        /// <summary>
        /// Builds a CswTableUpdate, for updates against a single table
        /// </summary>
        /// <param name="UniqueName">Any arbitrary unique name, used to identify this query for debugging</param>
        /// <param name="TableName">Name of table in schema</param>
        public CswTableUpdate makeCswTableUpdate( string UniqueName, string TableName ) { return _CswResources.makeCswTableUpdate( UniqueName, TableName ); }



        /// <summary>
        /// Directly executes a select command against the database connection
        /// </summary>
        /// <param name="UniqueName"></param>
        /// <param name="SqlText"></param>
        /// <returns></returns>
        public DataTable execArbitraryPlatformNeutralSqlSelect( string UniqueName, string SqlText ) { return ( _CswResources.execArbitraryPlatformNeutralSqlSelect( UniqueName, SqlText ) ); }

        /// <summary>
        /// Executes arbitrary sql.  It's your job to make sure it's platform neutral.
        /// You should *strongly* consider using CswArbitrarySelect, CswTableSelect, or CswTableUpdate instead of this. 
        /// </summary>
        /// <returns>Number of rows affected</returns>
        public Int32 execArbitraryPlatformNeutralSql( string SqlText ) { return ( _CswResources.execArbitraryPlatformNeutralSql( SqlText ) ); }


        public void execStoredProc( string StoredProcName, List<CswStoredProcParam> Params ) { _CswResources.execStoredProc( StoredProcName, Params ); }
        public DataTable getStoredProcResult( string StoredProcName, List<CswStoredProcParam> Params ) { return ( _CswResources.getStoredProcResult( StoredProcName, Params ) ); }



        public bool getNextSchemaDumpFileInfo( string DirectoryId, ref string PhysicalDirectoryPath, ref string NameOfCurrentDump, ref string StatusMsg )
        {
            return ( _CswResources.getNextSchemaDumpFileInfo( DirectoryId, ref PhysicalDirectoryPath, ref NameOfCurrentDump, ref StatusMsg ) );
        }

        public bool takeADump( string DirectoryId, ref string DumpFileName, ref string StatusMessage )
        {
            return ( _CswResources.takeADump( DirectoryId, ref DumpFileName, ref StatusMessage ) );
        }


        /// <summary>
        /// Allows you to get a value from one of the setup variables (located in CswSetupVbls.xml)
        /// </summary>
        public ICswSetupVbls SetupVbls { get { return _CswResources.SetupVbls; } }
        /// <summary>
        /// Database connection configuration information
        /// </summary>
        public ICswDbCfgInfo CswDbCfgInfo { get { return _CswResources.CswDbCfgInfo; } }
        /// <summary>
        /// Our collection of current sessions
        /// </summary>
        public CswSessionAttrs Session { get { return _CswResources.Session; } }
        /// <summary>
        /// Reading of values located in the configuration_variables table
        /// </summary>
        /// 
        public CswConfigurationVariables ConfigVbls { get { return ( _CswResources.ConfigVbls ); } }

        private Int32 _TreeViewResultLimit = Int32.MinValue;
        public Int32 TreeViewResultLimit
        {
            get
            {
                if( _TreeViewResultLimit == Int32.MinValue )
                {
                    _TreeViewResultLimit = CswConvert.ToInt32( ConfigVbls.getConfigVariableValue( ConfigurationVariables.treeview_resultlimit.ToString() ) );
                    if( _TreeViewResultLimit == Int32.MinValue )
                    {
                        _TreeViewResultLimit = 1001;
                    }
                }
                return _TreeViewResultLimit;
            }
        } // TreeViewResultLimit

        //public string getConfigVariableValue( string VariableName ) { return _CswResources.getConfigVariableValue( VariableName ); }
        ///// <summary>
        ///// Setting of values located in the configuration_variables table
        ///// </summary>
        //public void setConfigVariableValue( string VariableName, string VariableValue ) { _CswResources.setConfigVariableValue( VariableName, VariableValue ); }
        ///// <summary>
        ///// The collection of variables and values in the configuration_variables table
        ///// </summary>
        ////public ICollection ConfigVariables { get { return _CswResources.ConfigVariables; } }
        //public CswConfigurationVariables CswConfigVbls { get { return ( _CswResources.ConfigVbls ); } }

        /// <summary>
        /// Information associated with the currently logged in user, Nbt-specific.
        /// </summary>
        public ICswNbtUser CurrentNbtUser { get { return _CswResources.CurrentUser as ICswNbtUser; } } //set { _CswResources.CurrentUser = (ICswNbtUser) value; } }
        /// <summary>
        /// Information associated with the currently logged in user, Nbt-specific.
        /// </summary>
        public InitCurrentUserHandler InitCurrentUser { get { return _CswResources.InitCurrentUser; } set { _CswResources.InitCurrentUser = value; } }
        /// <summary>
        /// Information associated with the currently logged in user, application-independent.
        /// </summary>
        public ICswUser CurrentUser { get { return _CswResources.CurrentUser as ICswUser; } } //set { _CswResources.CurrentUser = (ICswUser) value; } }
        /// <summary>
        /// Clear the current user (for reauthenticating)
        /// </summary>
        public void clearCurrentUser() { _CswResources.clearCurrentUser(); }

        /// <summary>
        /// The SMTP interface
        /// </summary>
        public CswMail CswMail { get { return _CswResources.CswMail; } }
        /// <summary>
        /// Returns the name of the primary key column for a table
        /// </summary>
        public string getPrimeKeyColName( string TableName ) { return _CswResources.getPrimeKeyColName( TableName ); }
        /// <summary>
        /// Returns a new primary key value (CswTableUpdate will do this for you)
        /// </summary>
        public Int32 getNewPrimeKey( string TableName ) { return _CswResources.getNewPrimeKey( TableName ); }
        /// <summary>
        /// Returns true if the table uses logical delete
        /// </summary>
        public bool isLogicalDeleteTable( string TableName ) { return _CswResources.isLogicalDeleteTable( TableName ); }
        /// <summary>
        /// Access to the logging mechanism
        /// </summary>
        public ICswLogger CswLogger { get { return _CswResources.CswLogger; } }
        /// <summary>
        /// Appends a message to the log
        /// </summary>
        public void logMessage( string Msg ) { _CswResources.logMessage( "(" + _DebugID + ")\t" + Msg ); }
        /// <summary>
        /// Appends a message to the log
        /// </summary>
        public void logMessage( string Msg, string Filter ) { _CswResources.logMessage( "(" + _DebugID + ")\t" + Msg, Filter ); }
        /// <summary>
        /// Appends a timer result message to the log
        /// </summary>
        public void logTimerResult( string Msg, string TimerResult ) { _CswResources.logTimerResult( "(" + _DebugID + ")\t" + Msg, TimerResult ); }
        /// <summary>
        /// Appends a timer result message to the log
        /// </summary>
        public void logTimerResult( string Msg, CswTimer Timer ) { _CswResources.logTimerResult( "(" + _DebugID + ")\t" + Msg, Timer ); }
        /// <summary>
        /// Appends a timer result message to the log
        /// </summary>
        public void logTimerResult( string Msg, string TimerResult, string Filter ) { _CswResources.logTimerResult( "(" + _DebugID + ")\t" + Msg, TimerResult, Filter ); }
        /// <summary>
        /// Appends an exception to the log
        /// </summary>
        public void logError( Exception Ex ) { _CswResources.logError( Ex ); }
        /// <summary>
        /// Returns true if database connectivity has been established
        /// </summary>
        public bool IsInitializedForDbAccess { get { return _CswResources.IsInitializedForDbAccess; } }
        /// <summary>
        /// Returns true if the AccessId is valid
        /// </summary>
        public bool doesAccessIdExist( string AccessId ) { return _CswResources.doesAccessIdExist( AccessId ); }
        /// <summary>
        /// Refreshes data dictionary content stored in cache
        /// </summary>
        public void refreshDataDictionary() { _CswResources.refreshDataDictionary(); }
        /// <summary>
        /// Table factory used to create datatables
        /// </summary>
        public CswDnTblFactory CswTblFactory { get { return _CswResources.CswTblFactory; } set { _CswResources.CswTblFactory = value; } }
        //public ICswTableCaddyFactory CswTableCaddyFactory { set { _CswResources.CswTableCaddyFactory = value; } }
        /// <summary>
        /// Returns true if the unique sequence exists (for use by CswSequenceManager)
        /// </summary>
        public bool doesUniqueSequenceExist( string SequenceName ) { return _CswResources.doesUniqueSequenceExist( SequenceName ); }
        /// <summary>
        /// Creates a new unique sequence (for use by CswSequenceManager)
        /// </summary>
        public void makeUniqueSequence( string SequenceName, Int32 SeedVal ) { _CswResources.makeUniqueSequence( SequenceName, SeedVal ); }
        /// <summary>
        /// Fetches the next unique sequence value, and increments the value (for use by CswSequenceManager)
        /// </summary>
        public Int32 getNextUniqueSequenceVal( string SequenceName ) { return _CswResources.getNextUniqueSequenceVal( SequenceName ); }
        /// <summary>
        /// Resets the sequence to a given value (for use by CswSequenceManager)
        /// </summary>
        public void resetUniqueSequenceVal( string SequenceName, Int32 Value ) { _CswResources.resetUniqueSequenceVal( SequenceName, Value ); }
        /// <summary>
        /// Deletes a unique sequence (for use by CswSequenceManager)
        /// </summary>
        public void removeUniqueSequence( string SequenceName ) { _CswResources.removeUniqueSequence( SequenceName ); }
        /// <summary>
        /// Fetches the next unique sequence value without incrementing the value (for use by CswSequenceManager)
        /// </summary>
        public Int32 getCurrentUniqueSequenceVal( string SequenceName ) { return _CswResources.getCurrentUniqueSequenceVal( SequenceName ); }
        /// <summary>
        /// Globally available request timer
        /// </summary>
        public CswTimer Timer { get { return _CswResources.Timer; } }
        /// <summary>
        /// True if any exception was thrown
        /// </summary>
        public bool AnErrorOccurred { get { return _CswResources.AnErrorOccurred; } set { _CswResources.AnErrorOccurred = value; } }
        /// <summary>
        /// Converts a date into a platform-specific date format
        /// </summary>
        public string getDbNativeDate( DateTime DateTimeVal ) { return _CswResources.getDbNativeDate( DateTimeVal ); }
        /// <summary>
        /// Provides meta data information about tables and columns, from data_dictionary
        /// </summary>
        public ICswDataDictionaryReader DataDictionary { get { return _CswResources.DataDictionary; } }
        /// <summary>
        /// Set the context information for this audit transaction
        /// </summary>
        public string AuditContext { set { _CswResources.AuditContext = value; } }

        /// <summary>
        /// Set the context information for this audit transaction
        /// </summary>
        public string AuditUsername { set { _CswResources.AuditUsername = value; } }
        /// <summary>
        /// Set the context information for this audit transaction
        /// </summary>
        public string AuditFirstName { set { _CswResources.AuditFirstName = value; } }
        /// <summary>
        /// Set the context information for this audit transaction
        /// </summary>
        public string AuditLastName { set { _CswResources.AuditLastName = value; } }


        public void sendSystemAlertEmail( string Subject, string Message ) { _CswResources.sendSystemAlertEmail( Subject, Message ); }
        public Collection<CswMailMessage> makeMailMessages( string Subject, string Message, string Recipient ) { return _CswResources.makeMailMessages( Subject, Message, Recipient ); }
        public void sendEmailNotification( Collection<CswMailMessage> MailMessages ) { _CswResources.sendEmailNotification( MailMessages ); }


        public string makeUniqueConstraint( string TableName, string ColumnName ) { return ( _CswResources.makeUniqueConstraint( TableName, ColumnName ) ); }
        public string makeUniqueConstraint( string TableName, string ColumnName, bool AddDdData ) { return ( _CswResources.makeUniqueConstraint( TableName, ColumnName, AddDdData ) ); }

        public bool doesFkConstraintExistInDb( string ConstraintName ) { return ( _CswResources.doesFkConstraintExistInDb( ConstraintName ) ); }
        public bool doesUniqueConstraintExistInDb( string ConstraintName ) { return ( _CswResources.doesUniqueConstraintExistInDb( ConstraintName ) ); }
        public string getUniqueConstraintName( string TableName, string ColumName ) { return ( _CswResources.getUniqueConstraintName( TableName, ColumName ) ); }

        #endregion Pass-thru to CswResources


    } // CswNbtResources

}//ChemSW.NbtResources
