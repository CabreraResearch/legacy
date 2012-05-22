using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchManager
    {
        /// <summary>
        /// If an operation affects this number of nodes, run as a batch operation instead
        /// </summary>
        public static Int32 getBatchThreshold( CswNbtResources CswNbtResources )
        {
            Int32 ret = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( "batchthreshold" ) );
            if( Int32.MinValue == ret )
            {
                ret = 10;
            }
            return ret;
        }

        /// <summary>
        /// Restore an existing batch row from the database
        /// </summary>
        public static CswNbtObjClassBatchOp restore( CswNbtResources CswNbtResources, CswPrimaryKey BatchId )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( BatchId != null && BatchId.PrimaryKey != Int32.MinValue )
            {
                CswNbtNode Node = CswNbtResources.Nodes[BatchId];
                if( Node != null )
                {
                    BatchNode = (CswNbtObjClassBatchOp) Node;
                }
            }
            return BatchNode;
        } // restore()

        /// <summary>
        /// Makes a new batch operation instance in the database
        /// </summary>
        public static CswNbtObjClassBatchOp makeNew( CswNbtResources CswNbtResources,
                                                     NbtBatchOpName BatchOpName,
                                                     string BatchData,
                                                     CswPrimaryKey UserId = null,
                                                     Double Priority = Double.NaN )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            CswNbtMetaDataObjectClass BatchOpOC = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass );
            if( BatchOpOC != null )
            {
                CswNbtMetaDataNodeType BatchOpNT = BatchOpOC.getNodeTypes().First();
                if( BatchOpNT != null )
                {
                    CswNbtNode Node = CswNbtResources.Nodes.makeNodeFromNodeTypeId( BatchOpNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    BatchNode = (CswNbtObjClassBatchOp) Node;

                    BatchNode.BatchData.Text = BatchData;
                    BatchNode.OpName.Value = BatchOpName.ToString();
                    BatchNode.Priority.Value = Priority;
                    BatchNode.Status.Value = NbtBatchOpStatus.Pending.ToString();
                    BatchNode.User.RelatedNodeId = UserId ?? CswNbtResources.CurrentNbtUser.UserId;

                    BatchNode.postChanges( true );
                }
            }
            return BatchNode;
        } // makeNew()

        public static void runNextBatchOp( CswNbtResources CswNbtResources )
        {
            CswNbtMetaDataObjectClass BatchOpOC = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass );
            CswNbtMetaDataObjectClassProp StatusOCP = BatchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.StatusPropertyName );
            CswNbtMetaDataObjectClassProp PriorityOCP = BatchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PriorityPropertyName );

            CswNbtView NextBatchOpView = new CswNbtView( CswNbtResources );
            CswNbtViewRelationship BatchVR = NextBatchOpView.AddViewRelationship( BatchOpOC, false );
            CswNbtViewProperty StatusVP = NextBatchOpView.AddViewProperty( BatchVR, StatusOCP );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: NbtBatchOpStatus.Completed.ToString() );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: NbtBatchOpStatus.Error.ToString() );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: NbtBatchOpStatus.Unknown.ToString() );
            CswNbtViewProperty PriorityVP = NextBatchOpView.AddViewProperty( BatchVR, PriorityOCP );
            NextBatchOpView.setSortProperty( PriorityVP, NbtViewPropertySortMethod.Descending );

            ICswNbtTree BatchOpTree = CswNbtResources.Trees.getTreeFromView( NextBatchOpView, false, true );
            if( BatchOpTree.getChildNodeCount() > 0 )
            {
                BatchOpTree.goToNthChild( 0 );
                CswNbtNode Node = BatchOpTree.getNodeForCurrentPosition();
                CswNbtObjClassBatchOp BatchNode = (CswNbtObjClassBatchOp) Node;

                NbtBatchOpName OpName = (NbtBatchOpName) BatchNode.OpName.Value;
                if( OpName == NbtBatchOpName.FutureNodes )
                {
                    CswNbtBatchOpFutureNodes op = new CswNbtBatchOpFutureNodes( CswNbtResources );
                    op.runBatchOp( BatchNode );
                }
                else if( OpName == NbtBatchOpName.MultiEdit )
                {
                    CswNbtBatchOpMultiEdit op = new CswNbtBatchOpMultiEdit( CswNbtResources );
                    op.runBatchOp( BatchNode );
                }
                // New batch ops go here
                // else if( OpName == NbtBatchOpName.NEWNAME ) 
            }
        }

    } // class CswNbtBatchManager
} // namespace ChemSW.Nbt.Batch
