using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the SDS Module
    /// </summary>
    public class CswNbtModuleRuleSDS : CswNbtModuleRule
    {
        public CswNbtModuleRuleSDS( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.SDS; } }

        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.CISPro );
            }

            //Show the following Material properties...
            //   Assigned SDS
            //   Documents
            //   View SDS
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Documents", "Documents" );
                foreach( CswNbtMetaDataNodeTypeTab Tab in ChemicalNT.getNodeTypeTabs() )
                {
                    if( Tab.TabOrder >= 3 )
                        Tab.TabOrder += 1;
                }
                CswNbtMetaDataNodeTypeTab HazardsTab = ChemicalNT.getNodeTypeTab( "Hazards" ) ?? _CswNbtResources.MetaData.makeNewTabNew( ChemicalNT, "Hazards", 3 );
                _CswNbtResources.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Assigned SDS", HazardsTab, 1, 1 );
                _CswNbtResources.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "View SDS", ChemicalNT.getIdentityTab(), 3, 2 );
            }

            //Show the following Container properties...
            //   View SDS
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(CswEnumNbtObjectClass.ContainerClass);
                foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
                {
                    _CswNbtResources.Modules.AddPropToTab( ContainerNT.NodeTypeId, "View SDS", ContainerNT.getIdentityTab(), 2, 1 );
                }
            }

            _CswNbtResources.Modules.ToggleView( false, "SDS Expiring Next Month", CswEnumNbtViewVisibility.Global );
        }

        public override void OnDisable()
        {
            //Hide the following Material properties...
            //   Assigned SDS
            //   Documents
            //   View SDS
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( int ChemicalNTId in ChemicalOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( ChemicalNTId, "Documents" );
                _CswNbtResources.Modules.HideProp( ChemicalNTId, "Assigned SDS" );
                _CswNbtResources.Modules.HideProp( ChemicalNTId, "View SDS" );
            }

            //Hide the following Container properties...
            //   ViewSDS
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(CswEnumNbtObjectClass.ContainerClass);
                foreach( int ContainerNTId in ContainerOC.getNodeTypeIds().Keys )
                {
                    _CswNbtResources.Modules.HideProp( ContainerNTId, "View SDS" );
                }
            }

            _CswNbtResources.Modules.ToggleView( true, "SDS Expiring Next Month", CswEnumNbtViewVisibility.Global );
        } // OnDisable()
    } // class CswNbtModuleSDS
}// namespace ChemSW.Nbt
