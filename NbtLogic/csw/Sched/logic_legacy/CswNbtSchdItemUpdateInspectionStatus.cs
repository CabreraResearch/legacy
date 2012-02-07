using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    /// <summary>
    /// Update Generator Targets according to Vertical business logic
    /// </summary>
    public class CswNbtSchdItemUpdateInspectionStatus : CswNbtSchdItem
    {
        private CswNbtResources _CswNbtResources = null;
        //private CswNbtNode _CswNbtNodeGenerator;
        private CswNbtObjClassInspectionDesign _InspectionNode;
        private string _Pending = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Pending );
        private string _Overdue = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Overdue );

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtResources">CswNbtResources</param>
        /// <param name="CswNbtNodeInspection">An InspectionDesign node</param>
        public CswNbtSchdItemUpdateInspectionStatus( CswNbtResources CswNbtResources, CswNbtNode CswNbtNodeInspection )
         {
            _CswNbtResources = CswNbtResources;
            _InspectionNode = CswNbtNodeCaster.AsInspectionDesign( CswNbtNodeInspection );
            SchedItemName = "UpdateInspectionStatus";
        }//ctor

        /// <summary>
        /// Reset the schedule item
        /// </summary>
        public override void reset()
        {
            _Succeeded = true;
        }//

        /// <summary>
        /// Runs if Inspection status == Pending and is past the due date
        /// </summary>
        override public bool doesItemRunNow()
        {
            bool ReturnVal = false;
            if( null != _InspectionNode )
            {
				DateTime DueDate = _InspectionNode.Date.DateTimeValue;
                CswNbtNode GeneratorNode = _CswNbtResources.Nodes.GetNode( _InspectionNode.Generator.RelatedNodeId );
                if( null != GeneratorNode &&
                    _Pending == _InspectionNode.Status.Value &&
                    DateTime.Today >= DueDate &&    
                    Tristate.True != _InspectionNode.IsFuture.Checked )
                {
                    ReturnVal = true;
                }
            }
            return ( ReturnVal );

        }//doesItemRunNow() 

        /// <summary>
        /// Mark Inspection status overdue
        /// </summary>
        override public void run()
        {
            if( null != _InspectionNode )
            {
                _InspectionNode.Status.Value = _Overdue;
                _InspectionNode.postChanges( true );
            }
        }//run()

        /// <summary>
        /// Name of the schedule
        /// </summary>
        override public string Name
        {
            get
            {
                string ReturnVal = "";
                if( _InspectionNode != null )
                    ReturnVal = "Inspection Status Update (for Node: " + _InspectionNode.Name + ")";
                else
                    ReturnVal = "Inspection Status Update (for Unknown)";

                return ( ReturnVal );
            }//get

        }//Name

        private bool _Succeeded = true;
        /// <summary>
        /// Success status. Currently always true.
        /// </summary>
        override public bool Succeeded
        {
            get
            {
                return ( _Succeeded );
            }//

        }//Succeeded

        /// <summary>
        /// Schedule item status
        /// </summary>
        override public string StatusMessage
        {
            get
            {
                return ( "Inspection Status Update" );
            }//

        }//StatusMessage

    }//CswNbtSchdItemGenerateNode

}//namespace ChemSW.Nbt.Sched
