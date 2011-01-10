using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{
    public class CswNbtTreeLoaderFromXmlView : CswNbtTreeLoader
    {
        public Int32 ResultLimit = 300;  // BZ 8460

        private CswNbtResources _CswNbtResources = null;
        private CswNbtView _View;
        private bool _IncludeSystemNodes = false;
        private ICswNbtUser _RunAsUser;

        public CswNbtTreeLoaderFromXmlView( CswNbtResources CswNbtResources, ICswNbtUser RunAsUser, ICswNbtTree pCswNbtTree, CswNbtView View, bool IncludeSystemNodes )
            : base( pCswNbtTree )
        {
            _CswNbtResources = CswNbtResources;
            _RunAsUser = RunAsUser;
            _View = View;
            _IncludeSystemNodes = IncludeSystemNodes;
            string ResultLimitString = CswNbtResources.getConfigVariableValue( "treeview_resultlimit" );
            if( CswTools.IsInteger( ResultLimitString ) )
                ResultLimit = CswConvert.ToInt32( ResultLimitString );
        }

        public override void load( ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, bool FetchAllPrior, bool SingleLevelOnly, CswNbtNodeKey IncludedKey )
        {
            CswTimer _Timer = new CswTimer();
            Int32 ThisLevelNodeCount = 0;
            _CswNbtTree.SourceViewXml = _View.ToXml().InnerXml;

            CswNbtNodeKey GroupKey = null;

            if( ParentNodeKey != null && ParentNodeKey.TreeKey == _CswNbtTree.Key )
            {
                _handleRoot( _View.Root );
                _CswNbtTree.goToRoot();

                // We need to build the entire path of parent nodes.  That FetchIncludedKeyAndParentsOnly = true parameter is the key here.
                foreach( CswNbtViewRelationship R in _View.Root.ChildRelationships )
                {
                    _handleRelationship( null, R, //_View.Root.AddChildren, 
                                         PageSize, ref ThisLevelNodeCount, FetchAllPrior, false, ParentNodeKey, true );
                }

                if( ParentNodeKey.NodeSpecies == NodeSpecies.Group )
                {
                    //BZ 8461 - Grouping requires some special handling
                    GroupKey = ParentNodeKey;
                    _CswNbtTree.makeNodeCurrent( ParentNodeKey.getParentTreePath() );
                    ParentNodeKey = _CswNbtTree.getNodeKeyForCurrentPosition();
                }
                else
                {
                    _CswNbtTree.makeNodeCurrent( ParentNodeKey );
                }
            }
            else
            {
                _handleRoot( _View.Root );
                _CswNbtTree.goToRoot();
            }

            CswNbtViewNode ParentViewNode = null;
            if( ParentNodeKey == null || ParentNodeKey.TreeKey != _CswNbtTree.Key )
                ParentViewNode = _View.Root;
            else
                ParentViewNode = _View.FindViewNodeByUniqueId( ParentNodeKey.ViewNodeUniqueId );

            if( ParentViewNode != null )
            {
                bool FinishedWithThisRelationship = false;
                Int32 ThisPageSize = PageSize;
                CswNbtViewRelationship ThisRelationship = null;
                if( ChildRelationshipToStartWith != null )
                    ThisRelationship = ChildRelationshipToStartWith;
                ThisLevelNodeCount = 0;
                foreach( CswNbtViewRelationship R in ParentViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
                {
                    if( FinishedWithThisRelationship )
                    {
                        ThisPageSize = PageSize - ThisLevelNodeCount;
                        ThisLevelNodeCount = 0;
                        FinishedWithThisRelationship = false;
                        ThisRelationship = R;
                    }
                    if( ThisRelationship == null || R == ThisRelationship )   // skips ones before the ChildRelationshipToStartWith
                    {
                        if( ParentNodeKey == null || ParentNodeKey.TreeKey != _CswNbtTree.Key )
                        {
                            FinishedWithThisRelationship = _handleRelationship( null, R, //_View.Root.AddChildren, 
                                                                                ThisPageSize, ref ThisLevelNodeCount, FetchAllPrior, !SingleLevelOnly, IncludedKey, false );
                            _CswNbtResources.logTimerResult( "Built tree level from view " + _View.ViewName + " (" + _View.ViewId + ")", _Timer.ElapsedDurationInSecondsAsString );
                        }
                        else
                        {
                            FinishedWithThisRelationship = _handleRelationship( ParentNodeKey, R, //_View.Root.AddChildren, 
                                                                                ThisPageSize, ref ThisLevelNodeCount, FetchAllPrior, !SingleLevelOnly, IncludedKey, false );
                            _CswNbtResources.logTimerResult( "Built tree level from ParentKey: " + ParentNodeKey.ToString() + " ,for view " + _View.ViewName + " (" + _View.ViewId + ")", _Timer.ElapsedDurationInSecondsAsString );
                        }
                    }
                }
            }
        } // load()

        private void _handleRoot( CswNbtViewRoot ViewRoot )
        {
            _CswNbtTree.makeRootNode( ViewRoot );
        }

        private bool _handleRelationship( CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship Relationship, //NbtViewAddChildrenSetting AllowAddChildren, 
                                          Int32 PageSize, ref Int32 ThisLevelNodeCount, bool FetchAllPrior, bool Recurse, CswNbtNodeKey IncludedKey, bool FetchIncludedKeyAndParentsOnly )
        {
            CswPrimaryKey FindThisNodeId = null;
            Int32 FindThisNodeCount = Int32.MinValue;
            string FindThisGroupName = string.Empty;
            Int32 NodeCountLowerBoundExclusive = 0;
            Int32 NodeCountUpperBoundInclusive = PageSize;
            Int32 ParentDepth = 1;
            if( ParentNodeKey != null )
                ParentDepth = ParentNodeKey.TreeDepth;

            if( PageSize > 0 && IncludedKey != null && IncludedKey.TreeDepth > ParentDepth )
            {
                NodeSpecies TreePathNodeSpecies = IncludedKey.TreePathNodeSpecies( ParentDepth + 1 );
                if( TreePathNodeSpecies == NodeSpecies.Plain )
                {
                    FindThisNodeId = IncludedKey.TreePathNodeId( ParentDepth + 1 );
                    FindThisNodeCount = IncludedKey.getNodeCountAtDepth( ParentDepth + 1 );
                }
                else if( TreePathNodeSpecies == NodeSpecies.Group )
                {
                    if( IncludedKey.NodeSpecies == NodeSpecies.Group )
                    {
                        FindThisGroupName = IncludedKey.TreePathGroupName( ParentDepth + 1 );
                    }
                    else
                    {
                        FindThisNodeId = IncludedKey.TreePathNodeId( ParentDepth + 2 );
                        FindThisNodeCount = IncludedKey.getNodeCountAtDepth( ParentDepth + 2 );
                    }
                }

                if( FetchIncludedKeyAndParentsOnly )
                {
                    NodeCountUpperBoundInclusive = Int32.MinValue;
                    NodeCountLowerBoundExclusive = Int32.MinValue;
                }
                else
                {
                    while( NodeCountUpperBoundInclusive < FindThisNodeCount )
                        NodeCountUpperBoundInclusive += PageSize;
                    if( !FetchAllPrior )
                        NodeCountLowerBoundExclusive = NodeCountUpperBoundInclusive - PageSize;
                }
            }

            // BZ 8461 - Disable paging if using grouping
            if( Relationship.GroupByPropId != Int32.MinValue )
            {
                NodeCountUpperBoundInclusive = Int32.MinValue;
                NodeCountLowerBoundExclusive = Int32.MinValue;
            }

            bool NoMoreNodes = true;
            if( !FetchIncludedKeyAndParentsOnly || FindThisNodeId != null )
            {
                if( ( NodeCountUpperBoundInclusive < 0 ||
                      NodeCountLowerBoundExclusive < 0 ||
                      ThisLevelNodeCount <= ( NodeCountUpperBoundInclusive - NodeCountLowerBoundExclusive ) ) &&
                    ( ThisLevelNodeCount < ResultLimit ) )
                {
                    DataTable ResultTable = null;

                    _getNodes( _CswNbtTree.getNodeKeyForCurrentPosition(), Relationship, ref ResultTable, //NodeIdsToFilterOut, NodeIdsToFilterIn, 
                               NodeCountLowerBoundExclusive, NodeCountUpperBoundInclusive, FetchIncludedKeyAndParentsOnly ? FindThisNodeId : null );

                    bool NodeIsAllowed;
                    foreach( DataRow CurrentRow in ResultTable.Rows )
                    {
                        if( ThisLevelNodeCount < ResultLimit &&
                            ( NodeCountLowerBoundExclusive < 0 ||
                              NodeCountUpperBoundInclusive < 0 ||
                              ThisLevelNodeCount <= ( NodeCountUpperBoundInclusive - NodeCountLowerBoundExclusive ) ) ) // BZ 10315
                        {
                            NodeIsAllowed = false;
                            Collection<CswNbtNodeKey> ChildKeys = null;
                            if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                            {
                                if( _RunAsUser.CheckPermission( NodeTypePermission.View, CswConvert.ToInt32( CurrentRow["nodetypeid"] ), null, null ) )
                                    NodeIsAllowed = true;
                            }
                            else
                            {
                                // Don't know what permissions we'll put on object-class referencing views...for now just add the node.
                                NodeIsAllowed = true;
                            }

                            if( NodeIsAllowed )
                            {
                                ThisLevelNodeCount++;
                                if( NodeCountLowerBoundExclusive >= 0 && NodeCountUpperBoundInclusive > 0 &&
                                    ThisLevelNodeCount > ( NodeCountUpperBoundInclusive - NodeCountLowerBoundExclusive ) )
                                {
                                    // make the "more" node with the current node's count
                                    _CswNbtTree.makeMoreNodeFromRow( ParentNodeKey, CurrentRow, ( ThisLevelNodeCount + NodeCountLowerBoundExclusive ), Relationship );
                                    NoMoreNodes = false;
                                }
                                else
                                {
                                    // BZ 7686 - Group by
                                    string GroupName = string.Empty;
                                    if( Relationship.GroupByPropId != Int32.MinValue )
                                    {
                                        GroupName = CurrentRow["groupname"].ToString();
                                        if( GroupName == string.Empty )
                                            GroupName = "[blank]";
                                    }

                                    if( FindThisNodeCount != Int32.MinValue )
                                        ChildKeys = _CswNbtTree.loadNodeAsChildFromRow( ParentNodeKey, CurrentRow, ( Relationship.GroupByPropId != Int32.MinValue ), GroupName, Relationship, FindThisNodeCount );
                                    else
                                        ChildKeys = _CswNbtTree.loadNodeAsChildFromRow( ParentNodeKey, CurrentRow, ( Relationship.GroupByPropId != Int32.MinValue ), GroupName, Relationship, ThisLevelNodeCount + NodeCountLowerBoundExclusive );

                                    CswNbtNodeKey priorkey = _CswNbtTree.getNodeKeyForCurrentPosition();

                                    foreach( CswNbtNodeKey ChildKey in ChildKeys )
                                    {
                                        _CswNbtTree.makeNodeCurrent( ChildKey );
                                        bool ContinueDown = ( ( Recurse ||
                                                                ( FindThisNodeId != null && ChildKey.NodeId == FindThisNodeId ) ||
                                                                ( !Relationship.ShowInTree ) ||   // BZ 8082
                                                                ( FindThisGroupName != string.Empty && ChildKey.TreePathGroupName( ParentDepth + 2 ) == FindThisGroupName ) ) &&
                                                              Relationship.ChildRelationships.Count > 0 );

                                        if( ContinueDown || Relationship.ChildRelationships.Count == 0 )
                                            _CswNbtTree.setCurrentNodeExpandMode( "ClientSide" );
                                        else
                                            _CswNbtTree.setCurrentNodeExpandMode( "WebService" );

                                        _handleProperties( ChildKey, Relationship.Properties );
                                        if( ContinueDown )
                                        {
                                            Int32 NewLevelCount = 0;
                                            foreach( CswNbtViewRelationship R in Relationship.ChildRelationships )
                                            {
                                                _handleRelationship( ChildKey, R,
                                                    //Relationship.AddChildren,
                                                                     PageSize,
                                                                     ref NewLevelCount,
                                                                     FetchAllPrior, Recurse, IncludedKey, FetchIncludedKeyAndParentsOnly );
                                            }
                                        }
                                    } // foreach( CswNbtNodeKey ChildKey in ChildKeys )

                                    _CswNbtTree.makeNodeCurrent( priorkey );

                                } // if-else( NodeCountLowerBoundExclusive >= 0 && NodeCountUpperBoundInclusive > 0 && ThisLevelNodeCount > ( NodeCountUpperBoundInclusive - NodeCountLowerBoundExclusive ) )
                            } // if( NodeIsAllowed )
                        } // if( ThisLevelNodeCount < ResultLimit )
                    } // foreach (DataRow CurrentRow in ResultTable.Rows)
                } // if (ThisLevelNodeCount <= (NodeCountUpperBoundInclusive - NodeCountLowerBoundExclusive))
            }

            return NoMoreNodes;

        } // _handleRelationship()


        private void _handleProperties( CswNbtNodeKey Key, Collection<CswNbtViewProperty> PropertyList )
        {
            //// ObjectClassProps
            //string PropIds = String.Empty;
            //foreach( CswNbtViewProperty Prop in PropertyList )
            //{
            //    if( Prop.Type == CswNbtViewProperty.CswNbtPropType.ObjectClassPropId )
            //    {
            //        if( PropIds != String.Empty ) PropIds += ",";
            //        PropIds += Prop.ObjectClassPropId;
            //    }
            //}
            //if( PropIds != String.Empty )
            //{
            //    DataTable ResultTable = null;
            //    _getObjectClassProperties( Key, PropIds, ref ResultTable );

            //    foreach( DataRow CurrentRow in ResultTable.Rows )
            //    {
            //        _CswNbtTree.addProperty( CswConvert.ToInt32( CurrentRow["nodetypepropid"].ToString() ),
            //                                CurrentRow["propname"].ToString(),
            //                                CurrentRow["gestalt"].ToString(),
            //                                _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.getFieldTypeFromString( CurrentRow["fieldtype"].ToString() ) ) );
            //    }
            //}

            // NodeTypeProps
            string PropNames = String.Empty;
            foreach( CswNbtViewProperty Prop in PropertyList )
            {
                if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                {
                    // We use the ID from the CswNbtViewProperty to get the name from the CswNbtMetaDataNodeTypeProp, 
                    // and use that name to find a matching property on the node's NodeType later.
                    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );
                    if( MetaDataProp != null )
                    {
                        if( PropNames != String.Empty ) PropNames += ", ";
                        PropNames += "'" + CswTools.SafeSqlParam( MetaDataProp.PropName.ToLower() ) + "'";
                    }
                }
            }
            if( PropNames != String.Empty )
            {
                DataTable ResultTable = new CswDataTable( "getNodeTypeProperties_ResultTable", "" );
                _getNodeTypeProperties( Key, PropNames, ref ResultTable );

                foreach( DataRow CurrentRow in ResultTable.Rows )
                {
                    _CswNbtTree.addProperty( CswConvert.ToInt32( CurrentRow["nodetypepropid"].ToString() ),
                                            CurrentRow["propname"].ToString(),
                                            CurrentRow["gestalt"].ToString(),
                                            _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.getFieldTypeFromString( CurrentRow["fieldtype"].ToString() ) ) );
                }
            }
        }

        #region Query operations

        private void _getNodeTypeProperties( CswNbtNodeKey Key, string PropNames, ref DataTable ResultTable )
        {
            string Sql = @"select p.nodetypepropid, d.tablename, d.columnname
                             from nodetype_props p
                             left outer join jct_dd_ntp j on (p.nodetypepropid = j.nodetypepropid)
                             left outer join data_dictionary d on (j.datadictionaryid = d.tablecolid)
                            where nodetypeid = " + Key.NodeTypeId.ToString() + @" 
                              and lower(p.propname) in (" + PropNames + @") ";
            CswArbitrarySelect PropSelect = _CswNbtResources.makeCswArbitrarySelect( "_getNodeTypeProperties_select1", Sql );
            DataTable PropTable = null;
            try
            {
                PropTable = PropSelect.getTable();
            }
            catch( Exception ex )
            {
                throw new CswDniException( "Invalid View", "_getProperties() attempted to run invalid SQL: " + Sql, ex );
            }

            ResultTable.Columns.Add( new DataColumn( "nodetypepropid" ) );
            ResultTable.Columns.Add( new DataColumn( "propname" ) );
            ResultTable.Columns.Add( new DataColumn( "gestalt" ) );
            ResultTable.Columns.Add( new DataColumn( "fieldtype" ) );

            foreach( DataRow PropRow in PropTable.Rows )
            {
                DataTable ThisResultTable = null;
                string Sql2 = string.Empty;
                if( PropRow["tablename"].ToString() == string.Empty )
                {
                    Sql2 = @"select p.nodetypepropid, p.propname, j.gestalt, f.fieldtype
                             from nodetype_props p
                             join field_types f on (p.fieldtypeid = f.fieldtypeid) 
                             left outer join jct_nodes_props j 
                               on (p.nodetypepropid = j.nodetypepropid 
                                   and j.nodeid = " + Key.NodeId.PrimaryKey.ToString() + @")
                            where p.nodetypepropid = " + PropRow["nodetypepropid"].ToString();
                }
                else
                {
                    Sql2 = @"select p.nodetypepropid, p.propname, j." + PropRow["columnname"].ToString() + @" gestalt, f.fieldtype
                             from nodetype_props p
                             join field_types f on (p.fieldtypeid = f.fieldtypeid) 
                             left outer join " + PropRow["tablename"].ToString() + @" j 
                               on (j." + _CswNbtResources.getPrimeKeyColName( PropRow["tablename"].ToString() ) + @" = " + Key.NodeId.PrimaryKey.ToString() + @")
                            where p.nodetypepropid = " + PropRow["nodetypepropid"].ToString();
                }
                CswArbitrarySelect ThisResultSelect = _CswNbtResources.makeCswArbitrarySelect( "_getNodeTypeProperties_select2", Sql2 );
                try
                {
                    ThisResultTable = ThisResultSelect.getTable();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( "Invalid View", "_getProperties() attempted to run invalid SQL: " + Sql, ex );
                }

                DataRow ResultRow = ResultTable.NewRow();
                ResultRow["nodetypepropid"] = ThisResultTable.Rows[0]["nodetypepropid"];
                ResultRow["propname"] = ThisResultTable.Rows[0]["propname"];
                ResultRow["gestalt"] = ThisResultTable.Rows[0]["gestalt"];
                ResultRow["fieldtype"] = ThisResultTable.Rows[0]["fieldtype"];
                ResultTable.Rows.Add( ResultRow );
            }
        }

        //        private void _getObjectClassProperties( CswNbtNodeKey Key, string PropIds, ref DataTable ResultTable )
        //        {
        //            string Sql = @"select p.nodetypepropid, p.propname, 
        //                                  j.gestalt, f.fieldtype
        //                             from nodetype_props p
        //                             join field_types f on (p.fieldtypeid = f.fieldtypeid) 
        //                             join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
        //                             left outer join jct_nodes_props j on (p.nodetypepropid = j.nodetypepropid and j.nodeid = " + Key.NodeId.PrimaryKey.ToString() + @")
        //                            where op.objectclasspropid in (" + PropIds + @") ";

        //            CswArbitrarySelect ResultSelect = _CswNbtResources.makeCswArbitrarySelect( "_getNodeTypeProperties_select3", Sql );
        //            try
        //            {
        //                ResultTable = ResultSelect.getTable();
        //            }
        //            catch( Exception ex )
        //            {
        //                throw new CswDniException( "Invalid View", "_getProperties() attempted to run invalid SQL: " + Sql, ex );
        //            }
        //        } // _getObjectClassProperties()

        private string _getTableForNodeType( CswNbtViewRelationship.RelatedIdType Type, Int32 Id )
        {
            string TargetTable = "nodes";
            if( Type == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Id );
                TargetTable = NodeType.TableName;
            }
            //else if( Type == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
            //{
            //    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Id );
            //    TargetTable = ObjectClass.TableName;
            //}
            return TargetTable;
        }
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
        private CswNbtSubField _getSubFieldForProperty( CswNbtViewRelationship.PropIdType Type, Int32 Id, CswNbtSubField.SubFieldName SubFieldName )
        {
            CswNbtSubField ret = null;
            if( Type == CswNbtViewRelationship.PropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( Id );
                if( NodeTypeProp != null )
                    ret = NodeTypeProp.FieldTypeRule.SubFields[SubFieldName];
            }
            else if( Type == CswNbtViewRelationship.PropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp ObjectClassProp = _CswNbtResources.MetaData.getObjectClassProp( Id );
                ret = ObjectClassProp.FieldTypeRule.SubFields[SubFieldName];
            }
            return ret;
        }


        private void _getNodes( CswNbtNodeKey Key, CswNbtViewRelationship Relationship, ref DataTable ResultTable,
                               Int32 NodeCountLowerBoundExclusive, Int32 NodeCountUpperBoundInclusive, CswPrimaryKey LimitToNodeId )
        {
            bool AbortThis = false;

            string TargetTable = "nodes";
            if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType SecondNodeType = _CswNbtResources.MetaData.getNodeType( Relationship.SecondId );
                if( SecondNodeType != null )
                    TargetTable = SecondNodeType.TableName;
            }
            //else if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
            //{
            //    CswNbtMetaDataObjectClass SecondObjectClass = _CswNbtResources.MetaData.getObjectClass( Relationship.SecondId );
            //    TargetTable = SecondObjectClass.TableName;
            //}
            string TargetPkColumnName = _CswNbtResources.getPrimeKeyColName( TargetTable );


            string Select = @"select n." + TargetPkColumnName + @",
                                     n.nodename,
                                     t.iconfilename,
                                     t.nodetypename,
                                     t.nametemplate,
                                     t.nodetypeid,
                                     o.objectclass,
                                     o.objectclassid";
            string From = @"from " + TargetTable + @" n
                            join nodetypes t on (n.nodetypeid = t.nodetypeid)
                            join object_class o on (t.objectclassid = o.objectclassid)";
            string Where = string.Empty;
            string OrderBy = string.Empty;

            // Grouping
            if( Relationship.GroupByPropId != Int32.MinValue )
            {
                CswNbtSubField GroupBySubField = _getDefaultSubFieldForProperty( Relationship.GroupByPropType, Relationship.GroupByPropId );
                if( GroupBySubField.RelationalTable == string.Empty )
                {
                    Select += " ,g." + GroupBySubField.Column + " groupname";
                    if( Relationship.GroupByPropType == CswNbtViewRelationship.PropIdType.ObjectClassPropId )
                        From += @" left outer join (select j.nodeid, " + GroupBySubField.Column + @" 
                                                      from jct_nodes_props j 
                                                      join nodetype_props p on j.nodetypepropid = p.nodetypepropid 
                                                     where p.objectclasspropid = " + Relationship.GroupByPropId.ToString() + @"
                                                       and j.nodeidtablename = '" + TargetTable + @"') g
                                   on (g.nodeid = n." + TargetPkColumnName + @")";
                    else
                        From += @" left outer join (select j.nodeid, " + GroupBySubField.Column + @" 
                                                      from jct_nodes_props j 
                                                     where j.nodetypepropid = " + Relationship.GroupByPropId.ToString() + @"
                                                       and j.nodeidtablename = '" + TargetTable + @"') g 
                                   on (g.nodeid = n." + TargetPkColumnName + @")";
                }
                else
                {
                    Select += " ,n." + GroupBySubField.Column + " groupname";
                }
            }


            // Handle sort order
            Int32 sortAlias = 0;
            SortedList OrderByProps = new SortedList();
            String OrderByString = String.Empty;
            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                if( Prop.SortBy )
                {
                    // Case 10530
                    sortAlias++;
                    CswNbtSubField.PropColumn SubFieldColumn = Prop.NodeTypeProp.FieldTypeRule.SubFields.Default.Column;
                    if( SubFieldColumn == CswNbtSubField.PropColumn.Field1_Numeric ||
                        SubFieldColumn == CswNbtSubField.PropColumn.Field1_Date )
                    {
                        Select += ", j" + sortAlias + "." + SubFieldColumn.ToString() + " mssqlorder" + sortAlias;
                    }
                    else
                        Select += ",lower(j" + sortAlias + "." + SubFieldColumn.ToString() + ") mssqlorder" + sortAlias;

                    // Case 10533
                    if( SubFieldColumn == CswNbtSubField.PropColumn.Gestalt ||
                         SubFieldColumn == CswNbtSubField.PropColumn.ClobData )
                        OrderByString = "lower(to_char(j" + sortAlias + "." + SubFieldColumn.ToString() + "))";
                    else
                        OrderByString = "lower(j" + sortAlias + "." + SubFieldColumn.ToString() + ")";

                    From += " left outer join jct_nodes_props j" + sortAlias + " on (j" + sortAlias + ".nodeid = n.nodeid and j" + sortAlias + ".nodetypepropid = " + Prop.NodeTypePropId + ") ";

                    if( !OrderByProps.ContainsKey( Prop.Order ) )
                        OrderByProps.Add( Prop.Order, OrderByString );
                    else
                    {
                        Int32 propOrder = 0;
                        for( Int32 i = 0; i < OrderByProps.Count; i++ )
                        {
                            if( propOrder <= CswConvert.ToInt32( OrderByProps.GetKey( i ) ) )
                                propOrder = CswConvert.ToInt32( OrderByProps.GetKey( i ) ) + 1;
                        }
                        OrderByProps.Add( propOrder, OrderByString );
                    }
                } // if( Prop.SortBy )
            } // foreach( CswNbtViewProperty Prop in Relationship.Properties )

            foreach( String o in OrderByProps.Values )
            {
                if( String.Empty == OrderBy ) 
                    OrderBy = " order by " + o + " ";
                else
                    OrderBy += ", " + o + " "; 
            }
            

            if( OrderBy == string.Empty )
            {
                Select += ",lower(n.nodename) mssqlorder ";
                OrderBy = " order by lower(n.nodename)";
            }

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


            if( Key.NodeSpecies == NodeSpecies.Plain )
            {
                // Adding by relationship
                CswNbtSubField RelationshipSubField = _getSubFieldForProperty( Relationship.PropType, Relationship.PropId, CswNbtSubField.SubFieldName.NodeID );
                if( RelationshipSubField != null )
                {
                    if( RelationshipSubField.RelationalTable == string.Empty )
                    {
                        if( Relationship.PropOwner == CswNbtViewRelationship.PropOwnerType.First )
                        {
                            if( Relationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                            {
                                Where += @"and n." + TargetPkColumnName + @" in (
                                        select jnp.field1_fk
                                          from jct_nodes_props jnp
                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                         where jnp.nodeid = " + Key.NodeId.PrimaryKey.ToString() + @"
                                           and jnp.nodeidtablename = '" + Key.NodeId.TableName + @"'
                                           and p.firstpropversionid = " + Relationship.PropId + @")";
                            }
                            else
                            {

                                Where += @"and n." + TargetPkColumnName + @" in (
                                        select jnp.field1_fk
                                          from jct_nodes_props jnp
                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                          join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                         where jnp.nodeid = " + Key.NodeId.PrimaryKey.ToString() + @"
                                           and jnp.nodeidtablename = '" + Key.NodeId.TableName + @"'
                                           and op.objectclasspropid = " + Relationship.PropId + @")";
                            }
                        }
                        else
                        {
                            if( Relationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                            {
                                Where += @"and n." + TargetPkColumnName + @" in (
                                        select jnp.nodeid
                                          from jct_nodes_props jnp
                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                         where jnp.field1_fk = " + Key.NodeId.PrimaryKey.ToString() + @"
                                           and p.firstpropversionid = " + Relationship.PropId + @")";
                            }
                            else
                            {
                                Where += @"and n." + TargetPkColumnName + @" in (
                                        select jnp.nodeid
                                          from jct_nodes_props jnp
                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                          join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                         where jnp.field1_fk = " + Key.NodeId.PrimaryKey.ToString() + @"
                                           and op.objectclasspropid = " + Relationship.PropId + @")";
                            }
                        }
                    }
                    else
                    {
                        if( Relationship.PropOwner == CswNbtViewRelationship.PropOwnerType.First )
                        {
                            if( Key.NodeId.TableName != RelationshipSubField.RelationalTable )
                                throw new CswDniException( "Invalid Key", "The Key's Table does not match the Relationship Property's Table" );

                            Where += @"and n." + TargetPkColumnName + @" in (
                                        select jnp." + RelationshipSubField.RelationalColumn + @"
                                          from " + RelationshipSubField.RelationalTable + @" jnp
                                         where jnp." + _CswNbtResources.getPrimeKeyColName( RelationshipSubField.RelationalTable ) + @" = " + Key.NodeId.PrimaryKey.ToString() + @")";
                        }
                        else
                        {
                            Where += @"and n." + RelationshipSubField.RelationalColumn + @" = " + Key.NodeId.PrimaryKey.ToString() + " ";
                        }
                    }
                } // if( RelationshipSubField != null )
                else
                {
                    // This can happen if a view uses a disabled nodetype (due to module visibility)
                    // We should return 0 results
                    AbortThis = true;
                }
            } // if( Key.NodeSpecies == NodeSpecies.Plain )
            else if( Key.NodeSpecies == NodeSpecies.Root )
            {
                // Adding at the Root
                if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                    Where += "and (t.firstversionid = " + Relationship.SecondId + ") ";
                else
                    Where += "and (o.objectclassid = " + Relationship.SecondId + ") ";
            }
            else
            {
                throw new CswDniException( "Invalid Key", "CswNbtTreeLoaderFromXmlView._getNodes() was given a key with an unhandled NodeSpecies: " + Key.NodeSpecies.ToString() );
            }

            // Property Filters

            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
                {
                    ICswNbtFieldTypeRule FilterFieldTypeRule = null;
                    if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                        FilterFieldTypeRule = Prop.NodeTypeProp.FieldTypeRule;
                    else if( Prop.Type == CswNbtViewProperty.CswNbtPropType.ObjectClassPropId )
                        FilterFieldTypeRule = Prop.ObjectClassProp.FieldTypeRule;

                    string FilterValue = FilterFieldTypeRule.renderViewPropFilter( _RunAsUser, Filter );
                    CswNbtSubField FilterSubField = FilterFieldTypeRule.SubFields[Filter.SubfieldName];

                    if( FilterValue != string.Empty )
                    {
                        if( FilterSubField.RelationalTable == string.Empty )
                        {
                            if( Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
                            {
                                if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                                {
                                    Where += @" and (n." + TargetPkColumnName + @" not in (
                                            select jnp.nodeid
                                              from jct_nodes_props jnp
                                              join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                             where p.firstpropversionid = " + Prop.FirstVersionNodeTypeProp.PropId + @"
                                               and jnp.nodeidtablename = '" + TargetTable + @"')
                                            or n." + TargetPkColumnName + @" in (select s." + TargetPkColumnName + @" 
                                                              from " + TargetTable + @" s
                                                               join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"' 
                                                                                     and p.nodetypeid = s.nodetypeid)
                                                              left outer join (select j.jctnodepropid,
                                                                                      j.nodetypepropid,
                                                                                      j.nodeid, j.nodeidtablename,
                                                                                      j.field1, j.field2, j.field3, j.field4, j.field5,
                                                                                      j.gestalt,
                                                                                      j.field1_fk, j.field1_numeric, j.field1_date
                                                                                 from jct_nodes_props j) jnp
                                                                           ON (jnp.nodeid = s." + TargetPkColumnName + @" 
                                                                           and jnp.nodeidtablename = '" + TargetTable + @"'
                                                                           and jnp.nodetypepropid = p.nodetypepropid)
                                                             where " + FilterValue + @"))";
                                }
                                else
                                {
                                    Where += @" and (n." + TargetPkColumnName + @" not in (
                                            select jnp.nodeid
                                              from jct_nodes_props jnp
                                              join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
                                              join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
                                             where op.objectclasspropid = " + Prop.ObjectClassPropId + @"
                                               and jnp.nodeidtablename = '" + TargetTable + @"')
                                            or n." + TargetPkColumnName + @" in (select s." + TargetPkColumnName + @" 
                                                              from " + TargetTable + @" s
                                                              join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
                                                              join nodetype_props p on (p.objectclasspropid = op.objectclasspropid  
                                                                                    and p.nodetypeid = s.nodetypeid)
                                                              left outer join (select j.jctnodepropid,
                                                                                      j.nodetypepropid,
                                                                                      j.nodeid, j.nodeidtablename,
                                                                                      j.field1, j.field2, j.field3, j.field4, j.field5,
                                                                                      j.gestalt,
                                                                                      j.field1_fk, j.field1_numeric, j.field1_date
                                                                                 from jct_nodes_props j) jnp
                                                                           ON (jnp.nodeid = s." + TargetPkColumnName + @" 
                                                                           and jnp.nodeidtablename = '" + TargetTable + @"'
                                                                           and jnp.nodetypepropid = p.nodetypepropid)
                                                             where " + FilterValue + @"))";
                                }
                            }
                            else if( Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull )
                            {
                                if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                                {
                                    Where += @" and n." + TargetPkColumnName + @" in (select s." + TargetPkColumnName + @" 
                                                               from " + TargetTable + @" s
                                                               join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"' 
                                                                                     and p.nodetypeid = s.nodetypeid)
                                                               left outer join (select j.jctnodepropid,
                                                                                       j.nodetypepropid,
                                                                                       j.nodeid, j.nodeidtablename,
                                                                                       j.field1, j.field2, j.field3, j.field4, j.field5,
                                                                                       j.gestalt,
                                                                                       j.field1_fk, j.field1_numeric, j.field1_date
                                                                                  from jct_nodes_props j) jnp
                                                                           ON (jnp.nodeid = s." + TargetPkColumnName + @" 
                                                                           and jnp.nodeidtablename = '" + TargetTable + @"'
                                                                           and jnp.nodetypepropid = p.nodetypepropid)
                                                              where " + FilterValue + @")";
                                }
                                else
                                {
                                    Where += @" and n." + TargetPkColumnName + @" in (select s." + TargetPkColumnName + @" 
                                                               from " + TargetTable + @" s
                                                               join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
                                                               join nodetype_props p on (p.objectclasspropid = op.objectclasspropid  
                                                                                     and p.nodetypeid = s.nodetypeid)
                                                               left outer join (select j.jctnodepropid,
                                                                                       j.nodetypepropid,
                                                                                       j.nodeid, j.nodeidtablename,
                                                                                       j.field1, j.field2, j.field3, j.field4, j.field5,
                                                                                       j.gestalt,
                                                                                       j.field1_fk, j.field1_numeric, j.field1_date
                                                                                  from jct_nodes_props j) jnp 
                                                                           ON (jnp.nodeid = s." + TargetPkColumnName + @" 
                                                                           and jnp.nodeidtablename = '" + TargetTable + @"'
                                                                           and jnp.nodetypepropid = p.nodetypepropid)
                                                              where " + FilterValue + @")";
                                }

                            }
                            else if( Filter.Value != String.Empty )
                            {
                                if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                                {
                                    Where += @" and n." + TargetPkColumnName + @" in (select s." + TargetPkColumnName + @"
                                                               from " + TargetTable + @" s
                                                               join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"' 
                                                                                     and p.nodetypeid = s.nodetypeid)
                                                               left outer join (select j.jctnodepropid,
                                                                                       j.nodetypepropid,
                                                                                       j.nodeid, j.nodeidtablename,
                                                                                       field1, field2, field3, field4, field5,
                                                                                       j.gestalt,
                                                                                       j.field1_fk, j.field1_numeric, j.field1_date
                                                                                  from jct_nodes_props j) jnp
                                                                           ON (jnp.nodeid = s." + TargetPkColumnName + @" 
                                                                           and jnp.nodeidtablename = '" + TargetTable + @"'
                                                                           and jnp.nodetypepropid = p.nodetypepropid)
                                                              where " + FilterValue + @")";
                                }
                                else
                                {
                                    Where += @" and n." + TargetPkColumnName + @" in (select s." + TargetPkColumnName + @"
                                                               from " + TargetTable + @" s
                                                               join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
                                                               join nodetype_props p on (p.objectclasspropid = op.objectclasspropid  
                                                                                     and p.nodetypeid = s.nodetypeid)
                                                               left outer join (select j.jctnodepropid,
                                                                                       j.nodetypepropid,
                                                                                       j.nodeid, j.nodeidtablename,
                                                                                       field1, field2, field3, field4, field5,
                                                                                       j.gestalt,
                                                                                       j.field1_fk, j.field1_numeric, j.field1_date
                                                                                  from jct_nodes_props j) jnp 
                                                                           ON (jnp.nodeid = s." + TargetPkColumnName + @" 
                                                                           and jnp.nodeidtablename = '" + TargetTable + @"'
                                                                           and jnp.nodetypepropid = p.nodetypepropid)
                                                              where " + FilterValue + @")";
                                }
                            }
                        } //  if( FilterSubField.RelationalTable == string.empty )
                        else
                        {
                            Where += " and " + FilterValue; // n." + FilterSubField.Column + " is not null";
                        }
                    } // if( FilterValue != string.Empty )
                }
            }
            // NodeID Filters
            if( LimitToNodeId != null )
                Where += " and n." + TargetPkColumnName + " = " + LimitToNodeId.PrimaryKey.ToString() + " ";

            if( Relationship.NodeIdsToFilterOut.Count > 0 )
            {
                string inclause = "";
                bool first = true;
                foreach( CswPrimaryKey NodeId in Relationship.NodeIdsToFilterOut )
                {
                    if( NodeId != null && NodeId.TableName == TargetTable )
                    {
                        if( first ) first = false;
                        else inclause += ",";
                        inclause += NodeId.PrimaryKey.ToString();
                    }
                }
                if( inclause != string.Empty )
                    Where += " and n." + TargetPkColumnName + " not in ( " + inclause + " ) ";
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
                    Where += " and n." + TargetPkColumnName + " in ( " + inclause + " ) ";
            }

            // BZ 6008
            if( !_IncludeSystemNodes && TargetTable == "nodes" )
                Where += " and n.issystem = '0' ";

            if( !AbortThis )
            {
                string Sql = Select + " " + From + " " + Where + " " + OrderBy;
                //if( NodeCountLowerBoundExclusive > 0 || NodeCountUpperBoundInclusive > 0 )
                //    Sql = CswSqlPager.PageSql( Sql, DbVendorType.Oracle, NodeCountLowerBoundExclusive, NodeCountUpperBoundInclusive + 1 );   // +1 so that we know whether to generate a MORE node

                CswArbitrarySelect ResultSelect = _CswNbtResources.makeCswArbitrarySelect( "TreeLoader_select", Sql );
                CswTimer SqlTimer = new CswTimer();
                try
                {
                    if( NodeCountLowerBoundExclusive != Int32.MinValue && NodeCountUpperBoundInclusive != Int32.MinValue )
                    {
                        // + 1 is to know whether to make a 'More' Node
                        ResultTable = ResultSelect.getTable( NodeCountLowerBoundExclusive, NodeCountUpperBoundInclusive + 1, false, false );
                    }
                    else
                    {
                        ResultTable = ResultSelect.getTable();
                    }
                }
                catch( Exception ex )
                {
                    throw new CswDniException( "Invalid View", "_getNodes() attempted to run invalid SQL: " + Sql, ex );
                }

                if( SqlTimer.ElapsedDurationInSeconds > 2 )
                    _CswNbtResources.logMessage( "Tree View SQL required longer than 2 seconds to run: " + Sql );
            }
            else
            {
                ResultTable = new DataTable();
            }

        }//_getNodes()


        #endregion


    } // class CswNbtTreeLoader

} // namespace CswNbt
