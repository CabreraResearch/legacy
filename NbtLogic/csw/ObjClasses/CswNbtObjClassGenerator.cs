using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGenerator : CswNbtObjClass, ICswNbtPropertySetScheduler
    {
        #region Properties and ctor

        public const string InspectionGeneratorNodeTypeName = "Inspection Schedule";

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public static string DueDateInterval = "Due Date Interval";
            public static string RunTime = "Run Time";
            public static string FinalDueDate = "Final Due Date";
            public static string NextDueDate = "Next Due Date";
            public static string WarningDays = "Warning Days";
            public static string Enabled = "Enabled";
            public static string RunStatus = "Run Status";
            public static string TargetType = "Target Type";
            public static string Owner = "Owner";
            public static string Description = "Description";
            public static string Summary = "Summary";
            public static string ParentType = "Parent Type";
            public static string ParentView = "Parent View";
        }

        public string SchedulerFinalDueDatePropertyName { get { return PropertyName.FinalDueDate; } }
        public string SchedulerNextDueDatePropertyName { get { return PropertyName.NextDueDate; } }
        public string SchedulerRunStatusPropertyName { get { return PropertyName.RunStatus; } }
        public string SchedulerWarningDaysPropertyName { get { return PropertyName.WarningDays; } }
        public string SchedulerDueDateIntervalPropertyName { get { return PropertyName.DueDateInterval; } }
        public string SchedulerRunTimePropertyName { get { return PropertyName.RunTime; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault;
        private CswNbtPropertySetSchedulerImpl _CswNbtPropertySetSchedulerImpl;

        public CswNbtObjClassGenerator( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GeneratorClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGenerator
        /// </summary>
        public static implicit operator CswNbtObjClassGenerator( CswNbtNode Node )
        {
            CswNbtObjClassGenerator ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.GeneratorClass ) )
            {
                ret = (CswNbtObjClassGenerator) Node.ObjClass;
            }
            return ret;
        }

        private Collection<CswPrimaryKey> _TargetParents;
        public Collection<CswPrimaryKey> TargetParents
        {
            get
            {
                if( null == _TargetParents )
                {
                    _initTargetParents();
                }
                return _TargetParents;
            }
        }

        #endregion Properties and ctor

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
            _setDefaultValues();

            //Case 24572
            updateNextDueDate( ForceUpdate: false, DeleteFutureNodes: ( TargetType.WasModified || ParentType.WasModified ) );

            // case 28352
            Int32 max = DueDateInterval.getMaximumWarningDays();
            if( WarningDays.Value > max )
            {
                WarningDays.Value = max;
            }
        } //beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtPropertySetSchedulerImpl.setLastFutureDate();
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _deleteFutureNodes();
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        } //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            DueDateInterval.SetOnPropChange( OnDueDateIntervalChange );

            if( _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add )  // case 28352
            {
                // case 28146
                WarningDays.MinValue = 0;
                WarningDays.MaxValue = DueDateInterval.getMaximumWarningDays();
            }
            Owner.SetOnPropChange( onOwnerPropChange );
            TargetType.SetOnPropChange( onTargetTypePropChange );
            ParentType.SetOnPropChange( onParentTypePropChange );

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
            }
            return true;
        }

        public override CswNbtNode CopyNode()
        {
            CswNbtObjClassGenerator CopiedIDNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswEnumNbtMakeNodeOperation.DoNothing );
            CopiedIDNode.Node.copyPropertyValues( Node );
            CopiedIDNode.RunStatus.CommentsJson = new Newtonsoft.Json.Linq.JArray();
            CopiedIDNode.postChanges( true );
            return CopiedIDNode.Node;
        }

        #endregion Inherited Events

        #region Public

        public void updateNextDueDate( bool ForceUpdate, bool DeleteFutureNodes )
        {
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate( ForceUpdate, DeleteFutureNodes );
        }

        public Int32 GeneratedNodeCount( DateTime TargetDay )
        {
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "Generated Node Count";

            CswNbtMetaDataNodeType TargetNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( TargetType.SelectedNodeTypeIds[0] ) );
            CswNbtMetaDataNodeTypeProp GeneratorNTP = TargetNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.Generator );
            CswNbtMetaDataNodeTypeProp CreatedDateNTP = TargetNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.CreatedDate );

            CswNbtViewRelationship TargetRel = View.AddViewRelationship( TargetNT, false );
            View.AddViewPropertyAndFilter( TargetRel,
                                           GeneratorNTP,
                                           Conjunction: CswEnumNbtFilterConjunction.And,
                                           ResultMode: CswEnumNbtFilterResultMode.Hide,
                                           Value: this.NodeId.PrimaryKey.ToString(),
                                           SubFieldName: ( (CswNbtFieldTypeRuleRelationship) GeneratorNTP.getFieldTypeRule() ).NodeIDSubField.Name,
                                           FilterMode: CswEnumNbtFilterMode.Equals );

            if( DateTime.MinValue != TargetDay )
            {
                View.AddViewPropertyAndFilter( TargetRel,
                                               CreatedDateNTP,
                                               Conjunction: CswEnumNbtFilterConjunction.And,
                                               ResultMode: CswEnumNbtFilterResultMode.Hide,
                                               Value: TargetDay.Date.ToString(),
                                               FilterMode: CswEnumNbtFilterMode.Equals );
            }

            ICswNbtTree TargetTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, true );
            return TargetTree.getChildNodeCount();
        } // GeneratedNodeCount

        #endregion Public

        #region Private Helper Functions

        private void _setDefaultValues()
        {
            if( TargetType.Empty )
            {
                Enabled.Checked = Tristate.False;
            }
        }

        private void _trySetNodeTypeSelectDefaultValues()
        {
            //has owner and (is inspection schedule or has a view already)
            bool RequiresParentView = CswTools.IsPrimaryKey( Owner.RelatedNodeId ) &&
                            ( Node.getNodeType().getFirstVersionNodeType().NodeTypeName == InspectionGeneratorNodeTypeName ||
                            ( ParentView.ViewId != null &&
                              ParentView.ViewId.isSet() ) );

            if( RequiresParentView )
            {
                CswNbtNode OwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                Collection<CswNbtMetaDataNodeType> MatchingInspectionTargetNts = new Collection<CswNbtMetaDataNodeType>();

                //parent is selectable and is inspection and owner is valid and (parent untouched or empty)
                bool SetDefaultParentType = ( ( false == ParentType.WasModified ||
                                                ParentType.SelectedNodeTypeIds.Count == 0 ) &&
                                                null != OwnerNode &&
                                                OwnerNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.InspectionTargetGroupClass &&
                                                ParentType.SelectMode != PropertySelectMode.Blank );
                if( SetDefaultParentType )
                {
                    ParentType.SelectedNodeTypeIds.Clear();
                    CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetClass );
                    foreach( CswNbtMetaDataNodeType InspectionTargetNt in InspectionTargetOc.getNodeTypes() )
                    {
                        if( InspectionTargetNt.IsLatestVersion() )
                        {
                            CswNbtMetaDataNodeTypeProp TargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
                            if( _fkIsValid( TargetGroupNtp ) )
                            {
                                CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( TargetGroupNtp.FKValue ).getNodeTypeLatestVersion();
                                if( null != InspectionTargetGroupNt &&
                                    false == MatchingInspectionTargetNts.Contains( InspectionTargetNt ) &&
                                    InspectionTargetGroupNt == OwnerNode.getNodeTypeLatestVersion() )
                                {
                                    MatchingInspectionTargetNts.Add( InspectionTargetNt );
                                    ParentType.SelectedNodeTypeIds.Add( InspectionTargetNt.NodeTypeId.ToString(), false, true );
                                    if( ParentType.SelectMode == PropertySelectMode.Single )
                                    {
                                        break;
                                    }
                                }
                            } // is valid FK
                        } // if( InspectionTargetNt.IsLatestVersion )
                    } // foreach( CswNbtMetaDataNodeType InspectionTargetNt in InspectionTargetOc.NodeTypes )
                } // if( SetDefaultTargetType )

                //target is selectable and (parent or target not empty) and (target untouched or empty)
                bool SetDefaultTargetType = ( ( false == TargetType.WasModified ||
                                            TargetType.SelectedNodeTypeIds.Count == 0 ) &&
                                          TargetType.SelectMode != PropertySelectMode.Blank &&
                                          ( MatchingInspectionTargetNts.Count > 0 ||
                                            TargetType.SelectedNodeTypeIds.Count > 0 ) );
                if( SetDefaultTargetType )
                {
                    if( MatchingInspectionTargetNts.Count == 0 )
                    {
                        foreach( Int32 InspectionTargetNodeTypeId in TargetType.SelectedNodeTypeIds.ToIntCollection() )
                        {
                            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetNodeTypeId );
                            if( null != InspectionTargetNt )
                            {
                                CswNbtMetaDataNodeType LatestInspectionTargetNt = InspectionTargetNt.getNodeTypeLatestVersion();
                                if( LatestInspectionTargetNt.getObjectClass().ObjectClass == CswEnumNbtObjectClass.InspectionTargetClass &&
                                    false == MatchingInspectionTargetNts.Contains( LatestInspectionTargetNt ) )
                                {
                                    MatchingInspectionTargetNts.Add( LatestInspectionTargetNt );
                                }
                            }
                        }
                    }
                    if( MatchingInspectionTargetNts.Count > 0 )
                    {
                        TargetType.SelectedNodeTypeIds.Clear();
                        CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
                        foreach( CswNbtMetaDataNodeType InspectionDesignNt in InspectionDesignOc.getNodeTypes() )
                        {
                            if( InspectionDesignNt.IsLatestVersion() )
                            {
                                CswNbtMetaDataNodeTypeProp DesignTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
                                foreach( CswNbtMetaDataNodeType MatchingInspectionTargetNt in MatchingInspectionTargetNts )
                                {
                                    if( _fkIsValid( DesignTargetNtp ) && MatchingInspectionTargetNt.NodeTypeId == DesignTargetNtp.FKValue )
                                    {
                                        TargetType.SelectedNodeTypeIds.Add( InspectionDesignNt.NodeTypeId.ToString(), false, true );
                                        if( TargetType.SelectMode == PropertySelectMode.Single )
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void _deleteFutureNodes()
        {
            // BZ 6754 - Delete all future nodes
            CswNbtMetaDataObjectClass GeneratorObjectClass = ObjectClass;

            if( TargetType.SelectedNodeTypeIds.Count == 1 )
            {
                Int32 SelectedTargetNtId = CswConvert.ToInt32( TargetType.SelectedNodeTypeIds[0] );
                if( Int32.MinValue != SelectedTargetNtId )
                {
                    CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( SelectedTargetNtId );
                    if( null != TargetNodeType )
                    {
                        CswNbtMetaDataObjectClass TargetObjectClass = TargetNodeType.getObjectClass();

                        CswNbtObjClass TargetObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, TargetObjectClass );
                        if( !( TargetObjClass is CswNbtPropertySetGeneratorTarget ) )
                        {
                            throw new CswDniException( "CswNbtObjClassGenerator.beforeDeleteNode() got an invalid object class: " + TargetObjectClass.ObjectClass );
                        }

                        CswNbtMetaDataNodeTypeProp GeneratorProp = TargetNodeType.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.Generator );
                        CswNbtMetaDataNodeTypeProp IsFutureProp = TargetNodeType.getNodeTypePropByObjectClassProp( CswNbtPropertySetGeneratorTarget.PropertyName.IsFuture );

                        CswNbtView View = new CswNbtView( _CswNbtResources );
                        View.ViewName = "CswNbtObjClassSchedule.beforeDeleteNode()";
                        CswNbtViewRelationship GeneratorRelationship = View.AddViewRelationship( GeneratorObjectClass, false );
                        GeneratorRelationship.NodeIdsToFilterIn.Add( _CswNbtNode.NodeId );
                        CswNbtViewRelationship TargetRelationship = View.AddViewRelationship( GeneratorRelationship, CswEnumNbtViewPropOwnerType.Second, GeneratorProp, false );
                        CswNbtViewProperty IsFutureProperty = View.AddViewProperty( TargetRelationship, IsFutureProp );
                        View.AddViewPropertyFilter( IsFutureProperty, CswEnumNbtSubFieldName.Checked, CswEnumNbtFilterMode.Equals, "True" );

                        ICswNbtTree TargetTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );

                        TargetTree.goToRoot();
                        if( TargetTree.getChildNodeCount() > 0 ) // should always be the case
                        {
                            TargetTree.goToNthChild( 0 );
                            if( TargetTree.getChildNodeCount() > 0 ) // might not always be the case
                            {
                                for( int i = 0; i < TargetTree.getChildNodeCount(); i += 1 )
                                {
                                    TargetTree.goToNthChild( i );

                                    CswNbtNode TargetNode = TargetTree.getNodeForCurrentPosition();
                                    TargetNode.delete();

                                    TargetTree.goToParentNode();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void _removeTargetsNotMatchingSelectedParent()
        {
            //If only one Parent NT is allowed (and selected), cycle through selected Target NTs and remove any that aren't related to the selected Parent NT
            if( Node.getNodeType().getFirstVersionNodeType().NodeTypeName == InspectionGeneratorNodeTypeName &&
                ParentType.SelectMode == PropertySelectMode.Single &&
                ParentType.SelectedNodeTypeIds.Count > 0 )
            {
                CswCommaDelimitedString InvalidNodeTypes = new CswCommaDelimitedString();
                foreach( Int32 InspectionDesignNodeTypeId in TargetType.SelectedNodeTypeIds.ToIntCollection() )

                {
                    CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.getNodeType( InspectionDesignNodeTypeId );
                    if( null != InspectionDesignNt )
                    {
                        Int32 InspectionTargetNTId = CswConvert.ToInt32( ParentType.SelectedNodeTypeIds[0] );
                        CswNbtMetaDataNodeTypeProp DesignTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
                        if( _fkIsValid( DesignTargetNtp ) && InspectionTargetNTId != DesignTargetNtp.FKValue )
                        {
                            InvalidNodeTypes.Add( InspectionDesignNt.NodeTypeName );
                            TargetType.SelectedNodeTypeIds.Remove( InspectionDesignNt.NodeTypeId.ToString() );
                        }
                    }
                }
                if( InvalidNodeTypes.Count > 0 && false == Owner.WasModified )
                {
                    throw new CswDniException( CswEnumErrorType.Warning,
                        "Unable to add the following " + TargetType.PropName + " options because they do not belong to " + Owner.CachedNodeName + 
                        ": <br/>" + InvalidNodeTypes.ToString().Replace( ",", "<br/>" ),
                        "Invalid Target Type options selected: " + InvalidNodeTypes );
                }
            }
        }


        private bool _fkIsValid( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            return NodeTypeProp.IsFK &&
                   CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() == NodeTypeProp.FKType &&
                   Int32.MinValue != NodeTypeProp.FKValue;
        }

        private void _initTargetParents()
        {
            //SI will have a ParentView to fetch InspectionTargets which will be used to find existing InsepctionDesign nodes or create new ones
            CswNbtView theParentView = null;
            if( ParentView.ViewId.isSet() )
            {
                theParentView = _CswNbtResources.ViewSelect.restoreView( ParentView.ViewId );
            }

            if( null != theParentView &&
                false == theParentView.IsEmpty() &&
                ParentType.SelectedNodeTypeIds.Count > 0 )
            {
                // Case 20482
                ( theParentView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( NodeId );
                ICswNbtTree ParentsTree = _CswNbtResources.Trees.getTreeFromView( theParentView, false, false, false );
                if( ParentType.SelectMode == PropertySelectMode.Single )
                {
                    Int32 ParentNtId = CswConvert.ToInt32( ParentType.SelectedNodeTypeIds[0] );
                    _TargetParents = ParentsTree.getNodeKeysOfNodeType( ParentNtId );
                }
            }
            else
            {
                _TargetParents = new Collection<CswPrimaryKey> { Owner.RelatedNodeId };
            }
        } // getTargetParents()

        #endregion Private Helper Functions

        #region Object class specific properties

        public CswNbtNodePropDateTime FinalDueDate { get { return ( _CswNbtNode.Properties[PropertyName.FinalDueDate] ); } }
        public CswNbtNodePropDateTime NextDueDate { get { return ( _CswNbtNode.Properties[PropertyName.NextDueDate] ); } }
        /// <summary>
        /// Node type of target, where target is the node generated by schedule
        /// </summary>
        public CswNbtNodePropNodeTypeSelect TargetType { get { return ( _CswNbtNode.Properties[PropertyName.TargetType] ); } }
        private void onTargetTypePropChange( CswNbtNodeProp NodeProp )
        {
            _removeTargetsNotMatchingSelectedParent();
        }
        public CswNbtNodePropMemo Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        /// <summary>
        /// In IMCS, owner == Equipment or Assembly node, in SI owner == Location Group node
        /// </summary>
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }
        private void onOwnerPropChange( CswNbtNodeProp NodeProp )
        {
            if( Owner.WasModified )
            {
                _trySetNodeTypeSelectDefaultValues();
            }
        }
        public CswNbtNodePropComments RunStatus { get { return ( _CswNbtNode.Properties[PropertyName.RunStatus] ); } }
        /// <summary>
        /// Days before due date to anticipate upcoming event
        /// </summary>
        public CswNbtNodePropNumber WarningDays { get { return ( _CswNbtNode.Properties[PropertyName.WarningDays] ); } }
        public CswNbtNodePropText Summary { get { return ( _CswNbtNode.Properties[PropertyName.Summary] ); } }
        public CswNbtNodePropTimeInterval DueDateInterval { get { return ( _CswNbtNode.Properties[PropertyName.DueDateInterval] ); } }
        public void OnDueDateIntervalChange( CswNbtNodeProp Prop )
        {
            if( DueDateInterval.RateInterval.RateType == CswRateInterval.RateIntervalType.Hourly )
            {
                RunTime.setHidden( value: true, SaveToDb: true );
            }
            else
            {
                RunTime.setHidden( value: false, SaveToDb: true );
            }
        }
        public CswNbtNodePropDateTime RunTime { get { return ( _CswNbtNode.Properties[PropertyName.RunTime] ); } }
        public CswNbtNodePropLogical Enabled { get { return ( _CswNbtNode.Properties[PropertyName.Enabled] ); } }
        /// <summary>
        /// Node type of parent. In SI parent is node type of Inspection Target. In IMCS, parent type is not used.
        /// </summary>
        public CswNbtNodePropNodeTypeSelect ParentType { get { return ( _CswNbtNode.Properties[PropertyName.ParentType] ); } }
        private void onParentTypePropChange( CswNbtNodeProp NodeProp )
        {
            if( ParentType.WasModified )
            {
                
            }
        }
        /// <summary>
        /// View from owner to parent. In SI this is Inspection Group > Inspection Target > Inspection. Parent view not utilized elsewhere, yet.
        /// </summary>
        public CswNbtNodePropViewReference ParentView { get { return ( _CswNbtNode.Properties[PropertyName.ParentView] ); } }

        #endregion Object class specific properties

    }//CswNbtObjClassGenerator

}//namespace ChemSW.Nbt.ObjClasses