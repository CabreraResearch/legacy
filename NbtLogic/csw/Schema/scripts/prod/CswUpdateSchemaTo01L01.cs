

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
            CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );


            CswNbtMetaDataObjectClassProp NodeTypePermsOcp = RoleOc.getObjectClassProp( CswNbtObjClassRole.NodeTypePermissionsPropertyName );
            CswNbtMetaDataFieldType NodeTypePermFt = _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswNbtMetaDataFieldType.NbtFieldType.NodeTypePermissions, CswNbtMetaDataFieldType.DataType.TEXT );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NodeTypePermsOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, NodeTypePermFt.FieldTypeId );

            #endregion Case 23641
        }//Update()

    }//class CswUpdateSchemaTo01K01

}//namespace ChemSW.Nbt.Schema


