using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActFulfillRequest
    {
        #region Private, core methods

        private CswNbtResources _CswNbtResources = null;
        private CswNbtMetaDataObjectClass _RequestOc = null;
        private CswNbtMetaDataObjectClass _RequestItemOc = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActFulfillRequest( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            if ( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActFulfillRequest without the required module." );
            }
        }


        #endregion Constructor

        #region Classes
        
        /// <summary>
        /// Values necessary to assemble a set of Fulfillment Filters
        /// </summary>
        public class FilterDef
        {
            private CswNbtView _LocationView;
            private CswNbtResources _CswNbtResources = null;
            
            public FilterDef( CswNbtResources CswNbtResources )
            {
                _CswNbtResources = CswNbtResources;
                _LocationView = CswNbtNodePropLocation.LocationPropertyView( _CswNbtResources );
                if(null == _LocationView)
                {
                    throw new CswDniException( "Cannot create Request Fulfillment filters without a Locations view." );
                }
                _LocationView.SaveToCache( false );
                LocationViewId = _LocationView.SessionViewId;
                InventoryGroupObjectClassId = _CswNbtResources.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
                CswNbtObjClassRequestItem.Types.Options.Add( "All" );
                RequestTypes = CswNbtObjClassRequestItem.Types.Options;
            }
            /// <summary>
            /// Possible Request Item Types: Dispense, Dispose, Request, Move, All
            /// </summary>
            public readonly CswCommaDelimitedString RequestTypes;
            /// <summary>
            /// Session View Id for Location Tree view
            /// </summary>
            public CswNbtSessionDataId LocationViewId;
            /// <summary>
            /// Inventory Group ObjectClassId
            /// </summary>
            public Int32 InventoryGroupObjectClassId;
        }

        /// <summary>
        /// Filter to apply on Request Items
        /// </summary>
        public class Filter
        {
            private CswNbtResources _CswNbtResources = null;
            
            public Filter( CswNbtResources CswNbtResources, string LocationId = "", string InventoryGroupId = "" )
            {
                _CswNbtResources = CswNbtResources;
                SelectedLocationId.FromString( LocationId );
                SelectedInventoryGroupId.FromString( InventoryGroupId );
                RequestOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
                RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            }

            public CswNbtMetaDataObjectClass RequestOc;
            public CswNbtMetaDataObjectClass RequestItemOc;

            public string SelectedRequestType = "";
            public string SelectedMaterialName = "";
            public string SelectedRequestName = "";
            public string SelectedLocationName = "";
            public CswPrimaryKey SelectedLocationId = new CswPrimaryKey();
            public CswPrimaryKey SelectedInventoryGroupId = new CswPrimaryKey();            
        }

        #endregion Classes

        #region Private Methods
        
        /// <summary>
        /// Get the Request relationship (if any) and sets the Inventory Group filter if present
        /// </summary>
        private CswNbtViewRelationship _getRequestRelationshipFromInventoryGroupFilter( CswNbtView View, Filter Filter )
        {
            CswNbtViewRelationship Ret = null;
            if( null != Filter &&
                null != Filter.SelectedInventoryGroupId && 
                Int32.MinValue != Filter.SelectedInventoryGroupId.PrimaryKey )
            {
                CswNbtNode GroupNode = _CswNbtResources.Nodes.GetNode( Filter.SelectedInventoryGroupId );
                if( null != GroupNode &&
                    GroupNode.ObjClass.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass )
                {
                    CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
                    CswNbtViewRelationship InventoryGroupVr = View.AddViewRelationship( InventoryGroupOc, false );
                    InventoryGroupVr.NodeIdsToFilterIn.Add( Filter.SelectedInventoryGroupId );
                    Filter.RequestOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
                    CswNbtMetaDataObjectClassProp InventoryGroupOcp = Filter.RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.InventoryGroup.ToString() );
                    Ret = View.AddViewRelationship( InventoryGroupVr, NbtViewPropOwnerType.Second, InventoryGroupOcp, false );
                }
            }
            return Ret;
        }

        /// <summary>
        /// Gets the Request Item view relationship and attaches properties for the grid which are not filtered
        /// </summary>
        private CswNbtViewRelationship _getRequestItemRelationship( CswNbtView View, Filter Filter, CswNbtViewRelationship RequestVr = null )
        {
            CswNbtViewRelationship Ret = null;
            if( null != View && null != Filter )
            {
                CswNbtMetaDataObjectClassProp RequestOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );

                if ( null != RequestVr )
                {
                    Ret = View.AddViewRelationship( RequestVr, NbtViewPropOwnerType.Second, RequestOcp, false );
                }
                else
                {
                    Ret = View.AddViewRelationship( Filter.RequestItemOc, false );
                }

                CswNbtMetaDataObjectClassProp NumberOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Number );
                CswNbtMetaDataObjectClassProp TypeOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
                CswNbtMetaDataObjectClassProp QuantityOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Quantity );
                CswNbtMetaDataObjectClassProp CountOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Count );
                CswNbtMetaDataObjectClassProp SizeOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Size );
                CswNbtMetaDataObjectClassProp OrderNoOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );

                View.AddViewProperty( Ret, NumberOcp );
                View.AddViewProperty( Ret, TypeOcp );
                View.AddViewProperty( Ret, OrderNoOcp );

                CswNbtViewProperty NumberVp = View.AddViewProperty( Ret, NumberOcp );
                NumberVp.ShowInGrid = false;
                CswNbtViewProperty QuantityVp = View.AddViewProperty( Ret, QuantityOcp );
                QuantityVp.ShowInGrid = false;
                CswNbtViewProperty CountVp = View.AddViewProperty( Ret, CountOcp );
                CountVp.ShowInGrid = false;
                CswNbtViewProperty SizeVp = View.AddViewProperty( Ret, SizeOcp );
                SizeVp.ShowInGrid = false;
            }
            return Ret;
        }
        
        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Gets the necessary properties to create filter controls
        /// </summary>
        public JObject getFulfillmentFilters( JObject SelectedFilterValues )
        {
            JObject Ret = new JObject();

            FilterDef Filters = new FilterDef( _CswNbtResources );

            Ret["filters"] = new JObject();
            Ret["filters"]["requesttype"] = Filters.RequestTypes.ToString();
            Ret["filters"]["locationviewid"] = Filters.LocationViewId.ToString();
            Ret["filters"]["materialname"] = "";
            Ret["filters"]["requestname"] = "";
            Ret["filters"]["inventorygroupid"] = Filters.InventoryGroupObjectClassId;
            return Ret;
        }

        /// <summary>
        /// Gets a Grid view of Request Items matching filter criteria
        /// </summary>
        public CswNbtView RequestItemView( Filter Filter )
        {
            CswNbtView Ret = null;
            if( null != Filter )
            {
                Ret = new CswNbtView( _CswNbtResources );
                Ret.ViewMode = NbtViewRenderingMode.Grid;
                Ret.Category = "Requests";
                CswNbtViewRelationship RequestVr = _getRequestRelationshipFromInventoryGroupFilter( Ret, Filter );

                if( false == string.IsNullOrEmpty( Filter.SelectedRequestName ) )
                {
                    RequestVr = RequestVr ?? Ret.AddViewRelationship( Filter.RequestOc, false );
                    CswNbtMetaDataObjectClassProp NameOcp = Filter.RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Name.ToString() );
                    Ret.AddViewPropertyAndFilter( RequestVr, NameOcp, Filter.SelectedRequestName );
                }

                CswNbtViewRelationship RequestItemVr = _getRequestItemRelationship( Ret, Filter, RequestVr );

                CswNbtMetaDataObjectClassProp MaterialOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Material );
                CswNbtMetaDataObjectClassProp LocationOcp = Filter.RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location );
                Ret.AddViewPropertyAndFilter( RequestItemVr, MaterialOcp, Filter.SelectedMaterialName, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Begins );
                Ret.AddViewPropertyAndFilter( RequestItemVr, LocationOcp, Filter.SelectedLocationName, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Begins, SubFieldName: CswNbtSubField.SubFieldName.Path );
            }
            return Ret;
        }

        /// <summary>
        /// Get a View of request items based on a Filter
        /// </summary>
        public CswNbtView getRequestItems( string Filters )
        {
            JObject FilterObj = CswConvert.ToJObject( Filters );
            Filter Filter = new Filter( _CswNbtResources, CswConvert.ToString( FilterObj["selectedlocationid"] ), CswConvert.ToString( FilterObj["selectedinventorygroupid"] ) );
            Filter.SelectedRequestType = CswConvert.ToString( FilterObj["selectedrequesttype"] );
            Filter.SelectedMaterialName = CswConvert.ToString( FilterObj["selectedmaterialname"] );
            Filter.SelectedRequestName = CswConvert.ToString( FilterObj["selectedrequestname"] );
            Filter.SelectedLocationName = CswConvert.ToString( FilterObj["selectedlocationname"] );
            return RequestItemView( Filter );
        }

        #endregion Public Methods

    }
}
