using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-03
    /// </summary>
    public class CswUpdateSchemaTo01H03 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        /// <summary>
        /// Schema version 01H-03
        /// </summary>
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 03 ); } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtSchemaModTrnsctn"></param>
        public CswUpdateSchemaTo01H03( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        /// <summary>
        /// 01H-03 Update()
        /// </summary>
        public void update()
        {
            // Because of changes in previous script
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            // Case 20002
            Int32 FEModuleId = _CswNbtSchemaModTrnsctn.createModule( "Fire Extinguisher", "FE", true );
            CswNbtMetaDataObjectClass MountPointOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MountPointClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, MountPointOC.ObjectClassId );
            CswNbtMetaDataObjectClass FireExtinguisherOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.FireExtinguisherClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, FireExtinguisherOC.ObjectClassId );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );

            // Case 20062
            Int32 FEImportActionid = _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Import_Fire_Extinguisher_Data, true, "Act_ImportFireExtinguisher.aspx", "System" );

            // Case 20033
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( FEModuleId, FEImportActionid );

            CswTableSelect ActionsTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "actionid", "actions" );
            DataTable ActionsDT = ActionsTable.getTable( " where lower(actionname) like '%inspection%' " );
            Int32 Actionid;
            for( Int32 i = 0; i <= (ActionsDT.Rows.Count - 1); i++ )
            {
                Actionid = CswConvert.ToInt32( ActionsDT.Rows[i]["actionid"].ToString() );
                if ( Int32.MinValue != Actionid )
                    _CswNbtSchemaModTrnsctn.createModuleActionJunction(FEModuleId, Actionid);
                Actionid = Int32.MinValue;
            }

            //DT
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-03_OCP_Update", "object_class_props" );
            DataTable NewOCPTable = OCPUpdate.getEmptyTable();

            //Mount Point (MP) OCPs
            string Status = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected );
            Status += "," + CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.OK );
            Status += "," + CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.OOC );

            //MP: Last Inspection Date
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC, CswNbtObjClassMountPoint.LastInspectionDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Date,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );
            //MP: Status
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC, CswNbtObjClassMountPoint.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, Status,
                                                           Int32.MinValue, Int32.MinValue );
            //MP: Location
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC, CswNbtObjClassMountPoint.LocationPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Location,
                                                           false, false, true, "ObjectClassId", LocationOC.ObjectClassId, true, false, false, false, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );

            //FE OC Props

            //FE: Last Inspection Date
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC, CswNbtObjClassFireExtinguisher.LastInspectionDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Date,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );
            //FE: Status
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC, CswNbtObjClassFireExtinguisher.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, Status,
                                                           Int32.MinValue, Int32.MinValue );
            //FE: Mount Point
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC, CswNbtObjClassFireExtinguisher.MountPointPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false, false, true, "ObjectClassId", MountPointOC.ObjectClassId, false, false, false, false, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );

            // Case 20058
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC.ObjectClassId, "Description", CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC.ObjectClassId, "Description", CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC.ObjectClassId, "Type", CswNbtMetaDataFieldType.NbtFieldType.List, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC.ObjectClassId, "Type", CswNbtMetaDataFieldType.NbtFieldType.List, Int32.MinValue, Int32.MinValue );

            // Last DT Op
            OCPUpdate.update( NewOCPTable );
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            // Default values
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( MountPointOC.getObjectClassProp( CswNbtObjClassMountPoint.StatusPropertyName ),
                                                                 CswNbtSubField.SubFieldName.Value,
                                                                 "Not Inspected" );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.StatusPropertyName ),
                                                                 CswNbtSubField.SubFieldName.Value,
                                                                 "Not Inspected" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.MountPointPropertyName ), "setvalonadd", true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.TypePropertyName ), "setvalonadd", true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MountPointOC.getObjectClassProp( CswNbtObjClassMountPoint.TypePropertyName ), "setvalonadd", true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.DescriptionPropertyName ), "setvalonadd", true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MountPointOC.getObjectClassProp( CswNbtObjClassMountPoint.DescriptionPropertyName ), "setvalonadd", true );

            //Mount Point Group NT
            CswNbtMetaDataObjectClass GenericOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass );
            CswNbtMetaDataNodeType MountPointGroupNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOC.ObjectClassId, "Mount Point Group", "Fire Extinguisher" );
            MountPointGroupNT.IconFileName = "group.gif";
            CswNbtMetaDataNodeTypeProp MountPointGroupNameNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointGroupNT, CswNbtMetaDataFieldType.NbtFieldType.Text, "Name", Int32.MinValue );
            MountPointGroupNameNTP.IsUnique = true;
            MountPointGroupNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( "Name" );

            //New Mount Point Group Node
            CswNbtNode MPGroupNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MountPointGroupNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            MPGroupNode.Properties[MountPointGroupNameNTP].AsText.Text = "A";
            MPGroupNode.postChanges( true );

            //Mount Point NT with Hydrostatic Inspection, Barcode and Mount Point Group Props
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( MountPointOC.ObjectClassId, "Mount Point", "Fire Extinguisher" );
            MountPointNT.IconFileName = "safecab.gif";
            CswNbtMetaDataNodeTypeProp MPHydrostaticInspectionNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Date, "Hydrostatic Inspection", Int32.MinValue );
            CswNbtMetaDataNodeTypeProp MPBarcodeNTP =  _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Barcode, "Barcode", Int32.MinValue );
            MountPointNT.NameTemplateText = ( CswNbtMetaData.MakeTemplateEntry( "Barcode" ) + " " + 
                                              CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMountPoint.DescriptionPropertyName ) );
            CswNbtMetaDataNodeTypeProp MountPointGroupNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Mount Point Group", Int32.MinValue );
            MountPointGroupNTP.SetValueOnAdd = true;
            MountPointGroupNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointGroupNT.NodeTypeId, string.Empty, Int32.MinValue );
            MountPointGroupNTP.DefaultValue.AsRelationship.RelatedNodeId = MPGroupNode.NodeId;

            //Fire Extinguisher NT
            CswNbtMetaDataNodeType FireExtinguisherNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Extinguisher" );
            if( FireExtinguisherNT != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( FireExtinguisherNT );
            }
            FireExtinguisherNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( FireExtinguisherOC.ObjectClassId, "Fire Extinguisher", "Fire Extinguisher" );
            FireExtinguisherNT.IconFileName = "fireext.gif";
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FireExtinguisherNT, CswNbtMetaDataFieldType.NbtFieldType.Date, "Hydrostatic Inspection", Int32.MinValue );
            CswNbtMetaDataNodeTypeProp FEBarcodeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FireExtinguisherNT, CswNbtMetaDataFieldType.NbtFieldType.Barcode, "Barcode", Int32.MinValue );
            FireExtinguisherNT.NameTemplateText = ( CswNbtMetaData.MakeTemplateEntry( "Barcode" ) + " " +
                                                    CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassFireExtinguisher.DescriptionPropertyName ) );
            
            // Case 20058
            CswNbtMetaDataNodeType VendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
            CswNbtMetaDataNodeTypeProp FEManufacturerNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FireExtinguisherNT, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Manufacturer", Int32.MinValue );
            if( null != VendorNT )
                FEManufacturerNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), VendorNT.NodeTypeId, string.Empty, Int32.MinValue );

            CswNbtMetaDataNodeTypeProp FEModelNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FireExtinguisherNT, CswNbtMetaDataFieldType.NbtFieldType.Text, "Model", Int32.MinValue );
            CswNbtMetaDataNodeTypeProp FESizeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FireExtinguisherNT, CswNbtMetaDataFieldType.NbtFieldType.Quantity, "Size", Int32.MinValue );
            CswNbtMetaDataNodeTypeProp FETypeProp = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.TypePropertyName );
            FETypeProp.ListOptions = "Water,Foam,Powder ABE,Powder BE,CO2,Wet Chemical";
            CswNbtMetaDataNodeTypeProp MPTypeProp = MountPointNT.getNodeTypeProp( CswNbtObjClassMountPoint.TypePropertyName );
            MPTypeProp.ListOptions = "Water,Foam,Powder ABE,Powder BE,CO2,Wet Chemical";

            //Update NT Relationships to Target NT
            CswNbtMetaDataNodeTypeProp FEMountPointProp = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.MountPointPropertyName );
            FEMountPointProp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue );

            // </ Case 20002 >

            // < Case 20005 >
            //New FE Generator NT
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataNodeType PhysicalInspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GeneratorOC.ObjectClassId, "Physical Inspection Schedule", "Fire Extinguisher" );
            PhysicalInspectionScheduleNT.IconFileName = "clock.gif";
            PhysicalInspectionScheduleNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassGenerator.DescriptionPropertyName );

            //Generator Due Date Interval is Monthly on 1st
            CswNbtMetaDataNodeTypeProp DueDateIntervalNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.DueDateIntervalPropertyName );
            DueDateIntervalNTP.DefaultValue.AsTimeInterval.RateInterval.setMonthlyByDate( 1, 1, DateTime.Today.Month, DateTime.Today.Year );

            //Generator Parent type is Mount Point
            CswNbtMetaDataNodeTypeProp ParentTypeNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
            ParentTypeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue );

            //Generator Owner is Mount Point Group
            CswNbtMetaDataNodeTypeProp OwnerNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
            OwnerNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointGroupNT.NodeTypeId, string.Empty, Int32.MinValue );
            OwnerNTP.DefaultValue.AsRelationship.RelatedNodeId = MPGroupNode.NodeId;

            //Build the default ParentView
            CswNbtMetaDataNodeTypeProp MountPointNTP = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassFireExtinguisher.MountPointPropertyName );

            //CswNbtView ParentView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtMetaDataNodeTypeProp ParentViewNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );
            CswNbtView ParentView = CswNbtViewFactory.restoreView( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources, ParentViewNTP.ViewId );
            ParentView.ViewName = "Physical Inspection Schedule ParentView";
            CswNbtViewRelationship ParentRelationship = ParentView.AddViewRelationship( PhysicalInspectionScheduleNT, true );
            CswNbtViewRelationship MountPointGroupChild = ParentView.AddViewRelationship( ParentRelationship, CswNbtViewRelationship.PropOwnerType.First, OwnerNTP, true );
            CswNbtViewRelationship MountPointChild = ParentView.AddViewRelationship( MountPointGroupChild, CswNbtViewRelationship.PropOwnerType.Second, MountPointGroupNTP, true );
            CswNbtViewRelationship FireExtinguisherChild = ParentView.AddViewRelationship( MountPointChild, CswNbtViewRelationship.PropOwnerType.Second, MountPointNTP, true );
            ParentView.save();            

            //FE Route NT
            CswNbtMetaDataObjectClass InspectionRouteOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );
            CswNbtMetaDataNodeType PhysicalInspectionRouteNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InspectionRouteOC.ObjectClassId, "Physical Inspection Route", "Fire Extinguisher" );
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

            CswNbtMetaDataNodeType PhysicalInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InspectionDesignOC.ObjectClassId, "Physical Inspection", "Fire Extinguisher" );
            PhysicalInspectionNT.IconFileName = "test.gif";
            PhysicalInspectionNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInspectionDesign.NamePropertyName );

            // Physical Inspection has a Fire Extinguisher Relationship
            CswNbtMetaDataNodeTypeProp PIFireExtinguisherNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( PhysicalInspectionNT, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Fire Extinguisher", Int32.MinValue );
            PIFireExtinguisherNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), FireExtinguisherNT.NodeTypeId, string.Empty, Int32.MinValue );
            PIFireExtinguisherNTP.UseNumbering = false;

            //Generator Target NT is Inspection
            CswNbtMetaDataNodeTypeProp GeneratorTargetTypeNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
            GeneratorTargetTypeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionNT.NodeTypeId, string.Empty, Int32.MinValue );

            // </ Case 20005>

            //Inspection has MP Parent/Owner
            CswNbtMetaDataNodeTypeProp InspectionParentNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.OwnerPropertyName );
            InspectionParentNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue );

            //Inspection has FE Generator
            CswNbtMetaDataNodeTypeProp InspectionGeneratorNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            InspectionGeneratorNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionScheduleNT.NodeTypeId, string.Empty, Int32.MinValue );

            //Inspection has FE Route
            CswNbtMetaDataNodeTypeProp RouteNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.RoutePropertyName );
            RouteNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionRouteNT.NodeTypeId, string.Empty, Int32.MinValue );

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

                Permissions.SetValue( NodeTypePermission.Create.ToString(), FireExtinguisherNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), FireExtinguisherNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), FireExtinguisherNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), FireExtinguisherNT.NodeTypeId.ToString(), true );

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

                // Case 20033: Add action permissions to FE
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
                CswNbtMetaDataNodeTypeProp PINameNTP = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
                CswNbtViewProperty PINameVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PINameNTP );
                PhysicalInspectionView.AddViewPropertyFilter( PINameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                CswNbtMetaDataNodeTypeProp PIDueDateNTP = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
                CswNbtViewProperty PIDueDateVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PIDueDateNTP );
                PhysicalInspectionView.AddViewPropertyFilter( PIDueDateVP, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                CswNbtMetaDataNodeTypeProp PIRouteNTP = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.RoutePropertyName );
                CswNbtViewProperty PIRouteVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PIRouteNTP );
                PhysicalInspectionView.AddViewPropertyFilter( PIRouteVP, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                CswNbtMetaDataNodeTypeProp PIStatusNTP = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );
                CswNbtViewProperty PIStatusVP = PhysicalInspectionView.AddViewProperty( InspectionRelationship, PIStatusNTP );
                PhysicalInspectionView.AddViewPropertyFilter( PIStatusVP, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                //Fire Extinguisher: Barcode and Last Inspection Date
                CswNbtViewRelationship FireExtinguisherRelationship = PhysicalInspectionView.AddViewRelationship( InspectionRelationship, CswNbtViewRelationship.PropOwnerType.First, PIFireExtinguisherNTP, false );
                CswNbtMetaDataNodeTypeProp FEDateNTP = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassFireExtinguisher.LastInspectionDatePropertyName );
                CswNbtViewProperty FESDateVP = PhysicalInspectionView.AddViewProperty( FireExtinguisherRelationship, FEDateNTP );
                PhysicalInspectionView.AddViewPropertyFilter( FESDateVP, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                CswNbtViewProperty FEBarcodeVP = PhysicalInspectionView.AddViewProperty( FireExtinguisherRelationship, FEBarcodeNTP );
                PhysicalInspectionView.AddViewPropertyFilter( FEBarcodeVP, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                //Mount Point: Description
                CswNbtMetaDataNodeTypeProp PITargetNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                CswNbtViewRelationship FEMountPointRelationship = PhysicalInspectionView.AddViewRelationship( InspectionRelationship, CswNbtViewRelationship.PropOwnerType.First, PITargetNTP, false );
                CswNbtMetaDataNodeTypeProp MPDescriptionNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMountPoint.DescriptionPropertyName );
                CswNbtViewProperty MPDescriptionVP = PhysicalInspectionView.AddViewProperty( FEMountPointRelationship, MPDescriptionNTP );
                PhysicalInspectionView.AddViewPropertyFilter( MPDescriptionVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                //Mount Point Group: Name
                CswNbtViewRelationship FEMountPointGroupRelationship = PhysicalInspectionView.AddViewRelationship( FEMountPointRelationship, CswNbtViewRelationship.PropOwnerType.First, MountPointGroupNTP, false );
                CswNbtViewProperty MPGNameVP = PhysicalInspectionView.AddViewProperty( FEMountPointGroupRelationship, MountPointGroupNameNTP );
                PhysicalInspectionView.AddViewPropertyFilter( MPGNameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                PhysicalInspectionView.save();
            }

        }//Update()

    }//class CswUpdateSchemaTo01H03

}//namespace ChemSW.Nbt.Schema


