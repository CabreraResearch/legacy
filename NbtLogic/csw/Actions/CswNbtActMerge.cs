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
                public Collection<MergeInfoRelationship> Relationships;
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
                public string NodeResultId;
                [DataMember]
                public string NodeResultName;
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
                public Int32 Choice = 1;
            }

            /// <summary>
            /// Relationship data value to update (no conflicts)
            /// </summary>
            [DataContract]
            public class MergeInfoRelationship
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


        private void _addMergeNodes( MergeInfoData ret, CswPrimaryKey NodeId1, CswPrimaryKey NodeId2, CswNbtMetaDataNodeTypeProp IgnoreProp = null )
        {
            CswNbtNode Node1 = _CswNbtResources.Nodes[NodeId1];
            CswNbtNode Node2 = _CswNbtResources.Nodes[NodeId2];
            if( null != Node1 && null != Node2 )
            {
                MergeInfoData.MergeInfoNodePair NodePair = new MergeInfoData.MergeInfoNodePair();
                NodePair.Relationships = new Collection<MergeInfoData.MergeInfoRelationship>();
                NodePair.NodeTypeName = Node1.getNodeType().NodeTypeName;
                NodePair.Node1Id = Node1.NodeId.ToString();
                NodePair.Node2Id = Node2.NodeId.ToString();
                NodePair.Node1Name = Node1.NodeName;
                NodePair.Node2Name = Node2.NodeName;

                foreach( CswNbtNodePropWrapper Prop1 in Node1.Properties.Where( p => ( null == IgnoreProp || p.NodeTypePropId != IgnoreProp.PropId ) ) )
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

                // First, find relationships that point to the merged nodes
                foreach( CswNbtMetaDataNodeTypeProp RelationshipProp in _CswNbtResources.MetaData.getNodeTypeProps( CswEnumNbtFieldType.Relationship )
                                                                                        .Where( RelationshipProp => RelationshipProp.FkMatches( Node1.getNodeType() ) ) )
                {
                    // Find unique key sets for nodes using this relationship that point to either node1 or node2
                    Dictionary<CswDelimitedString, CswPrimaryKey> Node1UniqueKeysDict = _getUniqueKeysDict( NodePair, Node1, RelationshipProp );
                    Dictionary<CswDelimitedString, CswPrimaryKey> Node2UniqueKeysDict = _getUniqueKeysDict( NodePair, Node2, RelationshipProp );

                    // Look for redundant keys to indicate a potential unique violation
                    foreach( CswDelimitedString key in Node1UniqueKeysDict.Keys )
                    {
                        if( Node2UniqueKeysDict.ContainsKey( key ) )
                        {
                            // unique violation!  gotta merge these too.
                            _addMergeNodes( ret, Node1UniqueKeysDict[key], Node2UniqueKeysDict[key], RelationshipProp );
                        }
                    } // foreach( CswDelimitedString key in Node1UniqueKeysDict.Keys )
                } // foreach( CswNbtMetaDataNodeTypeProp RelationshipProp in ...)
            } // if( null != Node1 && null != Node2 )
        } // _addMergeNodes()


        /// <summary>
        /// Create a dictionary of the unique key values of related nodes
        /// As a side effect, also populate MergeInfoData.NodePair.Relationships
        /// </summary>
        private Dictionary<CswDelimitedString, CswPrimaryKey> _getUniqueKeysDict( MergeInfoData.MergeInfoNodePair NodePair, CswNbtNode Node, CswNbtMetaDataNodeTypeProp RelationshipProp )
        {
            Dictionary<CswDelimitedString, CswPrimaryKey> ret = new Dictionary<CswDelimitedString, CswPrimaryKey>();
            char delimiter = '|';

            // Find unique properties for this relationship's nodetype
            IEnumerable<CswNbtMetaDataNodeTypeProp> UniqueProps = RelationshipProp.getNodeType().getUniqueProps();
            if( UniqueProps.Count() > 0 )
            {
                // Create a view of nodes that point to the target node via this relationship
                CswNbtView view = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship rel1 = view.AddViewRelationship( Node.getNodeType(), false );
                rel1.NodeIdsToFilterIn.Add( Node.NodeId );
                CswNbtViewRelationship rel2 = view.AddViewRelationship( rel1, CswEnumNbtViewPropOwnerType.Second, RelationshipProp, false );
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

                        // Populate MergeInfoData.NodePair.Relationships while we're here
                        NodePair.Relationships.Add( new MergeInfoData.MergeInfoRelationship()
                            {
                                NodeId = tree.getNodeIdForCurrentPosition().ToString(),
                                NodeTypePropId = RelationshipProp.PropId
                            } );

                        CswDelimitedString key = new CswDelimitedString( delimiter );
                        foreach( CswNbtMetaDataNodeTypeProp uniqueProp in UniqueProps )
                        {
                            CswNbtTreeNodeProp prop = tree.getChildNodePropsOfNode().FirstOrDefault( p => p.NodeTypePropId == uniqueProp.PropId );
                            if( null != prop )
                            {
                                if( prop.NodeTypePropId == RelationshipProp.PropId )
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
                        ret.Add( key, tree.getNodeIdForCurrentPosition() );

                        tree.goToParentNode();
                    } // for( Int32 c = 0; c < tree.getChildNodeCount(); c++ )
                } // if( tree.getChildNodeCount() > 0 )

            } // if( UniqueProps.Count() > 0 )
            return ret;
        } // _getUniqueKeysDict()


        public MergeInfoData applyMergeChoices( MergeInfoData Choices )
        {
            foreach( MergeInfoData.MergeInfoNodePair nodePair in Choices.NodePairs )
            {
                CswNbtNode Node1 = _CswNbtResources.Nodes[nodePair.Node1Id];
                CswNbtNode Node2 = _CswNbtResources.Nodes[nodePair.Node2Id];
                if( null != Node1 && null != Node2 )
                {
                    Action<CswNbtNode> ApplyPropVals = delegate( CswNbtNode newNode )
                        {
                            newNode.copyPropertyValues( Node1 );
                            foreach( MergeInfoData.MergeInfoProperty mergeProp in nodePair.Properties.Where( mergeProp => mergeProp.Choice == 2 ) )
                            {
                                newNode.Properties[mergeProp.NodeTypePropId].copy( Node2.Properties[mergeProp.NodeTypePropId] );
                            }
                        };

                    CswNbtNode tempNode;
                    if( false == string.IsNullOrEmpty( nodePair.NodeResultId ) )
                    {
                        tempNode = _CswNbtResources.Nodes[nodePair.NodeResultId];
                        ApplyPropVals( tempNode );
                        tempNode.postChanges( false );
                    }
                    else
                    {
                        tempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( Node1.NodeTypeId, IsTemp: true, OverrideUniqueValidation: true, OnAfterMakeNode: ApplyPropVals );
                        nodePair.NodeResultId = tempNode.NodeId.ToString();
                        nodePair.NodeResultName = tempNode.NodeName;
                    }
                } // if( null != Node1 && null != Node2 )
            } // foreach( MergeInfoData.MergeInfoNodePair nodePair in Choices.NodePairs )
            return Choices;
        } // applyMergeChoices()


    }
}
