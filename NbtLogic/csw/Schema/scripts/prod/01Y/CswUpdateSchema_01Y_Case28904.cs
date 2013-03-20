using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28904
    /// </summary>
    public class CswUpdateSchema_01Y_Case28904 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28904; }
        }

        public override void update()
        {

            //Add the Regulatory Lists OC to the Reg Lists modules
            int RegListOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RegulatoryListClass );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, RegListOC_Id );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.RegulatoryLists, RegListOC_Id );

        } //Update()

    }//class CswUpdateSchema_01Y_Case28904

}//namespace ChemSW.Nbt.Schema