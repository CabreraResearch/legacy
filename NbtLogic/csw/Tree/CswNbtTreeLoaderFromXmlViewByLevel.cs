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
        public Int32 ResultLimit = 1001;  // BZ 8460

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

            string ResultLimitString = CswNbtResources.ConfigVbls.getConfigVariableValue( "treeview_resultlimit" );
            if( CswTools.IsInteger( ResultLimitString ) )
                ResultLimit = CswConvert.ToInt32( ResultLimitString );
        }

        /// <summary>
        /// Deprecated, non-functional, old interface
        /// </summary>
        public override void load( ref CswNbtNodeKey ParentNodeKey,
                                   CswNbtViewRelationship ChildRelationshipToStartWith,
                                   Int32 PageSize,
                                   bool FetchAllPrior,
                                   bool SingleLevelOnly,
                                   CswNbtNodeKey IncludedKey,
                                   bool RequireViewPermissions )
        {
        }

        public void load()
        {
            _CswNbtTree.makeRootNode( _View.Root );

            _CswNbtTree.goToRoot();
            foreach( CswNbtViewRelationship Relationship in _View.Root.ChildRelationships )
            {
                loadRelationshipRecursive( Relationship );
            }
            _CswNbtTree.goToRoot();

        } // load()

        private void loadRelationshipRecursive( CswNbtViewRelationship Relationship )
        {
            CswNbtNodeKey PriorCurrentNodeKey = _CswNbtTree.getNodeKeyForCurrentPosition();

            // Nodes and Properties
            DataTable NodesTable = new DataTable();
            string Sql = _makeNodeSql( Relationship );

            CswArbitrarySelect ResultSelect = _CswNbtResources.makeCswArbitrarySelect( "TreeLoader_select", Sql );
            CswTimer SqlTimer = new CswTimer();
            try
            {
                NodesTable = ResultSelect.getTable( 0, ResultLimit, false, false );
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
            CswNbtNodeKey ParentNodeKey = null;
            foreach( DataRow NodesRow in NodesTable.Rows )
            {
                Int32 ThisNodeId = CswConvert.ToInt32(NodesRow["nodeid"]);
                
                // Handle property multiplexing
                // This assumes that property rows for the same nodeid are next to one another
                if( ThisNodeId != PriorNodeId )
                {
                    PriorNodeId = ThisNodeId;
                    NewNodeKeys = null;
                    bool AddChild = true;
                    ParentNodeKey = null;

                    if( NodesTable.Columns.Contains( "parentnodeid" ) )
                    {
                        CswPrimaryKey ParentNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodesRow["parentnodeid"] ) );

                        // We can't use getNodeKeyByNodeId, because there may be more instances of this node at different places in the tree
                        //ParentNodeKey = _CswNbtTree.getNodeKeyByNodeId( ParentNodeId );
                        ParentNodeKey = _CswNbtTree.getNodeKeyByNodeIdAndViewNode( ParentNodeId, Relationship.Parent );

                        if( ParentNodeKey != null )
                        {
                            _CswNbtTree.makeNodeCurrent( ParentNodeKey );
                        }
                        else
                        {
                            // If the parent isn't in the tree, don't add the child
                            AddChild = false;
                        }
                    } // if( NodesTable.Columns.Contains( "parentnodeid" ) )

                    if( AddChild )
                    {
                        Int32 ChildCount = _CswNbtTree.getChildNodeCount();

                        string GroupName = string.Empty;
                        if( Relationship.GroupByPropId != Int32.MinValue )
                        {
                            GroupName = NodesRow["groupname"].ToString();
                            if( GroupName == string.Empty )
                                GroupName = "[blank]";
                        }

                        NewNodeKeys = _CswNbtTree.loadNodeAsChildFromRow( ParentNodeKey, NodesRow, ( Relationship.GroupByPropId != Int32.MinValue ), GroupName, Relationship, ChildCount + 1 );
                    }
                } // if( ThisNodeId != PriorNodeId )

                // This assumes that property rows for the same nodeid are next to one another
                // It also assumes that loadNodeAsChildFromRow() made the node current
                if( NewNodeKeys != null && NodesTable.Columns.Contains( "jctnodepropid" ) )
                {
                    Int32 ThisJctNodePropId = CswConvert.ToInt32( NodesRow["jctnodepropid"] );
                    if( ThisJctNodePropId != Int32.MinValue )
                    {
                        foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                        {
                            _CswNbtTree.makeNodeCurrent( NewNodeKey );
                            _CswNbtTree.addProperty( CswConvert.ToInt32( NodesRow["nodetypepropid"] ),
                                                     ThisJctNodePropId,
                                                     NodesRow["propname"].ToString(),
                                                     NodesRow["gestalt"].ToString(),
                                                     _CswNbtResources.MetaData.getFieldType( CswConvert.ToInt32( NodesRow["fieldtypeid"] ) ) );
                        } // foreach( CswNbtNodeKey NewNodeKey in NewNodeKeys )
                        if( ParentNodeKey != null )
                        {
                            _CswNbtTree.makeNodeCurrent( ParentNodeKey );
                        }
                        else
                        {
                            _CswNbtTree.goToRoot();
                        }
                    } // if( ThisJctNodePropId != Int32.MinValue )
                } // if( NewNodeKeys != null && NodesTable.Columns.Contains( "jctnodepropid" ) )

            } // foreach(DataRow NodesRow in NodesTable.Rows)
            

            // Recurse
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                loadRelationshipRecursive( ChildRelationship );
            }
            
            _CswNbtTree.makeNodeCurrent( PriorCurrentNodeKey );

        } // loadRelationshipRecursive()


        private string _makeNodeSql( CswNbtViewRelationship Relationship )
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
            Where += @"where ((exists (select j.jctmoduleobjectclassid
                              from jct_modules_objectclass j
                              join modules m on j.moduleid = m.moduleid
                             where j.objectclassid = t.objectclassid
                               and m.enabled = '1')
                or not exists (select j.jctmoduleobjectclassid
                                 from jct_modules_objectclass j
                                 join modules m on j.moduleid = m.moduleid
                                where j.objectclassid = t.objectclassid) )
               and (exists (select j.jctmodulenodetypeid
                              from jct_modules_nodetypes j
                              join modules m on j.moduleid = m.moduleid
                             where j.nodetypeid = t.firstversionid
                               and m.enabled = '1')
                or not exists (select j.jctmodulenodetypeid
                                 from jct_modules_nodetypes j
                                 join modules m on j.moduleid = m.moduleid
                                where j.nodetypeid = t.firstversionid) )) ";

            // Nodetype/Object Class filter
            if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                Where += " and (t.firstversionid = " + Relationship.SecondId + ") ";
            else
                Where += " and (o.objectclassid = " + Relationship.SecondId + ") ";

            // Parent Node
            if( Relationship.FirstId != Int32.MinValue )
            {
                Select += ",parent.parentnodeid ";
                Where += " and parent.parentnodeid is not null ";
                if( Relationship.PropOwner == CswNbtViewRelationship.PropOwnerType.First )
                {
                    From += @"            join (select jnp.nodeid parentnodeid, jnp.field1_fk thisnodeid
                                                  from jct_nodes_props jnp
                                                  join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) ";
                    if( Relationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
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
                    if( Relationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
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
            } // if( Relationship.FirstId != Int32.MinValue )

            // Grouping
            if( Relationship.GroupByPropId != Int32.MinValue )
            {
                CswNbtSubField GroupBySubField = _getDefaultSubFieldForProperty( Relationship.GroupByPropType, Relationship.GroupByPropId );
                Select += " ,g." + GroupBySubField.Column + " groupname";
                if( Relationship.GroupByPropType == CswNbtViewRelationship.PropIdType.ObjectClassPropId )
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
                    CswNbtSubField.PropColumn SubFieldColumn = Prop.NodeTypeProp.FieldTypeRule.SubFields.Default.Column;
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
                    else
                    {
                        OrderByString = "lower(j" + sortAlias + "." + SubFieldColumn.ToString() + ")";
                    }
                    From += " left outer join jct_nodes_props j" + sortAlias + " ";
                    From += "   on (j" + sortAlias + ".nodeid = n.nodeid and j" + sortAlias + ".nodetypepropid = " + Prop.NodeTypePropId + ") ";

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
                    if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId && Prop.NodeTypePropId != Int32.MinValue )
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
                    Select += @" ,props.nodetypepropid, props.propname, props.jctnodepropid, props.gestalt, props.fieldtypeid ";

                    From += @"  left outer join ( ";
                    if( NTPropsInClause.Count > 0 )
                    {
                        From += @"  select p2.nodetypepropid, p2.propname, j.jctnodepropid, TO_CHAR(j.gestalt) gestalt, p2.fieldtypeid, j.nodeid
                                  from nodetype_props p1
                                  join nodetype_props p2 on (p2.firstpropversionid = p1.firstpropversionid)
                                  join jct_nodes_props j on (p2.nodetypepropid = j.nodetypepropid)
                                 where p1.nodetypepropid in (" + NTPropsInClause.ToString() + @")";
                        if( OCPropsInClause.Count > 0 )
                        {
                            From += @" UNION ";
                        }
                    }
                    if( OCPropsInClause.Count > 0 )
                    {
                        From += @" select ntp.nodetypepropid, ntp.propname, j.jctnodepropid, TO_CHAR(j.gestalt) gestalt, ntp.fieldtypeid, j.nodeid
                                  from object_class_props op
                                  join nodetype_props ntp on (ntp.objectclasspropid = op.objectclasspropid)
                                  join jct_nodes_props j on (ntp.nodetypepropid = j.nodetypepropid)
                                 where op.objectclasspropid in (" + OCPropsInClause.ToString() + @")";
                    }
                    From += @"   ) props on (props.nodeid = n.nodeid)";
                } // if( NTPropsInClause.Count > 0 || OCPropsInClause.Count > 0 )
            } // if(Relationship.Properties.Count > 0)


            // Property Filters

            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
                {
                    if( Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null ||
                        Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull ||
                        Filter.Value != string.Empty )
                    {
                        ICswNbtFieldTypeRule FilterFieldTypeRule = null;
                        if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                        {
                            FilterFieldTypeRule = Prop.NodeTypeProp.FieldTypeRule;
                        }
                        else if( Prop.Type == CswNbtViewProperty.CswNbtPropType.ObjectClassPropId )
                        {
                            FilterFieldTypeRule = Prop.ObjectClassProp.FieldTypeRule;
                        }
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

                                if( Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
                                {
                                    Where += @" and (n.nodeid not in (
                                  select jnp.nodeid
                                    from jct_nodes_props jnp
                                    join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid) ";
                                    if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                                    {
                                        Where += @"  where p.firstpropversionid = " + Prop.FirstVersionNodeTypeProp.PropId + @")";
                                    }
                                    else
                                    {
                                        Where += @"   join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                   where op.objectclasspropid = " + Prop.ObjectClassPropId + @")";
                                    }
                                    Where += @"     or ";

                                }
                                else
                                {
                                    Where += @" and (";
                                }

                                Where += @" n.nodeid in (select s.nodeid from nodes s ";

                                if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                                {
                                    Where += @"            join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"' ";
                                }
                                else
                                {
                                    Where += @"            join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
                                             join nodetype_props p on (p.objectclasspropid = op.objectclasspropid  ";
                                }

                                Where += @"                                         and p.nodetypeid = s.nodetypeid) 
                                                         left outer join (select j.jctnodepropid,
                                                                     j.nodetypepropid, j.nodeid, j.nodeidtablename,
                                                                     j.field1, j.field2, j.field3, j.field4, j.field5,
                                                                     j.gestalt,
                                                                     j.field1_fk, j.field1_numeric, j.field1_date,
                                                                     j.field2_numeric, j.field2_date
                                                                     from jct_nodes_props j) jnp
                                                          ON (jnp.nodeid = s.nodeid and jnp.nodetypepropid = p.nodetypepropid)
                                             where " + FilterValue + @"))";


                            } // if( FilterSubField.RelationalTable == string.empty )
                            else if( false == string.IsNullOrEmpty( FilterValue ) )
                            {
                                Where += " and " + FilterValue; // n." + FilterSubField.Column + " is not null";
                            }
                        } // if we really have a filter
                    } // if we have a filter
                } // foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
            } // foreach( CswNbtViewProperty Prop in Relationship.Properties )


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
                Where += " and n.issystem = '0' ";

            return Select + " " + From + " " + Where + " " + OrderBy;
        } //_makeNodeSql()

        private CswNbtSubField _getDefaultSubFieldForProperty( CswNbtViewRelationship.PropIdType Type, Int32 Id )
        {
            CswNbtSubField ret = null;
            if( Type == CswNbtViewRelationship.PropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( Id );
                ret = NodeTypeProp.FieldTypeRule.SubFields.Default;
            }
            else if( Type == CswNbtViewRelationship.PropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp ObjectClassProp = _CswNbtResources.MetaData.getObjectClassProp( Id );
                ret = ObjectClassProp.FieldTypeRule.SubFields.Default;
            }
            return ret;
        }

    } // class CswNbtTreeLoaderFromXmlViewByLevel

} // namespace CswNbt
