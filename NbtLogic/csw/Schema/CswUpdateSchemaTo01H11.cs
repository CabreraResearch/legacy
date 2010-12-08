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
    /// Updates the schema to version 01H-11
    /// </summary>
    public class CswUpdateSchemaTo01H11 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 11 ); } }
        public CswUpdateSchemaTo01H11( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
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
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOCP, "servermanaged", CswConvert.ToDbVal( true ) );

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

        } // update()

    }//class CswUpdateSchemaTo01H11

}//namespace ChemSW.Nbt.Schema


