using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-04
    /// </summary>
    public class CswUpdateSchemaTo01H04 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        /// <summary>
        /// Schema version 01H-04
        /// </summary>
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 04 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtSchemaModTrnsctn"></param>
        public CswUpdateSchemaTo01H04( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );

        }

        /// <summary>
        /// 01H-04 Update()
        /// </summary>
        public void update()
        {
            // Because of changes in previous script
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            CswNbtMetaDataObjectClass MountPointOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass MountPointGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );

            // Case 20536 Inspection Target Group NT
            CswNbtMetaDataNodeType MountPointGroupNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( MountPointGroupOC.ObjectClassId, CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point_Group ), "Fire Extinguisher" );
            CswNbtMetaDataNodeTypeProp MountPointGroupNameNTP = MountPointGroupNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTargetGroup.NamePropertyName );
            MountPointGroupNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( "Name" );

            CswNbtNode MPGroupNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MountPointGroupNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            MPGroupNode.Properties[MountPointGroupNameNTP].AsText.Text = "A";
            MPGroupNode.postChanges( true );

            //Mount Point NT with Hydrostatic Inspection, Barcode and Mount Point Group Props
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( MountPointOC.ObjectClassId, CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ), "Fire Extinguisher" );
            MountPointNT.IconFileName = "safecab.gif";
            CswNbtMetaDataNodeTypeProp MPHydrostaticInspectionNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Date, "Hydrostatic Inspection", Int32.MinValue );
            CswNbtMetaDataNodeTypeProp MPBarcodeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Barcode, "Barcode", Int32.MinValue );
            MountPointNT.NameTemplateText = ( CswNbtMetaData.MakeTemplateEntry( "Barcode" ) + " " +
                                              CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInspectionTarget.DescriptionPropertyName ) );

            CswNbtMetaDataNodeTypeProp MountPointGroupNTP = MountPointNT.getNodeTypeProp( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            MountPointGroupNTP.PropName = "Mount Point Group";
            MountPointGroupNTP.SetValueOnAdd = true;
            MountPointGroupNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointGroupNT.NodeTypeId, string.Empty, Int32.MinValue );
            MountPointGroupNTP.DefaultValue.AsRelationship.RelatedNodeId = MPGroupNode.NodeId;

            // </ Case 20002 >

            // < Case 20005 >
            //New FE Generator NT
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataNodeType PhysicalInspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GeneratorOC.ObjectClassId, CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection_Schedule ), "Fire Extinguisher" );
            PhysicalInspectionScheduleNT.IconFileName = "clock.gif";
            PhysicalInspectionScheduleNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassGenerator.DescriptionPropertyName );

            //Generator Due Date Interval is Monthly on 1st
            CswNbtMetaDataNodeTypeProp DueDateIntervalNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.DueDateIntervalPropertyName );
            CswRateInterval MonthlyOnFirst = new CswRateInterval();
            MonthlyOnFirst.setMonthlyByDate( 1, 1, DateTime.Today.Month, DateTime.Today.Year );
            DueDateIntervalNTP.DefaultValue.AsTimeInterval.RateInterval = MonthlyOnFirst;

            //Generator Parent type is Mount Point
            CswNbtMetaDataNodeTypeProp ParentTypeNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
            ParentTypeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue );
            ParentTypeNTP.DefaultValue.AsNodeTypeSelect.SelectedNodeTypeIds.Add( MountPointNT.NodeTypeId.ToString() );

            //Generator Owner is Mount Point Group
            CswNbtMetaDataNodeTypeProp OwnerNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
            OwnerNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointGroupNT.NodeTypeId, string.Empty, Int32.MinValue );
            OwnerNTP.DefaultValue.AsRelationship.RelatedNodeId = MPGroupNode.NodeId;

            //CswNbtView ParentView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtMetaDataNodeTypeProp ParentViewNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );
			CswNbtView ParentView = _CswNbtSchemaModTrnsctn.restoreView( ParentViewNTP.DefaultValue.AsViewReference.ViewId );
            ParentView.ViewName = "PI Schedule ParentView";
            CswNbtViewRelationship ParentRelationship = ParentView.AddViewRelationship( PhysicalInspectionScheduleNT, true );
            CswNbtViewRelationship MountPointGroupChild = ParentView.AddViewRelationship( ParentRelationship, CswNbtViewRelationship.PropOwnerType.First, OwnerNTP, true );
            CswNbtViewRelationship MountPointChild = ParentView.AddViewRelationship( MountPointGroupChild, CswNbtViewRelationship.PropOwnerType.Second, MountPointGroupNTP, true );
            ParentView.save();

            //FE Route NT
            CswNbtMetaDataObjectClass InspectionRouteOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );
            CswNbtMetaDataNodeType PhysicalInspectionRouteNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InspectionRouteOC.ObjectClassId, CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection_Route ), "Fire Extinguisher" );
            PhysicalInspectionRouteNT.IconFileName = "arrows.gif";
            CswNbtMetaDataNodeTypeProp RouteNameNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( PhysicalInspectionRouteNT, CswNbtMetaDataFieldType.NbtFieldType.Text, "Name", Int32.MinValue );
            RouteNameNTP.SetValueOnAdd = true;
            PhysicalInspectionRouteNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( "Name" );

            // < Case 20004 >

            //FE Physical Inspection NT
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataNodeType OldFireExtinguisherInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Extinguisher Inspection" );
            if( null != OldFireExtinguisherInspectionNT )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( OldFireExtinguisherInspectionNT );
            }

            CswNbtMetaDataNodeType PhysicalInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InspectionDesignOC.ObjectClassId, CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection ), "Fire Extinguisher" );
            PhysicalInspectionNT.IconFileName = "test.gif";
            PhysicalInspectionNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInspectionDesign.NamePropertyName );

            //Generator Target NT is Inspection
            CswNbtMetaDataNodeTypeProp GeneratorTargetTypeNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
            GeneratorTargetTypeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionNT.NodeTypeId, string.Empty, Int32.MinValue );
            GeneratorTargetTypeNTP.DefaultValue.AsNodeTypeSelect.SelectedNodeTypeIds.Add( PhysicalInspectionNT.NodeTypeId.ToString() );

            // </ Case 20005>

            //Inspection has MP Parent/Owner
            CswNbtMetaDataNodeTypeProp InspectionParentNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.OwnerPropertyName );
            InspectionParentNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue );

            //Inspection has FE Generator
            CswNbtMetaDataNodeTypeProp InspectionGeneratorNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            InspectionGeneratorNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionScheduleNT.NodeTypeId, string.Empty, Int32.MinValue );

            ////Inspection has FE Route
            //CswNbtMetaDataNodeTypeProp RouteNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.RoutePropertyName );
            //RouteNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionRouteNT.NodeTypeId, string.Empty, Int32.MinValue );

            // </ Case 20004 >

            //Grant admin user FE NT permissions
            CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( RoleNode != null )
            {
                CswNbtNodePropLogicalSet Permissions = ( (CswNbtObjClassRole) CswNbtNodeCaster.AsRole( RoleNode ) ).NodeTypePermissions;

                Permissions.SetValue( NodeTypePermission.Create.ToString(), MountPointNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), MountPointNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), MountPointNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), MountPointNT.NodeTypeId.ToString(), true );

                Permissions.SetValue( NodeTypePermission.Create.ToString(), MountPointGroupNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), MountPointGroupNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), MountPointGroupNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), MountPointGroupNT.NodeTypeId.ToString(), true );

                Permissions.SetValue( NodeTypePermission.Create.ToString(), PhysicalInspectionScheduleNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), PhysicalInspectionScheduleNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), PhysicalInspectionScheduleNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), PhysicalInspectionScheduleNT.NodeTypeId.ToString(), true );

                Permissions.SetValue( NodeTypePermission.Create.ToString(), PhysicalInspectionNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), PhysicalInspectionNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), PhysicalInspectionNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), PhysicalInspectionNT.NodeTypeId.ToString(), true );

                Permissions.SetValue( NodeTypePermission.Create.ToString(), PhysicalInspectionRouteNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), PhysicalInspectionRouteNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), PhysicalInspectionRouteNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), PhysicalInspectionRouteNT.NodeTypeId.ToString(), true );

                Permissions.Save();
                RoleNode.postChanges( true );

                // Case 20043: Add action permissions to FE
                _CswNbtSchemaModTrnsctn.GrantActionPermission( RoleNode, CswNbtActionName.Assign_Inspection );
                _CswNbtSchemaModTrnsctn.GrantActionPermission( RoleNode, CswNbtActionName.Create_Inspection );
                _CswNbtSchemaModTrnsctn.GrantActionPermission( RoleNode, CswNbtActionName.Inspection_Design );
            }

            //_CswNbtSchemaModTrnsctn.MetaData.refreshAll();
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            // Case 20008
            CswNbtNode AdminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( AdminRoleNode != null )
            {
                CswNbtView MountPointGroupView = _CswNbtSchemaModTrnsctn.makeView();
                MountPointGroupView.makeNew( "Mount Point Groups", NbtViewVisibility.Role, AdminRoleNode.NodeId, null, null );
                MountPointGroupView.Category = "System";
                CswNbtViewRelationship GroupRelationship = MountPointGroupView.AddViewRelationship( MountPointGroupNT, false );
                CswNbtViewRelationship MountPointRelationship = MountPointGroupView.AddViewRelationship( GroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, MountPointGroupNTP, false );
                MountPointGroupView.save();

                CswNbtView ScheduleView = _CswNbtSchemaModTrnsctn.makeView();
                ScheduleView.makeNew( "Physical Inspection Schedule", NbtViewVisibility.Role, AdminRoleNode.NodeId, null, null );
                ScheduleView.Category = "Inspections";
                CswNbtViewRelationship SGroupRelationship = ScheduleView.AddViewRelationship( MountPointGroupNT, false );
                CswNbtViewRelationship ScheduleRelationship = ScheduleView.AddViewRelationship( SGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, OwnerNTP, false );
                ScheduleView.save();

                // 20094
                CswNbtView PhysicalInspectionView = _CswNbtSchemaModTrnsctn.makeView();
                PhysicalInspectionView.makeNew( "Physical Inspections", NbtViewVisibility.Global, null, null, null );
                PhysicalInspectionView.Category = "Inspections";
                PhysicalInspectionView.SetViewMode( NbtViewRenderingMode.Grid );

                //Physical Inspection: Name, Due Date, Route, Status
                CswNbtViewRelationship InspectionRelationship = PhysicalInspectionView.AddViewRelationship( PhysicalInspectionNT, false );
                CswNbtMetaDataNodeTypeProp PINameNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
                CswNbtViewProperty PINameVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PINameNTP );
                PhysicalInspectionView.AddViewPropertyFilter( PINameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );
                CswNbtMetaDataNodeTypeProp PIDueDateNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
                CswNbtViewProperty PIDueDateVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PIDueDateNTP );
                PhysicalInspectionView.AddViewPropertyFilter( PIDueDateVP, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );
                //CswNbtMetaDataNodeTypeProp PIRouteNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.RoutePropertyName );
                //CswNbtViewProperty PIRouteVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PIRouteNTP );
                //PhysicalInspectionView.AddViewPropertyFilter( PIRouteVP, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );
                CswNbtMetaDataNodeTypeProp PIStatusNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );
                CswNbtViewProperty PIStatusVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PIStatusNTP );
                PhysicalInspectionView.AddViewPropertyFilter( PIStatusVP, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                //Mount Point: Description
                CswNbtMetaDataNodeTypeProp PITargetNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                CswNbtViewRelationship FEMountPointRelationship = PhysicalInspectionView.AddViewRelationship( InspectionRelationship, CswNbtViewRelationship.PropOwnerType.First, PITargetNTP, false );
                CswNbtMetaDataNodeTypeProp MPDescriptionNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
                CswNbtViewProperty MPDescriptionVP = PhysicalInspectionView.AddViewProperty( FEMountPointRelationship, MPDescriptionNTP );
                PhysicalInspectionView.AddViewPropertyFilter( MPDescriptionVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

                //Mount Point Group: Name
                CswNbtViewRelationship FEMountPointGroupRelationship = PhysicalInspectionView.AddViewRelationship( FEMountPointRelationship, CswNbtViewRelationship.PropOwnerType.First, MountPointGroupNTP, false );
                CswNbtViewProperty MPGNameVP = PhysicalInspectionView.AddViewProperty( FEMountPointGroupRelationship, MountPointGroupNameNTP );
                PhysicalInspectionView.AddViewPropertyFilter( MPGNameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

                PhysicalInspectionView.save();
            }

        }//Update()

    }//class CswUpdateSchemaTo01H04

}//namespace ChemSW.Nbt.Schema


