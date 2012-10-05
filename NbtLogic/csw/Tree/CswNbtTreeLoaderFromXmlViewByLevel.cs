using System;
using System.Collections.ObjectModel;
using System.Data;
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
        private bool _IncludeSystemNodes = false;

        public CswNbtTreeLoaderFromXmlViewByLevel( CswNbtResources CswNbtResources, ICswNbtUser RunAsUser, ICswNbtTree pCswNbtTree, CswNbtView View, bool IncludeSystemNodes )
            : base( pCswNbtTree )
        {
            _CswNbtResources = CswNbtResources;
            _RunAsUser = RunAsUser;
            _View = View;
            _IncludeSystemNodes = IncludeSystemNodes;
        }

        public override void load( bool RequireViewPermissions )
        {
            _CswNbtTree.makeRootNode( _View.Root );

            _CswNbtTree.goToRoot();
            foreach( CswNbtViewRelationship Relationship in _View.Root.ChildRelationships )
            {
                loadRelationshipRecursive( Relationship, RequireViewPermissions );
            }
            _CswNbtTree.goToRoot();

        } // load()

        private void loadRelationshipRecursive( CswNbtViewRelationship Relationship, bool RequireViewPermissions )
        {
            CswNbtNodeKey PriorCurrentNodeKey = _CswNbtTree.getNodeKeyForCurrentPosition();

            // Nodes and Properties
            DataTable NodesTable = new DataTable();
            string Sql = _makeNodeSql( Relationship, false );

            Int32 thisResultLimit = _CswNbtResources.TreeViewResultLimit;
            if( Relationship.Properties.Count > 0 )
            {
                thisResultLimit = thisResultLimit * Relationship.Properties.Count;
            }

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
            _CswNbtResources.CswLogger.TreeLoaderSQLTime += SqlTimer.ElapsedDurationInMilliseconds;

            if( SqlTimer.ElapsedDurationInSeconds > 2 )
            {
                _CswNbtResources.logMessage( "Tree View SQL required longer than 2 seconds to run: " + Sql );
            }

            Int32 PriorNodeId = Int32.MinValue;
            Int32 PriorParentNodeId = Int32.MinValue;
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

                // Verify permissions
                // this could be a performance problem
                CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeTypeId );
                if( false == RequireViewPermissions ||
                    _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, ThisNodeType ) ||
                   _CswNbtResources.Permit.canAnyTab( CswNbtPermit.NodeTypePermission.View, ThisNodeType, _RunAsUser ) ||
                   _CswNbtResources.Permit.canNode( CswNbtPermit.NodeTypePermission.View, ThisNodeType, ThisNodePk, _RunAsUser )

                    )
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

                        string GroupName = string.Empty;
                        if( Relationship.GroupByPropId != Int32.MinValue )
                        {
                            GroupName = NodesRow["groupname"].ToString();
                            if( GroupName == string.Empty )
                                GroupName = "[blank]";
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
                                    ThisNewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( ParentNodeKey, NodesRow, ( Relationship.GroupByPropId != Int32.MinValue ), GroupName, Relationship, ChildCount + 1, Included );
                                    foreach( CswNbtNodeKey ThisNewNodeKey in ThisNewNodeKeys )
                                    {
                                        NewNodeKeys.Add( ThisNewNodeKey );
                                    }
                                } // foreach( CswNbtNodeKey ParentNodeKey in ParentNodeKeys )
                            }

                        } // if( NodesTable.Columns.Contains( "parentnodeid" ) )
                        else
                        {
                            Int32 ChildCount = _CswNbtTree.getChildNodeCount();
                            ThisNewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( null, NodesRow, ( Relationship.GroupByPropId != Int32.MinValue ), GroupName, Relationship, ChildCount + 1, Included );
                            foreach( CswNbtNodeKey ThisNewNodeKey in ThisNewNodeKeys )
                            {
                                NewNodeKeys.Add( ThisNewNodeKey );
                            }
                        } // if( AddChild )
                    } // if( ThisNodeId != PriorNodeId )

                    // This assumes that property rows for the same nodeid are next to one another
                    // It also assumes that loadNodeAsChildFromRow() made the node current
                    if( NewNodeKeys.Count > 0 && NodesTable.Columns.Contains( "jctnodepropid" ) )
                    {
                        //Int32 ThisJctNodePropId = CswConvert.ToInt32( NodesRow["jctnodepropid"] );
                        //if( ThisJctNodePropId != Int32.MinValue )
                        //{
                        foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                        {
                            _CswNbtTree.makeNodeCurrent( NewNodeKey );
                            _CswNbtTree.addProperty( CswConvert.ToInt32( NodesRow["nodetypepropid"] ),
                                                     CswConvert.ToInt32( NodesRow["jctnodepropid"] ),
                                                     NodesRow["propname"].ToString(),
                                                     NodesRow["gestalt"].ToString(),
                                                     CswConvert.ToString( NodesRow["fieldtype"] ),
                                                     CswConvert.ToString( NodesRow["field1"] ),
                                                     CswConvert.ToString( NodesRow["field2"] ),
                                                     CswConvert.ToInt32( NodesRow["field1_fk"] ),
                                                     CswConvert.ToDouble( NodesRow["field1_numeric"] ),
                                                     CswConvert.ToBoolean( NodesRow["hidden"] ) );

                        } // foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                        if( ParentNodeKeys.Count > 0 )
                        {
                            _CswNbtTree.makeNodeCurrent( ParentNodeKeys[0] );
                        }
                        else
                        {
                            _CswNbtTree.goToRoot();
                        }
                        //} // if( ThisJctNodePropId != Int32.MinValue )
                    } // if( NewNodeKeys != null && NodesTable.Columns.Contains( "jctnodepropid" ) )
                } // if( false == RequireViewPermissions || _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, ThisNodeType, true, null, _RunAsUser ) )
            } // foreach(DataRow NodesRow in NodesTable.Rows)

            if( NodesTable.Rows.Count > 0 ) // only recurse if there are results
            {
                // Recurse
                foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
                {
                    loadRelationshipRecursive( ChildRelationship, RequireViewPermissions );
                }

                // case 24678 - Mark truncated results
                if( NodesTable.Rows.Count == thisResultLimit )
                {
                    if( ParentNodeKeys != null && ParentNodeKeys.Count > 0 )
                    {
                        foreach( CswNbtNodeKey ParentNodeKey in ParentNodeKeys )
                        {
                            // assume truncation on every potential parent
                            _CswNbtTree.makeNodeCurrent( ParentNodeKey );
                            _CswNbtTree.goToParentNode();
                            for( Int32 c = 0; c < _CswNbtTree.getChildNodeCount(); c++ )
                            {
                                _CswNbtTree.goToNthChild( c );
                                _CswNbtTree.setCurrentNodeChildrenTruncated( true );
                                _CswNbtTree.goToParentNode();
                            }
                        }
                    }
                    else
                    {
                        _CswNbtTree.goToRoot();
                        _CswNbtTree.setCurrentNodeChildrenTruncated( true );
                    }
                } // if( NodesTable.Rows.Count == thisResultLimit )
            } // if( NodesTable.Rows.Count > 0 )

            _CswNbtTree.makeNodeCurrent( PriorCurrentNodeKey );

        } // loadRelationshipRecursive()


        private string _makeNodeSql( CswNbtViewRelationship Relationship, bool IsParentQuery )
        {
            string Select = "select n.nodeid ";
            if( false == IsParentQuery )
            {
                Select += @", n.nodename, 
                              n.locked,
                              t.iconfilename,
                              t.nodetypename,
                              t.nametemplate,
                              t.nodetypeid,
                              o.objectclass,
                              o.objectclassid ";
            }
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

            // Nodetype/Object Class filter
            if( Relationship.SecondType == NbtViewRelatedIdType.NodeTypeId )
                Where += " and (t.firstversionid = " + Relationship.SecondId + ") ";
            else
                Where += " and (o.objectclassid = " + Relationship.SecondId + ") ";

            // Parent Node
            if( Relationship.PropId != Int32.MinValue )
            {
                if( false == IsParentQuery )
                {
                    Select += ",parent.parentnodeid ";
                }
                //Where += " and parent.parentnodeid is not null ";

                Where += " and parent.parentnodeid in (" + _makeNodeSql( (CswNbtViewRelationship) Relationship.Parent, true ) + ")";

                if( Relationship.PropOwner == NbtViewPropOwnerType.First )
                {
                    From += @"            join (select jnp.nodeid parentnodeid, jnp.field1_fk thisnodeid
                                                  from jct_nodes_props jnp
                                                  join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) ";
                    if( Relationship.PropType == NbtViewPropIdType.NodeTypePropId )
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
                    if( Relationship.PropType == NbtViewPropIdType.NodeTypePropId )
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
            } // if( Relationship.PropId != Int32.MinValue )

            // Grouping
            if( false == IsParentQuery )
            {
                if( Relationship.GroupByPropId != Int32.MinValue )
                {
                    CswNbtSubField GroupBySubField = _getDefaultSubFieldForProperty( Relationship.GroupByPropType, Relationship.GroupByPropId );
                    Select += " ,g." + GroupBySubField.Column + " groupname";
                    if( Relationship.GroupByPropType == NbtViewPropIdType.ObjectClassPropId )
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
                CswCommaDelimitedString OrderByProps = new CswCommaDelimitedString();
                String OrderByString = String.Empty;
                foreach( CswNbtViewProperty Prop in Relationship.Properties )
                {
                    if( Prop.SortBy )
                    {
                        // Case 10530
                        sortAlias++;
                        if( null != Prop.MetaDataProp )
                        {
                            CswNbtSubField.PropColumn SubFieldColumn = Prop.MetaDataProp.getFieldTypeRule().SubFields.Default.Column;
                            if( SubFieldColumn == CswNbtSubField.PropColumn.Field1_Numeric ||
                                 SubFieldColumn == CswNbtSubField.PropColumn.Field1_Date ||
                                 SubFieldColumn == CswNbtSubField.PropColumn.Field2_Numeric ||
                                 SubFieldColumn == CswNbtSubField.PropColumn.Field2_Date )
                            {
                                Select += ", j" + sortAlias + "." + SubFieldColumn.ToString() + " mssqlorder" + sortAlias;
                            }
                            else
                            {
                                Select += ",lower(j" + sortAlias + "." + SubFieldColumn.ToString() + ") mssqlorder" + sortAlias;
                            }

                            // Case 10533
                            if( SubFieldColumn == CswNbtSubField.PropColumn.Gestalt ||
                                 SubFieldColumn == CswNbtSubField.PropColumn.ClobData )
                            {
                                OrderByString = "lower(to_char(j" + sortAlias + "." + SubFieldColumn.ToString() + "))";
                            }
                            else if( SubFieldColumn == CswNbtSubField.PropColumn.Field1_Numeric ||
                                      SubFieldColumn == CswNbtSubField.PropColumn.Field1_Date ||
                                      SubFieldColumn == CswNbtSubField.PropColumn.Field2_Numeric ||
                                      SubFieldColumn == CswNbtSubField.PropColumn.Field2_Date )
                            {
                                OrderByString = "j" + sortAlias + "." + SubFieldColumn.ToString();
                            }
                            else
                            {
                                OrderByString = "lower(j" + sortAlias + "." + SubFieldColumn.ToString() + ")";
                            }

                            if( Prop.SortMethod == NbtViewPropertySortMethod.Descending )
                            {
                                OrderByString += " desc";
                            }

                            From += " left outer join jct_nodes_props j" + sortAlias + " ";
                            From += "   on (j" + sortAlias + ".nodeid = n.nodeid and ";
                            if( Prop.Type == NbtViewPropType.NodeTypePropId )
                            {
                                From += " j" + sortAlias + ".nodetypepropid = " + Prop.NodeTypePropId + ") ";
                            }
                            else
                            {
                                From += " j" + sortAlias + ".nodetypepropid in (select nodetypepropid from nodetype_props where objectclasspropid = " + Prop.ObjectClassPropId + ")) ";
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

                if( OrderByProps.Count == 0 )
                {
                    Select += ",lower(n.nodename) mssqlorder ";
                    OrderBy = " order by lower(n.nodename)";
                }
                else
                {
                    OrderBy = " order by " + OrderByProps.ToString() + " ";
                }

                OrderBy += ",n.nodeid "; // for property multiplexing


                // Properties for Select
                if( Relationship.Properties.Count > 0 )
                {
                    CswCommaDelimitedString NTPropsInClause = new CswCommaDelimitedString( 0, true );
                    CswCommaDelimitedString OCPropsInClause = new CswCommaDelimitedString( 0, true );
                    foreach( CswNbtViewProperty Prop in Relationship.Properties )
                    {
                        if( Prop.Type == NbtViewPropType.NodeTypePropId && Prop.NodeTypePropId != Int32.MinValue )
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
                        Select += @" ,props.nodetypepropid, props.propname, props.fieldtype ";

                        From += @"  left outer join ( ";
                        if( NTPropsInClause.Count > 0 )
                        {
                            From += @"  select p2.nodetypeid, p2.nodetypepropid, p2.propname, f.fieldtype
                                  from nodetype_props p1
                                  join nodetype_props p2 on (p2.firstpropversionid = p1.firstpropversionid or p1.propname = p2.propname)
                                  join field_types f on f.fieldtypeid = p2.fieldtypeid
                                 where p1.nodetypepropid in (" + NTPropsInClause.ToString() + @")";
                            if( OCPropsInClause.Count > 0 )
                            {
                                From += @" UNION ALL ";
                            }
                        }
                        if( OCPropsInClause.Count > 0 )
                        {
                            From += @" select ntp.nodetypeid, ntp.nodetypepropid, ntp.propname, f.fieldtype
                                  from object_class_props op
                                  join nodetype_props ntp on (ntp.objectclasspropid = op.objectclasspropid or ntp.propname = op.propname)
                                  join field_types f on f.fieldtypeid = ntp.fieldtypeid
                                 where op.objectclasspropid in (" + OCPropsInClause.ToString() + @")";
                        }
                        From += @"   ) props on (props.nodetypeid = t.nodetypeid)";  // intentional multiplexing

                        // Property Values
                        Select += @" ,propval.jctnodepropid, propval.gestalt, propval.field1, propval.field2, propval.field1_fk, propval.field1_numeric, propval.hidden ";
                        From += @"  left outer join jct_nodes_props propvaljoin on (props.nodetypepropid = propvaljoin.nodetypepropid and propvaljoin.nodeid = n.nodeid) ";  // better performance from indexes if we do this first
                        From += @"  left outer join jct_nodes_props propval on (propval.jctnodepropid = propvaljoin.jctnodepropid) ";

                    } // if( NTPropsInClause.Count > 0 || OCPropsInClause.Count > 0 )
                } // if(Relationship.Properties.Count > 0)
            } // if( false == IsParentQuery )

            // Property Filters
            Int32 FilterCount = 0;
            string FilterWhere = string.Empty;
            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
                {
                    if( Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null ||
                        Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull ||
                        Filter.Value != string.Empty )
                    {
                        FilterCount += 1;
                        ICswNbtFieldTypeRule FilterFieldTypeRule = _CswNbtResources.MetaData.getFieldTypeRule( Prop.FieldType );
                        //if( Prop.Type == CswNbtPropType.NodeTypePropId )
                        //{
                        //    FilterFieldTypeRule = Prop.NodeTypeProp.getFieldTypeRule();
                        //}
                        //else if( Prop.Type == CswNbtPropType.ObjectClassPropId )
                        //{
                        //    FilterFieldTypeRule = Prop.ObjectClassProp.getFieldTypeRule();
                        //}
                        string FilterValue = string.Empty;
                        if( null != FilterFieldTypeRule )
                        {
                            FilterValue = FilterFieldTypeRule.renderViewPropFilter( _RunAsUser, Filter );
                        }
                        if( false == string.IsNullOrEmpty( FilterValue ) )
                        {
                            CswNbtSubField FilterSubField = FilterFieldTypeRule.SubFields[Filter.SubfieldName];

                            if( FilterSubField.RelationalTable == string.Empty )
                            {
                                string FilterClause = @"select z.nodeid, '1' as included from nodes z where ";
                                if( Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
                                {
                                    FilterClause += @"(z.nodeid not in (
                                  select jnp.nodeid
                                    from jct_nodes_props jnp
                                    join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) ";
                                    if( Prop.Type == NbtViewPropType.NodeTypePropId )
                                    {
                                        FilterClause += @"  where p.firstpropversionid = " + Prop.FirstVersionNodeTypeProp.PropId + @")";
                                    }
                                    else
                                    {
                                        FilterClause += @"   join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                   where op.objectclasspropid = " + Prop.ObjectClassPropId + @")";
                                    }
                                    FilterClause += @"     or ";

                                }
                                else
                                {
                                    FilterClause += @"(";
                                }

                                FilterClause += @" z.nodeid in (select n.nodeid from nodes n ";

                                if( Prop.Type == NbtViewPropType.NodeTypePropId )
                                {
                                    FilterClause += @"            join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"') ";
                                }
                                else
                                {
                                    FilterClause += @"            join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
                                                                  join nodetype_props p on (p.objectclasspropid = op.objectclasspropid) ";
                                }
                                FilterClause += @"                join jct_nodes_props jnp on (jnp.nodeid = n.nodeid and jnp.nodetypepropid = p.nodetypepropid)
                                                                  where " + FilterValue + @"))";


                                From += "left outer join (" + FilterClause + ") f" + FilterCount.ToString() + " on (f" + FilterCount.ToString() + ".nodeid = n.nodeid)";
                                if( Filter.ResultMode == CswNbtPropFilterSql.FilterResultMode.Disabled && false == IsParentQuery )
                                {
                                    Select += ",f" + FilterCount.ToString() + ".included as included" + Filter.Conjunction.ToString() + FilterCount.ToString();
                                }
                                if( Filter.ResultMode == CswNbtPropFilterSql.FilterResultMode.Hide )
                                {
                                    if( FilterWhere != string.Empty )
                                    {
                                        FilterWhere += Filter.Conjunction.ToString().ToLower();
                                    }
                                    FilterWhere += " f" + FilterCount.ToString() + ".included = '1' ";
                                }

                            } // if( FilterSubField.RelationalTable == string.empty )
                            else if( false == string.IsNullOrEmpty( FilterValue ) )
                            {
                                FilterWhere += Filter.Conjunction.ToString().ToLower() + " " + FilterValue; // n." + FilterSubField.Column + " is not null";
                            }
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
            if( !_IncludeSystemNodes )
            {
                Where += " and n.issystem = '0' ";
            }
            Where += " and n.istemp= '0' ";
            string ret = Select + " " + From + " " + Where;
            if( false == IsParentQuery )
            {
                ret += " " + OrderBy;
            }
            return ret;
        } //_makeNodeSql()

        private CswNbtSubField _getDefaultSubFieldForProperty( NbtViewPropIdType Type, Int32 Id )
        {
            CswNbtSubField ret = null;
            if( Type == NbtViewPropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( Id );
                ret = NodeTypeProp.getFieldTypeRule().SubFields.Default;
            }
            else if( Type == NbtViewPropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp ObjectClassProp = _CswNbtResources.MetaData.getObjectClassProp( Id );
                ret = ObjectClassProp.getFieldTypeRule().SubFields.Default;
            }
            return ret;
        }

    } // class CswNbtTreeLoaderFromXmlViewByLevel

} // namespace CswNbt
