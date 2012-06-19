using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainer : CswNbtObjClass
    {
        public const string BarcodePropertyName = "Barcode";
        public const string MaterialPropertyName = "Material";
        public const string LocationPropertyName = "Location";
        public const string LocationVerifiedPropertyName = "Location Verified";
        public const string StatusPropertyName = "Status";
        public const string MissingPropertyName = "Missing";
        public const string DisposedPropertyName = "Disposed";
        public const string SourceContainerPropertyName = "Source Container";
        public const string QuantityPropertyName = "Quantity";
        public const string ExpirationDatePropertyName = "Expiration Date";
        public const string SizePropertyName = "Size";
        public const string DispensePropertyName = "Dispense";
        public const string DisposePropertyName = "Dispose";
        public const string MovePropertyName = "Move";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainer( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainer
        /// </summary>
        public static implicit operator CswNbtObjClassContainer( CswNbtNode Node )
        {
            CswNbtObjClassContainer ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass ) )
            {
                ret = (CswNbtObjClassContainer) Node.ObjClass;
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
            Dispose.Hidden = ( Disposed.Checked == Tristate.True );
            Dispense.Hidden = ( Disposed.Checked == Tristate.True || Missing.Checked == Tristate.True || Quantity.Quantity <= 0 );
            Move.Hidden = ( Disposed.Checked == Tristate.True );

            if( Material.RelatedNodeId != null )
            {
                CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
                if( MaterialNode != null )
                {
                    CswNbtObjClassMaterial MaterialNodeAsMaterial = (CswNbtObjClassMaterial) MaterialNode;

                    // case 24488 - Expiration Date default is Today + Expiration Interval of the Material
                    // I'd like to do this on beforeCreateNode(), but the Material isn't set yet.
                    if( ExpirationDate.DateTimeValue == DateTime.MinValue )
                    {
                        DateTime DefaultExpDate = DateTime.Now;
                        switch( MaterialNodeAsMaterial.ExpirationInterval.CachedUnitName.ToLower() )
                        {
                            case "hours":
                                DefaultExpDate = DefaultExpDate.AddHours( MaterialNodeAsMaterial.ExpirationInterval.Quantity );
                                break;
                            case "days":
                                DefaultExpDate = DefaultExpDate.AddDays( MaterialNodeAsMaterial.ExpirationInterval.Quantity );
                                break;
                            case "months":
                                DefaultExpDate = DefaultExpDate.AddMonths( CswConvert.ToInt32( MaterialNodeAsMaterial.ExpirationInterval.Quantity ) );
                                break;
                            case "years":
                                DefaultExpDate = DefaultExpDate.AddYears( CswConvert.ToInt32( MaterialNodeAsMaterial.ExpirationInterval.Quantity ) );
                                break;
                            default:
                                DefaultExpDate = DateTime.MinValue;
                                break;
                        }
                        ExpirationDate.DateTimeValue = DefaultExpDate;
                    }

                    // case 24488 - When Location is modified, verify that:
                    //  the Material's Storage Compatibility is null,
                    //  or the Material's Storage Compatibility is one the selected values in the new Location.
                    if( Location.WasModified )
                    {
                        // Waiting on case 24441
                    }
                }
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
            // Disposed == false
            CswNbtMetaDataObjectClassProp DisposedOCP = ObjectClass.getObjectClassProp( DisposedPropertyName );

            //ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, DisposedOCP, Tristate.False.ToString() );

            CswNbtViewProperty viewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, DisposedOCP );
            viewProp.ShowInGrid = false;
            ParentRelationship.View.AddViewPropertyFilter( viewProp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, Value: Tristate.False.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
            if( null != NodeTypeProp && null != OCP )
            {
                CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart );

                CswNbtObjClassRequestItem NodeAsRequestItem = RequestAct.makeRequestItem( new CswNbtActSubmitRequest.RequestItem(), NodeId, OCP );
                NodeAsRequestItem.Material.RelatedNodeId = Material.RelatedNodeId;
                NodeAsRequestItem.Material.ReadOnly = true;
                switch( OCP.PropName )
                {
                    case DispensePropertyName:
                        break;
                    case DisposePropertyName:
                        NodeAsRequestItem.Material.Hidden = true;
                        NodeAsRequestItem.postChanges( true ); /* This is the only condition in which we want to commit the node upfront. */
                        break;
                    case MovePropertyName:
                        NodeAsRequestItem.Material.Hidden = true;
                        break;
                }

                JObject ActionDataObj = new JObject();
                ActionDataObj["requestaction"] = OCP.PropName;
                ActionDataObj["titleText"] = OCP.PropName + " Request for " + Material.CachedNodeName;
                ActionDataObj["requestItemProps"] = RequestAct.getRequestItemAddProps( NodeAsRequestItem );
                ActionDataObj["requestItemNodeTypeId"] = RequestAct.RequestItemNt.NodeTypeId;

                ActionData = ActionDataObj.ToString();

                ButtonAction = NbtButtonAction.request;
            }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[LocationPropertyName] ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[LocationPropertyName] ); } }
        public CswNbtNodePropDateTime LocationVerified { get { return ( _CswNbtNode.Properties[LocationVerifiedPropertyName] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[MaterialPropertyName] ); } }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[StatusPropertyName] ); } }
        public CswNbtNodePropLogical Missing { get { return ( _CswNbtNode.Properties[MissingPropertyName] ); } }
        public CswNbtNodePropLogical Disposed { get { return ( _CswNbtNode.Properties[DisposedPropertyName] ); } }
        public CswNbtNodePropRelationship SourceContainer { get { return ( _CswNbtNode.Properties[SourceContainerPropertyName] ); } }
        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[QuantityPropertyName] ); } }
        public CswNbtNodePropDateTime ExpirationDate { get { return ( _CswNbtNode.Properties[ExpirationDatePropertyName] ); } }
        public CswNbtNodePropButton Dispense { get { return ( _CswNbtNode.Properties[DispensePropertyName] ); } }
        public CswNbtNodePropButton Dispose { get { return ( _CswNbtNode.Properties[DisposePropertyName] ); } }
        public CswNbtNodePropButton Move { get { return ( _CswNbtNode.Properties[MovePropertyName] ); } }
        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
