using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28393
    /// </summary>
    public class CswUpdateSchema_01W_Case28393B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28393; }
        }

        public override void update()
        {
            //Add HMIS Reporting Action
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.HMIS_Reporting, true, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.HMIS_Reporting );

            //Add new Container Fire Reporting defualt values
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp StorageTemperatureOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.StorageTemperature );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StorageTemperatureOCP, CswNbtObjClassContainer.StorageTemperatures.RoomTemperature );
            CswNbtMetaDataObjectClassProp StoragePressureOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.StoragePressure );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StoragePressureOCP, CswNbtObjClassContainer.StoragePressures.Atmospheric );
            CswNbtMetaDataObjectClassProp UseTypeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.UseType );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( UseTypeOCP, CswNbtObjClassContainer.UseTypes.Storage );

            //Fix Fire Hazard Class Types default value
            String FireHazardClassTypes =
                        "Aero-1,Aero-2,Aero-3,Carc,CF/D (balled),CF/D (loose),CL-II,CL-IIIA,CL-IIIB,Corr,CRY-FG,CRY-OXY,Exp,FG (gaseous),FG (liquified),FL-1A,FL-1B,FL-1C,FL-Comb,FS,H.T.,Irr,OHH,Oxy1,Oxy2,Oxy3,Oxy4,Oxy-Gas,Oxy-Gas (liquid),Perox-Det,Perox-I,Perox-II,Perox-III,Perox-IV,Perox-V,Pyro,RAD-Alpha,RAD-Beta,RAD-Gamma,Sens,Tox,UR-1,UR-2,UR-3,UR-4,WR-1,WR-2,WR-3";

            CswNbtMetaDataObjectClass FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass );
            CswNbtMetaDataObjectClassProp FireHazardClassTypesOCP = FireClassExemptAmountOC.getObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.FireHazardClassType );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FireHazardClassTypesOCP, FireHazardClassTypes );
            CswNbtMetaDataNodeType FireClassExemptAmountNT = FireClassExemptAmountOC.FirstNodeType;
            if( null != FireClassExemptAmountNT )
            {
                CswNbtMetaDataNodeTypeProp FireClassHazardTypesNTP =
                    _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp(
                        FireClassExemptAmountNT.NodeTypeId,
                        CswNbtObjClassFireClassExemptAmount.PropertyName.FireHazardClassType
                        );
                FireClassHazardTypesNTP.ListOptions = FireHazardClassTypes;
            }

            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtMetaDataNodeTypeProp ChemicalHazardClassesNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Hazard Classes");
                if(null != ChemicalHazardClassesNTP)
                {
                    ChemicalHazardClassesNTP.ListOptions = FireHazardClassTypes;
                    CswNbtMetaDataNodeTypeTab HazardsTab = ChemicalNT.getNodeTypeTab( "Hazards" ) ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "Hazards", 2 );
                    HazardsTab.TabOrder = 2;
                    ChemicalHazardClassesNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    ChemicalHazardClassesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId, 5, 2, "Fire Reporting" );
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase_01W_28393B

}//namespace ChemSW.Nbt.Schema