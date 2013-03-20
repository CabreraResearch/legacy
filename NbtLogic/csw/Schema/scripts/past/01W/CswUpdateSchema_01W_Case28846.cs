using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28846
    /// </summary>
    public class CswUpdateSchema_01W_Case28846 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28846; }
        }

        public override void update()
        {
            CswNbtFieldTypeRuleCASNo CasNo_FTRule = (CswNbtFieldTypeRuleCASNo) _CswNbtSchemaModTrnsctn.MetaData.getFieldTypeRule( CswNbtMetaDataFieldType.NbtFieldType.CASNo );
            CswNbtMetaDataFieldType CasNo_FT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.CASNo );

            foreach( CswNbtSubField subfield in CasNo_FTRule.SubFields )
            {
                _CswNbtSchemaModTrnsctn.createFieldTypesSubFieldsJunction( CasNo_FT, subfield.Column, subfield.Name, true );
            }

        } //Update()

    }//class CswUpdateSchema_01W_Case28846

}//namespace ChemSW.Nbt.Schema