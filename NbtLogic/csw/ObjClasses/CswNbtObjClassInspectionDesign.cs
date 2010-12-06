using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Inspection Design Object Class
    /// </summary>
    public class CswNbtObjClassInspectionDesign : CswNbtObjClass, ICswNbtPropertySetGeneratorTarget
    {
        /// <summary>
        /// Inspection Route
        /// </summary>
        public static string RoutePropertyName { get { return "Route"; } }
        /// <summary>
        /// Target == Owner == Parent
        /// </summary>
        public static string TargetPropertyName { get { return "Target"; } }
        /// <summary>
        /// This Inspection's order within Route
        /// </summary>
        public static string RouteOrderPropertyName { get { return "Route Order"; } }
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
        public static string FinishedPropertyName { get { return "Finished"; } }
        /// <summary>
        /// Marked cancelled
        /// </summary>
        public static string CancelledPropertyName { get { return "Cancelled"; } }
        /// <summary>
        /// Reason for cancel
        /// </summary>
        public static string CancelReasonPropertyName { get { return "Cancel Reason"; } }

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
            /// Inspection finished, some answers OOC
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
            /// Out of compliance
            /// </summary>
            OOC,
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
            InspectionStatus ret = InspectionStatus.Null;
            if( Status != string.Empty )
                ret = (InspectionStatus) Enum.Parse( typeof( InspectionStatus ), Status.Replace( ' ', '_' ) );
            return ret;
        }

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
            TargetStatus ret = TargetStatus.Null;
            if( Status != string.Empty )
                ret = (TargetStatus) Enum.Parse( typeof( TargetStatus ), Status.Replace( ' ', '_' ) );
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
        /// <param name="CswNbtResources"></param>
        /// <param name="Node"></param>
        public CswNbtObjClassInspectionDesign( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        /// <summary>
        /// Constructor overload
        /// </summary>
        /// <param name="CswNbtResources"></param>
        public CswNbtObjClassInspectionDesign( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass ); }
        }

        #region Inherited Events
        /// <summary>
        /// Set any existing pending or overdue inspections on the same parent to missed
        /// </summary>
        public override void beforeCreateNode()
        {
            CswNbtNodePropList NodeStatus = null;
            CswNbtNodePropRelationship NodeTarget = null;
            CswNbtNodePropRelationship NodeGenerator = null;

            foreach( CswNbtNode InspectionNode in this.Node.NodeType.getNodes( true, true ) )
            {
                NodeStatus = InspectionNode.Properties[StatusPropertyName].AsList;
                //Inspection status is either Pending or Overdue
                if( InspectionStatusAsString( InspectionStatus.Overdue ) == NodeStatus.Value ||
                    InspectionStatusAsString( InspectionStatus.Pending ) == NodeStatus.Value )
                {
                    NodeGenerator = InspectionNode.Properties[GeneratorPropertyName].AsRelationship;
                    //Generator exists
                    if( null != NodeGenerator )
                    {
                        //Inspection is on the same generator
                        if( this.Generator.RelatedNodeId == NodeGenerator.RelatedNodeId )
                        {
                            NodeTarget = InspectionNode.Properties[TargetPropertyName].AsRelationship;
                            //Not this inspection, and Inspection has the same target
                            if( this.Target.NodeId == NodeTarget.RelatedNodeId &&
                                this.Node != InspectionNode )
                            {
                                NodeStatus.Value = InspectionStatus.Missed.ToString();
                            }
                        }
                    }
                }
            }

            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        /// <summary>
        /// Lock Node Type
        /// </summary>
        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();

            // Make sure the nodetype is locked
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( _CswNbtNode.NodeTypeId );
            NodeType.IsLocked = true;
        } // afterCreateNode()

        /// <summary>
        /// True if user checks Finished
        /// </summary>
        private bool _Finished = false;
        /// <summary>
        /// True if user checks Cancelled
        /// </summary>
        private bool _Cancelled = false;
        /// <summary>
        /// True if user checks Finished and any answer is OOC
        /// </summary>
        private bool _OOC = false;
        /// <summary>
        /// True if user checks Finished and all questions have answers
        /// </summary>
        private bool _allAnswered = true;
        /// <summary>
        /// True if user checks Finished, all questions are answered, and all answer dates are before Missed Date
        /// </summary>
        private bool _allAnsweredinTime = true;
        /// <summary>
        /// Due Date + Grace Days
        /// </summary>
        private DateTime _MissedDate = DateTime.MinValue;

        /// <summary>
        /// Determine Inspection Status and set read-only
        /// </summary>
        public override void beforeWriteNode()
        {
            CswNbtMetaDataFieldType QuestionFT = _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Question );
            CswNbtPropEnmrtrFiltered QuestionsFlt = this.Node.Properties[QuestionFT];
            _Finished = ( Tristate.True == this.Finished.Checked );
            _Cancelled = ( Tristate.True == this.Cancelled.Checked );

            if( _Cancelled )
            {
                this.Status.Value = InspectionStatusAsString( InspectionStatus.Cancelled );
            }
            else if( _Finished )
            {
                CswNbtNode ScheduleNode = _CswNbtResources.Nodes.GetNode( this.Generator.RelatedNodeId );
                CswNbtObjClassGenerator Schedule = CswNbtNodeCaster.AsGenerator( ScheduleNode );

                _MissedDate = this.Date.DateValue.AddDays( Schedule.GraceDays.Value );

                foreach( CswNbtNodePropWrapper Prop in QuestionsFlt )
                {
                    CswNbtNodePropQuestion QuestionProp = Prop.AsQuestion;
                    _OOC = ( _OOC || !QuestionProp.IsCompliant );
                    _allAnswered = ( _allAnswered && QuestionProp.Answer != string.Empty );
                    _allAnsweredinTime = ( _allAnsweredinTime && QuestionProp.DateAnswered <= _MissedDate );
                }

                if( _allAnswered )
                {
                    if( _OOC )
                    {
                        this.Status.Value = InspectionStatusAsString( InspectionStatus.Action_Required );
                        _Finished = false;
                    }
                    else
                    {
                        CswNbtNode ParentNode = _CswNbtResources.Nodes.GetNode( this.Target.RelatedNodeId );
                        ICswNbtPropertySetInspectionParent Parent = CswNbtNodeCaster.AsPropertySetInspectionParent( ParentNode );
                        if( null != Parent )
                            Parent.LastInspectionDate.DateValue = DateTime.Now;

                        if( _allAnsweredinTime )
                            this.Status.Value = InspectionStatusAsString( InspectionStatus.Completed );
                        else
                            this.Status.Value = InspectionStatusAsString( InspectionStatus.Completed_Late );
                    }
                }
                else
                {
                    _Finished = false;
                }


                this.Finished.Checked = CswConvert.ToTristate( _Finished );

            }//else if ( _Finished )

            if( this.Status.Value == InspectionStatusAsString( InspectionStatus.Cancelled ) ||
               this.Status.Value == InspectionStatusAsString( InspectionStatus.Completed ) ||
               this.Status.Value == InspectionStatusAsString( InspectionStatus.Completed_Late ) ||
               this.Status.Value == InspectionStatusAsString( InspectionStatus.Missed ) )
            {
                foreach( CswNbtNodePropWrapper Prop in QuestionsFlt )
                {
                    Prop.ReadOnly = true;
                }
            }

            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        /// <summary>
        /// Update Parent Status (OK,OOC) if Inspection is submitted
        /// </summary>
        public override void afterWriteNode()
        {
            CswNbtNode ParentNode = _CswNbtResources.Nodes.GetNode( this.Parent.RelatedNodeId );
            if( ParentNode != null )
            {
                ICswNbtPropertySetInspectionParent Parent = CswNbtNodeCaster.AsPropertySetInspectionParent( ParentNode );
                if( _allAnswered )
                {
                    if( _OOC )
                    {
                        Parent.Status.Value = "OOC";
                        ParentNode.PendingUpdate = true;
                    }
                    else
                    {
                        Parent.Status.Value = "OK";
                    }
                }
            }
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
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        /// <summary>
        /// Inspection route
        /// </summary>
        public CswNbtNodePropRelationship Route
        {
            get
            {
                return ( _CswNbtNode.Properties[RoutePropertyName].AsRelationship );
            }
        }

        /// <summary>
        /// Inspection target == owner == parent. 
        /// In FE, target == Mount Point
        /// </summary>
        public CswNbtNodePropRelationship Target
        {
            get
            {
                return ( _CswNbtNode.Properties[TargetPropertyName].AsRelationship );
            }
        }

        /// <summary>
        /// Order on route
        /// </summary>
        public CswNbtNodePropNumber RouteOrder
        {
            get
            {
                return ( _CswNbtNode.Properties[RouteOrderPropertyName].AsNumber );
            }
        }

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
        public CswNbtNodePropDate Date
        {
            get
            {
                return ( _CswNbtNode.Properties[DatePropertyName].AsDate );
            }
        }

        /// <summary>
        /// Date the inspection was generated
        /// </summary>
        public CswNbtNodePropDate GeneratedDate
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorTargetGeneratedDatePropertyName].AsDate );
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
        /// Temporary bool: if all questions have been answered, Finished == true
        /// If any answer is OOC, Finished = false and Inspection status = Action_Required
        /// </summary>
        public CswNbtNodePropLogical Finished
        {
            get
            {
                return ( _CswNbtNode.Properties[FinishedPropertyName].AsLogical );
            }
        }

        /// <summary>
        /// True if user has cancelled the inspection
        /// </summary>
        public CswNbtNodePropLogical Cancelled
        {
            get
            {
                return ( _CswNbtNode.Properties[CancelledPropertyName].AsLogical );
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

        #endregion



    }//CswNbtObjClassInspectionDesign

}//namespace ChemSW.Nbt.ObjClasses
