using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class NodeSelect
    {
        [DataContract]
        public class Request
        {
            [DataMember( IsRequired = false )]
            public Int32 NodeTypeId = Int32.MinValue;
            
            //NodeTypeName seems like a bad plan.
            [DataMember(IsRequired = false)] 
            public String NodeTypeName = String.Empty;

            [DataMember( IsRequired = false )]
            public Int32 ObjectClassId = Int32.MinValue;

            private NbtObjectClass _ObjectClass;

            [DataMember( IsRequired = false )]
            public string ObjectClass
            {
                get { return _ObjectClass; }
                set { _ObjectClass = value; }
            }

            private NbtObjectClass _RelatedToObjectClass;

            [DataMember( IsRequired = false )]
            public string RelatedToObjectClass
            {
                get { return _RelatedToObjectClass; }
                set { _RelatedToObjectClass = value; }
            }

            public CswPrimaryKey RelatedNodeId = null;

            [DataMember( IsRequired = false, EmitDefaultValue = false, Name = "RelatedNodeId" )]
            public string RelatedToNodeId
            {
                get
                {
                    string Ret = string.Empty;
                    if( null != RelatedNodeId )
                    {
                        Ret = RelatedNodeId.ToString();
                    }
                    return Ret;
                }
                set
                {
                    CswPrimaryKey RelatedPk = CswConvert.ToPrimaryKey( value );
                    if( CswTools.IsPrimaryKey( RelatedPk ) )
                    {
                        RelatedNodeId = RelatedPk;
                    }
                }
            }

            public CswNbtSessionDataId SessionViewId = null;
            public CswNbtViewId ViewId = null;
            
            [DataMember( IsRequired = false, EmitDefaultValue = false, Name = "ViewId" )]
            public string NbtViewId
            {
                get
                {
                    string Ret = string.Empty;
                    if( null != ViewId && ViewId.isSet() )
                    {
                        Ret = ViewId.ToString();
                        SessionViewId = null;
                    }
                    else if( null != SessionViewId && SessionViewId.isSet() )
                    {
                        Ret = SessionViewId.ToString();
                        ViewId = null;
                    }
                    return Ret;
                }
                set
                {
                    string ViewIdString = value;
                    if( CswNbtViewId.isViewIdString( ViewIdString ) )
                    {
                        ViewId = new CswNbtViewId( ViewIdString );
                        SessionViewId = null;
                    }
                    else if( CswNbtSessionDataId.isSessionDataIdString( ViewIdString ) )
                    {
                        SessionViewId = new CswNbtSessionDataId( ViewIdString );
                        ViewId = null;
                    }
                }
            }
        }

        [DataContract]
        public class Response
        {
            
            [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "RelatedObjectClassId" )]
            public Int32 RelatedObjectClassId = Int32.MinValue;

            [DataMember( EmitDefaultValue = true, IsRequired = true, Name = "CanAdd" )]
            public bool CanAdd = false;

            [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "UseSearch" )]
            public bool UseSearch = false;

            [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "NodeTypeId" )]
            public Int32 NodeTypeId = Int32.MinValue;

            [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "ObjectClassId" )]
            public Int32 ObjectClassId = Int32.MinValue;

            [DataMember( EmitDefaultValue = true, IsRequired = true, Name = "Nodes" )]
            public Collection<CswNbtNode.Node> Nodes = new Collection<CswNbtNode.Node>();

            [DataMember]
            public string NodeLink = string.Empty;
        }
    }


    public class CswNbtSdNode
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        public CswNbtSdNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents = null )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            Int32 SearchThreshold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.relationshipoptionlimit.ToString() ) );
            if( SearchThreshold > 0 )
            {
                _SearchThreshold = SearchThreshold;
            }
        }

        private Int32 _SearchThreshold = 100;

        public CswPrimaryKey CopyNode( CswPrimaryKey NodePk )
        {
            CswPrimaryKey RetKey = null;
            CswNbtNode OriginalNode = _CswNbtResources.Nodes.GetNode( NodePk );

            if( null != OriginalNode )
            {
                CswNbtNode NewNode = OriginalNode.ObjClass.CopyNode();
                if( NewNode != null && null != _CswNbtStatisticsEvents )
                {
                    _CswNbtStatisticsEvents.OnCopyNode( OriginalNode, NewNode );
                    RetKey = NewNode.NodeId;
                }
            }
            return RetKey;
        }

        public bool DeleteNode( CswPrimaryKey NodePk, out string NodeName, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _DeleteNode( NodePk, _CswNbtResources, out NodeName, DeleteAllRequiredRelatedNodes: DeleteAllRequiredRelatedNodes );
        }

        private bool _DeleteNode( CswPrimaryKey NodePk, CswNbtResources NbtResources, out string NodeName, bool DeleteAllRequiredRelatedNodes = false )
        {
            bool ret = false;
            NodeName = "";
            CswNbtNode NodeToDelete = NbtResources.Nodes[NodePk];
            if( null != NodeToDelete )
            {
                CswNbtMetaDataNodeType NodeType = NodeToDelete.getNodeType();
                NodeName = NodeType.NodeTypeName + ": " + NodeToDelete.NodeName;
                NodeToDelete.delete( DeleteAllRequiredRelatedNodes: DeleteAllRequiredRelatedNodes );
                ret = true;
            }
            return ret;
        }

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId, string SelectedText )
        {
            JObject RetObj = new JObject();
            if( null == PropId ||
                Int32.MinValue == PropId.NodeTypePropId ||
                null == PropId.NodeId ||
                Int32.MinValue == PropId.NodeId.PrimaryKey )
            {
                throw new CswDniException( ErrorType.Error, "Cannot execute a button click without valid parameters.", "Attempted to call DoObjectClassButtonClick with invalid NodeId and NodeTypePropId." );
            }

            CswNbtNode Node = _CswNbtResources.Nodes.GetNode( PropId.NodeId );
            if( null == Node )
            {
                throw new CswDniException( ErrorType.Error, "Cannot find a valid node with the provided parameters.", "No node exists for NodePk " + PropId.NodeId.ToString() + "." );
            }

            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( null == NodeTypeProp )
            {
                throw new CswDniException( ErrorType.Error, "Cannot find a valid property with the provided parameters.", "No property exists for NodeTypePropId " + PropId.NodeTypePropId.ToString() + "." );
            }

            CswNbtObjClass NbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, Node.getObjectClassId(), Node );

            CswNbtObjClass.NbtButtonData ButtonData = new CswNbtObjClass.NbtButtonData( NodeTypeProp ) { SelectedText = SelectedText };

            bool Success = NbtObjClass.onButtonClick( ButtonData );

            RetObj["action"] = ButtonData.Action.ToString();
            RetObj["actionData"] = ButtonData.Data;  //e.g. popup url
            RetObj["message"] = ButtonData.Message;
            RetObj["success"] = Success.ToString().ToLower();

            return RetObj;
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        public void addNodeProps( CswNbtNode Node, JObject PropsObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            if( Node != null && null != PropsObj && PropsObj.HasValues )
            {
                foreach( JObject PropObj in
                    from PropJProp
                        in PropsObj.Properties()
                    where null != PropJProp.Value
                    select CswConvert.ToJObject( PropJProp.Value )
                        into PropObj
                        where PropObj.HasValues 
                        select PropObj )
                {
                    addSingleNodeProp( Node, PropObj, Tab );
                }
            }
        }

        public void addSingleNodeProp( CswNbtNode Node, JObject PropObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( CswConvert.ToString( PropObj["id"] ) );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
            CswNbtMetaDataNodeType NodeType = MetaDataProp.getNodeType();

            if( _CswNbtResources.Permit.canNodeType( Security.CswNbtPermit.NodeTypePermission.Edit, NodeType ) ||
                _CswNbtResources.Permit.canTab( Security.CswNbtPermit.NodeTypePermission.Edit, NodeType, Tab ) ||
                _CswNbtResources.Permit.isPropWritable( Security.CswNbtPermit.NodeTypePermission.Edit, MetaDataProp, Tab ) )
            {
                Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null );

                // Recurse on sub-props
                if( null != PropObj["subprops"] )
                {
                    JObject SubPropsObj = (JObject) PropObj["subprops"];
                    if( SubPropsObj.HasValues )
                    {
                        foreach( JObject ChildPropObj in SubPropsObj.Properties()
                                    .Where( ChildProp => null != ChildProp.Value && ChildProp.Value.HasValues )
                                    .Select( ChildProp => (JObject) ChildProp.Value )
                                    .Where( ChildPropObj => ChildPropObj.HasValues ) )
                        {
                            addSingleNodeProp( Node, ChildPropObj, Tab );
                        }
                    }
                }

            }//if user has permission to edit the property

        } // _applyPropJson

        private void _getRelationshipSecondTypeRecursive( Collection<CswNbtViewRelationship> Relationships,
                                                          Dictionary<Int32, Int32> SecondTypes )
        {
            foreach( CswNbtViewRelationship Relationship in Relationships )
            {
                if( Relationship.ChildRelationships.Count == 0 )
                {
                    ICswNbtMetaDataObject MetaObj = Relationship.SecondMetaDataObject();
                    if( MetaObj.UniqueIdFieldName == CswNbtMetaDataObjectClass.MetaDataUniqueType )
                    {
                        if( SecondTypes.Count == 0 || 
                            SecondTypes.ContainsValue( Relationship.SecondId ) )
                        {
                            CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClass( MetaObj.UniqueId );
                            foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getLatestVersionNodeTypes() )
                            {
                                if( false == SecondTypes.ContainsKey( NodeType.NodeTypeId ) )
                                {
                                    SecondTypes.Add( NodeType.NodeTypeId, Relationship.SecondId );
                                }
                            }
                        }
                    }
                    else
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( MetaObj.UniqueId );
                        if( null != NodeType && 
                            false == SecondTypes.ContainsKey(NodeType.getNodeTypeLatestVersion().NodeTypeId) && 
                            ( SecondTypes.Count == 0 || SecondTypes.ContainsValue( NodeType.ObjectClassId ) ) )
                        {
                            SecondTypes.Add( NodeType.getNodeTypeLatestVersion().NodeTypeId, NodeType.ObjectClassId );
                        }
                    }
                }
                else
                {
                    _getRelationshipSecondTypeRecursive( Relationship.ChildRelationships, SecondTypes );
                }
            }
        }

        /// <summary>
        /// Get all NodeTypeIds of matching ObjectClassIds for each relationship at the lowest level of the View (relationships with no child relationships).
        /// </summary>
        /// <returns>A Dictionary of <NodeTypeId, ObjectClassId> of a single ObjectClassId</returns>
        private Dictionary<Int32, Int32> _getRelationshipSecondType( CswNbtView View )
        {
            Dictionary<Int32, Int32> SecondTypes = new Dictionary<Int32, Int32>();
            if( null != View && View.Root.ChildRelationships.Count > 0 )
            {
                _getRelationshipSecondTypeRecursive( View.Root.ChildRelationships, SecondTypes );
            }
            return SecondTypes;
        }

        /// <summary>
        /// Get all Nodes in this View which match the provided NodeTypes or ObjectClass
        /// </summary>
        public Collection<CswNbtNode.Node> getOptions( CswNbtView View, Collection<Int32> NodeTypeIds, Int32 ObjectClassId )
        {
            Collection<CswNbtNode.Node> Options = new Collection<CswNbtNode.Node>();
            if( View != null )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView(
                    View: View,
                    IncludeSystemNodes: false,
                    RequireViewPermissions: false,
                    IncludeHiddenNodes: false );

                if( NodeTypeIds.Count > 0 )
                {
                    Int32 TotalNodeCount = 0;
                    foreach( Int32 NodeTypeId in NodeTypeIds )
                    {
                        Tree.goToRoot();
                        TotalNodeCount += _addOptionsRecurse( Options, Tree, NodeTypeId, Int32.MinValue, TotalNodeCount );
                    }
                }
                else
                {
                    _addOptionsRecurse( Options, Tree, Int32.MinValue, ObjectClassId, 0 );
                }
            }
            return Options;
        }

        /// <summary>
        /// Get all Nodes in this View which match the provided NodeType or ObjectClass
        /// </summary>
        public Collection<CswNbtNode.Node> getOptions( CswNbtViewId ViewId, Int32 TargetNodeTypeId, Int32 TargetObjectClassId )
        {
            CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
            Collection<CswNbtNode.Node> Options = new Collection<CswNbtNode.Node>();
            if( View != null )
            {
                ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView(
                    View : View,
                    IncludeSystemNodes : false,
                    RequireViewPermissions : false,
                    IncludeHiddenNodes : false );
                _addOptionsRecurse( Options, CswNbtTree, TargetNodeTypeId, TargetObjectClassId, 0 );
            }
            return Options;
        }

        private Int32 _addOptionsRecurse( Collection<CswNbtNode.Node> Options, ICswNbtTree Tree, Int32 TargetNodeTypeId, Int32 TargetObjectClassId, Int32 NodeCount )
        {
            Int32 TotalNodeCount = NodeCount;
            Int32 ThisNodeCount = Tree.getChildNodeCount();
            for( Int32 c = 0; c < ThisNodeCount && TotalNodeCount <= _SearchThreshold; c++ )
            {
                Tree.goToNthChild( c );
                if( Tree.getNodeKeyForCurrentPosition().NodeTypeId == TargetNodeTypeId ||
                    Tree.getNodeKeyForCurrentPosition().ObjectClassId == TargetObjectClassId )
                {
                    TotalNodeCount += 1;
                    Options.Add( new CswNbtNode.Node( Tree.getNodeIdForCurrentPosition(), Tree.getNodeNameForCurrentPosition() ) );
                }

                _addOptionsRecurse( Options, Tree, TargetNodeTypeId, TargetObjectClassId, TotalNodeCount );

                Tree.goToParentNode();
            } // for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
            return TotalNodeCount;
        } // _addOptionsRecurse()

        private CswNbtView _getView( NodeSelect.Request Request )
        {
            CswNbtView Ret = null;
            try
            {
                if( null != Request.ViewId && Request.ViewId.isSet() )
                {
                    Ret = _CswNbtResources.ViewSelect.restoreView( Request.ViewId );
                }
                if( null != Request.SessionViewId && Request.SessionViewId.isSet() )
                {
                    Ret = _CswNbtResources.ViewSelect.getSessionView( Request.SessionViewId );
                }
            }
            catch( CswDniException DniException )
            {
                Ret = null;
            }
            return Ret;
        }

        public NodeSelect.Response getNodes( NodeSelect.Request Request )
        {
            NodeSelect.Response Ret = new NodeSelect.Response();

            Ret.CanAdd = true;
            Ret.UseSearch = false;
            Ret.NodeTypeId = Request.NodeTypeId;
            Ret.ObjectClassId = Request.ObjectClassId;

            CswNbtMetaDataObjectClass MetaDataObjectClass = null;
            // case 25956
            Collection<Int32> NodeTypeIds = new Collection<Int32>();

            // If we have a view, use it
            CswNbtView View = _getView( Request );
            if( null != View )
            {
                if( Request.NodeTypeId <= 0 && Request.ObjectClassId <= 0 &&
                    CswNbtResources.UnknownEnum == Request.ObjectClass )
                {
                    // Absent a MetaDataObject ID, 
                    // the safest assumption is that we want all nodes of the same Object Class at the lowest level of the View,
                    // using the relationships defined on the View, of course.
                    Dictionary<Int32, Int32> LowestLevelNodeTypes = _getRelationshipSecondType( View );
                    foreach( KeyValuePair<int, int> KeyValuePair in LowestLevelNodeTypes )
                    {
                        NodeTypeIds.Add( KeyValuePair.Key );
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( KeyValuePair.Key );
                            
                        Ret.CanAdd = Ret.CanAdd ||
                                        _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create,
                                                                            NodeType );
                    }
                    Ret.ObjectClassId = LowestLevelNodeTypes.FirstOrDefault().Value;
                    MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( Ret.ObjectClassId );
                }
            }
            

            // If we don't have a view, make one
            if( null == View )
            {
                if( Request.NodeTypeId > 0 || false == string.IsNullOrEmpty(Request.NodeTypeName))
                {
                    CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( Request.NodeTypeId ) ??
                                                                //Again, seems like a bad plan to use name
                                                              _CswNbtResources.MetaData.getNodeType( Request.NodeTypeName );
                    if( null != MetaDataNodeType )
                    {
                        MetaDataObjectClass = MetaDataNodeType.getObjectClass();
                        NodeTypeIds.Add( MetaDataNodeType.NodeTypeId );
                        View = new CswNbtView( _CswNbtResources );
                        View.AddViewRelationship( MetaDataNodeType, IncludeDefaultFilters: true );

                        Ret.NodeTypeId = MetaDataNodeType.NodeTypeId;
                        Ret.CanAdd = _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create,
                                                                          MetaDataNodeType );
                    }
                }
                else
                {
                    if( Request.ObjectClassId > 0 )
                    {
                        MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( Request.ObjectClassId );
                    }
                    else if( Request.ObjectClass != CswNbtResources.UnknownEnum )
                    {
                        MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( Request.ObjectClass );
                    }

                    if( null != MetaDataObjectClass )
                    {
                        Ret.ObjectClassId = MetaDataObjectClass.ObjectClassId;
                        
                        View = new CswNbtView(_CswNbtResources);
                        CswNbtViewRelationship Relationship = View.AddViewRelationship( MetaDataObjectClass, IncludeDefaultFilters: true );

                        if( false == string.IsNullOrEmpty( Request.RelatedToObjectClass ) &&
                           CswTools.IsPrimaryKey( Request.RelatedNodeId ) )
                        {
                            NbtObjectClass RealRelatedObjectClass = Request.RelatedToObjectClass;

                            CswNbtNode RelatedNode = _CswNbtResources.Nodes[Request.RelatedNodeId];
                            if( null != RelatedNode )
                            {
                                if( RelatedNode.ObjClass.ObjectClass.ObjectClass == RealRelatedObjectClass )
                                {
                                    Collection<CswNbtMetaDataObjectClassProp> RelatedProps = new Collection<CswNbtMetaDataObjectClassProp>();
                                    CswNbtMetaDataObjectClass MetaRelatedObjectClass = _CswNbtResources.MetaData.getObjectClass( RealRelatedObjectClass );
                                    Ret.RelatedObjectClassId = MetaRelatedObjectClass.ObjectClassId;
                                    foreach( CswNbtMetaDataObjectClassProp OcProp in
                                                from _OcProp
                                                    in MetaDataObjectClass.getObjectClassProps()
                                                where
                                                    _OcProp.getFieldType().FieldType ==
                                                    CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                                                    _OcProp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                                                    _OcProp.FKValue == MetaRelatedObjectClass.ObjectClassId
                                                select _OcProp )
                                    {
                                        RelatedProps.Add( OcProp );
                                    }

                                    if( RelatedProps.Any() )
                                    {
                                        foreach( CswNbtMetaDataObjectClassProp RelationshipProp in RelatedProps )
                                        {
                                            View.AddViewPropertyAndFilter( Relationship, RelationshipProp,
                                                                           SubFieldName :
                                                                               CswNbtSubField.SubFieldName.NodeID,
                                                                           Value :
                                                                               Request.RelatedNodeId.PrimaryKey.ToString() );
                                        }
                                    }
                                }
                            }
                        }


                        Ret.CanAdd = MetaDataObjectClass.getLatestVersionNodeTypes()
                                                    .Aggregate( false,
                                                                ( current, NodeType ) =>
                                                                current ||
                                                                _CswNbtResources.Permit.canNodeType(
                                                                    CswNbtPermit.NodeTypePermission.Create, NodeType ) );
                    }
                    else
                    {
                        Ret.CanAdd = false;
                    }
                }
            }
            
            if( null != View )
            {
                Ret.Nodes = getOptions( View, NodeTypeIds, Ret.ObjectClassId );
                Ret.UseSearch = Ret.UseSearch || Ret.Nodes.Count >= _SearchThreshold;
            }
            
            return Ret;
        }




    } // class CswNbtSdNode

} // namespace ChemSW.Nbt.WebServices
