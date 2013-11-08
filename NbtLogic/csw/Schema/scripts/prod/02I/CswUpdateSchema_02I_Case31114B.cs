using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31114B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31114; }
        }

        public override string Title
        {
            get { return "Position Link ChemWatch button"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp AssignedSDSNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.AssignedSDS );
                CswNbtMetaDataNodeTypeProp LinkChemWatchNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LinkChemWatch );
                LinkChemWatchNTP.removeFromAllLayouts();
                LinkChemWatchNTP.updateLayout( CswEnumNbtLayoutType.Edit, AssignedSDSNTP, true );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema