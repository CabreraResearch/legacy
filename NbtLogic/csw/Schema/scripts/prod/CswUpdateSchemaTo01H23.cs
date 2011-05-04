using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-23
    /// </summary>
    public class CswUpdateSchemaTo01H23 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 


        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 23 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H23( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            //CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

            CswNbtMetaDataNodeType PhysInspNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection ) );
            //CswNbtMetaDataNodeTypeProp PhysInsp_TargetNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInsp_GeneratorNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInsp_DueDateNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInsp_MountPointNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.OwnerPropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInsp_RouteNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( "Route" );
            //CswNbtMetaDataNodeTypeProp PhysInsp_RouteOrderNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( "Route Order" );
            //CswNbtMetaDataNodeTypeProp PhysInsp_BarcodeNTP = PhysInspNT.getNodeTypeProp( "Barcode" ); // propref
            //CswNbtMetaDataNodeTypeProp PhysInsp_FinishedNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.FinishedPropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInsp_LocationNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.LocationPropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInsp_NameNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInsp_StatusNTP = PhysInspNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );

            //CswNbtMetaDataNodeType PhysInspSchedNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection_Schedule ) );
            //CswNbtMetaDataNodeTypeProp PhysInspSched_OwnerNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInspSched_TargetTypeNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInspSched_ParentTypeNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
            //CswNbtMetaDataNodeTypeProp PhysInspSched_ParentViewNTP = PhysInspSchedNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );

            //CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ) );
            //CswNbtMetaDataNodeTypeProp MountPoint_BarcodeNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
            //CswNbtMetaDataNodeTypeProp MountPoint_DescriptionNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
            //CswNbtMetaDataNodeTypeProp MountPoint_MountPointGroupNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            //CswNbtMetaDataNodeTypeProp MountPoint_HydrostaticInspectionNTP = MountPointNT.getNodeTypeProp( "Hydrostatic Inspection" );
            //CswNbtMetaDataNodeTypeProp MountPoint_LocationNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
            //CswNbtMetaDataNodeTypeProp MountPoint_LastInspDateNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
            //CswNbtMetaDataNodeTypeProp MountPoint_StatusNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );

            CswNbtMetaDataNodeType MountPointGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inspection Group" );
            CswNbtMetaDataNodeTypeProp MountPointGroup_MountPointLocationsNTP = MountPointGroupNT.getNodeTypeProp( "Mount Point Locations" );

            //CswNbtMetaDataNodeType RouteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection_Route ) );


            // case 21093

            CswNbtView MPBLView = _CswNbtSchemaModTrnsctn.restoreView( "Mount Points by Location" );
            if( MPBLView != null )
            {
                MPBLView.ViewName = "Inspection Points by Location";
                MPBLView.save();
            }

            foreach( CswNbtView AllMPsView in _CswNbtSchemaModTrnsctn.restoreViews( "All Mount Points" ) )
            {
                AllMPsView.ViewName = "All FE Inspection Points";
                AllMPsView.save();
            }

            if( MountPointGroup_MountPointLocationsNTP != null )
                MountPointGroup_MountPointLocationsNTP.PropName = "FE Inspection Point Locations";

            CswNbtMetaDataNodeTypeProp PhysInsp_Question1NTP = PhysInspNT.getNodeTypeProp( "Mount Point is Located in the Designated Place?" );
            if( PhysInsp_Question1NTP != null )
                PhysInsp_Question1NTP.PropName = "Inspection Point is Located in the Designated Place?";

            CswNbtMetaDataNodeTypeProp PhysInsp_Question2NTP = PhysInspNT.getNodeTypeProp( "Mount Point is Free of Obstructions?" );
            if( PhysInsp_Question2NTP != null )
                PhysInsp_Question2NTP.PropName = "Inspection Point is Free of Obstructions?";



        } // update()

    }//class CswUpdateSchemaTo01H23

}//namespace ChemSW.Nbt.Schema

