using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Requesting Module
    /// </summary>
    public class CswNbtModuleRuleRequesting: CswNbtModuleRule
    {
        public CswNbtModuleRuleRequesting( CswNbtResources CswNbtResources ) :
            base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.Requesting; } }
        protected override void OnEnable()
        {
            //Show the following Container properties...
            //   Requests
            //   Submitted Requests
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int NodeTypeId in ContainerOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassContainer.PropertyName.Request );
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassContainer.PropertyName.SubmittedRequests );
            }

            //Show the following Inventory Group properties...
            //   Limit Request Delivery Location
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            foreach( int NodeTypeId in InventoryGroupOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassInventoryGroup.PropertyName.LimitRequestDeliveryLocation );
            }

            //Show the following Location properties...
            //   Request Delivery Location
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in LocationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.RequestDeliveryLocation );
            }

            //Show the following Material properties...
            //   Request Button
            CswNbtMetaDataPropertySet MaterialSet = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialSet.getObjectClasses() )
            {
                foreach( int NodeTypeId in MaterialOC.getNodeTypeIds().Keys )
                {
                    _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassChemical.PropertyName.Request );
                }
            }

            //Show all views in the Requests category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Requests", CswEnumNbtViewVisibility.Global );

            //Show the request fulfiller Role/User
            _CswNbtResources.Modules.ToggleRoleNodes( false, "request_fulfiller" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "request_fulfiller" );
            //TODO - Case 31274 - Add CISPro_Guardian Role/User to this list

            // Case 28930 - Enable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.GenRequest, Disabled : false );

            //Case 31546 - If we're enabling Requesting AFTER MLM, we need to show the MLM props
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MLM ) )
            {
                CswNbtObjClassRequestItem.ToggleMLMProps( _CswNbtResources, false );
            }
        }

        protected override void OnDisable()
        {
            //Hide the following Container properties...
            //   Requests
            //   Submitted Requests
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int NodeTypeId in ContainerOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassContainer.PropertyName.Request );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassContainer.PropertyName.SubmittedRequests );
            }

            //Hide the following Inventory Group properties...
            //   Limit Request Delivery Location
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            foreach( int NodeTypeId in InventoryGroupOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassInventoryGroup.PropertyName.LimitRequestDeliveryLocation );
            }

            //Hide the following Location properties...
            //   Request Delivery Location
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in LocationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.RequestDeliveryLocation );
            }

            //Hide the following Material properties...
            //   Request Button
            CswNbtMetaDataPropertySet MaterialSet = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialSet.getObjectClasses() )
            {
                foreach( int NodeTypeId in MaterialOC.getNodeTypeIds().Keys )
                {
                    _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassChemical.PropertyName.Request );
                }
            }

            //Hide all views in the Requests category
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Requests", CswEnumNbtViewVisibility.Global );

            //Hide the request fulfiller Role/User
            _CswNbtResources.Modules.ToggleRoleNodes( true, "request_fulfiller" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "request_fulfiller" );
            //TODO - Case 31274 - Add CISPro_Guardian Role/User to this list

            // Case 28930 - Disable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.GenRequest, Disabled : true );
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
