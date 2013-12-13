using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case31101 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31101; }
        }

        public override string Title
        {
            get { return "Remove UN Code/LQNo from Layouts"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LQNoNTP = ChemicalNT.getNodeTypePropByObjectClassProp(CswNbtObjClassChemical.PropertyName.LQNo);
                LQNoNTP.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp UNCodeNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.UNCode );
                UNCodeNTP.removeFromAllLayouts();
            }
        } // update()
    }

}//namespace ChemSW.Nbt.Schema