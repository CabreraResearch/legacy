
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-01
    /// </summary>
    public class CswUpdateSchemaTo01M01 : CswUpdateSchemaTo
    {
//        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 01 ); } }
//        public override string Description { set { ; } get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            #region case 24481
            
            // Also in 01M-06
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "field_types", "searchable" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "field_types", "searchable", "Whether the field type is searchable", false, true );
            }

            #endregion case 24481

            #region case 25322

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_props", "usenumbering" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class_props", "usenumbering", "Whether the property should be numbered", false, false );
            }

            #endregion case 25322

            #region case 24520

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "audit_transactions", "transactionfirstname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "audit_transactions", "transactionfirstname", "First name of transaction user", false, false, 50 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "audit_transactions", "transactionlastname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "audit_transactions", "transactionlastname", "Last name of transaction user", false, false, 50 );
            }

            #endregion case 24520


        }//Update()

    }//class CswUpdateSchemaTo01M01

}//namespace ChemSW.Nbt.Schema