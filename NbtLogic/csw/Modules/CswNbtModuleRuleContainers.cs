using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Container Module
    /// </summary>
    public class CswNbtModuleRuleContainers : CswNbtModuleRule
    {
        public CswNbtModuleRuleContainers( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.Containers; } }
        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.CISPro );
            }

            //Show the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Storate Compatibility
            //   Allow Inventory
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.AddPropToTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers, "Containers" );
                _CswNbtResources.Modules.AddPropToTab( NodeTypeId, "Inventory Levels", "Inventory Levels", 2 );
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
            }

            //Show the following Material properties...
            //   Inventory Levels
            //   Sizes
            //   Containers
            //   Approved for Receiving
            //   Receive (button)
            //   Request (button)
            //   Storage Compatibility
            int materialOC_ID = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in _CswNbtResources.MetaData.getNodeTypes( materialOC_ID ) )
            {
                string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                _CswNbtResources.Modules.AddPropToTab( materialNT.NodeTypeId, sizesNTPName, "Containers", 99 );

                _CswNbtResources.Modules.AddPropToTab( materialNT.NodeTypeId, "Inventory Levels", "Containers", 99 );

                string containersNTPName = materialNT.NodeTypeName + " Containers";
                _CswNbtResources.Modules.AddPropToTab( materialNT.NodeTypeId, containersNTPName, "Containers", 99 );

                CswNbtMetaDataNodeTypeTab materialNTT = materialNT.getFirstNodeTypeTab();
                _CswNbtResources.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.ApprovedForReceiving, materialNTT );

                CswNbtMetaDataNodeTypeTab materialIdentityNTT = materialNT.getIdentityTab();
                _CswNbtResources.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Receive, materialIdentityNTT, 2, 2 );
                _CswNbtResources.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Request, materialIdentityNTT, 1, 2 );

                _CswNbtResources.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.StorageCompatibility, "Hazards" );
            }

            //Show the following User props...
            //   Work Unit
            int userOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.UserClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( userOC_Id ) )
            {
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassUser.PropertyName.WorkUnit );
            }

            //Show all views in the Containers category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Containers", CswEnumNbtViewVisibility.Global );

            //Show all reports in the Containers category
            _CswNbtResources.Modules.ToggleReportNodes( "Containers", false );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( true, CswEnumNbtActionName.Kiosk_Mode );
            _CswNbtResources.Actions[CswEnumNbtActionName.Kiosk_Mode].SetCategory( "Containers" );


            //Show Print Labels with a dependent NodeType
            _CswNbtResources.Modules.TogglePrintLabels( false, CswEnumNbtModuleName.Containers );

            //Show the request fulfiller Role/User
            _CswNbtResources.Modules.ToggleRoleNodes( false, "request_fulfiller" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "request_fulfiller" );
        }

        public override void OnDisable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MLM ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.MLM );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.FireCode ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.FireCode );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MultiInventoryGroup ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.MultiInventoryGroup );
            }

            //Hide the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Storate Compatibility
            //   Allow Inventory
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers );
                _CswNbtResources.Modules.HideProp( NodeTypeId, "Inventory Levels" );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
            }

            //Hide the following Material properties...
            //   Inventory Levels
            //   Sizes
            //   Containers
            //   Approved for Receiving
            //   Receive (button)
            //   Request (button)
            //   Storage Compatibility
            int materialOC_ID = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in _CswNbtResources.MetaData.getNodeTypes( materialOC_ID ) )
            {
                string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, sizesNTPName );

                _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, "Inventory Levels" );

                string containersNTPName = materialNT.NodeTypeName + " Containers";
                _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, containersNTPName );

                _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.ApprovedForReceiving );
                _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Receive );
                _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Request );
                _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.StorageCompatibility );
            }

            //Hide the following User props...
            //   Work Unit
            int userOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.UserClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( userOC_Id ) )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassUser.PropertyName.WorkUnit );
            }

            //Hide all views in the Containers category
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Containers", CswEnumNbtViewVisibility.Global );

            //Hide all reports in the Containers category
            _CswNbtResources.Modules.ToggleReportNodes( "Containers", true );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
            {
                _CswNbtResources.Modules.ToggleAction( false, CswEnumNbtActionName.Kiosk_Mode );
            }
            {
                _CswNbtResources.Actions[CswEnumNbtActionName.Kiosk_Mode].SetCategory( "Equipment" );
            }

            //Hide Print Labels with a dependent NodeType
            _CswNbtResources.Modules.TogglePrintLabels( true, CswEnumNbtModuleName.Containers );

            //Hide the request fulfiller Role/User
            _CswNbtResources.Modules.ToggleRoleNodes( true, "request_fulfiller" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "request_fulfiller" );

        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
