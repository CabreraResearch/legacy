

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-09
    /// </summary>
    public class CswUpdateSchemaToCase25538_Part_1 : CswUpdateSchemaTo
    {
        //        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 11 ); } }
        //        public override string Description { set { ; } get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Component" ) );
            _CswNbtSchemaModTrnsctn.deleteView( "Components", true );

        }//Update()

    }//class  CswUpdateSchemaToCase25538_Part_1

}//namespace ChemSW.Nbt.Schema