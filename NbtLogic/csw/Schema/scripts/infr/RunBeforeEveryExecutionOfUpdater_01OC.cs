using System;
using ChemSW.Core;
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

        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region SEBASTIAN

            // case 27703 - change containers dispose/dispense buttons to say "Dispose this Container" and "Dispense this Container"
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.ContainerClass );

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

            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp ControlTypeOcp = PrintLabelOc.getObjectClassProp( "Control Type" );
            if( null != ControlTypeOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( ControlTypeOcp, DeleteNodeTypeProps: true );
            }

            //upgrade RequestItem Requestor prop from NTP to OCP
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType requestItemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Item" );
            if( null != requestItemNT && null == requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Requestor ) )
            {

                CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.RequestClass );
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

                CswNbtMetaDataNodeTypeProp reqItemRequestorNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( requestItemNT.NodeTypeId, reqItemrequestorOCP.PropId );

                reqItemRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }


            #endregion SEBASTIAN

            #region TITANIA

            #region CertMethodTemplate

            _acceptBlame( CswDeveloper.CF, 27868 );

            CswNbtMetaDataObjectClass CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.CertMethodTemplateClass );
            if( null == CertMethodTemplateOc )
            {
                CertMethodTemplateOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.CertMethodTemplateClass, CswNbtMetaDataIconName.NbtIcon.flask, true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.MLM, CertMethodTemplateOc.ObjectClassId );
            }


            CswNbtMetaDataObjectClassProp CmtMaterialOcp = CertMethodTemplateOc.getObjectClassProp( CswNbtObjClassCertMethodTemplate.PropertyName.Material );
            if( null == CmtMaterialOcp )
            {
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.MaterialClass );
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



            #endregion TITANIA


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

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01b

}//namespace ChemSW.Nbt.Schema


