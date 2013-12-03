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

            // Grant permission to all administrators
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( forceReInit: false, IncludeDefaultFilters: false, IncludeHiddenNodes: true, includeSystemNodes: true ) )
            {
                if( RoleNode.Administrator.Checked == CswEnumTristate.True )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Merge, RoleNode, true );
                }
            } // foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( forceReInit: false, IncludeDefaultFilters: false, IncludeHiddenNodes: true, includeSystemNodes: true ) )


            #region Debug test data
            // Some debug test data (TODO: REMOVE ME!)

            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            CswNbtMetaDataNodeType SizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            CswNbtMetaDataNodeType GhsNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS" );
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
            CswNbtNode ghs1 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GhsNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassGHS ghs = node;
                    ghs.Material.RelatedNodeId = chem1.NodeId;
                    ghs.Jurisdiction.RelatedNodeId = new CswPrimaryKey( "nodes", 31745 ); // default jurisdiction
                    ghs.Classifications.AddValue( "nodes_41958" ); // Acute Toxicity: Oral (Category 5)
                    ghs.Classifications.AddValue( "nodes_41963" ); // Acute Toxicity: Dermal (Category 5)
                    ghs.LabelCodes.AddValue( "nodes_27745" ); // H201
                    ghs.LabelCodes.AddValue( "nodes_27760" ); // H241
                    ghs.Pictograms.AddValue( "acid.jpg" );
                    ghs.Pictograms.AddValue( "exclam.jpg" );
                    ghs.SignalWord.RelatedNodeId = new CswPrimaryKey( "nodes", 41941 ); // Danger
                } );
            CswNbtNode ghs2 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GhsNT.NodeTypeId, delegate( CswNbtNode node )
                {
                    CswNbtObjClassGHS ghs = node;
                    ghs.Material.RelatedNodeId = chem2.NodeId;
                    ghs.Jurisdiction.RelatedNodeId = new CswPrimaryKey( "nodes", 31745 ); // default jurisdiction
                    ghs.Classifications.AddValue( "nodes_41958" ); // Acute Toxicity: Oral (Category 5)
                    ghs.LabelCodes.AddValue( "nodes_27745" ); // H201
                    ghs.Pictograms.AddValue( "acid.jpg" );
                    ghs.SignalWord.RelatedNodeId = new CswPrimaryKey( "nodes", 41942 ); // Warning
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