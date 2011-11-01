

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01K-02
    /// </summary>
    public class CswUpdateSchemaTo01K02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 02 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            //Case 23901


        }//Update()

    }//class CswUpdateSchemaTo01K02

}//namespace ChemSW.Nbt.Schema


