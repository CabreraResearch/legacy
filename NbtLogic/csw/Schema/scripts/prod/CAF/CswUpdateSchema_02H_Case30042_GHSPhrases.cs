using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30042_GHSPhrases : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30042; }
        }

        public override string ScriptName
        {
            get { return "02H_Case30042_GHSPhrases"; }
        }

        public override string Title
        {
            get { return "CAF Import - GHS Phrases"; }
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr GHSPhrasesMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "ghs_phrases", "GHS Phrase" );

            GHSPhrasesMgr.importBinding( "ghsphraseid", CswNbtObjClassGHSPhrase.PropertyName.LegacyId, "" );
            GHSPhrasesMgr.importBinding( "ghscategory", CswNbtObjClassGHSPhrase.PropertyName.Category, "" );
            GHSPhrasesMgr.importBinding( "ghscode", CswNbtObjClassGHSPhrase.PropertyName.Code, "" );
            GHSPhrasesMgr.importBinding( "phraseenglish", CswNbtObjClassGHSPhrase.PropertyName.English, "" );
            GHSPhrasesMgr.importBinding( "phrasedanish", CswNbtObjClassGHSPhrase.PropertyName.Danish, "" );
            GHSPhrasesMgr.importBinding( "phrasedutch", CswNbtObjClassGHSPhrase.PropertyName.Dutch, "" );
            GHSPhrasesMgr.importBinding( "phrasefinnish", CswNbtObjClassGHSPhrase.PropertyName.Finnish, "" );
            GHSPhrasesMgr.importBinding( "phrasefrench", CswNbtObjClassGHSPhrase.PropertyName.French, "" );
            GHSPhrasesMgr.importBinding( "phrasegerman", CswNbtObjClassGHSPhrase.PropertyName.German, "" );
            GHSPhrasesMgr.importBinding( "phraseitalian", CswNbtObjClassGHSPhrase.PropertyName.Italian, "" );
            GHSPhrasesMgr.importBinding( "phraseportuguese", CswNbtObjClassGHSPhrase.PropertyName.Portuguese, "" );
            GHSPhrasesMgr.importBinding( "phrasespanish", CswNbtObjClassGHSPhrase.PropertyName.Spanish, "" );
            GHSPhrasesMgr.importBinding( "phraseswedish", CswNbtObjClassGHSPhrase.PropertyName.Swedish, "" );
            GHSPhrasesMgr.importBinding( "phrasechinese", CswNbtObjClassGHSPhrase.PropertyName.Chinese, "" );

            GHSPhrasesMgr.finalize();

        }//update()
    }
}//namespace ChemSW.Nbt.Schema