using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29108B
    /// </summary>
    public class CswUpdateSchema_02A_Case29108B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29108; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass );
            foreach( CswNbtObjClassEquipmentAssembly assemblyNode in assemblyOC.getNodes( false, false, false, true ) )
            {
                assemblyNode.Barcode.setBarcodeValue();
                assemblyNode.postChanges( false );
            }


        } // update()

    }//class CswUpdateSchema_02A_Case29108B

}//namespace ChemSW.Nbt.Schema