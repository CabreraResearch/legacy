


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-01
    /// </summary>
    public class CswUpdateSchemaTo01L01 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 01 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 23641

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "valuepropid" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props", "valuepropid", "If the property values are derived from another table, tablecolid of column to save as foreign key", true, false, 20 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "valueproptype" ) )
            {
                _CswNbtSchemaModTrnsctn.addDoubleColumn( "object_class_props", "valueproptype", "If the property values are derived from another table, table reference to aid foreign key", true, false, 20 );
            }

            #endregion Case 23641
        }//Update()

    }//class CswUpdateSchemaTo01K01

}//namespace ChemSW.Nbt.Schema


