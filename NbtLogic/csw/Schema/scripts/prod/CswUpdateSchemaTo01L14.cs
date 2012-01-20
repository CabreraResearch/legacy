using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-14
    /// </summary>
    public class CswUpdateSchemaTo01L14 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 14 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24394

            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp TypeOcp = MailReportOc.getObjectClassProp( CswNbtObjClassMailReport.TypePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, CswNbtSubField.SubFieldName.Value, "View" );

            #endregion Case 24394

        }//Update()

    }//class CswUpdateSchemaTo01L14

}//namespace ChemSW.Nbt.Schema


