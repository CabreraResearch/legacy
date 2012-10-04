using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActGenerateFutureNodes
    {
        CswNbtResources _CswNbtResources = null;
        //CswNbtActGenerateNodes _CswNbtActGenerateNodes = null;
        public CswNbtActGenerateFutureNodes( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            //_CswNbtActGenerateNodes = new CswNbtActGenerateNodes( _CswNbtResources );
            //_CswNbtActGenerateNodes.MarkFuture = true;
        }//ctor

        public DateTime getDateOfLastExistingFutureNode( CswNbtNode CswNbtNodeGenerator )
        {
            DateTime ReturnVal = DateTime.MinValue.Date;

            CswNbtView FutureNodesView = getTreeViewOfFutureNodes( new CswNbtNode[] { CswNbtNodeGenerator } );
            ICswNbtTree TargetNodeTree = _CswNbtResources.Trees.getTreeFromView( FutureNodesView, true, true, false, false );
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
                        CswNbtNode CurrentTargetNode = TargetNodeTree.getNodeForCurrentPosition();
                        ICswNbtPropertySetGeneratorTarget CurrentTargetNodeAsGeneratorTarget = CswNbtPropSetCaster.AsPropertySetGeneratorTarget( CurrentTargetNode );

                        DateTime CurrentDate = CurrentTargetNodeAsGeneratorTarget.GeneratedDate.DateTimeValue;
                        if( CurrentDate.Date > ReturnVal.Date )
                        {
                            ReturnVal = CurrentDate.Date;
                        }
                        TargetNodeTree.goToParentNode();

                    } // for( int idx = 0; idx < TotalTargetNodes; idx++ )

                }//if we have nodes

            }//if we got a generator node

            return ( ReturnVal );

        }//getDateOfLastExistingFutureNode()

        public CswNbtObjClassBatchOp makeNodesBatch( CswNbtNode CswNbtNodeGenerator, DateTime FutureDate )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            CswNbtObjClassGenerator GeneratorNode = (CswNbtObjClassGenerator) CswNbtNodeGenerator;
            Int32 TargetNodeTypeId = CswConvert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds );
            //Int32 ReturnVal = 0;

            // Must have create permissions on this generator's target's nodetype
            if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.MetaData.getNodeType( TargetNodeTypeId ) ) )
            {
                deleteExistingFutureNodes( CswNbtNodeGenerator );

                // Send to background task
                CswNbtBatchOpFutureNodes BatchOp = new CswNbtBatchOpFutureNodes( _CswNbtResources );
                BatchNode = BatchOp.makeBatchOp( CswNbtNodeGenerator, FutureDate );

                //// BZ 6752 - The first future node is the first node generated 
                //// after today + warning days, according to the time interval
                //// But it has to include initial due date, no matter what the time interval.

                //CswNbtNodePropTimeInterval NextDueDateTimeInterval = GeneratorNode.DueDateInterval;
                //Double WarningDays = 0;
                //if( GeneratorNode.WarningDays.Value > 0 )
                //    WarningDays = GeneratorNode.WarningDays.Value;
                //DateTime StartDate = DateTime.Now.AddDays( WarningDays ).Date; //bz# 6937 (capture date only, not time)

                //DateTime DateOfNextOccurance = DateTime.MinValue;
                //if( GeneratorNode.DueDateInterval.getStartDate().Date >= StartDate ) //bz # 6937 (change gt to gteq)
                //{
                //    StartDate = GeneratorNode.DueDateInterval.getStartDate().Date;
                //    DateOfNextOccurance = StartDate;
                //}
                //else
                //{
                //    DateOfNextOccurance = NextDueDateTimeInterval.getNextOccuranceAfter( StartDate );
                //}

                //DateTime PreviousDateOfNextOccurance = DateOfNextOccurance;  // infinite loop guard
                //while( DateOfNextOccurance.Date <= FutureDate.Date &&
                //       ( GeneratorNode.FinalDueDate.Empty || DateOfNextOccurance.Date <= GeneratorNode.FinalDueDate.DateTimeValue.Date ) )
                //{
                //    Int32 TasksGenerated = _CswNbtActGenerateNodes.makeNode( CswNbtNodeGenerator, DateOfNextOccurance );
                //    if( TasksGenerated > 0 )
                //    {
                //        ReturnVal += 1;
                //    }

                //    PreviousDateOfNextOccurance = DateOfNextOccurance;
                //    DateOfNextOccurance = NextDueDateTimeInterval.getNextOccuranceAfter( DateOfNextOccurance );
                //    if( DateOfNextOccurance == PreviousDateOfNextOccurance )
                //    {
                //        throw new CswDniException( ErrorType.Error, "Invalid Rate Interval", "While generating future tasks for Generator: " + CswNbtNodeGenerator.NodeId + ", the next calculated date was equal to the previous calculated date" );
                //    }

                //}//create nodes until we hit either the future date or the final date

                ////bz# 6130
                ////_CswNbtResources.Nodes.finalize();

            } // if( _CswNbtResources.Permit.can( NodeTypePermission.Create, TargetNodeTypeId ) )

            //return ( ReturnVal );
            return BatchNode;
        }//makeNodes()


        public CswNbtView getTreeViewOfFutureNodes( IEnumerable GeneratorNodes )
        {
            CswNbtView ReturnVal = new CswNbtView( _CswNbtResources );
            ReturnVal.ViewName = "All Future Nodes";

            CswNbtMetaDataObjectClass GeneratorObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.GeneratorClass );

            CswNbtViewRelationship GeneratorRelationship = ReturnVal.AddViewRelationship( GeneratorObjectClass, false );

            ArrayList TargetNodeTypeIds = new ArrayList();
            foreach( CswNbtNode CurrentGeneratorNode in GeneratorNodes )
            {
                GeneratorRelationship.NodeIdsToFilterIn.Add( CurrentGeneratorNode.NodeId );
                CswNbtObjClassGenerator Generator = (CswNbtObjClassGenerator) CurrentGeneratorNode;
                //if( Generator.TargetType.SelectedNodeTypeIds.Count != 0 )   // BZ 8544
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
                    if( !( TargetObjClass is ICswNbtPropertySetGeneratorTarget ) )
                        throw new CswDniException( "CswNbtActGenerateFutureNodes.getTreeViewOfFutureNodes() got an invalid object class: " + TargetObjectClass.ObjectClass.ToString() );
                    ICswNbtPropertySetGeneratorTarget GeneratorTarget = (ICswNbtPropertySetGeneratorTarget) TargetObjClass;

                    CswNbtViewRelationship TargetRelationship = ReturnVal.AddViewRelationship( GeneratorRelationship, NbtViewPropOwnerType.Second, TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetGeneratorPropertyName ), false );
                    //bz# 5959
                    CswNbtViewProperty IsFutureFlagProperty = ReturnVal.AddViewProperty( TargetRelationship, TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetIsFuturePropertyName ) );
                    CswNbtViewPropertyFilter IsFutureFilter = ReturnVal.AddViewPropertyFilter( IsFutureFlagProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, "1", false );
                }
            }

            return ( ReturnVal );

        }//getTreeViewOfFutureNodes()


        public void deleteExistingFutureNodes( CswNbtNode GeneratorNode )
        {
            CswNbtView FutureNodesView = getTreeViewOfFutureNodes( new CswNbtNode[] { GeneratorNode } );
            ICswNbtTree TargetNodeTree = _CswNbtResources.Trees.getTreeFromView( FutureNodesView, true, true, false, false );

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
                        //_CswNbtResources.Nodes.Delete( CurrentTaskNode.NodeId );
                        CurrentNode.delete();
                        TargetNodeTree.goToParentNode();
                    }//

                }//if we have nodes

            }//if we got a Generator node

        }//deleteExistingFutureTasks()

    }//class CswNbtActGenerateFutureNodes

}//namespace ChemSW.Actions
