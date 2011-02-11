using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

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
            CswNbtMetaDataObjectClass InspectionTargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, InspectionTargetOC.ObjectClassId );
            CswNbtMetaDataObjectClass FireExtinguisherOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.FireExtinguisherClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, FireExtinguisherOC.ObjectClassId );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass InspectionTargetGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            
            // Case 20062
            Int32 FEImportActionid = _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Import_Fire_Extinguisher_Data, true, "Act_ImportFireExtinguisher.aspx", "System" );

            // Case 20033
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( FEModuleId, FEImportActionid );

            CswTableSelect ActionsTable = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "actionid", "actions" );
            DataTable ActionsDT = ActionsTable.getTable( " where lower(actionname) like '%inspection%' " );
            Int32 Actionid;
            for( Int32 i = 0; i < ActionsDT.Rows.Count; i++ )
            {
                Actionid = CswConvert.ToInt32( ActionsDT.Rows[i]["actionid"].ToString() );
                if ( Int32.MinValue != Actionid )
                    _CswNbtSchemaModTrnsctn.createModuleActionJunction(FEModuleId, Actionid);
                Actionid = Int32.MinValue;
            }

            //DT
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-03_OCP_Update", "object_class_props" );
            DataTable NewOCPTable = OCPUpdate.getEmptyTable();

            //Inspection Target (MP) OCPs
            string Status = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected );
            Status += "," + CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.OK );
            Status += "," + CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.OOC );

            //MP: Last Inspection Date
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, InspectionTargetOC, CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Date,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );
            //MP: Status
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, InspectionTargetOC, CswNbtObjClassInspectionTarget.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, true, false, string.Empty, Int32.MinValue, false, false, false, true, Status,
                                                           Int32.MinValue, Int32.MinValue );
            //MP: Location
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, InspectionTargetOC, CswNbtObjClassInspectionTarget.LocationPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Location,
                                                           false, false, true, "ObjectClassId", LocationOC.ObjectClassId, true, false, false, false, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );

            //MP: Inspection Target Group
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, InspectionTargetOC, CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false, false, true, "ObjectClassId", InspectionTargetGroupOC.ObjectClassId, true, false, false, false, string.Empty,
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
            //FE: Inspection Target
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC, CswNbtObjClassFireExtinguisher.InspectionTargetPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false, false, true, "ObjectClassId", InspectionTargetOC.ObjectClassId, false, false, false, false, string.Empty,
                                                           Int32.MinValue, Int32.MinValue );

            // Case 20058
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, InspectionTargetOC.ObjectClassId, "Description", CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC.ObjectClassId, "Description", CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, InspectionTargetOC.ObjectClassId, "Type", CswNbtMetaDataFieldType.NbtFieldType.List, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( NewOCPTable, FireExtinguisherOC.ObjectClassId, "Type", CswNbtMetaDataFieldType.NbtFieldType.List, Int32.MinValue, Int32.MinValue );

            // Last DT Op
            OCPUpdate.update( NewOCPTable );
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            // Default values
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( InspectionTargetOC.getObjectClassProp( CswNbtObjClassInspectionTarget.StatusPropertyName ),
                                                                 CswNbtSubField.SubFieldName.Value,
                                                                 "Not Inspected" );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.StatusPropertyName ),
                                                                 CswNbtSubField.SubFieldName.Value,
                                                                 "Not Inspected" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.InspectionTargetPropertyName ), CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.TypePropertyName ), CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InspectionTargetOC.getObjectClassProp( CswNbtObjClassInspectionTarget.TypePropertyName ), CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FireExtinguisherOC.getObjectClassProp( CswNbtObjClassFireExtinguisher.DescriptionPropertyName ), CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InspectionTargetOC.getObjectClassProp( CswNbtObjClassInspectionTarget.DescriptionPropertyName ), CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

        }//Update()

    }//class CswUpdateSchemaTo01H03

}//namespace ChemSW.Nbt.Schema


