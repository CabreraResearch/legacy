using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53357 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 53353; }
        }

        public override string Title
        {
            get { return "Attach internal vendor to MLM"; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType InternalVendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Internal Vendor" );
            if( null != InternalVendorNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswEnumNbtModuleName.MLM, InternalVendorNT.NodeTypeId );
            }
        } // update()

    } // class

}//namespace ChemSW.Nbt.Schema