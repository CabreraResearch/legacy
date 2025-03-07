using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Container Module
    /// </summary>
    public class CswNbtModuleRuleContainers: CswNbtModuleRule
    {
        public CswNbtModuleRuleContainers( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.Containers; } }
        protected override void OnEnable()
        {
            //Show the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Storate Compatibility
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers );
                _CswNbtResources.Modules.ShowProp( NodeTypeId, "Inventory Levels" );
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtResources.Modules.ShowProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
            }

            //Show the following Material properties...
            //   Inventory Levels
            //   Sizes
            //   Containers
            //   Approved for Receiving
            //   Receive (button)
            //   Storage Compatibility
            CswNbtMetaDataPropertySet MaterialSet = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass materialOC in MaterialSet.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType materialNT in _CswNbtResources.MetaData.getNodeTypes( materialOC.ObjectClassId ) )
                {
                    string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, sizesNTPName );

                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, "Inventory Levels" );

                    string containersNTPName = materialNT.NodeTypeName + " Containers";
                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, containersNTPName );

                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.ApprovedForReceiving );

                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.Receive );

                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.StorageCompatibility );
                }
            }

            //Show all views in the Containers category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Containers", CswEnumNbtViewVisibility.Global );

            //Show all reports in the Containers category
            _CswNbtResources.Modules.ToggleReportNodes( "Containers", false );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( true, CswEnumNbtActionName.Kiosk_Mode );
            
            //Show Print Labels with a dependent NodeType
            _CswNbtResources.Modules.TogglePrintLabels( false, CswEnumNbtModuleName.Containers );

            // Case 28930 - Enable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.ExpiredContainers, Disabled : false );
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.ContainerReconciliationActions, Disabled : false );
        }

        protected override void OnDisable()
        {
            //Hide the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Storate Compatibility
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers );
                _CswNbtResources.Modules.HideProp( NodeTypeId, "Inventory Levels" );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
            }

            //Hide the following Material properties...
            //   Inventory Levels
            //   Sizes
            //   Containers
            //   Approved for Receiving
            //   Receive (button)
            //   Storage Compatibility
            CswNbtMetaDataPropertySet MaterialSet = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass materialOC in MaterialSet.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType materialNT in _CswNbtResources.MetaData.getNodeTypes( materialOC.ObjectClassId ) )
                {
                    string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, sizesNTPName );

                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, "Inventory Levels" );

                    string containersNTPName = materialNT.NodeTypeName + " Containers";
                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, containersNTPName );

                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.ApprovedForReceiving );
                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.Receive );
                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.StorageCompatibility );
                }
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

            //Hide Print Labels with a dependent NodeType
            _CswNbtResources.Modules.TogglePrintLabels( true, CswEnumNbtModuleName.Containers );

            // Case 28930 - Disable Scheduled Rules
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.ExpiredContainers, Disabled : true );
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.ContainerReconciliationActions, Disabled : true );
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
