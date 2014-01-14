using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{
    public class CswNbtTreeLoaderFromXmlViewByLevel : CswNbtTreeLoader
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtView _View;
        private ICswNbtUser _RunAsUser;
        private bool _IncludeSystemNodes;
        private bool _IncludeHiddenNodes;
        private bool _IncludeTempNodes;

        public CswNbtTreeLoaderFromXmlViewByLevel( CswNbtResources CswNbtResources, ICswNbtUser RunAsUser, ICswNbtTree pCswNbtTree, CswNbtView View, bool IncludeSystemNodes, bool IncludeHiddenNodes, bool IncludeTempNodes )
            : base( pCswNbtTree )
        {
            _CswNbtResources = CswNbtResources;
            _RunAsUser = RunAsUser;
            _View = View;
            _IncludeSystemNodes = IncludeSystemNodes;
            _IncludeHiddenNodes = IncludeHiddenNodes;
            _IncludeTempNodes = IncludeTempNodes || View.IncludeTempNodes;
        }

        public override void load( bool RequireViewPermissions, Int32 ResultsLimit = Int32.MinValue )
        {
            _CswNbtTree.makeRootNode( _View.Root );

            _CswNbtTree.goToRoot();
            foreach( CswNbtViewRelationship Relationship in _View.Root.ChildRelationships )
            {
                bool GroupBySiblings = _View.GroupBySiblings && _View.Root.ChildRelationships.Count > 1;
                loadRelationshipRecursive( Relationship, RequireViewPermissions, GroupBySiblings, ResultsLimit: ResultsLimit );
            }
            _CswNbtTree.goToRoot();

        } // load()

        private void loadRelationshipRecursive( CswNbtViewRelationship Relationship, bool RequireViewPermissions, bool GroupBySiblings,
            IEnumerable<CswPrimaryKey> ParentNodeIds = null, Int32 ResultsLimit = Int32.MinValue )
        {
            CswNbtNodeKey PriorCurrentNodeKey = _CswNbtTree.getNodeKeyForCurrentPosition();

            // Nodes and Properties
            DataTable NodesTable = new DataTable();
            CswArbitrarySelect ResultSelect = _makeNodeSql( Relationship, ParentNodeIds );

            Int32 thisResultLimit = _CswNbtResources.TreeViewResultLimit;
            if( ResultsLimit != Int32.MinValue )
            {
                thisResultLimit = ResultsLimit;
            }

            if( Relationship.Properties.Count > 0 )
            {
                thisResultLimit = thisResultLimit * Relationship.Properties.Count;
            }

            CswTimer SqlTimer = new CswTimer();
            try
            {
                NodesTable = ResultSelect.getTable( 0, thisResultLimit, false );
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid View", "_getNodes() attempted to run invalid SQL: " + ResultSelect.Sql, ex );
            }
            _CswNbtResources.CswLogger.TreeLoaderSQLTime += SqlTimer.ElapsedDurationInMilliseconds;

            if( SqlTimer.ElapsedDurationInSeconds > 2 )
            {
                _CswNbtResources.logMessage( "Tree View SQL required longer than 2 seconds to run: " + ResultSelect.Sql );
            }

            Int32 PriorNodeId = Int32.MinValue;
            Int32 PriorParentNodeId = Int32.MinValue;
            Collection<CswPrimaryKey> KeysThisLevel = new Collection<CswPrimaryKey>();
            Collection<CswNbtNodeKey> NewNodeKeys = new Collection<CswNbtNodeKey>();
            Collection<CswNbtNodeKey> ParentNodeKeys = null;
            foreach( DataRow NodesRow in NodesTable.Rows )
            {
                Int32 ThisNodeId = CswConvert.ToInt32( NodesRow["nodeid"] );
                CswPrimaryKey ThisNodePk = new CswPrimaryKey( "nodes", ThisNodeId );
                Int32 ThisParentNodeId = Int32.MinValue;
                if( NodesTable.Columns.Contains( "parentnodeid" ) )
                {
                    ThisParentNodeId = CswConvert.ToInt32( NodesRow["parentnodeid"] );
                }
                Int32 ThisNodeTypeId = CswConvert.ToInt32( NodesRow["nodetypeid"] );
                bool ThisNodeFavorited = false == String.IsNullOrEmpty( NodesRow["userid"].ToString() );

                // Verify permissions
                // this could be a performance problem
                CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeTypeId );
                if( false == RequireViewPermissions ||
                  ( _CswNbtResources.Permit.canAnyTab( CswEnumNbtNodeTypePermission.View, ThisNodeType, _RunAsUser ) &&
                    _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.View, ThisNodeType, ThisNodePk, _RunAsUser ) ) )
                {
                    // Handle property multiplexing
                    // This assumes that property rows for the same nodeid are next to one another
                    if( ThisNodeId != PriorNodeId || ThisParentNodeId != PriorParentNodeId )
                    {
                        PriorNodeId = ThisNodeId;
                        PriorParentNodeId = ThisParentNodeId;
                        NewNodeKeys = new Collection<CswNbtNodeKey>();
                        Collection<CswNbtNodeKey> ThisNewNodeKeys = new Collection<CswNbtNodeKey>();
                        ParentNodeKeys = new Collection<CswNbtNodeKey>();

                        // Handle ResultMode.Disabled on filters
                        bool Included = true;
                        foreach( DataColumn Column in NodesRow.Table.Columns )
                        {
                            if( Column.ColumnName.StartsWith( "INCLUDED" ) )
                            {
                                string Conjunction = Column.ColumnName.Substring( "INCLUDED".Length );
                                if( Conjunction.StartsWith( "OR" ) )
                                {
                                    Included = Included || CswConvert.ToBoolean( NodesRow[Column] );
                                }
                                else
                                {
                                    Included = Included && CswConvert.ToBoolean( NodesRow[Column] );
                                }
                            }
                        }
                        bool UseGroupBy = Relationship.GroupByPropId != Int32.MinValue;
                        string GroupName = string.Empty;
                        if( UseGroupBy )
                        {
                            GroupName = CswConvert.ToString( NodesRow["groupname"] );
                            if( GroupName == string.Empty )
                            {
                                GroupName = "[blank]";
                            }
                        }
                        else if( GroupBySiblings )
                        {
                            UseGroupBy = true;
                            GroupName = CswConvert.ToString( NodesRow["nodetypename"] );
                        }

                        if( NodesTable.Columns.Contains( "parentnodeid" ) )
                        {
                            CswPrimaryKey ParentNodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodesRow["parentnodeid"] ) );

                            // We can't use getNodeKeyByNodeId, because there may be more instances of this node at different places in the tree
                            //ParentNodeKey = _CswNbtTree.getNodeKeyByNodeId( ParentNodeId );
                            ParentNodeKeys = _CswNbtTree.getNodeKeysByNodeIdAndViewNode( ParentNodePk, Relationship.Parent );

                            if( ParentNodeKeys.Count == 0 )
                            {
                                // If the parent isn't in the tree, don't add the child
                                PriorNodeId = Int32.MinValue;   // case 24788
                            }
                            else
                            {
                                foreach( CswNbtNodeKey ParentNodeKey in ParentNodeKeys )
                                {
                                    _CswNbtTree.makeNodeCurrent( ParentNodeKey );
                                    Int32 ChildCount = _CswNbtTree.getChildNodeCount();
                                    ThisNewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( ParentNodeKey, NodesRow, UseGroupBy, GroupName, Relationship, ChildCount + 1, Included, ThisNodeFavorited );
                                    foreach( CswNbtNodeKey ThisNewNodeKey in ThisNewNodeKeys )
                                    {
                                        NewNodeKeys.Add( ThisNewNodeKey );
                                        KeysThisLevel.Add( ThisNewNodeKey.NodeId );
                                    }
                                } // foreach( CswNbtNodeKey ParentNodeKey in ParentNodeKeys )
                            }

                        } // if( NodesTable.Columns.Contains( "parentnodeid" ) )
                        else
                        {
                            Int32 ChildCount = _CswNbtTree.getChildNodeCount();
                            ThisNewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( null, NodesRow, UseGroupBy, GroupName, Relationship, ChildCount + 1, Included, ThisNodeFavorited );
                            foreach( CswNbtNodeKey ThisNewNodeKey in ThisNewNodeKeys )
                            {
                                NewNodeKeys.Add( ThisNewNodeKey );
                                KeysThisLevel.Add( ThisNewNodeKey.NodeId );
                            }
                        } // if( AddChild )
                    } // if( ThisNodeId != PriorNodeId )

                    // This assumes that property rows for the same nodeid are next to one another
                    // It also assumes that loadNodeAsChildFromRow() made the node current
                    if( NewNodeKeys.Count > 0 && NodesTable.Columns.Contains( "jctnodepropid" ) )
                    {
                        foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                        {
                            _CswNbtTree.makeNodeCurrent( NewNodeKey );
                            _CswNbtTree.addProperty( CswConvert.ToInt32( NodesRow["nodetypepropid"] ),
                                                     CswConvert.ToInt32( NodesRow["objectclasspropid"] ),
                                                     CswConvert.ToInt32( NodesRow["jctnodepropid"] ),
                                                     NodesRow["propname"].ToString(),
                                                     NodesRow["objectclasspropname"].ToString(),
                                                     NodesRow["gestalt"].ToString(),
                                                     CswConvert.ToString( NodesRow["fieldtype"] ),
                                                     CswConvert.ToString( NodesRow["field1"] ),
                                                     CswConvert.ToString( NodesRow["field2"] ),
                                                     CswConvert.ToInt32( NodesRow["field1_fk"] ),
                                                     CswConvert.ToDouble( NodesRow["field1_numeric"] ),
                                                     CswConvert.ToBoolean( NodesRow["hidden"] ),
                                                     CswConvert.ToString( NodesRow["field1_big"] ) );

                        } // foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                        if( ParentNodeKeys.Count > 0 )
                        {
                            _CswNbtTree.makeNodeCurrent( ParentNodeKeys[0] );
                        }
                        else
                        {
                            _CswNbtTree.goToRoot();
                        }
                    } // if( NewNodeKeys != null && NodesTable.Columns.Contains( "jctnodepropid" ) )
                } // if( false == RequireViewPermissions || _CswNbtResources.Permit.can( CswEnumNbtNodeTypePermission.View, ThisNodeType, true, null, _RunAsUser ) )
            } // foreach(DataRow NodesRow in NodesTable.Rows)

            if( KeysThisLevel.Count > 0 ) // only recurse if there are results
            {
                // Recurse
                foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
                {
                    bool ContinueGroupingBySibling = ( _View.GroupBySiblings && Relationship.ChildRelationships.Count > 1 );
                    loadRelationshipRecursive( ChildRelationship, RequireViewPermissions, ContinueGroupingBySibling, KeysThisLevel );
                }

                // case 24678 - Mark truncated results
                if( KeysThisLevel.Count == thisResultLimit )
                {
                    //if( ParentNodeKeys != null && ParentNodeKeys.Count > 0 )
                    //{
                    //    foreach( CswNbtNodeKey ParentNodeKey in ParentNodeKeys )
                    //    {
                    //        // assume truncation on every potential parent
                    //        _CswNbtTree.makeNodeCurrent( ParentNodeKey );
                    //        _CswNbtTree.goToParentNode();
                    //        for( Int32 c = 0; c < _CswNbtTree.getChildNodeCount(); c++ )
                    //        {
                    //            _CswNbtTree.goToNthChild( c );
                    //            _CswNbtTree.setCurrentNodeChildrenTruncated( true );
                    //            _CswNbtTree.goToParentNode();
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    _CswNbtTree.goToRoot();
                    _CswNbtTree.setCurrentNodeChildrenTruncated( true );
                    //}
                } // if( NodesTable.Rows.Count == thisResultLimit )
            } // if( NodesTable.Rows.Count > 0 )

            _CswNbtTree.makeNodeCurrent( PriorCurrentNodeKey );

        } // loadRelationshipRecursive()


        private CswArbitrarySelect _makeNodeSql( CswNbtViewRelationship Relationship, IEnumerable<CswPrimaryKey> ParentNodeIds = null )
        {
            string CurrentUserIdClause = string.Empty;
            if( null != _CswNbtResources.CurrentNbtUser && null != _CswNbtResources.CurrentNbtUser.UserId )
            {
                CurrentUserIdClause = " and f.userid = " + _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey;
            }
            CswCommaDelimitedString With = new CswCommaDelimitedString();
            string Select = @"select n.nodeid,
                                     n.nodename, 
                                     n.locked,
                                     nvl(n.iconfilename, t.iconfilename) iconfilename,
                                     t.nodetypename,
                                     t.nametemplate,
                                     t.nodetypeid,
                                     o.objectclass,
                                     o.objectclassid,
                                     f.userid";
            string From = @"from nodes n
                            left join favorites f on n.nodeid = f.itemid " + CurrentUserIdClause + @"
                            join nodetypes t on (n.nodetypeid = t.nodetypeid)
                            join object_class o on (t.objectclassid = o.objectclassid) ";
            string OrderBy = string.Empty;

            // Nodetype/Object Class filter
            string Where;
            if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
            {
                Where = " where (t.firstversionid = " + Relationship.SecondId + ") ";
            }
            else if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
            {
                Where = " where (o.objectclassid = " + Relationship.SecondId + ") ";
            }
            else if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
            {
                From += @" join jct_propertyset_objectclass jpo on (o.objectclassid = jpo.objectclassid) 
                           join property_set ps on (jpo.propertysetid = ps.propertysetid) ";
                Where = " where (ps.propertysetid = " + Relationship.SecondId + ") ";
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid View", "CswNbtTreeLoaderFromXmlViewByLevel got a relationship with an unrecognized SecondType: " + Relationship.SecondType.ToString() );
            }

            //If we have access to disabled module MetaData, we should have access to their Nodes as well
            if( _CswNbtResources.MetaData.ExcludeDisabledModules )
            {
                // case 26029
                Where += " and t.enabled = '1' ";
            }

            // Parent Node
            if( Relationship.PropId != Int32.MinValue && null != ParentNodeIds )
            {
                bool first = true;
                string parentsWith = "parents as (";
                foreach( Int32 ParentNodeId in ParentNodeIds.Select( key => key.PrimaryKey ) )
                {
                    if( first )
                    {
                        first = false;
                    }
                    else
                    {
                        parentsWith += " union ";
                    }
                    parentsWith += "select " + ParentNodeId + " nodeid from dual ";
                }
                parentsWith += ")";
                With.Add( parentsWith );

                Select += ",parent.parentnodeid ";

                if( Relationship.PropOwner == CswEnumNbtViewPropOwnerType.First )
                {
                    From += @"            join (select jnp.nodeid parentnodeid, jnp.field1_fk thisnodeid
                                                  from jct_nodes_props jnp
                                                  join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) ";
                    if( Relationship.PropType == CswEnumNbtViewPropIdType.NodeTypePropId )
                    {
                        From += @"               where p.firstpropversionid = " + Relationship.PropId;
                    }
                    else
                    {
                        From += @"                join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                                 where op.objectclasspropid = " + Relationship.PropId;
                    }
                    From += @"                ) parent on (parent.thisnodeid = n.nodeid)";
                }
                else
                {
                    From += @"          join (select jnp.nodeid thisnodeid, jnp.field1_fk parentnodeid
                                                from jct_nodes_props jnp
                                                join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) ";
                    if( Relationship.PropType == CswEnumNbtViewPropIdType.NodeTypePropId )
                    {
                        From += @"             where p.firstpropversionid = " + Relationship.PropId;
                    }
                    else
                    {
                        From += @"              join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                               where op.objectclasspropid = " + Relationship.PropId;
                    }
                    From += @"        ) parent on (parent.thisnodeid = n.nodeid)";
                }
                From += " join parents on (parent.parentnodeid = parents.nodeid) ";
            } // if( Relationship.PropId != Int32.MinValue )

            CswCommaDelimitedString OrderByProps = new CswCommaDelimitedString();
            OrderByProps.Add( "f.userid" );
            // Grouping
            if( Relationship.GroupByPropId != Int32.MinValue )
            {
                CswNbtSubField GroupBySubField = _getDefaultSubFieldForProperty( Relationship.GroupByPropType, Relationship.GroupByPropId );
                Select += " ,g." + GroupBySubField.Column + " groupname";
                OrderByProps.Add( "g." + GroupBySubField.Column );
                if( Relationship.GroupByPropType == CswEnumNbtViewPropIdType.ObjectClassPropId )
                {
                    From += @" left outer join (select j.nodeid, " + GroupBySubField.Column + @" 
                                                    from jct_nodes_props j 
                                                    join nodetype_props p on j.nodetypepropid = p.nodetypepropid 
                                                    where p.objectclasspropid = " + Relationship.GroupByPropId.ToString() + @") g
                                on (g.nodeid = n.nodeid)";
                }
                else
                {
                    From += @" left outer join (select j.nodeid, " + GroupBySubField.Column + @" 
                                                    from jct_nodes_props j 
                                                    where j.nodetypepropid = " + Relationship.GroupByPropId.ToString() + @") g 
                                on (g.nodeid = n.nodeid)";
                }
            } // if( Relationship.GroupByPropId != Int32.MinValue )

            // Handle sort order
            Int32 sortAlias = 0;
            String OrderByString = String.Empty;
            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                if( Prop.SortBy )
                {
                    // Case 10530
                    sortAlias++;
                    if( null != Prop.MetaDataProp )
                    {
                        CswEnumNbtPropColumn SubFieldColumn = Prop.MetaDataProp.getFieldTypeRule().SubFields.Default.Column;
                        string aliasName = "mssqlorder" + sortAlias;
                        Select += ",(select ";
                        if( SubFieldColumn == CswEnumNbtPropColumn.Field1_Numeric ||
                                SubFieldColumn == CswEnumNbtPropColumn.Field1_Date ||
                                SubFieldColumn == CswEnumNbtPropColumn.Field2_Numeric ||
                                SubFieldColumn == CswEnumNbtPropColumn.Field2_Date )
                        {
                            Select += SubFieldColumn.ToString();
                        }
                        else
                        {
                            Select += "lower(" + SubFieldColumn.ToString() + ")";
                        }
                        Select += " from jct_nodes_props where nodeid = n.nodeid and ";
                        if( Prop.Type == CswEnumNbtViewPropType.NodeTypePropId )
                        {
                            Select += "nodetypepropid = " + Prop.NodeTypePropId;
                        }
                        else
                        {
                            Select += "nodetypepropid in (select nodetypepropid from nodetype_props where objectclasspropid = " + Prop.ObjectClassPropId + ")";
                        }
                        Select += ") as " + aliasName;

                        // Case 10533
                        if( SubFieldColumn == CswEnumNbtPropColumn.Gestalt ||
                                SubFieldColumn == CswEnumNbtPropColumn.ClobData )
                        {
                            OrderByString = "lower(to_char(" + aliasName + "))";
                        }
                        else if( SubFieldColumn == CswEnumNbtPropColumn.Field1_Numeric ||
                                    SubFieldColumn == CswEnumNbtPropColumn.Field1_Date ||
                                    SubFieldColumn == CswEnumNbtPropColumn.Field2_Numeric ||
                                    SubFieldColumn == CswEnumNbtPropColumn.Field2_Date )
                        {
                            OrderByString = aliasName;
                        }
                        else
                        {
                            OrderByString = "lower(" + aliasName + ")";
                        }

                        if( Prop.SortMethod == CswEnumNbtViewPropertySortMethod.Descending )
                        {
                            OrderByString += " desc";
                        }

                        Int32 OrderByOrder = Prop.Order;
                        if( OrderByOrder != 0 && ( OrderByProps.Count <= OrderByOrder || OrderByOrder < 0 ) )
                        {
                            if( OrderByProps.Count == 0 )
                            {
                                OrderByOrder = 0;
                            }
                            else
                            {
                                OrderByOrder = OrderByProps.Count - 1;
                            }
                        }

                        OrderByProps.Insert( OrderByOrder, OrderByString );
                    }
                } // if( Prop.SortBy )
            } // foreach( CswNbtViewProperty Prop in Relationship.Properties )

            // case 29193: always fall back on name sort
            sortAlias++;
            Select += ",lower(n.nodename) mssqlorder" + sortAlias;
            OrderByProps.Add( "lower(n.nodename)" );

            OrderBy = " order by " + OrderByProps.ToString() + " ";
            // for property multiplexing
            OrderBy += ",n.nodeid";
            if( Relationship.PropId != Int32.MinValue && null != ParentNodeIds )
            {
                OrderBy += ",parent.parentnodeid ";
            }

            // Properties for Select
            if( Relationship.Properties.Count > 0 )
            {
                CswCommaDelimitedString NTPropsInClause = new CswCommaDelimitedString( 0, "'" );
                CswCommaDelimitedString OCPropsInClause = new CswCommaDelimitedString( 0, "'" );
                foreach( CswNbtViewProperty Prop in Relationship.Properties )
                {
                    if( Prop.Type == CswEnumNbtViewPropType.NodeTypePropId && Prop.NodeTypePropId != Int32.MinValue )
                    {
                        NTPropsInClause.Add( Prop.NodeTypePropId.ToString() );
                    }
                    else if( Prop.ObjectClassPropId != Int32.MinValue )
                    {
                        OCPropsInClause.Add( Prop.ObjectClassPropId.ToString() );
                    }
                }

                // This will multiplex the results by the number of properties!
                if( NTPropsInClause.Count > 0 || OCPropsInClause.Count > 0 )
                {
                    // Properties
                    // We match on propname because that's how the view editor works.
                    Select += @" ,props.nodetypepropid, props.objectclasspropid, props.propname, props.objectclasspropname, props.fieldtype ";

                    string propsWith = "props as (";
                    if( NTPropsInClause.Count > 0 )
                    {
                        propsWith += @"  select p2.nodetypeid, p2.nodetypepropid, p2.objectclasspropid, p2.propname, f.fieldtype, ocp2.propname as objectclasspropname
                                from nodetype_props p1
                                join nodetype_props p2 on (p2.firstpropversionid = p1.firstpropversionid or p1.propname = p2.propname)
                                left outer join object_class_props ocp2 on p2.objectclasspropid = ocp2.objectclasspropid
                                join field_types f on f.fieldtypeid = p2.fieldtypeid
                                where p1.nodetypepropid in (" + NTPropsInClause.ToString() + @")";
                        if( OCPropsInClause.Count > 0 )
                        {
                            propsWith += @" UNION ALL ";
                        }
                    }
                    if( OCPropsInClause.Count > 0 )
                    {
                        propsWith += @" select ntp.nodetypeid, ntp.nodetypepropid, ntp.objectclasspropid, ntp.propname, f.fieldtype, op.propname as objectclasspropname
                                from object_class_props op
                                join nodetype_props ntp on (ntp.objectclasspropid = op.objectclasspropid or ntp.propname = op.propname)
                                join field_types f on f.fieldtypeid = ntp.fieldtypeid
                                where op.objectclasspropid in (" + OCPropsInClause.ToString() + @")";
                    }
                    propsWith += @"   )";
                    With.Add( propsWith );

                    From += " left outer join props on (props.nodetypeid = t.nodetypeid)";  // intentional multiplexing

                    // Property Values
                    Select += @" ,propval.jctnodepropid, propval.gestalt, propval.field1, propval.field2, propval.field1_fk, propval.field1_numeric, propval.hidden, propval.field1_big ";
                    From += @"  left outer join jct_nodes_props propvaljoin on (props.nodetypepropid = propvaljoin.nodetypepropid and propvaljoin.nodeid = n.nodeid) ";  // better performance from indexes if we do this first
                    From += @"  left outer join jct_nodes_props propval on (propval.jctnodepropid = propvaljoin.jctnodepropid) ";

                } // if( NTPropsInClause.Count > 0 || OCPropsInClause.Count > 0 )
            } // if(Relationship.Properties.Count > 0)

            // Property Filters
            Int32 FilterCount = 0;
            string FilterWhere = string.Empty;
            Dictionary<string, string> FilterParameters = new Dictionary<string, string>();

            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
                {
                    if( Filter.FilterMode == CswEnumNbtFilterMode.Null ||
                        Filter.FilterMode == CswEnumNbtFilterMode.NotNull ||
                        Filter.Value != string.Empty )
                    {
                        FilterCount += 1;
                        ICswNbtFieldTypeRule FilterFieldTypeRule = _CswNbtResources.MetaData.getFieldTypeRule( Prop.FieldType );
                        string FilterValue = string.Empty;
                        if( null != FilterFieldTypeRule )
                        {
                            FilterValue = FilterFieldTypeRule.renderViewPropFilter( _RunAsUser, Filter, FilterParameters, FilterCount );
                        }
                        if( false == string.IsNullOrEmpty( FilterValue ) )
                        {
                            CswNbtSubField FilterSubField = FilterFieldTypeRule.SubFields[Filter.SubfieldName];

                            //if( FilterSubField.RelationalTable == string.Empty )
                            //{
                                string FilterClause = string.Empty;// @"select z.nodeid, '1' as included from nodes z where ";
                                if( Filter.FilterMode == CswEnumNbtFilterMode.Null )
                                {
                                    FilterClause += @"select z.nodeid, '1' as included 
                                                        from nodes z ";

                                    if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                                    {
                                        FilterClause += @" join nodetypes t on z.nodetypeid = t.nodetypeid
                                                          where (t.firstversionid = :filt" + FilterCount + "relid) ";

                                    }
                                    else if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                                    {
                                        FilterClause += @" join nodetypes t on z.nodetypeid = t.nodetypeid
                                                           join object_class o on t.objectclassid = o.objectclassid
                                                          where (o.objectclassid = :filt" + FilterCount + "relid) ";
                                    }
                                    else if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                                    {
                                        FilterClause += @" join nodetypes t on z.nodetypeid = t.nodetypeid
                                                           join object_class o on t.objectclassid = o.objectclassid
                                                           join jct_propertyset_objectclass jpo on (o.objectclassid = jpo.objectclassid) 
                                                           join property_set ps on (jpo.propertysetid = ps.propertysetid) 
                                                          where (ps.propertysetid = :filt" + FilterCount + "relid) ";
                                    }
                                    FilterClause += @"      and (z.nodeid not in (
                                                           select jnp.nodeid
                                                             from jct_nodes_props jnp
                                                             join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) ";

                                    if( Prop.Type == CswEnumNbtViewPropType.NodeTypePropId )
                                    {
                                        FilterClause += @"  where (lower(p.propname) = :filt" + FilterCount + @"ntpname)) ";
                                    }
                                    else
                                    {
                                        FilterClause += @"   join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                                            where op.objectclasspropid = :filt" + FilterCount + @"ocpid)";
                                    }
                                    FilterClause += @" or z.nodeid in (select n.nodeid from nodes n ";

                                }
                                else
                                {
                                    FilterClause += @"select n.nodeid, '1' as included from nodes n ";
                                }

                                if( Prop.Type == CswEnumNbtViewPropType.NodeTypePropId )
                                {
                                    FilterClause += @"            join nodetype_props p on (lower(p.propname) = :filt" + FilterCount + @"ntpname) ";
                                    FilterParameters.Add( "filt" + FilterCount + "ntpname", CswTools.SafeSqlParam( Prop.NodeTypeProp.PropName.ToLower() ) );
                                }
                                else
                                {
                                    FilterClause += @"            join object_class_props op on (op.objectclasspropid = :filt" + FilterCount + @"ocpid)
                                                                  join nodetype_props p on (p.objectclasspropid = op.objectclasspropid) ";
                                    FilterParameters.Add( "filt" + FilterCount + "ocpid", Prop.ObjectClassPropId.ToString() );
                                }
                                FilterClause += @"                join jct_nodes_props jnp on (jnp.nodeid = n.nodeid and jnp.nodetypepropid = p.nodetypepropid) ";

                                FilterParameters.Add( "filt" + FilterCount + "relid", Relationship.SecondId.ToString() );
                                if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                                {
                                    FilterClause += @" join nodetypes t on n.nodetypeid = t.nodetypeid
                                                        where (t.firstversionid = :filt" + FilterCount + "relid) ";
                                }
                                else if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                                {
                                    FilterClause += @" join nodetypes t on n.nodetypeid = t.nodetypeid
                                                        join object_class o on t.objectclassid = o.objectclassid
                                                        where (o.objectclassid = :filt" + FilterCount + "relid) ";
                                }
                                else if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                                {
                                    FilterClause += @" join nodetypes t on n.nodetypeid = t.nodetypeid
                                                        join object_class o on t.objectclassid = o.objectclassid
                                                        join jct_propertyset_objectclass jpo on (o.objectclassid = jpo.objectclassid) 
                                                        join property_set ps on (jpo.propertysetid = ps.propertysetid) 
                                                        where (ps.propertysetid = :filt" + FilterCount + "relid) ";
                                }

                                FilterClause += @" and " + FilterValue + @"";
                                if( Filter.FilterMode == CswEnumNbtFilterMode.Null )
                                {
                                    FilterClause += "))";
                                }

                                With.Add( "filt" + FilterCount.ToString() + " as (" + FilterClause + ")" );

                                From += "left outer join filt" + FilterCount.ToString() + " f" + FilterCount.ToString() + " on (f" + FilterCount.ToString() + ".nodeid = n.nodeid)";
                                if( Filter.ResultMode == CswEnumNbtFilterResultMode.Disabled )
                                {
                                    Select += ",f" + FilterCount.ToString() + ".included as included" + Filter.Conjunction.ToString() + FilterCount.ToString();
                                }
                                if( Filter.ResultMode == CswEnumNbtFilterResultMode.Hide )
                                {
                                    if( FilterWhere != string.Empty )
                                    {
                                        FilterWhere += Filter.Conjunction.ToString().ToLower();
                                    }
                                    FilterWhere += " f" + FilterCount.ToString() + ".included = '1' ";
                                }

                            //} // if( FilterSubField.RelationalTable == string.empty )
                            //else if( false == string.IsNullOrEmpty( FilterValue ) )
                            //{
                            //    FilterWhere += Filter.Conjunction.ToString().ToLower() + " " + FilterValue;
                            //}
                        } // if we really have a filter
                    } // if we have a filter
                } // foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
            } // foreach( CswNbtViewProperty Prop in Relationship.Properties )
            if( FilterWhere != string.Empty )
            {
                Where += "and (" + FilterWhere + ")";
            }

            if( Relationship.NodeIdsToFilterOut.Count > 0 )
            {
                string inclause = "";
                bool first = true;
                foreach( CswPrimaryKey NodeId in Relationship.NodeIdsToFilterOut )
                {
                    if( NodeId != null )
                    {
                        if( first ) first = false;
                        else inclause += ",";
                        inclause += NodeId.PrimaryKey.ToString();
                    }
                }
                if( inclause != string.Empty )
                    Where += " and n.nodeid not in ( " + inclause + " ) ";
            }
            if( Relationship.NodeIdsToFilterIn.Count > 0 )
            {
                string inclause = "";
                bool first = true;
                foreach( CswPrimaryKey NodeId in Relationship.NodeIdsToFilterIn )
                {
                    if( NodeId != null )
                    {
                        if( first ) first = false;
                        else inclause += ",";
                        inclause += NodeId.PrimaryKey.ToString();
                    }
                }
                if( inclause != string.Empty )
                    Where += " and n.nodeid in ( " + inclause + " ) ";
            }

            // BZ 6008
            if( false == _IncludeSystemNodes )
            {
                Where += " and n.issystem = '0' ";
            }
            if( false == _IncludeHiddenNodes )
            {
                Where += " and n.hidden = '0' ";
            }
            if( false == _IncludeTempNodes )
            {
                Where += " and n.istemp= '0' ";
            }

            string Sql = string.Empty;
            if( With.Count > 0 )
            {
                Sql = "with " + With.ToString( false );
            }
            Sql += " " + Select + " " + From + " " + Where + " " + OrderBy;


            CswArbitrarySelect Ret = _CswNbtResources.makeCswArbitrarySelect( "TreeLoader_select", Sql );
            foreach( string Parameter in FilterParameters.Keys )
            {
                Ret.addParameter( Parameter, FilterParameters[Parameter] );
            }
            return Ret;
        } //_makeNodeSql()

        private CswNbtSubField _getDefaultSubFieldForProperty( CswEnumNbtViewPropIdType Type, Int32 Id )
        {
            CswNbtSubField ret = null;
            if( Type == CswEnumNbtViewPropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( Id );
                ret = NodeTypeProp.getFieldTypeRule().SubFields.Default;
            }
            else if( Type == CswEnumNbtViewPropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp ObjectClassProp = _CswNbtResources.MetaData.getObjectClassProp( Id );
                ret = ObjectClassProp.getFieldTypeRule().SubFields.Default;
            }
            return ret;
        }

    } // class CswNbtTreeLoaderFromXmlViewByLevel

} // namespace CswNbt
