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
			DataTable NodesTable = _getNodes( Relationship );
			foreach( DataRow NodesRow in NodesTable.Rows )
			{
				bool AddChild = true;
				CswNbtNodeKey ParentNodeKey = null;
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

					Collection<CswNbtNodeKey> ChildKeys = _CswNbtTree.loadNodeAsChildFromRow( ParentNodeKey, NodesRow, ( Relationship.GroupByPropId != Int32.MinValue ), GroupName, Relationship, ChildCount + 1 );
					foreach( CswNbtNodeKey ChildKey in ChildKeys )
					{
						_CswNbtTree.makeNodeCurrent( ChildKey );
						_handleProperties( ChildKey, Relationship.Properties );
					}
					_CswNbtTree.goToParentNode();
				}
			}
			

			foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
			{
				loadRelationshipRecursive( ChildRelationship );
			}
		} // loadRelationshipRecursive()

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
			CswCommaDelimitedString PropsInClause = new CswCommaDelimitedString( 0, true );
			foreach( CswNbtViewProperty Prop in PropertyList )
			{
				if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
				{
					PropsInClause.Add( Prop.NodeTypePropId.ToString() );
				}
			}
			if( PropsInClause.Count > 0 )
			{
				DataTable ResultTable = new CswDataTable( "getNodeTypeProperties_ResultTable", "" );
				_getNodeTypeProperties( Key, PropsInClause, ref ResultTable );

				foreach( DataRow CurrentRow in ResultTable.Rows )
				{
					_CswNbtTree.addProperty( CswConvert.ToInt32( CurrentRow["nodetypepropid"].ToString() ),
											 CswConvert.ToInt32( CurrentRow["jctnodepropid"].ToString() ),
											 CurrentRow["propname"].ToString(),
											 CurrentRow["gestalt"].ToString(),
											 _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.getFieldTypeFromString( CurrentRow["fieldtype"].ToString() ) ) );
				}
			}
		} // _handleProperties()

		private DataTable _getNodes( CswNbtViewRelationship Relationship )
		{
			DataTable ResultTable = new DataTable();
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

			// Nodetype/Object Class filter
			if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
				Where += "and (t.firstversionid = " + Relationship.SecondId + ") ";
			else
				Where += "and (o.objectclassid = " + Relationship.SecondId + ") ";

			// Parent Node
			if( Relationship.FirstId != Int32.MinValue )
			{
				Select += ",parent.parentnodeid ";
				if( Relationship.PropOwner == CswNbtViewRelationship.PropOwnerType.First )
				{
					if( Relationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
					{
						From += @"       join (select jnp.nodeid parentnodeid, jnp.field1_fk thisnodeid
		                                          from jct_nodes_props jnp
		                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
		                                         where p.firstpropversionid = " + Relationship.PropId + @") parent 
                                             on (parent.thisnodeid = n.nodeid)";
					}
					else
					{

						Where += @"      join (select jnp.nodeid parentnodeid, jnp.field1_fk thisnodeid
		                                          from jct_nodes_props jnp
		                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
		                                          join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
		                                         where op.objectclasspropid = " + Relationship.PropId + @") parent 
                                             on (parent.thisnodeid = n.nodeid)";
					}
				}
				else
				{
					if( Relationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
					{
						From += @"       join (select jnp.nodeid thisnodeid, jnp.field1_fk parentnodeid
		                                          from jct_nodes_props jnp
		                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
		                                         where p.firstpropversionid = " + Relationship.PropId + @") parent 
                                             on (parent.thisnodeid = n.nodeid)";
					}
					else
					{
						From += @"       join (select jnp.nodeid thisnodeid, jnp.field1_fk parentnodeid
		                                          from jct_nodes_props jnp
		                                          join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
		                                          join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
		                                         where op.objectclasspropid = " + Relationship.PropId + @") parent 
                                             on (parent.thisnodeid = n.nodeid)";
					}
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

					OrderByProps.Insert( Prop.Order, OrderByString );

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
									Where += @" and (n.nodeid not in (
		                                            select jnp.nodeid
		                                              from jct_nodes_props jnp
		                                              join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
		                                             where p.firstpropversionid = " + Prop.FirstVersionNodeTypeProp.PropId + @")
		                                            or n.nodeid in (select s.nodeid 
		                                                              from nodes s
		                                                               join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"' 
		                                                                                     and p.nodetypeid = s.nodetypeid)
		                                                              left outer join (select j.jctnodepropid,
		                                                                                      j.nodetypepropid,
		                                                                                      j.nodeid, j.nodeidtablename,
		                                                                                      j.field1, j.field2, j.field3, j.field4, j.field5,
		                                                                                      j.gestalt,
		                                                                                      j.field1_fk, j.field1_numeric, j.field1_date,
		                                                                                      j.field2_numeric, j.field2_date
		                                                                                 from jct_nodes_props j) jnp
		                                                                           ON (jnp.nodeid = s.nodeid 
		                                                                           and jnp.nodetypepropid = p.nodetypepropid)
		                                                             where " + FilterValue + @"))";
								}
								else
								{
									Where += @" and (n.nodeid not in (
		                                            select jnp.nodeid
		                                              from jct_nodes_props jnp
		                                              join nodetype_props p on (jnp.nodetypepropid = p.nodetypepropid)
		                                              join object_class_props op on (p.objectclasspropid = op.objectclasspropid)
		                                             where op.objectclasspropid = " + Prop.ObjectClassPropId + @")
		                                            or n.nodeid in (select s.nodeid 
		                                                              from nodes s
		                                                              join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
		                                                              join nodetype_props p on (p.objectclasspropid = op.objectclasspropid  
		                                                                                    and p.nodetypeid = s.nodetypeid)
		                                                              left outer join (select j.jctnodepropid,
		                                                                                      j.nodetypepropid,
		                                                                                      j.nodeid, j.nodeidtablename,
		                                                                                      j.field1, j.field2, j.field3, j.field4, j.field5,
		                                                                                      j.gestalt,
		                                                                                      j.field1_fk, j.field1_numeric, j.field1_date,
		                                                                                      j.field2_numeric, j.field2_date
		                                                                                 from jct_nodes_props j) jnp
		                                                                           ON (jnp.nodeid = s.nodeid 
		                                                                           and jnp.nodetypepropid = p.nodetypepropid)
		                                                             where " + FilterValue + @"))";
								}
							}
							else if( Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull )
							{
								if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
								{
									Where += @" and n.nodeid in (select s.nodeid 
		                                                               from nodes s
		                                                               join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"' 
		                                                                                     and p.nodetypeid = s.nodetypeid)
		                                                               left outer join (select j.jctnodepropid,
		                                                                                       j.nodetypepropid,
		                                                                                       j.nodeid, j.nodeidtablename,
		                                                                                       j.field1, j.field2, j.field3, j.field4, j.field5,
		                                                                                       j.gestalt,
		                                                                                       j.field1_fk, j.field1_numeric, j.field1_date,
		                                                                                       j.field2_numeric, j.field2_date
		                                                                                  from jct_nodes_props j) jnp
		                                                                           ON (jnp.nodeid = s.nodeid 
		                                                                           and jnp.nodetypepropid = p.nodetypepropid)
		                                                              where " + FilterValue + @")";
								}
								else
								{
									Where += @" and n.nodeid in (select s.nodeid 
		                                                               from nodes s
		                                                               join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
		                                                               join nodetype_props p on (p.objectclasspropid = op.objectclasspropid  
		                                                                                     and p.nodetypeid = s.nodetypeid)
		                                                               left outer join (select j.jctnodepropid,
		                                                                                       j.nodetypepropid,
		                                                                                       j.nodeid, j.nodeidtablename,
		                                                                                       j.field1, j.field2, j.field3, j.field4, j.field5,
		                                                                                       j.gestalt,
		                                                                                       j.field1_fk, j.field1_numeric, j.field1_date,
		                                                                                       j.field2_numeric, j.field2_date
		                                                                                  from jct_nodes_props j) jnp 
		                                                                           ON (jnp.nodeid = s.nodeid 
		                                                                           and jnp.nodetypepropid = p.nodetypepropid)
		                                                              where " + FilterValue + @")";
								}

							}
							else if( Filter.Value != String.Empty )
							{
								if( Prop.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
								{
									Where += @" and n.nodeid in (select s.nodeid
		                                                               from nodes s
		                                                               join nodetype_props p on (lower(p.propname) = '" + Prop.NodeTypeProp.PropName.ToLower() + @"' 
		                                                                                     and p.nodetypeid = s.nodetypeid)
		                                                               left outer join (select j.jctnodepropid,
		                                                                                       j.nodetypepropid,
		                                                                                       j.nodeid, j.nodeidtablename,
		                                                                                       field1, field2, field3, field4, field5,
		                                                                                       j.gestalt,
		                                                                                       j.field1_fk, j.field1_numeric, j.field1_date,
		                                                                                       j.field2_numeric, j.field2_date
		                                                                                  from jct_nodes_props j) jnp
		                                                                           ON (jnp.nodeid = s.nodeid 
		                                                                           and jnp.nodetypepropid = p.nodetypepropid)
		                                                              where " + FilterValue + @")";
								}
								else
								{
									Where += @" and n.nodeid in (select s.nodeid
		                                                               from nodes s
		                                                               join object_class_props op on (op.objectclasspropid = " + Prop.ObjectClassPropId + @")
		                                                               join nodetype_props p on (p.objectclasspropid = op.objectclasspropid  
		                                                                                     and p.nodetypeid = s.nodetypeid)
		                                                               left outer join (select j.jctnodepropid,
		                                                                                       j.nodetypepropid,
		                                                                                       j.nodeid, j.nodeidtablename,
		                                                                                       field1, field2, field3, field4, field5,
		                                                                                       j.gestalt,
		                                                                                       j.field1_fk, j.field1_numeric, j.field1_date,
		                                                                                       j.field2_numeric, j.field2_date
		                                                                                  from jct_nodes_props j) jnp 
		                                                                           ON (jnp.nodeid = s.nodeid 
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



			string Sql = Select + " " + From + " " + Where + " " + OrderBy;

			CswArbitrarySelect ResultSelect = _CswNbtResources.makeCswArbitrarySelect( "TreeLoader_select", Sql );
			CswTimer SqlTimer = new CswTimer();
			try
			{
				ResultTable = ResultSelect.getTable( 0, ResultLimit, false, false );
			}
			catch( Exception ex )
			{
				throw new CswDniException( ErrorType.Error, "Invalid View", "_getNodes() attempted to run invalid SQL: " + Sql, ex );
			}

			if( SqlTimer.ElapsedDurationInSeconds > 2 )
				_CswNbtResources.logMessage( "Tree View SQL required longer than 2 seconds to run: " + Sql );
			
			return ResultTable;

		} //_getNodes()

		private void _getNodeTypeProperties( CswNbtNodeKey Key, CswCommaDelimitedString PropsInClause, ref DataTable ResultTable )
		{
			string Sql = @"select p.nodetypepropid, d.tablename, d.columnname
                             from nodetype_props p
                             left outer join jct_dd_ntp j on (p.nodetypepropid = j.nodetypepropid)
                             left outer join data_dictionary d on (j.datadictionaryid = d.tablecolid)
                            where nodetypeid = " + Key.NodeTypeId.ToString() + @" 
							  and p.firstpropversionid in (select firstpropversionid 
														     from nodetype_props 
														    where nodetypepropid in (" + PropsInClause.ToString() + @"))";
			CswArbitrarySelect PropSelect = _CswNbtResources.makeCswArbitrarySelect( "_getNodeTypeProperties_select1", Sql );
			DataTable PropTable = null;
			try
			{
				PropTable = PropSelect.getTable();
			}
			catch( Exception ex )
			{
				throw new CswDniException( ErrorType.Error, "Invalid View", "_getProperties() attempted to run invalid SQL: " + Sql, ex );
			}

			ResultTable.Columns.Add( new DataColumn( "nodetypepropid" ) );
			ResultTable.Columns.Add( new DataColumn( "propname" ) );
			ResultTable.Columns.Add( new DataColumn( "jctnodepropid" ) );
			ResultTable.Columns.Add( new DataColumn( "gestalt" ) );
			ResultTable.Columns.Add( new DataColumn( "fieldtype" ) );

			foreach( DataRow PropRow in PropTable.Rows )
			{
				DataTable ThisResultTable = null;
				string Sql2 = string.Empty;
				if( PropRow["tablename"].ToString() == string.Empty )
				{
					Sql2 = @"select p.nodetypepropid, p.propname, j.jctnodepropid, j.gestalt, f.fieldtype
                             from nodetype_props p
                             join field_types f on (p.fieldtypeid = f.fieldtypeid) 
                             left outer join jct_nodes_props j 
                               on (p.nodetypepropid = j.nodetypepropid 
                                   and j.nodeid = " + Key.NodeId.PrimaryKey.ToString() + @")
                            where p.nodetypepropid = " + PropRow["nodetypepropid"].ToString();
				}
				else
				{
					Sql2 = @"select p.nodetypepropid, p.propname, j.jctnodepropid, j." + PropRow["columnname"].ToString() + @" gestalt, f.fieldtype
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
					throw new CswDniException( ErrorType.Error, "Invalid View", "_getProperties() attempted to run invalid SQL: " + Sql, ex );
				}

				DataRow ResultRow = ResultTable.NewRow();
				ResultRow["nodetypepropid"] = ThisResultTable.Rows[0]["nodetypepropid"];
				ResultRow["propname"] = ThisResultTable.Rows[0]["propname"];
				ResultRow["jctnodepropid"] = ThisResultTable.Rows[0]["jctnodepropid"];
				ResultRow["gestalt"] = ThisResultTable.Rows[0]["gestalt"];
				ResultRow["fieldtype"] = ThisResultTable.Rows[0]["fieldtype"];
				ResultTable.Rows.Add( ResultRow );
			}
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

	} // class CswNbtTreeLoaderFromXmlViewByLevel

} // namespace CswNbt
