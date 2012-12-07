using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28281
    /// </summary>
    public class CswUpdateSchema_01V_Case28281 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28281; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != ContainerNT )
            {
                CswNbtMetaDataNodeTypeTab ContainerTab = ContainerNT.getFirstNodeTypeTab();

                CswNbtMetaDataNodeTypeProp StoragePressureNTP = 
                    _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StoragePressure );
                CswNbtMetaDataNodeTypeProp StorageTemperatureNTP =
                    _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StorageTemperature );
                CswNbtMetaDataNodeTypeProp UseTypeNTP =
                    _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.UseType );

                StoragePressureNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                StorageTemperatureNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                UseTypeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                StoragePressureNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ContainerTab.TabId, 1, 2, "Fire Reporting");
                StorageTemperatureNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ContainerTab.TabId, 2, 2, "Fire Reporting" );
                UseTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ContainerTab.TabId, 3, 2, "Fire Reporting" );
            }

            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                foreach( CswNbtMetaDataNodeTypeTab Tab in ChemicalNT.getNodeTypeTabs() )
                {
                    if( Tab.TabOrder >= 2 )
                        Tab.TabOrder += 1;
                }
                CswNbtMetaDataNodeTypeTab HazardsTab = ChemicalNT.getNodeTypeTab( "Hazards" );
                if( null == HazardsTab )
                {
                    HazardsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "Hazards", 2 );
                }
                HazardsTab.TabOrder = 2;

                CswNbtMetaDataFieldType ListFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.List );
                CswNbtMetaDataFieldType MultiListFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MultiList );

                CswNbtMetaDataNodeTypeProp ChemicalMaterialTypeNTP = 
                    _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, ListFT, "Material Type" ) );
                CswNbtMetaDataNodeTypeProp ChemicalSpecialFlagsNTP = 
                    _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, MultiListFT, "Special Flags" ) );
                CswNbtMetaDataNodeTypeProp ChemicalHazardCategoriesNTP =
                    _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, MultiListFT, "Hazard Categories" ) );
                CswNbtMetaDataNodeTypeProp ChemicalHazardClassesNTP = 
                    _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( ChemicalNT, MultiListFT, "Hazard Classes" ) );

                ChemicalMaterialTypeNTP.ListOptions = "Pure,Mixture";
                ChemicalMaterialTypeNTP.DefaultValue.AsList.Value = "Pure";
                ChemicalMaterialTypeNTP.IsRequired = true;

                ChemicalSpecialFlagsNTP.ListOptions = "EHS,Waste,Not Reportable";
                CswCommaDelimitedString SpecialFlags = new CswCommaDelimitedString();
                SpecialFlags.FromString( "Not Reportable" );
                ChemicalSpecialFlagsNTP.DefaultValue.AsMultiList.Value = SpecialFlags;

                ChemicalHazardCategoriesNTP.ListOptions = "F = Fire,C = Chronic (delayed),I = Immediate (acute),R = Reactive,P = Pressure";
                ChemicalHazardClassesNTP.ListOptions = String.Join( ",", CswNbtObjClassFireClassExemptAmount.FireHazardClassTypes._All );

                ChemicalMaterialTypeNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                ChemicalSpecialFlagsNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                ChemicalHazardCategoriesNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                ChemicalHazardClassesNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                ChemicalMaterialTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 2, 2, "Fire Reporting" );
                ChemicalSpecialFlagsNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 3, 2, "Fire Reporting" );
                ChemicalHazardCategoriesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 4, 2, "Fire Reporting" );
                ChemicalHazardClassesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 5, 2, "Fire Reporting" );

                foreach( CswNbtNode ChemicalNode in ChemicalNT.getNodes( false, false ) )
                {
                    ChemicalNode.Properties[ChemicalMaterialTypeNTP].AsList.Value = "Pure";
                    ChemicalNode.Properties[ChemicalSpecialFlagsNTP].AsMultiList.Value = SpecialFlags;
                    ChemicalNode.postChanges( false );
                }
            }
        }

        //Update()

    }//class CswUpdateSchemaCase_01V_28281

}//namespace ChemSW.Nbt.Schema