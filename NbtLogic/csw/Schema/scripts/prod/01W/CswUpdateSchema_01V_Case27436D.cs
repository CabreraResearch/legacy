using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27436
    /// </summary>
    public class CswUpdateSchema_01V_Case27436D : CswUpdateSchemaTo
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
            CswNbtMetaDataNodeType GhsNT = GhsOC.getNodeTypes().First();
            if( null != GhsNT )
            {
                CswNbtMetaDataNodeTypeTab GhsGhsTab = GhsNT.getNodeTypeTab( "GHS" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( GhsNT, "GHS", 1 );

                // Fix nodetype name template
                GhsNT.setNameTemplateText(
                    //                       "GHS for " +
                    //                       CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassGHS.PropertyName.Material ) +
                    //                       ": " +
                                           CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassGHS.PropertyName.Jurisdiction ) );


                // Classification is now a list
                const string ClassificationPropName = "Classification";
                const string ClassificationTabGroup = "Classification";

                CswNbtMetaDataNodeTypeProp GhsClassificationNTP = GhsNT.getNodeTypeProp( ClassificationPropName );
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( GhsClassificationNTP );

                GhsClassificationNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( GhsNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.List ), ClassificationPropName ) );
                GhsClassificationNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DisplayRow: 6, DisplayColumn: 1, TabGroup: ClassificationTabGroup );
                GhsClassificationNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, GhsGhsTab.TabId, 6, 1, ClassificationTabGroup );

                // Classification list options
                CswCommaDelimitedString ClassificationOptions = new CswCommaDelimitedString
                    {
                        "Category 1", 
                        "Category 2", 
                        "Category 3", 
                        "Category 4", 
                        "Category 1 A/1 B/1 C", 
                        "Category 2 (skin)/2A (eye)",
                        "Category 2A",
                        "Category 2B",
                        "Category 1/ 1A / 1B",
                        "Category 1A or Category 1B",
                        "Type A",
                        "Type B",
                        "Type C&D",
                        "Type E&F",
                        "Compressed Gas",
                        "Liquidfied Gas",
                        "Dissolved Gas",
                        "Refridgerated Liquidified Gas"
                    };

                GhsClassificationNTP.ListOptions = ClassificationOptions.ToString();

                // Chemical GHS tab
                CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
                if( null != ChemicalNT )
                {
                    CswNbtMetaDataNodeTypeTab ChemicalGhsTab = ChemicalNT.getNodeTypeTab( "GHS" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "GHS", 3 );

                    // Delete the GHS grid, if present
                    CswNbtMetaDataNodeTypeProp ChemicalGhsGridNTP = ChemicalNT.getNodeTypeProp( "GHS" );
                    if( null != ChemicalGhsGridNTP )
                    {
                        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ChemicalGhsGridNTP );
                    }

                    // Add Child Contents property to Chemical GHS tab
                    CswNbtMetaDataNodeTypeProp JurisdictionNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ChildContents ), "Jurisdiction" ) );
                    JurisdictionNTP.SetFK( NbtViewPropIdType.ObjectClassPropId.ToString(), GhsMaterialOCP.ObjectClassPropId );
                    JurisdictionNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    JurisdictionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ChemicalGhsTab.TabId, 1, 1 );

                } // if(null != ChemicalNT)


            } // if (null != GhsNT)

        } //update()

    }//class CswUpdateSchema_01V_Case27436D

}//namespace ChemSW.Nbt.Schema