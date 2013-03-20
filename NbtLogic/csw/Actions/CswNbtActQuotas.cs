using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;

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
            return ( User.Username == CswNbtObjClassUser.ChemSWAdminUsername || User is CswNbtSystemUser );
        }

        /// <summary>
        /// Returns a dictionary of ObjectClassId=>Node-Count for all object classes
        /// </summary>
        public void GetNodeCounts( out Dictionary<Int32, Int32> NodeCountsForNodeType, out Dictionary<Int32, Int32> NodeCountsForObjectClass )
        {
            _GetNodeCounts( Int32.MinValue, out NodeCountsForNodeType, out NodeCountsForObjectClass );
        }

        /// <summary>
        /// Returns a Node Count for one object class
        /// </summary>
        public Int32 GetNodeCountForObjectClass( Int32 ObjectClassId )
        {
            Int32 ret = 0;

            Dictionary<Int32, Int32> NodeCountsForNodeType;
            Dictionary<Int32, Int32> NodeCountsForObjectClass;
            _GetNodeCounts( ObjectClassId, out NodeCountsForNodeType, out NodeCountsForObjectClass );

            if( NodeCountsForObjectClass.ContainsKey( ObjectClassId ) )
            {
                ret = NodeCountsForObjectClass[ObjectClassId];
            }
            return ret;
        } // GetNodeCountForObjectClass

        /// <summary>
        /// Returns a Node Count for one nodetype
        /// </summary>
        public Int32 GetNodeCountForNodeType( Int32 NodeTypeId )
        {
            Int32 ret = 0;
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            if( NodeType != null )
            {
                Dictionary<Int32, Int32> NodeCountsForNodeType;
                Dictionary<Int32, Int32> NodeCountsForObjectClass;
                _GetNodeCounts( NodeType.ObjectClassId, out NodeCountsForNodeType, out NodeCountsForObjectClass );

                if( NodeCountsForNodeType.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
                {
                    ret = NodeCountsForNodeType[NodeType.FirstVersionNodeTypeId];
                }
            }
            return ret;
        } // GetNodeCountForNodeType

        /// <summary>
        /// Returns the number of locked nodes for an object class
        /// </summary>
        public Int32 GetLockedNodeCountForObjectClass( Int32 ObjectClassId )
        {
            CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "CswNbtActQuotas_SelectLockedNodes", "nodes" );
            string WhereClause = @"where nodetypeid in (select nodetypeid from nodetypes 
                                                         where objectclassid = " + ObjectClassId.ToString() + @") 
                                        and locked = '" + CswConvert.ToDbVal( true ).ToString() + @"'";
            return NodesSelect.getRecordCount( WhereClause );
        } // GetLockedNodeCountForObjectClass()

        /// <summary>
        /// Returns the number of locked nodes for a nodetype
        /// </summary>
        public Int32 GetLockedNodeCountForNodeType( Int32 NodeTypeId )
        {
            Int32 ret = 0;
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            if( NodeType != null )
            {
                CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "CswNbtActQuotas_SelectLockedNodes", "nodes" );
                string WhereClause = @"where nodetypeid in (select nodetypeid from nodetypes
                                                             where firstversionid = " + NodeType.FirstVersionNodeTypeId.ToString() + @") 
                                        and locked = '" + CswConvert.ToDbVal( true ).ToString() + @"'";
                ret = NodesSelect.getRecordCount( WhereClause );
            }
            return ret;
        } // GetLockedNodeCountForNodeType()

        private void _GetNodeCounts( Int32 ObjectClassId, out Dictionary<Int32, Int32> NodeCountsForNodeType, out Dictionary<Int32, Int32> NodeCountsForObjectClass )
        {
            NodeCountsForNodeType = new Dictionary<Int32, Int32>();
            NodeCountsForObjectClass = new Dictionary<Int32, Int32>();

            // Look up the object class of all nodes (deleted or no)
            string SqlSelect = @"select count(distinct nodeid) cnt, firstversionid, objectclassid 
                                   from (select n.nodeid, t.firstversionid, o.objectclassid, n.istemp
                                           from nodes n
                                           left outer join nodetypes t on n.nodetypeid = t.nodetypeid
                                           left outer join object_class o on t.objectclassid = o.objectclassid
                                        UNION
                                         select n.nodeid, ta.firstversionid, o.objectclassid, n.istemp
                                           from nodes_audit n
                                           left outer join nodetypes_audit ta on n.nodetypeid = ta.nodetypeid
                                           left outer join object_class o on ta.objectclassid = o.objectclassid)";

            if( ObjectClassId != Int32.MinValue )
            {
                SqlSelect += "where objectclassid = '" + ObjectClassId.ToString() + "' and istemp = 0";
            }
            else
            {
                SqlSelect += "where istemp = 0";
            }
            SqlSelect += "group by objectclassid, firstversionid";

            CswArbitrarySelect NodeCountSelect = _CswNbtResources.makeCswArbitrarySelect( "CswNbtActQuotas_historicalNodeCount", SqlSelect );
            DataTable NodeCountTable = NodeCountSelect.getTable();
            foreach( DataRow NodeCountRow in NodeCountTable.Rows )
            {
                Int32 ThisObjectClassId = CswConvert.ToInt32( NodeCountRow["objectclassid"] );
                Int32 ThisNodeTypeId = CswConvert.ToInt32( NodeCountRow["firstversionid"] );

                Int32 NodeCount = CswConvert.ToInt32( NodeCountRow["cnt"] );
                if( NodeCountsForNodeType.ContainsKey( ThisNodeTypeId ) )
                {
                    NodeCountsForNodeType[ThisNodeTypeId] += NodeCount;
                }
                else
                {
                    NodeCountsForNodeType.Add( ThisNodeTypeId, NodeCount );
                }

                if( NodeCountsForObjectClass.ContainsKey( ThisObjectClassId ) )
                {
                    NodeCountsForObjectClass[ThisObjectClassId] += NodeCount;
                }
                else
                {
                    NodeCountsForObjectClass.Add( ThisObjectClassId, NodeCount );
                }
            } // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
        } // _getNodeCounts()

        /// <summary>
        /// Set the quota for an object class
        /// </summary>
        public void SetQuotaForObjectClass( Int32 ObjectClassId, Int32 NewQuota, bool ExcludeInQuotaBar )
        {
            if( NewQuota < 0 )
            {
                NewQuota = Int32.MinValue;
            }

            if( UserCanEditQuotas( _CswNbtResources.CurrentNbtUser ) )
            {
                CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
                if( ObjectClass != null )
                {
                    Int32 OldQuota = ObjectClass.Quota;
                    bool OldEx = ObjectClass.ExcludeInQuotaBar;
                    if( OldQuota != NewQuota || OldEx != ExcludeInQuotaBar )
                    {
                        CswTableUpdate OCUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtActQuotas_UpdateOC", "object_class" );
                        DataTable OCTable = OCUpdate.getTable( "objectclassid", ObjectClassId );
                        if( OCTable.Rows.Count > 0 )
                        {
                            OCTable.Rows[0]["quota"] = CswConvert.ToDbVal( NewQuota );
                            OCTable.Rows[0]["excludeinquotabar"] = CswConvert.ToDbVal( ExcludeInQuotaBar );
                            OCUpdate.update( OCTable );

                            if( NewQuota == Int32.MinValue )
                            {
                                // If the quota is cleared, we can unlock all nodes
                                _UnlockAllNodesByObjectClass( ObjectClassId );
                            }
                            else if( NewQuota > OldQuota )
                            {
                                // If the quota is increasing, we can unlock some nodes
                                // The number we can unlock is the difference between the new quota and the number of currently unlocked nodes
                                Int32 NodeCount = GetNodeCountForObjectClass( ObjectClassId );
                                Int32 LockedCount = GetLockedNodeCountForObjectClass( ObjectClassId );
                                Int32 UnlockedCount = NodeCount - LockedCount;
                                _UnlockNodesByObjectClass( ObjectClassId, ( NewQuota - UnlockedCount ) );
                            }
                        } // if( OCTable.Rows.Count > 0 )
                    } // if( OldQuota != NewQuota )
                } // if( ObjectClass != null )
            } // if( UserCanEditQuotas(_CswNbtResources.CurrentNbtUser ) )
            else
            {
                throw new CswDniException( ErrorType.Warning, "Insufficient Permissions for Quota Edits", "You do not have permission to edit object class quotas" );
            }
        } // SetQuota()

        /// <summary>
        /// Set the quota for an object class and whether or not it counts against the quota bar
        /// </summary>
        public void SetQuotaForNodeType( Int32 NodeTypeId, Int32 NewQuota, bool ExcludeInQuotaBar )
        {
            if( NewQuota < 0 )
            {
                NewQuota = Int32.MinValue;
            }

            if( UserCanEditQuotas( _CswNbtResources.CurrentNbtUser ) )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( NodeType != null )
                {
                    Int32 OldQuota = NodeType.Quota;
                    bool OldEx = NodeType.ExcludeInQuotaBar;
                    if( OldQuota != NewQuota || OldEx != ExcludeInQuotaBar )
                    {
                        CswTableUpdate NTUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtActQuotas_UpdateNT", "nodetypes" );
                        DataTable NTTable = NTUpdate.getTable( "nodetypeid", NodeType.FirstVersionNodeTypeId );
                        if( NTTable.Rows.Count > 0 )
                        {
                            NTTable.Rows[0]["quota"] = CswConvert.ToDbVal( NewQuota );
                            NTTable.Rows[0]["excludeinquotabar"] = CswConvert.ToDbVal( ExcludeInQuotaBar );
                            NTUpdate.update( NTTable );

                            if( NewQuota == Int32.MinValue )
                            {
                                // If the quota is cleared, we can unlock all nodes
                                _UnlockAllNodesByNodeType( NodeTypeId );
                            }
                            else if( NewQuota > OldQuota )
                            {
                                // If the quota is increasing, we can unlock some nodes
                                // The number we can unlock is the difference between the new quota and the number of currently unlocked nodes
                                Int32 NodeCount = GetNodeCountForNodeType( NodeTypeId );
                                Int32 LockedCount = GetLockedNodeCountForNodeType( NodeTypeId );
                                Int32 UnlockedCount = NodeCount - LockedCount;
                                _UnlockNodesByNodeType( NodeTypeId, ( NewQuota - UnlockedCount ) );
                            }
                        } // if( NTTable.Rows.Count > 0 )
                    } // if( OldQuota != NewQuota )
                } // if( NodeType != null )
            } // if( UserCanEditQuotas(_CswNbtResources.CurrentNbtUser ) )
            else
            {
                throw new CswDniException( ErrorType.Warning, "Insufficient Permissions for Quota Edits", "You do not have permission to edit object class quotas" );
            }
        } // SetQuota()



        /// <summary>
        /// Unlocks all nodes of an object class
        /// </summary>
        private void _UnlockAllNodesByObjectClass( Int32 ObjectClassId )
        {
            _UnlockNodesByObjectClass( ObjectClassId, Int32.MinValue );
        }
        /// <summary>
        /// Unlocks all nodes of an object class
        /// </summary>
        private void _UnlockAllNodesByNodeType( Int32 NodeTypeId )
        {
            _UnlockNodesByNodeType( NodeTypeId, Int32.MinValue );
        }

        /// <summary>
        /// Unlocks nodes of an object class
        /// </summary>
        private void _UnlockNodesByNodeType( Int32 NodeTypeId, Int32 NumberToUnlock )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            if( NodeType != null )
            {
                string WhereClause = @"where nodetypeid in (select nodetypeid from nodetypes
                                                            where firstversionid = " + NodeType.FirstVersionNodeTypeId.ToString() + @") ";
                _UnlockNodes( WhereClause, NumberToUnlock );
            }
        } // _UnlockNodesByNodeType()

        /// <summary>
        /// Unlocks nodes of an object class
        /// </summary>
        private void _UnlockNodesByObjectClass( Int32 ObjectClassId, Int32 NumberToUnlock )
        {
            string WhereClause = @"where nodetypeid in (select nodetypeid from nodetypes 
                                                         where objectclassid = " + ObjectClassId.ToString() + @") ";
            _UnlockNodes( WhereClause, NumberToUnlock );
        } // _UnlockNodesByObjectClass()

        /// <summary>
        /// Used by the other _UnlockNodes functions
        /// </summary>
        private void _UnlockNodes( string WhereClause, Int32 NumberToUnlock )
        {
            CswTableUpdate NodesUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtActQuotas_UpdateNodes", "nodes" );
            OrderByClause OrderBy = new OrderByClause( "nodeid", OrderByType.Ascending );
            WhereClause += @" and locked = '" + CswConvert.ToDbVal( true ).ToString() + @"'";
            DataTable NodesTable = NodesUpdate.getTable( WhereClause, new Collection<OrderByClause> { OrderBy } );

            for( Int32 i = 0; ( NumberToUnlock == Int32.MinValue || i < NumberToUnlock ) && i < NodesTable.Rows.Count; i++ )
            {
                DataRow NodesRow = NodesTable.Rows[i];
                NodesRow["locked"] = CswConvert.ToDbVal( false );
            }
            NodesUpdate.update( NodesTable );
        } // _UnlockNodes()



        /// <summary>
        /// Determines a percentage for total quota usage
        /// </summary>
        public Double GetTotalQuotaPercent()
        {
            Double ret = 0;
            Double TotalUsed = 0;
            Double TotalQuota = 0;

            Dictionary<Int32, Int32> NodeCountsForNodeType;
            Dictionary<Int32, Int32> NodeCountsForObjectClass;
            GetNodeCounts( out NodeCountsForNodeType, out NodeCountsForObjectClass );

            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClasses() )
            {
                if( ObjectClass.Quota > 0 && false == ObjectClass.ExcludeInQuotaBar )
                {
                    TotalQuota += ObjectClass.Quota;

                    if( NodeCountsForObjectClass.ContainsKey( ObjectClass.ObjectClassId ) &&
                        NodeCountsForObjectClass[ObjectClass.ObjectClassId] > 0 )
                    {
                        TotalUsed += NodeCountsForObjectClass[ObjectClass.ObjectClassId];
                    }
                }
            } // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes() )
            {
                if( NodeType.Quota > 0 && false == NodeType.ExcludeInQuotaBar )
                {
                    TotalQuota += NodeType.Quota;

                    if( NodeCountsForNodeType.ContainsKey( NodeType.NodeTypeId ) &&
                        NodeCountsForNodeType[NodeType.NodeTypeId] > 0 )
                    {
                        TotalUsed += NodeCountsForNodeType[NodeType.NodeTypeId];
                    }
                }
            } // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )

            if( TotalQuota > 0 )
            {
                ret = TotalUsed / TotalQuota * 100;
            }
            return ret;
        } // GetTotalQuotaPercent()

        /// <summary>
        /// Determines a percentage for highest quota usage
        /// </summary>
        public Double GetHighestQuotaPercent()
        {
            Double HighestPercent = 0;

            Dictionary<Int32, Int32> NodeCountsForNodeType;
            Dictionary<Int32, Int32> NodeCountsForObjectClass;
            GetNodeCounts( out NodeCountsForNodeType, out NodeCountsForObjectClass );

            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClasses() )
            {
                if( ObjectClass.Quota > 0 && false == ObjectClass.ExcludeInQuotaBar )
                {
                    Double ThisPercent = 0;
                    if( NodeCountsForObjectClass.ContainsKey( ObjectClass.ObjectClassId ) &&
                        NodeCountsForObjectClass[ObjectClass.ObjectClassId] > 0 )
                    {
                        ThisPercent = (Double) NodeCountsForObjectClass[ObjectClass.ObjectClassId] / ObjectClass.Quota * 100;
                    }
                    if( ThisPercent > HighestPercent )
                    {
                        HighestPercent = ThisPercent;
                    }
                }
            } // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes() )
            {
                if( NodeType.Quota > 0 && false == NodeType.ExcludeInQuotaBar )
                {
                    Double ThisPercent = 0;
                    if( NodeCountsForNodeType.ContainsKey( NodeType.NodeTypeId ) &&
                        NodeCountsForNodeType[NodeType.NodeTypeId] > 0 )
                    {
                        ThisPercent = (Double) NodeCountsForNodeType[NodeType.NodeTypeId] / NodeType.Quota * 100;
                    }
                    if( ThisPercent > HighestPercent )
                    {
                        HighestPercent = ThisPercent;
                    }
                }
            } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes() )

            return HighestPercent;
        } // GetHighestQuotaPercent()

        /// <summary>
        /// Returns true if the quota has not been reached for the given nodetype, or its object class
        /// </summary>
        public bool CheckQuotaNT( Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            return CheckQuotaNT( NodeType );
        } // CheckQuota()

        /// <summary>
        /// Returns true if the quota has not been reached for the given nodetype, or its object class
        /// </summary>
        public bool CheckQuotaNT( CswNbtMetaDataNodeType NodeType )
        {
            bool ret = true;
            if( null == NodeType )
            {
                throw new CswDniException( ErrorType.Warning, "Could not check the quota of the provided object.", "The supplied NodeType was null." );
            }

            Int32 Quota = NodeType.getFirstVersionNodeType().Quota;
            if( Quota > 0 )
            {
                ret = ( GetNodeCountForNodeType( NodeType.NodeTypeId ) < Quota );
            }
            ret = ret && CheckQuotaOC( NodeType.ObjectClassId );

            return ret;
        } // CheckQuota()

        /// <summary>
        /// Returns true if the quota has not been reached for the given object class
        /// </summary>
        public bool CheckQuotaOC( Int32 ObjectClassId )
        {
            bool ret = true;
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            if( ObjectClass.Quota > 0 )
            {
                ret = ( GetNodeCountForObjectClass( ObjectClassId ) < ObjectClass.Quota );
            }
            return ret;
        } // CheckQuota()

    } // class CswNbtActQuotas
}// namespace ChemSW.Nbt.Actions