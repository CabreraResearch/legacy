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
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-12
    /// </summary>
    public class CswUpdateSchemaTo01H12 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 12 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H12( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // Case 20429
            CswNbtMetaDataNodeType MountPointGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point_Group ) );
            MountPointGroupNT.IconFileName = "ball_blueS.gif";

            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ) );
            MountPointNT.IconFileName = "mountpoint.png";

            CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Floor ) );
            FloorNT.IconFileName = "buildings.png";

            // Case 20025
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp StatusOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOCP, CswNbtSubField.SubFieldName.Value, CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Pending ) );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, CswConvert.ToDbVal( true ) );

            // Case 20437
            CswNbtMetaDataNodeTypeTab InspectionTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( MountPointNT, "Inspections", 2 );
            CswNbtMetaDataNodeTypeProp PhysicalInspectNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MountPointNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Physical Inspections", InspectionTab.TabId );

            CswNbtView PhysicalInspectionVP = _CswNbtSchemaModTrnsctn.restoreView( PhysicalInspectNTP.ViewId );
            PhysicalInspectionVP.makeNew( "Physical Inspections", NbtViewVisibility.Property, null, null, null );
            CswNbtViewRelationship MountPointR = PhysicalInspectionVP.AddViewRelationship( MountPointNT, false );
            CswNbtMetaDataNodeType PhysicalInspectionsNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection ) );
            CswNbtMetaDataNodeTypeProp PITargetNTP = PhysicalInspectionsNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            CswNbtViewRelationship InspectionsR = PhysicalInspectionVP.AddViewRelationship( MountPointR, CswNbtViewRelationship.PropOwnerType.Second, PITargetNTP, false );
            CswNbtViewProperty StatusVP = PhysicalInspectionVP.AddViewProperty( InspectionsR, PhysicalInspectionsNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName ) );
            CswNbtViewProperty DueDateVP = PhysicalInspectionVP.AddViewProperty( InspectionsR, PhysicalInspectionsNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName ) );
            CswNbtViewProperty TargetVP = PhysicalInspectionVP.AddViewProperty( InspectionsR, PhysicalInspectionsNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName ) );
            PhysicalInspectionVP.save();

            // Case 20506
            Int32 SetupTabId;
            CswNbtMetaDataNodeTypeTab SetupTab = PhysicalInspectionsNT.getNodeTypeTab( "Setup" );
            if( null != SetupTab )
                SetupTabId = SetupTab.TabId;
            else
                SetupTabId = PhysicalInspectionsNT.getFirstNodeTypeTab().TabId;

            CswNbtMetaDataNodeTypeProp MPBarcodeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( PhysicalInspectionsNT, 
                                                                                                    CswNbtMetaDataFieldType.NbtFieldType.PropertyReference, 
                                                                                                    "Barcode",
                                                                                                    SetupTabId );
            CswNbtMetaDataNodeTypeProp TargetNTP = PhysicalInspectionsNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            CswNbtMetaDataNodeTypeProp BarcodeNTP = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
            MPBarcodeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), TargetNTP.PropId, CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString(), BarcodeNTP.PropId );
            MPBarcodeNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), TargetNTP.PropId, CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString(), BarcodeNTP.PropId );

            // Case 20515
            CswNbtMetaDataObjectClassProp CancelledOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.CancelledPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CancelledOCP, CswNbtSubField.SubFieldName.Checked, Tristate.False.ToString() );
            CswNbtMetaDataObjectClassProp FinishedOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.FinishedPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FinishedOCP, CswNbtSubField.SubFieldName.Checked, Tristate.False.ToString() );

            CswNbtMetaDataNodeTypeProp NameNTP = PhysicalInspectionsNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.NamePropertyName );
            PhysicalInspectionsNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( NameNTP.PropName );

            // Case 20093
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp LocNameOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.NamePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocNameOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

        } // update()

    }//class CswUpdateSchemaTo01H12

}//namespace ChemSW.Nbt.Schema


