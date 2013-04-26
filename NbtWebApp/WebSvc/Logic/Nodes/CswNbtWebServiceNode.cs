using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using NbtWebApp;
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
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( NodeKey );
                    if( null != NbtNodeKey &&
                        null != NbtNodeKey.NodeId &&
                        CswTools.IsPrimaryKey( NbtNodeKey.NodeId ) &&
                        false == NodePrimaryKeys.Contains( NbtNodeKey.NodeId ) )
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
                    if( CswTools.IsPrimaryKey( PrimaryKey ) &&
                        false == NodePrimaryKeys.Contains( PrimaryKey ) )
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

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId, string SelectedText, string TabId, JObject ReturnProps, string NodeIds, string PropIds )
        {
            return _NodeSd.doObjectClassButtonClick( PropId, SelectedText, TabId, ReturnProps, NodeIds, PropIds );
        }

        private JObject _makeDeletedNodeText( CswNbtMetaDataNodeType NodeType, string NodeName, Int32 NodeId, CswNbtNode Node = null )
        {
            string Type = CswNbtResources.UnknownEnum;
            if( null != NodeType )
            {
                Type = NodeType.NodeTypeName;
            }
            string Name = CswNbtResources.UnknownEnum;
            if( false == string.IsNullOrEmpty( NodeName ) )
            {
                Name = NodeName;
            }
            else if( null != Node )
            {
                Name = Node.NodeName;
            }
            string Link = string.Empty;
            if( null != Node )
            {
                Link = Node.NodeLink;
            }
            return _makeDeletedItemText( Type, Name, NodeId, Link );
        }

        private JObject _makeDeletedItemText( string Type, string Name, Int32 Id, string Link = null )
        {
            JObject Ret = new JObject();
            if( false == string.IsNullOrEmpty( Type ) )
            {
                Ret["type"] = Type;
            }
            else
            {
                Ret["type"] = CswNbtResources.UnknownEnum;
            }
            if( false == string.IsNullOrEmpty( Name ) )
            {
                Ret["name"] = Name;
            }
            else
            {
                Ret["name"] = CswNbtResources.UnknownEnum;
            }
            if( false == string.IsNullOrEmpty( Link ) )
            {
                Ret["link"] = Link;
            }
            Ret["id"] = Id;

            return Ret;
        }

        private void _tryDeleteNode( CswNbtResources NbtResources, DataRow NodeRow, JObject RetObj, Collection<Exception> Exceptions )
        {
            try
            {
                string DoomedNodeName = CswConvert.ToString( NodeRow["nodename"] );
                Int32 DoomedNodeId = CswConvert.ToInt32( NodeRow["nodeid"] );
                Int32 NodeTypeId = CswConvert.ToInt32( NodeRow["nodetypeid"] );
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( NodeTypeId );
                CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( DoomedNodeId ) );
                CswNbtNode DoomedNode = NbtResources.Nodes[NodePk];
                try
                {
                    if( null == DoomedNode )
                    {
                        RetObj["failed"][NodePk.ToString()] = _makeDeletedNodeText( NodeType, DoomedNodeName, DoomedNodeId, DoomedNode );
                    }
                    else
                    {
                        DoomedNode.delete( DeleteAllRequiredRelatedNodes: true );
                        RetObj["succeeded"][NodePk.ToString()] = _makeDeletedNodeText( NodeType, DoomedNodeName, DoomedNodeId );
                    }
                }
                catch( Exception Exception )
                {
                    RetObj["failed"][NodePk.ToString()] = _makeDeletedNodeText( NodeType, DoomedNodeName, DoomedNodeId, DoomedNode );
                    Exceptions.Add( Exception );
                }
            }
            catch( Exception Exception )
            {
                Exceptions.Add( Exception );
            }
        }

        public JObject deleteDemoDataNodes()
        {
            JObject Ret = new JObject();
            Int32 Total = 0;
            Int32 SuccessCount = 0;
            Int32 FailedCount = 0;

            Ret["counts"] = new JObject();

            Ret["successtext"] = string.Empty;
            Ret["succeeded"] = new JObject();

            Ret["failedtext"] = string.Empty;
            Ret["failed"] = new JObject();

            Ret["exceptions"] = new JArray();

            Collection<Exception> Exceptions = new Collection<Exception>();

            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                // Get a new CswNbtResources as the System User
                CswNbtWebServiceMetaData wsMd = new CswNbtWebServiceMetaData( _CswNbtResources );
                CswNbtResources NbtSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );

                try
                {
                    //Reassign required relationships which may be tied to Demo data
                    CswNbtResources UserSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );
                    CswNbtMetaDataObjectClass UserOc = UserSystemResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
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

                CswTableSelect NodesSelect = NbtSystemResources.makeCswTableSelect( "delete_demodata_nodes1", "nodes" );
                DataTable NodesTable = NodesSelect.getTable(
                    SelectColumns: new CswCommaDelimitedString { "nodeid", "nodename", "nodetypeid" },
                    WhereClause: "where isdemo='" + CswConvert.ToDbVal( true ) + "'",
                    OrderByColumns: new Collection<OrderByClause> { new OrderByClause( "nodeid", CswEnumOrderByType.Descending ) }
                );
                Total = NodesTable.Rows.Count;
                foreach( DataRow NodeRow in NodesTable.Rows )
                {
                    _tryDeleteNode( NbtSystemResources, NodeRow, Ret, Exceptions );
                }

                #endregion Delete Demo Nodes

                #region Delete Demo Views
                CswTableSelect ViewsSelect = NbtSystemResources.makeCswTableSelect( "delete_demodata_views", "node_views" );
                DataTable ViewsTable = ViewsSelect.getTable( new CswCommaDelimitedString { "nodeviewid", "viewname" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
                Total += ViewsTable.Rows.Count;
                foreach( DataRow ViewRow in ViewsTable.Rows )
                {
                    Int32 ViewPk = CswConvert.ToInt32( ViewRow["nodeviewid"] );
                    string ViewName = CswConvert.ToString( ViewRow["viewname"] );
                    try
                    {
                        CswNbtViewId ViewId = new CswNbtViewId( ViewPk );
                        CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ViewId );
                        if( null != View )
                        {
                            View.Delete();
                            Ret["succeeded"][View.ViewId.ToString()] = _makeDeletedItemText( "View", ViewName, ViewPk );
                        }
                        else
                        {
                            Ret["failed"][ViewPk] = _makeDeletedItemText( "View", ViewName, ViewPk );
                        }
                    }
                    catch( Exception Exception )
                    {
                        Ret["failed"][ViewPk] = _makeDeletedItemText( "View", ViewName, ViewPk );
                        Exceptions.Add( Exception );
                    }
                }
                #endregion Delete Demo Views

                wsMd.finalizeOtherResources( NbtSystemResources );

                SuccessCount = ( (JObject) Ret["succeeded"] ).Count;
                Ret["successtext"] = SuccessCount + " deletes succeeded out of " + Total + " total. <br>";

                FailedCount = ( (JObject) Ret["failed"] ).Count;
                if( FailedCount > 0 )
                {
                    Ret["failedtext"] = "Not all demo data was deleted. " + FailedCount + " deletes failed out of " + Total + " total.<br>";

                    foreach( Exception ex in Exceptions )
                    {
                        ( (JArray) Ret["exceptions"] ).Add( ex.Message + ": " + ex.InnerException );
                    }
                }
            }

            Ret["counts"]["succeeded"] = SuccessCount;
            Ret["counts"]["total"] = Total;
            Ret["counts"]["failed"] = FailedCount;

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
                Ret["unitName"] = Ret["name"];
                Ret["qtyReadonly"] = false;
                Ret["unitReadonly"] = "false";
                Ret["unitCount"] = "1";
                Ret["isRequired"] = InitialQuantity.Required.ToString();
                if( Action.ToLower() == ChemSW.Nbt.ObjClasses.CswEnumNbtButtonAction.receive.ToString() )
                {
                    Ret["unitReadonly"] = "true";
                    if( Size.QuantityEditable.Checked == CswEnumTristate.False )
                    {
                        Ret["qtyReadonly"] = true;
                    }
                    Ret["unitCount"] = CswTools.IsDouble( Size.UnitCount.Value ) ? Size.UnitCount.Value.ToString() : "";
                }
                else if( Action.ToLower() == ChemSW.Nbt.ObjClasses.CswEnumNbtButtonAction.dispense.ToString() )
                {
                    CswNbtObjClassUnitOfMeasure UnitNode = _CswNbtResources.Nodes.GetNode( Size.InitialQuantity.UnitId );
                    if( null != UnitNode &&
                    ( UnitNode.UnitType.Value == CswEnumNbtUnitTypes.Each.ToString() ||
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
                        case CswEnumNbtObjectClass.ContainerClass:
                            CswNbtObjClassContainer NodeAsContainer = Node;
                            if( null != NodeAsContainer )
                            {
                                SizeId = NodeAsContainer.Size.RelatedNodeId.ToString();
                            }
                            break;
                        case CswEnumNbtObjectClass.RequestContainerDispenseClass:
                            CswNbtObjClassRequestContainerDispense NodeAsCd = Node;
                            if( null != NodeAsCd )
                            {
                                if( null != NodeAsCd.Size.RelatedNodeId && Int32.MinValue != NodeAsCd.Size.RelatedNodeId.PrimaryKey )
                                {
                                    SizeId = NodeAsCd.Size.RelatedNodeId.ToString();
                                }
                                if( null != NodeAsCd.Container.RelatedNodeId && Int32.MinValue != NodeAsCd.Container.RelatedNodeId.PrimaryKey )
                                {
                                    SizeId = NodeAsCd.Container.RelatedNodeId.ToString();
                                }
                            }
                            break;
                        case CswEnumNbtObjectClass.RequestMaterialDispenseClass:
                            CswNbtObjClassRequestMaterialDispense NodeAsMd = Node;
                            if( null != NodeAsMd )
                            {
                                if( null != NodeAsMd.Size.RelatedNodeId && Int32.MinValue != NodeAsMd.Size.RelatedNodeId.PrimaryKey )
                                {
                                    SizeId = NodeAsMd.Size.RelatedNodeId.ToString();
                                }

                            }
                            break;
                        default:
                            throw new CswDniException( CswEnumErrorType.Warning, "Cannot derive a size from an instance of this type " + RelatedNode.ObjClass.ObjectClass.ObjectClass + ".", "getSizeFromRelatedNodeId does not support this Object Class." );
                    }
                }
            }
            if( false == string.IsNullOrEmpty( SizeId ) )
            {
                Ret["sizeid"] = SizeId;
            }
            return Ret;
        }

        /// <summary>
        /// WCF wrapper around getNodes
        /// </summary>
        public static void getNodes( ICswResources CswResources, NodeResponse Response, NodeSelect.Request Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtSdNode sd = new CswNbtSdNode( NbtResources );
                Response.Data = sd.getNodes( Request );
            }
        }

        public static void getSizes( ICswResources CswResources, NodeResponse Response, CswNbtNode.Node Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;

                CswPrimaryKey pk = CswConvert.ToPrimaryKey( Request.NodeId );
                if( CswTools.IsPrimaryKey( pk ) )
                {
                    CswNbtMetaDataObjectClass sizeOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                    CswNbtMetaDataObjectClassProp materialOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );

                    CswNbtView sizesView = new CswNbtView( NbtResources );
                    CswNbtViewRelationship parent = sizesView.AddViewRelationship( sizeOC, true );
                    sizesView.AddViewPropertyAndFilter( parent,
                        MetaDataProp: materialOCP,
                        Value: pk.PrimaryKey.ToString(),
                        SubFieldName: CswEnumNbtSubFieldName.NodeID,
                        FilterMode: CswEnumNbtFilterMode.Equals );

                    ICswNbtTree tree = NbtResources.Trees.getTreeFromView( sizesView, true, false, false );
                    for( int i = 0; i < tree.getChildNodeCount(); i++ )
                    {
                        tree.goToNthChild( i );
                        Response.Data.Nodes.Add( new CswNbtNode.Node( null )
                        {
                            NodeId = tree.getNodeIdForCurrentPosition(),
                            NodeName = tree.getNodeNameForCurrentPosition()
                        } );
                        tree.goToParentNode();
                    }

                }
            }
        }

    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
