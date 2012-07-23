using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25291
    /// </summary>
    public class CswUpdateSchemaCase25291GHS : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass GenericOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass );
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

            CswNbtMetaDataFieldType TextFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Text );
            CswNbtMetaDataFieldType ListFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.List );
            CswNbtMetaDataFieldType GridFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid );
            CswNbtMetaDataFieldType ImageListFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ImageList );
            CswNbtMetaDataFieldType RelationshipFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Relationship );


            //// Jurisdiction NodeType
            //CswNbtMetaDataNodeType JurisdictionNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( GenericOC )
            //{
            //    NodeTypeName = "Jurisdiction",
            //    Category = "System",
            //    IconFileName = "globe.gif"
            //} );
            //CswNbtMetaDataNodeTypeProp JurisdictionNameNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( JurisdictionNT, TextFT, "Name" ) );
            //JurisdictionNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "Name" ) );

            //// Default Jurisdictions
            //CswNbtNode JurisNorthAmericaNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( JurisdictionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            //JurisNorthAmericaNode.Properties[JurisdictionNameNTP].AsText.Text = "North America";
            //JurisNorthAmericaNode.postChanges( false );

            //CswNbtNode JurisEuropeNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( JurisdictionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            //JurisEuropeNode.Properties[JurisdictionNameNTP].AsText.Text = "Europe";
            //JurisEuropeNode.postChanges( false );


            // GHS Phrase NodeType
            CswNbtMetaDataNodeType GHSPhraseNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( GenericOC )
            {
                NodeTypeName = "GHS Phrase",
                Category = "System",
                IconFileName = "group.gif"
            } );
            CswNbtMetaDataNodeTypeProp GHSPhraseCodeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "Code" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseCategoryNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, ListFT, "Category" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseEnglishNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "English" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseChineseNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "Chinese" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseDanishNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "Danish" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseDutchNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "Dutch" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseFrenchNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "French" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseGermanNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "German" ) );
            CswNbtMetaDataNodeTypeProp GHSPhraseSpanishNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSPhraseNT, TextFT, "Spanish" ) );
            GHSPhraseCategoryNTP.ListOptions = "Physical,Health,Environmental";
            GHSPhraseNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "Code" ) );
            
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtResources.CswNbtModule.CISPro, GHSPhraseNT.NodeTypeId );
            
            // GHS NodeType
            CswNbtMetaDataNodeType GHSNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( GenericOC )
            {
                NodeTypeName = "GHS",
                Category = "Materials",
                IconFileName = "ball_redS.gif"
            } );
            CswNbtMetaDataNodeTypeProp GHSTypeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSNT, ListFT, "Type" ) { IsRequired = true } );
            CswNbtMetaDataNodeTypeProp GHSMaterialNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSNT, RelationshipFT, "Material" ) { IsRequired = true } );
            CswNbtMetaDataNodeTypeProp GHSGHSPhraseNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSNT, RelationshipFT, "GHS Phrase" ) { IsRequired = true } );
            //CswNbtMetaDataNodeTypeProp GHSJurisdictionNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSNT, RelationshipFT, "Jurisdiction" ) { IsRequired = true } );
            CswNbtMetaDataNodeTypeProp GHSJurisdictionNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GHSNT, ListFT, "Jurisdiction" ) { IsRequired = true } );

            GHSTypeNTP.ListOptions = "Class Code, Label Code";
            GHSMaterialNTP.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), MaterialOC.ObjectClassId );
            GHSGHSPhraseNTP.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), GHSPhraseNT.NodeTypeId );
            //GHSJurisdictionNTP.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), JurisdictionNT.NodeTypeId );
            GHSJurisdictionNTP.ListOptions = "North America, China, Europe, Japan";
            
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtResources.CswNbtModule.CISPro, GHSNT.NodeTypeId );

            // Chemical GHS Tab
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                foreach( CswNbtMetaDataNodeTypeTab Tab in ChemicalNT.getNodeTypeTabs() )
                {
                    if( Tab.TabOrder >= 3 )
                        Tab.TabOrder += 1;
                }
                CswNbtMetaDataNodeTypeTab GHSTab = ChemicalNT.getNodeTypeTab( "GHS" );
                if( null == GHSTab )
                {
                    GHSTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "GHS", 3 );
                }
                GHSTab.TabOrder = 3;

                // Chemical GHS properties
                CswNbtMetaDataNodeTypeProp ChemicalGHSClassificationNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, ListFT, "GHS Classification" ) );
                CswNbtMetaDataNodeTypeProp ChemicalGHSClassCodesNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, GridFT, "GHS Class Codes" ) );
                CswNbtMetaDataNodeTypeProp ChemicalGHSSignalWordNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, ListFT, "GHS Signal Word" ) );
                CswNbtMetaDataNodeTypeProp ChemicalGHSLabelCodesNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, GridFT, "GHS Label Codes" ) );
                CswNbtMetaDataNodeTypeProp ChemicalGHSPictosNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, ImageListFT, "GHS Pictos" ) );

                CswCommaDelimitedString GHSClasses = new CswCommaDelimitedString(){
                    "Category 1",
                    "Category 1 / 1A / 1B",
                    "Category 1A",
                    "Category 1A / 1B",
                    "Category 1A / 1B / 1C",
                    "Category 1B",
                    "Category 2",
                    "Category 2 (skin)/2A (eye)",
                    "Category 2A",
                    "Category 2B",
                    "Category 3",
                    "Category 4",
                    "Category 5",
                    "Type A",
                    "Type B",
                    "Type C&D",
                    "Type E&F",
                    "Compressed Gas",
                    "Liquified Gas",
                    "Dissolved Gas",
                    "Refridgerated Liquidified Gas"
                };
                ChemicalGHSClassificationNTP.ListOptions = GHSClasses.ToString();
                ChemicalGHSSignalWordNTP.ListOptions = "Danger,Warning";

                ChemicalGHSClassificationNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                ChemicalGHSClassCodesNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                ChemicalGHSSignalWordNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                ChemicalGHSLabelCodesNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                ChemicalGHSPictosNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                ChemicalGHSClassificationNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GHSTab.TabId, 1, 1, "Hazard Classification" );
                ChemicalGHSClassCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GHSTab.TabId, 2, 1, "Hazard Classification" );
                ChemicalGHSSignalWordNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GHSTab.TabId, 3, 1, "Hazard Label" );
                ChemicalGHSLabelCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GHSTab.TabId, 4, 1, "Hazard Label" );
                ChemicalGHSPictosNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GHSTab.TabId, 5, 1, "Hazard Label" );


                // Grid Views
                CswNbtView GHSClassCodesView = _CswNbtSchemaModTrnsctn.restoreView( ChemicalGHSClassCodesNTP.ViewId );
                GHSClassCodesView.Root.ChildRelationships.Clear();

                CswNbtViewRelationship ChemicalVR1 = GHSClassCodesView.AddViewRelationship( ChemicalNT, false );
                CswNbtViewRelationship GHSVR1 = GHSClassCodesView.AddViewRelationship( ChemicalVR1, NbtViewPropOwnerType.Second, GHSMaterialNTP, true );
                CswNbtViewRelationship GHSPhraseVR1 = GHSClassCodesView.AddViewRelationship( GHSVR1, NbtViewPropOwnerType.First, GHSGHSPhraseNTP, true );

                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseCodeNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseCategoryNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseEnglishNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseChineseNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseDanishNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseDutchNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseFrenchNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseGermanNTP );
                GHSClassCodesView.AddViewProperty( GHSPhraseVR1, GHSPhraseSpanishNTP );

                GHSClassCodesView.AddViewPropertyAndFilter( GHSVR1, GHSTypeNTP, Value: "Class Code" );
                GHSClassCodesView.AddViewPropertyAndFilter( GHSVR1, GHSJurisdictionNTP, Value: "North America", ShowAtRuntime: true );
                GHSClassCodesView.save();

                CswNbtView GHSLabelCodesView = _CswNbtSchemaModTrnsctn.restoreView( ChemicalGHSLabelCodesNTP.ViewId );
                GHSLabelCodesView.Root.ChildRelationships.Clear();

                CswNbtViewRelationship ChemicalVR2 = GHSLabelCodesView.AddViewRelationship( ChemicalNT, false );
                CswNbtViewRelationship GHSVR2 = GHSLabelCodesView.AddViewRelationship( ChemicalVR2, NbtViewPropOwnerType.Second, GHSMaterialNTP, true );
                CswNbtViewRelationship GHSPhraseVR2 = GHSLabelCodesView.AddViewRelationship( GHSVR2, NbtViewPropOwnerType.First, GHSGHSPhraseNTP, true );

                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseCodeNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseCategoryNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseEnglishNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseChineseNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseDanishNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseDutchNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseFrenchNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseGermanNTP );
                GHSLabelCodesView.AddViewProperty( GHSPhraseVR2, GHSPhraseSpanishNTP );

                GHSLabelCodesView.AddViewPropertyAndFilter( GHSVR2, GHSTypeNTP, Value: "Label Code" );
                GHSLabelCodesView.AddViewPropertyAndFilter( GHSVR2, GHSJurisdictionNTP, Value: "North America", ShowAtRuntime: true );
                GHSLabelCodesView.save();

                // Picto Options
                ChemicalGHSPictosNTP.Extended = "true"; // allow multiple values
                ChemicalGHSPictosNTP.TextAreaColumns = 100;
                ChemicalGHSPictosNTP.TextAreaRows = 100;

                CswDelimitedString PictoNames = new CswDelimitedString( '\n' ) { 
                    "Chemical",
                    "Explosive",
                    "Flammable",
                    "Natural",
                    "Oxidizer",
                    "Danger",
                    "X"
                };
                CswDelimitedString PictoPaths = new CswDelimitedString( '\n' ) { 
                    //"Images/cispro/1-1.gif",
                    //"Images/cispro/1-2.gif",
                    //"Images/cispro/1-3.gif",
                    //"Images/cispro/1-4.gif",
                    //"Images/cispro/1-5.gif",
                    //"Images/cispro/1-6.gif",
                    //"Images/cispro/5-2.gif",
                    "Images/cispro/acide.gif",
                    //"Images/cispro/acide2.gif",
                    //"Images/cispro/blan-red.gif",
                    //"Images/cispro/bleu4.gif",
                    //"Images/cispro/bleu4-noir.gif",
                    "Images/cispro/bottle.gif",
                    "Images/cispro/exclam.gif",
                    "Images/cispro/explos.gif",
                    "Images/cispro/flamme.gif",
                    //"Images/cispro/jaune5-1.gif",
                    "Images/cispro/pollu.gif",
                    "Images/cispro/rondflam.gif",
                    //"Images/cispro/rouge2.gif",
                    //"Images/cispro/rouge2_noir.gif",
                    //"Images/cispro/rouge3.gif",
                    //"Images/cispro/rouge3_noir.gif",
                    "Images/cispro/silouete.gif",
                    "Images/cispro/skull.gif"
                    //"Images/cispro/skull_2.gif",
                    //"Images/cispro/skull6.gif",
                    //"Images/cispro/stripes.gif",
                    //"Images/cispro/vert.gif"
                };
                ChemicalGHSPictosNTP.ListOptions = PictoNames.ToString();
                ChemicalGHSPictosNTP.ValueOptions = PictoPaths.ToString();
            }

            //GHS Phrase nodes
            Dictionary<Int32, string> GHSPhraseCodes = new Dictionary<Int32, string>();
            //Physical
            GHSPhraseCodes.Add( 200, "Unstable explosive" );
            GHSPhraseCodes.Add( 201, "Explosive; mass explosion hazard" );
            GHSPhraseCodes.Add( 202, "Explosive; severe projection hazard" );
            GHSPhraseCodes.Add( 203, "Explosive; fire, blast or projection hazard" );
            GHSPhraseCodes.Add( 204, "Fire or projection hazard" );
            GHSPhraseCodes.Add( 205, "May mass explode in fire" );
            GHSPhraseCodes.Add( 220, "Extremely flammable gas" );
            GHSPhraseCodes.Add( 221, "Flammable gas" );
            GHSPhraseCodes.Add( 222, "Extremely flammable material" );
            GHSPhraseCodes.Add( 223, "Flammable material" );
            GHSPhraseCodes.Add( 224, "Extremely flammable liquid and vapour" );
            GHSPhraseCodes.Add( 225, "Highly flammable liquid and vapour" );
            GHSPhraseCodes.Add( 226, "Flammable liquid and vapour" );
            GHSPhraseCodes.Add( 227, "Combustible liquid" );
            GHSPhraseCodes.Add( 228, "Flammable solid" );
            GHSPhraseCodes.Add( 240, "Heating may cause an explosion" );
            GHSPhraseCodes.Add( 241, "Heating may cause a fire or explosion" );
            GHSPhraseCodes.Add( 242, "Heating may cause a fire" );
            GHSPhraseCodes.Add( 250, "Catches fire spontaneously if exposed to air" );
            GHSPhraseCodes.Add( 251, "Self-heating; may catch fire" );
            GHSPhraseCodes.Add( 252, "Self-heating in large quantities; may catch fire" );
            GHSPhraseCodes.Add( 260, "In contact with water releases flammable gases which may ignite spontaneously" );
            GHSPhraseCodes.Add( 261, "In contact with water releases flammable gas" );
            GHSPhraseCodes.Add( 270, "May cause or intensify fire; oxidizer" );
            GHSPhraseCodes.Add( 271, "May cause fire or explosion; strong oxidizer" );
            GHSPhraseCodes.Add( 272, "May intensify fire; oxidizer" );
            GHSPhraseCodes.Add( 280, "Contains gas under pressure; may explode if heated" );
            GHSPhraseCodes.Add( 281, "Contains refrigerated gas; may cause cryogenic burns or injury" );
            GHSPhraseCodes.Add( 290, "May be corrosive to metals" );
            //Health
            GHSPhraseCodes.Add( 300, "Fatal if swallowed" );
            GHSPhraseCodes.Add( 301, "Toxic if swallowed" );
            GHSPhraseCodes.Add( 302, "Harmful if swallowed" );
            GHSPhraseCodes.Add( 303, "May be harmful if swallowed" );
            GHSPhraseCodes.Add( 304, "May be fatal if swallowed and enters airways" );
            GHSPhraseCodes.Add( 305, "May be harmful if swallowed and enters airways" );
            GHSPhraseCodes.Add( 310, "Fatal in contact with skin" );
            GHSPhraseCodes.Add( 311, "Toxic in contact with skin" );
            GHSPhraseCodes.Add( 312, "Harmful in contact with skin" );
            GHSPhraseCodes.Add( 313, "May be harmful in contact with skin" );
            GHSPhraseCodes.Add( 314, "Causes severe skin burns and eye damage" );
            GHSPhraseCodes.Add( 315, "Causes skin irritation" );
            GHSPhraseCodes.Add( 316, "Causes mild skin irritation" );
            GHSPhraseCodes.Add( 317, "May cause an allergic skin reaction" );
            GHSPhraseCodes.Add( 318, "Causes serious eye damage" );
            GHSPhraseCodes.Add( 319, "Causes serious eye irritation" );
            GHSPhraseCodes.Add( 320, "Causes eye irritation" );
            GHSPhraseCodes.Add( 330, "Fatal if inhaled" );
            GHSPhraseCodes.Add( 331, "Toxic if inhaled" );
            GHSPhraseCodes.Add( 332, "Harmful if inhaled" );
            GHSPhraseCodes.Add( 333, "May be harmful if inhaled" );
            GHSPhraseCodes.Add( 334, "May cause allergy or asthma symptoms of breathing difficulties if inhaled" );
            GHSPhraseCodes.Add( 335, "May cause respiratory irritation" );
            GHSPhraseCodes.Add( 336, "May cause drowsiness or dizziness" );
            GHSPhraseCodes.Add( 340, "May cause genetic defects" );
            GHSPhraseCodes.Add( 341, "Suspected of causing genetic defects" );
            GHSPhraseCodes.Add( 350, "May cause cancer" );
            GHSPhraseCodes.Add( 351, "Suspected of causing cancer" );
            GHSPhraseCodes.Add( 360, "May damage fertility or the unborn child" );
            GHSPhraseCodes.Add( 361, "Suspected of damaging fertility or the unborn child" );
            GHSPhraseCodes.Add( 362, "May cause harm to breast-fed children" );
            GHSPhraseCodes.Add( 370, "Causes damage to organs" );
            GHSPhraseCodes.Add( 371, "May cause damage to organs" );
            GHSPhraseCodes.Add( 372, "Causes damage to organs through prolonged or repeated exposure" );
            GHSPhraseCodes.Add( 373, "May cause damage to organs through prolonged or repeated exposure" );
            // Environmental
            GHSPhraseCodes.Add( 400, "Very toxic to aquatic life" );
            GHSPhraseCodes.Add( 401, "Toxic to aquatic life" );
            GHSPhraseCodes.Add( 402, "Harmful to aquatic life" );
            GHSPhraseCodes.Add( 410, "Very toxic to aquatic life with long lasting effects" );
            GHSPhraseCodes.Add( 411, "Toxic to aquatic life with long lasting effects" );
            GHSPhraseCodes.Add( 412, "Harmful to aquatic life with long lasting effects" );
            GHSPhraseCodes.Add( 413, "May cause long lasting harmful effects to aquatic life" );

            foreach( Int32 Code in GHSPhraseCodes.Keys )
            {
                string Category = "Physical";
                if( Code > 299 )
                {
                    Category = "Health";
                }
                else if( Code > 399 )
                {
                    Category = "Environmental";
                }

                CswNbtNode GHSPhraseNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GHSPhraseNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                GHSPhraseNode.Properties[GHSPhraseCodeNTP].AsText.Text = "H" + Code.ToString();
                GHSPhraseNode.Properties[GHSPhraseCategoryNTP].AsList.Value = Category;
                GHSPhraseNode.Properties[GHSPhraseEnglishNTP].AsText.Text = GHSPhraseCodes[Code];
                GHSPhraseNode.postChanges( false );
            } // foreach( Int32 Code in GHSPhraseCodes.Keys )

        }//Update()

    }//class CswUpdateSchemaCase25291

}//namespace ChemSW.Nbt.Schema