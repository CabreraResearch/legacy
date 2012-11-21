using System;
using System.Collections.ObjectModel;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpSyncLocation : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.SyncLocation;

        public CswNbtBatchOpSyncLocation( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle a copyNodeProps/multi edit operation
        /// </summary>
        /// <param name="NodePks"></param>
        /// <param name="CGLocationId"></param>
        /// <returns></returns>
        public CswNbtObjClassBatchOp makeBatchOp( Collection<CswPrimaryKey> NodePks, CswPrimaryKey LocationId )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            SyncLocationBatchData BatchData = new SyncLocationBatchData( string.Empty );
            BatchData.LocationId = LocationId;
            BatchData.StartingCount = NodePks.Count;
            BatchData.NodePks = _pkArrayToJArray( NodePks );

            BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            return BatchNode;
        } // makeBatchOp()

        private JArray _pkArrayToJArray( Collection<CswPrimaryKey> PkArray )
        {
            JArray ReturnVal = new JArray();
            foreach( CswPrimaryKey PK in PkArray )
            {
                ReturnVal.Add( PK.ToString() );
            }
            return ReturnVal;
        } //_pkArrayToJArray()

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.SyncLocation )
            {
                SyncLocationBatchData BatchData = new SyncLocationBatchData( BatchNode.BatchData.Text );
                if( BatchData.StartingCount > 0 )
                {
                    ret = Math.Round( (Double) ( BatchData.StartingCount - BatchData.NodePks.Count ) / BatchData.StartingCount * 100, 0 );
                }
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {

            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.SyncLocation )
                {
                    BatchNode.start();

                    SyncLocationBatchData BatchData = new SyncLocationBatchData( BatchNode.BatchData.Text );

                    if( BatchData.NodePks.Count > 0 )
                    {
                        int NodesProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                        for( int i = 0; i < NodesProcessedPerIteration && BatchData.NodePks.Count > 0; i++ )
                        {
                            CswNbtNode Node = _CswNbtResources.Nodes[CswConvert.ToString( BatchData.NodePks[0] )];
                            BatchData.NodePks.RemoveAt( 0 );
                            bool LocationUpdated = false;
                            if( null != Node )
                            {
                                CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                                if( null != NodeType )
                                {
                                    CswNbtMetaDataNodeTypeProp LocationNTP = NodeType.getLocationProperty();
                                    if( null != LocationNTP )
                                    {
                                        CswNbtNodePropLocation LocationProp = Node.Properties[LocationNTP];
                                        if( null != LocationProp )
                                        {
                                            LocationProp.SelectedNodeId = BatchData.LocationId;
                                            Node.postChanges( false );
                                            LocationUpdated = true;
                                        }
                                    }
                                }
                            }//if( null != Node )

                            if( false == LocationUpdated )
                            {
                                BatchNode.appendToLog( "Unable to update the location property of the following node: " + Node.NodeId );
                            }

                        }//for( int i = 0; i < NodesProcessedPerIteration && BatchData.NodePks.Count > 0; i++ )
                    }
                    else
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
                BatchNode.error( ex );
            }

        } // runBatchOp()

        #region SyncLocationBatchData

        // This internal class is specific to this batch operation
        private class SyncLocationBatchData
        {
            private JObject _BatchData;

            public SyncLocationBatchData( string BatchData )
            {
                if( BatchData != string.Empty )
                {
                    _BatchData = JObject.Parse( BatchData );
                }
                else
                {
                    _BatchData = new JObject();
                }
            }

            private CswPrimaryKey _LocationId = null;
            public CswPrimaryKey LocationId
            {
                get
                {
                    if( null == _LocationId )
                    {
                        if( null != _BatchData["locationid"] )
                        {
                            _LocationId = new CswPrimaryKey();
                            _LocationId.FromString( _BatchData["locationid"].ToString() );
                        }
                    }
                    return _LocationId;
                }
                set
                {
                    _LocationId = value;
                    _BatchData["locationid"] = _LocationId.ToString();
                }
            }

            private JArray _NodePks = null;
            public JArray NodePks
            {
                get
                {
                    if( null == _NodePks )
                    {
                        if( null != _BatchData["nodepks"] )
                        {
                            _NodePks = (JArray) _BatchData["nodepks"];
                        }
                    }
                    return _NodePks;
                }
                set
                {
                    _NodePks = value;
                    _BatchData["nodepks"] = _NodePks;
                }
            }

            private Int32 _StartingCount = Int32.MinValue;
            public Int32 StartingCount
            {
                get
                {
                    if( Int32.MinValue == _StartingCount )
                    {
                        if( null != _BatchData["startingcount"] )
                        {
                            _StartingCount = CswConvert.ToInt32( _BatchData["startingcount"].ToString() );
                        }
                    }
                    return _StartingCount;
                }
                set
                {
                    _StartingCount = value;
                    _BatchData["startingcount"] = _StartingCount;
                }
            }

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        } // class SyncLocationBatchData

        #endregion SyncLocationBatchData

    } // class CswNbtBatchOpSyncLocation
} // namespace ChemSW.Nbt.Batch
