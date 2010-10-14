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
    /// Updates the schema to version 01H-02
    /// </summary>
    public class CswUpdateSchemaTo01H02 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 02 ); } }
        public CswUpdateSchemaTo01H02( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // This script is reserved for object class changes 
            // which need to take place before any other changes can be made.

            // Because of changes in tables in previous script
            _CswNbtSchemaModTrnsctn.refreshDataDictionary();

            // BZ 10094
            // New Notification Object Class
            Int32 NotificationOCId = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass.ToString(), "bullhorn.gif", false, false );
            CswNbtMetaDataObjectClass NotificationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass );
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-02_OCP_Update", "object_class_props" );
            DataTable OCPTable = OCPUpdate.getEmptyTable();

            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, NotificationOC, CswNbtObjClassNotification.EventPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, false, false, string.Empty, Int32.MinValue, true, false, false, false,
                                                           CswNbtObjClassNotification.EventOption.Create.ToString() + CswNbtFieldTypeRuleList.delimiter.ToString() +
                                                           CswNbtObjClassNotification.EventOption.Edit.ToString() + CswNbtFieldTypeRuleList.delimiter.ToString() +
                                                           CswNbtObjClassNotification.EventOption.Delete.ToString(),
                                                           Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, NotificationOCId, CswNbtObjClassNotification.TargetTypePropertyName, CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, NotificationOCId, CswNbtObjClassNotification.PropertyPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, NotificationOCId, CswNbtObjClassNotification.ValuePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, NotificationOCId, CswNbtObjClassNotification.SubscribedUsersPropertyName, CswNbtMetaDataFieldType.NbtFieldType.UserSelect, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, NotificationOCId, CswNbtObjClassNotification.SubjectPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, NotificationOCId, CswNbtObjClassNotification.MessagePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Memo, Int32.MinValue, Int32.MinValue );

            // BZ 10454
            // Add Equipment Status as object class prop
            CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, EquipmentOC, CswNbtObjClassEquipment.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, false, false, string.Empty, Int32.MinValue, false, false, false, false,
                                                           CswNbtObjClassEquipment.StatusOptionToDisplayString( CswNbtObjClassEquipment.StatusOption.Available ) + CswNbtFieldTypeRuleList.delimiter.ToString() +
                                                           CswNbtObjClassEquipment.StatusOptionToDisplayString( CswNbtObjClassEquipment.StatusOption.In_Use ) + CswNbtFieldTypeRuleList.delimiter.ToString() +
                                                           CswNbtObjClassEquipment.StatusOptionToDisplayString( CswNbtObjClassEquipment.StatusOption.Retired ),
                                                           Int32.MinValue, Int32.MinValue );

            //BZ 10021: FE Object Classes
            Int32 MountPointClassObjectID = _CswNbtSchemaModTrnsctn.createObjectClass( "MountPointClass", "", false, false );
            Int32 FireExtinguisherClassObjectID = _CswNbtSchemaModTrnsctn.createObjectClass( "FireExtinguisherClass", "", false, false );

            // BZ 5073
            // Add entries for old existing default values

            CswNbtMetaDataObjectClass ProblemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );
            CswNbtMetaDataObjectClass CustomerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            // Since we can't use CswNbtNodePropWrappers, we have to use the field type rule to know what column to edit

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ProblemOC.getObjectClassProp( CswNbtObjClassProblem.ClosedPropertyName ),
                                                                             CswNbtSubField.SubFieldName.Checked,
                                                                             Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TaskOC.getObjectClassProp( CswNbtObjClassTask.IsFuturePropertyName ),
                                                                             CswNbtSubField.SubFieldName.Checked,
                                                                             Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TaskOC.getObjectClassProp( CswNbtObjClassTask.CompletedPropertyName ),
                                                                             CswNbtSubField.SubFieldName.Checked,
                                                                             Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CustomerOC.getObjectClassProp( CswNbtObjClassCustomer.DeactivatedPropertyName ),
                                                                             CswNbtSubField.SubFieldName.Checked,
                                                                             Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.IsFuturePropertyName ),
                                                                             CswNbtSubField.SubFieldName.Checked,
                                                                             Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.EnabledPropertyName ),
                                                                             CswNbtSubField.SubFieldName.Checked,
                                                                             Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.EnabledPropertyName ),
                                                                             CswNbtSubField.SubFieldName.Checked,
                                                                             Tristate.False );

            // BZ 10343
            DataRow ParentTypeDR = _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, GeneratorOC.ObjectClassId, CswNbtObjClassGenerator.ParentTypePropertyName, CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, GeneratorOC.ObjectClassId, CswNbtObjClassGenerator.ParentViewPropertyName, CswNbtMetaDataFieldType.NbtFieldType.ViewReference, Int32.MinValue, Int32.MinValue );
            ParentTypeDR["multi"] = CswConvert.ToDbVal(false);

            // BZ 10406: Add Grace Days to GeneratorClass
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, GeneratorOC.ObjectClassId, CswNbtObjClassGenerator.GraceDaysPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Number, Int32.MinValue, Int32.MinValue );
            
            // BZ 10406: Add Status, Finished, Cancelled and Cancel Reason to InspectionDesignClass
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, InspectionDesignOC, CswNbtObjClassInspectionDesign.StatusPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List,
                                                           false, false, false, string.Empty, Int32.MinValue, false, false, false, true, "Pending,Overdue,Action Required,Missed,Completed,Completed Late,Cancelled", Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, InspectionDesignOC.ObjectClassId, CswNbtObjClassInspectionDesign.FinishedPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Logical, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, InspectionDesignOC.ObjectClassId, CswNbtObjClassInspectionDesign.CancelledPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Logical, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, InspectionDesignOC.ObjectClassId, CswNbtObjClassInspectionDesign.CancelReasonPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Memo, Int32.MinValue, Int32.MinValue );            

            // BZ 10425 - Set warning days = 0 and readonly
            CswNbtMetaDataObjectClassProp MailReportWarningDaysOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.WarningDaysPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( MailReportWarningDaysOCP,
                                                                             CswNbtSubField.SubFieldName.Value,
                                                                             0 );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MailReportWarningDaysOCP, "readonly", true );


            // BZ 10425 - Remove 'Status' field from Mail Report (redundant with 'Run Status')
            CswNbtMetaDataObjectClassProp MailReportStatusOCP = MailReportOC.getObjectClassProp( "Status" );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( MailReportStatusOCP );

            OCPUpdate.update( OCPTable );

            // for synchronization
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();


        }//Update()

    }//class CswUpdateSchemaTo01H02

}//namespace ChemSW.Nbt.Schema


