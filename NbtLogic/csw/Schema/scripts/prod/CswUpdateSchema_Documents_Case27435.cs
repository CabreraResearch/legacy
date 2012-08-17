using ChemSW.Core;
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
                ListOptions = "en,fr,es,de",
                SetValOnAdd = false
                //This worked for Size on Request Item but does not seem to work when making the prop conditional on another prop of the same Object Class
                //IsFk = true,
                //FkType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                //FkValue = DocumentClassOcp.PropId,
                //FilterPropId = DocumentClassOcp.PropId,
                //Filter = CswNbtMetaDataObjectClassProp.makeFilter( DocumentClassOcp.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.MSDS )
            } );

            CswNbtMetaDataObjectClassProp FormatOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocumentOc )
            {
                PropName = CswNbtObjClassDocument.PropertyName.Format,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = CswNbtObjClassDocument.Formats.Options.ToString(),
                SetValOnAdd = false
                //IsFk = true,
                //FkType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                //FkValue = DocumentClassOcp.PropId,
                //FilterPropId = DocumentClassOcp.PropId,
                //Filter = CswNbtMetaDataObjectClassProp.makeFilter( DocumentClassOcp.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.MSDS )
            } );

            foreach( CswNbtMetaDataNodeType DocumentNt in DocumentOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp FormatNtp = DocumentNt.getNodeTypePropByObjectClassProp( FormatOcp.PropId );
                CswNbtMetaDataNodeTypeProp LanguageNtp = DocumentNt.getNodeTypePropByObjectClassProp( LanguageOcp.PropId );
                if( false == DocumentNt.NodeTypeName.Contains( "material" ) && false == DocumentNt.NodeTypeName.Contains( "Material" ) )
                {
                    FormatNtp.removeFromAllLayouts();
                    LanguageNtp.removeFromAllLayouts();
                }
                CswNbtMetaDataNodeTypeProp DocumentClassNtp = DocumentNt.getNodeTypePropByObjectClassProp( DocumentClassOcp.PropId );
                FormatNtp.setFilter( DocumentClassNtp, DocumentClassNtp.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.MSDS );
                LanguageNtp.setFilter( DocumentClassNtp, DocumentClassNtp.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.MSDS );
            }

            CswNbtMetaDataObjectClassProp LinkOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
            CswNbtMetaDataObjectClassProp FileOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
            CswNbtMetaDataObjectClassProp FileTypeOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.FileType );
            CswNbtMetaDataObjectClassProp ArchivedOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
            CswNbtMetaDataObjectClassProp AcquiredDateOcp = DocumentOc.getObjectClassProp( CswNbtObjClassDocument.PropertyName.AcquiredDate );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LinkOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FileOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FileTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AcquiredDateOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FileTypeOcp, FileTypeOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtObjClassDocument.FileTypes.File );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ArchivedOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ArchivedOcp, ArchivedOcp.getFieldTypeRule().SubFields.Default.Name, Tristate.False );

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema