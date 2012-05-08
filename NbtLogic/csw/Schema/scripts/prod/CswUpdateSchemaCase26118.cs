using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26118
    /// </summary>
    public class CswUpdateSchemaCase26118 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataFieldType QuestionFt = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Question );
            _CswNbtSchemaModTrnsctn.createFieldTypesSubFieldsJunction( QuestionFt, CswNbtSubField.PropColumn.Field4, CswNbtSubField.SubFieldName.ChangedDate, true );
            _CswNbtSchemaModTrnsctn.createFieldTypesSubFieldsJunction( QuestionFt, CswNbtSubField.PropColumn.Field5, CswNbtSubField.SubFieldName.Name, true );


        }//Update()

    }//class CswUpdateSchemaCase26118

}//namespace ChemSW.Nbt.Schema