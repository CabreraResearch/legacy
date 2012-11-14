using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01OC : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

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

        #region Titania Methods

        private void _makeCertMethodTemplateOc()
        {
            #region CertMethodTemplate

            _acceptBlame( CswDeveloper.CF, 27868 );

            CswNbtMetaDataObjectClass CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CofAMethodTemplateClass );
            if( null == CertMethodTemplateOc )
            {
                CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.CofAMethodTemplateClass, NbtIcon.flask, true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.MLM, CertMethodTemplateOc.ObjectClassId );
            }


            CswNbtMetaDataObjectClassProp CmtMaterialOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Material );
            if( null == CmtMaterialOcp )
            {
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CmtMaterialOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    ServerManaged = true,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId
                } );
            }

            //TODO: Create CertMethodId Property when PropRefSequence is implemented

            CswNbtMetaDataObjectClassProp CmtDescriptionOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Description );
            if( null == CmtDescriptionOcp )
            {
                CmtDescriptionOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Description,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtMethodNoOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.MethodNo );
            if( null == CmtMethodNoOcp )
            {
                CmtMethodNoOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.MethodNo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtConditionsOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Conditions );
            if( null == CmtConditionsOcp )
            {
                CmtConditionsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Conditions,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtLowerOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Lower );
            if( null == CmtLowerOcp )
            {
                CmtLowerOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Lower,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmtUpperOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Upper );
            if( null == CmtUpperOcp )
            {
                CmtUpperOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Upper,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmtUnitsOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Units );
            if( null == CmtUnitsOcp )
            {
                CmtUnitsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Units,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmtQualifiedOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Qualified );
            if( null == CmtQualifiedOcp )
            {
                CmtQualifiedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Qualified,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CmtQualifiedOcp, Tristate.False );
            }

            //TODO: Create CertDetConditionalSet when CertDef Object Class is implemented

            CswNbtMetaDataObjectClassProp CmtObsoleteOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCofAMethodTemplate.PropertyName.Obsolete );
            if( null == CmtObsoleteOcp )
            {
                CmtObsoleteOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodTemplateOc )
                {
                    PropName = CswNbtObjClassCofAMethodTemplate.PropertyName.Obsolete,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CmtObsoleteOcp, Tristate.False );
            }

            _resetBlame();

            #endregion CertMethodTemplate
        }

        private void _makeCertMethodOc()
        {
            #region CertMethod

            _acceptBlame( CswDeveloper.CF, 27868 );

            CswNbtMetaDataObjectClass CertMethodOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CofAMethodClass );
            if( null == CertMethodOc )
            {
                CertMethodOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.CofAMethodClass, NbtIcon.flask, true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.MLM, CertMethodOc.ObjectClassId );
            }

            CswNbtMetaDataObjectClassProp CmTemplateOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.CertMethodTemplate );
            if( null == CmTemplateOcp )
            {
                CswNbtMetaDataObjectClass CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CofAMethodTemplateClass );
                CmTemplateOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.CertMethodTemplate,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    ServerManaged = true,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = CertMethodTemplateOc.ObjectClassId
                } );
            }

            //TODO: Create ReceiptLot Relationship Property when Receipt Lot Object Class is implemented

            CswNbtMetaDataObjectClassProp CmDescriptionOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.Description );
            if( null == CmDescriptionOcp )
            {
                CmDescriptionOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.Description,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmMethodNoOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.MethodNo );
            if( null == CmMethodNoOcp )
            {
                CmMethodNoOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.MethodNo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmConditionsOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.Conditions );
            if( null == CmConditionsOcp )
            {
                CmConditionsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.Conditions,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmLowerOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.Lower );
            if( null == CmLowerOcp )
            {
                CmLowerOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.Lower,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmUpperOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.Upper );
            if( null == CmUpperOcp )
            {
                CmUpperOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.Upper,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number
                } );
            }

            CswNbtMetaDataObjectClassProp CmUnitsOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.Units );
            if( null == CmUnitsOcp )
            {
                CmUnitsOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.Units,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    ReadOnly = true
                } );
            }

            CswNbtMetaDataObjectClassProp CmValueOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.Value );
            if( null == CmValueOcp )
            {
                CmValueOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.Units,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );
            }

            CswNbtMetaDataObjectClassProp CmQualifiedOcp = CertMethodOc.getObjectClassProp( CswNbtObjClassCofAMethod.PropertyName.Qualified );
            if( null == CmQualifiedOcp )
            {
                CmQualifiedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( CertMethodOc )
                {
                    PropName = CswNbtObjClassCofAMethod.PropertyName.Qualified,
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

        private void _makeNewInvGroupProps()
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

                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CentralOCP, Tristate.False );

                CswNbtMetaDataObjectClassProp AutoCertAppOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( InventoryGroupOc )
                {
                    PropName = CswNbtObjClassInventoryGroup.PropertyName.AutomaticCertificateApproval,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AutoCertAppOCP, Tristate.False );
            }
            _resetBlame();
            #endregion Case 27870 - New InventoryGroup ObjClassProps
        }

        private void _makeEnterprisePartsAndManufacturerEquivalentPartsOCs()
        {
            _acceptBlame( CswDeveloper.MB, 27865 );
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
            _resetBlame();
        }

        private void _makeReceiptLotOC()
        {
            #region Case 27867 - Receipt Lot
            _acceptBlame( CswDeveloper.MB, 27867 );
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
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( underInvestigationOCP, false );

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

                //Appropriate Request OC doesn't exist yet. It will be responsible for fixing this.
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( receiptLotOC )
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.RequestItem,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship
                } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, receiptLotOC.ObjectClassId );
            }
            _resetBlame();
            #endregion
        }

        private void _setNodesToHiddenIfNull()
        {
            #region Case 27862 - set nodes hidden = "0" if null
            _acceptBlame( CswDeveloper.MB, 27862 );
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
            _resetBlame();
            #endregion
        }

        private void _makeContainerFamilyButton()
        {
            #region Case 27884 - container family display button
            _acceptBlame( CswDeveloper.MB, 27884 );
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp containerFamilyOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ContainerFamily,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
            } );
            _resetBlame();
            #endregion
        }
        #endregion

        #region Ursula Methods

        public void _makeContainerGroup()
        {
            #region 27866 - Container Group
            _acceptBlame( CswDeveloper.MB, 27866 );

            CswNbtMetaDataObjectClass containerGoupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerGroupClass );
            if( null == containerGoupOC )
            {
                containerGoupOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.ContainerGroupClass, "barcode.png", false );

                CswNbtMetaDataObjectClassProp containerGroupNameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.Name,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );

                //this barcode prop has to start with a "G" - sequence it set on the NTP and thus in a schemascript
                CswNbtMetaDataObjectClassProp containerGroupBarcodeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.Barcode,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Barcode
                } );

                CswNbtMetaDataObjectClassProp containerGroupSyncLocationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.SyncLocation,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( containerGroupSyncLocationOCP, false );

                CswNbtMetaDataObjectClassProp containerGroupLocationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.Location,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location
                } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, containerGoupOC.ObjectClassId );

            }
            _resetBlame();
            #endregion
        }

        public void _newContainerProperties27866()
        {
            #region Case 27866 part 2 - new container properties
            _acceptBlame( CswDeveloper.MB, 27866 );

            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

            CswNbtMetaDataObjectClass receiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );
            CswNbtMetaDataObjectClassProp receiptLotOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ReceiptLot,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = receiptLotOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                ServerManaged = true
            } );

            CswNbtMetaDataObjectClassProp lotControlledOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.LotControlled,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( lotControlledOCP, false );

            CswNbtMetaDataObjectClassProp requisitionableOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.Requisitionable,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( requisitionableOCP, false );

            CswNbtMetaDataObjectClass containerGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerGroupClass );
            CswNbtMetaDataObjectClassProp containerGroupOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ContainerGroup,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = containerGroupOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString()
            } );

            CswNbtMetaDataObjectClass printLabelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp labelFormatOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.LabelFormat,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = printLabelOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString()
            } );

            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp reservedForOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ReservedFor,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = userOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString()
            } );

            _resetBlame();
            #endregion
        }

        private void _destroyRequestItemOc( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "RequestItemClass" );
            if( null != RequestItemOc )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( RequestItemOc );
            }
            _resetBlame();
        }

        private CswNbtMetaDataObjectClass _createRequestItemBase( NbtObjectClass ObjectClass, Int32 StartAddRowAt = 1 )
        {
            CswNbtMetaDataObjectClass Ret = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjectClass );
            if( null == Ret )
            {
                CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupClass );
                CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                Ret = _CswNbtSchemaModTrnsctn.createObjectClass( ObjectClass, NbtIcon.cart, AuditLevel: true );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Location,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.RequestedFor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.InventoryGroup,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = InventoryGroupOc.ObjectClassId,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.NeededBy,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Name,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Description,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Static
                } );

                CswNbtMetaDataObjectClassProp StatusOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Status,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOcp, CswNbtPropertySetRequestItem.Statuses.Pending );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Type,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Requestor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp PriorityOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Priority,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                    NumberPrecision = 1,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( PriorityOcp, 0 );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Number,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    ServerManaged = true,
                    SetValOnAdd = false,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Request,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = RequestOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.ExternalOrderNumber,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = false
                } );



                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.AssignedTo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Comments,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Fulfill,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    Extended = CswNbtNodePropButton.ButtonMode.menu,
                    StaticText = CswNbtPropertySetRequestItem.FulfillMenu.Complete,
                    SetValOnAdd = false
                } );

            }
            return Ret;
        }

        private void _createRequestContainerDispense( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestContainerDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerDispenseClass );
            if( null == RequestContainerDispenseOc )
            {
                CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

                CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );

                RequestContainerDispenseOc = _createRequestItemBase( NbtObjectClass.RequestContainerDispenseClass );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Container,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ContainerOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestContainerDispense.FulfillMenu.Dispense );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.FulfillMenu.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Quantity,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    IsRequired = true,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 5
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Size,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = SizeOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.Statuses.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.TotalDispensed,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.Types.ContainerDispense );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, CswNbtObjClassRequestContainerDispense.Types.ContainerDispense );
            }
            _resetBlame();
        }

        private void _createRequestContainerUpdate( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestContainerUpdateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerUpdateClass );
            if( null == RequestContainerUpdateOc )
            {
                CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

                RequestContainerUpdateOc = _createRequestItemBase( NbtObjectClass.RequestContainerUpdateClass );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerUpdateOc )
                {
                    PropName = CswNbtObjClassRequestContainerUpdate.PropertyName.Container,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ContainerOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestContainerUpdate.FulfillMenu.Dispose );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerUpdate.FulfillMenu.DisposeOptions.ToString() );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerUpdate.Types.Options.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, CswNbtObjClassRequestContainerUpdate.Types.Dispose );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerUpdate.Statuses.Options.ToString() );
            }
            _resetBlame();
        }

        private void _createRequestMaterialDispense( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestMaterialDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            if( null == RequestMaterialDispenseOc )
            {
                CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );

                RequestMaterialDispenseOc = _createRequestItemBase( NbtObjectClass.RequestMaterialDispenseClass );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestMaterialDispense.FulfillMenu.Order );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialDispense.FulfillMenu.Options.ToString() );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialDispense.Types.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Quantity,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    IsRequired = true,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 5
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Count,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                    SetValOnAdd = true,
                    IsRequired = true,
                    NumberPrecision = 0,
                    NumberMinValue = 1,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 6
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Size,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = SizeOc.ObjectClassId,
                    IsRequired = true,
                    SetValOnAdd = true,
                    DisplayRowAdd = 7,
                    DisplayColAdd = 1
                } );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.Statuses.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.TotalDispensed,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.ReorderFrequency,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.TimeInterval,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Level,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp IsBatchOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.IsBatch,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IsBatchOcp, false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Batch,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                 {
                     PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.ReceiptLotsReceived,
                     FieldType = CswNbtMetaDataFieldType.NbtFieldType.Grid,
                     SetValOnAdd = false,
                     Extended = CswNbtNodePropGrid.GridPropMode.Link.ToString(),

                 } );

                CswNbtMetaDataObjectClassProp GoodsReceivedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.GoodsReceived,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( GoodsReceivedOcp, false );

                CswNbtMetaDataObjectClass ReceiptLotOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.ReceiptLotToDispense,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ReceiptLotOc.ObjectClassId,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp ReceiptLotRequestOcp = ReceiptLotOc.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.RequestItem );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReceiptLotRequestOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReceiptLotRequestOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, RequestMaterialDispenseOc.ObjectClassId );
                //We should fix ContainerDispenseTransaction too, but PropertySets aren't in the database. So we have to fix the relationship view on the NodeTypeProp.
            }
            _resetBlame();
        }

        private void _createRequestMaterialCreate( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestMaterialCreateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialCreateClass );
            if( null == RequestMaterialCreateOc )
            {
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClass SupplierOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );

                RequestMaterialCreateOc = _createRequestItemBase( NbtObjectClass.RequestMaterialCreateClass, 5 );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestMaterialCreateOc.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestMaterialCreate.FulfillMenu.Create );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialCreate.FulfillMenu.Options.ToString() );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestMaterialCreateOc.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialCreate.Types.Options.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, CswNbtObjClassRequestMaterialCreate.Types.Create );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestMaterialCreateOc.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialCreate.Statuses.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId,
                    SetValOnAdd = false,
                    ServerManaged = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialType,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId,
                    Multi = false,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 1,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialTradename,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 2,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialSupplier,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = SupplierOc.ObjectClassId,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 3,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialPartNo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 4
                } );
            }
            _resetBlame();
        }

        private void _createaNewMaterialComponentProp( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );

            CswNbtMetaDataObjectClassProp activeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialComponentOC )
            {
                PropName = CswNbtObjClassMaterialComponent.PropertyName.Active,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( activeOCP, false );

            _resetBlame();
        }

        private void _createUNCodeNodeType( CswDeveloper Dev, Int32 CaseNo )
        {
            CswNbtMetaDataObjectClass GenericOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GenericClass );
            if( null != GenericOc )
            {
                //LQNo NodeType
                CswNbtMetaDataNodeType LQNoNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "LQNo", "MLM" );
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, LQNoNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp LQNoLQNoNtp = _createNewProp( LQNoNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Text );
                LQNoLQNoNtp.setIsUnique( true );
                CswNbtMetaDataNodeTypeProp LQNoLimitNtp = _createNewProp( LQNoNt, "Limit", CswNbtMetaDataFieldType.NbtFieldType.Quantity );
                LQNoLimitNtp.IsRequired = true;
                CswNbtMetaDataNodeType WeightNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Weight)" );
                if( null != WeightNt )
                {
                    LQNoLimitNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), WeightNt.NodeTypeId );
                }
                LQNoNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "LQNo" ) );

                //UNCode NodeType
                CswNbtMetaDataNodeType UNCodeNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "UN Code", "MLM" );
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, UNCodeNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp UNCodeUNCodeNtp = _createNewProp( UNCodeNt, "UN Code", CswNbtMetaDataFieldType.NbtFieldType.Text );
                UNCodeUNCodeNtp.setIsUnique( true );
                CswNbtMetaDataNodeTypeProp UNCodeLQNoNtp = _createNewProp( UNCodeNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false );
                UNCodeLQNoNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), LQNoNt.NodeTypeId );
                UNCodeNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "UN Code" ) );

                //Create Demo Data
                if( null != WeightNt )
                {
                    CswPrimaryKey kgNodeId = null;
                    foreach( CswNbtObjClassUnitOfMeasure WeightNode in WeightNt.getNodes( false, false ) )
                    {
                        if( "kg" == WeightNode.Name.Text )
                        {
                            kgNodeId = WeightNode.NodeId;
                        }
                    }
                    CswNbtNode LQNoNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( LQNoNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    LQNoNode.Properties[LQNoLQNoNtp].AsText.Text = "1 Metric Ton";
                    LQNoNode.Properties[LQNoLimitNtp].AsQuantity.Quantity = 1000;
                    LQNoNode.Properties[LQNoLimitNtp].AsQuantity.UnitId = kgNodeId;
                    LQNoNode.IsDemo = true;
                    LQNoNode.postChanges( false );

                    CswNbtNode UNCodeNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UNCodeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    UNCodeNode.Properties[UNCodeLQNoNtp].AsRelationship.RelatedNodeId = LQNoNode.NodeId;
                    UNCodeNode.Properties[UNCodeUNCodeNtp].AsText.Text = "US ITH";
                    UNCodeNode.IsDemo = true;
                    UNCodeNode.postChanges( false );
                }

                //Create demo Views
                CswNbtView UNCodeView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes", NbtViewVisibility.Global );
                UNCodeView.Category = "MLM (demo)";
                UNCodeView.IsDemo = true;
                UNCodeView.ViewMode = NbtViewRenderingMode.Tree;
                UNCodeView.AddViewRelationship( UNCodeNt, true );
                UNCodeView.save();

                CswNbtView LQNoView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes by LQNo", NbtViewVisibility.Global );
                LQNoView.Category = "MLM (demo)";
                LQNoView.IsDemo = true;
                LQNoView.ViewMode = NbtViewRenderingMode.Tree;
                CswNbtViewRelationship LQNoRelationship = LQNoView.AddViewRelationship( LQNoNt, true );
                LQNoView.AddViewRelationship( LQNoRelationship, NbtViewPropOwnerType.Second, UNCodeLQNoNtp, false );
                LQNoView.save();

                //Update Chemical to include UN Code
                CswNbtMetaDataNodeType ChemicalNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
                if( null != ChemicalNt )
                {
                    CswNbtMetaDataNodeTypeProp ChemUNCodeNtp = _createNewProp( ChemicalNt, "UN Code", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false );
                    ChemUNCodeNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), UNCodeNt.NodeTypeId );
                }
            }
        }

        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswNbtMetaDataFieldType.NbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop.PropId,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop.PropId,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        private void _createNewMaterialProps( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            CswNbtMetaDataObjectClassProp materialIdOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
            {
                PropName = CswNbtObjClassMaterial.PropertyName.MaterialId,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                ServerManaged = true,
                IsUnique = true
            } );

            CswNbtMetaDataObjectClassProp approvedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
            {
                PropName = CswNbtObjClassMaterial.PropertyName.Approved,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( approvedOCP, false );

            CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            CswNbtMetaDataObjectClassProp vendorNameOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
            CswNbtView supplierView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship supplierParent = supplierView.AddViewRelationship( vendorOC, true );
            supplierView.AddViewPropertyAndFilter( supplierParent,
                MetaDataProp: vendorNameOCP,
                Value: "Corporate",
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            CswNbtMetaDataObjectClassProp supplierOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( supplierOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, supplierView.ToXml().ToString() );

            CswNbtMetaDataNodeType unCodeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "UN Code" );
            if( null != unCodeNT )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
                {
                    PropName = CswNbtObjClassMaterial.PropertyName.UNCode,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.NodeTypeId.ToString(),
                    FkValue = unCodeNT.NodeTypeId
                } );
            }

            CswNbtMetaDataObjectClass mepOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ManufacturerEquivalentPartClass );
            CswNbtMetaDataObjectClassProp manufacturerOCP = mepOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer );
            CswNbtMetaDataObjectClassProp materialOCP = mepOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material );

            CswNbtView manufacturingSitesView = _CswNbtSchemaModTrnsctn.makeNewView( CswNbtObjClassMaterial.PropertyName.ManufacturingSites, NbtViewVisibility.Property );
            CswNbtViewRelationship parent = manufacturingSitesView.AddViewRelationship( materialOC, true );
            CswNbtViewRelationship parent2 = manufacturingSitesView.AddViewRelationship( parent, NbtViewPropOwnerType.Second, materialOCP, false );
            manufacturingSitesView.AddViewProperty( parent2, manufacturerOCP );
            manufacturingSitesView.SetViewMode( NbtViewRenderingMode.Grid );
            manufacturingSitesView.save();

            CswNbtMetaDataObjectClassProp manufacturingSitesOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
            {
                PropName = CswNbtObjClassMaterial.PropertyName.ManufacturingSites,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Grid
            } );

            _resetBlame();
        }

        #endregion Ursula Methods


        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region TITANIA

            _makeCertMethodTemplateOc();
            _makeCertMethodOc();
            _makeMethodOc();
            _makeJurisdictionOc();
            _makeNewInvGroupProps();
            _makeEnterprisePartsAndManufacturerEquivalentPartsOCs();
            _makeReceiptLotOC();
            _setNodesToHiddenIfNull();
            _makeContainerFamilyButton();

            #endregion TITANIA

            #region URSULA

            _destroyRequestItemOc( CswDeveloper.CF, 27942 );
            _createRequestContainerDispense( CswDeveloper.CF, 27942 );
            _createRequestContainerUpdate( CswDeveloper.CF, 27942 );
            _createRequestMaterialDispense( CswDeveloper.CF, 27942 );
            _createRequestMaterialCreate( CswDeveloper.CF, 27871 );

            _makeContainerGroup();
            _newContainerProperties27866();

            _createaNewMaterialComponentProp( CswDeveloper.MB, 27864 );
            _createUNCodeNodeType( CswDeveloper.MB, 27872 );
            _createNewMaterialProps( CswDeveloper.MB, 27864 );

            #endregion URSULA

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01OC

}//namespace ChemSW.Nbt.Schema


