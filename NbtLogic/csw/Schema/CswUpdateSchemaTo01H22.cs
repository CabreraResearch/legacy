using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-22
    /// </summary>
    public class CswUpdateSchemaTo01H22 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 22 ); } }
        public CswUpdateSchemaTo01H22( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 21049

            CswNbtMetaDataNodeType PhysInspNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection ) );
            CswNbtMetaDataNodeTypeProp PhysInsp_TargetNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_GeneratorNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_RouteOrderNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.RouteOrderPropertyName );
            CswNbtMetaDataNodeTypeProp PhysInsp_DueDateNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );

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
            CswNbtMetaDataNodeType MountPointGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point_Group ) );

            // Rename properties            
            PhysInsp_TargetNTP.PropName = "FE Mount Point";
            PhysInsp_GeneratorNTP.PropName = "Schedule";
            PhysInspSched_OwnerNTP.PropName = "Inspection Group";
            PhysInspSched_TargetTypeNTP.PropName = "Inspection Type";
            PhysInspSched_ParentTypeNTP.PropName = "FE Mount Point Type";
            PhysInspSched_ParentViewNTP.PropName = "FE Mount Point View";

            // Add barcode to mount point name template
            MountPointNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( MountPoint_BarcodeNTP.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( MountPoint_DescriptionNTP.PropName );

            // Generator and Due Date should not be mobile search options
            PhysInsp_GeneratorNTP.MobileSearch = false;
            PhysInsp_DueDateNTP.MobileSearch = false;
            
            // Rename 'mount point group' to 'inspection group'
            MountPointGroupNT.NodeTypeName = "Inspection Group";
            MountPointGroupNT.getFirstNodeTypeTab().TabName = "Inspection Group";
            MountPoint_MountPointGroupNTP.PropName = "Inspection Group";

            // Rename 'mount point' to 'FE mount point'
            MountPointNT.NodeTypeName = "FE Mount Point";
            MountPointNT.getFirstNodeTypeTab().TabName = "FE Mount Point";

            // Make Inspection's Target Type editable
            PhysInspSched_TargetTypeNTP.ReadOnly = false;

            // Remove properties
            if( MountPoint_HydrostaticInspectionNTP != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( MountPoint_HydrostaticInspectionNTP );
            }

        } // update()

    }//class CswUpdateSchemaTo01H22

}//namespace ChemSW.Nbt.Schema


