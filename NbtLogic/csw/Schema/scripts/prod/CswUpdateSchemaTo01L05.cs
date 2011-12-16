using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-05
    /// </summary>
    public class CswUpdateSchemaTo01L05 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 05 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24242

            _CswNbtSchemaModTrnsctn.changeColumnDataType( "configuration_variables", "variablename", DataDictionaryPortableDataType.String, 100 );
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "configuration_variables", "variablevalue", DataDictionaryPortableDataType.String, 1000 );

            #endregion Case 24242

        }//Update()

    }//class CswUpdateSchemaTo01L05

}//namespace ChemSW.Nbt.Schema


