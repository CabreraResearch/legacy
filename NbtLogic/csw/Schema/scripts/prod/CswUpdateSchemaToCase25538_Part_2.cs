using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-09
    /// </summary>
    public class CswUpdateSchemaToCase25538_Part_2 : CswUpdateSchemaTo
    {
        //        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 11 ); } }
        //        public override string Description { set { ; } get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            string MaterialsCategory = "Materials";
            string ComponentNodeTypeName = "Material Component";
            CswNbtMetaDataNodeType ComponentNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass.ToString(), ComponentNodeTypeName, MaterialsCategory );

            CswNbtView ComponentView = _CswNbtSchemaModTrnsctn.makeView();
            ComponentView.makeNew( "Components", NbtViewVisibility.Global, null, null, null );
            CswNbtViewRelationship ComponentRelationship = ComponentView.AddViewRelationship( ComponentNodeType, false );
            ComponentView.Category = MaterialsCategory;
            ComponentView.save();

        }//Update()

    }//class  CswUpdateSchemaToCase25538_Part_2

}//namespace ChemSW.Nbt.Schema