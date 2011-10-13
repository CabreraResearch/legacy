using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	/// <summary>
	/// Webservice for object class quota management
	/// </summary>
	public class CswNbtWebServiceQuotas
	{
		private CswNbtResources _CswNbtResources;

		public CswNbtWebServiceQuotas( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		private bool CanEditQuotas { get { return ( _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername ); } }

		public JObject GetQuotas()
		{
			JObject ret = new JObject();

			Dictionary<Int32, Int32> NodeCounts = _getNodeCounts( Int32.MinValue );

			ret["canedit"] = CanEditQuotas.ToString().ToLower();
			ret["objectclasses"] = new JObject();
			foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
			{
				string OCId = "oc_" + ObjectClass.ObjectClassId.ToString();
				ret["objectclasses"][OCId] = new JObject();
				ret["objectclasses"][OCId]["objectclass"] = ObjectClass.ObjectClass.ToString();
				ret["objectclasses"][OCId]["objectclassid"] = ObjectClass.ObjectClassId.ToString();
				if( NodeCounts.ContainsKey( ObjectClass.ObjectClassId ) )
				{
					ret["objectclasses"][OCId]["currentusage"] = NodeCounts[ObjectClass.ObjectClassId];
				}
				else
				{
					ret["objectclasses"][OCId]["currentusage"] = "0";
				}
				if( ObjectClass.Quota != Int32.MinValue )
				{
					ret["objectclasses"][OCId]["quota"] = ObjectClass.Quota;
				}
				else
				{
					ret["objectclasses"][OCId]["quota"] = "";
				}
				ret["objectclasses"][OCId]["nodetypecount"] = ObjectClass.NodeTypes.Count.ToString();

				ret["objectclasses"][OCId]["nodetypes"] = new JObject();
				foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
				{
					if( NodeType.IsLatestVersion )
					{
						string NTId = "nt_" + NodeType.FirstVersionNodeTypeId.ToString();
						ret["objectclasses"][OCId]["nodetypes"][NTId] = new JObject();
						ret["objectclasses"][OCId]["nodetypes"][NTId]["nodetypename"] = NodeType.NodeTypeName;
						ret["objectclasses"][OCId]["nodetypes"][NTId]["nodetypeid"] = NodeType.NodeTypeId;
					} // if( NodeType.IsLatestVersion )
				} // foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
			} // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )

			return ret;
		} // GetQuotas()

		private Dictionary<Int32, Int32> _getNodeCounts( Int32 ObjectClassId )
		{
			Dictionary<Int32, Int32> NodeCounts = new Dictionary<Int32, Int32>();

			// Look up the object class of all nodes (deleted or no)
			string SqlSelect = "select count(distinct nodeid) cnt ";
			if( ObjectClassId == Int32.MinValue )
			{
				SqlSelect += "         , objectclassid ";
			}
			SqlSelect += @"       from (select n.nodeid, o.objectclassid
										   from nodes_audit n
										   left outer join nodetypes t on n.nodetypeid = t.nodetypeid
										   left outer join object_class o on t.objectclassid = o.objectclassid
										UNION
										 select n.nodeid, oa.objectclassid
										   from nodes_audit n
										   left outer join nodetypes_audit ta on n.nodetypeid = ta.nodetypeid
										   left outer join object_class_audit oa on ta.objectclassid = oa.objectclassid)";
			if( ObjectClassId != Int32.MinValue )
			{
				SqlSelect += "where objectclassid = '" + ObjectClassId.ToString() + "'";
			}
			else
			{
				SqlSelect += "group by objectclassid";
			}
			CswArbitrarySelect NodeCountSelect = _CswNbtResources.makeCswArbitrarySelect( "CswNbtWebServiceQuotas_historicalNodeCount", SqlSelect );
			DataTable NodeCountTable = NodeCountSelect.getTable();
			foreach( DataRow NodeCountRow in NodeCountTable.Rows )
			{
				NodeCounts.Add( CswConvert.ToInt32( NodeCountRow["objectclassid"] ),
								CswConvert.ToInt32( NodeCountRow["cnt"] ) );
			} // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
			return NodeCounts;
		} // _getNodeCount()

		public bool SaveQuotas( string inQuotas )
		{
			Dictionary<Int32, Int32> NodeCounts = _getNodeCounts( Int32.MinValue );

			JObject inQuotasJson = JObject.Parse( inQuotas );
			if( CanEditQuotas )
			{
				CswTableUpdate OCUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtWebServiceQuotas_UpdateOC", "object_class" );
				DataTable OCTable = OCUpdate.getTable();
				foreach( DataRow OCRow in OCTable.Rows )
				{
					Int32 ObjectClassId = CswConvert.ToInt32( OCRow["objectclassid"] );
					string OCId = "oc_" + ObjectClassId.ToString();
					if( inQuotasJson["objectclasses"][OCId] != null )
					{
						Int32 OldQuota = CswConvert.ToInt32( OCRow["quota"] );
						Int32 NewQuota = CswConvert.ToInt32( inQuotasJson["objectclasses"][OCId]["quota"] );
						if( OldQuota != NewQuota )
						{
							OCRow["quota"] = CswConvert.ToDbVal( NewQuota );

							// If the quota is increasing, we can unlock some nodes
							if( NewQuota > OldQuota )
							{
								if( NodeCounts.ContainsKey( ObjectClassId ) )
								{
									Int32 Count = NodeCounts[ObjectClassId];
									if( Count > OldQuota )
									{
										CswTableUpdate NodesUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtWebSErviceQuotas_UpdateNodes", "nodes" );
										OrderByClause OrderBy = new OrderByClause( "nodeid", OrderByType.Ascending );
										string WhereClause = @"where nodetypeid in (select nodetypeid 
																					  from nodetypes 
																					 where objectclassid = " + ObjectClassId.ToString() + @") 
																 and locked = '" + CswConvert.ToDbVal( true ).ToString() + @"'";
										DataTable NodesTable = NodesUpdate.getTable( WhereClause, new Collection<OrderByClause> { OrderBy } );
										for( Int32 i = 0; i < ( NewQuota - OldQuota ) && i < NodesTable.Rows.Count; i++ )
										{
											DataRow NodesRow = NodesTable.Rows[i];
											NodesRow["locked"] = CswConvert.ToDbVal( false );
										}
										NodesUpdate.update( NodesTable );

									}	 // if( Count > OldQuota )
								} // if( NodeCounts.ContainsKey( ObjectClassId ) )
							} // if( NewQuota > OldQuota )
						} // if(OldQuota != NewQuota)
					}
				} // foreach( DataRow OCRow in OCTable.Rows )
				OCUpdate.update( OCTable );

				return true;
			} // if( CanEditQuotas )
			else
			{
				return false;
			}
		} // SaveQuotas()

		public Double GetQuotaPercent()
		{
			Double TotalUsed = 0;
			Double TotalQuota = 0;

			Dictionary<Int32, Int32> NodeCounts = _getNodeCounts( Int32.MinValue );
			foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
			{
				if( ObjectClass.Quota > 0 )
				{
					TotalQuota += ObjectClass.Quota;
					if( NodeCounts.ContainsKey( ObjectClass.ObjectClassId ) &&
					NodeCounts[ObjectClass.ObjectClassId] > 0 )
					{
						TotalUsed += NodeCounts[ObjectClass.ObjectClassId];
					}
				}
			} // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )

			return ( TotalUsed / TotalQuota * 100 );
		} // GetQuotaPercent()

		/// <summary>
		/// Returns true if the quota has not been reached for the given nodetype
		/// </summary>
		public bool CheckQuota( Int32 NodeTypeId )
		{
			bool ret = false;
			CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
			if( NodeType != null )
			{
				Dictionary<Int32, Int32> NodeCounts = _getNodeCounts( NodeType.ObjectClass.ObjectClassId );
				Int32 NodeCount = NodeCounts[NodeType.ObjectClass.ObjectClassId];
				if( NodeCount < NodeType.ObjectClass.Quota )
				{
					ret = true;
				}
			} // if( NodeType != null )
			return ret;
		} // CheckQuota()

	} // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

