//using System.Data;
//using System;
//using ChemSW.Core;

//namespace ChemSW.Nbt
//{
//  public interface ICswNbtTreeRule
//  {
//        void setPostProcessRule( ICswNbtTreeRule pCswNbtRule ) ;
//        void setPreProcessRule( ICswNbtTreeRule pCswNbtRule ) ;

//        string getClassName() ;

//        void onBeforeInsertNode( CswPrimaryKey ParentNodeId, Int32 ParentNodeInstanceId, int NodeTypeId, bool bApplyToDuplicates );
//        void onAfterInsertNode( CswPrimaryKey ParentNodeId, Int32 ParentNodeInstanceId, int NodeTypeId, bool bApplyToDuplicates, CswPrimaryKey NewNodeId, Int32 NewNodeInstanceId );

//        void onBeforeDeleteNode( CswPrimaryKey NodeId, Int32 NodeInstanceId, bool IsRecursive, bool bApplyToDuplicates );
//        void onAfterDeleteNode( CswPrimaryKey NodeId, Int32 NodeInstanceId, bool IsRecursive, bool bApplyToDuplicates );

//        void onBeforeChangeParent( CswPrimaryKey ChildNodeId, Int32 ChildNodeInstanceId, CswPrimaryKey OldParentNodeId, Int32 OldNodeInstanceId, CswPrimaryKey NewParentNodeId, Int32 NewParentNodeIdInstanceId, bool bApplyToDuplicates, bool bApplyToDbOnly );
//        void onAfterChangeParent( CswPrimaryKey ChildNodeId, Int32 ChildNodeInstanceId, CswPrimaryKey OldParentNodeId, Int32 OldNodeInstanceId, CswPrimaryKey NewParentNodeId, Int32 NewParentNodeIdInstanceId, bool bApplyToDuplicates, bool bApplyToDbOnly );

//        void onBeforeSetNodeProperty( CswPrimaryKey NodeId , Int32 NodeInstanceId , CswNodePropVal pPropVal, bool bApplyToDuplicates, bool bApplyToDb );
//        void onAfterSetNodeProperty( CswPrimaryKey NodeId, Int32 NodeInstanceId, CswNodePropVal pPropVal, bool bApplyToDuplicates, bool bApplyToDb );

//        void onBeforeSetNodeName( CswPrimaryKey NodeId, Int32 NodeInstanceId, string NodeName, bool bApplyToDuplicates );
//        void onAfterSetNodeName( CswPrimaryKey NodeId, Int32 NodeInstanceId, string NodeName, bool bApplyToDuplicates );
   
//  }//ICswNbtTreeRule

//}//namespace ChemSW.Core

