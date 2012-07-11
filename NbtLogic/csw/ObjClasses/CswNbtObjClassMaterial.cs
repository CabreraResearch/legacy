using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterial : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMaterial( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ); }
        }

        public const string SupplierPropertyName = "Supplier";
        public const string ApprovalStatusPropertyName = "Approval Status";
        public const string PartNumberPropertyName = "Part Number";
        public const string SpecificGravityPropertyName = "Specific Gravity";
        public const string PhysicalStatePropertyName = "Physical State";
        public const string CasNoPropertyName = "CAS No";
        public const string RegulatoryListsPropertyName = "Regulatory Lists";
        public const string TradenamePropertyName = "Tradename";
        public const string StorageCompatibilityPropertyName = "Storage Compatibility";
        public const string ExpirationIntervalPropertyName = "Expiration Interval";
        public const string RequestPropertyName = "Request";
        public const string ReceivePropertyName = "Receive";

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterial
        /// </summary>
        public static implicit operator CswNbtObjClassMaterial( CswNbtNode Node )
        {
            CswNbtObjClassMaterial ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) )
            {
                ret = (CswNbtObjClassMaterial) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( ApprovalStatus.WasModified )
            {
                Receive.Hidden = ApprovalStatus.Checked != Tristate.True;
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                switch( OCP.PropName )
                {
                    case RequestPropertyName:
                        CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart );

                        CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeRequestItem( new CswNbtActSubmitRequest.RequestItem( CswNbtActSubmitRequest.RequestItem.Material ), NodeId, OCP );
                        ButtonData.Data["requestaction"] = OCP.PropName;
                        ButtonData.Data["titleText"] = "Request for " + TradeName.Text;
                        ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsRequestItem );
                        ButtonData.Data["requestItemNodeTypeId"] = RequestAct.RequestItemNt.NodeTypeId;
                        ButtonData.Action = NbtButtonAction.request;
                        break;
                    case ReceivePropertyName:
                        ButtonData.Data["materialId"] = NodeId.ToString();
                        ButtonData.Data["materialNodeTypeId"] = NodeTypeId;
                        ButtonData.Data["tradeName"] = TradeName.Text;
                        CswNbtActReceiving Act = new CswNbtActReceiving( _CswNbtResources, ObjectClass, NodeId );
                        ButtonData.Data["sizesViewId"] = Act.SizesView.SessionViewId.ToString();
                        Int32 ContainerLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.container_receipt_limit.ToString() ) );
                        ButtonData.Data["containerlimit"] = ContainerLimit;
                        CswNbtObjClassContainer Container = Act.makeContainer();
                        ButtonData.Data["containerNodeTypeId"] = Container.NodeTypeId;
                        ButtonData.Data["containerAddLayout"] = Act.getContainerAddProps( Container );
                        ButtonData.Action = NbtButtonAction.receive;
                        break;
                }
            }

            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Supplier { get { return ( _CswNbtNode.Properties[SupplierPropertyName] ); } }
        public CswNbtNodePropLogical ApprovalStatus { get { return ( _CswNbtNode.Properties[ApprovalStatusPropertyName] ); } }
        public CswNbtNodePropText PartNumber { get { return ( _CswNbtNode.Properties[PartNumberPropertyName] ); } }
        public CswNbtNodePropNumber SpecificGravity { get { return ( _CswNbtNode.Properties[SpecificGravityPropertyName] ); } }
        public CswNbtNodePropList PhysicalState { get { return ( _CswNbtNode.Properties[PhysicalStatePropertyName] ); } }
        public CswNbtNodePropText CasNo { get { return ( _CswNbtNode.Properties[CasNoPropertyName] ); } }
        public CswNbtNodePropStatic RegulatoryLists { get { return ( _CswNbtNode.Properties[RegulatoryListsPropertyName] ); } }
        public CswNbtNodePropText TradeName { get { return ( _CswNbtNode.Properties[TradenamePropertyName] ); } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[StorageCompatibilityPropertyName] ); } }
        public CswNbtNodePropQuantity ExpirationInterval { get { return ( _CswNbtNode.Properties[ExpirationIntervalPropertyName] ); } }
        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[RequestPropertyName] ); } }
        public CswNbtNodePropButton Receive { get { return ( _CswNbtNode.Properties[ReceivePropertyName] ); } }

        #endregion

    }//CswNbtObjClassMaterial

}//namespace ChemSW.Nbt.ObjClasses
