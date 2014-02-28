using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    /// <summary>
    /// Scheduled Rule that checks to see if any containers are missing expected records and creates them when needed.
    /// </summary>
    public class CswScheduleLogicNbtContainerRecords : ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.ContainerRecords ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        #endregion Properties

        #region State

        //It's possible that this Rule can handle other records (such as Container Location records) if needed
        private Collection<CswPrimaryKey> _ContainerIdsWithoutReceiveTransactions = new Collection<CswPrimaryKey>();
        private Collection<bool> _ContainerHasOtherTransactions = new Collection<bool>();

        private void _setLoad( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                #region SQL
                String SQLSelect = @"with
RemainingQuantity as (
select jnp.nodeid, jnp.field1_numeric as RemainingQuantity, jnp.field1_fk as RemainingQuantityUnit
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Remaining Source Container Quantity'
),
SourceContainer as (
select jnp.nodeid, jnp.field1_fk as containerid
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ( ocp.propname = 'Source Container' or ocp.propname = 'Destination Container' )
),
containers as (
select n.nodeid
      from nodes n
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'ContainerClass'
),
dispenses as (
select 
c.nodeid containerid,
n.nodeid dispensetransactionid,
rq.RemainingQuantity RemainingQuantity
      from nodes n
      left join RemainingQuantity rq on n.nodeid = rq.nodeid
      left join SourceContainer sc on n.nodeid = sc.nodeid
      left join containers c on sc.containerid = c.nodeid
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'ContainerDispenseTransactionClass'
        and sc.containerid is not null
)
select c.nodeid, ct.dispenses
from containers c 
left join (select c.nodeid containerid, count(d.dispensetransactionid) dispenses
  from containers c
  left join dispenses d on d.containerid = c.nodeid
  group by c.nodeid) ct on c.nodeid = ct.containerid
where c.nodeid not in (select containerid from dispenses where RemainingQuantity is null)";
                #endregion SQL
                CswArbitrarySelect TransactionlessContainers = new CswArbitrarySelect( NbtResources.CswResources, "TransactionlessContainers", SQLSelect );
                DataTable ContainersTable = TransactionlessContainers.getTable();
                foreach( DataRow ContainerRow in ContainersTable.Rows )
                {
                    _ContainerIdsWithoutReceiveTransactions.Add( new CswPrimaryKey( "nodes", CswConvert.ToInt32( ContainerRow["nodeid"].ToString() ) ) );
                    _ContainerHasOtherTransactions.Add( CswConvert.ToInt32( ContainerRow["dispenses"].ToString() ) > 0 );
                }
            }
        }

        #endregion State

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //Determine the number of containers without Receive Dispense Transactions and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            if( _ContainerIdsWithoutReceiveTransactions.Count == 0 )
            {
                _setLoad( CswResources );
            }
            return _ContainerIdsWithoutReceiveTransactions.Count;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
                    {
                        int ContainersProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                        int TotalProcessedThisIteration = 0;
                        while( TotalProcessedThisIteration < ContainersProcessedPerIteration && _ContainerIdsWithoutReceiveTransactions.Count > 0 && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ) )
                        {
                            _createReceiveDispenseTransaction( _CswNbtResources, _ContainerIdsWithoutReceiveTransactions[0], _ContainerHasOtherTransactions[0] );
                            CswNbtObjClassContainer expiredContainer = _CswNbtResources.Nodes[_ContainerIdsWithoutReceiveTransactions[0]];
                            if( null != expiredContainer )
                            {
                                expiredContainer.Status.Value = CswEnumNbtContainerStatuses.Expired;
                                expiredContainer.postChanges( false );
                            }
                            _ContainerIdsWithoutReceiveTransactions.RemoveAt( 0 );
                            TotalProcessedThisIteration++;
                        }
                    }
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded;
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicExpiredContainers::GetExpiredContainers() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }
            }
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        #endregion Scheduler Methods

        #region Custom Logic

        private void _createReceiveDispenseTransaction( CswNbtResources _CswNbtResources, CswPrimaryKey ContainerId, bool ContainerHasOtherTransactions )
        {
            CswNbtMetaDataObjectClass ContainerDispTransOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerDispenseTransactionClass );
            double Qty = 0.0;
            if( ContainerHasOtherTransactions )
            {
                Qty = _getQuantityToAdd( _CswNbtResources, ContainerId, ContainerDispTransOC );
            }
            CswNbtObjClassContainer Container = _CswNbtResources.Nodes[ContainerId];
            _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerDispTransOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = NewNode;
                ContDispTransNode.DestinationContainer.RelatedNodeId = Container.NodeId;
                ContDispTransNode.QuantityDispensed.Quantity = Container.Quantity.Quantity + Qty;
                ContDispTransNode.QuantityDispensed.UnitId = Container.Quantity.UnitId;
                ContDispTransNode.Type.Value = CswEnumNbtContainerDispenseType.Receive.ToString();
                ContDispTransNode.DispensedDate.DateTimeValue = Container.DateCreated.DateTimeValue;
            } );
        }

        private double _getQuantityToAdd( CswNbtResources _CswNbtResources, CswPrimaryKey ContainerId, CswNbtMetaDataObjectClass ContainerDispTransOC )
        {
            double Qty = 0.0;
            CswNbtMetaDataObjectClassProp CDTContainerOCP = ContainerDispTransOC.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.DestinationContainer );
            CswNbtMetaDataObjectClassProp CDTDispensedDateOCP = ContainerDispTransOC.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.DispensedDate );
            CswNbtView CDTView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship CDTRel = CDTView.AddViewRelationship( ContainerDispTransOC, false );
            CDTView.AddViewPropertyAndFilter( CDTRel, CDTContainerOCP, CswEnumNbtFilterConjunction.And, ContainerId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID, false, CswEnumNbtFilterMode.Equals );
            CswNbtViewProperty DispensedDateVP = CDTView.AddViewProperty( CDTRel, CDTDispensedDateOCP );
            CDTView.setSortProperty( DispensedDateVP, CswEnumNbtViewPropertySortMethod.Ascending );
            ICswNbtTree CDTTree = _CswNbtResources.Trees.getTreeFromView( CDTView, false, false, true );
            CDTTree.goToRoot();
            if( CDTTree.getChildNodeCount() > 0 )
            {
                CDTTree.goToNthChild( 0 );
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = CDTTree.getNodeForCurrentPosition();
                Qty = ContDispTransNode.QuantityDispensed.Quantity + ContDispTransNode.RemainingSourceContainerQuantity.Quantity;
            }
            return Qty;
        }

        #endregion Custom Logic

    }//CswScheduleLogicNbtContainerRecords

}//namespace ChemSW.Nbt.Sched
