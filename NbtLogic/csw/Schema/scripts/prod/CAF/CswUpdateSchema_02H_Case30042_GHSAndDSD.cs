using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30042_GHSAndDSD : CswUpdateSchemaTo
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
            get { return "02H_Case30042_GHSAndDSD"; }
        }

        public override string Title
        {
            get { return "CAF Import - GHS and DSD"; }
        }

        public override void update()
        {
            #region GHS

            {
                CswNbtSchemaUpdateImportMgr JurisdictionMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "sites", "Jurisdiction", "regions_view", "region" );

                JurisdictionMgr.importBinding( "region", CswNbtObjClassJurisdiction.PropertyName.LegacyId, "" ); // Regions are distinct so we can use as the LegacyId
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
                CswNbtSchemaUpdateImportMgr GHSMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "jct_ghsphrase_matsite", "GHS", "ghs_view", "legacyid" );

                GHSMgr.importBinding( "legacyid", CswNbtObjClassGHS.PropertyName.LegacyId, "" );
                GHSMgr.importBinding( "region", CswNbtObjClassGHS.PropertyName.Jurisdiction, "" );
                GHSMgr.importBinding( "materialid", CswNbtObjClassGHS.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
                GHSMgr.importBinding( "ghscodes", CswNbtObjClassGHS.PropertyName.AddLabelCodes, "" );
                GHSMgr.importBinding( "pictos", CswNbtObjClassGHS.PropertyName.Pictograms, "" );
                GHSMgr.importBinding( "signal", CswNbtObjClassGHS.PropertyName.SignalWord, "" );

                GHSMgr.finalize();
            }

            #endregion GHS

            #region DSD

            {
                CswNbtSchemaUpdateImportMgr DSDPhrasesMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "rs_phrases", "DSD Phrase" );

                DSDPhrasesMgr.importBinding( "rsphraseid", CswNbtObjClassDSDPhrase.PropertyName.LegacyId, "" );
                DSDPhrasesMgr.importBinding( "code", CswNbtObjClassDSDPhrase.PropertyName.Code, "" );
                DSDPhrasesMgr.importBinding( "phraseenglish", CswNbtObjClassDSDPhrase.PropertyName.English, "" );
                DSDPhrasesMgr.importBinding( "phrasedanish", CswNbtObjClassDSDPhrase.PropertyName.Danish, "" );
                DSDPhrasesMgr.importBinding( "phrasedutch", CswNbtObjClassDSDPhrase.PropertyName.Dutch, "" );
                DSDPhrasesMgr.importBinding( "phrasefinnish", CswNbtObjClassDSDPhrase.PropertyName.Finnish, "" );
                DSDPhrasesMgr.importBinding( "phrasefrench", CswNbtObjClassDSDPhrase.PropertyName.French, "" );
                DSDPhrasesMgr.importBinding( "phrasegerman", CswNbtObjClassDSDPhrase.PropertyName.German, "" );
                DSDPhrasesMgr.importBinding( "phraseitalian", CswNbtObjClassDSDPhrase.PropertyName.Italian, "" );
                DSDPhrasesMgr.importBinding( "phraseportuguese", CswNbtObjClassDSDPhrase.PropertyName.Portuguese, "" );
                DSDPhrasesMgr.importBinding( "phrasespanish", CswNbtObjClassDSDPhrase.PropertyName.Spanish, "" );
                DSDPhrasesMgr.importBinding( "phraseswedish", CswNbtObjClassDSDPhrase.PropertyName.Swedish, "" );
                DSDPhrasesMgr.importBinding( "phrasechinese", CswNbtObjClassDSDPhrase.PropertyName.Chinese, "" );

                DSDPhrasesMgr.finalize();
            }

            #endregion DSD

        }//update()
    }
}//namespace ChemSW.Nbt.Schema