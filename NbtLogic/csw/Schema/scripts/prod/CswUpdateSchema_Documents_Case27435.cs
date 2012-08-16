using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27435
    /// </summary>
    public class CswUpdateSchema_Documents_Case27435 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Do the update
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass DocumentOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass );

            CswNbtMetaDataObjectClassProp ArchivedDateOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOc )
                {
                    PropName = CswNbtObjClassDocument.PropertyName.ArchiveDate,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime
                } );

            CswNbtMetaDataObjectClassProp DocumentClassOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );

            CswNbtMetaDataObjectClassProp LanguageOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOc )
            {
                PropName = CswNbtObjClassDocument.PropertyName.Language,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "en,fr,es,de"
            } );

            CswNbtMetaDataObjectClassProp FormatOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOc )
            {
                PropName = CswNbtObjClassDocument.PropertyName.Format,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = CswNbtObjClassDocument.Formats.Options.ToString()
            } );

            CswNbtMetaDataObjectClassProp LinkOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
            CswNbtMetaDataObjectClassProp FileOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
            CswNbtMetaDataObjectClassProp FileTypeOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.FileType );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LinkOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FileOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FileTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FileTypeOcp, FileTypeOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtObjClassDocument.FileTypes.File );

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema