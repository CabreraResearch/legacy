using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case31203 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31203; }
        }

        public override string Title
        {
            get { return "Remove Dispenser from ContDispTrans Add Layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContDispTransOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerDispenseTransactionClass );
            foreach( CswNbtMetaDataNodeType ContDispTransNT in ContDispTransOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp DispenserNTP = ContDispTransNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.Dispenser );
                DispenserNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
            }
        } // update()
    }

}//namespace ChemSW.Nbt.Schema