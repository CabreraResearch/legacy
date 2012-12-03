using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActGenerateFutureNodes
    {
        CswNbtResources _CswNbtResources;

        public CswNbtActGenerateFutureNodes( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public DateTime getDateOfLastExistingFutureNode( CswNbtNode CswNbtNodeGenerator )
        {
            DateTime ReturnVal = DateTime.MinValue.Date;

            CswNbtView FutureNodesView = getTreeViewOfFutureNodes( new CswNbtNode[] { CswNbtNodeGenerator } );
            ICswNbtTree TargetNodeTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, FutureNodesView, true, false, false );
            TargetNodeTree.goToRoot();
            if( TargetNodeTree.getChildNodeCount() > 0 )
            {
                TargetNodeTree.goToNthChild( 0 );//go to generator node
                int TotalTargetNodes = TargetNodeTree.getChildNodeCount();
                if( TotalTargetNodes > 0 )
                {
                    for( int idx = 0; idx < TotalTargetNodes; idx++ )
                    {
                        TargetNodeTree.goToNthChild( idx );
                        CswNbtPropertySetGeneratorTarget CurrentTargetNode = TargetNodeTree.getNodeForCurrentPosition();
                        
                        DateTime CurrentDate = CurrentTargetNode.DueDate.DateTimeValue;
                        if( CurrentDate.Date > ReturnVal.Date )
                        {
                            ReturnVal = CurrentDate.Date;
                        }
                        TargetNodeTree.goToParentNode();
                    } // for( int idx = 0; idx < TotalTargetNodes; idx++ )
                }//if we have nodes
            }//if we got a generator node

            return ( ReturnVal );
        }

        public CswNbtObjClassBatchOp makeNodesBatch( CswNbtNode CswNbtNodeGenerator, DateTime FutureDate )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            CswNbtObjClassGenerator GeneratorNode = CswNbtNodeGenerator;
            Int32 TargetNodeTypeId = CswConvert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds );

            // Must have create permissions on this generator's target's nodetype
            if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.MetaData.getNodeType( TargetNodeTypeId ) ) )
            {
                deleteExistingFutureNodes( CswNbtNodeGenerator );

                // Send to background task
                CswNbtBatchOpFutureNodes BatchOp = new CswNbtBatchOpFutureNodes( _CswNbtResources );
                BatchNode = BatchOp.makeBatchOp( CswNbtNodeGenerator, FutureDate );
            }

            return BatchNode;
        }


        public CswNbtView getTreeViewOfFutureNodes( IEnumerable GeneratorNodes )
        {
            CswNbtView ReturnVal = new CswNbtView( _CswNbtResources );
            ReturnVal.ViewName = "All Future Nodes";
            CswNbtMetaDataObjectClass GeneratorObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.GeneratorClass );
            CswNbtViewRelationship GeneratorRelationship = ReturnVal.AddViewRelationship( GeneratorObjectClass, false );

            ArrayList TargetNodeTypeIds = new ArrayList();
            foreach( CswNbtNode CurrentGeneratorNode in GeneratorNodes )
            {
                GeneratorRelationship.NodeIdsToFilterIn.Add( CurrentGeneratorNode.NodeId );
                CswNbtObjClassGenerator Generator = CurrentGeneratorNode;
                foreach( String nodeTypeId in Generator.TargetType.SelectedNodeTypeIds )
                {
                    Int32 CurrentTargetNodeTypeId = CswConvert.ToInt32( nodeTypeId );
                    if( !( TargetNodeTypeIds.Contains( CurrentTargetNodeTypeId ) ) )
                        TargetNodeTypeIds.Add( CurrentTargetNodeTypeId );
                }
            }

            foreach( Int32 TargetNodeTypeId in TargetNodeTypeIds )
            {
                CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( TargetNodeTypeId );
                if( TargetNodeType != null )
                {
                    CswNbtMetaDataObjectClass TargetObjectClass = TargetNodeType.getObjectClass();

                    CswNbtObjClass TargetObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, TargetObjectClass );
                    if( !( TargetObjClass is CswNbtPropertySetGeneratorTarget ) )
                        throw new CswDniException( "CswNbtActGenerateFutureNodes.getTreeViewOfFutureNodes() got an invalid object class: " + TargetObjectClass.ObjectClass );

                    CswNbtViewRelationship TargetRelationship = ReturnVal.AddViewRelationship( GeneratorRelationship, NbtViewPropOwnerType.Second, TargetNodeType.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.Generator ), false );
                    CswNbtViewProperty IsFutureFlagProperty = ReturnVal.AddViewProperty( TargetRelationship, TargetNodeType.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.IsFuture ) );
                    CswNbtViewPropertyFilter IsFutureFilter = ReturnVal.AddViewPropertyFilter( IsFutureFlagProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, "1", false );
                }
            }

            return ( ReturnVal );
        }//getTreeViewOfFutureNodes()

        public void deleteExistingFutureNodes( CswNbtNode GeneratorNode )
        {
            CswNbtView FutureNodesView = getTreeViewOfFutureNodes( new CswNbtNode[] { GeneratorNode } );
            ICswNbtTree TargetNodeTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, FutureNodesView, true, false, false );

            TargetNodeTree.goToRoot();
            if( TargetNodeTree.getChildNodeCount() > 0 )
            {
                TargetNodeTree.goToNthChild( 0 );//go to Generator node
                int TotalNodes = TargetNodeTree.getChildNodeCount();
                if( TotalNodes > 0 )
                {
                    for( int idx = 0; idx < TotalNodes; idx++ )
                    {
                        TargetNodeTree.goToNthChild( idx );
                        CswNbtNode CurrentNode = TargetNodeTree.getNodeForCurrentPosition();
                        CurrentNode.delete();
                        TargetNodeTree.goToParentNode();
                    }
                }//if we have nodes
            }//if we got a Generator node
        }

    }//class CswNbtActGenerateFutureNodes

}//namespace ChemSW.Actions
