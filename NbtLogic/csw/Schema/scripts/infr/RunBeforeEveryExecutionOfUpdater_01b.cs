using System;
using ChemSW.Nbt.csw.Dev;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;

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
                    if( EventOpt != CswNbtObjClassMailReport.EventOption.Unknown )
                    {
                        Options.Add( EventOpt.ToString() );
                    }
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
            CswNbtMetaDataObjectClassProp ReportViewOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView );
            if( ReportViewOCP.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.ViewPickList )
            {
                // map jct_nodes_props records
                //   ViewReference: Name = field1, ViewId = field1_fk
                //   ViewPickList: Name = gestalt, ViewId = field1
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
            }
            #endregion case 27720

            #endregion SEBASTIAN

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            #region Also romeo (has to be last)
            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

            foreach( CswNbtNode userNode in userOC.getNodes( false, false ) )
            {
                userNode.Properties[CswNbtObjClassUser.PropertyName.Archived].AsLogical.Checked = Tristate.False;
                userNode.postChanges( false );
            }
            #endregion Also romeo (has to be last)

            #region TITANIA

            #region Case 27869 - Method ObjectClass

            CswNbtMetaDataObjectClass MethodOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MethodClass );
            if( null == MethodOc )
            {
                //Create new ObjectClass
                MethodOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MethodClass, "barchart.png", true, false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MethodOc )
                    {
                        PropName = CswNbtObjClassMethod.PropertyName.MethodNo,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                        IsRequired = true,
                        IsUnique = true,
                        SetValOnAdd = true
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MethodOc )
                    {
                        PropName = CswNbtObjClassMethod.PropertyName.MethodDescription,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                        SetValOnAdd = true
                    } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, MethodOc.ObjectClassId );
            }

            #endregion Case 27869 - Method ObjectClass

            #region Case 27873 - Jurisdiction ObjectClass

            CswNbtMetaDataObjectClass JurisdictionOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.JurisdictionClass );
            if( null == JurisdictionOc )
            {
                //Create new ObjectClass
                JurisdictionOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.JurisdictionClass, "person.png", true, false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( JurisdictionOc )
                {
                    PropName = CswNbtObjClassJurisdiction.PropertyName.Name,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    IsRequired = true,
                    SetValOnAdd = true
                } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, JurisdictionOc.ObjectClassId );
            }

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            if( null != UserOc )
            {
                //Create new User Prop
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( UserOc )
                {
                    PropName = CswNbtObjClassUser.PropertyName.Jurisdiction,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = JurisdictionOc.ObjectClassId
                } );
            }

            #endregion Case 27873 - Jurisdiction ObjectClass

            #region Case 27870 - New InventoryGroup ObjClassProps

            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            if( null != InventoryGroupOc )
            {
                CswNbtMetaDataObjectClassProp CentralOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( InventoryGroupOc )
                {
                    PropName = CswNbtObjClassInventoryGroup.PropertyName.Central,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = true
                } );

                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CentralOCP, CentralOCP.getFieldTypeRule().SubFields.Default.Name, Tristate.False );
            #endregion
            
            #region Case 27865 part 1 - Enterprise Part (EP)

            CswNbtMetaDataObjectClass enterprisePartOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EnterprisePartClass );
            if( null == enterprisePartOC )
            {
                enterprisePartOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EnterprisePartClass, "gear.png", false, false );

                CswNbtMetaDataObjectClassProp gcasOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( enterprisePartOC )
                {
                    PropName = CswNbtObjClassEnterprisePart.PropertyName.GCAS,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    IsUnique = true
                } );

                CswNbtMetaDataObjectClassProp requestOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( enterprisePartOC )
                {
                    PropName = CswNbtObjClassEnterprisePart.PropertyName.Request,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button
                } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, enterprisePartOC.ObjectClassId );

            }

            #endregion

            #region Case 27865 part 2 - Manufactuerer Equivalent Part

            CswNbtMetaDataObjectClass manufactuerEquivalentPartOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ManufacturerEquivalentPartClass );
            if( null == manufactuerEquivalentPartOC )
            {
                manufactuerEquivalentPartOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ManufacturerEquivalentPartClass, "gearset.png", false, false );

                CswNbtMetaDataObjectClassProp epOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( manufactuerEquivalentPartOC )
                {
                    PropName = CswNbtObjClassManufacturerEquivalentPart.PropertyName.EnterprisePart,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = enterprisePartOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp materialOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( manufactuerEquivalentPartOC )
                {
                    PropName = CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = materialOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass );
                CswNbtMetaDataObjectClassProp manufacturerOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( manufactuerEquivalentPartOC )
                {
                    PropName = CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = vendorOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClassProp vendorTypeOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                CswNbtView manufacturerOCPView = _CswNbtSchemaModTrnsctn.makeNewView( "mepManufacturerOCP", NbtViewVisibility.Property );
                CswNbtViewRelationship parent = manufacturerOCPView.AddViewRelationship( vendorOC, true );
                manufacturerOCPView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: vendorTypeOCP,
                    Value: "Manufacturing",
                    SubFieldName: CswNbtSubField.SubFieldName.Value,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( manufacturerOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, manufacturerOCPView.ToString() );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, manufactuerEquivalentPartOC.ObjectClassId );
            }

            #endregion

            #region 27867 - Receipt Lot

            CswNbtMetaDataObjectClass receiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReceiptLotClass );
            if( null == receiptLotOC )
            {
                receiptLotOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReceiptLotClass, "options.png", false, false );

                /*
                 * Receipt Lot No OCP- waiting on 27877
                 */

                CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp materialOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = materialOC.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = true
                } );

                /*
                 * Material ID - waiting on 27864
                 */

                CswNbtMetaDataObjectClassProp expirationDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.ExpirationDate,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime
                } );

                CswNbtMetaDataObjectClassProp underInvestigationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.UnderInvestigation,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( underInvestigationOCP, CswNbtSubField.SubFieldName.Checked, false );

                CswNbtMetaDataObjectClassProp investigationNotesOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.InvestigationNotes,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments
                } );

                CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass );
                CswNbtMetaDataObjectClassProp manufacturerOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.Manufacturer,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = vendorOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClassProp vendorTypeOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                CswNbtView manufacturerOCPView = _CswNbtSchemaModTrnsctn.makeNewView( "mepManufacturerOCP", NbtViewVisibility.Property );
                CswNbtViewRelationship parent = manufacturerOCPView.AddViewRelationship( vendorOC, true );
                manufacturerOCPView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: vendorTypeOCP,
                    Value: "Manufacturing",
                    SubFieldName: CswNbtSubField.SubFieldName.Value,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( manufacturerOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, manufacturerOCPView.ToString() );

                CswNbtMetaDataObjectClass requestItemOC_27867 = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
                CswNbtMetaDataObjectClassProp requestItemOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.RequestItem,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = requestItemOC_27867.ObjectClassId
                } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, receiptLotOC.ObjectClassId );
            }

                CswNbtMetaDataObjectClassProp AutoCertAppOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( InventoryGroupOc )
                {
                    PropName = CswNbtObjClassInventoryGroup.PropertyName.AutomaticCertificateApproval,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AutoCertAppOCP, AutoCertAppOCP.getFieldTypeRule().SubFields.Default.Name, Tristate.False );
            }

            #endregion Case 27870 - New InventoryGroup ObjClassProps

            #endregion TITANIA

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01b

}//namespace ChemSW.Nbt.Schema


