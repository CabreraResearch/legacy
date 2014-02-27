using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case32003: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 32003; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override string Title
        {
            get { return "Add Missing Dispense Transactions to Imported Containers"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );

            CswTableSelect ContainerNodesSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "contNodesSel", "nodes" );
            DataTable ContainerNodesTable = ContainerNodesSelect.getTable( "where nodetypeid in (select nodetypeid from nodetypes where objectclassid = " + ContainerOC.ObjectClassId + ")" );
            foreach( DataRow ContainerRow in ContainerNodesTable.Rows )
            {
                _fillMissingContainerDispenseTransactions( ContainerRow["nodeid"].ToString() );
                _fillMissingContainerLocations( ContainerRow["nodeid"].ToString() );
            }
        } // update()

        private void _fillMissingContainerDispenseTransactions( string ContainerId )
        {
            CswNbtMetaDataObjectClass ContainerDispTransOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataObjectClassProp CDTContainerOCP = ContainerDispTransOC.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.DestinationContainer );
            CswNbtMetaDataObjectClassProp CDTDispensedDateOCP = ContainerDispTransOC.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.DispensedDate );

            CswNbtView CDTView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship CDTRel = CDTView.AddViewRelationship( ContainerDispTransOC, false );
            CDTView.AddViewPropertyAndFilter( CDTRel, CDTContainerOCP, CswEnumNbtFilterConjunction.And, ContainerId, CswEnumNbtSubFieldName.NodeID, false, CswEnumNbtFilterMode.Equals );
            CswNbtViewProperty DispensedDateVP = CDTView.AddViewProperty( CDTRel, CDTDispensedDateOCP );
            CDTView.setSortProperty( DispensedDateVP, CswEnumNbtViewPropertySortMethod.Ascending );
            ICswNbtTree CDTTree = _CswNbtSchemaModTrnsctn.getTreeFromView( CDTView, true );
            CDTTree.goToRoot();
            bool Found = false;
            double Qty = 0.0;
            if( CDTTree.getChildNodeCount() > 0 )
            {
                CDTTree.goToNthChild( 0 );
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = CDTTree.getNodeForCurrentPosition();
                if( ContDispTransNode.Type.Value == CswEnumNbtContainerDispenseType.Receive.ToString() )
                {
                    Found = true;
                }
                else
                {
                    Qty = ContDispTransNode.QuantityDispensed.Quantity + ContDispTransNode.RemainingSourceContainerQuantity.Quantity;
                }
            }
            if( !Found )
            {
                CswPrimaryKey ContainerPK = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ContainerId ) );
                CswNbtObjClassContainer Container = _CswNbtSchemaModTrnsctn.Nodes[ContainerPK];
                _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ContainerDispTransOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainerDispenseTransaction ContDispTransNode = NewNode;
                    ContDispTransNode.DestinationContainer.RelatedNodeId = Container.NodeId;
                    ContDispTransNode.QuantityDispensed.Quantity = Container.Quantity.Quantity + Qty;
                    ContDispTransNode.QuantityDispensed.UnitId = Container.Quantity.UnitId;
                    ContDispTransNode.Type.Value = CswEnumNbtContainerDispenseType.Receive.ToString();
                    ContDispTransNode.DispensedDate.DateTimeValue = Container.DateCreated.DateTimeValue;
                } );
            }
        }

        private void _fillMissingContainerLocations( string ContainerId )
        {
            CswNbtMetaDataObjectClass ContainerLocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp CLContainerOCP = ContainerLocOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            CswNbtMetaDataObjectClassProp CLScanDateOCP = ContainerLocOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );

            CswNbtView CLView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship CLRel = CLView.AddViewRelationship( ContainerLocOC, false );
            CLView.AddViewPropertyAndFilter( CLRel, CLContainerOCP, CswEnumNbtFilterConjunction.And, ContainerId, CswEnumNbtSubFieldName.NodeID, false, CswEnumNbtFilterMode.Equals );
            CswNbtViewProperty ScanDateVP = CLView.AddViewProperty( CLRel, CLScanDateOCP );
            CLView.setSortProperty( ScanDateVP, CswEnumNbtViewPropertySortMethod.Ascending );
            ICswNbtTree CLTree = _CswNbtSchemaModTrnsctn.getTreeFromView( CLView, true );
            CLTree.goToRoot();
            bool Found = false;
            if( CLTree.getChildNodeCount() > 0 )
            {
                CLTree.goToNthChild( 0 );
                CswNbtObjClassContainerLocation ContDispTransNode = CLTree.getNodeForCurrentPosition();
                if( ContDispTransNode.Type.Value == CswEnumNbtContainerLocationTypeOptions.Receipt.ToString() ||
                    ContDispTransNode.Type.Value == CswEnumNbtContainerLocationTypeOptions.Move.ToString() )
                {
                    Found = true;
                }
            }
            if( !Found )
            {
                CswPrimaryKey ContainerPK = new CswPrimaryKey( "nodes", CswConvert.ToInt32( ContainerId ) );
                CswNbtObjClassContainer Container = _CswNbtSchemaModTrnsctn.Nodes[ContainerPK];
                _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ContainerLocOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainerLocation ContLocNode = NewNode;
                    ContLocNode.Type.Value = CswEnumNbtContainerLocationTypeOptions.Receipt.ToString();
                    ContLocNode.Container.RelatedNodeId = Container.NodeId;
                    ContLocNode.Location.SelectedNodeId = Container.Location.SelectedNodeId;
                    ContLocNode.Location.CachedNodeName = Container.Location.CachedNodeName;
                    ContLocNode.Location.CachedPath = Container.Location.CachedPath;
                    ContLocNode.ActionApplied.Checked = CswEnumTristate.False;
                    ContLocNode.ScanDate.DateTimeValue = Container.DateCreated.DateTimeValue;
                } );
            }
        }

    }

}//namespace ChemSW.Nbt.Schema