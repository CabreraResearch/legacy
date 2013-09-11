using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02G_Case28493A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28493; }
        }

        public override string ScriptName
        {
            get { return "02G_Case28493A"; }
        }

        public override void update()
        {
            //Add the missing Languages to the GHS Phrase obj class
            CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            foreach( string language in CswNbtObjClassGHSPhrase.SupportedLanguages.All )
            {
                _createLanguageProp( GhsPhraseOC, language );
            }
            
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "jct_nodes_props", "field1", CswEnumDataDictionaryPortableDataType.String, 500 );

        } // update()

        private void _createLanguageProp( CswNbtMetaDataObjectClass GhsPhraseOC, string OCPropName )
        {
            CswNbtMetaDataObjectClassProp ocp = GhsPhraseOC.getObjectClassProp( OCPropName ) ??
            _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsPhraseOC, new CswNbtWcfMetaDataModel.ObjectClassProp( GhsPhraseOC )
                {
                    PropName = OCPropName,
                    FieldType = CswEnumNbtFieldType.Text
                } );


        }

    }

}//namespace ChemSW.Nbt.Schema