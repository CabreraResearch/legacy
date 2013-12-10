using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActMerge
    {
        private CswNbtResources _CswNbtResources = null;

        public CswNbtActMerge( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }


        #region WCF

        [DataContract]
        public class MergeInfoData
        {
            public MergeInfoData()
            {
                NodePairs = new Collection<MergeInfoNodePair>();
            }

            [DataMember]
            public Collection<MergeInfoNodePair> NodePairs;

            /// <summary>
            /// Pair of nodes to merge
            /// </summary>
            [DataContract]
            public class MergeInfoNodePair
            {
                public MergeInfoNodePair()
                {
                    Properties = new Collection<MergeInfoProperty>();
                }

                [DataMember]
                public Collection<MergeInfoProperty> Properties;
                [DataMember]
                public Collection<MergeInfoNodeReference> NodeReferences;
                [DataMember]
                public Int32 NodeReferencePropId;
                [DataMember]
                public string NodeTypeName;
                [DataMember]
                public string Node1Id;
                [DataMember]
                public string Node1Name;
                [DataMember]
                public string Node2Id;
                [DataMember]
                public string Node2Name;
                [DataMember]
                public string NodeTempId;
                [DataMember]
                public string NodeTempName;
            }

            /// <summary>
            /// Conflicting property value to choose between
            /// </summary>
            [DataContract]
            public class MergeInfoProperty
            {
                [DataMember]
                public string PropName;
                [DataMember]
                public Int32 NodeTypePropId;
                [DataMember]
                public string Node1Value;
                [DataMember]
                public string Node2Value;
                [DataMember]
                public Int32 Choice = 2;
            }

            /// <summary>
            /// Reference data value to update (no conflicts)
            /// </summary>
            [DataContract]
            public class MergeInfoNodeReference
            {
                [DataMember]
                public string NodeId;
                [DataMember]
                public Int32 NodeTypePropId;
            }
        }

        #endregion WCF

        public MergeInfoData getMergeInfo( CswPrimaryKey NodeId1, CswPrimaryKey NodeId2 )
        {
            MergeInfoData ret = new MergeInfoData();

            // Set up the requested merge and all other required merges (recursively)
            _addMergeNodes( ret, NodeId1, NodeId2 );

            return ret;
        } // getMergeInfo()


        private void _addMergeNodes( MergeInfoData ret, CswPrimaryKey NodeId1, CswPrimaryKey NodeId2, CswNbtMetaDataNodeTypeProp NodeReferenceProp = null )
        {
            CswNbtNode Node1 = _CswNbtResources.Nodes[NodeId1];
            CswNbtNode Node2 = _CswNbtResources.Nodes[NodeId2];
            if( null != Node1 && null != Node2 )
            {
                // Make sure there isn't already a node pair for these nodes
                if( false == ret.NodePairs.Any( np => ( np.Node1Id == Node1.NodeId.ToString() && np.Node2Id == Node2.NodeId.ToString() ) ||
                                                      ( np.Node1Id == Node2.NodeId.ToString() && np.Node2Id == Node1.NodeId.ToString() ) ) )
                {
                    CswNbtMetaDataNodeType Node1NT = Node1.getNodeType();

                    MergeInfoData.MergeInfoNodePair NodePair = new MergeInfoData.MergeInfoNodePair();
                    NodePair.NodeReferences = new Collection<MergeInfoData.MergeInfoNodeReference>();
                    NodePair.NodeTypeName = Node1NT.NodeTypeName;
                    NodePair.Node1Id = Node1.NodeId.ToString();
                    NodePair.Node2Id = Node2.NodeId.ToString();
                    NodePair.Node1Name = Node1.NodeName;
                    NodePair.Node2Name = Node2.NodeName;
                    if( null != NodeReferenceProp )
                    {
                        NodePair.NodeReferencePropId = NodeReferenceProp.PropId;
                    }
                    else
                    {
                        NodePair.NodeReferencePropId = Int32.MinValue;
                    }

                    foreach( CswNbtNodePropWrapper Prop1 in Node1.Properties.Where( p => ( null == NodeReferenceProp || p.NodeTypePropId != NodeReferenceProp.PropId ) ) )
                    {
                        CswNbtNodePropWrapper Prop2 = Node2.Properties[Prop1.NodeTypePropId];
                        if( null == Prop2 || Prop1.Gestalt != Prop2.Gestalt )
                        {
                            NodePair.Properties.Add( new MergeInfoData.MergeInfoProperty()
                                {
                                    PropName = Prop1.PropName,
                                    NodeTypePropId = Prop1.NodeTypePropId,
                                    Node1Value = Prop1.Gestalt,
                                    Node2Value = Prop2.Gestalt
                                } );
                        } // if( null == Prop2 || Prop1.Gestalt != Prop2.Gestalt )
                    } // foreach( CswNbtNodePropWrapper Prop1 in Node1.Properties )
                    ret.NodePairs.Add( NodePair );


                    // If two nodes that formerly related to each of node1 and node2 now relate to the merged target,
                    // and they now would be compound unique violations, we have to merge those too.

                    // First, find references that point to the merged nodes
                    foreach( CswNbtMetaDataNodeTypeProp thisProp in _CswNbtResources.MetaData.getNodeTypeProps().Where( p => p.IsNodeReference() && p.FkMatches( Node1.getNodeType() ) ) )
                    {
                        // Find unique key sets for nodes using this reference that point to either node1 or node2
                        Dictionary<CswDelimitedString, CswPrimaryKey> Node1UniqueKeysDict = _getUniqueKeysDict( NodePair, Node1, thisProp );
                        Dictionary<CswDelimitedString, CswPrimaryKey> Node2UniqueKeysDict = _getUniqueKeysDict( NodePair, Node2, thisProp );

                        // Look for redundant keys to indicate a potential unique violation
                        foreach( CswDelimitedString key in Node1UniqueKeysDict.Keys )
                        {
                            if( Node2UniqueKeysDict.ContainsKey( key ) )
                            {
                                // unique violation!  gotta merge these too.
                                _addMergeNodes( ret, Node1UniqueKeysDict[key], Node2UniqueKeysDict[key], thisProp );
                            }
                        } // foreach( CswDelimitedString key in Node1UniqueKeysDict.Keys )
                    } // foreach( CswNbtMetaDataNodeTypeProp thisProp in _CswNbtResources.MetaData.getNodeTypeProps().Where( p => p.IsNodeReference() ) )

                    // Special case: UserSelect properties
                    if( Node1NT.getObjectClassValue() == CswEnumNbtObjectClass.UserClass )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp thisUserSelProp in _CswNbtResources.MetaData.getNodeTypeProps( CswEnumNbtFieldType.UserSelect ) )
                        {
                            // Create a view of nodes that point to either merged node via this reference
                            CswNbtView view = new CswNbtView( _CswNbtResources );
                            CswNbtViewRelationship rel1 = view.AddViewRelationship( thisUserSelProp.getNodeType(), false );
                            view.AddViewPropertyAndFilter( rel1,
                                                           thisUserSelProp,
                                                           Conjunction: CswEnumNbtFilterConjunction.And,
                                                           FilterMode: CswEnumNbtFilterMode.Contains,
                                                           Value: Node1.NodeId.PrimaryKey.ToString() );
                            view.AddViewPropertyAndFilter( rel1,
                                                           thisUserSelProp,
                                                           Conjunction: CswEnumNbtFilterConjunction.Or,
                                                           FilterMode: CswEnumNbtFilterMode.Contains,
                                                           Value: Node2.NodeId.PrimaryKey.ToString() );

                            // Add nodes with matching UserSelect properties to NodeReferences for later updating
                            ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( view, RequireViewPermissions: false, IncludeHiddenNodes: true, IncludeSystemNodes: true );
                            for( Int32 c = 0; c < tree.getChildNodeCount(); c++ )
                            {
                                tree.goToNthChild( c );
                                NodePair.NodeReferences.Add( new MergeInfoData.MergeInfoNodeReference()
                                    {
                                        NodeId = tree.getNodeIdForCurrentPosition().ToString(),
                                        NodeTypePropId = thisUserSelProp.PropId
                                    } );
                                tree.goToParentNode();
                            } // for( Int32 c = 0; c < tree.getChildNodeCount(); c++ )
                        } // foreach( CswNbtMetaDataNodeTypeProp thisUserSelProp in _CswNbtResources.MetaData.getNodeTypeProps( CswEnumNbtFieldType.UserSelect ) )
                    } // if( Node1NT.getObjectClassValue() == CswEnumNbtObjectClass.UserClass )

                } // if(false == ret.NodePairs.Any( ... ))
            } // if( null != Node1 && null != Node2 )
        } // _addMergeNodes()


        private Dictionary<CswPrimaryKey, CswDelimitedString> _AllUniqueKeys = new Dictionary<CswPrimaryKey, CswDelimitedString>();

        /// <summary>
        /// Create a dictionary of the unique key values of related nodes
        /// As a side effect, also populate MergeInfoData.NodePair.NodeReferences
        /// </summary>
        private Dictionary<CswDelimitedString, CswPrimaryKey> _getUniqueKeysDict( MergeInfoData.MergeInfoNodePair NodePair, CswNbtNode Node, CswNbtMetaDataNodeTypeProp NodeReferenceProp )
        {
            Dictionary<CswDelimitedString, CswPrimaryKey> ret = new Dictionary<CswDelimitedString, CswPrimaryKey>();
            char delimiter = '|';

            // Find unique properties for this reference's nodetype
            IEnumerable<CswNbtMetaDataNodeTypeProp> UniqueProps = NodeReferenceProp.getNodeType().getUniqueProps();

            // Create a view of nodes that point to the target node via this reference
            CswNbtView view = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship rel1 = view.AddViewRelationship( Node.getNodeType(), false );
            rel1.NodeIdsToFilterIn.Add( Node.NodeId );
            CswNbtViewRelationship rel2 = view.AddViewRelationship( rel1, CswEnumNbtViewPropOwnerType.Second, NodeReferenceProp, false );
            foreach( CswNbtMetaDataNodeTypeProp uniqueProp in UniqueProps )
            {
                view.AddViewProperty( rel2, uniqueProp );
            }

            // Iterate children and store unique property values in a dictionary key
            ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( view, RequireViewPermissions: false, IncludeHiddenNodes: true, IncludeSystemNodes: true );
            if( tree.getChildNodeCount() > 0 )
            {
                tree.goToNthChild( 0 );
                for( Int32 c = 0; c < tree.getChildNodeCount(); c++ )
                {
                    tree.goToNthChild( c );
                    CswPrimaryKey thisNodeId = tree.getNodeIdForCurrentPosition();

                    // Populate MergeInfoData.NodePair.NodeReferences while we're here
                    NodePair.NodeReferences.Add( new MergeInfoData.MergeInfoNodeReference()
                        {
                            NodeId = thisNodeId.ToString(),
                            NodeTypePropId = NodeReferenceProp.PropId
                        } );

                    CswDelimitedString key;
                    if( _AllUniqueKeys.ContainsKey( thisNodeId ) )
                    {
                        // If we've seen this node before, use the existing key but override the merge property
                        // (this will allow us to merge correctly if a nodetype has 
                        //  multiple compound unique references that are all involved in the merge)
                        key = _AllUniqueKeys[thisNodeId];
                        for( Int32 u = 0; u < UniqueProps.Count(); u++ )
                        {
                            if( UniqueProps.ElementAt( u ).PropId == NodeReferenceProp.PropId )
                            {
                                // This value will be equal after the merge
                                key[u] = "[mergeresult]";
                            }
                        } // foreach( CswNbtMetaDataNodeTypeProp uniqueProp in UniqueProps )
                    } // if( _AllUniqueKeys.ContainsKey( thisNodeId ) )
                    else
                    {
                        // generate a new key
                        key = new CswDelimitedString( delimiter );
                        foreach( CswNbtMetaDataNodeTypeProp uniqueProp in UniqueProps )
                        {
                            CswNbtTreeNodeProp prop = tree.getChildNodePropsOfNode().FirstOrDefault( p => p.NodeTypePropId == uniqueProp.PropId );
                            if( null != prop )
                            {
                                if( prop.NodeTypePropId == NodeReferenceProp.PropId )
                                {
                                    // This value will be equal after the merge
                                    key.Add( "[mergeresult]" );
                                }
                                else
                                {
                                    key.Add( prop.Gestalt );
                                }
                            }
                            else
                            {
                                key.Add( "" );
                            }
                        } // foreach( CswNbtMetaDataNodeTypeProp uniqueProp in UniqueProps )
                    } // if-else( _AllUniqueKeys.ContainsKey( thisNodeId ) )

                    if( key.Count > 0 )
                    {
                        ret.Add( key, thisNodeId );
                        _AllUniqueKeys[thisNodeId] = key;
                    }

                    tree.goToParentNode();
                } // for( Int32 c = 0; c < tree.getChildNodeCount(); c++ )
            } // if( tree.getChildNodeCount() > 0 )
            return ret;
        } // _getUniqueKeysDict()


        public MergeInfoData applyMergeChoices( MergeInfoData Choices )
        {
            foreach( MergeInfoData.MergeInfoNodePair nodePair in Choices.NodePairs )
            {
                // Apply the changes to a temp node, so that they can be previewed
                CswNbtNode tempNode;
                if( false == string.IsNullOrEmpty( nodePair.NodeTempId ) )
                {
                    tempNode = _CswNbtResources.Nodes[nodePair.NodeTempId];
                    _applyMergeChoicesToNode( Choices, nodePair, tempNode );
                    tempNode.postChanges( ForceUpdate: false, IsCopy: false, OverrideUniqueValidation: true );
                }
                else
                {
                    CswNbtNode Node1 = _CswNbtResources.Nodes[nodePair.Node1Id];
                    if( null != Node1 )
                    {
                        tempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( Node1.NodeTypeId,
                                                                                  IsTemp: true,
                                                                                  OverrideUniqueValidation: true,
                                                                                  OnAfterMakeNode: delegate( CswNbtNode newNode )
                                                                                      {
                                                                                          _applyMergeChoicesToNode( Choices, nodePair, newNode );
                                                                                      } );
                        nodePair.NodeTempId = tempNode.NodeId.ToString();
                        nodePair.NodeTempName = tempNode.NodeName;
                    }
                }
            } // foreach( MergeInfoData.MergeInfoNodePair nodePair in Choices.NodePairs )
            return Choices;
        } // applyMergeChoices()

        public void _applyMergeChoicesToNode( MergeInfoData Choices, MergeInfoData.MergeInfoNodePair nodePair, CswNbtNode resultNode )
        {
            CswNbtNode Node1 = _CswNbtResources.Nodes[nodePair.Node1Id];
            CswNbtNode Node2 = _CswNbtResources.Nodes[nodePair.Node2Id];
            if( null != Node1 && null != Node2 )
            {
                // Set property values according to choice
                resultNode.copyPropertyValuesGeneric( Node2 );
                foreach( MergeInfoData.MergeInfoProperty mergeProp in nodePair.Properties.Where( mergeProp => mergeProp.Choice == 1 ) )
                {
                    resultNode.Properties[mergeProp.NodeTypePropId].copyGeneric( Node1.Properties[mergeProp.NodeTypePropId] );
                }

                // Set references to new merged node
                if( Int32.MinValue != nodePair.NodeReferencePropId )
                {
                    // Find the new nodeid for the value of the reference
                    ICswNbtNodePropNodeReference NodeReferenceProp = resultNode.Properties[nodePair.NodeReferencePropId].AsNodeReference;
                    CswPrimaryKey oldNodeId = NodeReferenceProp.ReferencedNodeId;
                    MergeInfoData.MergeInfoNodePair otherNodePair = Choices.NodePairs.FirstOrDefault( np => np.Node1Id == oldNodeId.ToString() || np.Node2Id == oldNodeId.ToString() );
                    if( null != otherNodePair )
                    {
                        // Set the reference to point to the new merged node
                        NodeReferenceProp.ReferencedNodeId = CswConvert.ToPrimaryKey( otherNodePair.Node2Id );
                    }
                } // if( Int32.MinValue != nodePair.NodeReferencePropId )

            } // if( null != Node1 && null != Node2 )
        } // _applyMergeChoicesToNode()

        public CswNbtView finishMerge( MergeInfoData Choices )
        {
            CswNbtView view = new CswNbtView( _CswNbtResources );
            CswNbtNode firstMergedNode = null;

            foreach( MergeInfoData.MergeInfoNodePair nodePair in Choices.NodePairs )
            {
                // Remove the temp node
                CswNbtNode NodeTemp = _CswNbtResources.Nodes[nodePair.NodeTempId];
                if( null != NodeTemp )
                {
                    NodeTemp.delete( DeleteAllRequiredRelatedNodes: false, OverridePermissions: true, ValidateRequiredRelationships: false );
                }

                // Merge Node1 into Node2, and delete Node1
                CswNbtNode Node1 = _CswNbtResources.Nodes[nodePair.Node1Id];
                CswNbtNode Node2 = _CswNbtResources.Nodes[nodePair.Node2Id];
                if( null != Node1 && null != Node2 )
                {
                    // Store the first node merged to return
                    if( null == firstMergedNode )
                    {
                        firstMergedNode = Node2;
                    }

                    // Apply the merge to Node2
                    _applyMergeChoicesToNode( Choices, nodePair, Node2 );
                    Node2.postChanges( ForceUpdate: false, IsCopy: false, OverrideUniqueValidation: true );

                    // Update any references to point to node2
                    foreach( MergeInfoData.MergeInfoNodeReference Ref in nodePair.NodeReferences )
                    {
                        CswNbtNode refNode = _CswNbtResources.Nodes[Ref.NodeId];
                        if( refNode.Properties[Ref.NodeTypePropId].getFieldTypeValue() == CswEnumNbtFieldType.UserSelect )
                        {
                            // Special case: UserSelect
                            refNode.Properties[Ref.NodeTypePropId].AsUserSelect.RemoveUser( Node1.NodeId );
                            refNode.Properties[Ref.NodeTypePropId].AsUserSelect.AddUser( Node2.NodeId );
                            refNode.Properties[Ref.NodeTypePropId].AsUserSelect.SyncGestalt();
                        }
                        else
                        {
                            // Node Reference
                            refNode.Properties[Ref.NodeTypePropId].AsNodeReference.ReferencedNodeId = Node2.NodeId;
                            refNode.Properties[Ref.NodeTypePropId].AsNodeReference.RefreshNodeName();
                        }
                        refNode.postChanges( ForceUpdate: false, IsCopy: false, OverrideUniqueValidation: true );
                    }

                    // Delete merged node 1
                    Node1.delete( DeleteAllRequiredRelatedNodes: false, OverridePermissions: true, ValidateRequiredRelationships: false );

                } // if( null != Node1 && null != Node2 )
            } // foreach( MergeInfoData.MergeInfoNodePair nodePair in Choices.NodePairs )

            // Return a view of the first merged node
            if( null != firstMergedNode )
            {
                view = firstMergedNode.getViewOfNode( includeDefaultFilters: false );
            }
            return view;
        } // finishMerge()

    } // public class CswNbtActMerge
} // namespace ChemSW.Nbt.Actions
