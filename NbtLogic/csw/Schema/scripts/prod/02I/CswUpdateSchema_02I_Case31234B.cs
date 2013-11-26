using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31234B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31234; }
        }

        public override string Title
        {
            get { return "Remove new CAF Chemical Props from layouts"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LegacyMaterialIdNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LegacyMaterialId );
                LegacyMaterialIdNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp ProductDescriptionNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.ProductDescription );
                ProductDescriptionNTP.removeFromAllLayouts();
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema