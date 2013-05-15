using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Inspection Design Object Class
    /// </summary>
    public class CswNbtObjClassInspectionDesign: CswNbtPropertySetGeneratorTarget
    {
        public new sealed class PropertyName: CswNbtPropertySetGeneratorTarget.PropertyName
        {
            /// <summary>
            /// Target == Parent
            /// </summary>
            public const string Target = "Target";

            /// <summary>
            /// Inspection name
            /// </summary>
            public const string Name = "Name";

            /// <summary>
            /// Inspection status as list should match InspectionStatus enum
            /// </summary>
            public const string Status = "Status";

            /// <summary>
            /// Finished or submitted
            /// </summary>
            public const string Finish = "Finish";

            /// <summary>
            /// Marked cancelled
            /// </summary>
            public const string Cancel = "Cancel";

            /// <summary>
            /// Reason for cancel
            /// </summary>
            public const string CancelReason = "Cancel Reason";

            /// <summary>
            /// Location of Inspection's Target
            /// </summary>
            public const string Location = "Location";

            /// <summary>
            /// Nodetype Version
            /// </summary>
            public const string Version = "Version";

            public const string InspectionDate = "Inspection Date";
            public const string Inspector = "Inspector";
            public const string SetPreferred = "Set Preferred";
            public const string Pictures = "Pictures";
        }

        #region PropertySet

        // for CswNbtPropertySetGeneratorTarget
        public override string ParentPropertyName { get { return PropertyName.Target; } }

        #endregion PropertySet

        private class InspectionState
        {
            private Collection<CswNbtNodePropQuestion> _Questions = new Collection<CswNbtNodePropQuestion>();
            private CswNbtObjClassInspectionDesign _Design;
            public CswCommaDelimitedString UnAnsweredQuestions;

            public InspectionState( CswNbtObjClassInspectionDesign Design )
            {
                _Design = Design;
                UnAnsweredQuestions = new CswCommaDelimitedString();

                if( null != _Design && null != _Design.Node )
                {
                    CswNbtPropEnmrtrFiltered QuestionsFlt = _Design.Node.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.Question];
                    foreach( CswNbtNodePropWrapper PropWrapper in QuestionsFlt )
                    {
                        _Questions.Add( PropWrapper );
                    }
                    foreach( CswNbtNodePropQuestion Question in _Questions )
                    {
                        if( string.IsNullOrEmpty( Question.Answer.Trim() ) )
                        {
                            UnAnsweredQuestions.Add( Question.Question );
                        }
                    }

                }
            }
            public bool Deficient
            {
                get
                {
                    return _Questions.Aggregate( false, ( Ret, QuestionProp ) => ( Ret || false == QuestionProp.IsCompliant ) );
                }
            }
            public bool AllAnswered
            {
                get
                {
                    return _Questions.Aggregate( true, ( Ret, QuestionProp ) => ( Ret && false == string.IsNullOrEmpty( QuestionProp.Answer.Trim() ) ) );
                }
            }
            public bool AllAnsweredInTime
            {
                get
                {
                    bool Ret = AllAnswered;
                    Ret = Ret && _Questions.Aggregate( Ret, ( RetAns, QuestionProp ) => ( RetAns && DateTime.MinValue != QuestionProp.DateAnswered.Date && QuestionProp.DateAnswered.Date <= _Design.DueDate.DateTimeValue ) );
                    return Ret;
                }
            }
        }

        private InspectionState _InspectionState;

        /// <summary>
        /// The constructor
        /// </summary>
        public CswNbtObjClassInspectionDesign( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _InspectionState = new InspectionState( this );
        }

        //ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass ); }
        }

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionDesign
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionDesign( CswNbtNode Node )
        {
            CswNbtObjClassInspectionDesign ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InspectionDesignClass ) )
            {
                ret = (CswNbtObjClassInspectionDesign) Node.ObjClass;
            }
            return ret;
        }

        private void _setDefaultValues()
        {
            if( string.IsNullOrEmpty( Version.Text ) )
            {
                // case 8179 - set value of Version property
                CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( this.NodeTypeId );
                Version.Text = ThisNodeType.NodeTypeName + " v" + ThisNodeType.VersionNo.ToString();
            }
        }

        #region Inherited Events

        /// <summary>
        /// Determine Inspection Status and set read-only
        /// </summary>
        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _setDefaultValues();

            _InspectionState = new InspectionState( this );

            if( false == _genFutureNodesHasRun ) //redundant--for readability
            {
                //this is written in such a way that it should only execute once per instance of this node
                _genFutureNodes();
            }

            foreach( CswNbtNodePropWrapper PropWrapper in Node.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.Question] )
            {
                CswNbtNodePropQuestion QuestionProp = PropWrapper;
                if( QuestionProp.IsAnswerCompliant() )
                {
                    QuestionProp.CorrectiveAction = string.Empty;
                }
            }

            // !!!
            // Don't clear IsFuture here, like we do with Tasks.  See case 28317.
            // !!!

        } // beforeWriteNode()

        /// <summary>
        /// Update Parent Status (OK,Deficient) if Inspection is submitted
        /// </summary>
        public override void afterPropertySetWriteNode()
        {
        } //afterWriteNode()

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes )
        {
            //case 26113: check parent for bad inspections 
            CswNbtNode ParentNode = _CswNbtResources.Nodes.GetNode( this.Parent.RelatedNodeId );
            if( null != ParentNode )
            {
                ICswNbtPropertySetInspectionParent ParentAsParent = CswNbtPropSetCaster.AsPropertySetInspectionParent( ParentNode );
                //CswNbtObjClassInspectionTarget pnodeAsTarget = (CswNbtObjClassInspectionTarget) ParentNode;
                bool _alreadyDeficient = ( ParentAsParent.Status.Value == CswEnumNbtInspectionTargetStatus.TargetStatusAsString( CswEnumNbtInspectionTargetStatus.TargetStatus.Deficient ) );
                bool _Deficient = areMoreActionsRequired();
                if( _Deficient != _alreadyDeficient )
                {
                    ParentAsParent.Status.Value = _Deficient ? CswEnumNbtInspectionTargetStatus.TargetStatusAsString( CswEnumNbtInspectionTargetStatus.TargetStatus.Deficient ) : CswEnumNbtInspectionTargetStatus.TargetStatusAsString( CswEnumNbtInspectionTargetStatus.TargetStatus.OK );
                    ParentNode.postChanges( false );
                }
            }
        } //beforeDeleteNode()

        public override void afterPropertySetPopulateProps()
        {
            foreach( CswNbtNodePropWrapper PropWrapper in Node.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.Question] )
            {
                CswNbtNodePropQuestion QuestionProp = PropWrapper;
                QuestionProp.IsActionRequired = ( Status.Value == CswEnumNbtInspectionStatus.ActionRequired ); // case 25035
            }

            if( false == _CswNbtResources.Permit.canAnyTab( CswEnumNbtNodeTypePermission.Edit, NodeType ) || Node.Locked )
            {
                bool SaveToDb = Node.Locked;
                Finish.setHidden( value : true, SaveToDb : SaveToDb );
                Cancel.setHidden( value : true, SaveToDb : SaveToDb );
                SetPreferred.setHidden( value : true, SaveToDb : SaveToDb );
            }
            else
            {
                SetPreferred.setHidden( value : _InspectionState.AllAnswered, SaveToDb : true );
            }
            //// case 26584, 28155
            // removed by case 29095
            //Status.setReadOnly( value : false == _CswNbtResources.CurrentNbtUser.IsAdministrator(), SaveToDb : false );

            Generator.SetOnPropChange( OnGeneratorChange );
            IsFuture.SetOnPropChange( OnIsFutureChange );
            Status.SetOnPropChange( OnStatusPropChange );
        } //afterPopulateProps()

        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                switch( ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    case PropertyName.Finish:
                        if( false == _CswNbtResources.IsSystemUser )
                        {
                            InspectionDate.DateTimeValue = DateTime.Now;
                            Inspector.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                        }
                        if( _InspectionState.AllAnswered )
                        {
                            if( _InspectionState.Deficient )
                            {
                                ButtonData.Message = "Inspection is deficient and requires further action.";
                                this.Status.Value = CswEnumNbtInspectionStatus.ActionRequired;
                            }
                            else
                            {
                                string StatusValue = _InspectionState.AllAnsweredInTime ? CswEnumNbtInspectionStatus.Completed : CswEnumNbtInspectionStatus.CompletedLate;
                                ButtonData.Message = "Inspection marked " + StatusValue + ".";
                                this.Status.Value = StatusValue;
                                ButtonData.Action = CswEnumNbtButtonAction.refresh;
                            }
                        } // if( _allAnswered )
                        else
                        {
                            ButtonData.Message =
                                "Inspection can not be finished until all questions are answered.  Questions remaining: " +
                                _InspectionState.UnAnsweredQuestions.ToString();
                        }
                        break;

                    case PropertyName.Cancel:
                        ButtonData.Message = "Inspection has been cancelled.";
                        this.Status.Value = CswEnumNbtInspectionStatus.Cancelled;
                        ButtonData.Action = CswEnumNbtButtonAction.refresh;
                        break;

                    case PropertyName.SetPreferred:
                        CswNbtPropEnmrtrFiltered QuestionsFlt = Node.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.Question];
                        foreach( CswNbtNodePropWrapper PropWrapper in QuestionsFlt )
                        {
                            CswNbtNodePropQuestion QuestionProp = PropWrapper;  // don't refactor this into the foreach.  it doesn't work. case 28300.
                            if( string.IsNullOrEmpty( QuestionProp.Answer.Trim() ) )
                            {
                                QuestionProp.Answer = QuestionProp.PreferredAnswer;
                            }
                        }
                        ButtonData.Message = "Unanswered questions have been set to their preferred answer.";
                        SetPreferred.setHidden( value : true, SaveToDb : true );
                        ButtonData.Action = CswEnumNbtButtonAction.refresh;
                        break;
                    case CswNbtObjClass.PropertyName.Save:
                        break;

                }
                this.postChanges( false );
            } // if( null != NodeTypeProp )
            return true;
        }

        // onButtonClick()

        private bool areMoreActionsRequired() //case 25041
        {
            CswNbtView SiblingView = new CswNbtView( _CswNbtResources );
            SiblingView.ViewName = "SiblingView";
            CswNbtViewRelationship ParentRelationship = SiblingView.AddViewRelationship( this.NodeType, false );
            ParentRelationship.NodeIdsToFilterOut.Add( this.NodeId );
            SiblingView.AddViewPropertyAndFilter(
                ParentRelationship,
                this.NodeType.getNodeTypePropByObjectClassProp( PropertyName.Status ),
                CswEnumNbtInspectionStatus.ActionRequired,
                CswEnumNbtSubFieldName.Value,
                false,
                CswEnumNbtFilterMode.Equals
                );
            SiblingView.AddViewPropertyAndFilter(
                ParentRelationship,
                this.NodeType.getNodeTypePropByObjectClassProp( PropertyName.Target ),
                this.Parent.RelatedNodeId.PrimaryKey.ToString(),
                CswEnumNbtSubFieldName.NodeID,
                false,
                CswEnumNbtFilterMode.Equals
                );
            ICswNbtTree SiblingTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, SiblingView, true, false, false );
            int NumOfSiblings = SiblingTree.getChildNodeCount();

            return 0 < NumOfSiblings || Status.Value.Equals( CswEnumNbtInspectionStatus.ActionRequired );
        }

        public override CswNbtNode CopyNode()
        {
            CswNbtObjClassInspectionDesign CopiedIDNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    NewNode.copyPropertyValues( Node );
                    ( (CswNbtObjClassInspectionDesign) NewNode ).Generator.RelatedNodeId = null;
                    ( (CswNbtObjClassInspectionDesign) NewNode ).Generator.RefreshNodeName();
                    //CopiedIDNode.postChanges( true );
                } );
            return CopiedIDNode.Node;
        }

        #endregion

        #region Object class specific properties

        private bool _genFutureNodesHasRun = false;

        private void _genFutureNodes()
        {
            if( CswEnumTristate.True != this.IsFuture.Checked &&
                CswTools.IsPrimaryKey( this.Generator.RelatedNodeId ) &&
                false == _genFutureNodesHasRun )
            {
                String NodeStatus = String.Empty;
                CswNbtMetaDataNodeType ThisInspectionNT = this.Node.getNodeTypeLatestVersion();
                if( null != ThisInspectionNT )
                {
                    _genFutureNodesHasRun = true;
                    //Limit collection to Inspections on the same Generator
                    IEnumerable<CswNbtNode> AllNodesOfThisNT = ThisInspectionNT.getNodes( true, true )
                        .Where( InspectionNode => this.Generator.RelatedNodeId == InspectionNode.Properties[PropertyName.Generator].AsRelationship.RelatedNodeId );
                    foreach( CswNbtNode InspectionNode in AllNodesOfThisNT )
                    {
                        CswNbtObjClassInspectionDesign PriorInspection = (CswNbtObjClassInspectionDesign) InspectionNode;
                        NodeStatus = PriorInspection.Status.Value;

                        if( //Inspection status is Pending, Overdue or not set
                            ( CswEnumNbtInspectionStatus.Overdue == NodeStatus ||
                              CswEnumNbtInspectionStatus.Pending == NodeStatus ||
                              String.Empty == NodeStatus ) &&
                            //Inspections have the same target, and we're comparing different Inspection nodes
                            ( this.Target.RelatedNodeId == InspectionNode.Properties[PropertyName.Target].AsRelationship.RelatedNodeId &&
                              this.Node != InspectionNode ) &&
                            // Other inspection isn't future (case 28317)
                            CswEnumTristate.True != PriorInspection.IsFuture.Checked )
                        {
                            PriorInspection.Status.Value = CswEnumNbtInspectionStatus.Missed.ToString();
                            // Case 20755
                            PriorInspection.postChanges( true );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inspection target == parent. 
        /// In FE, target == Inspection Target
        /// </summary>
        public CswNbtNodePropRelationship Target { get { return ( _CswNbtNode.Properties[PropertyName.Target] ); } }

        /// <summary>
        /// Inspection name
        /// </summary>
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }

        private void OnIsFutureChange( CswNbtNodeProp NodeProp )
        {
            if( false == _genFutureNodesHasRun ) //redundant--for readability
            {
                //this is written in such a way that it should only execute once per instance of this node
                _genFutureNodes();
            }
        }

        private void OnGeneratorChange( CswNbtNodeProp NodeProp )
        {
            if( false == _genFutureNodesHasRun ) //redundant--for readability
            {
                //this is written in such a way that it should only execute once per instance of this node
                _genFutureNodes();
            }
        }

        /// <summary>
        /// In this context parent == target
        /// </summary>
        public override CswNbtNodePropRelationship Parent { get { return ( _CswNbtNode.Properties[ParentPropertyName] ); } }

        /// <summary>
        /// Actual status of Inspection
        /// </summary>
        public CswNbtNodePropList Status
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Status] ); }
        }

        private void OnStatusPropChange( CswNbtNodeProp NodeProp )
        {
            switch( Status.Value )
            {
                case CswEnumNbtInspectionStatus.Completed:
                case CswEnumNbtInspectionStatus.CompletedLate:
                    if( _InspectionState.Deficient )
                    {
                        Status.Value = CswEnumNbtInspectionStatus.ActionRequired;
                    }
                    else
                    {
                        Finish.setHidden( true, true );
                        SetPreferred.setHidden( true, true );
                        Cancel.setHidden( true, true );
                        Node.setReadOnly( value : true, SaveToDb : true );
                    }
                    break;

                case CswEnumNbtInspectionStatus.Cancelled:
                case CswEnumNbtInspectionStatus.Missed:
                    //InspectionDate.DateTimeValue = DateTime.Now;
                    Finish.setHidden( true, true );
                    SetPreferred.setHidden( true, true );
                    Cancel.setHidden( true, true );
                    Node.setReadOnly( value : true, SaveToDb : true );
                    break;

                case CswEnumNbtInspectionStatus.Overdue:
                case CswEnumNbtInspectionStatus.ActionRequired:
                case CswEnumNbtInspectionStatus.Pending:
                    Finish.setHidden( false, true );
                    SetPreferred.setHidden( false, true );
                    Cancel.setHidden( false, true );
                    Node.setReadOnly( value : false, SaveToDb : true );
                    break;

            } // switch( Status.Value )

            CswNbtNode ParentNode = _CswNbtResources.Nodes.GetNode( this.Parent.RelatedNodeId );
            if( ParentNode != null && false == IsTemp )
            {
                ICswNbtPropertySetInspectionParent ParentAsParent = CswNbtPropSetCaster.AsPropertySetInspectionParent( ParentNode );
                bool IsDeficient = areMoreActionsRequired();  //case 25041

                String OKStatus = ( ParentAsParent.Status.Value == CswEnumNbtInspectionTargetStatus.TargetStatusAsString( CswEnumNbtInspectionTargetStatus.TargetStatus.Not_Inspected ) &&
                    Status.Value == CswEnumNbtInspectionStatus.Pending || Status.Value == CswEnumNbtInspectionStatus.Overdue ) ?
                    CswEnumNbtInspectionTargetStatus.TargetStatusAsString( CswEnumNbtInspectionTargetStatus.TargetStatus.Not_Inspected ) : CswEnumNbtInspectionTargetStatus.TargetStatusAsString( CswEnumNbtInspectionTargetStatus.TargetStatus.OK );
                ParentAsParent.Status.Value = IsDeficient ? CswEnumNbtInspectionTargetStatus.TargetStatusAsString( CswEnumNbtInspectionTargetStatus.TargetStatus.Deficient ) : OKStatus;
                //Parent.LastInspectionDate.DateTimeValue = DateTime.Now;
                ParentNode.postChanges( false );
            } // if( ParentNode != null )

        } // OnStatusPropChange()


        /// <summary>
        /// Finish button
        /// </summary>
        public CswNbtNodePropButton Finish { get { return ( _CswNbtNode.Properties[PropertyName.Finish] ); } }

        /// <summary>
        /// Cancel button
        /// </summary>
        public CswNbtNodePropButton Cancel { get { return ( _CswNbtNode.Properties[PropertyName.Cancel] ); } }

        /// <summary>
        /// Optional reason for cancelling inspection.
        /// </summary>
        public CswNbtNodePropMemo CancelReason { get { return ( _CswNbtNode.Properties[PropertyName.CancelReason] ); } }

        /// <summary>
        /// Location of Inspection's Target
        /// </summary>
        public CswNbtNodePropPropertyReference Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }

        /// <summary>
        /// Nodetype Version of the Inspection
        /// </summary>
        public CswNbtNodePropText Version { get { return ( _CswNbtNode.Properties[PropertyName.Version] ); } }

        /// <summary>
        /// Date the inspection switched to action required or completed...
        /// </summary>
        public CswNbtNodePropDateTime InspectionDate { get { return ( _CswNbtNode.Properties[PropertyName.InspectionDate] ); } }

        /// <summary>
        /// inspector is a user
        /// </summary>
        public CswNbtNodePropRelationship Inspector { get { return ( _CswNbtNode.Properties[PropertyName.Inspector] ); } }

        public CswNbtNodePropButton SetPreferred { get { return _CswNbtNode.Properties[PropertyName.SetPreferred]; } }

        public CswNbtNodePropImage Pictures { get { return _CswNbtNode.Properties[PropertyName.Pictures]; } }

        #endregion

    }//CswNbtObjClassInspectionDesign

}//namespace ChemSW.Nbt.ObjClasses
