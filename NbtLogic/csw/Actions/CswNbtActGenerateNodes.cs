using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActGenerateNodes
    {
        CswNbtResources _CswNbtResources = null;
        public CswNbtActGenerateNodes( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public delegate void GenerateNodeHandler( object CswNbtNode );
        public GenerateNodeHandler onBeforeInsertNode = null;

        public bool MarkFuture = false;

        private CswNbtNode _getTargetNodeForGenerator( CswNbtNode CswNbtNodeGenerator, CswPrimaryKey ParentPk, string TargetDateFilter )
        {
            CswNbtNode ReturnVal = null;

            CswNbtObjClassGenerator GeneratorNode = CswNbtNodeCaster.AsGenerator( CswNbtNodeGenerator );

            if( GeneratorNode.TargetType.SelectMode == PropertySelectMode.Single )
            {
                Int32 NodeTypeId = CswConvert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds.ToString() );
                if( Int32.MinValue != NodeTypeId )
                {
                    CswNbtMetaDataNodeType ThisCreatedNodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                    if( null != ThisCreatedNodeType )
                    {
                        CswNbtMetaDataNodeType CreatedNodeType = ThisCreatedNodeType.getNodeTypeLatestVersion();
                        CswNbtMetaDataObjectClass CreatedMetaDataObjectClass = CreatedNodeType.getObjectClass();

                        CswNbtObjClass CreatedObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, CreatedMetaDataObjectClass );
                        if( !( CreatedObjClass is ICswNbtPropertySetGeneratorTarget ) )
                        {
                            throw new CswDniException( "CswNbtActGenerateNodes got an invalid object class: " + CreatedObjClass.ObjectClass.ToString() );
                        }
                        ICswNbtPropertySetGeneratorTarget GeneratorTarget = (ICswNbtPropertySetGeneratorTarget) CreatedObjClass;

                        // CreatedForNTP is the parent or owner of the new node. Inspections created for Inspection Targets, Tasks for Equipment, etc.
                        CswNbtMetaDataNodeTypeProp CreatedForNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetParentPropertyName );
                        CswNbtMetaDataNodeTypeProp GeneratorNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetGeneratorPropertyName );
                        //CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetIsFuturePropertyName );
                        CswNbtMetaDataNodeTypeProp DueDateNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetGeneratedDatePropertyName );

                        CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                        CswNbtView.ViewName = "Nodes for Generator";
                        //CswNbtViewRelationship GeneratorRelationship = CswNbtView.AddViewRelationship( GeneratorClass, false );
                        //GeneratorRelationship.NodeIdsToFilterIn.Add( CswNbtNodeGenerator.NodeId );
                        //CswNbtViewRelationship ChildRelationship = CswNbtView.AddViewRelationship( GeneratorRelationship, CswNbtViewRelationship.PropOwnerType.Second, TargetNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetGeneratorPropertyName ), false );
                        //CswNbtViewProperty GeneratedDateProperty = CswNbtView.AddViewProperty( ChildRelationship, TargetNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetGeneratedDatePropertyName ) );
                        //CswNbtViewPropertyFilter GeneratedDateFilter = CswNbtView.AddViewPropertyFilter( GeneratedDateProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, TargetDueDate.Date.ToShortDateString(), false );
                        CswNbtViewRelationship RootRelationship = CswNbtView.AddViewRelationship( CreatedNodeType, false );
                        CswNbtViewProperty CreatedForParentProp = CswNbtView.AddViewProperty( RootRelationship, CreatedForNTP );
                        CswNbtView.AddViewPropertyFilter( CreatedForParentProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, ParentPk.PrimaryKey.ToString(), false );
                        CswNbtViewProperty GeneratorProp = CswNbtView.AddViewProperty( RootRelationship, GeneratorNTP );
                        CswNbtView.AddViewPropertyFilter( GeneratorProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtNodeGenerator.NodeId.PrimaryKey.ToString(), false );
                        CswNbtViewProperty DueDateProp = CswNbtView.AddViewProperty( RootRelationship, DueDateNTP );
                        //Case 24572
                        CswNbtView.AddViewPropertyFilter( DueDateProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, TargetDateFilter, false );

                        ICswNbtTree ExistingNodesTree = _CswNbtResources.Trees.getTreeFromView( CswNbtView, true, true, false, false );

                        if( ExistingNodesTree.getChildNodeCount() > 0 )
                        {
                            ExistingNodesTree.goToNthChild( 0 );
                            ReturnVal = ExistingNodesTree.getNodeForCurrentPosition();
                        }

                    }
                }
                //else
                //{
                //    Collection<Int32> NodeTypeIds = new Collection<Int32>();
                //    NodeTypeIds = GeneratorNode.TargetType.SelectedNodeTypeIds.ToIntCollection();
                //    foreach( Int32 NodeTypeId in NodeTypeIds ) 
                //}
            }
            return ( ReturnVal );

        }//_getTargetNodeForGenerator

        public Int32 makeNode( CswNbtNode CswNbtNodeGenerator )
        {
            return makeNode( CswNbtNodeGenerator, DateTime.MinValue );
        }

        /// <summary>
        /// Generates a future IGeneratorTarget node.  If an existing node has the same due date, no node is generated.
        /// </summary>
        /// <returns>True if a future node was generated</returns>
        public Int32 makeNode( CswNbtNode CswNbtNodeGenerator, DateTime DueDate )
        {
            Int32 ret = 0;

            CswNbtObjClassGenerator GeneratorNodeAsGenerator = CswNbtNodeCaster.AsGenerator( CswNbtNodeGenerator );

            string SelectedNodeTypeIdStr = string.Empty;
            Int32 SelectedNodeTypeId = Int32.MinValue;
            if( 0 < GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.Count )
            {
                SelectedNodeTypeIdStr = GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds[0];
                SelectedNodeTypeId = CswConvert.ToInt32( SelectedNodeTypeIdStr );
            }

            if( string.IsNullOrEmpty( SelectedNodeTypeIdStr ) ||
                "0" == SelectedNodeTypeIdStr ||
                Int32.MinValue == SelectedNodeTypeId ||
                null == _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId ) )
            {
                throw ( new CswDniException( "Generator node " + CswNbtNodeGenerator.NodeName + " (" + CswNbtNodeGenerator.NodeId.ToString() + ") does not have a valid nodetypeid" ) );
            }

            string DateFilter = string.Empty;
            if( DueDate == DateTime.MinValue )
            {
                DueDate = GeneratorNodeAsGenerator.NextDueDate.DateTimeValue;
                DateFilter = DueDate.ToShortDateString() + " " + DueDate.ToLongTimeString();
            }
            if( DueDate == DateTime.MinValue || string.IsNullOrEmpty( DateFilter ) )
            {
                DueDate = GeneratorNodeAsGenerator.DueDateInterval.getStartDate();
                DateFilter = DueDate.ToShortDateString();
            }

            bool GeneratorBaseIsProperlyConfigured = ( null != GeneratorNodeAsGenerator.Owner &&
                                                   null != GeneratorNodeAsGenerator.Owner.RelatedNodeId &&
                                                   null != _CswNbtResources.Nodes.GetNode( GeneratorNodeAsGenerator.Owner.RelatedNodeId ) &&
                                                   GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.Count > 0 );

            if( false == GeneratorBaseIsProperlyConfigured )
            {
                throw new CswDniException( ErrorType.Error, "Cannot execute generator task if the generator does not have an owner and a target type.", "Generator node did not define both an Owner and a Target Type." );
            }
            Collection<CswPrimaryKey> Parents = new Collection<CswPrimaryKey>();

            //SI will have a ParentView to fetch InspectionTargets which will be used to find existing InsepctionDesign nodes or create new ones
            CswNbtView ParentView = null;
            if( GeneratorNodeAsGenerator.ParentView.ViewId.isSet() )
            {
                ParentView = _CswNbtResources.ViewSelect.restoreView( GeneratorNodeAsGenerator.ParentView.ViewId );
            }
            bool GeneratorUsesParentViews = ( null != ParentView &&
                                              false == ParentView.IsEmpty() &&
                                              GeneratorNodeAsGenerator.ParentType.SelectedNodeTypeIds.Count > 0 );
            if( GeneratorUsesParentViews )
            {
                // Case 20482
                ( ParentView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( GeneratorNodeAsGenerator.NodeId );
                ICswNbtTree ParentsTree = _CswNbtResources.Trees.getTreeFromView( ParentView, false );
                if( GeneratorNodeAsGenerator.ParentType.SelectMode == PropertySelectMode.Single )
                {
                    Int32 ParentNtId = CswConvert.ToInt32( GeneratorNodeAsGenerator.ParentType.SelectedNodeTypeIds[0] );
                    Parents = ParentsTree.getNodeKeysOfNodeType( ParentNtId );
                }
            }

            //IMCS won't have a ParentView or a ParentType
            bool GeneratorDoesNotUseParentViews = ( false == GeneratorUsesParentViews &&
                                                    Parents.Count == 0 );

            if( GeneratorDoesNotUseParentViews )
            {
                Parents.Add( GeneratorNodeAsGenerator.Owner.RelatedNodeId );
            }

            foreach( CswPrimaryKey NewParentPk in Parents )
            {
                if( null != NewParentPk )
                {
                    CswNbtNode ExistingNode = _getTargetNodeForGenerator( CswNbtNodeGenerator, NewParentPk, DateFilter );

                    bool MakeGeneratorTarget = ( null == ExistingNode );
                    if( MakeGeneratorTarget )
                    {
                        Collection<Int32> SelectedNodeTypeIds = new Collection<Int32>();
                        SelectedNodeTypeIds = GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.ToIntCollection();
                        foreach( Int32 refNodeTypeId in SelectedNodeTypeIds )
                        {
                            CswNbtMetaDataNodeType LatestVersionNT = _CswNbtResources.MetaData.getNodeType( refNodeTypeId ).getNodeTypeLatestVersion();

                            CswNbtNode NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( LatestVersionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                            NewNode.copyPropertyValues( CswNbtNodeGenerator );

                            ICswNbtPropertySetGeneratorTarget NewNodeAsGeneratorTarget = CswNbtNodeCaster.AsPropertySetGeneratorTarget( NewNode );
                            NewNodeAsGeneratorTarget.GeneratedDate.DateTimeValue = DueDate;
                            NewNodeAsGeneratorTarget.GeneratedDate.ReadOnly = true; //bz # 5349
                            NewNodeAsGeneratorTarget.Generator.RelatedNodeId = CswNbtNodeGenerator.NodeId;
                            NewNodeAsGeneratorTarget.Generator.CachedNodeName = CswNbtNodeGenerator.NodeName;
                            NewNodeAsGeneratorTarget.Parent.RelatedNodeId = NewParentPk;
                            //NewTaskNodeAsTask.Completed.Checked = Tristate.False;

                            if( MarkFuture )
                            {
                                NewNodeAsGeneratorTarget.IsFuture.Checked = Tristate.True;
                            }
                            else
                            {
                                NewNodeAsGeneratorTarget.IsFuture.Checked = Tristate.False;
                            }

                            if( null != onBeforeInsertNode )
                            {
                                onBeforeInsertNode( NewNode );
                            }
                            ret += 1;
                            NewNode.PendingUpdate = true;
                            NewNode.postChanges( true );
                        }

                    } //if ( null == ExistingNode )
                    else
                    {
                        ICswNbtPropertySetGeneratorTarget ExistingNodeAsGeneratorTarget = CswNbtNodeCaster.AsPropertySetGeneratorTarget( ExistingNode );
                        if( !MarkFuture )
                        {
                            if( ExistingNodeAsGeneratorTarget.IsFuture.Checked == Tristate.True )
                            {
                                ExistingNodeAsGeneratorTarget.IsFuture.Checked = Tristate.False;
                            }
                        }
                        else
                        {
                            if( DateTime.Now.Date >= ExistingNodeAsGeneratorTarget.GeneratedDate.DateTimeValue.Date )
                            {
                                ExistingNodeAsGeneratorTarget.IsFuture.Checked = Tristate.False;
                            }
                        }
                        ExistingNode.PendingUpdate = true;
                        ExistingNode.postChanges( false ); //BZ # 6961

                    } //if-else ( null == ExistingNode )
                } // if( null != NewParentPk )
            } // foreach( CswPrimaryKey NewParentPk in Parents )

            return ret;

        }//makeNode()

    }// class CswNbtActGenerateNodes

}//ns ChemSW.Actions
