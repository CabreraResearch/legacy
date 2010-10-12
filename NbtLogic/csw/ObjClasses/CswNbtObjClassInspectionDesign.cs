using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionDesign : CswNbtObjClass, ICswNbtPropertySetGeneratorTarget
    {

        public static string RoutePropertyName { get { return "Route"; } }
        public static string TargetPropertyName { get { return "Target"; } }
        public static string RouteOrderPropertyName { get { return "Route Order"; } }
        public static string NamePropertyName { get { return "Name"; } }
        public static string DatePropertyName { get { return "Due Date"; } }
        public static string IsFuturePropertyName { get { return "IsFuture"; } }
        public static string GeneratorPropertyName { get { return "Generator"; } }
        public static string OwnerPropertyName { get { return "Target"; } }
        public static string StatusPropertyName { get { return "Status"; } }
        public static string ActionRequiredPropertyName { get { return "Action Required"; } }

        /// <summary>
        /// Enum of possible Inspection status values
        /// </summary>
        public enum InspectionStatus { Pending, Completed, Overdue, Missed, Cancelled };

        /// <summary>
        /// This actually comes from OC Prop on Owner (Mount Point or Fire Extinguisher) == Last Inspection Date. 
        /// Eventually, this should be part of a PropertySet.
        /// </summary>
        private static string _LastInspectionDatePropertyName { get { return "Last Inspection Date"; } }

        //ICswNbtPropertySetRuleGeneratorTarget
        public string GeneratorTargetGeneratedDatePropertyName { get { return DatePropertyName; } }
        public string GeneratorTargetIsFuturePropertyName { get { return IsFuturePropertyName; } }
        public string GeneratorTargetGeneratorPropertyName { get { return GeneratorPropertyName; } }
        public string GeneratorTargetParentPropertyName { get { return OwnerPropertyName; } }
       
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInspectionDesign( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

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
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();

            // Make sure the nodetype is locked
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( _CswNbtNode.NodeTypeId );
            NodeType.IsLocked = true;
        } // afterCreateNode()

        /// <summary>
        /// OOC Status before update
        /// </summary>
        private bool _OOC = false;
        private bool _ActionRequired = false;
        private bool _allAnswered = true;
        private bool _allAnsweredinTime = true;
        private DateTime _MissedDate = DateTime.MinValue;

        /// <summary>
        /// Determine before/after OOC states and update Inspection status.
        /// </summary>
        public override void beforeWriteNode()
        {
            CswNbtMetaDataFieldType QuestionFT = _CswNbtResources.MetaData.getFieldType(CswNbtMetaDataFieldType.NbtFieldType.Question);
            CswNbtPropEnmrtrFiltered QuestionsFlt = this.Node.Properties[QuestionFT]; 
            CswNbtNode Schedule = _CswNbtResources.Nodes.GetNode(this.Generator.RelatedNodeId);
            CswNbtNodePropWrapper GraceDaysPW = Schedule.Properties[CswNbtObjClassGenerator.GraceDaysPropertyName];
            CswNbtNodePropNumber GraceDays = GraceDaysPW.AsNumber;
            _MissedDate = this.Date.DateValue.AddDays( CswConvert.ToDouble( GraceDays.Value ) );

            foreach (CswNbtNodePropWrapper Prop in QuestionsFlt)
            {
                CswNbtNodePropQuestion QuestionProp = Prop.AsQuestion;
                _OOC = ( _OOC || !QuestionProp.IsCompliant );
                _allAnswered = ( _allAnswered || QuestionProp.Answer != string.Empty );
                _allAnsweredinTime = ( _allAnsweredinTime || QuestionProp.DateAnswered < _MissedDate );
            }
            //foreach ( CswNbtNodePropWrapper Prop in QuestionsFlt )
            //{
            //    if ( Prop.WasModified )
            //    {
            //        CswNbtNodePropQuestion QuestionProp = Prop.AsQuestion;
            //        _newOOC = ( _newOOC || QuestionProp.InCompliance != true );
            //    }
            //}
            //if ( _oldOOC != _newOOC )
            //{

//                else
                    // Don't know yet.  this.Status.SelectedValue = InspectionStatus.ActionRequired.ToString();
            //}
            if ( _OOC )
                _ActionRequired = true;
            else
            {
                _ActionRequired = false;
                if ( _allAnswered && _allAnsweredinTime )
                {
                    CswNbtNode Parent = _CswNbtResources.Nodes.GetNode( this.Target.RelatedNodeId );
                    if ( null != Parent )
                        Parent.Properties[_LastInspectionDatePropertyName].AsDate.DateValue = DateTime.Now;
                    this.Status.Value = InspectionStatus.Completed.ToString();
                }
                //else if ( _allAnswered && !_allAnsweredinTime )
            }

            this.ActionRequired.Checked = CswConvert.ToTristate( _ActionRequired);
            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        /// <summary>
        /// Update Status on Owner nodes (Mount Point or Fire Extinguisher) if OOC status has changed.
        /// </summary>
        public override void afterWriteNode()
        {
            CswNbtNode ParentMountPoint = _CswNbtResources.Nodes.GetNode( this.Target.RelatedNodeId );
            CswNbtNodePropWrapper ParentStatusPW = ParentMountPoint.Properties[CswNbtObjClassMountPoint.StatusPropertyName];
            CswNbtNodePropList ParentStatus = ParentStatusPW.AsList;
            string newStatus = ParentStatus.Value;
            if ( !_OOC && _allAnswered )
            {
                newStatus = "OK";
            }
            else if ( _OOC )
            {
                newStatus = "OOC";
                ParentMountPoint.PendingUpdate = true;
            }
            //What to do with FireExtinguisher?

            ParentStatus.Value = newStatus;
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

        public CswNbtNodePropLogical ActionRequired
        {
            get
            {
                return ( _CswNbtNode.Properties[ActionRequiredPropertyName].AsLogical );
            }
        }

        public CswNbtNodePropRelationship Route
        {
            get
            {
                return ( _CswNbtNode.Properties[RoutePropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropRelationship Target
        {
            get
            {
                return ( _CswNbtNode.Properties[TargetPropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropNumber RouteOrder
        {
            get
            {
                return ( _CswNbtNode.Properties[RouteOrderPropertyName].AsNumber );
            }
        }

        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName].AsText );
            }
        }

        public CswNbtNodePropDate Date
        {
            get
            {
                return ( _CswNbtNode.Properties[DatePropertyName].AsDate );
            }
        }

        public CswNbtNodePropDate GeneratedDate
        {
            get
            {
                return ( _CswNbtNode.Properties[GeneratorTargetGeneratedDatePropertyName].AsDate );
            }
        }

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

        #endregion



    }//CswNbtObjClassInspectionDesign

}//namespace ChemSW.Nbt.ObjClasses
