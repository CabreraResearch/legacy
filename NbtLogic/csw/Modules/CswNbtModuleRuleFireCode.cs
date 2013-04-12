using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the FireCode Module
    /// </summary>
    public class CswNbtModuleRuleFireCode : CswNbtModuleRule
    {
        public CswNbtModuleRuleFireCode( CswNbtResources CswNbtResources ) : base( CswNbtResources ){}
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.FireCode; } }

        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.CISPro );
            }
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.Containers );
            }

            //Show the following Location properties...
            //   Control Zone
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int LocationNTId in LocationOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.AddPropToFirstTab( LocationNTId, CswNbtObjClassLocation.PropertyName.ControlZone );
            }

            //Show the following Material properties...
            //   Material Type
            //   Special Flags
            //   Hazard Categories
            //   Hazard Classes
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                foreach( CswNbtMetaDataNodeTypeTab Tab in ChemicalNT.getNodeTypeTabs() )
                {
                    if( Tab.TabOrder >= 4 )
                        Tab.TabOrder += 1;
                }
                CswNbtMetaDataNodeTypeTab HazardsTab = ChemicalNT.getNodeTypeTab( "Hazards" );
                if( null == HazardsTab )
                {
                    HazardsTab = _CswNbtResources.MetaData.makeNewTab( ChemicalNT, "Hazards", 4 );
                }
                _CswNbtResources.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Material Type", HazardsTab, 4, 2, "Fire Reporting" );
                _CswNbtResources.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Special Flags", HazardsTab, 5, 2, "Fire Reporting" );
                _CswNbtResources.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Hazard Categories", HazardsTab, 6, 2, "Fire Reporting" );
                _CswNbtResources.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Hazard Classes", HazardsTab, 7, 2, "Fire Reporting" );
            }

            //Show the following Container properties...
            //   Storage Pressure
            //   Storage Temperature
            //   Use Type
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int ContainerNTId in ContainerOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.AddPropToTab( ContainerNTId, "Storage Pressure", "Fire Code" );
                _CswNbtResources.Modules.AddPropToTab( ContainerNTId, "Storage Temperature", "Fire Code" );
                _CswNbtResources.Modules.AddPropToTab( ContainerNTId, "Use Type", "Fire Code" );
            }
        }

        public override void OnDisable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.FireDbSync ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.FireDbSync );
            }

            //Hide the following Location properties...
            //   Control Zone
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int LocationNTId in LocationOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( LocationNTId, CswNbtObjClassLocation.PropertyName.ControlZone );
            }

            //Hide the following Material properties...
            //   Material Type
            //   Special Flags
            //   Hazard Categories
            //   Hazard Classes
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType("Chemical");
            if( null != ChemicalNT )
            {
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, "Material Type" );
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, "Special Flags" );
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, "Hazard Categories" );
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, "Hazard Classes" );
            }

            //Hide the following Container properties...
            //   Storage Pressure
            //   Storage Temperature
            //   Use Type
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int ContainerNTId in ContainerOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( ContainerNTId, "Storage Pressure" );
                _CswNbtResources.Modules.HideProp( ContainerNTId, "Storage Temperature" );
                _CswNbtResources.Modules.HideProp( ContainerNTId, "Use Type" );
            }
        } // OnDisable()
    } // class CswNbtModuleFireCode
}// namespace ChemSW.Nbt
