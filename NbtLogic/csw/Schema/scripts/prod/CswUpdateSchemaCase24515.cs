using System;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 24515
    /// </summary>
    public class CswUpdateSchemaCase24515 : CswUpdateSchemaTo
    {

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Comments, MetaData.CswNbtMetaDataFieldType.DataType.XML );
        }//Update()

    }//class CswUpdateSchemaCase24515

}//namespace ChemSW.Nbt.Schema