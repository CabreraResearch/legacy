using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
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

        private CswNbtNode _getTargetNodeForGenerator( CswNbtNode CswNbtNodeGenerator, CswPrimaryKey ParentPk, DateTime TargetDueDate )
        {
            CswNbtNode ReturnVal = null;

            CswNbtObjClassGenerator GeneratorNode = CswNbtNodeCaster.AsGenerator( CswNbtNodeGenerator );
            CswNbtMetaDataObjectClass GeneratorClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            // CreatedNodeType will be of InspectionDesign, Task, etc.
            CswNbtMetaDataNodeType CreatedNodeType = _CswNbtResources.MetaData.getNodeType( Convert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds ) ).LatestVersionNodeType;
            CswNbtMetaDataObjectClass CreatedMetaDataObjectClass = CreatedNodeType.ObjectClass;

            CswNbtObjClass CreatedObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, CreatedMetaDataObjectClass );
            if( !( CreatedObjClass is ICswNbtPropertySetGeneratorTarget ) )
                throw new CswDniException( "CswNbtActGenerateNodes got an invalid object class: " + CreatedObjClass.ObjectClass.ToString() );
            ICswNbtPropertySetGeneratorTarget GeneratorTarget = (ICswNbtPropertySetGeneratorTarget) CreatedObjClass;

            // CreatedForNTP is the parent or owner of the new node. Inspections created for Mount Points, Tasks for Equipment, etc.
            CswNbtMetaDataNodeTypeProp CreatedForNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetParentPropertyName );
            CswNbtMetaDataNodeTypeProp GeneratorNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetGeneratorPropertyName );
            CswNbtMetaDataNodeTypeProp IsFutureNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetIsFuturePropertyName );
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
            CswNbtViewPropertyFilter CreatedForFilter = CswNbtView.AddViewPropertyFilter( CreatedForParentProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, ParentPk.PrimaryKey.ToString(), false );
            CswNbtViewProperty GeneratorProp = CswNbtView.AddViewProperty( RootRelationship, GeneratorNTP );
            CswNbtViewPropertyFilter GeneratorFilter = CswNbtView.AddViewPropertyFilter( GeneratorProp, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtNodeGenerator.NodeId.PrimaryKey.ToString(), false );
            CswNbtViewProperty DueDateProp = CswNbtView.AddViewProperty( RootRelationship, DueDateNTP );
            CswNbtViewPropertyFilter DueDateFilter = CswNbtView.AddViewPropertyFilter( DueDateProp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, TargetDueDate.ToShortDateString(), false );

            ICswNbtTree ExistingNodesTree = _CswNbtResources.Trees.getTreeFromView( CswNbtView, true, true, false, false );

            if( ExistingNodesTree.getChildNodeCount() > 0 )
            {
                ExistingNodesTree.goToNthChild( 0 );
                ReturnVal = ExistingNodesTree.getNodeForCurrentPosition();
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
            bool ret = false;

            CswNbtObjClassGenerator GeneratorNode = CswNbtNodeCaster.AsGenerator( CswNbtNodeGenerator );

            if ( string.Empty == GeneratorNode.TargetType.SelectedNodeTypeIds ||
                "0" == GeneratorNode.TargetType.SelectedNodeTypeIds ||
                null == _CswNbtResources.MetaData.getNodeType( Convert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds ) ) )
            {
                throw ( new CswDniException( "Generator node " + CswNbtNodeGenerator.NodeName + " (" + CswNbtNodeGenerator.NodeId.ToString() + ") does not have a valid nodetypeid" ) );
            }

            if ( DueDate == DateTime.MinValue )
                DueDate = GeneratorNode.NextDueDate.DateValue.Date;
            if ( DueDate == DateTime.MinValue )
                DueDate = GeneratorNode.DueDateInterval.getStartDate();

            Collection<CswPrimaryKey> Parents = new Collection<CswPrimaryKey>();
            if ( null == GeneratorNode.ParentView )
                Parents.Add( GeneratorNode.Owner.RelatedNodeId );
            else
            {
                CswNbtView ParentsView = CswNbtViewFactory.restoreView( _CswNbtResources, GeneratorNode.ParentView.ViewId );
                ICswNbtTree ParentsTree = _CswNbtResources.Trees.getTreeFromView( ParentsView, false, true, false, true );
                Parents = ParentsTree.getNodeKeysOfNodeType( CswConvert.ToInt32( GeneratorNode.ParentType.SelectedNodeTypeIds ) );
            }

            foreach ( CswPrimaryKey NewParentPK in Parents )
            {
                CswNbtNode ExistingNode = _getTargetNodeForGenerator( CswNbtNodeGenerator, NewParentPK, DueDate );
                if ( null == ExistingNode )
                {
                    Int32 refNodeTypeId = Convert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds );
                    CswNbtMetaDataNodeType LatestVersionNT = _CswNbtResources.MetaData.getNodeType( refNodeTypeId ).LatestVersionNodeType;

                    CswNbtNode NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( LatestVersionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                    NewNode.copyPropertyValues( CswNbtNodeGenerator );

                    ICswNbtPropertySetGeneratorTarget NewNodeAsGeneratorTarget = CswNbtNodeCaster.AsPropertySetGeneratorTarget( NewNode );
                    NewNodeAsGeneratorTarget.GeneratedDate.DateValue = DueDate;
                    NewNodeAsGeneratorTarget.GeneratedDate.ReadOnly = true;  //bz # 5349
                    NewNodeAsGeneratorTarget.Generator.RelatedNodeId = CswNbtNodeGenerator.NodeId;
                    NewNodeAsGeneratorTarget.Generator.CachedNodeName = CswNbtNodeGenerator.NodeName;
                    NewNodeAsGeneratorTarget.Parent.RelatedNodeId = NewParentPK;
                    //NewTaskNodeAsTask.Completed.Checked = Tristate.False;

                    if ( MarkFuture )
                        NewNodeAsGeneratorTarget.IsFuture.Checked = Tristate.True;
                    else
                        NewNodeAsGeneratorTarget.IsFuture.Checked = Tristate.False;

                    if ( null != onBeforeInsertNode )
                    {
                        onBeforeInsertNode( NewNode );
                    }
                    ret = true;
                    NewNode.PendingUpdate = true;
                    NewNode.postChanges( true );
                }//if ( null == ExistingNode )

                else
                {
                    ICswNbtPropertySetGeneratorTarget ExistingNodeAsGeneratorTarget = CswNbtNodeCaster.AsPropertySetGeneratorTarget( ExistingNode );
                    if ( !MarkFuture )
                    {
                        if ( ExistingNodeAsGeneratorTarget.IsFuture.Checked == Tristate.True )
                        {
                            ExistingNodeAsGeneratorTarget.IsFuture.Checked = Tristate.False;
                        }
                    }
                    else
                    {
                        if ( DateTime.Now.Date >= ExistingNodeAsGeneratorTarget.GeneratedDate.DateValue.Date )
                        {
                            ExistingNodeAsGeneratorTarget.IsFuture.Checked = Tristate.False;
                        }
                    }
                    ExistingNode.PendingUpdate = true;
                    ExistingNode.postChanges( false ); //BZ # 6961

                }//if-else ( null == ExistingNode )
            }//foreach ( CswPrimaryKey NewParentPK in Parents )

            return ret;

        }//makeNode()

    }// class CswNbtActGenerateNodes

}//ns ChemSW.Actions
