using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28903
    /// </summary>
    public class CswUpdateSchema_01Y_Case28903 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28903; }
        }

        public override void update()
        {
            int FCEAOCId = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.FireClassExemptAmountClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.FireCode, FCEAOCId );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, FCEAOCId );

            int FCEASOCId = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.FireClassExemptAmountSetClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.FireCode, FCEASOCId );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, FCEASOCId );

            CswNbtMetaDataNodeType ControlZoneNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
            if( null != ControlZoneNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.FireCode, ControlZoneNT.NodeTypeId );
                _CswNbtSchemaModTrnsctn.deleteModuleNodeTypeJunction( CswNbtModuleName.CISPro, ControlZoneNT.NodeTypeId );
            }

            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.FireCode, CswNbtActionName.Tier_II_Reporting );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.FireCode, CswNbtActionName.HMIS_Reporting );

            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Tier_II_Reporting );
            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.HMIS_Reporting );
        } //Update()
    }//class CswUpdateSchema_01Y_Case28903
}//namespace ChemSW.Nbt.Schema