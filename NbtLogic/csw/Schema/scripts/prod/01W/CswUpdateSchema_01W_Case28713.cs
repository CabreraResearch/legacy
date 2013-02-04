using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28713
    /// </summary>
    public class CswUpdateSchema_01W_Case28713 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28713; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach(CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes())
            {
                //Part 5
                CswNbtMetaDataNodeTypeProp ReceiveNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( MaterialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Receive );
                ReceiveNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true );

                //Part 7
                CswNbtMetaDataNodeTypeProp SynonymsNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( MaterialNT.NodeTypeId, "Synonyms" );
                if( null != SynonymsNTP )
                {
                    SynonymsNTP.Attribute1 = CswConvert.ToDbVal( false ).ToString();
                }
            }

            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                //Part 1
                foreach( CswNbtMetaDataNodeTypeTab Tab in ChemicalNT.getNodeTypeTabs() )
                {
                    if( Tab.TabOrder >= 2 )
                        Tab.TabOrder += 1;
                }
                CswNbtMetaDataNodeTypeTab ComponentsTab = ChemicalNT.getNodeTypeTab( "Components" ) 
                    ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "Components", 2 );
                CswNbtMetaDataNodeTypeTab HazardsTab = ChemicalNT.getNodeTypeTab( "Hazards" );
                CswNbtMetaDataNodeTypeProp ComponentsTypeNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Components");
                if( null != ComponentsTypeNTP )
                {
                    ComponentsTypeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, HazardsTab.TabId );
                    ComponentsTypeNTP.Extended = CswNbtNodePropGrid.GridPropMode.Full.ToString();
                    ComponentsTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ComponentsTab.TabId, 1, 1 );
                }
                //Part 3
                CswNbtMetaDataNodeTypeProp UNCodeNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ChemicalNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.UNCode );
                UNCodeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId );
                //Part 4
                CswNbtMetaDataNodeTypeProp ChemicalMaterialTypeNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Material Type" );
                CswNbtMetaDataNodeTypeProp ChemicalSpecialFlagsNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Special Flags" );
                CswNbtMetaDataNodeTypeProp ChemicalHazardCategoriesNTP =_CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Hazard Categories" );
                CswNbtMetaDataNodeTypeProp ChemicalHazardClassesNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Hazard Classes" );
                if( null != ChemicalMaterialTypeNTP)
                {
                    ChemicalMaterialTypeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
                    ChemicalMaterialTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 4, 2, "Fire Reporting" );
                }
                if( null != ChemicalSpecialFlagsNTP )
                {
                    ChemicalSpecialFlagsNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
                    ChemicalSpecialFlagsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 5, 2, "Fire Reporting" );
                }
                if( null != ChemicalHazardCategoriesNTP )
                {
                    ChemicalHazardCategoriesNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
                    ChemicalHazardCategoriesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 6, 2, "Fire Reporting" );
                }
                if( null != ChemicalHazardClassesNTP )
                {
                    ChemicalHazardClassesNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
                    ChemicalHazardClassesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 7, 2, "Fire Reporting" );
                }
                //Part 8
                CswNbtMetaDataNodeTypeProp AssignedSDS = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ChemicalNT.NodeTypeId, "Assigned SDS" );
                if( null != AssignedSDS )
                {
                    CswNbtMetaDataNodeType MaterialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
                    CswNbtView AssignedSDSView = _CswNbtSchemaModTrnsctn.restoreView( AssignedSDS.ViewId );
                    if( null != AssignedSDSView && null != MaterialDocumentNT )
                    {
                        CswNbtMetaDataNodeTypeProp RevisionDateNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( MaterialDocumentNT.NodeTypeId, "Revision Date" );

                        CswNbtViewRelationship MaterialVR = AssignedSDSView.Root.ChildRelationships[0];
                        CswNbtViewRelationship MaterialDocumentVR = MaterialVR.ChildRelationships[0];
                        if( null != RevisionDateNTP && null != MaterialDocumentVR )
                        {
                            CswNbtViewProperty RevisionDateVP = AssignedSDSView.AddViewProperty(MaterialDocumentVR, RevisionDateNTP);
                            RevisionDateVP.Order = 8;
                            AssignedSDSView.save();
                        }
                    }
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase_01W_28713

}//namespace ChemSW.Nbt.Schema