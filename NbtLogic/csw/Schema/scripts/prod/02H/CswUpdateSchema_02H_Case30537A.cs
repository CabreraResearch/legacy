using ChemSW.Core;
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
            get { return "Create DSD Phrase ObjClass and DSD Chemical Props"; }
        }

        public override void update()
        {
            #region Create DSD Phrase OC

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

            #endregion

            #region Create DSD Chemical Props

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );

            const string _pre = "Images/cispro/DSD/";
            CswDelimitedString dsdImgs = new CswDelimitedString( '\n' )
                {
                    _pre + "none.gif",
                    _pre + "e.gif",
                    _pre + "o.gif",
                    _pre + "f.gif",
                    _pre + "f_plus.gif",
                    _pre + "t.gif",
                    _pre + "t_plus.gif",
                    _pre + "xn.gif",
                    _pre + "xi.gif",
                    _pre + "c.gif",
                    _pre + "n.gif"
                };

            CswDelimitedString dsdValues = new CswDelimitedString( '\n' )
                {
                    "None","Explosive","Oxidizing","Highly flammable","Extremely flammable","Toxic","Very Toxic","Harmful","Irritant","Corrosive","Dangerous for the environment"
                };

            CswNbtMetaDataObjectClassProp PictorgramsOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.Pictograms ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                    {
                        PropName = CswNbtObjClassChemical.PropertyName.Pictograms,
                        FieldType = CswEnumNbtFieldType.ImageList,
                        ListOptions = dsdValues.ToString(),
                        ValueOptions = dsdImgs.ToString(),
                        Extended = true.ToString()
                    } );

            CswNbtMetaDataObjectClassProp LabelCodesOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodes ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                    {
                        PropName = CswNbtObjClassChemical.PropertyName.LabelCodes,
                        FieldType = CswEnumNbtFieldType.MultiList,
                        ListOptions = "" //Intentionally empty - will be dynamic
                    } );

            CswNbtView DSDLabelCodesView = _CswNbtSchemaModTrnsctn.makeSafeView( "DSD Label Codes Property Grid", CswEnumNbtViewVisibility.Hidden );
            DSDLabelCodesView.SetViewMode( CswEnumNbtViewRenderingMode.Grid );
            DSDLabelCodesView.AddViewRelationship( DSDPhraseOC, false );
            DSDLabelCodesView.save();

            CswNbtMetaDataObjectClassProp LabelCodesGridOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodesGrid ) ??
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.LabelCodesGrid,
                    FieldType = CswEnumNbtFieldType.Grid,
                    ViewXml = DSDLabelCodesView.ToString(),
                    Extended = "Small"
                } );

            #endregion

        } // update()

    }

}//namespace ChemSW.Nbt.Schema