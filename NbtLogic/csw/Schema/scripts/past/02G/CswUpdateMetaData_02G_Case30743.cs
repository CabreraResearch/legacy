using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02G_Case30743: CswUpdateSchemaTo
    {
        public override string Title { get { return "Add new Material Properties"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30743; }
        }

        public override string ScriptName
        {
            get { return "Case30743_New_Materials_Props"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );

            CswNbtMetaDataObjectClassProp DisposalOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.DisposalInstructions ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                    {
                        PropName = CswNbtObjClassChemical.PropertyName.DisposalInstructions,
                        FieldType = CswEnumNbtFieldType.Memo
                    } );

            CswNbtMetaDataObjectClassProp HazardsOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.HazardInfo ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.HazardInfo,
                    FieldType = CswEnumNbtFieldType.Memo
                } );

            CswNbtMetaDataObjectClassProp CompressedGasOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.CompressedGas ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.CompressedGas,
                    FieldType = CswEnumNbtFieldType.Logical
                } );

            CswNbtMetaDataObjectClassProp SMILESOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.SMILES ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.SMILES,
                    FieldType = CswEnumNbtFieldType.Text
                } );

            CswNbtMetaDataNodeType TimeUoM_OC = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Time)" );
            CswNbtMetaDataObjectClassProp OpenExpireIntervalOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.OpenExpireInterval ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.OpenExpireInterval,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    IsFk = true,
                    FkType = "NodeTypeId",
                    FkValue = TimeUoM_OC.NodeTypeId
                } );

            CswNbtMetaDataObjectClassProp EINECSOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.EINECS ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.EINECS,
                    FieldType = CswEnumNbtFieldType.Text
                } );

            CswNbtMetaDataObjectClassProp DOTCode = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.DOTCode ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                    {
                        PropName = CswNbtObjClassChemical.PropertyName.DOTCode,
                        FieldType = CswEnumNbtFieldType.Number
                    } );

            CswNbtMetaDataObjectClassProp SubclassName = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.SubclassName ) ??
               _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
               {
                   PropName = CswNbtObjClassChemical.PropertyName.SubclassName,
                   FieldType = CswEnumNbtFieldType.Text,
                   ServerManaged = true
               } );


        }
    }
}