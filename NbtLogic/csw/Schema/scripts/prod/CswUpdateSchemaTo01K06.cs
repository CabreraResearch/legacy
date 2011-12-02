using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01K-06
    /// </summary>
    public class CswUpdateSchemaTo01K06 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 06 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            //Case 24324
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "node_views", "viewname", DataDictionaryPortableDataType.String, 200 );

        }//Update()

    }//class CswUpdateSchemaTo01K06

}//namespace ChemSW.Nbt.Schema


