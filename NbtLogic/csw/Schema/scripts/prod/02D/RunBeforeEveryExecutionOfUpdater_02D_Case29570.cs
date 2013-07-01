using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case29570 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 29570";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29570; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            _createInvGrpPermApplyToAllOCPs();
            _createPermissionObjClasses();
            _createPermissionPropertySet();
        }

        private void _createInvGrpPermApplyToAllOCPs()
        {
            CswNbtMetaDataObjectClass InvGrpPermOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );
            CswNbtMetaDataObjectClassProp ApplyToAllWorkUnitsOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( InvGrpPermOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.ApplyToAllWorkUnits,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ApplyToAllWorkUnitsOCP, CswEnumTristate.False );
            CswNbtMetaDataObjectClassProp ApplyToAllRolesOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( InvGrpPermOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.ApplyToAllRoles,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ApplyToAllRolesOCP, CswEnumTristate.False );
            CswNbtMetaDataObjectClassProp WorkUnitOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.WorkUnit );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( WorkUnitOCP, CswEnumNbtObjectClassPropAttributes.isrequired, false );
            WorkUnitOCP.setFilter( ApplyToAllWorkUnitsOCP, ApplyToAllWorkUnitsOCP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.False );
            CswNbtMetaDataObjectClassProp RoleOCP = InvGrpPermOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Role );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RoleOCP, CswEnumNbtObjectClassPropAttributes.isrequired, false );
            RoleOCP.setFilter( ApplyToAllRolesOCP, ApplyToAllRolesOCP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.False );

            CswNbtMetaDataObjectClassProp PermissionGroupOCP = InvGrpPermOC.getObjectClassProp( "Inventory Group" );
            if( null != PermissionGroupOCP )
            {
                CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ChangeInvGrpPermName", "object_class_props" );
                DataTable OCPropTable = OCPUpdate.getTable( "where objectclasspropid = " + PermissionGroupOCP.PropId );
                if( OCPropTable.Rows.Count > 0 )
                {
                    OCPropTable.Rows[0]["propname"] = CswNbtPropertySetPermission.PropertyName.PermissionGroup;
                    OCPUpdate.update( OCPropTable );
                }
            }
        }

        #region ObjectClasses

        private class PermissionSetAttributes
        {
            public CswEnumNbtObjectClass TargetClassName;
            public CswEnumNbtObjectClass GroupClassName;
            public CswEnumNbtObjectClass PermissionClassName;
            public String GroupTargetPropName;
            public String TargetGroupPropName;
            public List<String> TargetsGridProperties;
        }

        private void _createPermissionObjClasses()
        {
            PermissionSetAttributes ReportPermissionSet = new PermissionSetAttributes
            {
                TargetClassName = CswEnumNbtObjectClass.ReportClass,
                GroupClassName = CswEnumNbtObjectClass.ReportGroupClass,
                PermissionClassName = CswEnumNbtObjectClass.ReportGroupPermissionClass,
                GroupTargetPropName = CswNbtObjClassReportGroup.PropertyName.Reports,
                TargetGroupPropName = CswNbtObjClassReport.PropertyName.ReportGroup,
                TargetsGridProperties = new List<string>
                                    {
                                        CswNbtObjClassReport.PropertyName.ReportName,
                                        CswNbtObjClassReport.PropertyName.Category,
                                        CswNbtObjClassReport.PropertyName.Instructions
                                    }
            };
            PermissionSetAttributes MailReportPermissionSet = new PermissionSetAttributes
            {
                TargetClassName = CswEnumNbtObjectClass.MailReportClass,
                GroupClassName = CswEnumNbtObjectClass.MailReportGroupClass,
                PermissionClassName = CswEnumNbtObjectClass.MailReportGroupPermissionClass,
                GroupTargetPropName = CswNbtObjClassMailReportGroup.PropertyName.MailReports,
                TargetGroupPropName = CswNbtObjClassMailReport.PropertyName.MailReportGroup,
                TargetsGridProperties = new List<string>
                                    {
                                        CswNbtObjClassMailReport.PropertyName.Name,
                                        CswNbtObjClassMailReport.PropertyName.Type,
                                        CswNbtObjClassMailReport.PropertyName.NextDueDate
                                    }
            };
            CswNbtMetaDataObjectClass ReportPermissionGroupOC = _createPermissionGroupClass( ReportPermissionSet );
            CswNbtMetaDataObjectClass MailReportPermissionGroupOC = _createPermissionGroupClass( MailReportPermissionSet );
            _createPermissionClass( ReportPermissionSet );
            _createPermissionClass( MailReportPermissionSet );
            _createGroupPermissionsGridProp( ReportPermissionGroupOC, ReportPermissionSet );
            _createGroupPermissionsGridProp( MailReportPermissionGroupOC, MailReportPermissionSet );
        }

        private void _createPermissionClass( PermissionSetAttributes PermSet )
        {
            CswNbtMetaDataObjectClass PermissionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PermSet.PermissionClassName );
            if( null == PermissionOC )
            {
                PermissionOC = _CswNbtSchemaModTrnsctn.createObjectClass( PermSet.PermissionClassName, "doc.png", false );

                CswNbtMetaDataObjectClass GroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PermSet.GroupClassName );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.PermissionGroup,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsRequired = true,
                    SetValOnAdd = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = GroupOC.ObjectClassId
                } );
                CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
                CswNbtMetaDataObjectClassProp WorkUnitOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.WorkUnit,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    SetValOnAdd = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = WorkUnitOC.ObjectClassId
                } );
                CswNbtMetaDataObjectClassProp ApplyToAllWorkUnitsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.ApplyToAllWorkUnits,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ApplyToAllWorkUnitsOCP, CswEnumTristate.False );
                WorkUnitOCP.setFilter( ApplyToAllWorkUnitsOCP, ApplyToAllWorkUnitsOCP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.False );
                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                CswNbtMetaDataObjectClassProp RoleOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.Role,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    SetValOnAdd = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = RoleOC.ObjectClassId
                } );
                CswNbtMetaDataObjectClassProp ApplyToAllRolesOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.ApplyToAllRoles,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ApplyToAllRolesOCP, CswEnumTristate.False );
                RoleOCP.setFilter( ApplyToAllRolesOCP, ApplyToAllRolesOCP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswEnumTristate.False );
                CswNbtMetaDataObjectClassProp ViewOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.View,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ViewOCP, CswEnumTristate.False );
                CswNbtMetaDataObjectClassProp EditOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionOC )
                {
                    PropName = CswNbtPropertySetPermission.PropertyName.Edit,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( EditOCP, CswEnumTristate.False );
            }
        }

        private CswNbtMetaDataObjectClass _createPermissionGroupClass( PermissionSetAttributes PermSet )
        {
            CswNbtMetaDataObjectClass PermissionGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PermSet.GroupClassName );
            if( null == PermissionGroupOC )
            {
                PermissionGroupOC = _CswNbtSchemaModTrnsctn.createObjectClass( PermSet.GroupClassName, "smallicons.png", false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionGroupOC )
                {
                    PropName = CswNbtObjClassReportGroup.PropertyName.Name,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionGroupOC )
                {
                    PropName = CswNbtObjClassReportGroup.PropertyName.Description,
                    FieldType = CswEnumNbtFieldType.Memo,
                    SetValOnAdd = true
                } );

                CswNbtMetaDataObjectClassProp TargetsGridOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionGroupOC )
                {
                    PropName = PermSet.GroupTargetPropName,
                    FieldType = CswEnumNbtFieldType.Grid
                } );

                CswNbtMetaDataObjectClass TargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PermSet.TargetClassName );
                CswNbtMetaDataObjectClassProp GroupOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TargetOC )
                {
                    PropName = PermSet.TargetGroupPropName,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = PermissionGroupOC.ObjectClassId,
                    IsRequired = true
                } );
                CswNbtView TargetsView = _CswNbtSchemaModTrnsctn.makeView();
                TargetsView.ViewName = PermSet.GroupTargetPropName;
                TargetsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship GroupRel = TargetsView.AddViewRelationship( PermissionGroupOC, false );
                CswNbtViewRelationship TargetRel = TargetsView.AddViewRelationship( GroupRel, CswEnumNbtViewPropOwnerType.Second, GroupOCP, true );
                for( int i = 0; i < PermSet.TargetsGridProperties.Count; i++ )
                {
                    TargetsView.AddViewProperty( TargetRel, TargetOC.getObjectClassProp( PermSet.TargetsGridProperties[i] ), i + 1 );
                }
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetsGridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, TargetsView.ToString() );
            }
            return PermissionGroupOC;
        }

        private void _createGroupPermissionsGridProp( CswNbtMetaDataObjectClass PermissionGroupOC, PermissionSetAttributes PermSet )
        {
            CswNbtMetaDataObjectClassProp PermissionsGridOCP = PermissionGroupOC.getObjectClassProp( "Permissions" );
            if( null == PermissionsGridOCP )
            {
                PermissionsGridOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PermissionGroupOC )
                    {
                        PropName = CswNbtObjClassReportGroup.PropertyName.Permissions,
                        FieldType = CswEnumNbtFieldType.Grid
                    } );

                CswNbtMetaDataObjectClass PermissionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( PermSet.PermissionClassName );
                CswNbtMetaDataObjectClassProp PermGroupOCP = PermissionOC.getObjectClassProp( CswNbtPropertySetPermission.PropertyName.PermissionGroup );
                CswNbtView PermissionsView = _CswNbtSchemaModTrnsctn.makeView();
                PermissionsView.ViewName = PermSet.GroupTargetPropName;
                PermissionsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship RootRel = PermissionsView.AddViewRelationship( PermissionGroupOC, false );
                CswNbtViewRelationship PermRel = PermissionsView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, PermGroupOCP, true );
                PermissionsView.AddViewProperty( PermRel, PermissionOC.getObjectClassProp( CswNbtPropertySetPermission.PropertyName.PermissionGroup ), 1 );
                PermissionsView.AddViewProperty( PermRel, PermissionOC.getObjectClassProp( CswNbtPropertySetPermission.PropertyName.Role ), 2 );
                PermissionsView.AddViewProperty( PermRel, PermissionOC.getObjectClassProp( CswNbtPropertySetPermission.PropertyName.WorkUnit ), 3 );
                PermissionsView.AddViewProperty( PermRel, PermissionOC.getObjectClassProp( CswNbtPropertySetPermission.PropertyName.View ), 4 );
                PermissionsView.AddViewProperty( PermRel, PermissionOC.getObjectClassProp( CswNbtPropertySetPermission.PropertyName.Edit ), 5 );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PermissionsGridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, PermissionsView.ToString() );
            }
        }

        #endregion ObjectClasses

        #region PropertySet

        private void _createPermissionPropertySet()
        {
            CswNbtMetaDataPropertySet PermissionPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.PermissionSet );
            if( null == PermissionPS )
            {
                PermissionPS = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropertySet( CswEnumNbtPropertySetName.PermissionSet, "check.png" );

                CswTableUpdate JctPSOCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29570_jctpsoc_update", "jct_propertyset_objectclass" );
                DataTable JctPSOCTable = JctPSOCUpdate.getEmptyTable();
                _addObjClassToPropertySetPermission( JctPSOCTable, CswEnumNbtObjectClass.InventoryGroupPermissionClass, PermissionPS.PropertySetId );
                _addObjClassToPropertySetPermission( JctPSOCTable, CswEnumNbtObjectClass.ReportGroupPermissionClass, PermissionPS.PropertySetId );
                _addObjClassToPropertySetPermission( JctPSOCTable, CswEnumNbtObjectClass.MailReportGroupPermissionClass, PermissionPS.PropertySetId );
                JctPSOCUpdate.update( JctPSOCTable );

                CswTableUpdate JctPSOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29570_jctpsocp_update", "jct_propertyset_ocprop" );
                DataTable JctPSOCPTable = JctPSOCPUpdate.getEmptyTable();
                _addObjClassPropsToPropertySetPermission( JctPSOCPTable, CswEnumNbtObjectClass.InventoryGroupPermissionClass, PermissionPS.PropertySetId );
                _addObjClassPropsToPropertySetPermission( JctPSOCPTable, CswEnumNbtObjectClass.ReportGroupPermissionClass, PermissionPS.PropertySetId );
                _addObjClassPropsToPropertySetPermission( JctPSOCPTable, CswEnumNbtObjectClass.MailReportGroupPermissionClass, PermissionPS.PropertySetId );
                JctPSOCPUpdate.update( JctPSOCPTable );
            }
        }

        private void _addObjClassToPropertySetPermission( DataTable JctPSOCTable, string ObjClassName, int PropertySetId )
        {
            DataRow NewJctPSOCRow = JctPSOCTable.NewRow();
            NewJctPSOCRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( ObjClassName );
            NewJctPSOCRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
            JctPSOCTable.Rows.Add( NewJctPSOCRow );
        }

        private void _addObjClassPropsToPropertySetPermission( DataTable JctPSOCPTable, string ObjClassName, int PropertySetId )
        {
            CswNbtMetaDataObjectClass PermissionObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClassName );
            foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in PermissionObjectClass.getObjectClassProps() )
            {
                bool doInsert = (
                                    ObjectClassProp.PropName == CswNbtPropertySetPermission.PropertyName.PermissionGroup ||
                                    ObjectClassProp.PropName == "Inventory Group" || //Special Case
                                    ObjectClassProp.PropName == CswNbtPropertySetPermission.PropertyName.WorkUnit ||
                                    ObjectClassProp.PropName == CswNbtPropertySetPermission.PropertyName.ApplyToAllWorkUnits ||
                                    ObjectClassProp.PropName == CswNbtPropertySetPermission.PropertyName.Role ||
                                    ObjectClassProp.PropName == CswNbtPropertySetPermission.PropertyName.ApplyToAllRoles ||
                                    ObjectClassProp.PropName == CswNbtPropertySetPermission.PropertyName.View ||
                                    ObjectClassProp.PropName == CswNbtPropertySetPermission.PropertyName.Edit 
                                );

                if( doInsert )
                {
                    DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                    NewJctPSOCPRow["objectclasspropid"] = ObjectClassProp.PropId;
                    NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
                    JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
                }
            }
        }

        #endregion PropertySet

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case29570
}//namespace ChemSW.Nbt.Schema


