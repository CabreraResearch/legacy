using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMultiEdit : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.MultiEdit;

        public CswNbtBatchOpMultiEdit( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle a copyNodeProps/multi edit operation
        /// </summary>
        /// <param name="GeneratorNodeId">Primary key of Generator</param>
        public CswNbtObjClassBatchOp makeBatchOp( CswNbtNode SourceNode, string[] CopyNodeIds, Collection<Int32> NodeTypePropIds )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( null != SourceNode )
            {
                MultiEditBatchData BatchData = new MultiEditBatchData( string.Empty );
                BatchData.SourceNodeId = SourceNode.NodeId;
                BatchData.CopyNodeIds = stringArrayToJArray( CopyNodeIds );
                BatchData.NodeTypePropIds = Int32CollectionToJArray( NodeTypePropIds );

                BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            } // if(null != SourceNode)
            return BatchNode;
        } // makeBatchOp()

        private JArray stringArrayToJArray( string[] strArray )
        {
            JArray ret = new JArray();
            foreach( string str in strArray )
            {
                ret.Add( str );
            }
            return ret;
        } // stringArrayToJArray

        private JArray Int32CollectionToJArray( Collection<Int32> intColl )
        {
            JArray ret = new JArray();
            foreach( Int32 i in intColl )
            {
                ret.Add( i.ToString() );
            }
            return ret;
        } // Int32CollectionToJArray


        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                BatchNode.start();

                MultiEditBatchData BatchData = new MultiEditBatchData( BatchNode.BatchData.Text );
                if( BatchData.SourceNodeId != null && BatchData.CopyNodeIds.Count > 0 && BatchData.NodeTypePropIds.Count > 0 )
                {
                    CswNbtNode SourceNode = _CswNbtResources.Nodes[BatchData.SourceNodeId];
                    if( SourceNode != null )
                    {
                        string NodeIdStr = BatchData.CopyNodeIds.First.ToString();

                        CswPrimaryKey CopyToNodePk = new CswPrimaryKey();
                        CopyToNodePk.FromString( NodeIdStr );

                        if( Int32.MinValue != CopyToNodePk.PrimaryKey )
                        {
                            CswNbtNode CopyToNode = _CswNbtResources.Nodes[CopyToNodePk];
                            if( CopyToNode != null &&
                                _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, CopyToNode.getNodeType(), false, null, null, CopyToNode.NodeId, null ) )
                            {
                                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in BatchData.NodeTypePropIds.Select( PropId => _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) ) ) )
                                {
                                    CopyToNode.Properties[NodeTypeProp].copy( SourceNode.Properties[NodeTypeProp] );
                                }

                                CopyToNode.postChanges( false );

                                BatchNode.appendToLog( "Copied values for: " + CopyToNode.NodeName + " (" + CopyToNode.NodeId.PrimaryKey.ToString() + ")" );

                            } // if( CopyToNode != null )
                        } // if( Int32.MinValue != CopyToNodePk.PrimaryKey )

                        // Setup for next iteration
                        BatchData.CopyNodeIds.RemoveAt( 0 );
                        BatchNode.BatchData.Text = BatchData.ToString();

                    } // if( SourceNode != null )
                    else
                    {
                        BatchNode.finish();
                    }
                }
                else
                {
                    BatchNode.finish();
                }

                BatchNode.postChanges( false );
            }
            catch( Exception ex )
            {
                BatchNode.error( ex );
            }
        } // runBatchOp()


        #region MultiEditBatchData

        // This internal class is specific to this batch operation
        private class MultiEditBatchData
        {
            private JObject _BatchData;

            public MultiEditBatchData( string BatchData )
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

            private CswPrimaryKey _SourceNodeId = null;
            public CswPrimaryKey SourceNodeId
            {
                get
                {
                    if( null == _SourceNodeId )
                    {
                        if( null != _BatchData["sourcenodeid"] )
                        {
                            _SourceNodeId = new CswPrimaryKey();
                            _SourceNodeId.FromString( _BatchData["sourcenodeid"].ToString() );
                        }
                    }
                    return _SourceNodeId;
                }
                set
                {
                    _SourceNodeId = value;
                    _BatchData["sourcenodeid"] = _SourceNodeId.ToString();
                }
            }

            private JArray _CopyNodeIds = null;
            public JArray CopyNodeIds
            {
                get
                {
                    if( null == _CopyNodeIds )
                    {
                        if( null != _BatchData["copynodeids"] )
                        {
                            _CopyNodeIds = (JArray) _BatchData["copynodeids"];
                        }
                    }
                    return _CopyNodeIds;
                }
                set
                {
                    _CopyNodeIds = value;
                    _BatchData["copynodeids"] = _CopyNodeIds;
                }
            }
            private JArray _NodeTypePropIds = null;
            public JArray NodeTypePropIds
            {
                get
                {
                    if( null == _NodeTypePropIds )
                    {
                        if( null != _BatchData["nodetypepropids"] )
                        {
                            _NodeTypePropIds = (JArray) _BatchData["nodetypepropids"];
                        }
                    }
                    return _NodeTypePropIds;
                }
                set
                {
                    _NodeTypePropIds = value;
                    _BatchData["nodetypepropids"] = _NodeTypePropIds;
                }
            }

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        } // class MultiEditBatchData

        #endregion MultiEditBatchData


    } // class CswNbtBatchOpMultiEdit
} // namespace ChemSW.Nbt.Batch
