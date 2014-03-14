using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ViewEditor;

namespace NbtWebApp.Actions.Explorer
{
    public class CswNbtWebServiceNodeExplorer
    {
        private static int MAX_DEPTH = 1;
        private static HashSet<int> SEEN = new HashSet<int>();
        private static CswNbtNode StartingNode;
        private static CswCommaDelimitedString FilterVal;

        public static void GetFilterOpts( ICswResources CswResources, CswNbtExplorerReturn Return, string Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswCommaDelimitedString ValsCDS = new CswCommaDelimitedString();
            Return.Data.FilterVal = CswNbtArborGraph._setDefaultFilterVal( NbtResources );
            ValsCDS.FromString( Return.Data.FilterVal );

            foreach( CswNbtMetaDataNodeType NodeType in NbtResources.MetaData.getNodeTypes() )
            {
                Return.Data.Opts.Add( new ArborFilterOpt()
                    {
                        selected = ValsCDS.Contains( "NT_" + NodeType.NodeTypeId ),
                        text = NodeType.NodeTypeName,
                        value = "NT_" + NodeType.NodeTypeId.ToString()
                    } );
            }

            foreach( CswNbtMetaDataObjectClass ObjClass in NbtResources.MetaData.getObjectClasses() )
            {
                Return.Data.Opts.Add( new ArborFilterOpt()
                {
                    selected = ValsCDS.Contains( "OC_" + ObjClass.ObjectClassId.ToString() ),
                    text = ObjClass.ObjectClassName,
                    value = "OC_" + ObjClass.ObjectClassId.ToString()
                } );
            }
        }

        public static void Initialize( ICswResources CswResources, CswNbtExplorerReturn Return, CswNbtExplorerRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey NodeId = CswConvert.ToPrimaryKey( Request.NodeId );

            if( Request.Depth <= 4 || Request.Depth > 0 ) //We never want a request higher than 4 and 0 doesn't make sense
            {
                MAX_DEPTH = Request.Depth;
            }

            FilterVal = new CswCommaDelimitedString();
            if( String.IsNullOrEmpty( Request.FilterVal ) )
            {
                FilterVal.FromString( CswNbtArborGraph._setDefaultFilterVal( NbtResources ) );
            }
            else
            {
                FilterVal.FromString( Request.FilterVal );
            }


            StartingNode = NbtResources.Nodes[NodeId];
            CswNbtMetaDataNodeType startingNT = StartingNode.getNodeType();
            //Add the initial node to the graph
            _addToGraph( Return, StartingNode.NodeName, string.Empty, NodeId.ToString(), startingNT.IconFileName, 0, "Instance", NodeId.ToString(), startingNT.NodeTypeName );

            _recurseForRelatingNodes( NbtResources, Return, StartingNode, 1, NodeId.ToString() );

            _recurseForRelatedNTs( NbtResources, Return, StartingNode.NodeTypeId, 1, NodeId.ToString() );
        }

        private static void _recurseForRelatingNodes( CswNbtResources NbtResources, CswNbtExplorerReturn Return, CswNbtNode Node, int Level, string OwnerIdStr )
        {
            CswNbtMetaDataNodeType TargetNodeType = Node.getNodeType();
            foreach( CswNbtMetaDataNodeTypeProp RelNTP in TargetNodeType.getNodeTypeProps( CswEnumNbtFieldType.Relationship ) ) //TODO: Locations are just like relationships, we should be able to handle them
            {
                CswNbtNodePropRelationship RelProp = Node.Properties[RelNTP];
                string Icon = _getIconFromRelationshipProp( NbtResources, RelNTP );
                if( CswTools.IsPrimaryKey( RelProp.RelatedNodeId ) )
                {
                    CswNbtNode TargetNode = NbtResources.Nodes[RelProp.RelatedNodeId];
                    CswNbtMetaDataNodeType TargetNT = TargetNode.getNodeType();

                    if( FilterVal.Contains( "NT_" + TargetNode.NodeTypeId ) || FilterVal.Contains( "OC_" + TargetNode.getObjectClassId() ) )
                    {
                        string targetIdStr = OwnerIdStr + "_" + RelProp.RelatedNodeId.ToString();
                        _addToGraph( Return, RelProp.PropName + ": " + RelProp.CachedNodeName, Node.NodeId.ToString(), RelProp.RelatedNodeId.ToString(), Icon, Level, "Instance", RelProp.RelatedNodeId.ToString(), TargetNT.NodeTypeName );

                        if( Level + 1 <= MAX_DEPTH )
                        {
                            _recurseForRelatedNTs( NbtResources, Return, TargetNode.NodeTypeId, 1, RelProp.RelatedNodeId.ToString() );
                        }

                        if( Level + 1 <= MAX_DEPTH )
                        {
                            _recurseForRelatingNodes( NbtResources, Return, TargetNode, Level + 1, targetIdStr );
                        }
                    }
                }
            }
        }

