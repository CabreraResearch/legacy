using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
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
            {
                CswNbtSchemaUpdateImportMgr JurisdictionMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "sites", "Jurisdiction", "regions_view", "region" );

                JurisdictionMgr.importBinding( "region", CswNbtObjClassJurisdiction.PropertyName.LegacyId, "" );
                JurisdictionMgr.importBinding( "region", CswNbtObjClassJurisdiction.PropertyName.Name, "" );

                JurisdictionMgr.finalize();
            }

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
            }

            {
                CswNbtSchemaUpdateImportMgr GHSPhrasesMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "jct_ghsphrase_matsite", "GHS", "ghs_view", "legacyid" );

                GHSPhrasesMgr.importBinding( "legacyid", CswNbtObjClassGHS.PropertyName.LegacyId, "" );
                GHSPhrasesMgr.importBinding( "region", CswNbtObjClassGHS.PropertyName.Jurisdiction, "" );
                GHSPhrasesMgr.importBinding( "materialid", CswNbtObjClassGHS.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
                GHSPhrasesMgr.importBinding( "ghscodes", CswNbtObjClassGHS.PropertyName.AddLabelCodes, "" );
                GHSPhrasesMgr.importBinding( "pictos", CswNbtObjClassGHS.PropertyName.Pictograms, "" );

                GHSPhrasesMgr.finalize();
            }

        }//update()
    }
}//namespace ChemSW.Nbt.Schema