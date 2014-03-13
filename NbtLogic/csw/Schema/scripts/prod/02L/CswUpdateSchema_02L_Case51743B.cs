using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case51743B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 51743; }
        }

        public override string Title
        {
            get { return "Generate Sequence Number for Batch Ops"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass BatchOpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BatchOpClass );

            foreach( CswNbtMetaDataNodeType BatchOpNT in BatchOpOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp BatchOpIdNTP = BatchOpNT.getNodeTypeProp( "Batch Op Id" );
                CswNbtView batchOpView = _CswNbtSchemaModTrnsctn.makeNewView( "BatchOps_51743", CswEnumNbtViewVisibility.Hidden );
                CswNbtViewRelationship parent = batchOpView.AddViewRelationship( BatchOpNT, false );
                batchOpView.AddViewPropertyAndFilter( parent, BatchOpIdNTP, FilterMode : CswEnumNbtFilterMode.Null );

                ICswNbtTree batchOpsTree = _CswNbtSchemaModTrnsctn.getTreeFromView( batchOpView, true );
                for( int i = 0; i < batchOpsTree.getChildNodeCount(); i++ )
                {
                    batchOpsTree.goToNthChild( i );

                    CswNbtObjClassBatchOp batchNode = batchOpsTree.getNodeForCurrentPosition();
                    batchNode.Node.Properties[BatchOpIdNTP].AsSequence.setSequenceValue();
                    batchNode.postChanges( false );

                    batchOpsTree.goToParentNode();
                }
            }


        } // update()

    }

}//namespace ChemSW.Nbt.Schema