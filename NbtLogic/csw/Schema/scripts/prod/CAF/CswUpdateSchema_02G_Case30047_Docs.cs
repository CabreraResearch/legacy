using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30047_Docs: CswUpdateSchemaTo
    {
        public override string Title { get { return "Setup Material Document import bindings"; } }

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
            get { return "Case30743_Docs"; }
        }

        public override void update()
        {
            // CAF bindings definitions for Vendors
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "documents", "Material Document", ViewName : "docs_view", SourceColumn : "legacyid" );

            //simple props
            ImpMgr.importBinding( "acquisitiondate", CswNbtObjClassDocument.PropertyName.AcquiredDate, "" );
            ImpMgr.importBinding( "description", CswNbtObjClassDocument.PropertyName.Title, "" );

            //relationships
            ImpMgr.importBinding( "packageid", "Material", CswEnumNbtSubFieldName.NodeID.ToString() ); //Material Document NTP "Owner" is changed to "Material"

            //transformed props
            ImpMgr.importBinding( "fileextension_trans", CswNbtObjClassDocument.PropertyName.FileType, "" );

            //file specific bindings
            ImpMgr.importBinding( "content_type", CswNbtObjClassSDSDocument.PropertyName.File, CswEnumNbtSubFieldName.ContentType.ToString() );
            ImpMgr.importBinding( "filename", CswNbtObjClassDocument.PropertyName.File, CswEnumNbtSubFieldName.Name.ToString() );

            //Link and BlobData are stored in the same column, we're going to import it twice and let the "FileType" property dictate what is shown
            ImpMgr.importBinding( "document", CswNbtObjClassDocument.PropertyName.File, CswEnumNbtSubFieldName.Blob.ToString(), BlobTableName : "documents", LobDataPkColOverride : "legacyid" );
            ImpMgr.importBinding( "document", CswNbtObjClassDocument.PropertyName.Link, CswEnumNbtSubFieldName.Href.ToString(), BlobTableName : "documents", LobDataPkColOverride : "legacyid" );

            //Use the url as the text that displays for links
            ImpMgr.importBinding( "description", CswNbtObjClassDocument.PropertyName.Link, CswEnumNbtSubFieldName.Text.ToString() );

            //Legacy Id for documents is "<documentid>_<packageid>" (ex: "123_343")
            ImpMgr.importBinding( "legacyid", "Legacy Id", "" );

            ImpMgr.finalize();

        }
    }
}