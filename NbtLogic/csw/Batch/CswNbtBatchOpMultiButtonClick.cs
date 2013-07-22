using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMultiButtonClick : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private CswEnumNbtBatchOpName _BatchOpName = CswEnumNbtBatchOpName.MultiButtonClick;

        public CswNbtBatchOpMultiButtonClick( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle clicking an objectclass button for multiple nodes
        /// </summary>
        /// <param name="MultiNodeIds">NodeIds with which ObjectClassButtonClick logic should be applied</param>
        /// <param name="NodeTypePropId">PropId of the ObjectClassButton</param>
        public CswNbtObjClassBatchOp makeBatchOp( Collection<CswPrimaryKey> MultiNodeIds, Int32 NodeTypePropId )
        {
            MultiButtonClickBatchData BatchData = new MultiButtonClickBatchData( string.Empty );
            BatchData.MultiNodeIds = _pkArrayToJArray( MultiNodeIds );
            BatchData.StartingCount = MultiNodeIds.Count();
            BatchData.NodeTypePropId = NodeTypePropId;

            CswNbtObjClassBatchOp BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            return BatchNode;
        }

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.MultiButtonClick )
            {
                MultiButtonClickBatchData BatchData = new MultiButtonClickBatchData( BatchNode.BatchData.Text );
                if( BatchData.StartingCount > 0 )
                {
                    ret = Math.Round( (Double) ( BatchData.StartingCount - BatchData.MultiNodeIds.Count() ) / BatchData.StartingCount * 100, 0 );
                }
            }
            return ret;
        }

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.MultiButtonClick )
                {
                    BatchNode.start();

                    MultiButtonClickBatchData BatchData = new MultiButtonClickBatchData( BatchNode.BatchData.Text );
                    if( BatchData.MultiNodeIds.Count > 0 )
                    {
                        String NodeIdStr = BatchData.MultiNodeIds.First.ToString();
                        CswPrimaryKey MultiNodePk = new CswPrimaryKey();
                        MultiNodePk.FromString( NodeIdStr );

                        CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( BatchData.NodeTypePropId );
                        if( Int32.MinValue != MultiNodePk.PrimaryKey && null != NodeTypeProp )
                        {
                            CswNbtObjClass.NbtButtonData ButtonData = new CswNbtObjClass.NbtButtonData( NodeTypeProp );
                            CswNbtNode MultiNode = _CswNbtResources.Nodes[MultiNodePk];
                            if( MultiNode != null )
                            {
                                MultiNode.ObjClass.triggerOnButtonClick( ButtonData );
                            }
                        }
                        // Setup for next iteration
                        BatchData.MultiNodeIds.RemoveAt( 0 );
                        BatchNode.BatchData.Text = BatchData.ToString();
                        BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    }
                    else
                    {
                        BatchNode.finish();
                    }
                    BatchNode.postChanges( false );
                }
            }
            catch( Exception ex )
            {
                BatchNode.error( ex );
            }
        } // runBatchOp()

        #region Private Helper Functions

        private JArray _pkArrayToJArray( Collection<CswPrimaryKey> strArray )
        {
            JArray ret = new JArray();
            foreach( CswPrimaryKey k in strArray )
            {
                ret.Add( k.ToString() );
            }
            return ret;
        }

        #endregion Private Helper Functions

        #region MultiButtonClickBatchData

        private class MultiButtonClickBatchData
        {
            private JObject _BatchData;

            public MultiButtonClickBatchData( string BatchData )
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

            private JArray _MultiNodeIds;
            public JArray MultiNodeIds
            {
                get
                {
                    if( null == _MultiNodeIds && null != _BatchData["copynodeids"])
                    {
                        _MultiNodeIds = (JArray) _BatchData["copynodeids"];
                    }
                    return _MultiNodeIds;
                }
                set
                {
                    _MultiNodeIds = value;
                    _BatchData["copynodeids"] = _MultiNodeIds;
                }
            }

            private Int32 _StartingCount = Int32.MinValue;
            public Int32 StartingCount
            {
                get
                {
                    if( Int32.MinValue == _StartingCount && null != _BatchData["startingcount"])
                    {
                        _StartingCount = CswConvert.ToInt32( _BatchData["startingcount"].ToString() );
                    }
                    return _StartingCount;
                }
                set
                {
                    _StartingCount = value;
                    _BatchData["startingcount"] = _StartingCount;
                }
            }

            private Int32 _NodeTypePropId = Int32.MinValue;
            public Int32 NodeTypePropId
            {
                get
                {
                    if( Int32.MinValue == _NodeTypePropId && null != _BatchData["nodetypepropid"] )
                    {
                        _NodeTypePropId = CswConvert.ToInt32( _BatchData["nodetypepropid"] );
                    }
                    return _NodeTypePropId;
                }
                set
                {
                    _NodeTypePropId = value;
                    _BatchData["nodetypepropid"] = _NodeTypePropId;
                }
            }

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        }

        #endregion MultiButtonClickBatchData

    } // class CswNbtBatchOpMultiButtonClick
} // namespace ChemSW.Nbt.Batch
