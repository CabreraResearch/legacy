using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the SDS Module
    /// </summary>
    public class CswNbtModuleRuleSDS : CswNbtModuleRule
    {
        public CswNbtModuleRuleSDS( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.SDS; } }

        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
            }

            //Show the following Material properties...
            //   Assigned SDS
            //   Documents
            //   View SDS
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.AddPropToTab( MaterialNT.NodeTypeId, "Documents", "Documents" );
                if( MaterialNT.NodeTypeName == "Chemical" )
                {
                    foreach( CswNbtMetaDataNodeTypeTab Tab in MaterialNT.getNodeTypeTabs() )
                    {
                        if( Tab.TabOrder >= 3 )
                            Tab.TabOrder += 1;
                    }
                    CswNbtMetaDataNodeTypeTab HazardsTab = MaterialNT.getNodeTypeTab( "Hazards" ) ?? _CswNbtResources.MetaData.makeNewTab( MaterialNT, "Hazards", 3 );
                    _CswNbtResources.Modules.AddPropToTab( MaterialNT.NodeTypeId, "Assigned SDS", HazardsTab, 1, 1 );
                    _CswNbtResources.Modules.AddPropToTab( MaterialNT.NodeTypeId, "View SDS", MaterialNT.getIdentityTab(), 3, 2 );
                }
            }

            //Show the following Container properties...
            //   View SDS
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(NbtObjectClass.ContainerClass);
                foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
                {
                    _CswNbtResources.Modules.AddPropToTab( ContainerNT.NodeTypeId, "View SDS", ContainerNT.getIdentityTab(), 2, 1 );
                }
            }

            _CswNbtResources.Modules.ToggleView( false, "SDS Expiring Next Month", NbtViewVisibility.Global );
        }

        public override void OnDisable()
        {
            //Hide the following Material properties...
            //   Assigned SDS
            //   Documents
            //   View SDS
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( int MaterialNTId in MaterialOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( MaterialNTId, "Documents" );
                _CswNbtResources.Modules.HideProp( MaterialNTId, "Assigned SDS" );
                _CswNbtResources.Modules.HideProp( MaterialNTId, "View SDS" );
            }

            //Hide the following Container properties...
            //   ViewSDS
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(NbtObjectClass.ContainerClass);
                foreach( int ContainerNTId in ContainerOC.getNodeTypeIds() )
                {
                    _CswNbtResources.Modules.HideProp( ContainerNTId, "View SDS" );
                }
            }

            _CswNbtResources.Modules.ToggleView( true, "SDS Expiring Next Month", NbtViewVisibility.Global );
        } // OnDisable()
    } // class CswNbtModuleSDS
}// namespace ChemSW.Nbt
