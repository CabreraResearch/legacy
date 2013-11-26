using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31056 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31056; }
        }

        public override string Title
        {
            get { return "Merge Action"; }
        }

        public override void update()
        {
            // Create new action 'Merge'
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Merge, true, "", "System" );

            #region Debug test data
            // Some debug test data (TODO: REMOVE ME!)

            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            CswNbtMetaDataNodeType SizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );

            CswNbtMetaDataNodeTypeProp ContainerBarcodeNTP = ContainerNT.getNodeTypeProp( CswNbtObjClassContainer.PropertyName.Barcode );
            CswNbtMetaDataNodeTypeProp ContainerMaterialNTP = ContainerNT.getNodeTypeProp( CswNbtObjClassContainer.PropertyName.Material );
            ContainerBarcodeNTP.setIsUnique( false );
            ContainerBarcodeNTP.setIsCompoundUnique( true );
            ContainerMaterialNTP.setIsCompoundUnique( true );

            CswNbtNode chem1 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassChemical chem = node;
                    chem.TradeName.Text = "isopropylguacamolate";
                    chem.Supplier.RelatedNodeId = new CswPrimaryKey( "nodes", 30744 ); // sigma
                    chem.PartNumber.Text = "123";
                    chem.BoilingPoint.Text = "108";
                    chem.MeltingPoint.Text = "54";
                } );
            CswNbtNode chem2 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassChemical chem = node;
                    chem.TradeName.Text = "iso-propyl-guacamol-ate";
                    chem.Supplier.RelatedNodeId = new CswPrimaryKey( "nodes", 30744 ); // sigma
                    chem.PartNumber.Text = "123.1";
                    chem.BoilingPoint.Text = "113";
                    chem.MeltingPoint.Text = "54";
                } );

            CswNbtNode size1 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassSize size = node;
                    size.Material.RelatedNodeId = chem1.NodeId;
                    size.InitialQuantity.Quantity = 100;
                    size.InitialQuantity.UnitId = new CswPrimaryKey( "nodes", 26744 ); // kg
                    size.CatalogNo.Text = "321";
                    size.Description.Text = "Drum";
                } );

            CswNbtNode size2 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassSize size = node;
                    size.Material.RelatedNodeId = chem2.NodeId;
                    size.InitialQuantity.Quantity = 100;
                    size.InitialQuantity.UnitId = new CswPrimaryKey( "nodes", 26744 ); // kg
                    size.CatalogNo.Text = "321";
                    size.Description.Text = "Tub";
                } );

            CswNbtNode container1 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassContainer container = node;
                    container.Material.RelatedNodeId = chem1.NodeId;
                    container.Size.RelatedNodeId = size1.NodeId;
                    container.Barcode.setBarcodeValueOverride( "C500051", false );
                    container.Quantity.Quantity = 101;
                    container.Quantity.UnitId = new CswPrimaryKey( "nodes", 26744 ); // kg
                } );
            CswNbtNode container2 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassContainer container = node;
                    container.Material.RelatedNodeId = chem2.NodeId;
                    container.Size.RelatedNodeId = size2.NodeId;
                    container.Barcode.setBarcodeValueOverride( "C500051", false );
                    container.Quantity.Quantity = 102;
                    container.Quantity.UnitId = new CswPrimaryKey( "nodes", 26744 ); // kg
                } );

            CswNbtNode container3 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassContainer container = node;
                    container.Material.RelatedNodeId = chem2.NodeId;
                    container.Size.RelatedNodeId = size2.NodeId;
                    container.Barcode.setBarcodeValueOverride( "C500052", false );
                    container.Quantity.Quantity = 103;
                    container.Quantity.UnitId = new CswPrimaryKey( "nodes", 26744 ); // kg
                } );

            #endregion Debug test data

        } // update()

    }

}//namespace ChemSW.Nbt.Schema