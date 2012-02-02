
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-01
    /// </summary>
    public class CswUpdateSchemaTo01M01 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 03 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

          
            #region //case 22962
            
//            _CswNbtSchemaModTrnsctn.addTable( "field_types_subfields", "ftsubfieldid" );
//            _CswNbtSchemaModTrnsctn.addLongColumn( "field_types_subfielod", "fieldtypeid", "FK to field_types", true, true );
            #endregion


        }//Update()

    }//class CswUpdateSchemaTo01M03

}//namespace ChemSW.Nbt.Schema