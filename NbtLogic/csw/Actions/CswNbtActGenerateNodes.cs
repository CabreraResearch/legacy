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

            CswNbtObjClassGenerator GeneratorNode = (CswNbtObjClassGenerator) CswNbtNodeGenerator;

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
                        if( !( CreatedObjClass is CswNbtPropertySetGeneratorTarget ) )
                        {
                            throw new CswDniException( "CswNbtActGenerateNodes got an invalid object class: " + CreatedObjClass.ObjectClass.ToString() );
                        }
                        CswNbtPropertySetGeneratorTarget GeneratorTarget = (CswNbtPropertySetGeneratorTarget) CreatedObjClass;

                        // CreatedForNTP is the parent or owner of the new node. Inspections created for Inspection Targets, Tasks for Equipment, etc.
                        CswNbtMetaDataNodeTypeProp CreatedForNTP = CreatedNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.ParentPropertyName );
                        CswNbtMetaDataNodeTypeProp GeneratorNTP = CreatedNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorPropertyName );
                        //CreatedNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetIsFuturePropertyName );
                        CswNbtMetaDataNodeTypeProp DueDateNTP = CreatedNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratedDatePropertyName );

                        CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                        CswNbtView.ViewName = "Nodes for Generator";
                        //CswNbtViewRelationship GeneratorRelationship = CswNbtView.AddViewRelationship( GeneratorClass, false );
                        //GeneratorRelationship.NodeIdsToFilterIn.Add( CswNbtNodeGenerator.NodeId );
                        //CswNbtViewRelationship ChildRelationship = CswNbtView.AddViewRelationship( GeneratorRelationship, PropOwnerType.Second, TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetGeneratorPropertyName ), false );
                        //CswNbtViewProperty GeneratedDateProperty = CswNbtView.AddViewProperty( ChildRelationship, TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetGeneratedDatePropertyName ) );
                        //CswNbtViewPropertyFilter GeneratedDateFilter = CswNbtView.AddViewPropertyFilter( GeneratedDateProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, TargetDueDate.Date.ToShortDateString(), false );
                        CswNbtViewRelationship RootRelationship = CswNbtView.AddViewRelationship( CreatedNodeType, false );
                        CswNbtViewProperty CreatedForParentProp = CswNbtView.AddViewProperty( RootRelationship, CreatedForNTP );
                        CswNbtView.AddViewPropertyFilter( CreatedForParentProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, ParentPk.PrimaryKey.ToString(), false );
                        CswNbtViewProperty GeneratorProp = CswNbtView.AddViewProperty( RootRelationship, GeneratorNTP );
                        CswNbtView.AddViewPropertyFilter( GeneratorProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtNodeGenerator.NodeId.PrimaryKey.ToString(), false );
                        CswNbtViewProperty DueDateProp = CswNbtView.AddViewProperty( RootRelationship, DueDateNTP );
                        //Case 24572
                        CswNbtView.AddViewPropertyFilter( DueDateProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, TargetDateFilter, false );

                        ICswNbtTree ExistingNodesTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, CswNbtView, true, false, false );

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

        public bool makeNode( CswNbtNode CswNbtNodeGenerator )
        {
            return makeNode( CswNbtNodeGenerator, DateTime.MinValue );
        }

        /// <summary>
        /// Generates a future IGeneratorTarget node.  If an existing node has the same due date, no node is generated.
        /// </summary>
        /// <returns>True if a future node was generated</returns>
        public bool makeNode( CswNbtNode CswNbtNodeGenerator, DateTime DueDate )
        {
            Int32 NodesCreated = 0;

            CswNbtObjClassGenerator GeneratorNodeAsGenerator = (CswNbtObjClassGenerator) CswNbtNodeGenerator;

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
                DueDate = GeneratorNodeAsGenerator.NextDueDate.DateTimeValue.Date;
            }
            if( DueDate == DateTime.MinValue )
            {
                DueDate = GeneratorNodeAsGenerator.DueDateInterval.getStartDate().Date;
            }
            DateFilter = DueDate.ToShortDateString();

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
                ICswNbtTree ParentsTree = _CswNbtResources.Trees.getTreeFromView( ParentView, false, false, false );
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

            // case 26111 - only generate a few at a time, and only increment NextDueDate when we're completely done
            Int32 GeneratorTargetLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.generatortargetlimit.ToString() ) );
            if( Int32.MinValue == GeneratorTargetLimit )
            {
                GeneratorTargetLimit = 5;
            }

            foreach( CswPrimaryKey NewParentPk in Parents )
            {
                if( null != NewParentPk && NodesCreated < GeneratorTargetLimit )
                {
                    CswNbtPropertySetGeneratorTarget ExistingNode = _getTargetNodeForGenerator( CswNbtNodeGenerator, NewParentPk, DateFilter );
                    if( null == ExistingNode )
                    {
                        Collection<Int32> SelectedNodeTypeIds = new Collection<Int32>();
                        SelectedNodeTypeIds = GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.ToIntCollection();
                        foreach( Int32 refNodeTypeId in SelectedNodeTypeIds )
                        {
                            CswNbtMetaDataNodeType LatestVersionNT = _CswNbtResources.MetaData.getNodeType( refNodeTypeId ).getNodeTypeLatestVersion();

                            CswNbtPropertySetGeneratorTarget NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( LatestVersionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                            NewNode.Node.copyPropertyValues( CswNbtNodeGenerator );

                            NewNode.GeneratedDate.DateTimeValue = DueDate;
                            NewNode.GeneratedDate.setReadOnly( value: true, SaveToDb: true ); //bz # 5349
                            NewNode.Generator.RelatedNodeId = CswNbtNodeGenerator.NodeId;
                            NewNode.Generator.CachedNodeName = CswNbtNodeGenerator.NodeName;
                            NewNode.Parent.RelatedNodeId = NewParentPk;
                            //NewTaskNodeAsTask.Completed.Checked = Tristate.False;

                            if( MarkFuture )
                            {
                                NewNode.IsFuture.Checked = Tristate.True;
                            }
                            else
                            {
                                NewNode.IsFuture.Checked = Tristate.False;
                            }

                            if( null != onBeforeInsertNode )
                            {
                                onBeforeInsertNode( NewNode );
                            }
                            NodesCreated += 1;
                            NewNode.Node.PendingUpdate = true;
                            NewNode.postChanges( true );
                        }

                    } //if ( null == ExistingNode )
                    else
                    {
                        if( false == MarkFuture )
                        {
                            if( ExistingNode.IsFuture.Checked == Tristate.True )
                            {
                                ExistingNode.IsFuture.Checked = Tristate.False;
                            }
                        }
                        else
                        {
                            if( DateTime.Now.Date >= ExistingNode.GeneratedDate.DateTimeValue.Date )
                            {
                                ExistingNode.IsFuture.Checked = Tristate.False;
                            }
                        }
                        //ExistingNode.PendingUpdate = true;
                        ExistingNode.postChanges( false ); //BZ # 6961

                    } //if-else ( null == ExistingNode )
                } // if( null != NewParentPk )
            } // foreach( CswPrimaryKey NewParentPk in Parents )

            // case 26111 - we're finished if we ran out of nodes to generate
            return ( NodesCreated < GeneratorTargetLimit );

        }//makeNode()

    }// class CswNbtActGenerateNodes

}//ns ChemSW.Actions
