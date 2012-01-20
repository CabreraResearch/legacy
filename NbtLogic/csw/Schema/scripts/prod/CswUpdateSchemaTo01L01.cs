


using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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

            #region Case 24086

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", "quota" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "nodetypes", "quota", "NodeType Quota", false, false );
            }

            #endregion Case 24656

            #region Case 24434

            CswNbtMetaDataObjectClass AssemblyOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );

            CswNbtMetaDataObjectClassProp AssemblyPartsOcp = AssemblyOc.getObjectClassProp( CswNbtObjClassEquipmentAssembly.AssemblyPartsPropertyName );
            if( null == AssemblyPartsOcp )
            {
                AssemblyPartsOcp = AssemblyOc.getObjectClassProp( "Parts" );
                if( null != AssemblyPartsOcp )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AssemblyPartsOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, CswNbtObjClassEquipmentAssembly.AssemblyPartsPropertyName );
                }
            }

            #endregion Case 24434

        }//Update()

    }//class CswUpdateSchemaTo01K01

}//namespace ChemSW.Nbt.Schema


