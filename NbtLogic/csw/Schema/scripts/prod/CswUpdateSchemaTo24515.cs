using System;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to case 24515
    /// </summary>
    public class CswUpdateSchemaToCase24515 : CswUpdateSchemaTo
    {

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Comments, MetaData.CswNbtMetaDataFieldType.DataType.XML );
        }//Update()

    }//class CswUpdateSchemaTo24515

}//namespace ChemSW.Nbt.Schema