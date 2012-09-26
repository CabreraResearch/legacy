using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Inspection Design Object Class
    /// </summary>
    public class CswNbtObjClassInspectionDesign : CswNbtObjClass, ICswNbtPropertySetGeneratorTarget
    {
        public sealed class PropertyName
        {
            /// <summary>
            /// Target == Owner == Parent
            /// </summary>
            public const string Target = "Target";

            /// <summary>
            /// Inspection name
            /// </summary>
            public const string Name = "Name";

            /// <summary>
            /// Due date
            /// </summary>
            public const string Date = "Due Date";

            /// <summary>
            /// Is Future Inspection
            /// </summary>
            public const string IsFuture = "IsFuture";

            /// <summary>
            /// Schedule generating this inspection
            /// </summary>
            public const string Generator = "Generator";

            /// <summary>
            /// Owner == Target == Parent
            /// </summary>
            public const string Owner = "Target";

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
        }


        public sealed class InspectionStatus : IEquatable<InspectionStatus>
        {
            #region Internals
            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Pending       , Pending },
                                                                       { Overdue       , Overdue },
                                                                       { ActionRequired, ActionRequired },
                                                                       { Missed        , Missed },
                                                                       { Completed     , Completed },
                                                                       { CompletedLate , CompletedLate },
                                                                       { Cancelled     , Cancelled }
                                                                   };
            /// <summary>
            /// The string value of the current instance
            /// </summary>
            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }

            /// <summary>
            /// The enum constructor
            /// </summary>
            public InspectionStatus( string ItemName = CswResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            /// <summary>
            /// Implicit cast to Enum
            /// </summary>
            public static implicit operator InspectionStatus( string Val )
            {
                return new InspectionStatus( Val );
            }

            /// <summary>
            /// Implicit cast to string
            /// </summary>
            public static implicit operator string( InspectionStatus item )
            {
                return item.Value;
            }

            /// <summary>
            /// Override of ToString
            /// </summary>
            public override string ToString()
            {
                return Value;
            }

            #endregion Internals

            #region Enum members

            /// <summary>
            /// No action has been taken, not yet due
            /// </summary>
            public const string Pending = "Pending";
            /// <summary>
            /// No action has been taken, past due
            /// </summary>
            public const string Overdue = "Overdue";
            /// <summary>
            /// Inspection finished, some answers Deficient
            /// </summary>
            public const string ActionRequired = "Action Required";
            /// <summary>
            /// Inspection was never finished, past missed date
            /// </summary>
            public const string Missed = "Missed";
            /// <summary>
            /// Inspection complete, all answers OK
            /// </summary>
            public const string Completed = "Completed";

            /// <summary>
            /// Inspection completed late, all answers OK
            /// </summary>
            public const string CompletedLate = "Completed Late";

            /// <summary>
            /// Admin has cancelled the Inspection
            /// </summary>
            public const string Cancelled = "Cancelled";

            #endregion Enum members

            #region IEquatable (InspectionStatus)

            /// <summary>
            /// == Equality operator guarantees we're evaluating instance values
            /// </summary>
            public static bool operator ==( InspectionStatus ft1, InspectionStatus ft2 )
            {
                //do a string comparison on the fieldtypes
                return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
            }

            /// <summary>
            ///  != Inequality operator guarantees we're evaluating instance values
            /// </summary>
            public static bool operator !=( InspectionStatus ft1, InspectionStatus ft2 )
            {
                return !( ft1 == ft2 );
            }

            /// <summary>
            /// Equals
            /// </summary>
            public override bool Equals( object obj )
            {
                if( !( obj is InspectionStatus ) )
                {
                    return false;
                }
                return this == (InspectionStatus) obj;
            }

            /// <summary>
            /// Equals
            /// </summary>
            public bool Equals( InspectionStatus obj )
            {
                return this == obj;
            }

            /// <summary>
            /// Get Hash Code
            /// </summary>
            public override int GetHashCode()
            {
                int ret = 23, prime = 37;
                ret = ( ret * prime ) + Value.GetHashCode();
                ret = ( ret * prime ) + _Enums.GetHashCode();
                return ret;
            }

            #endregion IEquatable (InspectionStatus)

        };

        public enum TargetStatus
        {
            /// <summary>
            /// Not yet inspected
            /// </summary>
            Not_Inspected,

            /// <summary>
            /// Last inspection complete and in compliance
            /// </summary>
            OK,

            /// <summary>
            /// Deficient, Out of compliance
            /// </summary>
            Deficient,

            /// <summary>
            /// For unset values
            /// </summary>
            Null
        }

        /// <summary>
        /// Returns Target status as string from TargetStatus Enum
        /// </summary>
        public static string TargetStatusAsString( TargetStatus Status )
        {
            string ret = string.Empty;
            if( Status != TargetStatus.Null )
                ret = Status.ToString().Replace( '_', ' ' );
            return ret;
        }

        /// <summary>
        /// Replaces space with underscore in enum
        /// </summary>
        public static TargetStatus TargetStatusFromString( string Status )
        {
            TargetStatus ret;
            if( !Enum.TryParse<TargetStatus>( Status.Replace( ' ', '_' ), out ret ) )
                ret = TargetStatus.Null;
            return ret;
        }

        //ICswNbtPropertySetRuleGeneratorTarget
        /// <summary>
        /// Due Date
        /// </summary>
        public string GeneratorTargetGeneratedDatePropertyName
        {
            get { return PropertyName.Date; }
        }

        /// <summary>
        /// Is Future
        /// </summary>
        public string GeneratorTargetIsFuturePropertyName
        {
            get { return PropertyName.IsFuture; }
        }

        /// <summary>
        /// Schedule generating Inspection
        /// </summary>
        public string GeneratorTargetGeneratorPropertyName
        {
            get { return PropertyName.Generator; }
        }

        /// <summary>
        /// Parent == Owner == Target
        /// </summary>
        public string GeneratorTargetParentPropertyName
        {
            get { return PropertyName.Owner; }
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        /// <summary>
        /// The constructor
        /// </summary>
        public CswNbtObjClassInspectionDesign( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        //ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionDesign
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionDesign( CswNbtNode Node )
        {
            CswNbtObjClassInspectionDesign ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass ) )
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
        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _setDefaultValues();
            if( false == _genFutureNodesHasRun ) //redundant--for readability
            {
                //this is written in such a way that it should only execute once per instance of this node
                _genFutureNodes();
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }

        //beforeWriteNode()

        /// <summary>
        /// Update Parent Status (OK,Deficient) if Inspection is submitted
        /// </summary>
        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }

        //afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            //case 26113: check parent for bad inspections 
            CswNbtNode ParentNode = _CswNbtResources.Nodes.GetNode( this.Parent.RelatedNodeId );
            ICswNbtPropertySetInspectionParent Parent = CswNbtPropSetCaster.AsPropertySetInspectionParent( ParentNode );
            //CswNbtObjClassInspectionTarget pnodeAsTarget = (CswNbtObjClassInspectionTarget) ParentNode;
            bool _alreadyDeficient = ( Parent.Status.Value == TargetStatusAsString( TargetStatus.Deficient ) );
            bool _Deficient = areMoreActionsRequired();
            if( _Deficient != _alreadyDeficient )
            {
                Parent.Status.Value = _Deficient ? TargetStatusAsString( TargetStatus.Deficient ) : TargetStatusAsString( TargetStatus.OK );
                ParentNode.postChanges( false );
            }

            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }

        //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();

        }

        //afterDeleteNode()        

        public override void afterPopulateProps()
        {
            Generator.SetOnPropChange( OnGeneratorChange );
            IsFuture.SetOnPropChange( OnIsFutureChange );
            CswNbtPropEnmrtrFiltered QuestionsFlt = this.Node.Properties[(CswNbtMetaDataFieldType.NbtFieldType) CswNbtMetaDataFieldType.NbtFieldType.Question];
            QuestionsFlt.Reset();
            bool AllAnswered = true;
            foreach( CswNbtNodePropWrapper Prop in QuestionsFlt )
            {
                CswNbtNodePropQuestion QuestionProp = Prop.AsQuestion;

                // case 25035
                if( this.Status.Value == InspectionStatus.ActionRequired )
                {
                    QuestionProp.IsActionRequired = true;
                }

                AllAnswered = ( false == string.IsNullOrEmpty( QuestionProp.Answer ) ) && AllAnswered;

                // case 26705
                QuestionProp.SetOnPropChange( onQuestionChange );
            }

            SetPreferred.setReadOnly( value: AllAnswered, SaveToDb: true );

            _CswNbtObjClassDefault.afterPopulateProps();

            // case 26584
            if( false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                if( this.Status.Value == InspectionStatus.Cancelled ||
                    this.Status.Value == InspectionStatus.Completed ||
                    this.Status.Value == InspectionStatus.CompletedLate ||
                    this.Status.Value == InspectionStatus.Missed )
                {
                    _CswNbtNode.setReadOnly( value: true, SaveToDb: false );
                }
                this.Status.setReadOnly( value: true, SaveToDb: false );
            }
        }

        //afterPopulateProps()

        public void onQuestionChange( CswNbtNodeProp Prop )
        {
            CswNbtNodePropWrapper PropWrapper = _CswNbtNode.Properties[Prop.NodeTypeProp];
            CswNbtNodePropQuestion QuestionProp = PropWrapper.AsQuestion;
            // case 26705
            if( string.IsNullOrEmpty( QuestionProp.Answer ) )
            {
                SetPreferred.setReadOnly( value: false, SaveToDb: true );
            }
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                CswNbtMetaDataObjectClassProp ButtonOCP = ButtonData.NodeTypeProp.getObjectClassProp();
                CswNbtPropEnmrtrFiltered QuestionsFlt;
                switch( ButtonOCP.PropName )
                {
                    case PropertyName.Finish:
                        bool _Deficient = false;
                        bool _allAnswered = true;
                        bool _allAnsweredinTime = true;

                        QuestionsFlt = Node.Properties[(CswNbtMetaDataFieldType.NbtFieldType) CswNbtMetaDataFieldType.NbtFieldType.Question];
                        QuestionsFlt.Reset();
                        CswCommaDelimitedString UnansweredQuestions = new CswCommaDelimitedString();
                        foreach( CswNbtNodePropWrapper Prop in QuestionsFlt )
                        {
                            CswNbtNodePropQuestion QuestionProp = Prop;
                            _Deficient = ( _Deficient || !QuestionProp.IsCompliant );
                            if( QuestionProp.Answer.Trim() == string.Empty )
                            {
                                UnansweredQuestions.Add( Prop.NodeTypeProp.FullQuestionNo );
                                _allAnswered = false;
                            }
                            _allAnsweredinTime = ( _allAnsweredinTime &&
                                                   DateTime.MinValue != QuestionProp.DateAnswered.Date &&
                                                   QuestionProp.DateAnswered.Date <= this.Date.DateTimeValue );
                        }

                        if( _allAnswered )
                        {
                            if( _Deficient )
                            {
                                ButtonData.Message = "Inspection is deficient and requires further action.";
                                this.Status.Value = InspectionStatus.ActionRequired;
                            }
                            else
                            {
                                string StatusValue = _allAnsweredinTime ? InspectionStatus.Completed : InspectionStatus.CompletedLate;
                                ButtonData.Message = "Inspection marked " + StatusValue + ".";
                                this.Status.Value = StatusValue;
                            }
                            if( true == this.InspectionDate.Empty )
                            {
                                this.InspectionDate.DateTimeValue = DateTime.Now;
                                this.Inspector.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                            }
                            CswNbtNode ParentNode = _CswNbtResources.Nodes.GetNode( this.Parent.RelatedNodeId );
                            if( ParentNode != null )
                            {
                                ICswNbtPropertySetInspectionParent Parent =
                                    CswNbtPropSetCaster.AsPropertySetInspectionParent( ParentNode );
                                if( false == _Deficient ) //case 25041
                                {
                                    _Deficient = areMoreActionsRequired();
                                }
                                Finish.setReadOnly( value: true, SaveToDb: true );
                                Finish.setHidden( value: true, SaveToDb: true );
                                Cancel.setReadOnly( value: true, SaveToDb: true );
                                Cancel.setHidden( value: true, SaveToDb: true );
                                SetPreferred.setReadOnly( value: true, SaveToDb: true );
                                SetPreferred.setHidden( value: true, SaveToDb: true );

                                Parent.Status.Value = _Deficient
                                                          ? TargetStatusAsString( TargetStatus.Deficient )
                                                          : TargetStatusAsString( TargetStatus.OK );
                                //Parent.LastInspectionDate.DateTimeValue = DateTime.Now;
                                ParentNode.postChanges( false );
                            }

                        } // if( _allAnswered )
                        else
                        {
                            ButtonData.Message =
                                "Inspection can not be finished until all questions are answered.  Questions remaining: " +
                                UnansweredQuestions.ToString();
                        }
                        break;

                    case PropertyName.Cancel:
                        ButtonData.Message = "Inspection has been cancelled.";

                        this.Status.Value = InspectionStatus.Cancelled;
                        break;

                    case PropertyName.SetPreferred:
                        QuestionsFlt = Node.Properties[(CswNbtMetaDataFieldType.NbtFieldType) CswNbtMetaDataFieldType.NbtFieldType.Question];
                        QuestionsFlt.Reset();
                        foreach( CswNbtNodePropWrapper Prop in QuestionsFlt )
                        {
                            CswNbtNodePropQuestion QuestionProp = Prop;
                            if( string.IsNullOrEmpty( QuestionProp.Answer.Trim() ) )
                            {
                                QuestionProp.Answer = QuestionProp.PreferredAnswer;
                            }
                        }
                        ButtonData.Message = "Unanswered questions have been set to their preferred answer.";
                        SetPreferred.setReadOnly( value: true, SaveToDb: true );
                        SetPreferred.setHidden( value: true, SaveToDb: true );
                        break;
                }
                ButtonData.Action = NbtButtonAction.refresh;
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
                InspectionStatus.ActionRequired,
                CswNbtSubField.SubFieldName.Value,
                false,
                CswNbtPropFilterSql.PropertyFilterMode.Equals
                );
            SiblingView.AddViewPropertyAndFilter(
                ParentRelationship,
                this.NodeType.getNodeTypePropByObjectClassProp( PropertyName.Target ),
                this.Parent.RelatedNodeId.PrimaryKey.ToString(),
                CswNbtSubField.SubFieldName.NodeID,
                false,
                CswNbtPropFilterSql.PropertyFilterMode.Equals
                );
            ICswNbtTree SiblingTree = _CswNbtResources.Trees.getTreeFromView( SiblingView, true, true, false, false );
            int NumOfSiblings = SiblingTree.getChildNodeCount();

            return 0 < NumOfSiblings;
        }

        #endregion

        #region Object class specific properties

        private bool _genFutureNodesHasRun = false;

        private void _genFutureNodes()
        {
            if( Tristate.True != this.IsFuture.Checked &&
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
                            ( InspectionStatus.Overdue == NodeStatus ||
                              InspectionStatus.Pending == NodeStatus ||
                              String.Empty == NodeStatus ) &&
                            //Inspections have the same target, and we're comparing different Inspection nodes
                            ( this.Target.RelatedNodeId == InspectionNode.Properties[PropertyName.Target].AsRelationship.RelatedNodeId &&
                              this.Node != InspectionNode ) )
                        {
                            PriorInspection.Status.Value = InspectionStatus.Missed.ToString();
                            // Case 20755
                            PriorInspection.postChanges( true );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inspection target == owner == parent. 
        /// In FE, target == Inspection Target
        /// </summary>
        public CswNbtNodePropRelationship Target
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Target] ); }
        }

        /// <summary>
        /// Inspection name
        /// </summary>
        public CswNbtNodePropText Name
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Name] ); }
        }

        /// <summary>
        /// Due Date of inspection
        /// </summary>
        public CswNbtNodePropDateTime Date
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Date] ); }
        }

        /// <summary>
        /// Date the inspection was generated
        /// </summary>
        public CswNbtNodePropDateTime GeneratedDate
        {
            get { return ( _CswNbtNode.Properties[GeneratorTargetGeneratedDatePropertyName] ); }
        }

        /// <summary>
        /// Inspection is preemptively generated for future date
        /// </summary>
        public CswNbtNodePropLogical IsFuture
        {
            get { return ( _CswNbtNode.Properties[PropertyName.IsFuture] ); }
        }

        private void OnIsFutureChange( CswNbtNodeProp NodeProp )
        {
            if( false == _genFutureNodesHasRun ) //redundant--for readability
            {
                //this is written in such a way that it should only execute once per instance of this node
                _genFutureNodes();
            }
        }

        public CswNbtNodePropRelationship Generator
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Generator] ); }
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
        /// In this context owner == parent
        /// </summary>
        public CswNbtNodePropRelationship Owner
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); }
        }

        /// <summary>
        /// In this context parent == owner
        /// </summary>
        public CswNbtNodePropRelationship Parent
        {
            get { return ( _CswNbtNode.Properties[GeneratorTargetParentPropertyName] ); }
        }

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
                case "Cancelled":
                    Finish.setReadOnly( value: true, SaveToDb: true );
                    Finish.setHidden( value: true, SaveToDb: true );
                    Cancel.setReadOnly( value: true, SaveToDb: true );
                    Cancel.setHidden( value: true, SaveToDb: true );
                    SetPreferred.setReadOnly( value: true, SaveToDb: true );
                    SetPreferred.setHidden( value: true, SaveToDb: true );
                    break;

            }
        }




        /// <summary>
        /// Finish button
        /// </summary>
        public CswNbtNodePropButton Finish
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Finish] );
            }
        }

        /// <summary>
        /// Cancel button
        /// </summary>
        public CswNbtNodePropButton Cancel
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Cancel] );
            }
        }

        /// <summary>
        /// Optional reason for cancelling inspection.
        /// </summary>
        public CswNbtNodePropMemo CancelReason
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.CancelReason] );
            }
        }

        /// <summary>
        /// Location of Inspection's Target
        /// </summary>
        public CswNbtNodePropPropertyReference Location
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Location] );
            }
        }

        /// <summary>
        /// Nodetype Version of the Inspection
        /// </summary>
        public CswNbtNodePropText Version
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Version] );
            }
        }

        /// <summary>
        /// Date the inspection switched to action required or completed...
        /// </summary>
        public CswNbtNodePropDateTime InspectionDate
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.InspectionDate] );
            }
        }

        /// <summary>
        /// inspector is a user
        /// </summary>
        public CswNbtNodePropRelationship Inspector
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Inspector] );
            }
        }

        public CswNbtNodePropButton SetPreferred { get { return _CswNbtNode.Properties[PropertyName.SetPreferred]; } }

        #endregion

    }//CswNbtObjClassInspectionDesign

}//namespace ChemSW.Nbt.ObjClasses
