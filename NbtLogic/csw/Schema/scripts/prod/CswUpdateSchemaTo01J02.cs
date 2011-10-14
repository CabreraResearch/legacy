

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-02
    /// </summary>
    public class CswUpdateSchemaTo01J02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 02 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            //Case 23809
            foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.restoreViews( "Roles and Users" ) )
            {
                View.Category = "System";
                View.save();
            }

        }//Update()

    }//class CswUpdateSchemaTo01J02

}//namespace ChemSW.Nbt.Schema


