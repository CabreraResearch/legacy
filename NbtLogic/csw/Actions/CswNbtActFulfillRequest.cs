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

        public class FulfillmentFilters
        {
            private CswNbtView _LocationView;
            private CswNbtResources _CswNbtResources = null;
            public FulfillmentFilters( CswNbtResources CswNbtResources )
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
            }
            public readonly CswCommaDelimitedString RequestTypes = CswNbtObjClassRequestItem.Types.Options;
            public CswNbtSessionDataId LocationViewId;
            public string MaterialName;
            public string RequestName;
            public Int32 InventoryGroupObjectClassId;
        }

        public JObject getFulfillmentFilters( JObject SelectedFilterValues)
        {
            JObject Ret = new JObject();

            FulfillmentFilters Filters = new FulfillmentFilters( _CswNbtResources );

            Ret["filters"] = new JObject();
            Ret["filters"]["requesttype"] = Filters.RequestTypes.ToString();
            Ret["filters"]["locationviewid"] = Filters.LocationViewId.ToString();
            Ret["filters"]["materialname"] = "";
            Ret["filters"]["requestname"] = "";
            Ret["filters"]["inventorygroupid"] = Filters.InventoryGroupObjectClassId;
            return Ret;
        }

        #endregion Constructor
    }
}
