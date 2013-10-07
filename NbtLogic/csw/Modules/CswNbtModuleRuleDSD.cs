
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the DSD Module
    /// </summary>
    public class CswNbtModuleRuleDSD: CswNbtModuleRule
    {
        public CswNbtModuleRuleDSD( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }

        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.DSD; } }

        protected override void OnEnable()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PictogramsNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.Pictograms );
                PictogramsNTP.Hidden = false;

                CswNbtMetaDataNodeTypeProp LabelCodesNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodes );
                LabelCodesNTP.Hidden = false;

                CswNbtMetaDataNodeTypeProp LabelCodesGridNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodesGrid );
                LabelCodesGridNTP.Hidden = false;
            }
        }// OnEnable()

        protected override void OnDisable()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PictogramsNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.Pictograms );
                PictogramsNTP.Hidden = true;

                CswNbtMetaDataNodeTypeProp LabelCodesNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodes );
                LabelCodesNTP.Hidden = true;

                CswNbtMetaDataNodeTypeProp LabelCodesGridNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LabelCodesGrid );
                LabelCodesGridNTP.Hidden = true;
            }
        } // OnDisable()

    } // class CswNbtModuleC3
}// namespace ChemSW.Nbt
