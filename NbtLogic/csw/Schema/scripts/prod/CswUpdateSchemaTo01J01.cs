
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-01
    /// </summary>
    public class CswUpdateSchemaTo01J01 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 01 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            // This script is reserved for schema changes, 
            // such as adding tables or columns, 
            // which need to take place before any other changes can be made.


            // case 20970 - Add 'locked' column to nodes
            // These are in 01J-02 as well
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodes", "locked" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes", "locked", "Prevents access to a node, for nodes beyond subscription level", false, false );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodes_audit", "locked" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes_audit", "locked", "Prevents access to a node, for nodes beyond subscription level", false, false );
            }

            // case 20970 - Add 'quota' column to object_class
            // This is in 01J-02 as well
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class", "quota" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class", "quota", "Sets the subscription count for nodes of this object class", false, false );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_audit", "quota" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class_audit", "quota", "Sets the subscription count for nodes of this object class", false, false );
            }

        }//Update()

    }//class CswUpdateSchemaTo01J01

}//namespace ChemSW.Nbt.Schema


