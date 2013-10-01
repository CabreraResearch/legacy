using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30047_SDS: CswUpdateSchemaTo
    {
        public override string Title { get { return "Setup SDS import bindings"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30047; }
        }

        public override string ScriptName
        {
            get { return "Case30743_SDS"; }
        }

        public override void update()
        {
            // CAF bindings definitions for Vendors
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "documents", "SDS Document", ViewName : "sds_view", SourceColumn : "legacyid" );

            //simple props
            ImpMgr.importBinding( "acquisitiondate", CswNbtObjClassSDSDocument.PropertyName.AcquiredDate, "" );
            ImpMgr.importBinding( "description", CswNbtObjClassSDSDocument.PropertyName.Title, "" );
            ImpMgr.importBinding( "captureddate", CswNbtObjClassSDSDocument.PropertyName.RevisionDate, "" );

            //relationships
            ImpMgr.importBinding( "packageid", "Material", CswEnumNbtSubFieldName.NodeID.ToString() ); //SDS Document NTP "Owner" is changed to "Material"

            //transformed props
            ImpMgr.importBinding( "language_trans", CswNbtObjClassSDSDocument.PropertyName.Language, "" );
            ImpMgr.importBinding( "fileextension_trans", CswNbtObjClassSDSDocument.PropertyName.FileType, "" );

            //file specific bindings
            ImpMgr.importBinding( "content_type", CswNbtObjClassSDSDocument.PropertyName.File, CswEnumNbtSubFieldName.ContentType.ToString() );
            ImpMgr.importBinding( "filename", CswNbtObjClassSDSDocument.PropertyName.File, CswEnumNbtSubFieldName.Name.ToString() );

            //Link and BlobData are stored in the same column, we're going to import it twice and let the "FileType" property dictate what is shown
            ImpMgr.importBinding( "document", CswNbtObjClassSDSDocument.PropertyName.File, CswEnumNbtSubFieldName.Blob.ToString(), BlobTableName : "documents", LobDataPkColOverride : "documentid" );
            ImpMgr.importBinding( "document", CswNbtObjClassSDSDocument.PropertyName.Link, CswEnumNbtSubFieldName.Href.ToString(), BlobTableName : "documents", LobDataPkColOverride : "documentid" );

            //Use the url as the text that displays for links
            ImpMgr.importBinding( "document", CswNbtObjClassSDSDocument.PropertyName.Link, CswEnumNbtSubFieldName.Text.ToString(), BlobTableName : "documents", LobDataPkColOverride : "documentid" );

            //Legacy Id for documents is "<documentid>_<packageid>" (ex: "123_343")
            ImpMgr.importBinding( "legacyid", "Legacy Id", "" );

            ImpMgr.finalize();

        }
    }
}