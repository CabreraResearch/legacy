using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.StructureSearch;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMolFingerprints : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.MolFingerprints;

        public CswNbtBatchOpMolFingerprints( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Returns the percentage of the task that is complete
        /// </summary>
        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 0;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MolFingerprints )
            {
                MolFingerprintBatchData BatchData = BatchNode.BatchData.Text;
                return ( ( (double) ( BatchData.totalNodes - BatchData.nodeIds.Count ) / BatchData.totalNodes ) * 100 );
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Create a new batch operation to update materials regulatory lists property
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswCommaDelimitedString nodeIds, CswCommaDelimitedString propIds, int nodesPerIteration )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( false == nodeIds.IsEmpty && false == propIds.IsEmpty )
            {
                MolFingerprintBatchData BatchData = new MolFingerprintBatchData( nodeIds, propIds, nodesPerIteration );
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
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MolFingerprints )
                {
                    BatchNode.start();
                    MolFingerprintBatchData BatchData = BatchNode.BatchData.Text;

                    int processedThisIteration = 0;
                    while( processedThisIteration <= BatchData.nodesPerIteration && false == BatchData.nodeIds.IsEmpty ) //terminate if we've reached the config limit or if there are no more nodes to update
                    {
                        int nodeId = CswConvert.ToInt32( BatchData.nodeIds[0] );
                        int propId = CswConvert.ToInt32( BatchData.propIds[0] );
                        BatchData.nodeIds.RemoveAt( 0 );
                        BatchData.propIds.RemoveAt( 0 );

                        CswNbtMetaDataNodeTypeProp molNTP = _CswNbtResources.MetaData.getNodeTypeProp( propId );
                        if( null != molNTP )
                        {
                            CswPrimaryKey pk = new CswPrimaryKey( "nodes", nodeId );
                            CswNbtNode node = _CswNbtResources.Nodes.GetNode( pk );
                            _CswNbtResources.StructureSearchManager.InsertFingerprintRecord( nodeId, node.Properties[molNTP].AsMol.Mol );
                        }
                        processedThisIteration++;
                    }

                    BatchNode.BatchData.Text = BatchData.ToString();
                    BatchNode.PercentDone.Value = getPercentDone( BatchNode );

                    if( BatchData.nodeIds.IsEmpty )
                    {
                        BatchNode.finish();
                    }
                    BatchNode.postChanges( false );
                }
            }
            catch( Exception ex )
            {
                if( BatchNode != null ) BatchNode.error( ex );
            }
        } // runBatchOp()

        #region MolFingerprintBatchData

        // This internal class is specific to this batch operation
        private class MolFingerprintBatchData
        {
            public CswCommaDelimitedString nodeIds = new CswCommaDelimitedString();
            public CswCommaDelimitedString propIds = new CswCommaDelimitedString();
            public int nodesPerIteration;
            public int totalNodes;

            public MolFingerprintBatchData( CswCommaDelimitedString nodeIds, CswCommaDelimitedString propIds, int nodesPerIteration )
            {
                this.nodeIds = nodeIds;
                this.propIds = propIds;
                this.nodesPerIteration = nodesPerIteration;
                totalNodes = nodeIds.Count;
            }

            public MolFingerprintBatchData( CswCommaDelimitedString nodeIds, CswCommaDelimitedString propIds, int nodesPerIteration, int totalNodes )
            {
                this.nodeIds = nodeIds;
                this.propIds = propIds;
                this.nodesPerIteration = nodesPerIteration;
                this.totalNodes = totalNodes;
            }

            public static implicit operator MolFingerprintBatchData( string item )
            {
                JObject Obj = CswConvert.ToJObject( item );

                int totalNodes = CswConvert.ToInt32( Obj["totalNodes"] );
                int nodesPerIteration = CswConvert.ToInt32( Obj["nodesPerIteration"] );

                CswCommaDelimitedString nodeIds = new CswCommaDelimitedString();
                nodeIds.FromString( Obj["nodeIds"].ToString() );

                CswCommaDelimitedString propIds = new CswCommaDelimitedString();
                propIds.FromString( Obj["propIds"].ToString() );

                return new MolFingerprintBatchData( nodeIds, propIds, nodesPerIteration, totalNodes );
            }

            public static implicit operator string( MolFingerprintBatchData item )
            {
                return item.ToString();
            }

            public override string ToString()
            {
                JObject Obj = new JObject();
                Obj["nodeIds"] = nodeIds.ToString();
                Obj["propIds"] = propIds.ToString();
                Obj["nodesPerIteration"] = nodesPerIteration.ToString();
                Obj["totalNodes"] = totalNodes.ToString();
                return Obj.ToString();
            }


        } // class InventoryLevelsBatchData

        #endregion

        #region private helper functions


        #endregion

    } // class CswNbtBatchOpUpdateRegulatoryLists
} // namespace ChemSW.Nbt.Batch
