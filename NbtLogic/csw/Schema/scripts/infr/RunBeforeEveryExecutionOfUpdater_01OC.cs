using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01OC : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo = 0;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        private void _makeCertMethodTemplateOc()
        {
            #region CertMethodTemplate

            _acceptBlame( CswDeveloper.CF, 27868 );

            CswNbtMetaDataObjectClass CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CertMethodTemplateClass );
            if( null == CertMethodTemplateOc )
            {
                CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.CertMethodTemplateClass, NbtIcon.flask, true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.MLM, CertMethodTemplateOc.ObjectClassId );
            }


            CswNbtMetaDataObjectClassProp CmtMaterialOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Material );
            if( null == CmtMaterialOcp )
            {
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CmtMaterialOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    ServerManaged = true,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId
                } );
            }

            //TODO: Create CertMethodId Property when PropRefSequence is implemented

            CswNbtMetaDataObjectClassProp CmtDescriptionOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Description );
            if( null == CmtDescriptionOcp )
            {
                CmtDescriptionOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Description,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtMethodNoOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.MethodNo );
            if( null == CmtMethodNoOcp )
            {
                CmtMethodNoOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.MethodNo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtConditionsOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Conditions );
            if( null == CmtConditionsOcp )
            {
                CmtConditionsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Conditions,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtLowerOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Lower );
            if( null == CmtLowerOcp )
            {
                CmtLowerOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Lower,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmtUpperOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Upper );
            if( null == CmtUpperOcp )
            {
                CmtUpperOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Upper,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmtUnitsOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Units );
            if( null == CmtUnitsOcp )
            {
                CmtUnitsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Units,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtQualifiedOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Qualified );
            if( null == CmtQualifiedOcp )
            {
                CmtQualifiedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Qualified,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CmtQualifiedOcp, null, Tristate.False );
            }

            //TODO: Create CertDetConditionalSet when CertDef Object Class is implemented

            CswNbtMetaDataObjectClassProp CmtObsoleteOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Obsolete );
            if( null == CmtObsoleteOcp )
            {
                CmtObsoleteOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCertMethodTemplate.PropertyName.Obsolete,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CmtObsoleteOcp, null, Tristate.False );
            }

            _resetBlame();

            #endregion CertMethodTemplate
        }

        private void _makeCertMethodOc()
        {
            #region CertMethod

            _acceptBlame( CswDeveloper.CF, 27868 );

            CswNbtMetaDataObjectClass CertMethodOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CertMethodClass );
            if( null == CertMethodOc )
            {
                CertMethodOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.CertMethodClass, NbtIcon.flask, true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.MLM, CertMethodOc.ObjectClassId );
            }

            CswNbtMetaDataObjectClassProp CmTemplateOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.CertMethodTemplate );
            if( null == CmTemplateOcp )
            {
                CswNbtMetaDataObjectClass CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CertMethodTemplateClass );
                CmTemplateOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.CertMethodTemplate,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    ServerManaged = true,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = CertMethodTemplateOc.ObjectClassId
                } );
            }

            //TODO: Create ReceiptLot Relationship Property when Receipt Lot Object Class is implemented

            CswNbtMetaDataObjectClassProp CmDescriptionOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.Description );
            if( null == CmDescriptionOcp )
            {
                CmDescriptionOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.Description,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmMethodNoOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.MethodNo );
            if( null == CmMethodNoOcp )
            {
                CmMethodNoOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.MethodNo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmConditionsOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.Conditions );
            if( null == CmConditionsOcp )
            {
                CmConditionsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.Conditions,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmLowerOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.Lower );
            if( null == CmLowerOcp )
            {
                CmLowerOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.Lower,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmUpperOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.Upper );
            if( null == CmUpperOcp )
            {
                CmUpperOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.Upper,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmUnitsOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.Units );
            if( null == CmUnitsOcp )
            {
                CmUnitsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.Units,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmValueOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.Value );
            if( null == CmValueOcp )
            {
                CmValueOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.Units,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmQualifiedOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCertMethod.PropertyName.Qualified );
            if( null == CmQualifiedOcp )
            {
                CmQualifiedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCertMethod.PropertyName.Qualified,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    ReadOnly = true
                } );
            }

            _resetBlame();

            #endregion CertMethod
        }

        private void _makeMethodOc()
        {
            #region Case 27869 - Method ObjectClass

            _acceptBlame( CswDeveloper.BV, 27869 );

            CswNbtMetaDataObjectClass MethodOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MethodClass );
            if( null == MethodOc )
            {
                //Create new ObjectClass
                MethodOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.MethodClass, "barchart.png", true );
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

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.MLM, MethodOc.ObjectClassId );

                _resetBlame();
            }
            #endregion Case 27869 - Method ObjectClass
        }

        private void _makeJurisdictionOc()
        {
            #region Case 27873 - Jurisdiction ObjectClass
            _acceptBlame( CswDeveloper.BV, 27873 );

            CswNbtMetaDataObjectClass JurisdictionOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.JurisdictionClass );
            if( null == JurisdictionOc )
            {
                //Create new ObjectClass
                JurisdictionOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.JurisdictionClass, "person.png", true );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( JurisdictionOc )
                {
                    PropName = CswNbtObjClassJurisdiction.PropertyName.Name,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    IsRequired = true,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, JurisdictionOc.ObjectClassId );
            }

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
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
            _resetBlame();

            #endregion Case 27873 - Jurisdiction ObjectClass
        }

        public void _makeNewInvGroupProps()
        {
            #region Case 27870 - New InventoryGroup ObjClassProps
            _acceptBlame( CswDeveloper.BV, 27870 );
            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupClass );
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

                CswNbtMetaDataObjectClassProp AutoCertAppOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( InventoryGroupOc )
                {
                    PropName = CswNbtObjClassInventoryGroup.PropertyName.AutomaticCertificateApproval,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AutoCertAppOCP, AutoCertAppOCP.getFieldTypeRule().SubFields.Default.Name, Tristate.False );
            }
            _resetBlame();
            #endregion Case 27870 - New InventoryGroup ObjClassProps
        }

        public void _makeEnterprisePartsAndManufacturerEquivalentPartsOCs()
        {
            #region Case 27865 part 1 - Enterprise Part (EP)

            CswNbtMetaDataObjectClass enterprisePartOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EnterprisePartClass );
            if( null == enterprisePartOC )
            {
                enterprisePartOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.EnterprisePartClass, "gear.png", false );

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

            CswNbtMetaDataObjectClass manufactuerEquivalentPartOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ManufacturerEquivalentPartClass );
            if( null == manufactuerEquivalentPartOC )
            {
                manufactuerEquivalentPartOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.ManufacturerEquivalentPartClass, "gearset.png", false );

                CswNbtMetaDataObjectClassProp epOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( manufactuerEquivalentPartOC )
                {
                    PropName = CswNbtObjClassManufacturerEquivalentPart.PropertyName.EnterprisePart,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = enterprisePartOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp materialOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( manufactuerEquivalentPartOC )
                {
                    PropName = CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = materialOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );
                CswNbtMetaDataObjectClassProp manufacturerOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( manufactuerEquivalentPartOC )
                {
                    PropName = CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = vendorOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClassProp vendorTypeOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                CswNbtView manufacturerOCPView = _CswNbtSchemaModTrnsctn.makeView();
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
        }

        public void _makeReceiptLotOC()
        {
            #region Case 27867 - Receipt Lot

            CswNbtMetaDataObjectClass receiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );
            if( null == receiptLotOC )
            {
                receiptLotOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.ReceiptLotClass, "options.png", false );

                /*
                 * Receipt Lot No OCP- waiting on 27877
                 */

                CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
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

                CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );
                CswNbtMetaDataObjectClassProp manufacturerOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.Manufacturer,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = vendorOC.ObjectClassId
                } );

                CswNbtMetaDataObjectClassProp vendorTypeOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                CswNbtView manufacturerOCPView = _CswNbtSchemaModTrnsctn.makeView();
                CswNbtViewRelationship parent = manufacturerOCPView.AddViewRelationship( vendorOC, true );
                manufacturerOCPView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: vendorTypeOCP,
                    Value: "Manufacturing",
                    SubFieldName: CswNbtSubField.SubFieldName.Value,
                    FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( manufacturerOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, manufacturerOCPView.ToString() );

                CswNbtMetaDataObjectClass requestItemOC_27867 = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestItemClass );
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
            #endregion
        }

        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.
            #region SEBASTIAN

            // case 27703 - change containers dispose/dispense buttons to say "Dispose this Container" and "Dispense this Container"
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

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

            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp ControlTypeOcp = PrintLabelOc.getObjectClassProp( "Control Type" );
            if( null != ControlTypeOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( ControlTypeOcp, DeleteNodeTypeProps: true );
            }

            //upgrade RequestItem Requestor prop from NTP to OCP
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestItemClass );
            if( null == requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Requestor ) )
            {
                CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
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
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );
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
            // NOTE: Due to case 27950, we have to fix nodetypes and object classes separately
            CswNbtMetaDataFieldType ViewReferenceFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ViewReference );
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ReportViewNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView );
                if( ReportViewNTP.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.ViewPickList )
                {
                    // map jct_nodes_props records
                    //   ViewReference: Name = field1, ViewId = field1_fk
                    //   ViewPickList: Name = gestalt, ViewId = field1
                    CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "27720_update_jnp", "jct_nodes_props" );
                    DataTable JctTable = JctUpdate.getTable( "nodetypepropid", ReportViewNTP.PropId );
                    foreach( DataRow JctRow in JctTable.Rows )
                    {
                        JctRow["field1_fk"] = JctRow["field1"];
                        JctRow["field1"] = JctRow["gestalt"];
                    }
                    JctUpdate.update( JctTable );

                    // fix the nodetype_prop record
                    // slightly kludgey, but works
                    ReportViewNTP._DataRow["fieldtypeid"] = ViewReferenceFT.FieldTypeId;
                }

            }

            // fix the object class record
            CswNbtMetaDataObjectClassProp ReportViewOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView );
            if( ReportViewOCP.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.ViewPickList )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReportViewOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, ViewReferenceFT.FieldTypeId );
            }

            #endregion case 27720

            #endregion SEBASTIAN

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );

            #region TITANIA

            _makeCertMethodTemplateOc();
            _makeCertMethodOc();
            _makeMethodOc();
            _makeJurisdictionOc();
            _makeNewInvGroupProps();
            _makeEnterprisePartsAndManufacturerEquivalentPartsOCs();
            _makeReceiptLotOC();

            #region Case 27862 - set nodes hidden = "0" if null

            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "hidden" ) )
            {
                //find all nodes where hidden = null and make it false (0)
                CswTableUpdate tu = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "hiddenNodes_27862", "nodes" );
                DataTable nodes = tu.getTable( "where hidden is null" );
                if( nodes.Rows.Count > 0 ) //only do an update if there were results from the query
                {
                    foreach( DataRow row in nodes.Rows )
                    {
                        row["hidden"] = CswConvert.ToDbVal( false );
                    }

                    tu.update( nodes );
                }
            }

            #endregion

            #endregion TITANIA
        }


        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01b

}//namespace ChemSW.Nbt.Schema


