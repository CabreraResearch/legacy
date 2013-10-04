using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30537A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30537; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo + "A"; }
        }

        public override string Title
        {
            get { return "Create DSD Phrase ObjClass"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass DSDPhraseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DSDPhraseClass );
            if( null == DSDPhraseOC )
            {
                DSDPhraseOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DSDPhraseClass, "warning.png", false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( DSDPhraseOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DSDPhraseOC )
                    {
                        PropName = CswNbtObjClassDSDPhrase.PropertyName.Code,
                        FieldType = CswEnumNbtFieldType.Text
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( DSDPhraseOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DSDPhraseOC )
                    {
                        PropName = CswNbtObjClassDSDPhrase.PropertyName.Category,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = "Risk,Safety,Hazard"
                    } );

                foreach( string Language in CswNbtPropertySetPhrase.SupportedLanguages.All )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( DSDPhraseOC, new CswNbtWcfMetaDataModel.ObjectClassProp( DSDPhraseOC )
                        {
                            PropName = Language,
                            FieldType = CswEnumNbtFieldType.Text
                        } );
                }

            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema