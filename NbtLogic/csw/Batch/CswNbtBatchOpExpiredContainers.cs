using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpExpiredContainers : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.ExpiredContainers;

        public CswNbtBatchOpExpiredContainers( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Returns the percentage of the task that is complete
        /// </summary>
        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 0;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.ExpiredContainers )
            {
                ExpiredContainerBatchData BatchData = BatchNode.BatchData.Text;
                return ( ( BatchData.expiredContainerIDs.Count / BatchData.totalExpiredContainers ) * 100 );
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Create a new batch operation to update materials regulatory lists property
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswCommaDelimitedString ExpiredContainerIds, int totalContainerProcessedPerIteration )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( false == ExpiredContainerIds.IsEmpty )
            {
                ExpiredContainerBatchData BatchData = new ExpiredContainerBatchData( ExpiredContainerIds, totalContainerProcessedPerIteration );
                BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            }
            return BatchNode;
        } // makeBatchOp()

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.ExpiredContainers )
                {
                    BatchNode.start();
                    ExpiredContainerBatchData BatchData = BatchNode.BatchData.Text;

                    int totalProcessedThisIteration = 0;
                    //loop until we've hit the allowed limit of containers processed per iteration or there are no more containers to update
                    while( totalProcessedThisIteration <= BatchData.ContainersProcessedPerIteration && false == BatchData.expiredContainerIDs.IsEmpty )
                    {
                        CswPrimaryKey pk = new CswPrimaryKey();
                        pk.FromString( BatchData.expiredContainerIDs[0].ToString() );
                        BatchData.expiredContainerIDs.RemoveAt( 0 );
                        CswNbtObjClassContainer expiredContainer = _CswNbtResources.Nodes.GetNode( pk );
                        expiredContainer.Status.Value = CswNbtObjClassContainer.Statuses.Expired;
                        expiredContainer.postChanges( false );
                        totalProcessedThisIteration++;
                    }

                    if( BatchData.expiredContainerIDs.IsEmpty )
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
        } // runBatchOp()

        #region ExpiredContainerBatchData

        // This internal class is specific to this batch operation
        private class ExpiredContainerBatchData
        {
            public int ContainersProcessedPerIteration; //how many containers/receipt lots are processed per iteration of the batch op
            public CswCommaDelimitedString expiredContainerIDs = new CswCommaDelimitedString();
            public int totalExpiredContainers;

            public ExpiredContainerBatchData( CswCommaDelimitedString expiredContainerIDsIn, int ContainersProcessedPerIterationIn )
            {
                this.ContainersProcessedPerIteration = ContainersProcessedPerIterationIn;
                this.expiredContainerIDs = expiredContainerIDsIn;
                this.totalExpiredContainers = expiredContainerIDsIn.Count;
            }

            public ExpiredContainerBatchData( CswCommaDelimitedString expiredContainerIDsIn, int ContainersProcessedPerIterationIn, int totalContainersIn )
            {
                this.ContainersProcessedPerIteration = ContainersProcessedPerIterationIn;
                this.expiredContainerIDs = expiredContainerIDsIn;
                this.totalExpiredContainers = totalContainersIn;
            }

            public static implicit operator ExpiredContainerBatchData( string item )
            {
                JObject Obj = CswConvert.ToJObject( item );

                CswCommaDelimitedString expiredContainerIDsFromItem = new CswCommaDelimitedString();
                expiredContainerIDsFromItem.FromString( Obj["expiredContainerIDs"].ToString() );

                int total = CswConvert.ToInt32( Obj["totalExpiredContainers"] );

                int ContainersProcessedPerIterationFromItem = CswConvert.ToInt32( Obj["ContainersProcessedPerIteration"] );

                return new ExpiredContainerBatchData( expiredContainerIDsFromItem, ContainersProcessedPerIterationFromItem, total );
            }

            public static implicit operator string( ExpiredContainerBatchData item )
            {
                return item.ToString();
            }

            public override string ToString()
            {
                JObject Obj = new JObject();
                Obj["ContainersProcessedPerIteration"] = ContainersProcessedPerIteration.ToString();
                Obj["totalExpiredContainers"] = totalExpiredContainers.ToString();
                Obj["expiredContainerIDs"] = expiredContainerIDs.ToString();
                return Obj.ToString();
            }


        } // class InventoryLevelsBatchData

        #endregion

        #region private helper functions


        #endregion

    } // class CswNbtBatchOpUpdateRegulatoryLists
} // namespace ChemSW.Nbt.Batch