        private static string _getIconFromRelationshipProp( CswNbtResources NbtResources, CswNbtMetaDataNodeTypeProp RelationshipNTP )
        {
            string ret = string.Empty;
            if( RelationshipNTP.FKType == "ObjectClassId" )
            {
                CswNbtMetaDataObjectClass ObjClass = NbtResources.MetaData.getObjectClass( RelationshipNTP.FKValue );
                ret = ObjClass.IconFileName;
            }
            else
            {
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( RelationshipNTP.FKValue );
                ret = NodeType.IconFileName;
            }
            return ret;
        }
        
        private static void _recurseForRelatedNTs( CswNbtResources NbtResources, CswNbtExplorerReturn Return, int NodeTypeId, int level, string OwnerIdStr )
        {
            if( false == SEEN.Contains( NodeTypeId ) && level <= MAX_DEPTH )
            {
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( NodeTypeId );
                CswNbtView View = new CswNbtView( NbtResources );
                CswNbtViewRelationship Relationship = View.AddViewRelationship( NodeType, false );

                CswNbtViewEditorRule ViewRule = new CswNbtViewEditorRuleAddViewLevels( NbtResources, new CswNbtViewEditorData()
                    {
                        CurrentView = View
                    } );

                string DisplayName = string.Empty;
                string TargetIdStr = string.Empty;
                string IconFilename = string.Empty;
                string Id = string.Empty;

                Collection<CswNbtViewRelationship> RelatedTypes = ViewRule.getViewChildRelationshipOptions( View, Relationship.ArbitraryId );
                foreach( CswNbtViewRelationship Related in RelatedTypes )
                {
                    bool WasNT = false;
                    bool ObjClassAllowed = false;
                    DisplayName = Related.TextLabel;
                    string MetaDataName = string.Empty;

                    int id = ( Related.PropOwner == CswEnumNbtViewPropOwnerType.First && Related.FirstId != Int32.MinValue ? Related.FirstId : Related.SecondId );
                    CswEnumNbtViewRelatedIdType IdType = ( Related.PropOwner == CswEnumNbtViewPropOwnerType.First && Related.FirstId != Int32.MinValue ? Related.FirstType : Related.SecondType );

                    if( Related.PropOwner == CswEnumNbtViewPropOwnerType.Second )
                    {

                        if( IdType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                        {
                            CswNbtMetaDataNodeType RelatedNodeType = NbtResources.MetaData.getNodeType( id );
                            IconFilename = RelatedNodeType.IconFileName;
                            TargetIdStr = OwnerIdStr + "_NT_" + RelatedNodeType.NodeTypeId;
                            Id = "NT_" + RelatedNodeType.NodeTypeId;

                            CswNbtMetaDataObjectClass ObjClass = RelatedNodeType.getObjectClass();
                            ObjClassAllowed = FilterVal.Contains( "OC_" + ObjClass.ObjectClassId );
                            MetaDataName = RelatedNodeType.NodeTypeName;

                            WasNT = true;
                        }
                        else if( IdType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                        {
                            CswNbtMetaDataObjectClass RelatedObjClass = NbtResources.MetaData.getObjectClass( id );
                            IconFilename = RelatedObjClass.IconFileName;
                            TargetIdStr = OwnerIdStr + "_OC_" + RelatedObjClass.ObjectClassId;
                            Id = "OC_" + RelatedObjClass.ObjectClassId;
                            MetaDataName = RelatedObjClass.ObjectClassName;
                        }

                        if( ( ( IdType == CswEnumNbtViewRelatedIdType.NodeTypeId && FilterVal.Contains( "NT_" + id ) || ObjClassAllowed ) )
                            || ( IdType == CswEnumNbtViewRelatedIdType.ObjectClassId && FilterVal.Contains( "OC_" + id ) ) )
                        {

                            _addToGraph( Return, DisplayName, OwnerIdStr, TargetIdStr, IconFilename, level, "Category", Id, MetaDataName );

                            if( level + 1 <= MAX_DEPTH && WasNT )
                            {
                                _recurseForRelatedNTs( NbtResources, Return, id, level + 1, TargetIdStr );
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Helper method for adding data to the return object
        /// </summary>
        private static void _addToGraph( CswNbtExplorerReturn Return, string Label, string OwnerId, string TargetId, string Icon, int level, string Type, string Id, string MetaDataName )
        {
            string URI = "api/v1/" + MetaDataName;
            if( "Instance" == Type )
            {
                CswPrimaryKey nodeid = new CswPrimaryKey();
                nodeid.FromString( TargetId );
                URI += "/" + nodeid.PrimaryKey;
            }

            Return.Data.Nodes.Add( new CswNbtArborNode()
                {
                    NodeIdStr = TargetId,
                    data = new CswNbtArborNode.CswNbtArborNodeData()
                        {
                            Icon = "Images/newicons/100/" + Icon,
                            Label = Label,
                            NodeId = Id,
                            Level = level,
                            Type = Type,
                            URI = URI
                        }
                } );

            if( false == String.IsNullOrEmpty( OwnerId ) && false == String.IsNullOrEmpty( TargetId ) )
            {
                Return.Data.Edges.Add( new CswNbtArborEdge()
                    {
                        OwnerNodeIdStr = OwnerId,
                        TargetNodeIdStr = TargetId,
                        data = new CswNbtArborEdge.CswNbtArborEdgeData()
                            {
                                Length = level * 10
                            }
                    } );
            }
        }
        
    }
}