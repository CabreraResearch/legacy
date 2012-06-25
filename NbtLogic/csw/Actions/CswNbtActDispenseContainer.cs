using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.csw.Actions
{
    public class CswNbtActDispenseContainer
    {
        private CswNbtResources _CswNbtResources = null;

        #region Constructor

        public CswNbtActDispenseContainer( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }
        }

        #endregion Constructor

        #region Public Methods

        public JObject getRequestHistory()
        {
            JObject Ret = new JObject();

            //ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestHistoryView, true, false );
            //Int32 RequestCount = Tree.getChildNodeCount();
            //Ret["count"] = RequestCount;
            //if( RequestCount > 0 )
            //{
            //    for( Int32 I = 0; I < RequestCount; I += 1 )
            //    {
            //        Tree.goToNthChild( I );

            //        Ret[Tree.getNodeNameForCurrentPosition()] = new JObject();
            //        Ret[Tree.getNodeNameForCurrentPosition()]["requestnodeid"] = Tree.getNodeIdForCurrentPosition().ToString();
            //        foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
            //        {
            //            string PropName = Prop["propname"].ToString().ToLower();
            //            Ret[Tree.getNodeNameForCurrentPosition()][PropName] = Prop["gestalt"].ToString();
            //        }

            //        Tree.goToParentNode();
            //    }
            //}
            return Ret;
        }

        public JObject submitRequest( CswPrimaryKey NodeId, string NodeName )
        {
            JObject Ret = new JObject();
            if( null != NodeId )
            {
                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( NodeId );
                if( null != NodeAsRequest )
                {
                    NodeAsRequest.SubmittedDate.DateTimeValue = DateTime.Now;
                    NodeAsRequest.Name.Text = NodeName;
                    NodeAsRequest.postChanges( true );
                    Ret["succeeded"] = true;
                }
            }

            return Ret;
        }

        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public JObject getRequestItemAddProps( CswNbtObjClassRequestItem RetAsRequestItem )
        {
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            _CswNbtResources.EditMode = NodeEditMode.Add;

            return PropsAction.getProps( RetAsRequestItem.Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
        }

        public JObject updateDispensedContainer( string SourceContainerNodeId, string DispenseType, string Quantity )
        {
            JObject ret = new JObject();
            if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Add.ToString() )
            {
                ret = _addMaterialToContainer( SourceContainerNodeId, Quantity );
            }
            else if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste.ToString() )
            {
                ret = _wasteMaterialFromContainer( SourceContainerNodeId, Quantity );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Failed to dispense container: Dispense type not supported.", "Invalid Dispense Type." );
            }
            return ret;
        }

        private JObject _addMaterialToContainer( string SourceContainerNodeId, string Quantity )
        {
            //TODO - add quantity to source container, create transaction node
            return new JObject();
        }

        private JObject _wasteMaterialFromContainer( string SourceContainerNodeId, string Quantity )
        {
            //TODO - waste quantity from source container, create transaction node
            return new JObject();
        }

        public JObject upsertDispenseContainers( string SourceContainerNodeId, string ContainerNodeTypeId, string DesignGrid )
        {
            JArray GridArray = JArray.Parse( DesignGrid );
            return _upsertDispenseContainers( SourceContainerNodeId, ContainerNodeTypeId, GridArray );
        }

        private JObject _upsertDispenseContainers( string SourceContainerNodeId, string ContainerNodeTypeId, JArray DesignGrid )
        {
            //TODO - create distination containers with respective quantities, create transaction nodes for each dispense instance, update source container
            return new JObject();
        }

        #endregion Public Methods
    }
}
