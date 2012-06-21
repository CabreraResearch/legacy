using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
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

        public CswNbtTreeLoaderFromSearchByLevel( CswNbtResources CswNbtResources, ICswNbtUser RunAsUser, ICswNbtTree pCswNbtTree, string SearchTerm, string WhereClause, bool IncludeSystemNodes )
            : base( pCswNbtTree )
        {
            _CswNbtResources = CswNbtResources;
            _RunAsUser = RunAsUser;
            _SearchTerm = SearchTerm;
            _ExtraWhereClause = WhereClause;
            _IncludeSystemNodes = IncludeSystemNodes;
        }

        /// <summary>
        /// Returns the maximum number of properties in any Table Layout, for result limits
        /// </summary>
        private Int32 _getMaxPropertyCount()
        {
            // come back to this!
            return 10;
        }

        public override void load( bool RequireViewPermissions )
        {
            _CswNbtTree.makeRootNode( "", false, NbtViewAddChildrenSetting.None );

            _CswNbtTree.goToRoot();

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
                if( false == RequireViewPermissions || _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, ThisNodeType ) )
                {

                    // Handle property multiplexing
                    // This assumes that property rows for the same nodeid are next to one another
                    if( ThisNodeId != PriorNodeId )
                    {
                        PriorNodeId = ThisNodeId;
                        NewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( null, NodesRow, false, string.Empty, true, true, NbtViewAddChildrenSetting.None, RowCount );
                        RowCount++;
                    } // if( ThisNodeId != PriorNodeId )

                    if( NewNodeKeys != null && NodesTable.Columns.Contains( "nodetypepropid" ) )
                    {
                        Int32 ThisNTPId = CswConvert.ToInt32( NodesRow["nodetypepropid"] );
                        if( ThisNTPId != Int32.MinValue )
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
                                                         CswConvert.ToInt32( NodesRow["field1_fk"] ),
                                                         CswConvert.ToInt32( NodesRow["field1_numeric"] ) );

                            } // foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                        } // if( ThisNTPId != Int32.MinValue )
                        _CswNbtTree.goToRoot();
                    } // if( NewNodeKeys != null && NodesTable.Columns.Contains( "jctnodepropid" ) )
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

        } // load()


        private string _makeNodeSql()
        {
            string Select = @"select n.nodeid,
                                     n.nodename, 
                                     n.locked,
                                     t.iconfilename,
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

            // Filter out disabled nodetypes/object classes
            //            Where += @"where ((exists (select j.jctmoduleobjectclassid
            //                              from jct_modules_objectclass j
            //                              join modules m on j.moduleid = m.moduleid
            //                             where j.objectclassid = t.objectclassid
            //                               and m.enabled = '1')
            //                or not exists (select j.jctmoduleobjectclassid
            //                                 from jct_modules_objectclass j
            //                                 join modules m on j.moduleid = m.moduleid
            //                                where j.objectclassid = t.objectclassid) )
            //               and (exists (select j.jctmodulenodetypeid
            //                              from jct_modules_nodetypes j
            //                              join modules m on j.moduleid = m.moduleid
            //                             where j.nodetypeid = t.firstversionid
            //                               and m.enabled = '1')
            //                or not exists (select j.jctmodulenodetypeid
            //                                 from jct_modules_nodetypes j
            //                                 join modules m on j.moduleid = m.moduleid
            //                                where j.nodetypeid = t.firstversionid) )) ";
            // case 26029
            Where += "where t.enabled = '1' ";

            Select += ",lower(n.nodename) mssqlorder ";
            OrderBy = " order by lower(n.nodename)";

            OrderBy += ",n.nodeid,lower(props.propname) "; // for property multiplexing


            string SafeLikeClause = CswTools.SafeSqlLikeClause( _SearchTerm, CswTools.SqlLikeMode.Contains, true );

            // Properties
            Select += @" ,props.nodetypepropid, props.propname, props.fieldtype, propval.jctnodepropid, propval.gestalt, propval.field1, propval.field1_fk, propval.field1_numeric   ";

            From += @" left outer join (select p.nodetypeid, p.nodetypepropid, p.propname, f.fieldtype, nl.nodetypelayoutid, nl.display_row
                                                  from nodetype_props p
                                                  join field_types f on p.fieldtypeid = f.fieldtypeid
                                                  left outer join nodetype_layout nl on (nl.nodetypepropid = p.nodetypepropid and nl.layouttype = 'Table')
                                                 where (nl.nodetypelayoutid is not null 
                                                        or f.fieldtype in ('Image', 'MOL')
                                                        or (f.searchable = '1' 
                                                            and p.nodetypepropid in (select nodetypepropid 
                                                                                  from jct_nodes_props j 
                                                                                 where (lower(j.gestaltsearch) " + SafeLikeClause + @"))))
                                               ) props on (props.nodetypeid = t.nodetypeid)
                               left outer join jct_nodes_props propvaljoin on (    props.nodetypepropid = propvaljoin.nodetypepropid 
                                                                               and propvaljoin.nodeid = n.nodeid)
                               left outer join jct_nodes_props propval on (propval.jctnodepropid = propvaljoin.jctnodepropid) ";

            Where += @" and (n.nodeid in (select nodeid 
                                                   from jct_nodes_props jnp 
                                                   join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) 
                                                   join field_types f on (p.fieldtypeid = f.fieldtypeid) 
                                                  where f.searchable = '1' 
                                                    and lower(jnp.gestaltsearch) " + SafeLikeClause + @" )";

            if( CswTools.IsInteger( _SearchTerm ) )
            {
                Where += " or n.nodeid = '" + _SearchTerm + "')";
            }
            else
            {
                Where += ")";
            }

            OrderBy += ", props.display_row ";

            // BZ 6008
            if( !_IncludeSystemNodes )
                Where += " and n.issystem = '0' ";

            Where += _ExtraWhereClause;

            return Select + " " + From + " " + Where + " " + OrderBy;
        } //_makeNodeSql()

    } // class CswNbtTreeLoaderFromSearchByLevel

} // namespace CswNbt
