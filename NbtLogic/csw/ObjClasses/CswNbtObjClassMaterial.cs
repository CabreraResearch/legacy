using System;
using ChemSW.Core;
using ChemSW.Exceptions;
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

        public sealed class PhysicalStates
        {
            public const string NA = "n/a";
            public const string Liquid = "liquid";
            public const string Solid = "solid";
            public const string Gas = "gas";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Solid, Liquid, NA };
        }

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
                Receive.setHidden( value: ApprovalStatus.Checked != Tristate.True, SaveToDb: true );
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
            _toggleButtonVisibility();
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        private void _toggleButtonVisibility()
        {
            Receive.setHidden( value: false == _CswNbtResources.Permit.can( CswNbtActionName.Receiving ), SaveToDb: false );
            Request.setHidden( value: false == _CswNbtResources.Permit.can( CswNbtActionName.Submit_Request ), SaveToDb: false );
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                bool HasPermission = false;
                switch( OCP.PropName )
                {
                    case RequestPropertyName:
                        if( _CswNbtResources.Permit.can( CswNbtActionName.Submit_Request ) )
                        {
                            HasPermission = true;
                            CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true );

                            CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeMaterialRequestItem( new CswNbtActSubmitRequest.RequestItem( CswNbtActSubmitRequest.RequestItem.Material ), NodeId, OCP );
                            NodeAsRequestItem.postChanges( false );
                            ButtonData.Data["requestaction"] = OCP.PropName;
                            ButtonData.Data["titleText"] = "Request for " + TradeName.Text;
                            ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsRequestItem );
                            ButtonData.Data["requestItemNodeTypeId"] = RequestAct.RequestItemNt.NodeTypeId;
                            ButtonData.Action = NbtButtonAction.request;
                        }
                        break;
                    case ReceivePropertyName:
                        if( _CswNbtResources.Permit.can( CswNbtActionName.Receiving ) )
                        {
                            HasPermission = true;
                            ButtonData.Data["state"] = new JObject();
                            ButtonData.Data["state"]["materialId"] = NodeId.ToString();
                            ButtonData.Data["state"]["materialNodeTypeId"] = NodeTypeId;
                            ButtonData.Data["state"]["tradeName"] = TradeName.Text;
                            CswNbtActReceiving Act = new CswNbtActReceiving( _CswNbtResources, ObjectClass, NodeId );
                            //ButtonData.Data["sizesViewId"] = Act.SizesView.SessionViewId.ToString();
                            Int32 ContainerLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.container_receipt_limit.ToString() ) );
                            ButtonData.Data["state"]["containerlimit"] = ContainerLimit;
                            CswNbtObjClassContainer Container = Act.makeContainer();
                            Container.Location.SelectedNodeId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                            ButtonData.Data["state"]["containerNodeTypeId"] = Container.NodeTypeId;
                            ButtonData.Data["state"]["containerAddLayout"] = Act.getContainerAddProps( Container );
                            bool customBarcodes = CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.custom_barcodes.ToString() ) );
                            ButtonData.Data["state"]["customBarcodes"] = customBarcodes;

                            CswDateTime CswDate = new CswDateTime( _CswNbtResources, getDefaultExpirationDate() );
                            if( false == CswDate.IsNull )
                            {
                                foreach( JProperty child in ButtonData.Data["state"]["containerAddLayout"].Children() )
                                {
                                    JToken name = child.First.SelectToken( "name" );
                                    if( name.ToString() == "Expiration Date" )
                                    {
                                        ButtonData.Data["state"]["containerAddLayout"][child.Name]["values"]["value"] = CswDate.ToClientAsDateTimeJObject();
                                    }
                                }
                            }

                            ButtonData.Action = NbtButtonAction.receive;
                        }
                        break;
                }
                if( false == HasPermission )
                {
                    throw new CswDniException( ErrorType.Warning, "You do not have permission to the " + OCP.PropName + " action.", "You do not have permission to the " + OCP.PropName + " action." );
                }
            }

            return true;
        }
        #endregion

        #region Custom Logic

        /// <summary>
        /// Calculates the expiration date from today based on the Material's Expiration Interval
        /// </summary>
        public DateTime getDefaultExpirationDate()
        {
            DateTime DefaultExpDate = DateTime.Now;
            switch( this.ExpirationInterval.CachedUnitName.ToLower() )
            {
                case "seconds":
                    DefaultExpDate = DefaultExpDate.AddSeconds( this.ExpirationInterval.Quantity );
                    break;
                case "minutes":
                    DefaultExpDate = DefaultExpDate.AddMinutes( this.ExpirationInterval.Quantity );
                    break;
                case "hours":
                    DefaultExpDate = DefaultExpDate.AddHours( this.ExpirationInterval.Quantity );
                    break;
                case "days":
                    DefaultExpDate = DefaultExpDate.AddDays( this.ExpirationInterval.Quantity );
                    break;
                case "weeks":
                    DefaultExpDate = DefaultExpDate.AddDays( this.ExpirationInterval.Quantity * 7 );
                    break;
                case "months":
                    DefaultExpDate = DefaultExpDate.AddMonths( CswConvert.ToInt32( this.ExpirationInterval.Quantity ) );
                    break;
                case "years":
                    DefaultExpDate = DefaultExpDate.AddYears( CswConvert.ToInt32( this.ExpirationInterval.Quantity ) );
                    break;
                default:
                    DefaultExpDate = DateTime.MinValue;
                    break;
            }
            return DefaultExpDate;
        }

        #endregion Custom Logic

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
