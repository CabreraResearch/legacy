using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS53248 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 53248; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo + ": MLM2: Certificate Definition OC"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            // Create Object Class
            CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.CertificateDefinitionClass, "doc.png", true );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, CertDefOC.ObjectClassId );

            // Create Object Class Props
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.CertDefId,
                    FieldType = CswEnumNbtFieldType.Text,
                    ReadOnly = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.Material,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsRequired = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.Version,
                    FieldType = CswEnumNbtFieldType.Number
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.RetainCount,
                    FieldType = CswEnumNbtFieldType.Number
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.RetainQuantity,
                    FieldType = CswEnumNbtFieldType.Quantity
                } );

            CswNbtMetaDataNodeType UnitTimeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit Time" );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.RetainExpiration,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    FkType = CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(),
                    FkValue = UnitTimeNT.NodeTypeId
                } );


            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.Approved,
                    FieldType = CswEnumNbtFieldType.Logical
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.ApprovedDate,
                    FieldType = CswEnumNbtFieldType.DateTime,
                    ServerManaged = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.QualifiedManufacturerOnly,
                    FieldType = CswEnumNbtFieldType.MultiList
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.CertDefSpecs,
                    FieldType = CswEnumNbtFieldType.Grid
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.NewDraft,
                    FieldType = CswEnumNbtFieldType.Button
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.CurrentApproved,
                    FieldType = CswEnumNbtFieldType.Logical,
                    ServerManaged = true
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.Obsolete,
                    FieldType = CswEnumNbtFieldType.Logical
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CertDefOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassCertificateDefinition.PropertyName.Versions,
                    FieldType = CswEnumNbtFieldType.Grid
                } );
        }
    }
}