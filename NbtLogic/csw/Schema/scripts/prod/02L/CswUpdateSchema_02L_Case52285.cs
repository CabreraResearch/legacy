using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52285 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52285; }
        }

        public override void update()
        {
            // Duplicate the existing Vendor nodetype, and name the new Vendor nodetype "Internal Vendor"
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            CswNbtMetaDataNodeType VendorNT = VendorOC.getNodeTypes().FirstOrDefault();
            if( null != VendorNT )
            {
                CswNbtObjClassDesignNodeType NewVendorNTNode = VendorNT.DesignNode.CopyNode();
                NewVendorNTNode.NodeTypeName.Text = "Internal Vendor";
                NewVendorNTNode.postChanges( false );
            }
            //foreach( CswNbtMetaDataNodeType VendorNT in VendorOC.getNodeTypes() )
            //{

            //}
        } // update()

    } // class CswUpdateSchema_02L_Case52285

}//namespace ChemSW.Nbt.Schema