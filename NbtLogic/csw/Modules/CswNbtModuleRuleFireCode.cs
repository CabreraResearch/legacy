using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the FireCode Module
    /// </summary>
    public class CswNbtModuleRuleFireCode: CswNbtModuleRule
    {
        public CswNbtModuleRuleFireCode( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.FireCode; } }

        protected override void OnEnable()
        {
            //Show the following Location properties...
            //   Control Zone
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int LocationNTId in LocationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( LocationNTId, CswNbtObjClassLocation.PropertyName.ControlZone );
            }

            //Show the following Chemical properties...
            //   Material Type
            //   Special Flags
            //   Hazard Categories
            //   Hazard Classes
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in _CswNbtResources.MetaData.getNodeTypes( ChemicalOC.ObjectClassId ) )
            {
                _CswNbtResources.Modules.ShowProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.MaterialType );
                _CswNbtResources.Modules.ShowProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.SpecialFlags );
                _CswNbtResources.Modules.ShowProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.HazardCategories );
                _CswNbtResources.Modules.ShowProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.HazardClasses );
            }

            //Show the following Container properties...
            //   Storage Pressure
            //   Storage Temperature
            //   Use Type
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int ContainerNTId in ContainerOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.StoragePressure );
                _CswNbtResources.Modules.ShowProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.StorageTemperature );
                _CswNbtResources.Modules.ShowProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.UseType );
            }

            //Show the HMIS Materials and HMIS Totals
            _toggleHMISReports( false );
        }

        protected override void OnDisable()
        {
            //Hide the following Location properties...
            //   Control Zone
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int LocationNTId in LocationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( LocationNTId, CswNbtObjClassLocation.PropertyName.ControlZone );
            }

            //Hide the following Chemical properties...
            //   Material Type
            //   Special Flags
            //   Hazard Categories
            //   Hazard Classes
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in _CswNbtResources.MetaData.getNodeTypes( ChemicalOC.ObjectClassId ) )
            {
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.MaterialType );
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.SpecialFlags );
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.HazardCategories );
                _CswNbtResources.Modules.HideProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.HazardClasses );
            }

            //Hide the following Container properties...
            //   Storage Pressure
            //   Storage Temperature
            //   Use Type
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int ContainerNTId in ContainerOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.StoragePressure );
                _CswNbtResources.Modules.HideProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.StorageTemperature );
                _CswNbtResources.Modules.HideProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.UseType );
            }

            //Show the HMIS Materials and HMIS Totals
            _toggleHMISReports( true );

        } // OnDisable()

        private void _toggleHMISReports( bool Hide )
        {
            CswNbtMetaDataObjectClass ReportOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp ReportNameOCP = ReportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.ReportName );

            CswNbtView reportsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = reportsView.AddViewRelationship( ReportOC, false );
            reportsView.AddViewPropertyAndFilter( parent, ReportNameOCP, Value : "HMIS Totals" );
            reportsView.AddViewPropertyAndFilter( parent, ReportNameOCP, Value : "HMIS Materials", Conjunction : CswEnumNbtFilterConjunction.Or );

            ICswNbtTree reportsTree = _CswNbtResources.Trees.getTreeFromView( reportsView, false, true, true );
            for( int i = 0; i < reportsTree.getChildNodeCount(); i++ )
            {
                reportsTree.goToNthChild( i );
                CswNbtNode ReportNode = reportsTree.getNodeForCurrentPosition();
                ReportNode.Hidden = Hide;
                ReportNode.postChanges( false, SkipEvents : true );
                reportsTree.goToParentNode();
            }
        }

    } // class CswNbtModuleFireCode
}// namespace ChemSW.Nbt
