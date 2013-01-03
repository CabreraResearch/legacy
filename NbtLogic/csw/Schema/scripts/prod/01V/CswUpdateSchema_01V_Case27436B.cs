using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

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
            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GHSClass );
            CswNbtMetaDataObjectClassProp GhsMaterialOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
            CswNbtMetaDataObjectClassProp GhsJurisdictionOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );
            CswNbtMetaDataObjectClassProp GhsLabelCodesOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.LabelCodes );
            CswNbtMetaDataObjectClassProp GhsSignalWordOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.SignalWord );

            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if(null != ChemicalNT)
            {
                // Clear out old GHS tab
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Classification" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Class Codes" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Signal Word" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Label Codes" ) );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalNT.getNodeTypeProp( "GHS Pictos" ) );

                // Add a new grid
                CswNbtMetaDataNodeTypeProp ChemicalGhsGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ), "GHS" ) );
                CswNbtView ChemicalGhsGridView = _CswNbtSchemaModTrnsctn.restoreView( ChemicalGhsGridNTP.ViewId );
                ChemicalGhsGridView.Clear();
                CswNbtViewRelationship ChemRel = ChemicalGhsGridView.AddViewRelationship( ChemicalNT, false );
                CswNbtViewRelationship GhsRel = ChemicalGhsGridView.AddViewRelationship( ChemRel, NbtViewPropOwnerType.Second, GhsMaterialOCP, true );
                ChemicalGhsGridView.AddViewProperty( GhsRel, GhsJurisdictionOCP );
                ChemicalGhsGridView.AddViewProperty( GhsRel, GhsSignalWordOCP );
                ChemicalGhsGridView.AddViewProperty( GhsRel, GhsLabelCodesOCP );
            } // if(null != ChemicalNT)

            CswNbtMetaDataNodeType GhsNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS" );
            if( null != GhsNT )
            {
                CswNbtMetaDataNodeTypeTab GhsJurisdictionTab = GhsNT.getNodeTypeTab( "Jurisdiction" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( GhsNT, "Jurisdiction", 1 );
                CswNbtMetaDataNodeTypeTab GhsLabelingTab = GhsNT.getNodeTypeTab( "Labeling" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( GhsNT, "Labeling", 2 );
                CswNbtMetaDataNodeTypeTab GhsClassificationTab = GhsNT.getNodeTypeTab( "Classification" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( GhsNT, "Classification", 3 );

                CswNbtMetaDataNodeTypeProp GhsJurisdictionNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );
                GhsJurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsJurisdictionTab.TabId );

                CswNbtMetaDataNodeTypeProp GhsMaterialNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                GhsMaterialNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsJurisdictionTab.TabId );

                CswNbtMetaDataNodeTypeProp GhsSignalWordNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.SignalWord );
                GhsSignalWordNTP.ListOptions = "Danger,Warning";
                GhsSignalWordNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsLabelingTab.TabId, 1, 1 );

                CswNbtMetaDataNodeTypeProp GhsPictogramsNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GhsNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ImageList ), "Pictograms" ) );
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
                    "Images/cispro/oxid.gif",
                    "Images/cispro/flamme.gif",
                    "Images/cispro/explos.gif",
                    "Images/cispro/skull.gif",
                    "Images/cispro/acide.gif",
                    "Images/cispro/bottle.gif",
                    "Images/cispro/silouete.gif",
                    "Images/cispro/pollu.gif",
                    "Images/cispro/exclam.gif"
                };
                GhsPictogramsNTP.ListOptions = PictoNames.ToString();
                GhsPictogramsNTP.ValueOptions = PictoPaths.ToString();
                GhsPictogramsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsLabelingTab.TabId, 1, 2 );

                CswNbtMetaDataNodeTypeProp GhsLabelCodesNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.LabelCodes );
                GhsLabelCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsLabelingTab.TabId, 2, 1 );

                CswNbtMetaDataNodeTypeProp GhsClassificationCodeNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GhsNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.List ), "Classification Code" ) );
                GhsClassificationCodeNTP.ListOptions = "Danger,Warning";
                GhsClassificationCodeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsLabelingTab.TabId, 1, 1 );

                CswNbtMetaDataNodeTypeProp GhsClassCodesNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.ClassCodes );
                GhsClassCodesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsClassificationTab.TabId, 2, 1 );
            } // if( null != GhsNT )
        } //Update()

    }//class CswUpdateSchema_01V_Case27436B

}//namespace ChemSW.Nbt.Schema