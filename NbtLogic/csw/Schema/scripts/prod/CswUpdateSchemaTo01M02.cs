
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-02
    /// </summary>
    public class CswUpdateSchemaTo01M02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 02 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

          
            #region //case 22962
            _CswNbtSchemaModTrnsctn.addTable( "field_types_subfields", "ftsubfieldid" );
            _CswNbtSchemaModTrnsctn.addLongColumn( "field_types_subfields", "fieldtypeid", "FK to field_types", true, true );
            _CswNbtSchemaModTrnsctn.addStringColumn( "field_types_subfields", "propcolname", "name of storage column in jct_node_props", true, true, 20 );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "field_types_subfields", "reportable", "whether to include auto-generated views", true, true );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "field_types_subfields", "is_default", "this field gets no subfield suffix in the view", true, true );
            _CswNbtSchemaModTrnsctn.addStringColumn( "field_types_subfields", "subfieldname", "suffix on this propertyname in the view", true, false, 20 );
            #endregion


        }//Update()

    }//class CswUpdateSchemaTo01M02

}//namespace ChemSW.Nbt.Schema