using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31234A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31234; }
        }

        public override string Title
        {
            get { return "Add Product Description, Legacy Material Id to Chemical ObjClass"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.LegacyMaterialId,
                    FieldType = CswEnumNbtFieldType.Text
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.ProductDescription,
                FieldType = CswEnumNbtFieldType.Memo
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema