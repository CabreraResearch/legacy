using System.Collections.Generic;
using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30537C: CswUpdateSchemaTo
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
            get { return "02G_Case" + CaseNo + "C"; }
        }

        public override string Title
        {
            get { return "Add DSD Phrases, DSD Tab, DSD Module"; }
        }

        public override void update()
        {
            #region Create SD Phrases

            #region Risk Phrases
            Dictionary<string, string> RiskPhrases = new Dictionary<string, string>()
                {
                    {"R0",	"no code"},
                    {"R1",	"Explosive when dry."},
                    {"R10",	"Flammable."},
                    {"R11",	"Highly flammable."},
                    {"R12",	"Extremely flammable."},
                    {"R14",	"Reacts violently with water."},
                    {"R14/15",	"Reacts violently with water, liberating extremely flammable gases."},
                    {"R15",	"Contact with water liberates extremely flammable gases."},
                    {"R15/29",	"Contact with water liberates toxic, extremely flammable gas."},
                    {"R16",	"Explosive when mixed with oxidising substances."},
                    {"R17",	"Spontaneously flammable in air."},
                    {"R18",	"In use, may form flammable/explosive vapour-air mixture."},
                    {"R19",	"May form explosive peroxides."},
                    {"R2",	"Risk of explosion by shock, friction, fire or other sources of ignition."},
                    {"R20",	"Harmful by inhalation."},
                    {"R20/21",	"Harmful by inhalation and in contact with skin."},
                    {"R20/21/22",	"Harmful by inhalation, in contact with skin and if swallowed."},
                    {"R20/22",	"Harmful by inhalation and if swallowed."},
                    {"R21",	"Harmful in contact with skin."},
                    {"R21/22",	"Harmful in contact with skin and if swallowed."},
                    {"R22",	"Harmful if swallowed."},
                    {"R23",	"Toxic by inhalation."},
                    {"R23/24",	"Toxic by inhalation and in contact with skin."},
                    {"R23/24/25",	"Toxic by inhalation, in contact with skin and if swallowed."},
                    {"R23/25",	"Toxic by inhalation and if swallowed."},
                    {"R24",	"Toxic in contact with skin."},
                    {"R24/25",	"Toxic in contact with skin and if swallowed."},
                    {"R25",	"Toxic if swallowed."},
                    {"R26",	"Very toxic by inhalation."},
                    {"R26/27",	"Very toxic by inhalation and in contact with skin."},
                    {"R26/27/28",	"Very toxic by inhalation, in contact with skin and if swallowed."},
                    {"R26/28",	"Very toxic by inhalation and if swallowed."},
                    {"R27",	"Very toxic in contact with skin."},
                    {"R27/28",	"Very toxic in contact with skin and if swallowed."},
                    {"R28",	"Very toxic if swallowed."},
                    {"R29",	"Contact with water liberates toxic gas."},
                    {"R3",	"Extreme risk of explosion by shock, friction, fire or other sources of ignition."},
                    {"R30",	"Can become highly flammable in use."},
                    {"R31",	"Contact with acids liberates toxic gas."},
                    {"R32",	"Contact with acids liberates very toxic gas."},
                    {"R33",	"Danger of cumulative effects."},
                    {"R34",	"Causes burns."},
                    {"R35",	"Causes severe burns."},
                    {"R36",	"Irritating to eyes."},
                    {"R36/37",	"Irritating to eyes and respiratory system."},
                    {"R36/37/38",	"Irritating to eyes, respiratory system and skin."},
                    {"R36/38",	"Irritating to eyes and skin."},
                    {"R37",	"Irritating to respiratory system."},
                    {"R37/38",	"Irritating to respiratory system and skin."},
                    {"R38",	"Irritating to skin."},
                    {"R39",	"Danger of very serious irreversible effects."},
                    {"R39/23",	"Toxic: danger of very serious irreversible effects through inhalation."},
                    {"R39/23/24",	"Toxic: danger of very serious irreversible effects through inhalation and in contact with skin."},
                    {"R39/23/24/25",	"Toxic: danger of very serious irreversible effects through inhalation, in contact with skin and if swallowed."},
                    {"R39/23/25",	"Toxic: danger of very serious irreversible effects through inhalation and if swallowed."},
                    {"R39/24",	"Toxic: danger of very serious irreversible effects in contact with skin."},
                    {"R39/24/25",	"Toxic: danger of very serious irreversible effects in contact with skin and if swallowed."},
                    {"R39/25",	"Toxic: danger of very serious irreversible effects if swallowed."},
                    {"R39/26",	"Very toxic: danger of very serious irreversible effects through inhalation."},
                    {"R39/26/27",	"Very toxic: danger of very serious irreversible effects through inhalation and in contact with skin."},
                    {"R39/26/27/28",	"Very toxic: danger of very serious irreversible effects through inhalation, in contact with skin and if swallowed."},
                    {"R39/26/28",	"Very toxic: danger of very serious irreversible effects through inhalation and if swallowed."},
                    {"R39/27",	"Very toxic: danger of very serious irreversible effects in contact with skin."},
                    {"R39/27/28",	"Very toxic: danger of very serious irreversible effects in contact with skin and if swallowed."},
                    {"R39/28",	"Very toxic: danger of very serious irreversible effects if swallowed."},
                    {"R4",	"Forms very sensitive explosive metallic compounds."},
                    {"R40",	"Limited evidence of a carcinogenic effect."},
                    {"R41",	"Risk of serious damage to eyes."},
                    {"R42",	"May cause sensitisation by inhalation."},
                    {"R42/43",	"May cause sensitisation by inhalation and skin contact."},
                    {"R43",	"May cause sensitisation by skin contact."},
                    {"R44",	"Risk of explosion if heated under confinement."},
                    {"R45",	"May cause cancer."},
                    {"R46",	"May cause heritable genetic damage."},
                    {"R48",	"Danger of serious damage to health by prolonged exposure."},
                    {"R48/20",	"Harmful: danger of serious damage to health by prolonged exposure through inhalation."},
                    {"R48/20/21",	"Harmful: danger of serious damage to health by prolonged exposure through inhalation and in contact with skin."},
                    {"R48/20/21/22",	"Harmful: danger of serious damage to health by prolonged exposure through inhalation, in contact with skin and if swallowed."},
                    {"R48/20/22",	"Harmful: danger of serious damage to health by prolonged exposure through inhalation and if swallowed."},
                    {"R48/21",	"Harmful: danger of serious damage to health by prolonged exposure in contact with skin."},
                    {"R48/21/22",	"Harmful: danger of serious damage to health by prolonged exposure in contact with skin and if swallowed."},
                    {"R48/22",	"Harmful: danger of serious damage to health by prolonged exposure if swallowed."},
                    {"R48/23",	"Toxic: danger of serious damage to health by prolonged exposure through inhalation."},
                    {"R48/23/24",	"Toxic: danger of serious damage to health by prolonged exposure through inhalation and in contact with skin."},
                    {"R48/23/24/25",	"Toxic: danger of serious damage to health by prolonged exposure through inhalation, in contact with skin and if swallowed."},
                    {"R48/23/25",	"Toxic: danger of serious damage to health by prolonged exposure through inhalation and if swallowed."},
                    {"R48/24",	"Toxic: danger of serious damage to health by prolonged exposure in contact with skin."},
                    {"R48/24/25",	"Toxic: danger of serious damage to health by prolonged exposure in contact with skin and if swallowed."},
                    {"R48/25",	"Toxic: danger of serious damage to health by prolonged exposure if swallowed."},
                    {"R49",	"May cause cancer by inhalation."},
                    {"R5",	"Heating may cause an explosion."},
                    {"R50",	"Very toxic to aquatic organisms."},
                    {"R50/53",	"Very toxic to aquatic organisms, may cause long-term adverse effects in the aquatic environment."},
                    {"R51",	"Toxic to aquatic organisms."},
                    {"R51/53",	"Toxic to aquatic organisms, may cause long-term adverse effects in the aquatic environment."},
                    {"R52",	"Harmful to aquatic organisms."},
                    {"R52/53",	"Harmful to aquatic organisms, may cause long-term adverse effects in the aquatic environment."},
                    {"R53",	"May cause long-term adverse effects in the aquatic environment."},
                    {"R54",	"Toxic to flora."},
                    {"R55",	"Toxic to fauna."},
                    {"R56",	"Toxic to soil organisms."},
                    {"R57",	"Toxic to bees."},
                    {"R58",	"May cause long-term adverse effects in the environment."},
                    {"R59",	"Dangerous for the ozone layer."},
                    {"R6",	"Explosive with or without contact with air."},
                    {"R60",	"May impair fertility."},
                    {"R61",	"May cause harm to the unborn child."},
                    {"R62",	"Possible risk of impaired fertility."},
                    {"R63",	"Possible risk of harm to the unborn child."},
                    {"R64",	"May cause harm to breastfed babies."},
                    {"R65",	"Harmful: may cause lung damage if swallowed."},
                    {"R66",	"Repeated exposure may cause skin dryness or cracking."},
                    {"R67",	"Vapours may cause drowsiness and dizziness."},
                    {"R68",	"Possible risk of irreversible effects."},
                    {"R68/20",	"Harmful: possible risk of irreversible effects through inhalation."},
                    {"R68/20/21",	"Harmful: possible risk of irreversible effects through inhalation and in contact with skin."},
                    {"R68/20/21/22",	"Harmful: possible risk of irreversible effects through inhalation, in contact with skin and if swallowed."},
                    {"R68/20/22",	"Harmful: possible risk of irreversible effects through inhalation and if swallowed."},
                    {"R68/21",	"Harmful: possible risk of irreversible effects in contact with skin."},
                    {"R68/21/22",	"Harmful: possible risk of irreversible effects in contact with skin and if swallowed."},
                    {"R68/22",	"Harmful: possible risk of irreversible effects if swallowed."},
                    {"R7",	"May cause fire."},
                    {"R8",	"Contact with combustible material may cause fire."},
                    {"R9",	"Explosive when mixed with combustible material."}
                };
            #endregion

            #region Safety Phrases

            Dictionary<string, string> SafetyPhrases = new Dictionary<string, string>()
                {
                    {"S1",	"Keep locked up."},
                    {"S1/2",	"Keep locked up and out of the reach of children."},
                    {"S12",	"Do not keep the container sealed."},
                    {"S13",	"Keep away from food, drink and animal feedingstuffs."},
                    {"S14",	"Keep away from . (incompatible materials to be indicated by the manufacturer)."},
                    {"S15",	"Keep away from heat."},
                    {"S16",	"Keep away from sources of ignition . No smoking."},
                    {"S17",	"Keep away from combustible material."},
                    {"S18",	"Handle and open container with care."},
                    {"S2",	"Keep out of the reach of children."},
                    {"S20",	"When using do not eat or drink."},
                    {"S20/21",	"When using do not eat, drink or smoke."},
                    {"S21",	"When using do not smoke."},
                    {"S22",	"Do not breathe dust."},
                    {"S23",	"Do not breathe gas/fumes/vapour/spray (appropriate wording to be specified by the manufacturer)."},
                    {"S24",	"Avoid contact with skin."},
                    {"S24/25",	"Avoid contact with skin and eyes."},
                    {"S25",	"Avoid contact with eyes."},
                    {"S26",	"In case of contact with eyes, rinse immediately with plenty of water and seek medical advice."},
                    {"S27",	"Take off immediately all contaminated clothing."},
                    {"S27/28",	"After contact with skin, take off immediately all contaminated clothing, and wash immediately with plenty of (to be specified by the manufacturer)."},
                    {"S28",	"After contact with skin, wash immediately with plenty of . (to be specified by the manufacturer)."},
                    {"S29",	"Do not empty into drains."},
                    {"S29/35",	"Do not empty into drains; dispose of this material and its container in a safe way."},
                    {"S29/56",	"Do not empty into drains, dispose of this material and its container at hazardous or special waste collection point."},
                    {"S3",	"Keep in a cool place."},
                    {"S3/14",	"Keep in a cool place away from . (incompatible materials to be indicated by the manufacturer)."},
                    {"S3/7",	"Keep container tightly closed in a cool place."},
                    {"S3/9/14",	"Keep in a cool, well-ventilated place away from . (incompatible materials to be indicated by the manufacturer)."},
                    {"S3/9/14/49",	"Keep only in the original container in a cool, well-ventilated place away from . (incompatible materials to be indicated by the manufacturer)."},
                    {"S3/9/49",	"Keep only in the original container in a cool, well-ventilated place."},
                    {"S30",	"Never add water to this product."},
                    {"S33",	"Take precautionary measures against static discharges."},
                    {"S35",	"This material and its container must be disposed of in a safe way."},
                    {"S36",	"Wear suitable protective clothing."},
                    {"S36/37",	"Wear suitable protective clothing and gloves."},
                    {"S36/37/39",	"Wear suitable protective clothing, gloves and eye/face protection."},
                    {"S36/39",	"Wear suitable protective clothing and eye/face protection."},
                    {"S37",	"Wear suitable gloves."},
                    {"S37/39",	"Wear suitable gloves and eye/face protection."},
                    {"S38",	"In case of insufficient ventilation, wear suitable respiratory equipment."},
                    {"S39",	"Wear eye/face protection."},
                    {"S4",	"Keep away from living quarters."},
                    {"S40",	"To clean the floor and all objects contaminated by this material, use . (to be specified by the manufacturer)."},
                    {"S41",	"In case of fire and/or explosion do not breathe fumes."},
                    {"S42",	"During fumigation/spraying wear suitable respiratory equipment (appropriate wording to be specified by the manufacturer)."},
                    {"S43",	"In case of fire, use . (indicate in the space the precise type of fire-fighting equipment. If water increases risk, add .Never use water.)."},
                    {"S45",	"In case of accident or if you feel unwell, seek medical advice immediately (show the label where possible)."},
                    {"S46",	"If swallowed, seek medical advice immediately and show this container or label."},
                    {"S47",	"Keep at temperature not exceeding . °C (to be specified by the manufacturer)."},
                    {"S47/49",	"Keep only in the original container at a temperature not exceeding . °C (to be specified by the manufacturer)."},
                    {"S48",	"Keep wet with . (appropriate material to be specified by the manufacturer)."},
                    {"S49",	"Keep only in the original container."},
                    {"S5",	"Keep contents under . (appropriate liquid to be specified by the manufacturer)."},
                    {"S50",	"Do not mix with . (to be specified by the manufacturer)."},
                    {"S51",	"Use only in well-ventilated areas."},
                    {"S52",	"Not recommended for interior use on large surface areas."},
                    {"S53",	"Avoid exposure . obtain special instructions before use."},
                    {"S56",	"Dispose of this material and its container to hazardous or special waste collection point."},
                    {"S57",	"Use appropriate container to avoid environmental contamination."},
                    {"S59",	"Refer to manufacturer/supplier for information on recovery/recycling."},
                    {"S6",	"Keep under . (inert gas to be specified by the manufacturer)."},
                    {"S60",	"This material and its container must be disposed of as hazardous waste."},
                    {"S61",	"Avoid release to the environment. Refer to special instructions/safety data sheets."},
                    {"S62",	"If swallowed, do not induce vomiting: seek medical advice immediately and show this container or label."},
                    {"S63",	"In case of accident by inhalation: remove casualty to fresh air and keep at rest."},
                    {"S64",	"If swallowed, rinse mouth with water (only if the person is conscious)."},
                    {"S7",	"Keep container tightly closed."},
                    {"S7/47",	"Keep container tightly closed and at a temperature not exceeding . °C (to be specified by the manufacturer)."},
                    {"S7/8",	"Keep container tightly closed and dry."},
                    {"S7/9",	"Keep container tightly closed and in a well-ventilated place."},
                    {"S8",	"Keep container dry."},
                    {"S9",	"Keep container in a well-ventilated place."}
                };

            #endregion

            CswNbtMetaDataObjectClass DSDPhraseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DSDPhraseClass );
            foreach( KeyValuePair<string, string> Phrase in RiskPhrases )
            {
                _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( DSDPhraseOC.getNodeTypeIds().FirstOrDefault(), OnAfterMakeNode : delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassDSDPhrase NewPhrase = NewNode;
                        NewPhrase.Code.Text = Phrase.Key;
                        NewPhrase.English.Text = Phrase.Value;
                        NewPhrase.Category.Value = "Risk";
                    } );
            }

            foreach( KeyValuePair<string, string> Phrase in SafetyPhrases )
            {
                _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( DSDPhraseOC.getNodeTypeIds().FirstOrDefault(), OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassDSDPhrase NewPhrase = NewNode;
                    NewPhrase.Code.Text = Phrase.Key;
                    NewPhrase.English.Text = Phrase.Value;
                    NewPhrase.Category.Value = "Safety";
                } );
            }

            #endregion

            #region DSD Tab

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab GHSTab = ChemicalNT.getNodeTypeTab( "GHS" );
                int DSDTabOrder = ( null != GHSTab ? GHSTab.TabOrder : ChemicalNT.getMaximumTabOrder() ) + 1;

                CswNbtMetaDataNodeTypeTab DSDTab = ChemicalNT.getNodeTypeTab( "DSD" ) ??
                    _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "DSD", DSDTabOrder );

                CswNbtMetaDataNodeTypeProp PictogramsNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.Pictograms );
                PictogramsNTP.removeFromAllLayouts();
                PictogramsNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, DSDTab.TabId );

                CswNbtMetaDataNodeTypeProp LabelCodesNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodes );
                LabelCodesNTP.removeFromAllLayouts();
                LabelCodesNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, DSDTab.TabId );

                CswNbtView LabelCodesView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( "DSD Label Codes Property Grid", CswEnumNbtViewVisibility.Property );

                CswNbtMetaDataNodeTypeProp LabelCodesGridNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodesGrid );
                LabelCodesGridNTP.ViewId = LabelCodesView.ViewId;
                LabelCodesGridNTP.Extended = "Small";
                LabelCodesGridNTP.removeFromAllLayouts();
                LabelCodesGridNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, DSDTab.TabId );
            }

            #endregion

            #region DSD Module

            int dsdModuleId = _CswNbtSchemaModTrnsctn.createModule( "Dangerous Substances Directive", CswEnumNbtModuleName.DSD, false );
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.CISPro, CswEnumNbtModuleName.DSD );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( dsdModuleId, DSDPhraseOC.ObjectClassId );

            #endregion

        } // update()

    }

}//namespace ChemSW.Nbt.Schema