using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
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

            //BZ 10021
            Int32 FEModuleId = _CswNbtSchemaModTrnsctn.createModule( "Fire Extinguisher", "FE", true );
            CswNbtMetaDataObjectClass MountPointOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MountPointClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, MountPointOC.ObjectClassId );
            CswNbtMetaDataObjectClass LocationGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationGroupClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, LocationGroupOC.ObjectClassId );
            CswNbtMetaDataObjectClass FireExtinguisherOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.FireExtinguisherClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, FireExtinguisherOC.ObjectClassId );

            //DT
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-03_OCP_Update", "object_class_props" );
            DataTable NewOCPTable = OCPUpdate.getEmptyTable();

            //Add Location Group Relationship Prop to Location OC
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );

            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, LocationOC, CswNbtObjClassLocation.LocationGroupPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false, false, true, "ObjectClassId", LocationGroupOC.ObjectClassId, false, false, false, false, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );

            //Mount Point (MP) OCPs

            //MP: Last Inspection Date
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC, CswNbtObjClassMountPoint.LastInspectionDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Date,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );
            //MP: Status
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC, CswNbtObjClassMountPoint.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, "OK,OOC",
                                                           Int32.MinValue, Int32.MinValue );
            //MP: Location
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, MountPointOC, CswNbtObjClassMountPoint.LocationPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Location,
                                                           false, false, true, "ObjectClassId", LocationOC.ObjectClassId, true, false, false, false, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );

            //Location Group (LG) OC Props

            //LG: Name
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, LocationGroupOC, CswNbtObjClassLocationGroup.LocationGroupNamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text,
                                                           false, false, false, string.Empty, Int32.MinValue, true, true, false, false, string.Empty, 
                                                           Int32.MinValue, Int32.MinValue );

            //FE OC Props

            //FE: Last Inspection Date
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC, CswNbtObjClassFireExtinguisher.LastInspectionDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Date,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );
            //FE: Status
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC, CswNbtObjClassFireExtinguisher.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, "OK,OOC",
                                                           Int32.MinValue, Int32.MinValue );
            //FE: Mount Point
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC, CswNbtObjClassFireExtinguisher.MountPointPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false, false, true, "ObjectClassId", MountPointOC.ObjectClassId, false, false, false, false, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );

            //Last DT Op
            OCPUpdate.update( NewOCPTable );
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            // Default values
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( MountPointOC.getObjectClassProp( CswNbtObjClassMountPoint.StatusPropertyName ),
                                                                 CswNbtSubField.SubFieldName.Value,
                                                                 "OK" );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.StatusPropertyName ),
                                                                 CswNbtSubField.SubFieldName.Value,
                                                                 "OK" );            

            //Default Mount Point NT with Hydrostatic Inspection and Barcode Props
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( MountPointOC.ObjectClassId, "Mount Point", "Fire Extinguisher" );
            MountPointNT.IconFileName = "room.gif";
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Date, "Hydrostatic Inspection", Int32.MinValue );
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Barcode, "Barcode", Int32.MinValue );

            //Default Location Group NT
            CswNbtMetaDataNodeType LocationGroupNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( LocationGroupOC.ObjectClassId, "Location Group", "Fire Extinguisher" );
            LocationGroupNT.IconFileName = "group.gif";
            LocationGroupNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassLocationGroup.LocationGroupNamePropertyName );

            //Default Fire Extinguisher NT
            CswNbtMetaDataNodeType FireExtinguisherNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Extinguisher" );
            if( FireExtinguisherNT != null )
            {
                CswNbtMetaDataObjectClass FireExtinguisherMDOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( FireExtinguisherOC.ObjectClassId );
                _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( FireExtinguisherNT, FireExtinguisherMDOC );
                FireExtinguisherNT.Category = "Fire Extinguisher";
            }
            else
            {
                FireExtinguisherNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( FireExtinguisherOC.ObjectClassId, "Fire Extinguisher", "Fire Extinguisher" );
            }
            FireExtinguisherNT.IconFileName = "fireext.gif";
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FireExtinguisherNT, CswNbtMetaDataFieldType.NbtFieldType.Date, "Hydrostatic Inspection", Int32.MinValue );
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FireExtinguisherNT, CswNbtMetaDataFieldType.NbtFieldType.Barcode, "Barcode", Int32.MinValue );

            //Update NT Relationships to Target NT
            CswNbtMetaDataNodeTypeProp FEMountPointProp = FireExtinguisherNT.getNodeTypeProp( CswNbtObjClassFireExtinguisher.MountPointPropertyName );
            FEMountPointProp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue );

            //New LG Node
            CswNbtNode LocationGroupNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( LocationGroupNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassLocationGroup LGNode = CswNbtNodeCaster.AsLocationGroup( LocationGroupNode );
            LGNode.LocationGroupName.Text = "A";
            LGNode.postChanges( true );

            CswNbtMetaDataNodeTypeProp LocationLGProp;
            CswNbtView LocationView = null;
            ICswNbtTree LocationTree = null;

            foreach( CswNbtMetaDataNodeType NodeType in LocationOC.NodeTypes )
            {
                //Update Location NTs to Target Location Group NT
                LocationLGProp = NodeType.getNodeTypeProp( "Location Group" );

                if( LocationLGProp != null )
                {
                    LocationLGProp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), LocationGroupNT.NodeTypeId, string.Empty, Int32.MinValue );
                    //LocationLGProp.DefaultValue = LocationGroupNode.NodeId.ToString();
                }
                //Assign Existing Location Relationships to New LG Node
                LocationView = _CswNbtSchemaModTrnsctn.getTreeViewOfNodeType( NodeType.NodeTypeId );
                LocationTree = _CswNbtSchemaModTrnsctn.getTreeFromView( LocationView, false );
                for( Int32 i = 0; i < LocationTree.getChildNodeCount(); i++ )
                {
                    LocationTree.goToNthChild( i );
                    CswNbtNode Node = LocationTree.getNodeForCurrentPosition();
                    CswNbtObjClassLocation LocationNode = CswNbtNodeCaster.AsLocation( Node );
                    LocationNode.LocationGroup.RelatedNodeId = LocationGroupNode.NodeId;
                    LocationNode.postChanges( true );
                    LocationTree.goToParentNode();
                }

            }//foreach (NodeType)
            //</BZ 10021>

            // BZ 10405
            //New FE Generator NT
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataNodeType PhysicalInspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GeneratorOC.ObjectClassId, "Physical Inspection Schedule", "Fire Extinguisher" );
            PhysicalInspectionScheduleNT.IconFileName = "clock.gif";

            //Generator Parent type is Mount Point
            CswNbtMetaDataNodeTypeProp ParentTypeNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
            ParentTypeNTP.SetFK(CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue);

            //Generator Owner is Location Group
            CswNbtMetaDataNodeTypeProp OwnerNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName ); 
            OwnerNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), LocationGroupNT.NodeTypeId, string.Empty, Int32.MinValue );
            OwnerNTP.DefaultValue.AsRelationship.RelatedNodeId = LocationGroupNode.NodeId;

            //Build the default ParentView
            CswNbtMetaDataNodeTypeProp LocationGroupNTP = LocationGroupNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationGroupPropertyName);
            CswNbtMetaDataObjectClassProp LocationOCP = MountPointOC.getObjectClassProp( CswNbtObjClassMountPoint.LocationPropertyName);
            CswNbtMetaDataNodeTypeProp MountPointNTP = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassFireExtinguisher.MountPointPropertyName );

            CswNbtView ParentView = _CswNbtSchemaModTrnsctn.makeView();
            ParentView.ViewName = "Physical Inspection Schedule ParentView";
            CswNbtViewRelationship ParentRelationship = ParentView.AddViewRelationship( PhysicalInspectionScheduleNT, true );
            CswNbtViewRelationship LocationGroupChild = ParentView.AddViewRelationship( ParentRelationship, CswNbtViewRelationship.PropOwnerType.First, OwnerNTP, true );
            CswNbtViewRelationship LocationChild = ParentView.AddViewRelationship( LocationGroupChild, CswNbtViewRelationship.PropOwnerType.Second, LocationGroupNTP, true );
            CswNbtViewRelationship MountPointChild = ParentView.AddViewRelationship( LocationChild, CswNbtViewRelationship.PropOwnerType.Second, LocationOCP, true );
            CswNbtViewRelationship FireExtinguisherChild = ParentView.AddViewRelationship( MountPointChild, CswNbtViewRelationship.PropOwnerType.Second, LocationOCP, true );
            ParentView.save();

            //  Ask Steve re: "The property or indexer 'ChemSW.Nbt.PropTypes.CswNbtNodePropViewReference.ViewId' cannot be used in this context 
            //                 because the set accessor is inaccessible	C:\Vault\Dn\Nbt\Nbt\NbtLogic\csw\Schema\CswUpdateSchemaTo01H03.cs"
            //Generator ParentView = Generator NT > Location Group NT > Locations OC > Mount Point NT > Fire Extinguisher NT
            //CswNbtMetaDataNodeTypeProp ParentViewNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );
            //ParentViewNTP.DefaultValue.AsViewReference.ViewId = ParentView.ViewId;

            //FE Route NT
            CswNbtMetaDataObjectClass InspectionRouteOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );
            CswNbtMetaDataNodeType PhysicalInspectionRouteNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InspectionRouteOC.ObjectClassId, "Physical Inspection Route", "Fire Extinguisher" );
            PhysicalInspectionRouteNT.IconFileName = "arrows.gif";

            //FE Physical Inspection NT
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataNodeType PhysicalInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Physical Inspection" );
            if ( null != PhysicalInspectionNT )
            {
                PhysicalInspectionNT.NodeTypeName = "Physical Inspection";
                PhysicalInspectionNT.Category = "Fire Extinguisher";
            }
            else
            {
                PhysicalInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InspectionDesignOC.ObjectClassId, "Physical Inspection", "Fire Extinguisher" );
            }
            PhysicalInspectionNT.IconFileName = "test.gif";

            //Generator Target NT is Inspection
            CswNbtMetaDataNodeTypeProp GeneratorTargetTypeNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
            GeneratorTargetTypeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionNT.NodeTypeId, string.Empty, Int32.MinValue );

            //Inspection has MP Parent/Owner
            CswNbtMetaDataNodeTypeProp InspectionParentNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.OwnerPropertyName );
            InspectionParentNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), MountPointNT.NodeTypeId, string.Empty, Int32.MinValue );

            //Inspection has FE Generator
            CswNbtMetaDataNodeTypeProp InspectionGeneratorNTP = PhysicalInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            InspectionGeneratorNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionScheduleNT.NodeTypeId, string.Empty, Int32.MinValue );
            
            //Inspection has FE Route
            CswNbtMetaDataNodeTypeProp RouteNTP = PhysicalInspectionScheduleNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.RoutePropertyName );
            RouteNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), PhysicalInspectionRouteNT.NodeTypeId, string.Empty, Int32.MinValue );

            //Grant admin user FE NT permissions
            CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if ( RoleNode != null )
            {
                CswNbtNodePropLogicalSet Permissions = ( (CswNbtObjClassRole)CswNbtNodeCaster.AsRole( RoleNode ) ).NodeTypePermissions;

                Permissions.SetValue( NodeTypePermission.Create.ToString(), MountPointNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), MountPointNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), MountPointNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), MountPointNT.NodeTypeId.ToString(), true );

                Permissions.SetValue( NodeTypePermission.Create.ToString(), LocationGroupNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), LocationGroupNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), LocationGroupNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), LocationGroupNT.NodeTypeId.ToString(), true );

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
            }

            //_CswNbtSchemaModTrnsctn.MetaData.refreshAll();
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
    
            //BZ 10408
            CswNbtNode AdminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if ( AdminRoleNode != null )
            {
                CswNbtView LocationGroupView = _CswNbtSchemaModTrnsctn.makeView();
                LocationGroupView.makeNew( "Location Groups", NbtViewVisibility.Role, AdminRoleNode.NodeId, null, null );
                LocationGroupView.Category = "System";
                LocationGroupView.AddViewRelationship( LocationGroupNT, false );
                LocationGroupView.save();
            }
        }//Update()

    }//class CswUpdateSchemaTo01H03

}//namespace ChemSW.Nbt.Schema


