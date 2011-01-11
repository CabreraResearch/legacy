using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT View based on Relationships
    /// </summary>
    [Serializable()]
    public class CswNbtView : IEquatable<CswNbtView>
    {
        /// <summary>
        /// CswNbtResources reference
        /// </summary>
        protected CswNbtResources _CswNbtResources;

        /// <summary>
        /// Character delimiter used for saving the view as a string
        /// </summary>
        public static char delimiter = '|';

        /// <summary>
        /// Type of View (for this class, it's always RelationshipView)
        /// </summary>
        private static string _ViewType = "RelationshipView";

        /// <summary>
        /// Root View Node
        /// </summary>
        public CswNbtViewRoot Root;

        /// <summary>
        /// Name of View
        /// </summary>
        public string ViewName
        {
            get { return Root.ViewName; }
            set { Root.ViewName = value; }
        }

        /// <summary>
        /// Icon for View
        /// </summary>
        public string IconFileName
        {
            get { return Root.IconFileName; }
        }

        /// <summary>
        /// Visibility permission setting
        /// </summary>
        public NbtViewVisibility Visibility
        {
            get { return Root.Visibility; }
            set { Root.Visibility = value; }
        }
        /// <summary>
        /// Visibility permission setting (restrict to user)
        /// </summary>
        public CswPrimaryKey VisibilityRoleId
        {
            get { return Root.VisibilityRoleId; }
            set { Root.VisibilityRoleId = value; }
        }
        /// <summary>
        /// Visibility permission setting (restrict to role)
        /// </summary>
        public CswPrimaryKey VisibilityUserId
        {
            get { return Root.VisibilityUserId; }
            set { Root.VisibilityUserId = value; }
        }

        /// <summary>
        /// Use view in Mobile
        /// </summary>
        public bool ForMobile
        {
            get { return Root.ForMobile; }
            set { Root.ForMobile = value; }
        }

        /// <summary>
        /// Category name (arbitrary string) 
        /// </summary>
        public string Category
        {
            get { return Root.Category; }
            set { Root.Category = value; }
        }
        /// <summary>
        /// Sets the View's Visibility
        /// </summary>
        public void SetVisibility( NbtViewVisibility Visibility, CswPrimaryKey RoleId, CswPrimaryKey UserId )
        {
            this.Visibility = Visibility;
            this.VisibilityRoleId = RoleId;
            this.VisibilityUserId = UserId;
        }
        /// <summary>
        /// Rendering Mode
        /// </summary>
        public NbtViewRenderingMode ViewMode
        {
            get { return Root.ViewMode; }
            set { Root.ViewMode = value; }
        }
        /// <summary>
        /// Sets the ViewMode
        /// </summary>
        public void SetViewMode( NbtViewRenderingMode value )
        {
            Root.ViewMode = value;
        }
        /// <summary>
        /// Grid Width
        /// </summary>
        public Int32 Width
        {
            get { return Root.Width; }
            set { Root.Width = value; }
        }
        /// <summary>
        /// Database Primary Key
        /// </summary>
        public Int32 ViewId
        {
            get { return Root.ViewId; }
            set { Root.ViewId = value; }
        }

        /// <summary>
        /// True if the View can be used as a search (contains a property filter)
        /// </summary>
        /// <returns></returns>
        public bool IsSearchable()
        {
            return ( this.FindFirstPropertyFilter() != null );
        }

        /// <summary>
        /// Constructor - empty view
        /// </summary>
        public CswNbtView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Clear();
        }

        #region Child constructors

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For copying an existing relationship
        /// </summary>
        public CswNbtViewRelationship CopyViewRelationship( CswNbtViewNode ParentViewNode, CswNbtViewRelationship CopyFrom, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, CopyFrom, IncludeDefaultFilters );
            if( ParentViewNode != null )
                ParentViewNode.AddChild( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For adding a nodetype to the root level of the view.
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtMetaDataNodeType NodeType, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, NodeType, IncludeDefaultFilters );
            if( this.Root != null )
                this.Root.AddChild( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For adding an object class to the root level of the view
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtMetaDataObjectClass ObjectClass, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, ObjectClass, IncludeDefaultFilters );
            if( this.Root != null )
                this.Root.AddChild( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For a relationship below the root level, determined by a nodetype property
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtViewRelationship ParentViewRelationship, CswNbtViewRelationship.PropOwnerType OwnerType, CswNbtMetaDataNodeTypeProp Prop, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, OwnerType, Prop, IncludeDefaultFilters );
            if( ParentViewRelationship != null )
                ParentViewRelationship.addChildRelationship( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For a relationship below the root level, determined by an object class property
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtViewRelationship ParentViewRelationship, CswNbtViewRelationship.PropOwnerType OwnerType, CswNbtMetaDataObjectClassProp Prop, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, OwnerType, Prop, IncludeDefaultFilters );
            if( ParentViewRelationship != null )
                ParentViewRelationship.addChildRelationship( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewProperty"/> for this view
        /// For adding a new nodetype property
        /// </summary>
        public CswNbtViewProperty AddViewProperty( CswNbtViewRelationship ParentViewRelationship, CswNbtMetaDataNodeTypeProp Prop )
        {
            CswNbtViewProperty NewProperty = new CswNbtViewProperty( _CswNbtResources, this, Prop );
            if( ParentViewRelationship != null )
                ParentViewRelationship.addProperty( NewProperty );
            return NewProperty;
        }
        /// <summary>
        /// Creates a new <see cref="CswNbtViewProperty"/> for this view
        /// For adding a new object class property
        /// </summary>
        public CswNbtViewProperty AddViewProperty( CswNbtViewRelationship ParentViewRelationship, CswNbtMetaDataObjectClassProp Prop )
        {
            CswNbtViewProperty NewProperty = new CswNbtViewProperty( _CswNbtResources, this, Prop );
            if( ParentViewRelationship != null )
                ParentViewRelationship.addProperty( NewProperty );
            return NewProperty;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewPropertyFilter"/> for this view
        /// </summary>
        public CswNbtViewPropertyFilter AddViewPropertyFilter( CswNbtViewProperty ParentViewProperty, CswNbtSubField.SubFieldName SubFieldName, CswNbtPropFilterSql.PropertyFilterMode FilterMode, string Value, bool CaseSensitive )
        {
            CswNbtViewPropertyFilter NewFilter = new CswNbtViewPropertyFilter( _CswNbtResources, this, SubFieldName, FilterMode, Value, CaseSensitive );
            if( ParentViewProperty != null )
                ParentViewProperty.addFilter( NewFilter );
            return NewFilter;
        }

        #endregion Child constructors


        private Int32 _lastUniqueId = 0;
        /// <summary>
        /// This allows child View nodes to fetch an ID which is guaranteed unique within a view
        /// </summary>
        public string GenerateUniqueId()
        {
            _lastUniqueId++;
            return _lastUniqueId.ToString();
        }

        /// <summary>
        /// load view by view name
        /// </summary>
        /// <param name="ViewName">viewname in node_views table</param>
        /// <returns></returns>
        public bool LoadView( string ViewName )
        {
            bool ReturnVal = false;

            CswTableSelect NodeViewsTableSelect = _CswNbtResources.makeCswTableSelect( "loadviewbyviewname", "node_views" );
            DataTable NodeViewsTable = NodeViewsTableSelect.getTable( " where lower(viewname) = '" + ViewName.ToLower() + "'" );

            if( 1 == NodeViewsTable.Rows.Count )
            {
                if( null != NodeViewsTable.Rows[0]["viewxml"] )
                {
                    LoadXml( NodeViewsTable.Rows[0]["viewxml"].ToString() );
                    ReturnVal = true;
                }
            }

            return ( ReturnVal );
        }

        /// <summary>
        /// Load View XML into this View (String)
        /// </summary>
        public bool LoadXml( string ViewString )
        {
            string ViewXmlAsString = ViewString;
            //Prefixing the Type to the ViewXml is not required, for backwards compatibility
            string[] SplitStr = ViewString.Split( delimiter );
            if( SplitStr.Length > 1 )
            {
                //if( ( ( NbtViewType ) Enum.Parse( typeof( NbtViewType ), SplitStr[ 0 ] ) ) != ViewType )
                //    throw new CswDniException( "Invalid View", "Attempted to restore CswNbtView using ViewType: " + SplitStr[ 0 ] );
                ViewXmlAsString = SplitStr[1];
            }
            XmlDocument ViewXmlDoc = new XmlDocument();
            ViewXmlDoc.LoadXml( ViewXmlAsString );
            return _load( ViewXmlDoc );
        }
        /// <summary>
        /// Load View XML into this View (XML)
        /// </summary>
        public bool LoadXml( XmlDocument ViewXmlDoc )
        {
            return _load( ViewXmlDoc );
        }

        private bool _load( XmlDocument ViewXmlDoc )
        {
            XmlNode RootXmlNode = ViewXmlDoc.DocumentElement;

            // This handles the recursive load operation internally
            Root = new CswNbtViewRoot( _CswNbtResources, this, RootXmlNode );

            return true;
        }


        public delegate void BeforeEditViewEventHandler( CswNbtView View );
        public delegate void AfterEditViewEventHandler( CswNbtView View );
        public static string BeforeEditViewEventName = "BeforeEditView";
        public static string AfterEditViewEventName = "AfterEditView";

        /// <summary>
        /// Save the view to the database
        /// </summary>
        public void save()
        {
            if( ViewId == Int32.MinValue )
                throw new CswDniException( "Invalid View", "You must call makeNewView() before calling save() on a new view" );

            CswTableUpdate ViewTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtView_save_update", "node_views" );
            DataTable ViewTable = ViewTableUpdate.getTable( "nodeviewid", ViewId, true );
            if( ViewTable.Rows.Count == 0 )
                throw new CswDniException( "Invalid View", "No views that match viewid = " + ViewId.ToString() + " were found while attempting to save" );

            // Enforce name-visibility compound unique constraint
            if( ViewName == string.Empty )
                throw new CswDniException( "View name cannot be blank" );

            if( Visibility == NbtViewVisibility.Unknown )
                throw new CswDniException( "View visibility is Unknown", "User attempted to save a view (" + ViewId + ", " + ViewName + ") with visibility == Unknown" );

            if( !ViewIsUnique( ViewId, ViewName, Visibility, VisibilityUserId, VisibilityRoleId ) )
                throw new CswDniException( "View name is already in use", "There is already a view with name: " + ViewName + " and visibility setting: " + Visibility.ToString() );

            // Before Edit View Events
            Collection<object> BeforeEditEvents = _CswNbtResources.CswEventLinker.Trigger( BeforeEditViewEventName );
            foreach( object Handler in BeforeEditEvents )
            {
                if( Handler is BeforeEditViewEventHandler )
                    ( (BeforeEditViewEventHandler) Handler )( this );
            }


            ViewTable.Rows[0]["viewname"] = ViewName;
            ViewTable.Rows[0]["category"] = Category;
            ViewTable.Rows[0]["viewxml"] = this.ToString();
            ViewTable.Rows[0]["formobile"] = CswConvert.ToDbVal( ForMobile );
            ViewTable.Rows[0]["visibility"] = Visibility.ToString();
            if( Visibility == NbtViewVisibility.Role )
                ViewTable.Rows[0]["roleid"] = CswConvert.ToDbVal( VisibilityRoleId.PrimaryKey );
            else
                ViewTable.Rows[0]["roleid"] = DBNull.Value;
            if( Visibility == NbtViewVisibility.User )
                ViewTable.Rows[0]["userid"] = CswConvert.ToDbVal( VisibilityUserId.PrimaryKey );
            else
                ViewTable.Rows[0]["userid"] = DBNull.Value;
            ViewTableUpdate.update( ViewTable );


            // After Edit View Events
            Collection<object> AfterEditEvents = _CswNbtResources.CswEventLinker.Trigger( AfterEditViewEventName );
            foreach( object Handler in AfterEditEvents )
            {
                if( Handler is AfterEditViewEventHandler )
                    ( (AfterEditViewEventHandler) Handler )( this );
            }
        }

        /// <summary>
        /// Create a new view
        /// </summary>
        /// <param name="ViewName">Name of New View</param>
        /// <param name="Visibility">Visibility Permission setting</param>
        /// <param name="RoleId">Primary key of role restriction (if Visibility is Role-based)</param>
        /// <param name="UserId">Primary key of user restriction (if Visibility is User-based)</param>
        /// <param name="CopyViewId">Primary key of view to copy</param>
        public void makeNew( string ViewName, NbtViewVisibility Visibility, CswPrimaryKey RoleId, CswPrimaryKey UserId, Int32 CopyViewId )
        {
            CswNbtView CopyView = null;
            if( CopyViewId > 0 )
            {
                CswTableSelect ViewTableSelect = _CswNbtResources.makeCswTableSelect( "makenewview_select", "node_views" );
                DataTable CopyViewTable = ViewTableSelect.getTable( "nodeviewid", CopyViewId );
                if( CopyViewTable.Rows.Count > 0 )
                {
                    string CopyViewAsString = CopyViewTable.Rows[0]["viewxml"].ToString();
                    CopyView = new CswNbtView( _CswNbtResources );
                    CopyView.LoadXml( CopyViewAsString );
                }
            }
            makeNew( ViewName, Visibility, RoleId, UserId, CopyView );
        }

        public delegate void BeforeNewViewEventHandler();
        public delegate void AfterNewViewEventHandler( CswNbtView View );
        public static string BeforeNewViewEventName = "BeforeNewView";
        public static string AfterNewViewEventName = "AfterNewView";

        /// <summary>
        /// Create a new view
        /// </summary>
        /// <param name="ViewName">Name of New View</param>
        /// <param name="Visibility">Visibility Permission setting</param>
        /// <param name="RoleId">Primary key of role restriction (if Visibility is Role-based)</param>
        /// <param name="UserId">Primary key of user restriction (if Visibility is User-based)</param>
        /// <param name="CopyView">View to copy</param>
        public void makeNew( string ViewName, NbtViewVisibility Visibility, CswPrimaryKey RoleId, CswPrimaryKey UserId, CswNbtView CopyView )
        {
            if( ViewName == string.Empty )
                throw new CswDniException( "View name cannot be blank", "CswNbtView.makeNew() called with empty ViewName parameter" );

            // Enforce name-visibility compound unique constraint
            if( !ViewIsUnique( Int32.MinValue, ViewName, Visibility, UserId, RoleId ) )
                throw new CswDniException( "View name is already in use", "There is already a view with conflicting name and visibility settings" );

            // Before New View Events
            Collection<object> BeforeNewEvents = _CswNbtResources.CswEventLinker.Trigger( BeforeNewViewEventName );
            foreach( object Handler in BeforeNewEvents )
            {
                if( Handler is BeforeNewViewEventHandler )
                    ( (BeforeNewViewEventHandler) Handler )();
            }

            // Insert a new view
            CswTableUpdate ViewTableUpdate = _CswNbtResources.makeCswTableUpdate( "ViewTableUpdate", "node_views" );
            DataTable ViewTable = ViewTableUpdate.getEmptyTable();

            DataRow NewRow = ViewTable.NewRow();
            NewRow["viewname"] = ViewName;
            NewRow["formobile"] = CswConvert.ToDbVal( ForMobile );
            NewRow["visibility"] = Visibility.ToString();

            NewRow["userid"] = CswConvert.ToDbVal( Int32.MinValue );
            if( UserId != null )
                NewRow["userid"] = CswConvert.ToDbVal( UserId.PrimaryKey );

            NewRow["roleid"] = CswConvert.ToDbVal( Int32.MinValue );
            if( Visibility == NbtViewVisibility.Role && RoleId != null )
                NewRow["roleid"] = CswConvert.ToDbVal( RoleId.PrimaryKey );

            ViewTable.Rows.Add( NewRow );
            ViewTableUpdate.update( ViewTable );

            // Reset this view info to the new one
            Clear();
            if( CopyView != null )
                this.LoadXml( CopyView.ToXml() );
            this.ViewId = CswConvert.ToInt32( NewRow["nodeviewid"].ToString() );
            this.ViewName = ViewName;
            this.Visibility = Visibility;
            this.VisibilityRoleId = RoleId;
            this.VisibilityUserId = UserId;
            this.ForMobile = ForMobile;

            // The XML includes the viewid and viewname, so it has to be updated before it can be saved
            NewRow["viewxml"] = this.ToString();
            ViewTableUpdate.update( ViewTable );

            _CswNbtResources.ViewSelect.clearCache();  // BZ 9197


            // After New View Events
            Collection<object> AfterNewEvents = _CswNbtResources.CswEventLinker.Trigger( AfterNewViewEventName );
            foreach( object Handler in AfterNewEvents )
            {
                if( Handler is AfterNewViewEventHandler )
                    ( (AfterNewViewEventHandler) Handler )( this );
            }
        }

        /// <summary>
        /// For importing a view from another schema
        /// </summary>
        public void ImportView( string ViewName, string ViewXml, Dictionary<Int32, Int32> NodeTypeMap, Dictionary<Int32, Int32> NodeTypePropMap, Dictionary<string, Int32> NodeMap )
        {
            this.makeNew( ViewName, NbtViewVisibility.Unknown, null, null, null );
            Int32 NewViewId = this.ViewId;
            this.LoadXml( ViewXml );  // this overwrites the viewid
            this.ViewId = NewViewId;  // so set it back

            foreach( CswNbtViewRelationship Relationship in this.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
            {
                if( Relationship.PropId != Int32.MinValue )
                {
                    if( Relationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                    {
                        CswNbtMetaDataNodeTypeProp NewProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropMap[Relationship.PropId] );
                        Relationship.overrideProp( Relationship.PropOwner, NewProp );
                    }
                }
                else
                {
                    if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType NewNodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeMap[Relationship.SecondId] );
                        Relationship.overrideSecond( NewNodeType );
                    }
                }
            }
            foreach( CswNbtViewProperty Property in this.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewProperty ) )
            {
                if( Property.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                {
                    if( NodeTypePropMap.ContainsKey( Property.NodeTypePropId ) )
                        Property.NodeTypePropId = NodeTypePropMap[Property.NodeTypePropId];
                }
            }

            // Map the role and user
            if( this.VisibilityRoleId != null && this.VisibilityRoleId.PrimaryKey != Int32.MinValue )
                this.VisibilityRoleId = new CswPrimaryKey( "nodes", NodeMap[this.VisibilityRoleId.PrimaryKey.ToString().ToLower()] );
            if( this.VisibilityUserId != null && this.VisibilityUserId.PrimaryKey != Int32.MinValue )
                this.VisibilityUserId = new CswPrimaryKey( "nodes", NodeMap[this.VisibilityUserId.PrimaryKey.ToString().ToLower()] );

        } // ImportView()


        public delegate void BeforeDeleteViewEventHandler( CswNbtView View );
        public delegate void AfterDeleteViewEventHandler();
        public static string BeforeDeleteViewEventName = "BeforeDeleteView";
        public static string AfterDeleteViewEventName = "AfterDeleteView";
        /// <summary>
        /// Delete this view from the database
        /// This will reset all users using this view as their default to not having a default view.
        /// </summary>
        public void Delete()
        {
            // Before Delete View Events
            Collection<object> BeforeDeleteEvents = _CswNbtResources.CswEventLinker.Trigger( BeforeDeleteViewEventName );
            foreach( object Handler in BeforeDeleteEvents )
            {
                if( Handler is BeforeDeleteViewEventHandler )
                    ( (BeforeDeleteViewEventHandler) Handler )( this );
            }


            // Reset user information

            CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

            // generate the view
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtView.Delete(" + ViewId + ")";
            CswNbtViewRelationship UserRelationship = View.AddViewRelationship( User_ObjectClass, false );

            // generate the tree
            ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );

            // reset the default view and quick launch views
            UserTree.goToRoot();
            if( UserTree.getChildNodeCount() > 0 )
            {
                for( Int32 u = 0; u < UserTree.getChildNodeCount(); u++ )
                {
                    UserTree.goToNthChild( u );
                    CswNbtNode UserNode = UserTree.getNodeForCurrentPosition();
                    CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) CswNbtNodeCaster.AsUser( UserNode );
                    // Remove this view from the Quick Launch views
                    if( UserNodeAsUser.QuickLaunchViews != null )
                    {
                        if( UserNodeAsUser.QuickLaunchViews.ContainsViewId( this.ViewId ) )
                            UserNodeAsUser.QuickLaunchViews.RemoveViewId( this.ViewId );
                    }
                    UserNode.postChanges( false );
                    UserTree.goToParentNode();
                }
            }

            // Now remove all foreign keys to this view
            CswTableUpdate NodeTypePropsUpdate = _CswNbtResources.makeCswTableUpdate( "DeleteView_prop_update", "nodetype_props" );
            DataTable NodeTypePropsTable = NodeTypePropsUpdate.getTable( "nodeviewid", ViewId );
            foreach( DataRow NTPRow in NodeTypePropsTable.Rows )
            {
                NTPRow["nodeviewid"] = CswConvert.ToDbVal( Int32.MinValue );
            }
            NodeTypePropsUpdate.update( NodeTypePropsTable );

            _CswNbtResources.ViewCache.clearFromCache( this );

            // Now delete the view
            CswTableUpdate ViewTableUpdate = _CswNbtResources.makeCswTableUpdate( "Delete_view_nodeview_update", "node_views" );
            DataTable ViewTable = ViewTableUpdate.getTable( "nodeviewid", ViewId );
            if( ViewTable.Rows.Count > 0 )
            {
                ViewTable.Rows[0].Delete();
                ViewTableUpdate.update( ViewTable );
            }

            _CswNbtResources.ViewSelect.clearCache();

            // After Delete View Events
            Collection<object> AfterDeleteEvents = _CswNbtResources.CswEventLinker.Trigger( AfterDeleteViewEventName );
            foreach( object Handler in AfterDeleteEvents )
            {
                if( Handler is AfterDeleteViewEventHandler )
                    ( (AfterDeleteViewEventHandler) Handler )();
            }
        } // Delete()

        private bool ViewIsUnique( Int32 ViewId, string ViewName, NbtViewVisibility Visibility, CswPrimaryKey UserId, CswPrimaryKey RoleId )
        {
            if( Visibility != NbtViewVisibility.Property )
            {
                CswTableSelect CheckViewTableSelect = _CswNbtResources.makeCswTableSelect( "ViewIsUnique_select", "node_views" );
                string WhereClause = "where viewname = '" + CswTools.SafeSqlParam( ViewName ) + "'";
                if( ViewId > 0 )
                    WhereClause += " and nodeviewid <> " + ViewId.ToString();

                switch( Visibility )
                {
                    case NbtViewVisibility.User:
                        // Must be unique against other private views for this user
                        // Must be unique against all role and global views 
                        WhereClause += " and ((visibility = '" + Visibility.ToString() + "'";
                        WhereClause += "       and userid = " + UserId.PrimaryKey.ToString() + ")";
                        WhereClause += "      or visibility <> '" + Visibility.ToString() + "')";
                        break;
                    case NbtViewVisibility.Role:
                        // Must be unique against other role views for the same role
                        // Must be unique against all private and global views 
                        WhereClause += " and ((visibility = '" + Visibility.ToString() + "'";
                        WhereClause += "       and roleid = " + RoleId.PrimaryKey.ToString() + ")";
                        WhereClause += "      or visibility <> '" + Visibility.ToString() + "')";
                        break;
                    case NbtViewVisibility.Global:
                        // Must be globally unique 
                        break;
                }
                // don't include Property views for uniqueness
                WhereClause += " and visibility <> '" + NbtViewVisibility.Property.ToString() + "'";
                //DataTable CheckViewTable = CheckViewTableSelect.getTable( WhereClause );
                return ( CheckViewTableSelect.getRecordCount( WhereClause ) == 0 );
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Convert the View XML to a String
        /// </summary>
        public override string ToString()
        {
            XmlDocument Doc = this.ToXml();
            return _ViewType.ToString() + delimiter + Doc.InnerXml;
        }
        /// <summary>
        /// Returns the View XML as XML
        /// </summary>
        public XmlDocument ToXml()
        {
            XmlDocument Doc = new XmlDocument();
            Doc.AppendChild( Root.ToXml( Doc ) );
            return Doc;
        }

        /// <summary>
        /// Blank out the View
        /// </summary>
        public void Clear()
        {
            string OldName = string.Empty;
            Int32 ViewId = Int32.MinValue;
            if( Root != null )
            {
                OldName = Root.ViewName;
                ViewId = Root.ViewId;
            }
            Root = new CswNbtViewRoot( _CswNbtResources, this );
            Root.ViewName = OldName;
            Root.ViewId = ViewId;
        }

        #region Find ViewNode

        /// <summary>
        /// Determines if all the object classes and nodetypes are enabled in a view
        /// </summary>
        public bool IsFullyEnabled()
        {
            return IsFullyEnabledRecursive( Root );
        }
        private bool IsFullyEnabledRecursive( CswNbtViewNode ViewNode )
        {
            bool ret = true;
            if( ViewNode is CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                {
                    ret = ret && IsFullyEnabledRecursive( Child );
                    if( !ret ) break;
                }
            }
            else if( ViewNode is CswNbtViewRelationship )
            {
                // Make sure this nodetype/object class is enabled
                CswNbtViewRelationship ThisRelationship = ViewNode as CswNbtViewRelationship;
                if( ThisRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisRelationship.SecondId );
                    ret = ret && ( ThisNodeType != null );
                }
                else
                {
                    CswNbtMetaDataObjectClass ThisObjectClass = _CswNbtResources.MetaData.getObjectClass( ThisRelationship.SecondId );
                    ret = ret && ( ThisObjectClass != null );
                }

                // Recurse
                foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                {
                    ret = ret && IsFullyEnabledRecursive( Child );
                    if( !ret ) break;
                }
                if( ret )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                    {
                        ret = ret && IsFullyEnabledRecursive( Child );
                        if( !ret ) break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Returns true if the NodeType is present in one of the View Relationships
        /// </summary>
        public bool ContainsNodeType( CswNbtMetaDataNodeType NodeType )
        {
            return ContainsNodeTypeRecursive( Root.ChildRelationships, NodeType );
        }
        private bool ContainsNodeTypeRecursive( Collection<CswNbtViewRelationship> Relationships, CswNbtMetaDataNodeType NodeType )
        {
            bool ReturnVal = false;
            foreach( CswNbtViewRelationship CurrentRelationship in Relationships )
            {
                if( ( ( CswNbtViewRelationship.RelatedIdType.NodeTypeId == CurrentRelationship.FirstType ) &&
                     ( CurrentRelationship.FirstId == NodeType.FirstVersionNodeTypeId ) ) ||
                    ( ( CswNbtViewRelationship.RelatedIdType.NodeTypeId == CurrentRelationship.SecondType ) &&
                     ( CurrentRelationship.SecondId == NodeType.FirstVersionNodeTypeId ) ) )
                {
                    ReturnVal = true;
                    break;
                }
                else if( CurrentRelationship.ChildRelationships.Count > 0 )
                {
                    ReturnVal = ContainsNodeTypeRecursive( CurrentRelationship.ChildRelationships, NodeType );
                    if( ReturnVal ) break;
                }
            }
            return ( ReturnVal );
        }

        /// <summary>
        /// Returns true if the NodeType Prop is present in one of the View Properties
        /// </summary>
        public bool ContainsNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            return ContainsNodeTypePropRecursive( Root.ChildRelationships, NodeTypeProp );
        }

        private bool ContainsNodeTypePropRecursive( Collection<CswNbtViewRelationship> Relationships, CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            bool ReturnVal = false;
            foreach( CswNbtViewRelationship CurrentRelationship in Relationships )
            {
                foreach( CswNbtViewProperty CurrentProp in CurrentRelationship.Properties )
                {
                    if( CurrentProp.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId &&
                        CurrentProp.NodeTypePropId == NodeTypeProp.FirstPropVersionId )
                    {
                        ReturnVal = true;
                        break;
                    }
                }

                if( ReturnVal )
                    break;

                if( CurrentRelationship.ChildRelationships.Count > 0 )
                {
                    ReturnVal = ContainsNodeTypePropRecursive( CurrentRelationship.ChildRelationships, NodeTypeProp );
                }

                if( ReturnVal )
                    break;
            }
            return ( ReturnVal );
        }

        /// <summary>
        /// Returns true if the ViewNode is present in the View
        /// </summary>
        public CswNbtViewNode FindViewNode( CswDelimitedString ViewNodeToFindAsString )
        {
            CswNbtViewNode ViewNodeToFind = CswNbtViewNode.makeViewNode( _CswNbtResources, this, ViewNodeToFindAsString );
            return FindViewNode( ViewNodeToFind );
        }

        /// <summary>
        /// Returns true if the ViewNode is present in the View
        /// </summary>
        public CswNbtViewNode FindViewNode( CswNbtViewNode ViewNodeToFind )
        {
            return FindViewNodeRecursive( Root, ViewNodeToFind );
        }

        private CswNbtViewNode FindViewNodeRecursive( CswNbtViewNode ViewNode, CswNbtViewNode Match )
        {
            CswNbtViewNode ret = null;
            if( ViewNode == Match )
            {
                ret = ViewNode;
            }
            else
            {
                // Recurse
                if( ViewNode is CswNbtViewRoot )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                    {
                        ret = FindViewNodeRecursive( Child, Match );
                        if( ret != null )
                            break;
                    }
                }
                else if( ViewNode is CswNbtViewRelationship )
                {
                    foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                    {
                        ret = FindViewNodeRecursive( Child, Match );
                        if( ret != null )
                            break;
                    }
                    if( ret == null )
                    {
                        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                        {
                            ret = FindViewNodeRecursive( Child, Match );
                            if( ret != null )
                                break;
                        }
                    }
                }
                else if( ViewNode is CswNbtViewProperty )
                {
                    foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
                    {
                        ret = FindViewNodeRecursive( Child, Match );
                        if( ret != null )
                            break;
                    }
                }
            }
            return ret;
        } // FindViewNodeRecursive

        public CswNbtViewNode FindViewNodeByArbitraryId( string ArbitraryId )
        {
            return FindViewNodeByArbitraryIdRecursive( Root, ArbitraryId );
        }
        private CswNbtViewNode FindViewNodeByArbitraryIdRecursive( CswNbtViewNode ViewNode, string ArbitraryId )
        {
            CswNbtViewNode ret = null;
            if( ViewNode.ArbitraryId == ArbitraryId )
            {
                ret = ViewNode;
            }
            else
            {
                // Recurse
                if( ViewNode is CswNbtViewRoot )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                    {
                        ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                        if( ret != null )
                            break;
                    }
                }
                else if( ViewNode is CswNbtViewRelationship )
                {
                    foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                    {
                        ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                        if( ret != null )
                            break;
                    }
                    if( ret == null )
                    {
                        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                        {
                            ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                            if( ret != null )
                                break;
                        }
                    }
                }
                else if( ViewNode is CswNbtViewProperty )
                {
                    foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
                    {
                        ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                        if( ret != null )
                            break;
                    }
                }
            }
            return ret;
        } // FindViewNodeByArbitraryIdRecursive

        public CswNbtViewNode FindViewNodeByUniqueId( string UniqueId )
        {
            return FindViewNodeByUniqueIdRecursive( Root, UniqueId );
        }
        private CswNbtViewNode FindViewNodeByUniqueIdRecursive( CswNbtViewNode ViewNode, string UniqueId )
        {
            CswNbtViewNode ret = null;
            if( ViewNode.UniqueId == UniqueId )
            {
                ret = ViewNode;
            }
            else
            {
                // Recurse
                if( ViewNode is CswNbtViewRoot )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                    {
                        ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                        if( ret != null )
                            break;
                    }
                }
                else if( ViewNode is CswNbtViewRelationship )
                {
                    foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                    {
                        ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                        if( ret != null )
                            break;
                    }
                    if( ret == null )
                    {
                        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                        {
                            ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                            if( ret != null )
                                break;
                        }
                    }
                }
                else if( ViewNode is CswNbtViewProperty )
                {
                    foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
                    {
                        ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                        if( ret != null )
                            break;
                    }
                }
            }
            return ret;
        } // FindViewNodeByUniqueIdRecursive



        /// <summary>
        /// Returns the CswNbtViewProperty which corresponds to the property type and primary key provided
        /// </summary>
        public CswNbtViewProperty FindPropertyById( CswNbtViewProperty.CswNbtPropType PropType, Int32 PropId )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
            {
                ret = FindPropertyByIdRecursive( Child, PropType, PropId );
                if( ret != null )
                    break;
            }
            return ret;
        }
        private CswNbtViewProperty FindPropertyByIdRecursive( CswNbtViewRelationship Relationship, CswNbtViewProperty.CswNbtPropType PropType, Int32 PropId )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewProperty ChildProperty in Relationship.Properties )
            {
                if( ( PropType == CswNbtViewProperty.CswNbtPropType.ObjectClassPropId && ChildProperty.ObjectClassPropId == PropId ) ||
                    ( PropType == CswNbtViewProperty.CswNbtPropType.NodeTypePropId && ChildProperty.NodeTypePropId == PropId ) )
                {
                    ret = ChildProperty;
                    break;
                }
            }
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                if( ret != null )
                    break;
                ret = FindPropertyByIdRecursive( ChildRelationship, PropType, PropId );
            }
            return ret;

        }
        /// <summary>
        /// Returns the CswNbtViewProperty which corresponds to the property type and name provided
        /// </summary>
        public CswNbtViewProperty FindPropertyByName( string PropName )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
            {
                ret = FindPropertyByNameRecursive( Child, PropName );
                if( ret != null )
                    break;
            }
            return ret;
        }
        private CswNbtViewProperty FindPropertyByNameRecursive( CswNbtViewRelationship Relationship, string PropName )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewProperty ChildProperty in Relationship.Properties )
            {
                if( ChildProperty.Name.ToLower() == PropName.ToLower() )
                {
                    ret = ChildProperty;
                    break;
                }
            }
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                if( ret != null )
                    break;
                ret = FindPropertyByNameRecursive( ChildRelationship, PropName );
            }
            return ret;

        }

        /// <summary>
        /// Find the first Property in the view tree
        /// </summary>
        public CswNbtViewProperty FindFirstProperty()
        {
            return _findFirstPropertyRecursive( Root );
        }

        private CswNbtViewProperty _findFirstPropertyRecursive( CswNbtViewNode ViewNode )
        {
            CswNbtViewProperty ret = null;
            if( ViewNode is CswNbtViewProperty )
            {
                ret = (CswNbtViewProperty) ViewNode;
            }
            else if( ViewNode is CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship R in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                {
                    ret = _findFirstPropertyRecursive( R );
                    if( ret != null ) break;
                }
            }
            else if( ViewNode is CswNbtViewRelationship )
            {
                if( ( (CswNbtViewRelationship) ViewNode ).Properties.Count > 0 )
                    ret = (CswNbtViewProperty) ( (CswNbtViewRelationship) ViewNode ).Properties[0];

                if( ret == null )
                {
                    foreach( CswNbtViewRelationship R in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                    {
                        ret = _findFirstPropertyRecursive( R );
                        if( ret != null ) break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Find the first Property Filter in the view tree
        /// </summary>
        public CswNbtViewPropertyFilter FindFirstPropertyFilter()
        {
            return _findFirstFilterRecursive( Root );
        }

        private CswNbtViewPropertyFilter _findFirstFilterRecursive( CswNbtViewNode ViewNode )
        {
            CswNbtViewPropertyFilter ret = null;
            if( ViewNode is CswNbtViewPropertyFilter )
            {
                ret = (CswNbtViewPropertyFilter) ViewNode;
            }
            else if( ViewNode is CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship R in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                {
                    ret = _findFirstFilterRecursive( R );
                    if( ret != null ) break;
                }
            }
            else if( ViewNode is CswNbtViewRelationship )
            {
                foreach( CswNbtViewProperty P in ( (CswNbtViewRelationship) ViewNode ).Properties )
                {
                    ret = _findFirstFilterRecursive( P );
                    if( ret != null ) break;
                }
                if( ret == null )
                {
                    foreach( CswNbtViewRelationship R in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                    {
                        ret = _findFirstFilterRecursive( R );
                        if( ret != null ) break;
                    }
                }
            }
            else if( ViewNode is CswNbtViewProperty )
            {
                if( ( (CswNbtViewProperty) ViewNode ).Filters.Count > 0 )
                    ret = (CswNbtViewPropertyFilter) ( (CswNbtViewProperty) ViewNode ).Filters[0];
            }
            return ret;
        }

        #endregion Find ViewNode

        #region Sort Property

        /// <summary>
        /// Returns the property used to sort the View
        /// </summary>
        public CswNbtViewProperty getSortProperty()
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
            {
                ret = getSortPropertyRecursive( Child );
                if( ret != null )
                    break;
            }
            return ret;
        }
        private CswNbtViewProperty getSortPropertyRecursive( CswNbtViewRelationship ViewNode )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewProperty Child in ViewNode.Properties )
            {
                if( Child.SortBy )
                {
                    ret = Child;
                    break;
                }
            }
            foreach( CswNbtViewRelationship Child in ViewNode.ChildRelationships )
            {
                if( ret != null )
                    break;
                ret = getSortPropertyRecursive( Child );
            }
            return ret;
        }

        /// <summary>
        /// Sets the Property used to sort the View.  Only one property can have this setting.
        /// </summary>
        public void setSortProperty( CswNbtViewProperty Property, CswNbtViewProperty.PropertySortMethod SortMethod )
        {
            clearSortProperty();
            Property.SortBy = true;
            Property.SortMethod = SortMethod;
        }
        /// <summary>
        /// Clears all properties of the Sort Property setting
        /// </summary>
        public void clearSortProperty()
        {
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
                clearSortPropertyRecursive( Child );
        }
        private void clearSortPropertyRecursive( CswNbtViewRelationship ViewNode )
        {
            foreach( CswNbtViewProperty Child in ViewNode.Properties )
                Child.SortBy = false;

            foreach( CswNbtViewRelationship Child in ViewNode.ChildRelationships )
                clearSortPropertyRecursive( Child );
        }

        #endregion Sort Property

        #region View Cache functions

        /// <summary>
        /// Save this View to Session's ViewCache
        /// </summary>
        public void SaveToCache()
        {
            _SessionViewId = _CswNbtResources.ViewCache.putView( this );
        }

        private Int32 _SessionViewId = Int32.MinValue;
        /// <summary>
        /// Key for retrieving the view from the Session's View Cache
        /// </summary>
        public Int32 SessionViewId
        {
            get { return _SessionViewId; }
        }

        #endregion View Cache functions

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtView view1, CswNbtView view2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( view1, view2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) view1 == null ) || ( (object) view2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( view1.ToString() == view2.ToString() )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtView view1, CswNbtView view2 )
        {
            return !( view1 == view2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtView ) )
                return false;
            return this == (CswNbtView) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtView obj )
        {
            return this == (CswNbtView) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = 0;
            if( this.ViewId > 0 )
            {
                // Positive hashes are for saved views
                hashcode = this.ViewId;
            }
            else
            {
                // Negative hashes are for dynamic views or searches
                hashcode = Math.Abs( this.ToString().GetHashCode() ) * -1;
            }
            return hashcode;
        }
        #endregion IEquatable

    } // class CswNbtView


} // namespace ChemSW.Nbt



