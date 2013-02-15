using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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
                    BatchNode = Node;
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
            CswNbtMetaDataObjectClass BatchOpOC = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.BatchOpClass );
            if( BatchOpOC != null )
            {
                CswNbtMetaDataNodeType BatchOpNT = BatchOpOC.getNodeTypes().First();
                if( BatchOpNT != null )
                {
                    CswNbtNode Node = CswNbtResources.Nodes.makeNodeFromNodeTypeId( BatchOpNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    BatchNode = Node;

                    BatchNode.BatchData.Text = BatchData;
                    BatchNode.CreatedDate.DateTimeValue = DateTime.Now;
                    BatchNode.OpName.Value = BatchOpName.ToString();
                    if( false == Double.IsNaN( Priority ) )
                    {
                        BatchNode.Priority.Value = Priority;
                    }
                    BatchNode.Status.Value = NbtBatchOpStatus.Pending.ToString();
                    BatchNode.User.RelatedNodeId = UserId ?? CswNbtResources.CurrentNbtUser.UserId;

                    BatchNode.postChanges( true );
                }
            }
            return BatchNode;
        } // makeNew()

        public static void runNextBatchOp( CswNbtResources CswNbtResources )
        {
            CswNbtMetaDataObjectClass BatchOpOC = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.BatchOpClass );
            CswNbtMetaDataObjectClassProp StatusOCP = BatchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PropertyName.Status );
            CswNbtMetaDataObjectClassProp PriorityOCP = BatchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PropertyName.Priority );


            CswNbtView NextBatchOpView = new CswNbtView( CswNbtResources );
            CswNbtViewRelationship BatchVR = NextBatchOpView.AddViewRelationship( BatchOpOC, false );
            CswNbtViewProperty StatusVP = NextBatchOpView.AddViewProperty( BatchVR, StatusOCP );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: NbtBatchOpStatus.Completed.ToString() );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: NbtBatchOpStatus.Error.ToString() );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: NbtBatchOpStatus.Unknown.ToString() );
            CswNbtViewProperty PriorityVP = NextBatchOpView.AddViewProperty( BatchVR, PriorityOCP );
            NextBatchOpView.setSortProperty( PriorityVP, NbtViewPropertySortMethod.Descending );

            ICswNbtTree BatchOpTree = CswNbtResources.Trees.getTreeFromView( NextBatchOpView, false, true, false );
            if( BatchOpTree.getChildNodeCount() > 0 )
            {
                BatchOpTree.goToNthChild( 0 );
                CswNbtNode Node = BatchOpTree.getNodeForCurrentPosition();
                CswNbtObjClassBatchOp BatchNode = Node;

                NbtBatchOpName OpName = BatchNode.OpNameValue;
                ICswNbtBatchOp op = null;
                if( OpName == NbtBatchOpName.FutureNodes )
                {
                    op = new CswNbtBatchOpFutureNodes( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.MultiEdit )
                {
                    op = new CswNbtBatchOpMultiEdit( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.InventoryLevel )
                {
                    op = new CswNbtBatchOpInventoryLevels( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.MultiDelete )
                {
                    op = new CswNbtBatchOpMultiDelete( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.MailReport )
                {
                    op = new CswNbtBatchOpMailReport( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.UpdateRegulatoryLists )
                {
                    op = new CswNbtBatchOpUpdateRegulatoryLists( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.UpdateRegulatoryListsForMaterials )
                {
                    op = new CswNbtBatchOpUpdateRegulatoryListsForMaterials( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.ExpiredContainers )
                {
                    op = new CswNbtBatchOpExpiredContainers( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.MolFingerprints )
                {
                    op = new CswNbtBatchOpMolFingerprints( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.ContainerReconciliationActions )
                {
                    op = new CswNbtBatchOpContainerReconciliationActions( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.SyncLocation )
                {
                    op = new CswNbtBatchOpSyncLocation( CswNbtResources );
                }
                else if( OpName == NbtBatchOpName.MobileMultiOpUpdates )
                {
                    op = new CswNbtBatchOpMobileMultiOpUpdates( CswNbtResources );
                }

                // New batch ops go here
                // else if( OpName == NbtBatchOpName.NEWNAME ) 
                if( null != op )
                {

                    CswNbtNode UserNode = CswNbtResources.Nodes[BatchNode.User.RelatedNodeId];
                    if( null != UserNode )
                    {
                        CswNbtObjClassUser UserOC = UserNode;
                        if( null != UserOC )
                        {
                            CswNbtResources.DoAuditing = true;
                            CswNbtResources.AuditContext = "Batch Op: " + BatchNode.OpNameValue;
                            CswNbtResources.AuditFirstName = UserOC.FirstName;
                            CswNbtResources.AuditLastName = UserOC.LastName;
                            CswNbtResources.AuditUsername = UserOC.Username;
                        }
                    }

                    op.runBatchOp( BatchNode );
                }
            }
        }

    } // class CswNbtBatchManager
} // namespace ChemSW.Nbt.Batch
