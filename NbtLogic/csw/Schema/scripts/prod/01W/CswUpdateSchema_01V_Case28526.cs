using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28526
    /// </summary>
    public class CswUpdateSchema_01V_Case28526 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28526; }
        }

        public override void update()
        {
            // Hide 'Document Class' on equipment documents
            CswNbtMetaDataNodeType EquipmentDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Document" );
            if( null != EquipmentDocumentNT )
            {
                CswNbtMetaDataNodeTypeProp DocClassNTP = EquipmentDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );
                DocClassNTP.removeFromAllLayouts();
            }
        } //update()

    }//class CswUpdateSchema_01V_Case28526

}//namespace ChemSW.Nbt.Schema