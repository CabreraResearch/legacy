using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpContainerReconciliationActions : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.ContainerReconciliationActions;

        public CswNbtBatchOpContainerReconciliationActions( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #region BatchOp Methods

        /// <summary>
        /// Returns the percentage of the task that is complete
        /// </summary>
        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double PercentDone = 0;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.ContainerReconciliationActions )
            {
                ContainerReconciliationActionsBatchData BatchData = BatchNode.BatchData.Text;
                PercentDone = CswConvert.ToDouble( BatchData.TotalContainerLocations - BatchData.ContainerLocationIds.Count )
                    / CswConvert.ToDouble( BatchData.TotalContainerLocations * 100 );
            }
            return PercentDone;
        }

        /// <summary>
        /// Create a new batch operation to execute outstanding ContainerLocation reconciliation actions
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswCommaDelimitedString ContainerLocationIds, int totalContainerProcessedPerIteration )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( false == ContainerLocationIds.IsEmpty )
            {
                ContainerReconciliationActionsBatchData BatchData = new ContainerReconciliationActionsBatchData( ContainerLocationIds, totalContainerProcessedPerIteration );
                BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            }
            return BatchNode;
        }

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.ContainerReconciliationActions )
                {
                    BatchNode.start();
                    ContainerReconciliationActionsBatchData BatchData = BatchNode.BatchData.Text;
                    int TotalProcessedThisIteration = 0;
                    while( TotalProcessedThisIteration <= BatchData.ContainerLocationsProcessedPerIteration && false == BatchData.ContainerLocationIds.IsEmpty )
                    {
                        _executeReconciliationActions( CswConvert.ToPrimaryKey( CswConvert.ToString( BatchData.ContainerLocationIds[0] ) ) );
                        BatchData.ContainerLocationIds.RemoveAt( 0 );
                        TotalProcessedThisIteration++;
                    }
                    if( BatchData.ContainerLocationIds.IsEmpty )
                    {
                        BatchNode.finish();
                    }
                    BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    BatchNode.BatchData.Text = BatchData.ToString();
                    BatchNode.postChanges( false );
                }
            }
            catch( Exception ex )
            {
                if( BatchNode != null ) BatchNode.error( ex );
            }
        }

        #endregion BatchOp Methods

        #region BatchOp-Specific Logic

        private void _executeReconciliationActions( CswPrimaryKey ContainerLocationId )
        {
            CswNbtObjClassContainerLocation ContainerLocation = _CswNbtResources.Nodes[ContainerLocationId];
            if( null != ContainerLocation )
            {
                if( _isMostRecentContainerLocation( ContainerLocation ) )
                {
                    _executeReconciliationAction( ContainerLocation );
                }
                ContainerLocation.ActionApplied.Checked = Tristate.True;
                ContainerLocation.postChanges( false );
            }
        }

        private bool _isMostRecentContainerLocation( CswNbtObjClassContainerLocation ContainerLocation )
        {
            bool isMostRecent = true;
            CswNbtView ContainerLocationsView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass ContainerLocationOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            CswNbtViewRelationship ParentRelationship = ContainerLocationsView.AddViewRelationship( ContainerLocationOc, true );
            ParentRelationship.NodeIdsToFilterOut.Add( ContainerLocation.NodeId );
            CswNbtMetaDataObjectClassProp ContainerOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ContainerOcp,
                Value: ContainerLocation.Container.RelatedNodeId.PrimaryKey.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
            CswNbtMetaDataObjectClassProp ScanDateOcp = ContainerLocationOc.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );
            ContainerLocationsView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: ScanDateOcp,
                Value: ContainerLocation.ScanDate.DateTimeValue.ToString(),
                SubFieldName: CswNbtSubField.SubFieldName.Value,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.GreaterThan );
            ICswNbtTree ContainerLocationsTree = _CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            if( ContainerLocationsTree.getChildNodeCount() > 0 )
            {
                isMostRecent = false;
            }
            return isMostRecent;
        }

        private void _executeReconciliationAction( CswNbtObjClassContainerLocation ContainerLocation )
        {
            CswNbtObjClassContainer Container = _CswNbtResources.Nodes[ContainerLocation.Container.RelatedNodeId];
            if( null != Container )
            {
                CswNbtObjClassContainerLocation.ActionOptions Action = ContainerLocation.Action.Value;
                if( Action == CswNbtObjClassContainerLocation.ActionOptions.Undispose ||
                    Action == CswNbtObjClassContainerLocation.ActionOptions.UndisposeAndMove )
                {
                    Container.UndisposeContainer();
                }
                if( Action == CswNbtObjClassContainerLocation.ActionOptions.MoveToLocation ||
                    Action == CswNbtObjClassContainerLocation.ActionOptions.UndisposeAndMove )
                {
                    Container.Location.SelectedNodeId = ContainerLocation.Location.SelectedNodeId;
                    Container.Location.RefreshNodeName();
                }
                Container.Missing.Checked = Tristate.False;
                Container.postChanges( false );
            }
        }

        #endregion BatchOp-Specific Logic

        #region ContainerReconciliationActionsBatchData

        private class ContainerReconciliationActionsBatchData
        {
            public int ContainerLocationsProcessedPerIteration;
            public CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            public int TotalContainerLocations;

            public ContainerReconciliationActionsBatchData( CswCommaDelimitedString ContainerLocationIdsIn, int ContainerLocationsProcessedPerIterationIn )
            {
                this.ContainerLocationsProcessedPerIteration = ContainerLocationsProcessedPerIterationIn;
                this.ContainerLocationIds = ContainerLocationIdsIn;
                this.TotalContainerLocations = ContainerLocationIdsIn.Count;
            }

            public ContainerReconciliationActionsBatchData( CswCommaDelimitedString ContainerLocationIdsIn, int ContainerLocationsProcessedPerIterationIn, int TotalContainerLocationsIn )
            {
                this.ContainerLocationsProcessedPerIteration = ContainerLocationsProcessedPerIterationIn;
                this.ContainerLocationIds = ContainerLocationIdsIn;
                this.TotalContainerLocations = TotalContainerLocationsIn;
            }

            public static implicit operator ContainerReconciliationActionsBatchData( string item )
            {
                JObject Obj = CswConvert.ToJObject( item );
                CswCommaDelimitedString ContainerLocationIdsFromItem = new CswCommaDelimitedString();
                ContainerLocationIdsFromItem.FromString( Obj["ContainerLocationIds"].ToString() );
                int total = CswConvert.ToInt32( Obj["TotalContainerLocations"] );
                int ContainersProcessedPerIterationFromItem = CswConvert.ToInt32( Obj["ContainerLocationsProcessedPerIteration"] );
                return new ContainerReconciliationActionsBatchData( ContainerLocationIdsFromItem, ContainersProcessedPerIterationFromItem, total );
            }

            public static implicit operator string( ContainerReconciliationActionsBatchData item )
            {
                return item.ToString();
            }

            public override string ToString()
            {
                JObject Obj = new JObject();
                Obj["ContainerLocationsProcessedPerIteration"] = ContainerLocationsProcessedPerIteration.ToString();
                Obj["TotalContainerLocations"] = TotalContainerLocations.ToString();
                Obj["ContainerLocationIds"] = ContainerLocationIds.ToString();
                return Obj.ToString();
            }
        }

        #endregion ContainerReconciliationActionsBatchData

    } // class CswNbtBatchOpContainerReconciliationActions
} // namespace ChemSW.Nbt.Batch
