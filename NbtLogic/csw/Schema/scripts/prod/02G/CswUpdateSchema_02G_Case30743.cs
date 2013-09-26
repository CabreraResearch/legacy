using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30743: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30743; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30743"; }
        }

        public override string Title
        {
            get { return "Case30743_New_Materials_Props_Tab_Locations"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp StorageNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.StorageCompatibility );
                CswNbtMetaDataNodeTypeProp HazardInfoNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.HazardInfo );
                HazardInfoNTP.removeFromAllLayouts();
                HazardInfoNTP.updateLayout( CswEnumNbtLayoutType.Edit, StorageNTP, true );

                CswNbtMetaDataNodeTypeProp DisposalNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.DisposalInstructions );
                DisposalNTP.removeFromAllLayouts();
                DisposalNTP.updateLayout( CswEnumNbtLayoutType.Edit, HazardInfoNTP, true );

                CswNbtMetaDataNodeTypeProp FormulaNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.Formula );
                CswNbtMetaDataNodeTypeProp SMILESNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.SMILES );
                SMILESNTP.removeFromAllLayouts();
                SMILESNTP.updateLayout( CswEnumNbtLayoutType.Edit, FormulaNTP, true );

                CswNbtMetaDataNodeTypeProp AquNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.AqueousSolubility );
                CswNbtMetaDataNodeTypeProp CompGasNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.CompressedGas );
                CompGasNTP.removeFromAllLayouts();
                CompGasNTP.updateLayout( CswEnumNbtLayoutType.Edit, AquNTP, true );

                CswNbtMetaDataNodeTypeProp DOTCodeNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.DOTCode );
                DOTCodeNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                DOTCodeNTP.MaxValue = 999; //DOT codes are 3 numbers
                DOTCodeNTP.MinValue = 0;

                CswNbtMetaDataNodeTypeProp OpenExpireIntervalNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.OpenExpireInterval );
                OpenExpireIntervalNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                CswNbtMetaDataNodeTypeProp CompressedGasNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.CompressedGas );
                CompressedGasNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                CswNbtMetaDataNodeTypeProp EINECSNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.EINECS );
                EINECSNTP.removeFromLayout( CswEnumNbtLayoutType.Add );

                CswNbtMetaDataNodeTypeProp SubclassNameNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.SubclassName );
                SubclassNameNTP.removeFromAllLayouts();
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema