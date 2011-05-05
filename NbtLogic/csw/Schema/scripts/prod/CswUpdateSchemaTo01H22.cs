using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-22
    /// </summary>
    public class CswUpdateSchemaTo01H22 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 


        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 22 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H22( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

            CswNbtMetaDataNodeType PhysInspNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection ) );
            CswNbtMetaDataNodeTypeProp PhysInsp_TargetNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_GeneratorNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_DueDateNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_MountPointNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.OwnerPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_RouteNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( "Route" );
            CswNbtMetaDataNodeTypeProp PhysInsp_RouteOrderNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( "Route Order" );
            CswNbtMetaDataNodeTypeProp PhysInsp_BarcodeNTP = PhysInspNT.getNodeTypeProp( "Barcode" ); // propref
            CswNbtMetaDataNodeTypeProp PhysInsp_FinishedNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.FinishedPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_LocationNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_NameNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_StatusNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );

            CswNbtMetaDataNodeType PhysInspSchedNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection_Schedule ) );
            CswNbtMetaDataNodeTypeProp PhysInspSched_OwnerNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInspSched_TargetTypeNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
            CswNbtMetaDataNodeTypeProp PhysInspSched_ParentTypeNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
            CswNbtMetaDataNodeTypeProp PhysInspSched_ParentViewNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );

            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ) );
            CswNbtMetaDataNodeTypeProp MountPoint_BarcodeNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
            CswNbtMetaDataNodeTypeProp MountPoint_DescriptionNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
            CswNbtMetaDataNodeTypeProp MountPoint_MountPointGroupNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            CswNbtMetaDataNodeTypeProp MountPoint_HydrostaticInspectionNTP = MountPointNT.getNodeTypeProp( "Hydrostatic Inspection" );
            CswNbtMetaDataNodeTypeProp MountPoint_LocationNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp MountPoint_LastInspDateNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
            CswNbtMetaDataNodeTypeProp MountPoint_StatusNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );

            CswNbtMetaDataNodeType MountPointGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point_Group ) );

            CswNbtMetaDataNodeType RouteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection_Route ) );

            // case 21049

            // Rename properties            
            if( !PhysInspNT.IsLocked )
            {
                PhysInsp_TargetNTP.PropName = "FE Inspection Point";
                PhysInsp_GeneratorNTP.PropName = "Schedule";
            }
            PhysInspSched_OwnerNTP.PropName = "Inspection Group";
            PhysInspSched_TargetTypeNTP.PropName = "Inspection Type";
            PhysInspSched_ParentTypeNTP.PropName = "FE Inspection Point Type";
            PhysInspSched_ParentViewNTP.PropName = "FE Inspection Point View";

            // Fix layout of Physical Inspection Schedule Settings tab        
            PhysInspSched_OwnerNTP.DisplayRow = 1;
            PhysInspSched_ParentTypeNTP.DisplayRow = 2;
            PhysInspSched_ParentViewNTP.DisplayRow = 3;
            PhysInspSched_TargetTypeNTP.DisplayRow = 4;

            // Add barcode to mount point name template
            MountPointNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( MountPoint_BarcodeNTP.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( MountPoint_DescriptionNTP.PropName );

            // Generator and Due Date should not be mobile search options
            if( !PhysInspNT.IsLocked )
            {
                PhysInsp_GeneratorNTP.MobileSearch = false;
                PhysInsp_DueDateNTP.MobileSearch = false;
            }
            // Rename 'mount point group' to 'inspection group'
            MountPointGroupNT.NodeTypeName = "Inspection Group";
            MountPointGroupNT.getFirstNodeTypeTab().TabName = "Inspection Group";
            MountPoint_MountPointGroupNTP.PropName = "Inspection Group";

            // Rename 'mount point' to 'FE Inspection Point'
            MountPointNT.NodeTypeName = "FE Inspection Point";
            MountPointNT.getFirstNodeTypeTab().TabName = "FE Inspection Point";

            // Make Inspection's Target Type editable
            PhysInspSched_TargetTypeNTP.ReadOnly = false;

            // Remove properties
            if( MountPoint_HydrostaticInspectionNTP != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( MountPoint_HydrostaticInspectionNTP );
            }


            // case 21050

            // Create 'Inspector' relationship property
            CswNbtMetaDataNodeTypeProp Route_InspectorNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( RouteNT, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Inspector", RouteNT.getFirstNodeTypeTab().TabId );
            Route_InspectorNTP.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), UserOC.ObjectClassId, string.Empty, Int32.MinValue );

            // Remove Route and Route Order properties from Physical Inspection
            //  1. get rid of objectclasspropid on nodetype props
            if( !PhysInspNT.IsLocked )
            {
                CswTableUpdate NTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H22_NTP_Update", "nodetype_props" );
                DataTable NTPTable = NTPUpdate.getTable( "where objectclasspropid in (" + PhysInsp_RouteNTP.ObjectClassPropId.ToString() + "," + PhysInsp_RouteOrderNTP.ObjectClassPropId.ToString() + ")" );
                Collection<CswNbtMetaDataNodeTypeProp> NTPsToDelete = new Collection<CswNbtMetaDataNodeTypeProp>();
                foreach( DataRow NTPRow in NTPTable.Rows )
                {
                    NTPRow["objectclasspropid"] = CswConvert.ToDbVal( string.Empty );
                    NTPsToDelete.Add( _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( CswConvert.ToInt32( NTPRow["nodetypepropid"] ) ) );
                }
                NTPUpdate.update( NTPTable );

                //  2. delete object class props
                CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H22_OCP_Update", "object_class_props" );
                DataTable OCPTable = OCPUpdate.getTable( "where objectclasspropid in (" + PhysInsp_RouteNTP.ObjectClassPropId.ToString() + "," + PhysInsp_RouteOrderNTP.ObjectClassPropId.ToString() + ")" );
                Collection<DataRow> OCPRowsToDelete = new Collection<DataRow>();
                foreach( DataRow OCPRow in OCPTable.Rows )
                {
                    OCPRowsToDelete.Add( OCPRow );
                }
                foreach( DataRow DoomedRow in OCPRowsToDelete )
                {
                    DoomedRow.Delete();
                }
                OCPUpdate.update( OCPTable );

                //  3. delete nodetype props
                _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

                foreach( CswNbtMetaDataNodeTypeProp DoomedNTP in NTPsToDelete )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( DoomedNTP );
                }
            }
            // Add Route relationship to Mount Point
            CswNbtMetaDataNodeTypeProp MountPoint_RouteNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Route", MountPointNT.getFirstNodeTypeTab().TabId );
            MountPoint_RouteNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), RouteNT.NodeTypeId, string.Empty, Int32.MinValue );

            // Add grid of Mount Points to Route
            CswNbtMetaDataNodeTypeTab RouteMountPointsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( RouteNT, "FE Inspection Points", 2 );
            CswNbtMetaDataNodeTypeProp Route_MountPointsGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( RouteNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Inspection Points Grid", RouteMountPointsTab.TabId );

            CswNbtView RouteMountPointsGridView = _CswNbtSchemaModTrnsctn.restoreView( Route_MountPointsGridNTP.ViewId );
            CswNbtViewRelationship RouteRel = RouteMountPointsGridView.AddViewRelationship( RouteNT, true );
            CswNbtViewRelationship MountPointsRel = RouteMountPointsGridView.AddViewRelationship( RouteRel, CswNbtViewRelationship.PropOwnerType.Second, MountPoint_RouteNTP, true );
            CswNbtViewProperty BarcodeViewProp = RouteMountPointsGridView.AddViewProperty( MountPointsRel, MountPoint_BarcodeNTP );
            CswNbtViewProperty DescriptionViewProp = RouteMountPointsGridView.AddViewProperty( MountPointsRel, MountPoint_DescriptionNTP );
            CswNbtViewProperty LocationViewProp = RouteMountPointsGridView.AddViewProperty( MountPointsRel, MountPoint_LocationNTP );
            CswNbtViewProperty LastInspDateViewProp = RouteMountPointsGridView.AddViewProperty( MountPointsRel, MountPoint_LastInspDateNTP );
            CswNbtViewProperty StatusViewProp = RouteMountPointsGridView.AddViewProperty( MountPointsRel, MountPoint_StatusNTP );
            BarcodeViewProp.Order = 1;
            LocationViewProp.Order = 2;
            DescriptionViewProp.Order = 3;
            LastInspDateViewProp.Order = 4;
            StatusViewProp.Order = 5;
            RouteMountPointsGridView.save();

            // Add Route management view
            CswNbtView RouteMgrView = _CswNbtSchemaModTrnsctn.makeView();
            RouteMgrView.makeNew( "Route Assignment", NbtViewVisibility.Global, null, null, null );
            RouteMgrView.ViewMode = NbtViewRenderingMode.Tree;
            RouteMgrView.Category = "Inspections";
            CswNbtViewRelationship RoutesRel = RouteMgrView.AddViewRelationship( RouteNT, true );
            CswNbtViewRelationship UsersRel = RouteMgrView.AddViewRelationship( RoutesRel, CswNbtViewRelationship.PropOwnerType.First, Route_InspectorNTP, true );
            RouteMgrView.save();

            // Relationships to route in master data
            Collection<CswNbtNode> RouteNodes = RouteNT.getNodes( false, false );
            if( RouteNodes.Count > 0 )
            {
                CswNbtNode FirstRouteNode = RouteNodes[0];

                // route's inspector
                Collection<CswNbtNode> UserNodes = UserOC.getNodes( false, false );
                CswNbtNode InspectorNode = null;
                foreach( CswNbtNode UserNode in UserNodes )
                {
                    if( CswNbtNodeCaster.AsUser( UserNode ).Username.ToLower() == "inspector" )
                    {
                        InspectorNode = UserNode;
                        break;
                    }
                }
                if( InspectorNode != null )
                {
                    FirstRouteNode.Properties[Route_InspectorNTP].AsRelationship.RelatedNodeId = InspectorNode.NodeId;
                    FirstRouteNode.postChanges( false );
                }

                // mount point's route
                Collection<CswNbtNode> MountPointNodes = MountPointNT.getNodes( false, false );
                foreach( CswNbtNode MountPointNode in MountPointNodes )
                {
                    MountPointNode.Properties[MountPoint_RouteNTP].AsRelationship.RelatedNodeId = FirstRouteNode.NodeId;
                    MountPointNode.postChanges( false );
                }

            } // if( RouteNodes.Count > 0 ) 


            // New view: My Open Inspections
            CswNbtView MyInspView = _CswNbtSchemaModTrnsctn.makeView();
            MyInspView.makeNew( "My Open Inspections", NbtViewVisibility.Global, null, null, null );
            MyInspView.Category = "Inspections";
            MyInspView.ViewMode = NbtViewRenderingMode.Tree;
            MyInspView.ForMobile = true;

            CswNbtViewRelationship MyRouteRel = MyInspView.AddViewRelationship( RouteNT, true );
            MyRouteRel.ShowInTree = false;
            CswNbtViewProperty RouteInspectorViewProp = MyInspView.AddViewProperty( MyRouteRel, Route_InspectorNTP );
            CswNbtViewPropertyFilter RouteInspectorMeFilter = MyInspView.AddViewPropertyFilter( RouteInspectorViewProp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, "me", false );

            CswNbtViewRelationship MPRel = MyInspView.AddViewRelationship( MyRouteRel, CswNbtViewRelationship.PropOwnerType.Second, MountPoint_RouteNTP, true );
            MPRel.ShowInTree = false;

            CswNbtViewRelationship PIRel = MyInspView.AddViewRelationship( MPRel, CswNbtViewRelationship.PropOwnerType.Second, PhysInsp_MountPointNTP, true );

            CswNbtViewProperty PI_BarcodeProp = MyInspView.AddViewProperty( PIRel, PhysInsp_BarcodeNTP );
            CswNbtViewProperty PI_DueDateProp = MyInspView.AddViewProperty( PIRel, PhysInsp_DueDateNTP );
            CswNbtViewProperty PI_FinishedProp = MyInspView.AddViewProperty( PIRel, PhysInsp_FinishedNTP );
            CswNbtViewProperty PI_GeneratorProp = MyInspView.AddViewProperty( PIRel, PhysInsp_GeneratorNTP );
            CswNbtViewProperty PI_LocationProp = MyInspView.AddViewProperty( PIRel, PhysInsp_LocationNTP );
            CswNbtViewProperty PI_NameProp = MyInspView.AddViewProperty( PIRel, PhysInsp_NameNTP );
            CswNbtViewProperty PI_StatusProp = MyInspView.AddViewProperty( PIRel, PhysInsp_StatusNTP );
            CswNbtViewProperty PI_TargetProp = MyInspView.AddViewProperty( PIRel, PhysInsp_TargetNTP );

            CswNbtViewPropertyFilter PI_BarcodeFilter = MyInspView.AddViewPropertyFilter( PI_BarcodeProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Contains, "", false );
            CswNbtViewPropertyFilter PI_DueDateFilter = MyInspView.AddViewPropertyFilter( PI_DueDateProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, "today+7", false );
            CswNbtViewPropertyFilter PI_FinishedFilter = MyInspView.AddViewPropertyFilter( PI_FinishedProp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Tristate.True.ToString(), false );
            CswNbtViewPropertyFilter PI_GeneratorFilter = MyInspView.AddViewPropertyFilter( PI_GeneratorProp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, "", false );
            CswNbtViewPropertyFilter PI_LocationFilter = MyInspView.AddViewPropertyFilter( PI_LocationProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Begins, "", false );
            CswNbtViewPropertyFilter PI_NameFilter = MyInspView.AddViewPropertyFilter( PI_NameProp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, "", false );
            CswNbtViewPropertyFilter PI_StatusFilter1 = MyInspView.AddViewPropertyFilter( PI_StatusProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, "Missed", false );
            CswNbtViewPropertyFilter PI_StatusFilter2 = MyInspView.AddViewPropertyFilter( PI_StatusProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, "Completed", false );
            CswNbtViewPropertyFilter PI_StatusFilter3 = MyInspView.AddViewPropertyFilter( PI_StatusProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, "Completed Late", false );
            CswNbtViewPropertyFilter PI_StatusFilter4 = MyInspView.AddViewPropertyFilter( PI_StatusProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, "Cancelled", false );
            CswNbtViewPropertyFilter PI_TargetFilter = MyInspView.AddViewPropertyFilter( PI_TargetProp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, "", false );

            MyInspView.save();

            // Rename "Mount Points by Location"
            CswNbtView MPBLView = _CswNbtSchemaModTrnsctn.restoreView( "Mount Points by Location" );
			if( MPBLView != null )
			{
				MPBLView.ViewName = "FE Inspection Points By Location";
				MPBLView.save();
			}

        } // update()

    }//class CswUpdateSchemaTo01H22

}//namespace ChemSW.Nbt.Schema

