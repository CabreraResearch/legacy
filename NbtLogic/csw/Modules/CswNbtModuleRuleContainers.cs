using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.DB;
using ChemSW.Core;

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
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.Containers; } }
        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
            }

            //Show the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Storate Compatibility
            //   Allow Inventory
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
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
            int materialOC_ID = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.MaterialClass );
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
            int userOC_Id = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.UserClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( userOC_Id ) )
            {
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassUser.PropertyName.WorkUnit );
            }

            //Show all views in the Containers category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Containers", NbtViewVisibility.Global );

            //Show all reports in the Containers category
            _CswNbtResources.Modules.ToggleReportNodes( "Containers", false );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( true, CswNbtActionName.KioskMode );

            //Show Print Labels with a dependent NodeType
            _CswNbtResources.Modules.TogglePrintLabels( false, CswNbtModuleName.Containers );
        }

        public override void OnDisable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.MLM );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.FireCode ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.FireCode );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MultiInventoryGroup ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.MultiInventoryGroup );
            }

            //Hide the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Storate Compatibility
            //   Allow Inventory
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
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
            int materialOC_ID = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.MaterialClass );
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
            int userOC_Id = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.UserClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( userOC_Id ) )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassUser.PropertyName.WorkUnit );
            }

            //Hide all views in the Containers category
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Containers", NbtViewVisibility.Global );

            //Hide all reports in the Containers category
            _CswNbtResources.Modules.ToggleReportNodes( "Containers", true );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( false, CswNbtActionName.KioskMode );

            //Hide Print Labels with a dependent NodeType
            _CswNbtResources.Modules.TogglePrintLabels( true, CswNbtModuleName.Containers );

        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
