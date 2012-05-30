using System;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassContainer : CswNbtObjClass
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

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

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
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, DisposedOCP, false.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[LocationPropertyName].AsBarcode ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[LocationPropertyName].AsLocation ); } }
        public CswNbtNodePropDateTime LocationVerified { get { return ( _CswNbtNode.Properties[LocationVerifiedPropertyName].AsDateTime ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[MaterialPropertyName].AsRelationship ); } }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[StatusPropertyName].AsList ); } }
        public CswNbtNodePropLogical Missing { get { return ( _CswNbtNode.Properties[MissingPropertyName].AsLogical ); } }
        public CswNbtNodePropLogical Disposed { get { return ( _CswNbtNode.Properties[DisposedPropertyName].AsLogical ); } }
        public CswNbtNodePropRelationship SourceContainer { get { return ( _CswNbtNode.Properties[SourceContainerPropertyName].AsRelationship ); } }
        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[QuantityPropertyName].AsQuantity ); } }
        public CswNbtNodePropDateTime ExpirationDate { get { return ( _CswNbtNode.Properties[ExpirationDatePropertyName].AsDateTime ); } }

        #endregion


    }//CswNbtObjClassContainer

}//namespace ChemSW.Nbt.ObjClasses
