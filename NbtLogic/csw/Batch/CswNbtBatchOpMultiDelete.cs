using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMultiDelete : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.MultiDelete;

        public CswNbtBatchOpMultiDelete( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle a deleteNodes/multi edit operation
        /// </summary>
        /// <param name="DeleteNodeIds"></param>
        public CswNbtObjClassBatchOp makeBatchOp( Collection<CswPrimaryKey> DeleteNodeIds )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            MultiDeleteBatchData BatchData = new MultiDeleteBatchData( string.Empty );
            BatchData.DeleteNodeIds = _pkArrayToJArray( DeleteNodeIds );
            BatchData.StartingCount = DeleteNodeIds.Count();
            BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            return BatchNode;
        }

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MultiDelete )
            {
                MultiDeleteBatchData BatchData = new MultiDeleteBatchData( BatchNode.BatchData.Text );
                if( BatchData.StartingCount > 0 )
                {
                    ret = Math.Round( (Double) ( BatchData.StartingCount - BatchData.DeleteNodeIds.Count() ) / BatchData.StartingCount * 100, 0 );
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
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MultiDelete )
                {
                    BatchNode.start();
                    MultiDeleteBatchData BatchData = new MultiDeleteBatchData( BatchNode.BatchData.Text );
                    if( BatchData.DeleteNodeIds.Count > 0 )
                    {
                        string NodeIdStr = BatchData.DeleteNodeIds.First.ToString();
                        _deleteNode( NodeIdStr );

                        // Setup for next iteration
                        BatchData.DeleteNodeIds.RemoveAt( 0 );
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
        }

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

        private void _deleteNode( string NodePk )
        {
            CswPrimaryKey DeleteNodePk = new CswPrimaryKey();
            DeleteNodePk.FromString( NodePk );

            if( Int32.MinValue != DeleteNodePk.PrimaryKey )
            {
                CswNbtNode DeleteNode = _CswNbtResources.Nodes[DeleteNodePk];
                if( DeleteNode != null )
                {
                    DeleteNode.delete();
                }
            }
        }

        #endregion

        #region MultiDeleteBatchData

        private class MultiDeleteBatchData
        {
            private JObject _BatchData;

            public MultiDeleteBatchData( string BatchData )
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

            private JArray _DeleteNodeIds = null;
            public JArray DeleteNodeIds
            {
                get
                {
                    if( null == _DeleteNodeIds )
                    {
                        if( null != _BatchData["deletenodeids"] )
                        {
                            _DeleteNodeIds = (JArray) _BatchData["deletenodeids"];
                        }
                    }
                    return _DeleteNodeIds;
                }
                set
                {
                    _DeleteNodeIds = value;
                    _BatchData["deletenodeids"] = _DeleteNodeIds;
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
        }

        #endregion MultiDeleteBatchData
    }
}
