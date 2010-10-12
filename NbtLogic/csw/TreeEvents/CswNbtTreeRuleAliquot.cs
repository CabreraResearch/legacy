//using System;
//using System.Collections.Generic;
//using System.Text;
//using ChemSW.Core;
//using ChemSW.Nbt;

//namespace ChemSW.Nbt.TreeEvents
//{
//    public class CswNbtTreeRuleAliquot: ICswNbtTreeRule
//    {

//        private ICswNbtTree _CswNbtTree = null;
//        private CswDataObjects _CswDataObjects = null;
//        public CswNbtTreeRuleAliquot( ICswNbtTree CswNbtTree, CswDataObjects CswDataObjects )
//        {
//            _CswNbtTree = CswNbtTree;
//            _CswDataObjects = CswDataObjects;
//        }//ctor

//        #region ICswNbtTreeRule Members

//        public string getClassName()
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onAfterChangeParent( int ChildNodeId, int ChildNodeInstanceId, int OldParentNodeId, int OldNodeInstanceId, int NewParentNodeId, int NewParentNodeIdInstanceId, bool bApplyToDuplicates, bool bApplyToDbOnly )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onAfterDeleteNode( int NodeId, int NodeInstanceId, bool IsRecursive, bool bApplyToDuplicates )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onAfterInsertNode( int ParentNodeId, int ParentNodeInstanceId, int NodeTypeId, bool bApplyToDuplicates, int NewNodeId, int NewNodeInstanceId )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onAfterSetNodeName( int NodeId, int NodeInstanceId, string NodeName, bool bApplyToDuplicates )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onAfterSetNodeProperty( int NodeId, int NodeInstanceId, CswNodePropVal pPropVal, bool bApplyToDuplicates, bool bApplyToDb )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onBeforeChangeParent( int ChildNodeId, int ChildNodeInstanceId, int OldParentNodeId, int OldNodeInstanceId, int NewParentNodeId, int NewParentNodeIdInstanceId, bool bApplyToDuplicates, bool bApplyToDbOnly )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onBeforeDeleteNode( int NodeId, int NodeInstanceId, bool IsRecursive, bool bApplyToDuplicates )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onBeforeInsertNode( int ParentNodeId, int ParentNodeInstanceId, int NodeTypeId, bool bApplyToDuplicates )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onBeforeSetNodeName( int NodeId, int NodeInstanceId, string NodeName, bool bApplyToDuplicates )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void onBeforeSetNodeProperty( int NodeId, int NodeInstanceId, CswNodePropVal pPropVal, bool bApplyToDuplicates, bool bApplyToDb )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void setPostProcessRule( ICswNbtTreeRule pCswNbtRule )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        public void setPreProcessRule( ICswNbtTreeRule pCswNbtRule )
//        {
//            throw new Exception( "The method or operation is not implemented." );
//        }

//        #endregion
//    }//class CswNbtTreeRuleAliquot

//}//ChemSW.Nbt.TreeEvents
