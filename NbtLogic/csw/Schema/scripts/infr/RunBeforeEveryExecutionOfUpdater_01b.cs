using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01b : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region ROMEO

            // moved from CswUpdateSchemaCase24525 for case 27706
            #region ADD ARCHIVED PROP TO USER
            CswNbtMetaDataFieldType logicalFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Logical );
            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            if( null == userOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Archived ) )
            {
                CswNbtMetaDataObjectClassProp archivedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( userOC )
                {
                    PropName = CswNbtObjClassUser.PropertyName.Archived,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsFk = false,
                    IsRequired = true,
                    ValuePropType = logicalFT.FieldType,
                    ValuePropId = logicalFT.FieldTypeId,
                } );
                //set the default val to false - we don't want new users to be archived
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( archivedOCP, archivedOCP.getFieldTypeRule().SubFields.Default.Name, Tristate.False );
            }
            #endregion

            #endregion ROMEO


            #region SEBASTIAN

            // case 27703 - change containers dispose/dispense buttons to say "Dispose this Container" and "Dispense this Container"
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            CswNbtMetaDataObjectClassProp dispenseOCP = containerOC.getObjectClassProp( "Dispense" );
            if( null != dispenseOCP ) //have to null check because property might have already been updated
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( dispenseOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Dispense this Container" );
            }

            CswNbtMetaDataObjectClassProp disposeOCP = containerOC.getObjectClassProp( "Dispose" );
            if( null != disposeOCP ) //have to null check here because property might have been updated
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( disposeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Dispose this Container" );
            }

            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp ControlTypeOcp = PrintLabelOc.getObjectClassProp( "Control Type" );
            if( null != ControlTypeOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( ControlTypeOcp, DeleteNodeTypeProps: true );
            }

            //upgrade RequestItem Requestor prop from NTP to OCP
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            if( null == requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Requestor ) )
            {
                CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
                CswNbtMetaDataObjectClassProp requestorOCP = requestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
                CswNbtMetaDataObjectClassProp requestOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );

                CswNbtMetaDataObjectClassProp reqItemrequestorOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( requestItemOC )
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Requestor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.PropertyReference,
                    IsFk = true,
                    FkType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                    FkValue = requestOCP.PropId,
                    ValuePropType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                    ValuePropId = requestorOCP.PropId
                } );
            }


            #region case 27720

            // remove Notification nodes, nodetypes, and object class
            CswNbtMetaDataObjectClass NotificationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "NotificationClass" );
            if( null != NotificationOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( NotificationOC );
            }

            // add properties to mail reports
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp TypeOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.Type );
            if( null == MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.TargetType ) )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MailReportOC )
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect,
                    PropName = CswNbtObjClassMailReport.PropertyName.TargetType,
                    FilterPropId = TypeOCP.PropId,
                    Filter = CswNbtObjClassMailReport.TypeOptionView
                } );
            }
            if( null == MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.Event ) )
            {
                CswCommaDelimitedString Options = new CswCommaDelimitedString();
                foreach( CswNbtObjClassMailReport.EventOption EventOpt in CswNbtObjClassMailReport.EventOption._All )
                {
                    Options.Add( EventOpt.ToString() );
                }
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MailReportOC )
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    PropName = CswNbtObjClassMailReport.PropertyName.Event,
                    ListOptions = Options.ToString(),
                    FilterPropId = TypeOCP.PropId,
                    Filter = CswNbtObjClassMailReport.TypeOptionView
                } );
            }
            if( null == MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.NodesToReport ) )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MailReportOC )
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Memo,
                    PropName = CswNbtObjClassMailReport.PropertyName.NodesToReport
                } );
            }

            // Change "Report View" from ViewPickList to ViewReference
            
            // map jct_nodes_props records
            //   ViewReference: Name = field1, ViewId = field1_fk
            //   ViewPickList: Name = gestalt, ViewId = field1
            CswNbtMetaDataObjectClassProp ReportViewOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView);
            
            CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "27720_update_jnp", "jct_nodes_props" );
            DataTable JctTable = JctUpdate.getTable( "where nodetypepropid in (select nodetypepropid from nodetype_props where objectclasspropid = " + ReportViewOCP.ObjectClassPropId + ")" );
            foreach( DataRow JctRow in JctTable.Rows )
            {
                JctRow["field1_fk"] = JctRow["field1"];
                JctRow["field1"] = JctRow["gestalt"];
            }
            JctUpdate.update( JctTable );
            
            // update the field types
            CswNbtMetaDataFieldType ViewReferenceFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ViewReference );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReportViewOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, ViewReferenceFT.FieldTypeId );

            #endregion case 27720

            #endregion SEBASTIAN

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
            
            #region Also romeo (has to be last)
            foreach( CswNbtNode userNode in userOC.getNodes( false, false ) )
            {
                userNode.Properties[CswNbtObjClassUser.PropertyName.Archived].AsLogical.Checked = Tristate.False;
                userNode.postChanges( false );
            }
            #endregion Also romeo (has to be last)

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01b

}//namespace ChemSW.Nbt.Schema


