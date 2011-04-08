using System;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.MtSched.Core;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtUpdtInspection : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( "NbtUpdtInspection" ); }
        }

        public bool doesItemRunNow()
        {
            bool ReturnVal = false;

            _InspectionNode = null; //WE NEED TO QUERY FOR NODE HERE; IT USED TO BE PASE

            if( null != _InspectionNode )
            {
                DateTime DueDate = _InspectionNode.Date.DateValue;
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

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private string _CompletionMessage = string.Empty;
        public string CompletionMessage
        {
            get { return ( _CompletionMessage ); }
        }


        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }


        private CswNbtResources _CswNbtResources = null;
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
        }

        private CswNbtNode _CswNbtNodeGenerator;
        private CswNbtObjClassInspectionDesign _InspectionNode;
        private string _Pending = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Pending );
        private string _Overdue = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Overdue );
        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    if( null != _InspectionNode )
                    {
                        _InspectionNode.Status.Value = _Overdue;
                        _InspectionNode.postChanges( true );
                    }
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CompletionMessage = "Csw3ETasks::GetUpdatedItems() exception: " + Exception.Message;
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;
                    _CswNbtResources.logError( new CswDniException( _CompletionMessage ) );

                }//catch

            }//if we're not shutting down

        }//threadCallBack()


        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = MtSched.Core.LogicRunStatus.Idle;
        }

        public void releaseResources()
        {
            _CswNbtResources.release();
        }


    }//CswScheduleLogicNbtUpdtInspection


}//namespace ChemSW.Nbt.Sched
