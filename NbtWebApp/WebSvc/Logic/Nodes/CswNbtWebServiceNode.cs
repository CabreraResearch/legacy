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
                    CswPrimaryKey PrimaryKey = new CswPrimaryKey();
                    PrimaryKey.FromString( NodePk );
                    if( null != PrimaryKey && !NodePrimaryKeys.Contains( PrimaryKey ) )
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
                    foreach( CswPrimaryKey Npk in NodePrimaryKeys )
                    {
                        success = DeleteNode( Npk ) && success;
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

        public bool DeleteNode( CswPrimaryKey NodePk, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _NodeSd.DeleteNode( NodePk, DeleteAllRequiredRelatedNodes );
        }

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId, string SelectedText )
        {
            return _NodeSd.doObjectClassButtonClick( PropId, SelectedText );
        }

        public JObject deleteDemoDataNodes()
        {
            JObject Ret = new JObject();
            Int32 Succeeded = 0;
            Int32 Total = 0;
            Int32 Failed = 0;
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                /* Get a new CswNbtResources as the System User */
                CswNbtWebServiceMetaData wsMd = new CswNbtWebServiceMetaData( _CswNbtResources );
                CswNbtResources NbtSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );
                Collection<Exception> Exceptions = new Collection<Exception>();
                try
                {
                    CswNbtResources UserSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );
                    CswNbtMetaDataObjectClass UserOc = UserSystemResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                    foreach( CswNbtObjClassUser User in UserOc.getNodes( forceReInit: true, includeSystemNodes: false ) )
                    {
                        User.WorkUnitProperty.RelatedNodeId = null;
                        User.DefaultLocationProperty.SelectedNodeId = null;
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
                    try
                    {
                        CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) );
                        if( _NodeSd.DeleteNode( NodePk, DeleteAllRequiredRelatedNodes: true ) )
                        {
                            Succeeded += 1;
                        }
                    }
                    catch( Exception Exception )
                    {
                        Failed += 1;
                        Exceptions.Add( Exception );
                    }
                }
                #endregion Delete Demo Nodes

                #region Delete Demo Views
                CswTableSelect ViewsSelect = _CswNbtResources.makeCswTableSelect( "delete_demodata_views", "node_views" );
                DataTable ViewsTable = ViewsSelect.getTable( new CswCommaDelimitedString { "nodeviewid" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
                Total += ViewsTable.Rows.Count;
                foreach( DataRow ViewRow in ViewsTable.Rows )
                {
                    try
                    {
                        CswNbtViewId ViewId = new CswNbtViewId( CswConvert.ToInt32( ViewRow["nodeviewid"] ) );
                        CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                        View.Delete();
                        Succeeded += 1;
                    }
                    catch( Exception Exception )
                    {
                        Failed += 1;
                        Exceptions.Add( Exception );
                    }
                }
                #endregion Delete Demo Views

                wsMd.finalizeOtherResources( NbtSystemResources );
                if( Exceptions.Count > 0 )
                {
                    string ExceptionText = "";
                    foreach( Exception ex in Exceptions )
                    {
                        ExceptionText += ex.Message + " " + ex.InnerException + " /n";
                    }
                    throw new CswDniException( ErrorType.Warning, "Not all demo data nodes were deleted. " + Failed + " failed out of " + Total + " total.", "The following exception(s) occurred: " + ExceptionText );
                }
            }
            Ret["succeeded"] = Succeeded;
            Ret["total"] = Total;
            Ret["failed"] = Failed;

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
                CswNbtNodePropQuantity Capacity = Size.InitialQuantity;
                Capacity.ToJSON( Ret );
                Ret["qtyReadonly"] = "false";
                Ret["unitReadonly"] = "false";
                Ret["unitCount"] = "1";
                if( Action.ToLower() == ChemSW.Nbt.ObjClasses.CswNbtObjClass.NbtButtonAction.receive.ToString() )
                {
                    Ret["unitReadonly"] = "true";
                    if( Size.QuantityEditable.Checked == Tristate.False )
                    {
                        Ret["qtyReadonly"] = "true";
                    }
                    Ret["unitCount"] = Size.UnitCount.Value.ToString();
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
                        case CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass:
                            CswNbtObjClassContainer NodeAsContainer = Node;
                            if( null != NodeAsContainer )
                            {
                                SizeId = NodeAsContainer.Size.RelatedNodeId.ToString();
                            }
                            break;
                        case CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass:
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
            CswNbtMetaDataObjectClass.NbtObjectClass RealObjectClass;
            Enum.TryParse( ObjectClass, true, out RealObjectClass );
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
                else if( RealObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.Unknown )
                {
                    MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( RealObjectClass );
                    RealObjectClassId = MetaDataObjectClass.ObjectClassId;
                }
                if( null != MetaDataObjectClass )
                {
                    bool doGetNodes = true;
                    if( false == string.IsNullOrEmpty( RelatedToObjectClass ) && false == string.IsNullOrEmpty( RelatedToNodeId ) )
                    {
                        CswNbtMetaDataObjectClass.NbtObjectClass RealRelatedObjectClass;
                        Enum.TryParse( RelatedToObjectClass, true, out RealRelatedObjectClass );
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
                                        ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, false );

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
