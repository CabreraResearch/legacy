using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25894
    /// </summary>
    public class CswUpdateSchemaCase25894 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "session_data", "name", DataDictionaryPortableDataType.String, 1000 );

        }//Update()

    }//class CswUpdateSchemaCase25894

}//namespace ChemSW.Nbt.Schema