using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNode
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        private readonly CswNbtSdNode _NodeSd;
        public CswNbtWebServiceNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _NodeSd = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
        }

        public CswPrimaryKey CopyNode( CswPrimaryKey NodePk )
        {
            return _NodeSd.CopyNode( NodePk );
        }

        public JObject DeleteNodes( string[] NodePks, string[] NodeKeys )
        {
            JObject ret = new JObject();
            Collection<CswPrimaryKey> NodePrimaryKeys = new Collection<CswPrimaryKey>();

            if( NodeKeys.Length > 0 )
            {
                foreach( string NodeKey in NodeKeys )
                {
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
                    if( null != NbtNodeKey )
                    {
                        NodePrimaryKeys.Add( NbtNodeKey.NodeId );
                    }
                }
            }
            if( NodePks.Length > 0 )
            {
                foreach( string NodePk in NodePks )
                {
                    CswPrimaryKey PrimaryKey = CswConvert.ToPrimaryKey( NodePk );
                    if( CswTools.IsPrimaryKey( PrimaryKey ) && false == NodePrimaryKeys.Contains( PrimaryKey ) )
                    {
                        NodePrimaryKeys.Add( PrimaryKey );
                    }
                }
            }
            if( NodePrimaryKeys.Count > 0 )
            {
                if( NodePrimaryKeys.Count < CswNbtBatchManager.getBatchThreshold( _CswNbtResources ) )
                {
                    bool success = true;
                    string DeletedNodes = "";
                    foreach( CswPrimaryKey Npk in NodePrimaryKeys )
                    {
                        string DeletedNode = "";
                        success = DeleteNode( Npk, out DeletedNode ) && success;
                        if( success )
                        {
                            DeletedNodes += DeletedNode;
                        }
                    }
                    ret["Succeeded"] = success.ToString();
                }
                else
                {
                    CswNbtBatchOpMultiDelete op = new CswNbtBatchOpMultiDelete( _CswNbtResources );
                    CswNbtObjClassBatchOp BatchNode = op.makeBatchOp( NodePrimaryKeys );
                    ret["batch"] = BatchNode.NodeId.ToString();
                }
            }

            return ret;
        }

        public bool DeleteNode( CswPrimaryKey NodePk, out string DeletedNodeName, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _NodeSd.DeleteNode( NodePk, out DeletedNodeName, DeleteAllRequiredRelatedNodes );
        }

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId, string SelectedText )
        {
            return _NodeSd.doObjectClassButtonClick( PropId, SelectedText );
        }

        public JObject deleteDemoDataNodes()
        {
            JObject Ret = new JObject();
            CswCommaDelimitedString SuccessfulDeletes = new CswCommaDelimitedString();
            CswCommaDelimitedString FailedDeletes = new CswCommaDelimitedString();
            Int32 Total = 0;
            string SuccessText = "";
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                /* Get a new CswNbtResources as the System User */
                CswNbtWebServiceMetaData wsMd = new CswNbtWebServiceMetaData( _CswNbtResources );
                CswNbtResources NbtSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );
                Collection<Exception> Exceptions = new Collection<Exception>();
                try
                {
                    CswNbtResources UserSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );
                    CswNbtMetaDataObjectClass UserOc = UserSystemResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
                    foreach( CswNbtObjClassUser User in UserOc.getNodes( forceReInit: true, includeSystemNodes: false ) )
                    {
                        if( CswTools.IsPrimaryKey( User.WorkUnitProperty.RelatedNodeId ) )
                        {
                            CswNbtNode WorkUnit = UserSystemResources.Nodes[User.WorkUnitProperty.RelatedNodeId];
                            if( null != WorkUnit && WorkUnit.IsDemo )
                            {
                                User.WorkUnitProperty.RelatedNodeId = null;
                            }
                        }
                        if( CswTools.IsPrimaryKey( User.DefaultLocationProperty.SelectedNodeId ) )
                        {
                            CswNbtNode Location = UserSystemResources.Nodes[User.DefaultLocationProperty.SelectedNodeId];
                            if( null != Location && Location.IsDemo )
                            {
                                User.DefaultLocationProperty.SelectedNodeId = null;
                            }
                        }
                        User.postChanges( ForceUpdate: true );
                    }
                    wsMd.finalizeOtherResources( UserSystemResources );
                }
                catch( Exception Ex )
                {
                    Exceptions.Add( Ex );
                }
                #region Delete Demo Nodes
                CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "delete_demodata_nodes", "nodes" );
                DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString { "nodeid" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
                Total = NodesTable.Rows.Count;
                foreach( DataRow NodeRow in NodesTable.Rows )
                {
                    string DeletedNodeName = "";
                    try
                    {
                        CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) );

                        if( _NodeSd.DeleteNode( NodePk, out DeletedNodeName, DeleteAllRequiredRelatedNodes: true ) )
                        {
                            SuccessfulDeletes.Add( DeletedNodeName );
                        }
                        else if( false == string.IsNullOrEmpty( DeletedNodeName ) )
                        {
                            //The cascading delete from above has actually failed to delete this node
                            FailedDeletes.Add( DeletedNodeName );
                        }
                    }
                    catch( Exception Exception )
                    {
                        FailedDeletes.Add( DeletedNodeName );
                        Exceptions.Add( Exception );
                    }
                }

                CswTableSelect PostDeleteNodesSelect = _CswNbtResources.makeCswTableSelect( "delete_demodata_nodes", "nodes" );
                DataTable PostDeleteNodesTable = PostDeleteNodesSelect.getTable( new CswCommaDelimitedString { "nodeid" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
                if( PostDeleteNodesTable.Rows.Count == 0 )
                {
                    //Since DeleteNode() cascades across all required relationships, not all nodes iterated above will be deleted in the visible loop. 
                    //As we can't know the nodenames of these deletes, eliminate confusion and make the numbers match.
                    Total = SuccessfulDeletes.Count;
                    FailedDeletes = new CswCommaDelimitedString();
                }

                #endregion Delete Demo Nodes

                #region Delete Demo Views
                CswTableSelect ViewsSelect = _CswNbtResources.makeCswTableSelect( "delete_demodata_views", "node_views" );
                DataTable ViewsTable = ViewsSelect.getTable( new CswCommaDelimitedString { "nodeviewid" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
                Total += ViewsTable.Rows.Count;
                foreach( DataRow ViewRow in ViewsTable.Rows )
                {
                    Int32 ViewPk = CswConvert.ToInt32( ViewRow["nodeviewid"] );
                    string ViewName = "View: ViewId " + ViewPk;
                    try
                    {
                        CswNbtViewId ViewId = new CswNbtViewId( ViewPk );
                        CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                        if( null != View )
                        {
                            ViewName = "View: " + View.ViewName;
                            SuccessfulDeletes.Add( ViewName );
                            View.Delete();
                        }
                        else
                        {
                            FailedDeletes.Add( ViewName );
                        }
                    }
                    catch( Exception Exception )
                    {
                        FailedDeletes.Add( ViewName );
                        Exceptions.Add( Exception );
                    }
                }
                #endregion Delete Demo Views

                wsMd.finalizeOtherResources( NbtSystemResources );


                SuccessText += SuccessfulDeletes.Count + " deletes succeeded out of " + Total + " total. <br>";
                SuccessText += "The following items were successfully deleted: <br>";
                foreach( string SuccessfulDelete in SuccessfulDeletes )
                {
                    SuccessText += SuccessfulDelete + ", ";
                }

                if( FailedDeletes.Count > 0 )
                {
                    SuccessText += "<br>";
                    SuccessText += "Not all demo data was deleted. " + FailedDeletes.Count + " deletes failed out of " + Total + " total.<br>";
                    SuccessText += "The following items have not been deleted: \n";
                    foreach( string FailedDelete in FailedDeletes )
                    {
                        SuccessText += FailedDelete + ", ";
                    }
                    SuccessText += "<br><br>";
                    foreach( Exception ex in Exceptions )
                    {
                        SuccessText += ex.Message + " " + ex.InnerException + " <br><br>";
                    }
                }
            }
            Ret["message"] = SuccessText;
            Ret["succeeded"] = SuccessfulDeletes.Count;
            Ret["total"] = Total;
            Ret["failed"] = FailedDeletes.Count;

            return Ret;
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        public void addNodeProps( CswNbtNode Node, JObject PropsObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            _NodeSd.addNodeProps( Node, PropsObj, Tab );
        }

        public void addSingleNodeProp( CswNbtNode Node, JObject PropObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            _NodeSd.addSingleNodeProp( Node, PropObj, Tab );

        } // _applyPropJson

        public JObject getQuantityFromSize( CswPrimaryKey SizeId, string Action )
        {
            JObject Ret = new JObject();

            CswNbtObjClassSize Size = _CswNbtResources.Nodes.GetNode( SizeId );
            if( null != Size )
            {
                CswNbtNodePropQuantity InitialQuantity = Size.InitialQuantity;
                InitialQuantity.ToJSON( Ret );
                Ret["qtyReadonly"] = "false";
                Ret["unitReadonly"] = "false";
                Ret["unitCount"] = "1";
                Ret["isRequired"] = InitialQuantity.Required.ToString();
                if( Action.ToLower() == ChemSW.Nbt.ObjClasses.CswNbtObjClass.NbtButtonAction.receive.ToString() )
                {
                    Ret["unitReadonly"] = "true";
                    if( Size.QuantityEditable.Checked == Tristate.False )
                    {
                        Ret["qtyReadonly"] = "true";
                    }
                    Ret["unitCount"] = Size.UnitCount.Value.ToString();
                }
                else if( Action.ToLower() == ChemSW.Nbt.ObjClasses.CswNbtObjClass.NbtButtonAction.dispense.ToString() )
                {
                    CswNbtObjClassUnitOfMeasure UnitNode = _CswNbtResources.Nodes.GetNode( Size.InitialQuantity.UnitId );
                    if( null != UnitNode &&
                    ( UnitNode.UnitType.Value == CswNbtObjClassUnitOfMeasure.UnitTypes.Each.ToString() ||
                    false == CswTools.IsDouble( UnitNode.ConversionFactor.Base ) ) )
                    {
                        Ret["unitReadonly"] = "true";
                    }
                }
            }
            return Ret;
        }

        public JObject getSizeFromRelatedNodeId( CswPrimaryKey RelatedNodeId )
        {
            JObject Ret = new JObject();
            string SizeId = string.Empty;
            CswNbtNode RelatedNode = _CswNbtResources.Nodes.GetNode( RelatedNodeId );
            if( null != RelatedNode )
            {
                CswNbtNode Node = _CswNbtResources.Nodes[RelatedNodeId];
                if( null != Node )
                {
                    switch( RelatedNode.ObjClass.ObjectClass.ObjectClass )
                    {
                        case NbtObjectClass.ContainerClass:
                            CswNbtObjClassContainer NodeAsContainer = Node;
                            if( null != NodeAsContainer )
                            {
                                SizeId = NodeAsContainer.Size.RelatedNodeId.ToString();
                            }
                            break;
                        case NbtObjectClass.RequestItemClass:
                            CswNbtObjClassRequestItem NodeAsRequestItem = Node;
                            if( null != NodeAsRequestItem )
                            {
                                if( null != NodeAsRequestItem.Size.RelatedNodeId && Int32.MinValue != NodeAsRequestItem.Size.RelatedNodeId.PrimaryKey )
                                {
                                    SizeId = NodeAsRequestItem.Size.RelatedNodeId.ToString();
                                }
                                else if( null != NodeAsRequestItem.Container.RelatedNodeId && Int32.MinValue != NodeAsRequestItem.Container.RelatedNodeId.PrimaryKey )
                                {
                                    SizeId = NodeAsRequestItem.Container.RelatedNodeId.ToString();
                                }
                            }
                            break;
                        default:
                            throw new CswDniException( ErrorType.Warning, "Cannot derive a size from an instance of this type " + RelatedNode.ObjClass.ObjectClass.ObjectClass + ".", "getSizeFromRelatedNodeId does not support this Object Class." );
                    }
                }
            }
            if( false == string.IsNullOrEmpty( SizeId ) )
            {
                Ret["sizeid"] = SizeId;
            }
            return Ret;
        }

        public JObject getNodes( string NodeTypeId, string ObjectClassId, string ObjectClass, string RelatedToObjectClass, string RelatedToNodeId )
        {
            JObject Ret = new JObject();

            Int32 RealNodeTypeId = CswConvert.ToInt32( NodeTypeId );
            Int32 RealObjectClassId = CswConvert.ToInt32( ObjectClassId );
            NbtObjectClass RealObjectClass = ObjectClass;
            bool CanAdd;
            Collection<CswNbtNode> Nodes = new Collection<CswNbtNode>();
            bool UseSearch = false;
            // case 25956
            Int32 SearchThreshold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.relationshipoptionlimit.ToString() ) );

            if( SearchThreshold <= 0 )
            {
                SearchThreshold = 100;
            }

            if( RealNodeTypeId != Int32.MinValue )
            {
                CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( RealNodeTypeId );
                Nodes = MetaDataNodeType.getNodes( true, false );
                CanAdd = _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, MetaDataNodeType );
            }
            else
            {
                CswNbtMetaDataObjectClass MetaDataObjectClass = null;
                if( RealObjectClassId != Int32.MinValue )
                {
                    MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClassId );
                }
                else if( RealObjectClass != CswNbtResources.UnknownEnum )
                {
                    MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClass );
                    RealObjectClassId = MetaDataObjectClass.ObjectClassId;
                }
                if( null != MetaDataObjectClass )
                {
                    bool doGetNodes = true;
                    if( false == string.IsNullOrEmpty( RelatedToObjectClass ) && false == string.IsNullOrEmpty( RelatedToNodeId ) )
                    {
                        NbtObjectClass RealRelatedObjectClass = RelatedToObjectClass;
                        CswPrimaryKey RelatedNodePk = new CswPrimaryKey();
                        RelatedNodePk.FromString( RelatedToNodeId );
                        if( Int32.MinValue != RelatedNodePk.PrimaryKey )
                        {
                            CswNbtNode RelatedNode = _CswNbtResources.Nodes[RelatedNodePk];
                            if( null != RelatedNode )
                            {
                                if( RelatedNode.ObjClass.ObjectClass.ObjectClass == RealRelatedObjectClass )
                                {
                                    Collection<CswNbtMetaDataObjectClassProp> RelatedProps = new Collection<CswNbtMetaDataObjectClassProp>();
                                    CswNbtMetaDataObjectClass MetaRelatedObjectClass = _CswNbtResources.MetaData.getObjectClass( RealRelatedObjectClass );
                                    Ret["relatedobjectclassid"] = MetaRelatedObjectClass.ObjectClassId;
                                    foreach( CswNbtMetaDataObjectClassProp OcProp in from _OcProp in MetaDataObjectClass.getObjectClassProps()
                                                                                     where _OcProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                                                                                           _OcProp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                                                                                           _OcProp.FKValue == MetaRelatedObjectClass.ObjectClassId
                                                                                     select _OcProp )
                                    {
                                        RelatedProps.Add( OcProp );
                                    }

                                    if( RelatedProps.Any() )
                                    {
                                        doGetNodes = false;
                                        CswNbtView View = new CswNbtView( _CswNbtResources );
                                        CswNbtViewRelationship Relationship = View.AddViewRelationship( MetaDataObjectClass, true );
                                        foreach( CswNbtMetaDataObjectClassProp RelationshipProp in RelatedProps )
                                        {
                                            View.AddViewPropertyAndFilter( Relationship, RelationshipProp, SubFieldName: CswNbtSubField.SubFieldName.NodeID, Value: RelatedNodePk.PrimaryKey.ToString() );
                                        }
                                        ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, false, false );

                                        UseSearch = UseSearch || Tree.getChildNodeCount() > SearchThreshold;
                                        for( int N = 0; N < Tree.getChildNodeCount() && N < SearchThreshold; N += 1 )
                                        {
                                            Tree.goToNthChild( N );
                                            Ret[Tree.getNodeIdForCurrentPosition().ToString()] = Tree.getNodeNameForCurrentPosition();
                                            Tree.goToParentNode();
                                        }
                                    }
                                }

                            }
                        }
                    }
                    if( doGetNodes )
                    {
                        Nodes = MetaDataObjectClass.getNodes( true, false );
                    }
                    CanAdd = MetaDataObjectClass.getLatestVersionNodeTypes().Aggregate( false, ( current, NodeType ) => current || _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType ) );
                }
                else
                {
                    CanAdd = false;
                }
            }
            UseSearch = UseSearch || Nodes.Count > SearchThreshold;
            foreach( CswNbtNode Node in ( from _Node in Nodes orderby _Node.NodeName select _Node ).Take( SearchThreshold ) )
            {
                Ret[Node.NodeId.ToString()] = Node.NodeName;
            }
            Ret["usesearch"] = UseSearch;
            Ret["canadd"] = CanAdd;
            Ret["nodetypeid"] = RealNodeTypeId;
            Ret["objectclassid"] = RealObjectClassId;

            return Ret;
        }

    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
