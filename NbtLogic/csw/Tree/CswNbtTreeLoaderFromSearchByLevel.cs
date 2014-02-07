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
        private bool _SingleNodetype;
        private bool _OnlyMergeableNodeTypes;
        private CswEnumSqlLikeMode _SearchType;
        private CswCommaDelimitedString _ExcludeNodeIds;

        public CswNbtTreeLoaderFromSearchByLevel( CswNbtResources CswNbtResources, ICswNbtUser RunAsUser, ICswNbtTree pCswNbtTree, string SearchTerm, CswEnumSqlLikeMode SearchType, string WhereClause,
                                                  bool IncludeSystemNodes, bool IncludeHiddenNodes, bool SingleNodetype, bool OnlyMergeableNodeTypes, List<string> ExcludeNodeIds = null )
            : base( pCswNbtTree )
        {
            _CswNbtResources = CswNbtResources;
            _RunAsUser = RunAsUser;
            _SearchTerm = _makeSafeSearchTerm(SearchTerm);
            _SearchType = SearchType;
            _ExtraWhereClause = WhereClause;
            _IncludeSystemNodes = IncludeSystemNodes;
            _IncludeHiddenNodes = IncludeHiddenNodes;
            _SingleNodetype = SingleNodetype;
            _OnlyMergeableNodeTypes = OnlyMergeableNodeTypes;
            if( null != ExcludeNodeIds )
            {
                _ExcludeNodeIds = new CswCommaDelimitedString( string.Join( ",", ExcludeNodeIds.ToArray() ) );
            }

        }

        public string _makeSafeSearchTerm( string SearchTerm )
        {
            string ret = string.Empty;
            foreach( char c in SearchTerm )
            {
                // case 31076 - fix subscripts from Formula -- see CswNbtNodePropFormula._parseChemicalFormula().
                if( 0x2080 <= c && c <= 0x2089 ) // subscript 0 - 9
                {
                    ret += (char) ( c - 0x2050 );
                }
                else
                {
                    ret += c;
                }
            }
            return ret;
        } // _makeSafeSearchTerm()

        public override void load( bool RequireViewPermissions, Int32 ResultsLimit = Int32.MinValue )
        {
            _CswNbtTree.makeRootNode( "", false );

            _CswNbtTree.goToRoot();

            if( string.Empty != _SearchTerm )
            {
                DataTable NodesTable = new DataTable();
                string Sql = _makeNodeSql();

                CswArbitrarySelect ResultSelect = _CswNbtResources.makeCswArbitrarySelect( "TreeLoader_select", Sql );
                CswTimer SqlTimer = new CswTimer();
                try
                {
                    NodesTable = ResultSelect.getTable();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid View", "_getNodes() attempted to run invalid SQL: " + Sql, ex );
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
                    CswPrimaryKey ThisPermGrpId = null;
                    // Verify permissions
                    // this could be a performance problem
                    CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeTypeId );
                    if( false == RequireViewPermissions || _CswNbtResources.Permit.canAnyTab( CswEnumNbtNodeTypePermission.View, ThisNodeType ) )
                    {
                        Int32 ThisNTPId = Int32.MinValue;
                        if( NodesTable.Columns.Contains( "nodetypepropid" ) )
                        {
                            ThisNTPId = CswConvert.ToInt32( NodesRow["nodetypepropid"] );
                        }
                        if( NodesTable.Columns.Contains( "permissiongroupid" ) )
                        {
                            ThisPermGrpId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodesRow["permissiongroupid"] ) );
                        }
                        bool ThisNodeFavorited = false == String.IsNullOrEmpty( NodesRow["userid"].ToString() );

                        // donb't include properties in search results to which the user has no permissions
                        if( false == RequireViewPermissions ||
                            ( _canViewNode( ThisPermGrpId, ThisNodeType ) &&
                              ( Int32.MinValue == ThisNTPId || _canViewProp( ThisNTPId, ThisPermGrpId ) ) ) )
                        {
                            // Handle property multiplexing
                            // This assumes that property rows for the same nodeid are next to one another
                            if( ThisNodeId != PriorNodeId )
                            {
                                PriorNodeId = ThisNodeId;
                                NewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( null, NodesRow, false, string.Empty, true, true, CswEnumNbtViewAddChildrenSetting.None, RowCount, Favorited: ThisNodeFavorited );
                                RowCount++;
                            } // if( ThisNodeId != PriorNodeId )

                            if( NewNodeKeys != null && ThisNTPId != Int32.MinValue )
                            {
                                foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                                {
                                    _CswNbtTree.makeNodeCurrent( NewNodeKey );
                                    _CswNbtTree.addProperty( ThisNTPId,
                                                             CswConvert.ToInt32( NodesRow["objectclasspropid"] ),
                                                             CswConvert.ToInt32( NodesRow["jctnodepropid"] ),
                                                             NodesRow["propname"].ToString(),
                                                             NodesRow["objectclasspropname"].ToString(),
                                                             NodesRow["gestalt"].ToString(),
                                                             CswConvert.ToString( NodesRow["fieldtype"] ),
                                                             CswConvert.ToString( NodesRow["field1"] ),
                                                             CswConvert.ToString( NodesRow["field2"] ),
                                                             CswConvert.ToInt32( NodesRow["field1_fk"] ),
                                                             CswConvert.ToInt32( NodesRow["field1_numeric"] ),
                                                             CswConvert.ToBoolean( NodesRow["hidden"] ),
                                                             CswConvert.ToString( NodesRow["field1_big"] ) );
                                } // foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                            } // if( NewNodeKeys != null && ThisNTPId != Int32.MinValue )
                            _CswNbtTree.goToRoot();
                        } // if( _canViewNode( ThisNodeType, ThisNodeId ) &&
                    } // if( _CswNbtResources.Permit.can( CswEnumNbtNodeTypePermission.View, ThisNodeTypeId ) )
                } // foreach(DataRow NodesRow in NodesTable.Rows)

                // case 24678 - Mark truncated results
                if( RowCount > _CswNbtResources.TreeViewResultLimit )
                {
                    // Mark root truncated
                    _CswNbtTree.goToRoot();
                    _CswNbtTree.setCurrentNodeChildrenTruncated( true );
                }

                _CswNbtTree.goToRoot();
            }
        } // load()

        private bool _canViewNode( CswPrimaryKey PermissionGroupId, CswNbtMetaDataNodeType NodeType )
        {
            bool canView = true;
            if( null != PermissionGroupId )
            {
                canView = _CswNbtResources.Permit.canNode( CswEnumNbtNodeTypePermission.View, PermissionGroupId, _CswNbtResources.CurrentNbtUser, NodeType );
            }
            return canView;
        }

        private bool _canViewProp( int NodeTypePropId, CswPrimaryKey PermissionGroupId )
        {
            CswNbtMetaDataNodeTypeProp NTProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );

            // Must have permission to at least one tab where this property appears
            Dictionary<Int32, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout> EditLayouts = NTProp.getEditLayouts();
            bool canView = EditLayouts.Values.Aggregate( false,
                                                        ( current, EditLayout ) => current || _CswNbtResources.Permit.canTab(
                                                            CswEnumNbtNodeTypePermission.View,
                                                            NTProp.getNodeType(),
                                                            _CswNbtResources.MetaData.getNodeTypeTab( EditLayout.TabId ) ) );

            #region Container Request Button Inventory Group Permission

            if( canView )
            {
                CswNbtMetaDataObjectClass ContainerClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                if( null != ContainerClass )
                {
                    CswNbtMetaDataObjectClassProp RequestProp = _CswNbtResources.MetaData.getObjectClassProp( ContainerClass.ObjectClassId, CswNbtObjClassContainer.PropertyName.Request );
                    if( NTProp.ObjectClassPropId == RequestProp.PropId )
                    {
                        if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Requesting ) )
                        {
                            canView = CswNbtObjClassContainer.canContainer( _CswNbtResources, _CswNbtResources.Actions[CswEnumNbtActionName.Submit_Request], PermissionGroupId );
                        }
                        else
                        {
                            canView = false; // case 31851
                        }
                    }
                }
            }

            #endregion

            #region Material Receive Button View Permission

            if( canView )
            {
                CswNbtMetaDataObjectClass MaterialClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
                if( null != MaterialClass )
                {
                    CswNbtMetaDataObjectClassProp RequestProp = _CswNbtResources.MetaData.getObjectClassProp( MaterialClass.ObjectClassId, CswNbtPropertySetMaterial.PropertyName.Receive );
                    if( NTProp.ObjectClassPropId == RequestProp.PropId )
                    {
                        canView = _CswNbtResources.Permit.can( CswEnumNbtActionName.Receiving );
                    }
                }
            }

            #endregion

            return canView;
        }

        private string _makeNodeSql()
        {
            string CurrentUserIdClause = string.Empty;
            if( null != _CswNbtResources.CurrentNbtUser && null != _CswNbtResources.CurrentNbtUser.UserId )
            {
                CurrentUserIdClause = " and f.userid = " + _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey;
            }
            IEnumerable<string> SafeLikeClauses = _makeSafeLikeClauses();
            string Query = string.Empty;
            if( SafeLikeClauses.Any() )
            {
                Query += @" with props as ( select p.nodetypeid, p.objectclasspropid, p.nodetypepropid, p.propname, f.fieldtype, nl.nodetypelayoutid, nl.display_row, op.propname as objectclasspropname
                                              from nodetype_props p
                                              join field_types f on p.fieldtypeid = f.fieldtypeid
                                              left outer join nodetype_layout nl on (nl.nodetypepropid = p.nodetypepropid and nl.layouttype = 'Table')
                                              left outer join object_class_props op on p.objectclasspropid = op.objectclasspropid
                                             where ( nl.nodetypelayoutid is not null 
                                                     or f.fieldtype in ('Image', 'MOL') 
                                                     or ( f.searchable = '1'
                                                          and p.nodetypepropid in (select nodetypepropid 
                                                                                     from jct_nodes_props j ";
                Query += "                                                          where ( ";
                bool first = true;
                foreach( string SafeLikeClause in SafeLikeClauses )
                {
                    if( false == first )
                    {
                        Query += "                                                     or ";
                    }
                    Query += "                                                             j.gestaltsearch " + SafeLikeClause + " ";
                    first = false;
                }
                Query += @"                                                               )
                                                                                  )
                                                       )
                                                  )
                                          ),

                                 jctnd as ( select jnp.nodeid, jnp.gestaltsearch
                                              from jct_nodes_props jnp
                                              join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                              join nodetypes t on (p.nodetypeid = t.nodetypeid)
                                              join field_types f on (p.fieldtypeid = f.fieldtypeid)
                                              where f.searchable = '1'";
                if( false == _SingleNodetype )
                {
                    Query += @"                   and t.searchdeferpropid is null";
                    Query += @"                   and t.searchable = '1'";
                }
                Query += @"                UNION
                                            select rn.nodeid, jnp.gestaltsearch
                                              from nodes n
                                              join jct_nodes_props jnp on jnp.nodeid = n.nodeid
                                              join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                              join field_types f on (p.fieldtypeid = f.fieldtypeid)
                                              join nodetypes t on t.nodetypeid = p.nodetypeid
                                              join nodetype_props r on t.searchdeferpropid = r.nodetypepropid
                                              join jct_nodes_props rj on (r.nodetypepropid = rj.nodetypepropid and rj.nodeid = n.nodeid)
                                              join nodes rn on rj.field1_fk = rn.nodeid
                                             where f.searchable = '1'
                                          ),

                                pval as (select j.nodeid, op.propname, j.field1_fk
                                           from object_class_props op
                                           join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                           join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                          where op.propname in ('Location', 'Inventory Group', 'Report Group', 'Mail Report Group')
                                        ),
                                permgrp as ( " + _makePermissionGroupSQL() + @" ),
                                  srch as ( select n.nodeid,
                                                   n.nodename,
                                                   n.locked,
                                                   nvl(n.iconfilename, t.iconfilename) iconfilename,
                                                   t.nodetypename,
                                                   t.nametemplate,
                                                   t.nodetypeid,
                                                   o.objectclass,
                                                   o.objectclassid,
                                                   lower(n.nodename) mssqlorder,
                                                   props.nodetypepropid,
                                                   props.objectclasspropid,
                                                   props.propname,
                                                   props.objectclasspropname,
                                                   props.fieldtype,
                                                   props.display_row,
                                                   propval.jctnodepropid,
                                                   propval.gestaltsearch as gestalt,
                                                   propval.field1,
                                                   propval.field2,
                                                   propval.field1_fk,
                                                   propval.field1_numeric,
                                                   propval.hidden,
                                                   propval.field1_big,
                                                   i.permissiongroupid,
                                                   f.userid
                                              from nodes n
                                              join nodetypes t on (n.nodetypeid = t.nodetypeid)
                                              join object_class o on (t.objectclassid = o.objectclassid)
                                              left outer join favorites f on n.nodeid = f.itemid " + CurrentUserIdClause + @"
                                              left outer join permgrp i on (n.nodeid = i.nodeid)
                                              left outer join props on (props.nodetypeid = t.nodetypeid)
                                              left outer join jct_nodes_props propvaljoin on (props.nodetypepropid = propvaljoin.nodetypepropid and propvaljoin.nodeid = n.nodeid)
                                              left outer join jct_nodes_props propval on (propval.jctnodepropid = propvaljoin.jctnodepropid)
                                             where n.istemp = '0' ";
                //If we have access to disabled module MetaData, we should have access to their Nodes as well
                if( _CswNbtResources.MetaData.ExcludeDisabledModules )
                {
                    Query += "                 and t.enabled = '1' ";
                }
                // case 31056
                if( _OnlyMergeableNodeTypes )
                {
                    Query += "                 and t.mergeable = '1' ";
                }
                // BZ 6008
                if( !_IncludeSystemNodes )
                {
                    Query += "                 and n.issystem = '0' ";
                }
                //case 27862
                if( false == _IncludeHiddenNodes )
                {
                    Query += "                 and n.hidden = '0' ";
                }

                Query += "                     and ( ";
                first = true;
                foreach( string SafeLikeClause in SafeLikeClauses )
                {
                    if( false == first )
                    {
                        Query += "                     and ";
                    }
                    Query += "                             n.nodeid in (select nodeid from jctnd where gestaltsearch " + SafeLikeClause;
                    if( CswTools.IsInteger( _SearchTerm ) )
                    {
                        Query += @"                                      union select " + _SearchTerm + " from dual";
                    }
                    Query += "                                         ) ";
                    first = false;
                }
                Query += @"                        ) ";
                if( false == _SingleNodetype )
                {
                    Query += @"                    and t.searchable = '1'";
                    Query += @"                    and t.searchdeferpropid is null";
                }
                Query += @"                    and ( n.searchable = '1' or ( props.fieldtype = 'Barcode' and propval.field1 = '" + CswTools.SafeSqlParam( _SearchTerm ) + @"' ) )";
                Query += _ExtraWhereClause;
                // Case 31351: Exclude specific nodes
                if( _ExcludeNodeIds.Count > 0 )
                {
                    Query += "                     and n.nodeid not in (" + _ExcludeNodeIds.ToString() + ") ";
                }
                Query += @"               )
                
                                   select *
                                     from srch";

                // Handle result limits by looking at unique nodeids in the results
                Query += @"         where srch.nodeid in ( select nodeid 
                                                             from ( select nodeid, rownum as rnum
                                                                      from (select distinct nodeid from srch))
                                                            where rnum <= " + _CswNbtResources.TreeViewResultLimit + @")
                                    order by srch.userid, lower(srch.nodename), srch.nodeid, lower(srch.propname), srch.display_row ";

            }
            return Query;
        } //_makeNodeSql()

        //If we add any more PermissionSet ObjectClasses, this will need to be updated.
        //This is not ideal, but better than stuffing query-specific SQL in the ObjectClasses themselves,
        //and more performant than grabbing the permission group nodes in a separate query.
        private string _makePermissionGroupSQL()
        {
            string SQL = @"select n.nodeid, ivgval.field1_fk permissiongroupid
                             from nodes n
                             join pval locval on (locval.nodeid = n.nodeid and locval.propname = 'Location')
                                and n.nodetypeid in (select nodetypeid from nodetypes where objectclassid in
                                    (select objectclassid from object_class where objectclass = '" + CswEnumNbtObjectClass.ContainerClass + @"') )
                             join pval ivgval on (ivgval.nodeid = locval.field1_fk and ivgval.propname = 'Inventory Group')
                         union
                           select n.nodeid, rgval.field1_fk permissiongroupid
                             from nodes n
                             join pval rgval on (rgval.nodeid = n.nodeid and rgval.propname = 'Report Group')
                         union
                           select n.nodeid, mrgval.field1_fk permissiongroupid
                             from nodes n
                             join pval mrgval on (mrgval.nodeid = n.nodeid and mrgval.propname = 'Mail Report Group')";
            return SQL;
        }

        private IEnumerable<string> _makeSafeLikeClauses()
        {
            string SearchTerm = _SearchTerm.Trim();
            Collection<string> Clauses = new Collection<string>();

            if( _SearchType == CswEnumSqlLikeMode.Contains )
            {
                // For Contains, we treat each word individually, unless enclosed in quotes

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
                            Clauses.Add( CswTools.SafeSqlLikeClause( QueryItem.ToLower(), CswEnumSqlLikeMode.Contains, true ) );
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
                    Clauses.Add( CswTools.SafeSqlLikeClause( TrimmedQueryItem.ToLower(), CswEnumSqlLikeMode.Contains, true ) );
                }
            } // if( _SearchType == CswEnumSqlLikeMode.Contains )
            else
            {
                Clauses.Add( CswTools.SafeSqlLikeClause( SearchTerm.ToLower(), _SearchType, true ) );
            }
            return Clauses;
        } // makeSafeLikeClauses


    } // class CswNbtTreeLoaderFromSearchByLevel

} // namespace CswNbt
