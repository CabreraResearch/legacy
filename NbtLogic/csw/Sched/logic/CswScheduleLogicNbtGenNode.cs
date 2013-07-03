using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtGenNode : ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.GenNode ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        private CswScheduleLogicNodes _CswScheduleLogicNodes;       

        #endregion Properties

        #region State

        private string _StatusMessage;
        private int _TotalGeneratorsProcessed = 0;
        private Collection<CswPrimaryKey> _GeneratorPks = new Collection<CswPrimaryKey>();

        private void _setLoad( ICswResources CswResources )
        {
            _StatusMessage = string.Empty;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( (CswNbtResources) CswResources );
            foreach( CswNbtObjClassGenerator Generator in _CswScheduleLogicNodes.getGenerators() )
            {
                if( _doesGeneratorRunNow( Generator ) )
                {
                    _GeneratorPks.Add( Generator.NodeId );
                }
            }
        }

        #endregion State

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //Determines the number of generators with targets to create and returns that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            if( _GeneratorPks.Count == 0 )
            {
                _setLoad( CswResources );
            }
            _CswScheduleLogicDetail.LoadCount = _GeneratorPks.Count;
            return _CswScheduleLogicDetail.LoadCount;
        } 

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( CswNbtResources );

            try
            {
                Int32 GeneratorLimit = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.generatorlimit.ToString() ) );
                if( Int32.MinValue == GeneratorLimit )
                {
                    GeneratorLimit = 1;
                }

                Int32 GeneratorsProcessed = 0;
                while( GeneratorsProcessed < GeneratorLimit && _GeneratorPks.Count > 0 && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ) )
                {
                    _processGenerator( CswNbtResources, CswNbtResources.Nodes[_GeneratorPks[0]] );
                    GeneratorsProcessed ++;
                    _GeneratorPks.RemoveAt( 0 ); 
                }

                _TotalGeneratorsProcessed += GeneratorsProcessed;
                _CswScheduleLogicDetail.StatusMessage = _TotalGeneratorsProcessed.ToString() + " generators processed: " + _StatusMessage;
                _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line
            }
            catch( Exception Exception )
            {
                _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenNode::GetUpdatedItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
            }
        }//threadCallBack()

        private bool _doesGeneratorRunNow( CswNbtObjClassGenerator CurrentGenerator )
        {
            bool RunNow = false;
            DateTime ThisDueDateValue = CurrentGenerator.NextDueDate.DateTimeValue.Date;
            DateTime InitialDueDateValue = CurrentGenerator.DueDateInterval.getStartDate().Date;
            if( InitialDueDateValue == DateTime.MinValue )
            {
                InitialDueDateValue = ThisDueDateValue;
            }
            DateTime FinalDueDateValue = CurrentGenerator.FinalDueDate.DateTimeValue.Date;
            if( ThisDueDateValue != DateTime.MinValue )
            {
                if( CurrentGenerator.RunTime.DateTimeValue != DateTime.MinValue &&
                    CswEnumRateIntervalType.Hourly != CurrentGenerator.DueDateInterval.RateInterval.RateType ) // Ignore runtime for hourly generators
                {
                    ThisDueDateValue = ThisDueDateValue.AddTicks( CurrentGenerator.RunTime.DateTimeValue.TimeOfDay.Ticks );
                }

                Int32 WarnDays = CswConvert.ToInt32( CurrentGenerator.WarningDays.Value );
                if( WarnDays > 0 )
                {
                    TimeSpan WarningDaysSpan = new TimeSpan( WarnDays, 0, 0, 0, 0 );
                    ThisDueDateValue = ThisDueDateValue.Subtract( WarningDaysSpan );
                    InitialDueDateValue = InitialDueDateValue.Subtract( WarningDaysSpan );
                }

                // if we're within the initial and final due dates, but past the current due date (- warning days) and runtime
                RunNow = ( DateTime.Now.Date >= InitialDueDateValue ) &&
                         ( DateTime.Now.Date <= FinalDueDateValue || DateTime.MinValue.Date == FinalDueDateValue ) &&
                         ( DateTime.Now >= ThisDueDateValue );
            }
            return RunNow;
        }

        private void _processGenerator( CswNbtResources CswNbtResources, CswNbtObjClassGenerator CurrentGenerator )
        {
            try
            {
                // case 28069
                // It should not be possible to make more than 24 nodes per parent in a single day, 
                // since the fastest interval is 1 hour, and we're not creating things into the past anymore.
                // Therefore, disable anything that is erroneously spewing things.
                if( CurrentGenerator.GeneratedNodeCount( DateTime.Today ) >= ( 24 * CurrentGenerator.TargetParents.Count ) )
                {
                    string Message = "Disabled due to error: Generated too many " + CurrentGenerator.TargetType.SelectedNodeTypeNames() + " targets in a single day";
                    _StatusMessage += Message + "; ";
                    CurrentGenerator.Enabled.Checked = CswEnumTristate.False;
                    CurrentGenerator.RunStatus.AddComment( Message );
                    CurrentGenerator.postChanges( false );
                }
                else
                {
                    CswNbtActGenerateNodes CswNbtActGenerateNodes = new CswNbtActGenerateNodes( CswNbtResources );
                    bool Finished = CswNbtActGenerateNodes.makeNode( CurrentGenerator.Node );
                    if( Finished ) // case 26111
                    {
                        string Message = "Created all " + CurrentGenerator.TargetType.SelectedNodeTypeNames() + " target(s) for " + CurrentGenerator.NextDueDate.DateTimeValue.Date.ToShortDateString();
                        CurrentGenerator.RunStatus.AddComment( Message );
                        CurrentGenerator.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                        CurrentGenerator.postChanges( false );
                    }
                    _StatusMessage += CurrentGenerator.Description.Text + "; ";
                } // if-else( CurrentGenerator.GeneratedNodeCount( DateTime.Today ) >= 24 )
            } //try
            catch( Exception Exception )
            {
                string Message = "Unable to process generator " + CurrentGenerator.Description.Text + ", which will now be disabled, due to the following exception: " + Exception.Message;
                _StatusMessage += Message + "; ";
                CurrentGenerator.Enabled.Checked = CswEnumTristate.False;
                CurrentGenerator.RunStatus.AddComment( "Disabled due to exception: " + Exception.Message );
                CurrentGenerator.postChanges( false );
                CswNbtResources.logError( new CswDniException( Message ) );
            }
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtGenNode

}//namespace ChemSW.Nbt.Sched
