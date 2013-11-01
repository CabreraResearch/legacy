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
                CswNbtMetaDataNodeTypeProp HazardInfoNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.HazardInfo );
                HazardInfoNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp DisposalNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.DisposalInstructions );
                DisposalNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp SMILESNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.SMILES );
                SMILESNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp CompGasNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.CompressedGas );
                CompGasNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp DOTCodeNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.DOTCode );
                DOTCodeNTP.removeFromAllLayouts();
                DOTCodeNTP.MaxValue = 999; //DOT codes are 3 numbers
                DOTCodeNTP.MinValue = 0;

                CswNbtMetaDataNodeTypeProp OpenExpireIntervalNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.OpenExpireInterval );
                OpenExpireIntervalNTP.removeFromAllLayouts();


                CswNbtMetaDataNodeTypeProp EINECSNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.EINECS );
                EINECSNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp SubclassNameNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.SubclassName );
                SubclassNameNTP.removeFromAllLayouts();
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema