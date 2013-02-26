using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{
    public class CswNbtTreeLoaderFromSearchByLevel : CswNbtTreeLoader
    {
        private CswNbtResources _CswNbtResources = null;
        private string _SearchTerm;
        private string _ExtraWhereClause;
        private ICswNbtUser _RunAsUser;
        private bool _IncludeSystemNodes = false;
        private bool _IncludeHiddenNodes;

        public CswNbtTreeLoaderFromSearchByLevel( CswNbtResources CswNbtResources, ICswNbtUser RunAsUser, ICswNbtTree pCswNbtTree, string SearchTerm, string WhereClause, bool IncludeSystemNodes, bool IncludeHiddenNodes )
            : base( pCswNbtTree )
        {
            _CswNbtResources = CswNbtResources;
            _RunAsUser = RunAsUser;
            _SearchTerm = SearchTerm;
            _ExtraWhereClause = WhereClause;
            _IncludeSystemNodes = IncludeSystemNodes;
            _IncludeHiddenNodes = IncludeHiddenNodes;
        }

        /// <summary>
        /// Returns the maximum number of properties in any Table Layout, for result limits
        /// </summary>
        private Int32 _getMaxPropertyCount()
        {
            // TODO: come back to this!
            return 10;
        }

        public override void load( bool RequireViewPermissions )
        {
            _CswNbtTree.makeRootNode( "", false );

            _CswNbtTree.goToRoot();

            if( string.Empty != _SearchTerm )
            {
                DataTable NodesTable = new DataTable();
                string Sql = _makeNodeSql();

                Int32 thisResultLimit = _CswNbtResources.TreeViewResultLimit * _getMaxPropertyCount();

                CswArbitrarySelect ResultSelect = _CswNbtResources.makeCswArbitrarySelect( "TreeLoader_select", Sql );
                CswTimer SqlTimer = new CswTimer();
                try
                {
                    NodesTable = ResultSelect.getTable( 0, thisResultLimit, false, false );
                }
                catch( Exception ex )
                {
                    throw new CswDniException( ErrorType.Error, "Invalid View", "_getNodes() attempted to run invalid SQL: " + Sql, ex );
                }

                if( SqlTimer.ElapsedDurationInSeconds > 2 )
                {
                    _CswNbtResources.logMessage( "Tree View SQL required longer than 2 seconds to run: " + Sql );
                }

                Int32 PriorNodeId = Int32.MinValue;
                Collection<CswNbtNodeKey> NewNodeKeys = null;
                Int32 RowCount = 1;
                foreach( DataRow NodesRow in NodesTable.Rows )
                {
                    Int32 ThisNodeId = CswConvert.ToInt32( NodesRow["nodeid"] );
                    Int32 ThisNodeTypeId = CswConvert.ToInt32( NodesRow["nodetypeid"] );

                    // Verify permissions
                    // this could be a performance problem
                    CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeTypeId );
                    if( false == RequireViewPermissions || _CswNbtResources.Permit.canAnyTab( CswNbtPermit.NodeTypePermission.View, ThisNodeType ) )
                    {
                        Int32 ThisNTPId = Int32.MinValue;
                        if( NodesTable.Columns.Contains( "nodetypepropid" ) )
                        {
                            ThisNTPId = CswConvert.ToInt32( NodesRow["nodetypepropid"] );
                        }

                        // don't include properties in search results to which the user has no permissions
                        if( false == RequireViewPermissions ||
                            ( _canViewNode( ThisNodeType, ThisNodeId ) &&
                              ( Int32.MinValue == ThisNTPId || _canViewProp( ThisNTPId, ThisNodeId ) ) ) )
                        {
                            // Handle property multiplexing
                            // This assumes that property rows for the same nodeid are next to one another
                            if( ThisNodeId != PriorNodeId )
                            {
                                PriorNodeId = ThisNodeId;
                                NewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( null, NodesRow, false, string.Empty, true, true, NbtViewAddChildrenSetting.None, RowCount );
                                RowCount++;
                            } // if( ThisNodeId != PriorNodeId )

                            if( NewNodeKeys != null && ThisNTPId != Int32.MinValue )
                            {
                                foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                                {
                                    _CswNbtTree.makeNodeCurrent( NewNodeKey );
                                    _CswNbtTree.addProperty( ThisNTPId,
                                                            CswConvert.ToInt32( NodesRow["jctnodepropid"] ),
                                                            NodesRow["propname"].ToString(),
                                                            NodesRow["gestalt"].ToString(),
                                                            CswConvert.ToString( NodesRow["fieldtype"] ),
                                                            CswConvert.ToString( NodesRow["field1"] ),
                                                            CswConvert.ToString( NodesRow["field2"] ),
                                                            CswConvert.ToInt32( NodesRow["field1_fk"] ),
                                                            CswConvert.ToInt32( NodesRow["field1_numeric"] ),
                                                            CswConvert.ToBoolean( NodesRow["hidden"] ) );
                                } // foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                            } // if( NewNodeKeys != null && ThisNTPId != Int32.MinValue )
                            _CswNbtTree.goToRoot();
                        } // if( _canViewNode( ThisNodeType, ThisNodeId ) &&
                    } // if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, ThisNodeTypeId ) )
                } // foreach(DataRow NodesRow in NodesTable.Rows)

                // case 24678 - Mark truncated results
                if( NodesTable.Rows.Count == thisResultLimit )
                {
                    // Mark root truncated
                    _CswNbtTree.goToRoot();
                    _CswNbtTree.setCurrentNodeChildrenTruncated( true );

                    // Remove the last node (whose properties might have been truncated)
                    _CswNbtTree.goToNthChild( ( _CswNbtTree.getChildNodeCount() - 1 ) );
                    _CswNbtTree.removeCurrentNode();
                }

                _CswNbtTree.goToRoot();
            }
        } // load()

        private bool _canViewNode( CswNbtMetaDataNodeType NodeType, int NodeId )
        {
            bool canView = true;
            CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClass( NodeType.ObjectClassId );
            #region Container View Inventory Group Permission
            if( ObjClass.ObjectClass.Value == NbtObjectClass.ContainerClass )
            {

                CswNbtObjClassContainer CswNbtObjClassContainer = _CswNbtResources.Nodes[CswConvert.ToPrimaryKey( "nodes_" + NodeId )];
                if( null != CswNbtObjClassContainer )
                {
                    canView = CswNbtObjClassContainer.canContainer( CswNbtPermit.NodeTypePermission.View, null );
                }
            }
            #endregion
            return canView;
        }

        private bool _canViewProp( int NodeTypePropId, int NodeId )
        {
            CswNbtMetaDataNodeTypeProp NTProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
            
            // Must have permission to at least one tab where this property appears
            Dictionary<Int32, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout> EditLayouts = NTProp.getEditLayouts();
            bool canView = EditLayouts.Values.Aggregate( false,
                                                        ( current, EditLayout ) => current || _CswNbtResources.Permit.canTab( 
                                                            CswNbtPermit.NodeTypePermission.View, 
                                                            NTProp.getNodeType(), 
                                                            _CswNbtResources.MetaData.getNodeTypeTab( EditLayout.TabId ) ) );

            #region Container Request Button Inventory Group Permission

            if( canView )
            {
                CswNbtMetaDataObjectClass ContainerClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                if( null != ContainerClass )
                {
                    CswNbtMetaDataObjectClassProp RequestProp = _CswNbtResources.MetaData.getObjectClassProp( ContainerClass.ObjectClassId, CswNbtObjClassContainer.PropertyName.Request );
                    if( NTProp.ObjectClassPropId == RequestProp.PropId )
                    {
                        CswNbtObjClassContainer CswNbtObjClassContainerInstance = _CswNbtResources.Nodes[CswConvert.ToPrimaryKey( "nodes_" + NodeId )];
                        if( null != CswNbtObjClassContainerInstance )
                        {
                            canView = CswNbtObjClassContainerInstance.canContainer( _CswNbtResources.Actions[CswNbtActionName.Submit_Request] );
                        }
                    }
                }
            }

            #endregion

            #region Material Receive Button View Permission

            if( canView )
            {
                CswNbtMetaDataObjectClass MaterialClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                if( null != MaterialClass )
                {
                    CswNbtMetaDataObjectClassProp RequestProp = _CswNbtResources.MetaData.getObjectClassProp(MaterialClass.ObjectClassId, CswNbtObjClassMaterial.PropertyName.Receive);
                    if (NTProp.ObjectClassPropId == RequestProp.PropId)
                    {
                        canView = _CswNbtResources.Permit.can(CswNbtActionName.Receiving);
                    }
                }
            }

            #endregion

            return canView;
        }

        private string _makeNodeSql()
        {
            string Select = @"select n.nodeid,
                                     n.nodename, 
                                     n.locked,
                                     nvl(n.iconfilename, t.iconfilename) iconfilename, 
                                     t.nodetypename,
                                     t.nametemplate,
                                     t.nodetypeid,
                                     o.objectclass,
                                     o.objectclassid";
            string From = @"from nodes n
                            join nodetypes t on (n.nodetypeid = t.nodetypeid)
                            join object_class o on (t.objectclassid = o.objectclassid) ";
            string Where = string.Empty;
            string OrderBy = string.Empty;

            // Filter out disabled nodetypes/object classes (see case 26029)
            Where += "where t.enabled = '1' ";
            
            Select += ",lower(n.nodename) mssqlorder ";
            OrderBy = " order by lower(n.nodename)";
            OrderBy += ",n.nodeid,lower(props.propname) "; // for property multiplexing

            IEnumerable<string> SafeLikeClauses = _makeSafeLikeClauses();

            // Properties
            Select += @" ,props.nodetypepropid, props.propname, props.fieldtype, propval.jctnodepropid, propval.gestalt, propval.field1, propval.field2, propval.field1_fk, propval.field1_numeric, propval.hidden   ";

            From += @" left outer join (select p.nodetypeid, p.nodetypepropid, p.propname, f.fieldtype, nl.nodetypelayoutid, nl.display_row
                                                  from nodetype_props p
                                                  join field_types f on p.fieldtypeid = f.fieldtypeid
                                                  left outer join nodetype_layout nl on (nl.nodetypepropid = p.nodetypepropid and nl.layouttype = 'Table')
                                                 where (nl.nodetypelayoutid is not null 
                                                        or f.fieldtype in ('Image', 'MOL')
                                                        or (f.searchable = '1' 
                                                            and p.nodetypepropid in (select nodetypepropid 
                                                                                  from jct_nodes_props j ";

            bool first = true;
            foreach( string SafeLikeClause in SafeLikeClauses )
            {
                if( first )
                {
                    From += @"                                                   where (lower(j.gestaltsearch) " + SafeLikeClause + @" ";
                    Where += @" and (";
                    first = false;
                }
                else
                {
                    From += @"                                                     or lower(j.gestaltsearch) " + SafeLikeClause + @" ";
                    Where += @" and ";
                }

                // case 26827 - search deferment
                Where += @" n.nodeid in (select jnp.nodeid
                                           from jct_nodes_props jnp
                                           join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                           join nodetypes t on (p.nodetypeid = t.nodetypeid)
                                           join field_types f on (p.fieldtypeid = f.fieldtypeid)
                                          where f.searchable = '1'
                                            and t.searchdeferpropid is null
                                            and (lower(jnp.gestaltsearch) " + SafeLikeClause + @" )
                                      UNION
                                         select rn.nodeid
                                           from nodes n
                                           join jct_nodes_props jnp on jnp.nodeid = n.nodeid
                                           join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                           join field_types f on (p.fieldtypeid = f.fieldtypeid)
                                           join nodetypes t on t.nodetypeid = p.nodetypeid
                                           join nodetype_props r on t.searchdeferpropid = r.nodetypepropid
                                           join jct_nodes_props rj on (r.nodetypepropid = rj.nodetypepropid and rj.nodeid = n.nodeid)
                                           join nodes rn on rj.field1_fk = rn.nodeid
                                          where f.searchable = '1'
                                            and (lower(jnp.gestaltsearch) " + SafeLikeClause + @" )) ";
            }
            From += @"                                                              ))
                                                           )
                                                       )
                                               ) props on (props.nodetypeid = t.nodetypeid)
                               left outer join jct_nodes_props propvaljoin on (    props.nodetypepropid = propvaljoin.nodetypepropid 
                                                                               and propvaljoin.nodeid = n.nodeid)
                               left outer join jct_nodes_props propval on (propval.jctnodepropid = propvaljoin.jctnodepropid) ";



            if( CswTools.IsInteger( _SearchTerm ) )
            {
                Where += " or (n.nodeid = '" + _SearchTerm + "' and t.searchdeferpropid is null))";
            }
            else
            {
                Where += ")";
            }

            OrderBy += ", props.display_row ";

            // BZ 6008
            if( !_IncludeSystemNodes )
            {
                Where += " and n.issystem = '0' ";
            }
            //27862
            if( false == _IncludeHiddenNodes )
            {
                Where += " and n.hidden = '0' ";
            }
            Where += " and n.istemp= '0' ";

            Where += _ExtraWhereClause;

            return Select + " " + From + " " + Where + " " + OrderBy;
        } //_makeNodeSql()


        private IEnumerable<string> _makeSafeLikeClauses()
        {
            string SearchTerm = _SearchTerm.Trim();
            Collection<string> Clauses = new Collection<string>();

            // Find entries in quotes
            bool StopLoop = false;
            while( SearchTerm.Contains( "\"" ) && false == StopLoop )
            {
                int begin = SearchTerm.IndexOf( '"' );
                int length = SearchTerm.Substring( begin + 1 ).IndexOf( '"' );
                if( length > 0 )
                {
                    string QueryItem = SearchTerm.Substring( begin + 1, length ).Trim();
                    SearchTerm = SearchTerm.Remove( begin, length + 2 ).Trim();
                    if( false == string.IsNullOrEmpty( QueryItem ) )
                    {
                        Clauses.Add( CswTools.SafeSqlLikeClause( QueryItem, CswTools.SqlLikeMode.Contains, true ) );
                    }
                }
                else
                {
                    StopLoop = true;
                }
            } // while( SearchTerm.Contains( "\"" ) && Continue)

            // Split by spaces (case 27532)
            foreach( string TrimmedQueryItem in SearchTerm.Split( new char[] { ' ' } )
                                                          .Select( QueryItem => QueryItem.Trim() )
                                                          .Where( TrimmedQueryItem => false == string.IsNullOrEmpty( TrimmedQueryItem ) ) )
            {
                Clauses.Add( CswTools.SafeSqlLikeClause( TrimmedQueryItem, CswTools.SqlLikeMode.Contains, true ) );
            }
            return Clauses;
        } // makeSafeLikeClauses


    } // class CswNbtTreeLoaderFromSearchByLevel

} // namespace CswNbt
