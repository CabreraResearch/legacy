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

                CswNbtMetaDataObjectClass requestItemOC_27867 = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtDoomedObjectClasses.RequestItemClass );
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

        #endregion Titania Methods

        #region Ursula Methods

        private void _destroyRequestItemOc()
        {
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtDoomedObjectClasses.RequestItemClass );
            if( null != RequestItemOc )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( RequestItemOc );
            }
        }

        private void _createRequestContainerDispense( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestContainerDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerDispenseClass );
            if( null == RequestContainerDispenseOc )
            {
                CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupClass );
                CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
                RequestContainerDispenseOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.RequestContainerDispenseClass, NbtIcon.cart, AuditLevel: true );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.AssignedTo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Comments,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments,
                    SetValOnAdd = false
                } );

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

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.ExternalOrderNumber,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Fulfill,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    Extended = CswNbtNodePropButton.ButtonMode.menu,
                    StaticText = CswNbtObjClassRequestContainerDispense.FulfillMenu.Dispense,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.InventoryGroup,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = InventoryGroupOc.ObjectClassId,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Location,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId,
                    SetValOnAdd = false,
                    ServerManaged = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Name,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.NeededBy,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Number,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Quantity,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    IsRequired = true,
                    SetValOnAdd = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Request,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = RequestOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.RequestedFor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Requestor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    ServerManaged = true
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

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Status,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    ListOptions = CswNbtObjClassRequestContainerDispense.Statuses.Options.ToString(),
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.TotalDispensed,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

            }
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

            _createRequestContainerDispense( CswDeveloper.CF, 27942 );

            #endregion URSULA

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01OC

}//namespace ChemSW.Nbt.Schema


