using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-13
    /// </summary>
    public class CswUpdateSchemaTo01L13 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 13 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24086

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", "quota" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "nodetypes", "quota", "NodeType Quota", false, false );
            }

            #endregion Case 24656



        }//Update()

    }//class CswUpdateSchemaTo01L13

}//namespace ChemSW.Nbt.Schema


