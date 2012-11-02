using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27920
    /// </summary>
    public class CswUpdateSchema_01T_Case27920 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27920; }
        }

        public override void update()
        {
            /*
             * Untie the Cabinet, Shelf and Box NodeTypes from the IMCS module so the Locations view is not tied to the IMCS module
             */

            CswNbtMetaDataNodeType cabinetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Cabinet" );
            CswNbtMetaDataNodeType shelfNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Shelf" );
            CswNbtMetaDataNodeType boxNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Box" );

            if( null != cabinetNT &&
                null != shelfNT &&
                null != boxNT )
            {
                _CswNbtSchemaModTrnsctn.removeModuleNodeTypeJunction( CswNbtModuleName.IMCS, cabinetNT.NodeTypeId );
                _CswNbtSchemaModTrnsctn.removeModuleNodeTypeJunction( CswNbtModuleName.IMCS, shelfNT.NodeTypeId );
                _CswNbtSchemaModTrnsctn.removeModuleNodeTypeJunction( CswNbtModuleName.IMCS, boxNT.NodeTypeId );
            }

        }

        //Update()

    }//class CswUpdateSchemaCase27920

}//namespace ChemSW.Nbt.Schema