
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-01
    /// </summary>
    public class CswUpdateSchemaTo01M01 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 01 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {



        }//Update()

    }//class CswUpdateSchemaTo01M01

}//namespace ChemSW.Nbt.Schema