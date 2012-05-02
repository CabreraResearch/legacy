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
        ///// <summary>
        ///// Inspection Route
        ///// </summary>
        //public static string RoutePropertyName { get { return "Route"; } }
        /// <summary>
        /// Target == Owner == Parent
        /// </summary>
        public static string TargetPropertyName { get { return "Target"; } }
        ///// <summary>
        ///// This Inspection's order within Route
        ///// </summary>
        //public static string RouteOrderPropertyName { get { return "Route Order"; } }
        /// <summary>
        /// Inspection name
        /// </summary>
        public static string NamePropertyName { get { return "Name"; } }
        /// <summary>
        /// Due date
        /// </summary>
        public static string DatePropertyName { get { return "Due Date"; } }
        /// <summary>
        /// Is Future Inspection
        /// </summary>
        public static string IsFuturePropertyName { get { return "IsFuture"; } }
        /// <summary>
        /// Schedule generating this inspection
        /// </summary>
        public static string GeneratorPropertyName { get { return "Generator"; } }
        /// <summary>
        /// Owner == Target == Parent
        /// </summary>
        public static string OwnerPropertyName { get { return "Target"; } }
        /// <summary>
        /// Inspection status as list should match InspectionStatus enum
        /// </summary>
        public static string StatusPropertyName { get { return "Status"; } }
        /// <summary>
        /// Finished or submitted
        /// </summary>
        public static string FinishPropertyName { get { return "Finish"; } }
        /// <summary>
        /// Marked cancelled
        /// </summary>
        public static string CancelPropertyName { get { return "Cancel"; } }
        /// <summary>
        /// Reason for cancel
        /// </summary>
        public static string CancelReasonPropertyName { get { return "Cancel Reason"; } }
        /// <summary>
        /// Location of Inspection's Target
        /// </summary>
        public static string LocationPropertyName { get { return "Location"; } }
        /// <summary>
        /// Nodetype Version
        /// </summary>
        public static string VersionPropertyName { get { return "Version"; } }
        public static string InspectionDatePropertyName { get { return "Inspection Date"; } }
        public static string InspectorPropertyName { get { return "Inspector"; } }

        /// <summary>
        /// Possible status values for Inspection. Should match List values on ID Status attribute.
        /// </summary>
        public enum InspectionStatus
        {
            /// <summary>
            /// No action has been taken, not yet due
            /// </summary>
            Pending,
            /// <summary>
            /// No action has been taken, past due
            /// </summary>
            Overdue,
            /// <summary>
            /// Inspection finished, some answers Deficient
            /// </summary>
            Action_Required,
            /// <summary>
            /// Inspection was never finished, past missed date
            /// </summary>
            Missed,
            /// <summary>
            /// Inspection complete, all answers OK
            /// </summary>
            Completed,
            /// <summary>
            /// Inspection completed late, all answers OK
            /// </summary>
            Completed_Late,
            /// <summary>
            /// Admin has cancelled the Inspection
            /// </summary>
            Cancelled,
            /// <summary>
            /// For unset values
            /// </summary>
            Null
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
        /// Replaces underscore with space in enum
        /// </summary>
        public static string InspectionStatusAsString( InspectionStatus Status )
        {
            string ret = string.Empty;
            if( Status != InspectionStatus.Null )
                ret = Status.ToString().Replace( '_', ' ' );
            return ret;
        }

        /// <summary>
        /// Replaces space with underscore in enum
        /// </summary>
        public static InspectionStatus InspectionStatusFromString( string Status )
        {
            InspectionStatus ret;
            if( !Enum.TryParse<InspectionStatus>( Status.Replace( ' ', '_' ), out ret ) )
                ret = InspectionStatus.Null;
            return ret;
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
        public string GeneratorTargetGeneratedDatePropertyName { get { return DatePropertyName; } }
        /// <summary>
        /// Is Future
        /// </summary>
        public string GeneratorTargetIsFuturePropertyName { get { return IsFuturePropertyName; } }
        /// <summary>
        /// Schedule generating Inspection
        /// </summary>
        public string GeneratorTargetGeneratorPropertyName { get { return GeneratorPropertyName; } }
        /// <summary>
        /// Parent == Owner == Target
        /// </summary>
        public string GeneratorTargetParentPropertyName { get { return OwnerPropertyName; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        /// <summary>
        /// The constructor
        /// </summary>
        public CswNbtObjClassInspectionDesign( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass ); }
        }

        #region Inherited Events
        /// <summary>
        /// Set any existing pending or overdue inspections on the same parent to missed
        /// </summary>
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            if( Tristate.True != this.IsFuture.Checked &&
                null != this.Generator.RelatedNodeId )
            {
                String NodeStatus = String.Empty;
                CswNbtMetaDataNodeType ThisInspectionNT = this.Node.getNodeTypeLatestVersion();
                if( null != ThisInspectionNT )
                {
                    //Limit collection to Inspections on the same Generator
                    IEnumerable<CswNbtNode> AllNodesOfThisNT = ThisInspectionNT.getNodes( true, true )
                        .Where( InspectionNode => this.Generator.RelatedNodeId == InspectionNode.Properties[GeneratorPropertyName].AsRelationship.RelatedNodeId );
                    foreach( CswNbtNode InspectionNode in AllNodesOfThisNT )
                    {
                        CswNbtObjClassInspectionDesign PriorInspection = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );
                        NodeStatus = PriorInspection.Status.Value;

                        if( //Inspection status is Pending, Overdue or not set
                            ( InspectionStatusAsString( InspectionStatus.Overdue ) == NodeStatus ||
                              InspectionStatusAsString( InspectionStatus.Pending ) == NodeStatus ||
                              String.Empty == NodeStatus ) &&
                            //Inspections have the same target, and we're comparing different Inspection nodes
                            ( this.Target.RelatedNodeId == InspectionNode.Properties[TargetPropertyName].AsRelationship.RelatedNodeId &&
                              this.Node != InspectionNode ) )
                        {
                            PriorInspection.Status.Value = InspectionStatus.Missed.ToString();
                            // Case 20755
                            PriorInspection.postChanges( true );
                        }
                    }
                }
            }

            // case 8179 - set value of Version property
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( this.NodeTypeId );
            Version.Text = ThisNodeType.NodeTypeName + " v" + ThisNodeType.VersionNo.ToString();

            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        /// <summary>
        /// Lock Node Type
        /// </summary>
        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()


        /// <summary>
        /// Determine Inspection Status and set read-only
        /// </summary>
        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( this.Status.Value == InspectionStatusAsString( InspectionStatus.Cancelled ) ||
                this.Status.Value == InspectionStatusAsString( InspectionStatus.Completed ) ||
                this.Status.Value == InspectionStatusAsString( InspectionStatus.Completed_Late ) ||
                this.Status.Value == InspectionStatusAsString( InspectionStatus.Missed ) )
            {
                _CswNbtNode.ReadOnly = true;
            }

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        /// <summary>
        /// Update Parent Status (OK,Deficient) if Inspection is submitted
        /// </summary>
        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
            this.Status.ReadOnly = ( true != _CswNbtResources.CurrentNbtUser.IsAdministrator() );
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp )
            {
                CswNbtMetaDataObjectClassProp ButtonOCP = NodeTypeProp.getObjectClassProp();
                if( ButtonOCP.PropName == FinishPropertyName )
                {

                    bool _Deficient = false;
                    bool _allAnswered = true;
                    bool _allAnsweredinTime = true;

                    CswNbtPropEnmrtrFiltered QuestionsFlt = this.Node.Properties[CswNbtMetaDataFieldType.NbtFieldType.Question];
                    QuestionsFlt.Reset();
                    CswCommaDelimitedString UnansweredQuestions = new CswCommaDelimitedString();
                    foreach( CswNbtNodePropWrapper Prop in QuestionsFlt )
                    {
                        CswNbtNodePropQuestion QuestionProp = Prop.AsQuestion;
                        _Deficient = ( _Deficient || !QuestionProp.IsCompliant );
                        if( QuestionProp.Answer.Trim() == string.Empty )
                        {

                            UnansweredQuestions.Add( Prop.NodeTypeProp.FullQuestionNo );
                            _allAnswered = false;
                        }
                        _allAnsweredinTime = ( _allAnsweredinTime && QuestionProp.DateAnswered.Date <= this.Date.DateTimeValue );
                    }

                    if( _allAnswered )
                    {
                        if( _Deficient )
                        {
                            Message = "Inspection is out of compliance and requires further action.";
                            this.Status.Value = InspectionStatusAsString( InspectionStatus.Action_Required );
                        }
                        else
                        {
                            string StatusValue = InspectionStatusAsString( _allAnsweredinTime ? InspectionStatus.Completed : InspectionStatus.Completed_Late );
                            Message = "Inspection marked " + StatusValue + ".";
                            ButtonAction = NbtButtonAction.refresh;
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
                            ICswNbtPropertySetInspectionParent Parent = CswNbtNodeCaster.AsPropertySetInspectionParent( ParentNode );
                            if( false == _Deficient )//case 25041
                            {
                                _Deficient = areMoreActionsRequired();
                            }
                            Parent.Status.Value = _Deficient ? TargetStatusAsString( TargetStatus.Deficient ) : TargetStatusAsString( TargetStatus.OK );
                            //Parent.LastInspectionDate.DateTimeValue = DateTime.Now;
                            ParentNode.postChanges( false );
                        }

                    } // if( _allAnswered )
                    else
                    {
                        Message = "Inspection can not be finished until all questions are answered.  Questions remaining: " + UnansweredQuestions.ToString();
                    }
                } // if( ButtonOCP.PropName == FinishPropertyName )

                else if( ButtonOCP.PropName == CancelPropertyName )
                {
                    Message = "Inspection has been cancelled.";
                    ButtonAction = NbtButtonAction.refresh;
                    this.Status.Value = InspectionStatusAsString( InspectionStatus.Cancelled );
                }

                this.postChanges( false );
            } // if( null != NodeTypeProp )
            return true;
        } // onButtonClick()

        private bool areMoreActionsRequired()//case 25041
        {
            CswNbtView SiblingView = new CswNbtView( _CswNbtResources );
            SiblingView.ViewName = "SiblingView";
            CswNbtViewRelationship ParentRelationship = SiblingView.AddViewRelationship( this.NodeType, false );
            ParentRelationship.NodeIdsToFilterOut.Add( this.NodeId );
            SiblingView.AddViewPropertyAndFilter(
                ParentRelationship,
                this.NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName ),
                InspectionStatusAsString( InspectionStatus.Action_Required ),
                CswNbtSubField.SubFieldName.Value,
                false,
                CswNbtPropFilterSql.PropertyFilterMode.Equals
                );
            SiblingView.AddViewPropertyAndFilter(
                ParentRelationship,
                this.NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName ),
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

        ///// <summary>
        ///// Inspection route
        ///// </summary>
        //public CswNbtNodePropRelationship Route
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[RoutePropertyName].AsRelationship );
        //    }
        //}

        /// <summary>
        /// Inspection target == owner == parent. 
        /// In FE, target == Inspection Target
        /// </summary>
        public CswNbtNodePropRelationship Target
        {
            get
            {
                return ( _CswNbtNode.Properties[TargetPropertyName].AsRelationship );
            }
        }

        ///// <summary>
        ///// Order on route
        ///// </summary>
        //public CswNbtNodePropNumber RouteOrder
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[RouteOrderPropertyName].AsNumber );
        //    }
        //}

        /// <summary>
        /// Inspection name
        /// </summary>
        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName].AsText );
            }
        }

        /// <summary>
        /// Due Date of inspection
        /// </summary>
        public CswNbtNodePropDateTime Date
        {
            get
            {
                return ( _CswNbtNode.Properties[DatePropertyName].AsDateTime );
            }
        }

        /// <summary>
        /// Date the inspection was generated
        /// </summary>
        public CswNbtNodePropDateTime GeneratedDate
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorTargetGeneratedDatePropertyName].AsDateTime );
            }
        }

        /// <summary>
        /// Inspection is preemptively generated for future date
        /// </summary>
        public CswNbtNodePropLogical IsFuture
        {
            get
            {
                return ( _CswNbtNode.Properties[IsFuturePropertyName].AsLogical );
            }
        }

        public CswNbtNodePropRelationship Generator
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorPropertyName].AsRelationship );
            }
        }

        /// <summary>
        /// In this context owner == parent
        /// </summary>
        public CswNbtNodePropRelationship Owner
        {
            get
            {
                return ( _CswNbtNode.Properties[OwnerPropertyName].AsRelationship );
            }
        }

        /// <summary>
        /// In this context parent == owner
        /// </summary>
        public CswNbtNodePropRelationship Parent
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorTargetParentPropertyName].AsRelationship );
            }
        }

        /// <summary>
        /// Actual status of Inspection
        /// </summary>
        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[StatusPropertyName].AsList );
            }
        }
        /// <summary>
        /// Finish button
        /// </summary>
        public CswNbtNodePropButton Finish
        {
            get
            {
                return ( _CswNbtNode.Properties[FinishPropertyName].AsButton );
            }
        }

        /// <summary>
        /// Cancel button
        /// </summary>
        public CswNbtNodePropButton Cancel
        {
            get
            {
                return ( _CswNbtNode.Properties[CancelPropertyName].AsButton );
            }
        }

        /// <summary>
        /// Optional reason for cancelling inspection.
        /// </summary>
        public CswNbtNodePropMemo CancelReason
        {
            get
            {
                return ( _CswNbtNode.Properties[CancelReasonPropertyName].AsMemo );
            }
        }

        /// <summary>
        /// Location of Inspection's Target
        /// </summary>
        public CswNbtNodePropPropertyReference Location
        {
            get
            {
                return ( _CswNbtNode.Properties[LocationPropertyName].AsPropertyReference );
            }
        }

        /// <summary>
        /// Nodetype Version of the Inspection
        /// </summary>
        public CswNbtNodePropText Version
        {
            get
            {
                return ( _CswNbtNode.Properties[VersionPropertyName].AsText );
            }
        }

        /// <summary>
        /// Date the inspection switched to action required or completed...
        /// </summary>
        public CswNbtNodePropDateTime InspectionDate
        {
            get
            {
                return ( _CswNbtNode.Properties[InspectionDatePropertyName].AsDateTime );
            }
        }

        /// <summary>
        /// inspector is a user
        /// </summary>
        public CswNbtNodePropRelationship Inspector
        {
            get
            {
                return ( _CswNbtNode.Properties[InspectorPropertyName].AsRelationship );
            }
        }

        #endregion

    }//CswNbtObjClassInspectionDesign

}//namespace ChemSW.Nbt.ObjClasses
