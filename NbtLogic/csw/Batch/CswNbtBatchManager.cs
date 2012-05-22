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
                                                     Int32 Priority = Int32.MinValue )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            CswNbtMetaDataObjectClass BatchOpOC = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass );
            if( BatchOpOC != null )
            {
                CswNbtMetaDataNodeType BatchOpNT = BatchOpOC.getNodeTypes().First();
                if( BatchOpNT != null )
                {
                    CswNbtNode Node = CswNbtResources.Nodes.makeNodeFromNodeTypeId( BatchOpNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
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

    } // class CswNbtBatchManager
} // namespace ChemSW.Nbt.Batch
