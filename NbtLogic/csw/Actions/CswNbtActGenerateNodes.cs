using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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

            if( String.IsNullOrEmpty( GeneratorNode.TargetType.SelectedNodeTypeIds.ToString() ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid generator configuration", "_getTargetNodeForGenerator got a null SelectedNodeTypeIds on nodeid " + GeneratorNode.NodeId );
            }
            else
            {
                Int32 NodeTypeId = CswConvert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds[0] ); // case 28612 - just check the first one
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
                        CswNbtMetaDataNodeTypeProp GeneratorNTP = CreatedNodeType.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.Generator );
                        CswNbtMetaDataNodeTypeProp DueDateNTP = CreatedNodeType.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.DueDate );

                        CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                        CswNbtView.ViewName = "Nodes for Generator";
                        
                        CswNbtViewRelationship RootRelationship = CswNbtView.AddViewRelationship( CreatedNodeType, false );
                        CswNbtViewProperty CreatedForParentProp = CswNbtView.AddViewProperty( RootRelationship, CreatedForNTP );
                        CswNbtView.AddViewPropertyFilter( CreatedForParentProp, CswEnumNbtSubFieldName.NodeID, CswEnumNbtFilterMode.Equals, ParentPk.PrimaryKey.ToString(), false );
                        CswNbtViewProperty GeneratorProp = CswNbtView.AddViewProperty( RootRelationship, GeneratorNTP );
                        CswNbtView.AddViewPropertyFilter( GeneratorProp, CswEnumNbtSubFieldName.NodeID, CswEnumNbtFilterMode.Equals, CswNbtNodeGenerator.NodeId.PrimaryKey.ToString(), false );
                        CswNbtViewProperty DueDateProp = CswNbtView.AddViewProperty( RootRelationship, DueDateNTP );
                        //Case 24572
                        CswNbtView.AddViewPropertyFilter( DueDateProp, CswEnumNbtSubFieldName.Value, CswEnumNbtFilterMode.Equals, TargetDateFilter, false );

                        ICswNbtTree ExistingNodesTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, CswNbtView, true, false, false );

                        if( ExistingNodesTree.getChildNodeCount() > 0 )
                        {
                            ExistingNodesTree.goToNthChild( 0 );
                            ReturnVal = ExistingNodesTree.getNodeForCurrentPosition();
                        }

                    }
                }
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

            if( DueDate == DateTime.MinValue )
            {
                DueDate = GeneratorNodeAsGenerator.NextDueDate.DateTimeValue.Date;
            }
            if( DueDate == DateTime.MinValue )
            {
                DueDate = GeneratorNodeAsGenerator.DueDateInterval.getStartDate().Date;
            }
            string DateFilter = DueDate.ToShortDateString();

            bool GeneratorBaseIsProperlyConfigured = ( null != GeneratorNodeAsGenerator.Owner &&
                                                       null != GeneratorNodeAsGenerator.Owner.RelatedNodeId &&
                                                       null != _CswNbtResources.Nodes.GetNode( GeneratorNodeAsGenerator.Owner.RelatedNodeId ) &&
                                                       GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.Count > 0 );

            if( false == GeneratorBaseIsProperlyConfigured )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot execute generator task if the generator does not have an owner and a target type.", "Generator node did not define both an Owner and a Target Type." );
            }
            
            // case 26111 - only generate a few at a time, and only increment NextDueDate when we're completely done
            Int32 GeneratorTargetLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.generatortargetlimit.ToString() ) );
            if( Int32.MinValue == GeneratorTargetLimit )
            {
                GeneratorTargetLimit = 5;
            }

            foreach( CswPrimaryKey NewParentPk in GeneratorNodeAsGenerator.TargetParents )
            {
                if( null != NewParentPk && NodesCreated < GeneratorTargetLimit )
                {
                    CswNbtPropertySetGeneratorTarget ExistingNode = null;
                    if( GeneratorNodeAsGenerator.DueDateInterval.RateInterval.RateType != CswEnumRateIntervalType.Hourly )
                    {
                        ExistingNode = _getTargetNodeForGenerator( CswNbtNodeGenerator, NewParentPk, DateFilter );
                    }
                    if( null == ExistingNode )
                    {
                        Collection<Int32> SelectedNodeTypeIds = GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.ToIntCollection();
                        foreach( Int32 refNodeTypeId in SelectedNodeTypeIds )
                        {
                            CswNbtMetaDataNodeType LatestVersionNT = _CswNbtResources.MetaData.getNodeType( refNodeTypeId ).getNodeTypeLatestVersion();

                            CswNbtPropertySetGeneratorTarget NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( LatestVersionNT.NodeTypeId, CswEnumNbtMakeNodeOperation.DoNothing );
                            NewNode.Node.copyPropertyValues( CswNbtNodeGenerator );

                            NewNode.DueDate.DateTimeValue = DueDate;
                            NewNode.DueDate.setReadOnly( value: true, SaveToDb: true ); //bz # 5349
                            NewNode.Generator.RelatedNodeId = CswNbtNodeGenerator.NodeId;
                            NewNode.Generator.CachedNodeName = CswNbtNodeGenerator.NodeName;
                            NewNode.Parent.RelatedNodeId = NewParentPk;

                            if( MarkFuture )
                            {
                                NewNode.IsFuture.Checked = CswEnumTristate.True;
                            }
                            else
                            {
                                NewNode.IsFuture.Checked = CswEnumTristate.False;
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
                            if( ExistingNode.IsFuture.Checked == CswEnumTristate.True )
                            {
                                ExistingNode.IsFuture.Checked = CswEnumTristate.False;
                            }
                        }
                        else
                        {
                            if( DateTime.Now.Date >= ExistingNode.DueDate.DateTimeValue.Date )
                            {
                                ExistingNode.IsFuture.Checked = CswEnumTristate.False;
                            }
                        }
                        ExistingNode.postChanges( false ); //BZ # 6961

                    } //if-else ( null == ExistingNode )
                } // if( null != NewParentPk )
            } // foreach( CswPrimaryKey NewParentPk in Parents )

            // case 26111 - we're finished if we ran out of nodes to generate
            return ( NodesCreated < GeneratorTargetLimit );

        }//makeNode()

    }// class CswNbtActGenerateNodes

}//ns ChemSW.Actions
