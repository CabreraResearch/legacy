using System.Collections.Specialized;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27436
    /// </summary>
    public class CswUpdateSchema_01V_Case27436B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 27436; }
        }

        public override void update()
        {
            // Jurisdiction Nodes
            CswNbtMetaDataObjectClass JurisdictionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.JurisdictionClass );
            CswNbtMetaDataNodeType JurisdictionNT = JurisdictionOC.FirstNodeType;
            if( null != JurisdictionNT )
            {
                StringCollection Jurisdictions = new StringCollection { "North America", "China", "Europe", "Japan" };
                foreach( string j in Jurisdictions )
                {
                    CswNbtObjClassJurisdiction jNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( JurisdictionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    jNode.Name.Text = j;
                    jNode.postChanges( false );
                }
            }


            //CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GHSClass );
            //CswNbtMetaDataObjectClassProp GhsMaterialOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
            //CswNbtMetaDataObjectClassProp GhsJurisdictionOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );
            //CswNbtMetaDataObjectClassProp GhsLabelCodesOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.LabelCodes );
            //CswNbtMetaDataObjectClassProp GhsSignalWordOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.SignalWord );

            // Chemical GHS tab
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                //CswNbtMetaDataNodeTypeTab ChemicalGhsTab = ChemicalNT.getNodeTypeTab( "GHS" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "GHS", 3 );

                // Clear out old GHS tab
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Classification" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Class Codes" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Signal Word" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Label Codes" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Pictos" ) );

                //// Add a new grid
                //CswNbtMetaDataNodeTypeProp ChemicalGhsGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ), "GHS" ) );
                //CswNbtView ChemicalGhsGridView = _CswNbtSchemaModTrnsctn.restoreView( ChemicalGhsGridNTP.ViewId );
                //ChemicalGhsGridView.Root.ChildRelationships.Clear();
                //CswNbtViewRelationship ChemRel = ChemicalGhsGridView.AddViewRelationship( ChemicalNT, false );
                //CswNbtViewRelationship GhsRel = ChemicalGhsGridView.AddViewRelationship( ChemRel, NbtViewPropOwnerType.Second, GhsMaterialOCP, true );
                //CswNbtViewProperty JurisVP = ChemicalGhsGridView.AddViewProperty( GhsRel, GhsJurisdictionOCP );
                //CswNbtViewProperty SignalVP = ChemicalGhsGridView.AddViewProperty( GhsRel, GhsSignalWordOCP );
                //CswNbtViewProperty LabelVP = ChemicalGhsGridView.AddViewProperty( GhsRel, GhsLabelCodesOCP );
                //JurisVP.Order = 1;
                //SignalVP.Order = 2;
                //LabelVP.Order = 3;
                //ChemicalGhsGridView.save();

                //ChemicalGhsGridNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ChemicalGhsTab.TabId );

            } // if(null != ChemicalNT)

            CswNbtMetaDataNodeType GhsNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS" );
            if( null != GhsNT )
            {
                // Tabs
                CswNbtMetaDataNodeTypeTab GhsGhsTab = GhsNT.getNodeTypeTab( "GHS" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( GhsNT, "GHS", 1 );


                // Properties
                CswNbtMetaDataNodeTypeProp GhsJurisdictionNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );
                CswNbtMetaDataNodeTypeProp GhsMaterialNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                CswNbtMetaDataNodeTypeProp GhsSignalWordNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.SignalWord );
                CswNbtMetaDataNodeTypeProp GhsLabelCodesNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.LabelCodes );
                CswNbtMetaDataNodeTypeProp GhsClassCodesNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.ClassCodes );
                CswNbtMetaDataNodeTypeProp GhsLabelCodesGridNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.LabelCodesGrid );
                CswNbtMetaDataNodeTypeProp GhsClassCodesGridNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.ClassCodesGrid );
                CswNbtMetaDataNodeTypeProp GhsPictogramsNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GhsNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ImageList ), "Pictograms" ) );
                CswNbtMetaDataNodeTypeProp GhsClassificationNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GhsNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.List ), "Classification" ) );


                // Property Configuration
                GhsSignalWordNTP.ListOptions = "Danger,Warning";
                GhsSignalWordNTP.IsRequired = true;

                GhsMaterialNTP.IsRequired = true;
                GhsMaterialNTP.ReadOnly = true;
                
                GhsPictogramsNTP.Extended = "true";
                GhsPictogramsNTP.TextAreaColumns = 77;
                GhsPictogramsNTP.TextAreaRows = 77;
                CswDelimitedString PictoNames = new CswDelimitedString( '\n' ) { 
                    "Oxidizer",
                    "Flammable",
                    "Explosive",
                    "Acute Toxicity (severe)",
                    "Corrosive",
                    "Gases Under Pressure",
                    "Target Organ Toxicity",
                    "Environmental Toxicity",
                    "Irritant"
                };
                CswDelimitedString PictoPaths = new CswDelimitedString( '\n' ) { 
                    "Images/cispro/ghs/rondflam.jpg",
                    "Images/cispro/ghs/flamme.jpg",
                    "Images/cispro/ghs/explos.jpg",
                    "Images/cispro/ghs/skull.jpg",
                    "Images/cispro/ghs/acid.jpg",
                    "Images/cispro/ghs/bottle.jpg",
                    "Images/cispro/ghs/silhouet.jpg",
                    "Images/cispro/ghs/pollut.jpg",
                    "Images/cispro/ghs/exclam.jpg"
                };
                GhsPictogramsNTP.ListOptions = PictoNames.ToString();
                GhsPictogramsNTP.ValueOptions = PictoPaths.ToString();

                GhsLabelCodesGridNTP.Extended = CswNbtNodePropGrid.GridPropMode.Small.ToString();
                GhsLabelCodesGridNTP.MaxValue = 10;

                GhsClassCodesGridNTP.Extended = CswNbtNodePropGrid.GridPropMode.Small.ToString();
                GhsClassCodesGridNTP.MaxValue = 10;

                const string LabelingTabGroup = "Labeling";
                const string ClassificationTabGroup = "Classification";


                // Add Layout
                GhsMaterialNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
                GhsJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 2, DisplayColumn: 1 );
                GhsSignalWordNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 3, DisplayColumn: 1, TabGroup: LabelingTabGroup );
                GhsPictogramsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 4, DisplayColumn: 1, TabGroup: LabelingTabGroup );
                GhsLabelCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 5, DisplayColumn: 1, TabGroup: LabelingTabGroup );
                GhsClassificationNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 6, DisplayColumn: 1, TabGroup: ClassificationTabGroup );
                GhsClassCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 7, DisplayColumn: 1, TabGroup: ClassificationTabGroup );
                GhsLabelCodesGridNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                GhsClassCodesGridNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );


                // Preview Layout
                GhsJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                GhsSignalWordNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
                GhsPictogramsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );
                GhsLabelCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 4, DisplayColumn: 1 );


                // Edit Layout
                GhsMaterialNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 1, DisplayColumn: 1 );
                GhsJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 2, DisplayColumn: 1 );
                GhsSignalWordNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 3, DisplayColumn: 1, TabGroup: LabelingTabGroup );
                GhsPictogramsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 4, DisplayColumn: 1, TabGroup: LabelingTabGroup );
                GhsLabelCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 5, DisplayColumn: 1, TabGroup: LabelingTabGroup );
                GhsLabelCodesGridNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 6, DisplayColumn: 1, TabGroup: LabelingTabGroup );
                GhsClassificationNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 7, DisplayColumn: 1, TabGroup: ClassificationTabGroup );
                GhsClassCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 8, DisplayColumn: 1, TabGroup: ClassificationTabGroup );
                GhsClassCodesGridNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, DisplayRow: 9, DisplayColumn: 1, TabGroup: ClassificationTabGroup );


            } // if( null != GhsNT )



            //CswNbtMetaDataNodeType GhsPhraseNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS Phrase" );
            //if( null != GhsPhraseNT )
            //{
            //    CswNbtMetaDataNodeTypeProp GhsPhraseCodeNTP = GhsPhraseNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHSPhrase.PropertyName.Code );
            //    CswNbtMetaDataNodeTypeProp GhsPhraseEnglishNTP = GhsPhraseNT.getNodeTypeProp( "English" );

            //    string NameTemplate = CswNbtMetaData.MakeTemplateEntry( GhsPhraseCodeNTP.PropName ) + " ";
            //    if(null != GhsPhraseEnglishNTP)
            //    {
            //        NameTemplate += CswNbtMetaData.MakeTemplateEntry( GhsPhraseEnglishNTP.PropName );
            //    }
            //    GhsPhraseNT.setNameTemplateText( NameTemplate );

            //    foreach(CswNbtNode Node in GhsPhraseNT.getNodes( false, false ))
            //    {
            //        Node.postChanges( true );
            //    }

            //} // if( null != GhsPhraseNT )

        } //Update()

    }//class CswUpdateSchema_01V_Case27436B

}//namespace ChemSW.Nbt.Schema