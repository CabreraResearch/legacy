using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Properties;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26760
    /// </summary>
    public class CswUpdateSchemaCase26760 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataNodeType vendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
            if( null != vendorNT )
            {
                CswNbtObjClassVendor sigmaNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( vendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                sigmaNode.VendorName.Text = "Sigma-Aldrich";
                sigmaNode.postChanges( false );

                CswNbtObjClassVendor vwrNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( vendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                vwrNode.VendorName.Text = "VWR Scientific";
                vwrNode.postChanges( false );

                CswNbtObjClassVendor alfaNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( vendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                alfaNode.VendorName.Text = "Alfa-Aesar";
                alfaNode.postChanges( false );
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema