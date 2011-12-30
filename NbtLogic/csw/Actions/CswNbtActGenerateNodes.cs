using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

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
            CswNbtMetaDataObjectClass GeneratorClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            if( GeneratorNode.TargetType.SelectMode == PropertySelectMode.Single )
            {
                Int32 NodeTypeId = CswConvert.ToInt32( GeneratorNode.TargetType.SelectedNodeTypeIds.ToString() );
                CswNbtMetaDataNodeType CreatedNodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId ).LatestVersionNodeType;
                CswNbtMetaDataObjectClass CreatedMetaDataObjectClass = CreatedNodeType.ObjectClass;

                CswNbtObjClass CreatedObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, CreatedMetaDataObjectClass );
                if( !( CreatedObjClass is ICswNbtPropertySetGeneratorTarget ) )
                {
                    throw new CswDniException( "CswNbtActGenerateNodes got an invalid object class: " + CreatedObjClass.ObjectClass.ToString() );
                }
                ICswNbtPropertySetGeneratorTarget GeneratorTarget = (ICswNbtPropertySetGeneratorTarget) CreatedObjClass;

                // CreatedForNTP is the parent or owner of the new node. Inspections created for Inspection Targets, Tasks for Equipment, etc.
                CswNbtMetaDataNodeTypeProp CreatedForNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetParentPropertyName );
                CswNbtMetaDataNodeTypeProp GeneratorNTP = CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetGeneratorPropertyName );
                CreatedNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetIsFuturePropertyName );
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
            //else
            //{
            //    Collection<Int32> NodeTypeIds = new Collection<Int32>();
            //    NodeTypeIds = GeneratorNode.TargetType.SelectedNodeTypeIds.ToIntCollection();
            //    foreach( Int32 NodeTypeId in NodeTypeIds ) 
            //}
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

            CswNbtObjClassGenerator GeneratorNodeAsGenerator = CswNbtNodeCaster.AsGenerator( CswNbtNodeGenerator );

            if( 0 == GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.Count ||
                "0" == GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds[0] ||
                null == _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds[0] ) ) )
            {
                throw ( new CswDniException( "Generator node " + CswNbtNodeGenerator.NodeName + " (" + CswNbtNodeGenerator.NodeId.ToString() + ") does not have a valid nodetypeid" ) );
            }

            string DateFilter = string.Empty;
            if( DueDate == DateTime.MinValue )
            {
                switch( GeneratorNodeAsGenerator.NextDueDate.DisplayMode )
                {
                    case CswNbtNodePropDateTime.DateDisplayMode.Date:
                        DueDate = GeneratorNodeAsGenerator.NextDueDate.DateTimeValue.Date;
                        DateFilter = DueDate.ToShortDateString();
                        break;
                    case CswNbtNodePropDateTime.DateDisplayMode.DateTime:
                        DueDate = GeneratorNodeAsGenerator.NextDueDate.DateTimeValue;
                        DateFilter = DueDate.ToShortDateString() + " " + DueDate.ToLongTimeString();
                        break;
                }
            }
            if( DueDate == DateTime.MinValue || string.IsNullOrEmpty( DateFilter ) )
            {
                DueDate = GeneratorNodeAsGenerator.DueDateInterval.getStartDate();
                DateFilter = DueDate.ToShortDateString();
            }

            Collection<CswPrimaryKey> Parents = new Collection<CswPrimaryKey>();

            //SI will have a ParentView to fetch InspectionTargets which will be used to find existing InsepctionDesign nodes or create new ones
            bool IsSI = GeneratorNodeAsGenerator.ParentView.ViewId.isSet();
            if( IsSI )
            {
                CswNbtView ParentsView = _CswNbtResources.ViewSelect.restoreView( GeneratorNodeAsGenerator.ParentView.ViewId );
                // Case 20482
                if( ParentsView.Root.ChildRelationships.Count > 0 )
                {
                    ( (CswNbtViewRelationship) ParentsView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( GeneratorNodeAsGenerator.NodeId );
                    ICswNbtTree ParentsTree = _CswNbtResources.Trees.getTreeFromView( ParentsView, false, true, false, true );
                    if( GeneratorNodeAsGenerator.ParentType.SelectMode == PropertySelectMode.Single )
                    {
                        Int32 ParentNtId = CswConvert.ToInt32( GeneratorNodeAsGenerator.ParentType.SelectedNodeTypeIds.ToString() );
                        Parents = ParentsTree.getNodeKeysOfNodeType( ParentNtId );
                    }
                }
            }

            //IMCS won't have a ParentView or a ParentType
            bool IsIMCS = ( false == IsSI &&
                            Parents.Count == 0 &&
                            string.Empty == GeneratorNodeAsGenerator.ParentType.SelectedNodeTypeIds.ToString() &&
                            null != GeneratorNodeAsGenerator.Owner &&
                            null != GeneratorNodeAsGenerator.Owner.RelatedNodeId );

            if( IsIMCS )
            {
                Parents.Add( GeneratorNodeAsGenerator.Owner.RelatedNodeId );
            }

            foreach( CswPrimaryKey NewParentPK in Parents )
            {
                if( null != NewParentPK )
                {
                    CswNbtNode ExistingNode = _getTargetNodeForGenerator( CswNbtNodeGenerator, NewParentPK, DateFilter );
                    ICswNbtPropertySetGeneratorTarget ExistingNodeAsGeneratorTarget;

                    bool MakeGeneratorTarget = ( null == ExistingNode );
                    /* Case 24572 */
                    if( false == MakeGeneratorTarget )
                    {
                        ExistingNodeAsGeneratorTarget = CswNbtNodeCaster.AsPropertySetGeneratorTarget( ExistingNode );
                        if( IsSI )
                        {
                            try
                            {
                                /* Make sure that the existing target matches the target type of the generator */
                                Int32 GeneratorTargetNodeTypeId = CswConvert.ToInt32( GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.ToString() );
                                CswNbtMetaDataNodeType GeneratorTargetNodeType = _CswNbtResources.MetaData.getNodeType( GeneratorTargetNodeTypeId );
                                MakeGeneratorTarget = ( GeneratorTargetNodeType.LatestVersionNodeType != ExistingNode.NodeType.LatestVersionNodeType );
                                if( false == MakeGeneratorTarget )
                                {
                                    /* Make sure the Inspection Target matches the Generator's Parent Type */
                                    Int32 GeneratorParentTypeNodeTypeId = CswConvert.ToInt32( GeneratorNodeAsGenerator.ParentType.SelectedNodeTypeIds.ToString() );
                                    CswNbtMetaDataNodeType GeneratorParentType = _CswNbtResources.MetaData.getNodeType( GeneratorParentTypeNodeTypeId );
                                    CswNbtObjClassInspectionDesign ExistingNodeAsInspectionDesign = CswNbtNodeCaster.AsInspectionDesign( ExistingNode );
                                    CswNbtMetaDataNodeType InspectionOwnerType = _CswNbtResources.MetaData.getNodeType( ExistingNodeAsInspectionDesign.Target.RelatedNodeId.PrimaryKey );
                                    MakeGeneratorTarget = ( GeneratorParentType.LatestVersionNodeType != InspectionOwnerType.LatestVersionNodeType );
                                }
                            }
                            catch
                            {
                                MakeGeneratorTarget = true;
                            }

                        }
                    }

                    if( MakeGeneratorTarget )
                    {
                        Collection<Int32> SelectedNodeTypeIds = new Collection<Int32>();
                        SelectedNodeTypeIds = GeneratorNodeAsGenerator.TargetType.SelectedNodeTypeIds.ToIntCollection();
                        foreach( Int32 refNodeTypeId in SelectedNodeTypeIds )
                        {
                            CswNbtMetaDataNodeType LatestVersionNT = _CswNbtResources.MetaData.getNodeType( refNodeTypeId ).LatestVersionNodeType;

                            CswNbtNode NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( LatestVersionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                            NewNode.copyPropertyValues( CswNbtNodeGenerator );

                            ICswNbtPropertySetGeneratorTarget NewNodeAsGeneratorTarget = CswNbtNodeCaster.AsPropertySetGeneratorTarget( NewNode );
                            NewNodeAsGeneratorTarget.GeneratedDate.DateTimeValue = DueDate;
                            NewNodeAsGeneratorTarget.GeneratedDate.ReadOnly = true; //bz # 5349
                            NewNodeAsGeneratorTarget.Generator.RelatedNodeId = CswNbtNodeGenerator.NodeId;
                            NewNodeAsGeneratorTarget.Generator.CachedNodeName = CswNbtNodeGenerator.NodeName;
                            NewNodeAsGeneratorTarget.Parent.RelatedNodeId = NewParentPK;
                            //NewTaskNodeAsTask.Completed.Checked = Tristate.False;

                            if( MarkFuture )
                                NewNodeAsGeneratorTarget.IsFuture.Checked = Tristate.True;
                            else
                                NewNodeAsGeneratorTarget.IsFuture.Checked = Tristate.False;

                            if( null != onBeforeInsertNode )
                            {
                                onBeforeInsertNode( NewNode );
                            }
                            ret = true;
                            NewNode.PendingUpdate = true;
                            NewNode.postChanges( true );
                        }

                    }//if ( null == ExistingNode )

                    else
                    {
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

                    }//if-else ( null == ExistingNode )
                }//foreach ( CswPrimaryKey NewParentPK in Parents )
            }
            return ret;

        }//makeNode()

    }// class CswNbtActGenerateNodes

}//ns ChemSW.Actions
