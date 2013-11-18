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

        public override string AppendToScriptName()
        {
            return "GHSAndDSD";
        }

        public override string Title
        {
            get { return "CAF Import - GHS and DSD"; }
        }

        public override void update()
        {
            #region GHS

                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

                ImpMgr.CAFimportOrder( "Jurisdiction", "sites", "regions_view", "region", false );

                ImpMgr.importBinding( "region", CswNbtObjClassJurisdiction.PropertyName.LegacyId, "" ); // Regions are distinct so we can use as the LegacyId
                ImpMgr.importBinding( "region", CswNbtObjClassJurisdiction.PropertyName.Name, "" );


                ImpMgr.CAFimportOrder( "GHS Phrase", "ghs_phrases", PkColumnName: "ghsphraseid" );

                ImpMgr.importBinding( "ghscategory", CswNbtObjClassGHSPhrase.PropertyName.Category, "" );
                ImpMgr.importBinding( "ghscode", CswNbtObjClassGHSPhrase.PropertyName.Code, "" );
                ImpMgr.importBinding( "phraseenglish", CswNbtObjClassGHSPhrase.PropertyName.English, "" );
                ImpMgr.importBinding( "phrasedanish", CswNbtObjClassGHSPhrase.PropertyName.Danish, "" );
                ImpMgr.importBinding( "phrasedutch", CswNbtObjClassGHSPhrase.PropertyName.Dutch, "" );
                ImpMgr.importBinding( "phrasefinnish", CswNbtObjClassGHSPhrase.PropertyName.Finnish, "" );
                ImpMgr.importBinding( "phrasefrench", CswNbtObjClassGHSPhrase.PropertyName.French, "" );
                ImpMgr.importBinding( "phrasegerman", CswNbtObjClassGHSPhrase.PropertyName.German, "" );
                ImpMgr.importBinding( "phraseitalian", CswNbtObjClassGHSPhrase.PropertyName.Italian, "" );
                ImpMgr.importBinding( "phraseportuguese", CswNbtObjClassGHSPhrase.PropertyName.Portuguese, "" );
                ImpMgr.importBinding( "phrasespanish", CswNbtObjClassGHSPhrase.PropertyName.Spanish, "" );
                ImpMgr.importBinding( "phraseswedish", CswNbtObjClassGHSPhrase.PropertyName.Swedish, "" );
                ImpMgr.importBinding( "phrasechinese", CswNbtObjClassGHSPhrase.PropertyName.Chinese, "" );



                ImpMgr.CAFimportOrder( "GHS", "jct_ghsphrase_matsite", "ghs_view", "legacyid", false );

                ImpMgr.importBinding( "legacyid", CswNbtObjClassGHS.PropertyName.LegacyId, "" );
                ImpMgr.importBinding( "region", CswNbtObjClassGHS.PropertyName.Jurisdiction, "" );
                ImpMgr.importBinding( "packageid", CswNbtObjClassGHS.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
                ImpMgr.importBinding( "ghscodes", CswNbtObjClassGHS.PropertyName.AddLabelCodes, "" );
                ImpMgr.importBinding( "pictos", CswNbtObjClassGHS.PropertyName.Pictograms, "" );
                ImpMgr.importBinding( "signal", CswNbtObjClassGHS.PropertyName.SignalWord, "" );

            #endregion GHS

            #region DSD


                ImpMgr.CAFimportOrder( "DSD Phrase", "rs_phrases", PkColumnName: "rsphraseid" );

                ImpMgr.importBinding( "code", CswNbtObjClassDSDPhrase.PropertyName.Code, "" );
                ImpMgr.importBinding( "phraseenglish", CswNbtObjClassDSDPhrase.PropertyName.English, "" );
                ImpMgr.importBinding( "phrasedanish", CswNbtObjClassDSDPhrase.PropertyName.Danish, "" );
                ImpMgr.importBinding( "phrasedutch", CswNbtObjClassDSDPhrase.PropertyName.Dutch, "" );
                ImpMgr.importBinding( "phrasefinnish", CswNbtObjClassDSDPhrase.PropertyName.Finnish, "" );
                ImpMgr.importBinding( "phrasefrench", CswNbtObjClassDSDPhrase.PropertyName.French, "" );
                ImpMgr.importBinding( "phrasegerman", CswNbtObjClassDSDPhrase.PropertyName.German, "" );
                ImpMgr.importBinding( "phraseitalian", CswNbtObjClassDSDPhrase.PropertyName.Italian, "" );
                ImpMgr.importBinding( "phraseportuguese", CswNbtObjClassDSDPhrase.PropertyName.Portuguese, "" );
                ImpMgr.importBinding( "phrasespanish", CswNbtObjClassDSDPhrase.PropertyName.Spanish, "" );
                ImpMgr.importBinding( "phraseswedish", CswNbtObjClassDSDPhrase.PropertyName.Swedish, "" );
                ImpMgr.importBinding( "phrasechinese", CswNbtObjClassDSDPhrase.PropertyName.Chinese, "" );


            #endregion DSD


            ImpMgr.finalize();


        }//update()
    }
}//namespace ChemSW.Nbt.Schema