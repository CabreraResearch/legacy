﻿using System;
using System.Data;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29833
    /// </summary>
    public class CswUpdateMetaData_02K_Case29311 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override void update()
        {
            _metaDataListFieldType();
            _listText();
            _designObjectClasses();
            _sequenceOC();
            _defaultViewButton();

        } // update()

        private void _listText()
        {
            // Add 'Text' subfield for Lists
            CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateLists_jnp", "jct_nodes_props" );
            string WhereClause = @"where jctnodepropid in (select j.jctnodepropid
                                                             from jct_nodes_props j
                                                             join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                                             join field_types f on p.fieldtypeid = f.fieldtypeid
                                                            where f.fieldtype = 'List' and j.field2 is null)";
            DataTable JctTable = JctUpdate.getTable( WhereClause );
            foreach( DataRow JctRow in JctTable.Rows )
            {
                JctRow["field2"] = JctRow["field1"];
            }
            JctUpdate.update( JctTable );
        }

        private void _designObjectClasses()
        {
            if( null == _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass ) )
            {
                CswNbtMetaDataObjectClass NodeTypeOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass, "wrench.png", true );
                CswNbtMetaDataObjectClass PropOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass, "wrench.png", true );
                CswNbtMetaDataObjectClass TabOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass, "wrench.png", true );

                // DesignNodeType
                {
                    CswNbtMetaDataObjectClassProp AuditLevelOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.AuditLevel,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumAuditLevel.NoAudit.ToString(),
                                    CswEnumAuditLevel.PlainAudit.ToString()
                                }.ToString(),
                        IsRequired = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.Category,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
                    CswNbtMetaDataObjectClassProp DeferSearchToOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.DeferSearchTo,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsRequired = false,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = PropOC.ObjectClassId
                    } );
                    CswNbtMetaDataObjectClassProp SearchableOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.Searchable,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp HasLabelOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.HasLabel,
                        FieldType = CswEnumNbtFieldType.Logical,
                        ServerManaged = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.IconFileName,
                        FieldType = CswEnumNbtFieldType.ImageList,
                        Extended = false.ToString(),
                        TextAreaRows = 16,
                        TextAreaColumns = 16,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp LockedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.Locked,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp EnabledOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.Enabled,
                        FieldType = CswEnumNbtFieldType.Logical,
                        ServerManaged = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.NameTemplateText,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.NameTemplateValue,
                        FieldType = CswEnumNbtFieldType.Text,
                        ServerManaged = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.NameTemplateAdd,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = PropOC.ObjectClassId
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName,
                        FieldType = CswEnumNbtFieldType.Text,
                        IsRequired = true
                    } );
                    //_CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    //{
                    //    PropName = CswNbtObjClassDesignNodeType.PropertyName.ObjectClassName,
                    //    FieldType = CswEnumNbtFieldType.Text,
                    //    ServerManaged = true
                    //} );
                    CswNbtMetaDataObjectClassProp ObjectClassValueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.ObjectClass,
                        FieldType = CswEnumNbtFieldType.List,
                        ReadOnly = true,
                        IsRequired = true
                    } );

                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AuditLevelOCP, CswEnumAuditLevel.NoAudit.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( SearchableOCP, CswConvert.TristateToDbVal( CswEnumTristate.True ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( HasLabelOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( LockedOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( EnabledOCP, CswConvert.TristateToDbVal( CswEnumTristate.True ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ObjectClassValueOCP, CswConvert.ToDbVal( _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswEnumNbtObjectClass.GenericClass ) ), CswNbtFieldTypeRuleList.SubFieldName.Value );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ObjectClassValueOCP, CswConvert.ToDbVal( CswEnumNbtObjectClass.GenericClass.ToString() ), CswNbtFieldTypeRuleList.SubFieldName.Text );
                }

                // DesignNodeTypeProp
                {
                    CswNbtMetaDataObjectClassProp AuditLevelOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.AuditLevel,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumAuditLevel.NoAudit.ToString(),
                                    CswEnumAuditLevel.PlainAudit.ToString()
                                }.ToString(),
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp CompoundUniqueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.CompoundUnique,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp DisplayConditionFilterOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionFilterMode,
                        FieldType = CswEnumNbtFieldType.List
                    } );
                    CswNbtMetaDataObjectClassProp DisplayConditionPropertyOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionProperty,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = PropOC.ObjectClassId
                    } );
                    CswNbtMetaDataObjectClassProp DisplayConditionSubfieldOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionSubfield,
                        FieldType = CswEnumNbtFieldType.List
                    } );
                    CswNbtMetaDataObjectClassProp DisplayConditionValueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionValue,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType,
                        FieldType = CswEnumNbtFieldType.List,
                        ReadOnly = true,
                        IsRequired = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.HelpText,
                        FieldType = CswEnumNbtFieldType.Memo
                    } );
                    CswNbtMetaDataObjectClassProp NodeTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = NodeTypeOC.ObjectClassId,
                        ReadOnly = true,
                        IsRequired = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.ObjectClassPropName,
                        FieldType = CswEnumNbtFieldType.List,
                        ServerManaged = true
                    } );
                    CswNbtMetaDataObjectClassProp PropNameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
                    CswNbtMetaDataObjectClassProp ReadOnlyOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.ReadOnly,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp RequiredOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.Required,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp ServerManagedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.ServerManaged,
                        FieldType = CswEnumNbtFieldType.Logical,
                        ServerManaged = true
                    } );
                    CswNbtMetaDataObjectClassProp UniqueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.Unique,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp UseNumberingOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.UseNumbering,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    CswNbtMetaDataObjectClassProp QuestionNoOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.QuestionNo,
                        FieldType = CswEnumNbtFieldType.Number,
                        ServerManaged = true
                    } );
                    CswNbtMetaDataObjectClassProp SubQuestionNoOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.SubQuestionNo,
                        FieldType = CswEnumNbtFieldType.Number,
                        ServerManaged = true
                    } );


                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AuditLevelOCP, CswEnumAuditLevel.NoAudit.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CompoundUniqueOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( UseNumberingOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ReadOnlyOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( RequiredOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ServerManagedOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( UniqueOCP, CswConvert.TristateToDbVal( CswEnumTristate.False ) );

                    //// Display condition view includes all properties on the same nodetype
                    //CswNbtView DispCondView = _CswNbtSchemaModTrnsctn.makeView();
                    //CswNbtViewRelationship PropRel1 = DispCondView.AddViewRelationship( PropOC, false );
                    //CswNbtViewRelationship TypeRel2 = DispCondView.AddViewRelationship( PropRel1, CswEnumNbtViewPropOwnerType.First, NodeTypeOCP, false );
                    //CswNbtViewRelationship PropRel3 = DispCondView.AddViewRelationship( TypeRel2, CswEnumNbtViewPropOwnerType.Second, NodeTypeOCP, false );
                    //_CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionPropertyOCP, CswEnumNbtObjectClassPropAttributes.viewxml, DispCondView.ToXml().InnerXml );

                    // Display condition filters rely on a property being selected
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionSubfieldOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, DisplayConditionPropertyOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionFilterOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, DisplayConditionPropertyOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionValueOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, DisplayConditionPropertyOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionSubfieldOCP, CswEnumNbtObjectClassPropAttributes.filtersubfield, CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionFilterOCP, CswEnumNbtObjectClassPropAttributes.filtersubfield, CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionValueOCP, CswEnumNbtObjectClassPropAttributes.filtersubfield, CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionSubfieldOCP, CswEnumNbtObjectClassPropAttributes.filtermode, CswEnumNbtFilterMode.NotNull.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionFilterOCP, CswEnumNbtObjectClassPropAttributes.filtermode, CswEnumNbtFilterMode.NotNull.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionValueOCP, CswEnumNbtObjectClassPropAttributes.filtermode, CswEnumNbtFilterMode.NotNull.ToString() );

                    // QuestionNo and SubQuestionNo depend on UseNumbering
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, UseNumberingOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filtersubfield, CswNbtFieldTypeRuleLogical.SubFieldName.Checked.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filtermode, CswEnumNbtFilterMode.Equals.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filtervalue, CswEnumTristate.True.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SubQuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, UseNumberingOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SubQuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filtersubfield, CswNbtFieldTypeRuleLogical.SubFieldName.Checked.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SubQuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filtermode, CswEnumNbtFilterMode.Equals.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SubQuestionNoOCP, CswEnumNbtObjectClassPropAttributes.filtervalue, CswEnumTristate.True.ToString() );

                }

                // DesignNodeTypeTab
                {
                    CswNbtMetaDataObjectClassProp IncludeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.IncludeInReport,
                        FieldType = CswEnumNbtFieldType.Logical,
                        IsRequired = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = NodeTypeOC.ObjectClassId,
                        ReadOnly = true,
                        IsRequired = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.Order,
                        FieldType = CswEnumNbtFieldType.Number
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName,
                        FieldType = CswEnumNbtFieldType.Text,
                        IsRequired = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                    {
                        PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.ServerManaged,
                        FieldType = CswEnumNbtFieldType.Logical,
                        ServerManaged = true
                    } );

                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IncludeOCP, CswConvert.TristateToDbVal( CswEnumTristate.True ) );
                }
            }
        } // _designObjectClasses()

        private void _metaDataListFieldType()
        {
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldTypeDeprecated( CswEnumNbtFieldType.MetaDataList, CswEnumNbtFieldTypeDataType.INTEGER );
        }

        private void _sequenceOC()
        {
            if( null == _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass ) )
            {
                CswNbtMetaDataObjectClass SequenceOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DesignSequenceClass, "wrench.png", true );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( SequenceOC )
                    {
                        PropName = CswNbtObjClassDesignSequence.PropertyName.Name,
                        FieldType = CswEnumNbtFieldType.Text,
                        IsRequired = true
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( SequenceOC )
                    {
                        PropName = CswNbtObjClassDesignSequence.PropertyName.NextValue,
                        FieldType = CswEnumNbtFieldType.Text,
                        ServerManaged = true
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( SequenceOC )
                    {
                        PropName = CswNbtObjClassDesignSequence.PropertyName.Pad,
                        FieldType = CswEnumNbtFieldType.Number,
                        IsRequired = true,
                        NumberMinValue = 0
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( SequenceOC )
                    {
                        PropName = CswNbtObjClassDesignSequence.PropertyName.Post,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( SequenceOC )
                    {
                        PropName = CswNbtObjClassDesignSequence.PropertyName.Pre,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
            }
        } // _sequenceOC()

        private void _defaultViewButton()
        {
            CswNbtMetaDataObjectClass DesignNodeTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass );
            if( null != DesignNodeTypeOC )
            {
                CswNbtMetaDataObjectClassProp MakeDefaultViewOCP = DesignNodeTypeOC.getObjectClassProp( CswNbtObjClassDesignNodeType.PropertyName.ViewNodesButton );
                if( null == MakeDefaultViewOCP )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DesignNodeTypeOC )
                    {
                        PropName = CswNbtObjClassDesignNodeType.PropertyName.ViewNodesButton,
                        FieldType = CswEnumNbtFieldType.Button
                    } );
                }
            }
        } // _defaultViewButton()

    }//class CswUpdateMetaData_02K_Case29311

}//namespace ChemSW.Nbt.Schema