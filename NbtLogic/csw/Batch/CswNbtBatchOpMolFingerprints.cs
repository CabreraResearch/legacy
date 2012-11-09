using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.StructureSearch;
using ChemSW.Nbt.PropTypes;

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
        /// Create a new batch operation to update the mols with no fingerprints
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswCommaDelimitedString nodeIds, int nodesPerIteration )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( false == nodeIds.IsEmpty )
            {
                MolFingerprintBatchData BatchData = new MolFingerprintBatchData( nodeIds, nodesPerIteration );
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
                        BatchData.nodeIds.RemoveAt( 0 );

                        CswPrimaryKey pk = new CswPrimaryKey( "nodes", nodeId );
                        CswNbtNode node = _CswNbtResources.Nodes.GetNode( pk );

                        bool hasntBeenInserted = true;
                        foreach( CswNbtNodePropWrapper prop in node.Properties[(CswNbtMetaDataFieldType.NbtFieldType) CswNbtMetaDataFieldType.NbtFieldType.MOL] )
                        {
                            if( hasntBeenInserted )
                            {
                                _CswNbtResources.StructureSearchManager.InsertFingerprintRecord( nodeId, prop.AsMol.Mol );
                                hasntBeenInserted = false;
                            }
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
            public int nodesPerIteration;
            public int totalNodes;

            public MolFingerprintBatchData( CswCommaDelimitedString nodeIds, int nodesPerIteration )
            {
                this.nodeIds = nodeIds;
                this.nodesPerIteration = nodesPerIteration;
                totalNodes = nodeIds.Count;
            }

            public MolFingerprintBatchData( CswCommaDelimitedString nodeIds, int nodesPerIteration, int totalNodes )
            {
                this.nodeIds = nodeIds;
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

                return new MolFingerprintBatchData( nodeIds, nodesPerIteration, totalNodes );
            }

            public static implicit operator string( MolFingerprintBatchData item )
            {
                return item.ToString();
            }

            public override string ToString()
            {
                JObject Obj = new JObject();
                Obj["nodeIds"] = nodeIds.ToString();
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
