using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainer : CswNbtObjClass, ICswNbtPropertySetRequest
    {
        public static string BarcodePropertyName { get { return "Barcode"; } }
        public static string MaterialPropertyName { get { return "Material"; } }
        public static string LocationPropertyName { get { return "Location"; } }
        public static string LocationVerifiedPropertyName { get { return "Location Verified"; } }
        public static string StatusPropertyName { get { return "Status"; } }
        public static string MissingPropertyName { get { return "Missing"; } }
        public static string DisposedPropertyName { get { return "Disposed"; } }
        public static string SourceContainerPropertyName { get { return "Source Container"; } }
        public static string QuantityPropertyName { get { return "Quantity"; } }
        public static string ExpirationDatePropertyName { get { return "Expiration Date"; } }
        public static string SizePropertyName { get { return "Size"; } }
        public static string RequestPropertyName { get { return "Request"; } }
        public static string DispensePropertyName { get { return "Dispense"; } }

        public string RequestButtonPropertyName { get { return RequestPropertyName; } }

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
            //TODO - case 24508 - create a new ContainerDispenseTransaction node of type Receiving, with this node as the SourceContainer
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            //TODO - case 24508, part 4 - 'disposed' modification logic (how do we know it changed? is this where we do this?)

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
            //case 25759 - set quantity unittype view based on related material physical state
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( this.Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                CswNbtObjClassMaterial MaterialNodeAsMaterial = (CswNbtObjClassMaterial) MaterialNode;
                if( false == String.IsNullOrEmpty( MaterialNodeAsMaterial.PhysicalState.Value ) )
                {
                    CswNbtMetaDataNodeType ContainerNodeType = this.Node.getNodeType();
                    CswNbtMetaDataNodeTypeProp Quantity = ContainerNodeType.getNodeTypeProp( "Quantity" );

                    CswNbtView StateSpecificUnitTypeView = new CswNbtView( _CswNbtResources );
                    StateSpecificUnitTypeView.makeNew( "CswNbtNodeTypePropQuantity_" + Quantity.NodeTypeId.ToString(), NbtViewVisibility.Property );

                    CswNbtMetaDataNodeType WeightNT = _CswNbtResources.MetaData.getNodeType( "Weight Unit" );
                    StateSpecificUnitTypeView.AddViewRelationship( WeightNT, true );
                    if( MaterialNodeAsMaterial.PhysicalState.Value != "Solid" )
                    {
                        CswNbtMetaDataNodeType VolumeNT = _CswNbtResources.MetaData.getNodeType( "Volume Unit" );
                        StateSpecificUnitTypeView.AddViewRelationship( VolumeNT, true );
                    }

                    StateSpecificUnitTypeView.save();
                    this.Quantity.View = StateSpecificUnitTypeView;
                }
            }
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
                if( RequestPropertyName == OCP.PropName )
                {
                    ButtonAction = NbtButtonAction.request;
                }
                else if( DispensePropertyName == OCP.PropName )
                {
                    //TODO - case 24508, part 6 - when Dispense button is clicked, trigger DispenseContainer action
                }
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
        public CswNbtNodePropButton Request { get { return ( _CswNbtNode.Properties[RequestPropertyName] ); } }
        public CswNbtNodePropButton Dispense { get { return ( _CswNbtNode.Properties[DispensePropertyName] ); } }

        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
