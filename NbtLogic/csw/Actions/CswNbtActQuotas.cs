using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

using ChemSW.Core;

namespace ChemSW.Nbt.Actions
{
	/// <summary>
	/// Holds logic for handling node quotas
	/// </summary>
	public class CswNbtActQuotas
	{
		private CswNbtResources _CswNbtResources = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public CswNbtActQuotas( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		/// <summary>
		/// True if the user is allowed to edit quotas
		/// </summary>
		public bool UserCanEditQuotas( ICswNbtUser User )
		{
			return ( User.Username == CswNbtObjClassUser.ChemSWAdminUsername );
		}

		/// <summary>
		/// Returns a dictionary of ObjectClassId=>Node-Count for all object classes
		/// </summary>
		public Dictionary<Int32, Int32> GetNodeCounts()
		{
			return _GetNodeCounts( Int32.MinValue );
		}

		/// <summary>
		/// Returns a Node Count for one object class
		/// </summary>
		public Int32 GetNodeCount( Int32 ObjectClassId )
		{
			Int32 ret = 0;
			Dictionary<Int32, Int32> NodeCounts = _GetNodeCounts( ObjectClassId );
			if( NodeCounts.ContainsKey( ObjectClassId ) )
			{
				ret = NodeCounts[ObjectClassId];
			}
			return ret;
		} // GetNodeCount

		/// <summary>
		/// Returns the number of locked nodes for an object class
		/// </summary>
		public Int32 GetLockedNodeCount( Int32 ObjectClassId )
		{
			CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "CswNbtActQuotas_SelectLockedNodes", "nodes" );
			string WhereClause = @"where nodetypeid in (select nodetypeid from nodetypes 
														 where objectclassid = " + ObjectClassId.ToString() + @") 
										and locked = '" + CswConvert.ToDbVal( true ).ToString() + @"'";
			return NodesSelect.getRecordCount( WhereClause );
		} // GetLockedNodeCount()

		private Dictionary<Int32, Int32> _GetNodeCounts( Int32 ObjectClassId )
		{
			Dictionary<Int32, Int32> NodeCounts = new Dictionary<Int32, Int32>();

			// Look up the object class of all nodes (deleted or no)
			string SqlSelect = @"select count(distinct nodeid) cnt, objectclassid 
							       from (select n.nodeid, o.objectclassid
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
			SqlSelect += "group by objectclassid";

			CswArbitrarySelect NodeCountSelect = _CswNbtResources.makeCswArbitrarySelect( "CswNbtActQuotas_historicalNodeCount", SqlSelect );
			DataTable NodeCountTable = NodeCountSelect.getTable();
			foreach( DataRow NodeCountRow in NodeCountTable.Rows )
			{
				NodeCounts.Add( CswConvert.ToInt32( NodeCountRow["objectclassid"] ),
								CswConvert.ToInt32( NodeCountRow["cnt"] ) );
			} // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
			return NodeCounts;
		} // _getNodeCounts()

		/// <summary>
		/// Set the quota for an object class
		/// </summary>
		public void SetQuota( Int32 ObjectClassId, Int32 NewQuota )
		{
			if( UserCanEditQuotas( _CswNbtResources.CurrentNbtUser ) )
			{
				CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
				if( ObjectClass != null )
				{
					Int32 OldQuota = ObjectClass.Quota;
					if( OldQuota != NewQuota )
					{
						CswTableUpdate OCUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtActQuotas_UpdateOC", "object_class" );
						DataTable OCTable = OCUpdate.getTable( "objectclassid", ObjectClassId );
						if( OCTable.Rows.Count > 0 )
						{
							OCTable.Rows[0]["quota"] = CswConvert.ToDbVal( NewQuota );
							OCUpdate.update( OCTable );

							if( NewQuota == Int32.MinValue )
							{
								// If the quota is cleared, we can unlock all nodes
								_UnlockAllNodes( ObjectClassId );
							}
							else if( NewQuota > OldQuota )
							{
								// If the quota is increasing, we can unlock some nodes
								// The number we can unlock is the difference between the new quota and the number of currently unlocked nodes
								Int32 NodeCount = GetNodeCount( ObjectClassId );
								Int32 LockedCount = GetLockedNodeCount( ObjectClassId );
								Int32 UnlockedCount = NodeCount - LockedCount;
								_UnlockNodes( ObjectClassId, ( NewQuota - UnlockedCount ) );
							}
						}
					} // if( OldQuota != NewQuota )
				} // if( ObjectClass != null )
			} // if( UserCanEditQuotas(_CswNbtResources.CurrentNbtUser ) )
			else
			{
				throw new CswDniException( ErrorType.Warning, "Insufficient Permissions for Quota Edits", "You do not have permission to edit object class quotas" );
			}
		} // SetQuota()

		/// <summary>
		/// Unlocks all nodes of an object class
		/// </summary>
		private void _UnlockAllNodes( Int32 ObjectClassId )
		{
			_UnlockNodes( ObjectClassId, Int32.MinValue );
		}

		/// <summary>
		/// Unlocks nodes of an object class
		/// </summary>
		private void _UnlockNodes( Int32 ObjectClassId, Int32 NumberToUnlock )
		{
			CswTableUpdate NodesUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtActQuotas_UpdateNodes", "nodes" );
			OrderByClause OrderBy = new OrderByClause( "nodeid", OrderByType.Ascending );
			string WhereClause = @"where nodetypeid in (select nodetypeid from nodetypes 
														 where objectclassid = " + ObjectClassId.ToString() + @") 
										and locked = '" + CswConvert.ToDbVal( true ).ToString() + @"'";
			DataTable NodesTable = NodesUpdate.getTable( WhereClause, new Collection<OrderByClause> { OrderBy } );

			for( Int32 i = 0; ( NumberToUnlock == Int32.MinValue || i < NumberToUnlock ) && i < NodesTable.Rows.Count; i++ )
			{
				DataRow NodesRow = NodesTable.Rows[i];
				NodesRow["locked"] = CswConvert.ToDbVal( false );
			}
			NodesUpdate.update( NodesTable );
		} // _UnlockNodes()

		/// <summary>
		/// Determins a percentage for total quota usage
		/// </summary>
		public Double GetQuotaPercent()
		{
			Double ret = 0;
			Double TotalUsed = 0;
			Double TotalQuota = 0;

			Dictionary<Int32, Int32> NodeCounts = GetNodeCounts();
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

			if( TotalQuota > 0 )
			{
				ret = TotalUsed / TotalQuota * 100;
			}
			return ret;
		} // GetQuotaPercent()

		/// <summary>
		/// Returns true if the quota has not been reached for the given nodetype
		/// </summary>
		public bool CheckQuotaNT( Int32 NodeTypeId )
		{
			bool ret = false;
			CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
			if( NodeType != null )
			{
				ret = CheckQuotaOC( NodeType.ObjectClass.ObjectClassId );
			}
			return ret;
		} // CheckQuota()

		/// <summary>
		/// Returns true if the quota has not been reached for the given object class
		/// </summary>
		public bool CheckQuotaOC( Int32 ObjectClassId )
		{
			CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
			Int32 NodeCount = GetNodeCount( ObjectClassId );
			return ( ObjectClass.Quota <= 0 || NodeCount < ObjectClass.Quota );
		} // CheckQuota()

	} // class CswNbtActQuotas
}// namespace ChemSW.Nbt.Actions