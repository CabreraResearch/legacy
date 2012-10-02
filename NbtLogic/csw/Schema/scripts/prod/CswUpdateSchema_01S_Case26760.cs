using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26760
    /// </summary>
    public class CswUpdateSchema_01S_Case26760 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataNodeType vendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
            bool makeSigma = true;
            bool makeAlfa = true;
            bool makeVWR = true;

            foreach( CswNbtObjClassVendor vendorNode in vendorNT.getNodes( false, false ) )
            {
                if( vendorNode.VendorName.Text.Equals( "Sigma-Aldrich" ) ) makeSigma = false;
                if( vendorNode.VendorName.Text.Equals( "VWR Scientific" ) ) makeVWR = false;
                if( vendorNode.VendorName.Text.Equals( "Alfa-Aesar" ) ) makeAlfa = false;
            }

            if( null != vendorNT )
            {
                if( makeSigma )
                {
                    CswNbtObjClassVendor sigmaNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( vendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    sigmaNode.VendorName.Text = "Sigma-Aldrich";
                    sigmaNode.postChanges( false );
                }

                if( makeVWR )
                {
                    CswNbtObjClassVendor vwrNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( vendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    vwrNode.VendorName.Text = "VWR Scientific";
                    vwrNode.postChanges( false );
                }

                if( makeAlfa )
                {
                    CswNbtObjClassVendor alfaNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( vendorNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    alfaNode.VendorName.Text = "Alfa-Aesar";
                    alfaNode.postChanges( false );
                }
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema