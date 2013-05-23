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
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.GenNode ); }
        }

        //Determine the number of generators with targets to create and returns that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            Int32 NumOfGeneratorsAndTargets = 0;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( (CswNbtResources) CswResources );
            Collection<CswNbtObjClassGenerator> ObjectGenerators = _CswScheduleLogicNodes.getGenerators();
            for( Int32 idx = 0; idx < ObjectGenerators.Count; idx++ )
            {
                if( _doesGeneratorRunNow( ObjectGenerators[idx] ) )
                {
                    NumOfGeneratorsAndTargets++;
                }
            }
            _CswScheduleLogicDetail.LoadCount = NumOfGeneratorsAndTargets;
            return _CswScheduleLogicDetail.LoadCount;
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;
        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }//initScheduleLogicDetail()

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( CswNbtResources );

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    Int32 GeneratorLimit = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.generatorlimit.ToString() ) );
                    if( Int32.MinValue == GeneratorLimit )
                    {
                        GeneratorLimit = 1;
                    }

                    Collection<CswNbtObjClassGenerator> ObjectGenerators = _CswScheduleLogicNodes.getGenerators();

                    Int32 TotalGeneratorsProcessed = 0;
                    string GeneratorDescriptions = string.Empty;

                    for( Int32 idx = 0; ( idx < ObjectGenerators.Count && TotalGeneratorsProcessed < GeneratorLimit ) && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassGenerator CurrentGenerator = ObjectGenerators[idx];

                        if( CurrentGenerator.Enabled.Checked == CswEnumTristate.True )
                        {
                            try
                            {
                                // if we're within the initial and final due dates, but past the current due date (- warning days) and runtime
                                if( _doesGeneratorRunNow( CurrentGenerator ) )
                                {
                                    // case 28069
                                    // It should not be possible to make more than 24 nodes per parent in a single day, 
                                    // since the fastest interval is 1 hour, and we're not creating things into the past anymore.
                                    // Therefore, disable anything that is erroneously spewing things.
                                    if( CurrentGenerator.GeneratedNodeCount( DateTime.Today ) >= ( 24*CurrentGenerator.TargetParents.Count ) )
                                    {
                                        CurrentGenerator.Enabled.Checked = CswEnumTristate.False;
                                        CurrentGenerator.RunStatus.AddComment( "Disabled due to error: Generated too many " + CurrentGenerator.TargetType.SelectedNodeTypeNames() + " target(s) in a single day" );
                                        CurrentGenerator.postChanges( false );
                                    }
                                    else
                                    {
                                        CswNbtActGenerateNodes CswNbtActGenerateNodes = new CswNbtActGenerateNodes( CswNbtResources );
                                        bool Finished = CswNbtActGenerateNodes.makeNode( CurrentGenerator.Node );
                                        if( Finished ) // case 26111
                                        {
                                            string Message = "Created all " + CurrentGenerator.TargetType.SelectedNodeTypeNames() + " target(s) for " + CurrentGenerator.NextDueDate.DateTimeValue.Date.ToShortDateString();
                                            //case 25702 - add comment:
                                            if( false == String.IsNullOrEmpty( Message ) )
                                            {
                                                CurrentGenerator.RunStatus.AddComment( Message );
                                            }
                                            CurrentGenerator.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                                            CurrentGenerator.postChanges( false );
                                        }

                                        GeneratorDescriptions += CurrentGenerator.Description.Text + "; ";
                                        //Case 29684: only increment if the generator actually runs
                                        TotalGeneratorsProcessed += 1;
                                    } // if-else( CurrentGenerator.GeneratedNodeCount( DateTime.Today ) >= 24 )
                                } // if due
                            } //try
                            catch( Exception Exception )
                            {
                                string Message = "Unable to process generator " + CurrentGenerator.Description.Text + ", which will now be disabled, due to the following exception: " + Exception.Message;
                                GeneratorDescriptions += Message;
                                CurrentGenerator.Enabled.Checked = CswEnumTristate.False;
                                CurrentGenerator.RunStatus.AddComment( "Disabled due do exception: " + Exception.Message );
                                CurrentGenerator.postChanges( false );
                                CswNbtResources.logError( new CswDniException( Message ) );

                            } //catch

                        } // if( CurrentGenerator.Enabled.Checked == Tristate.True )

                    }//iterate generators

                    _CswScheduleLogicDetail.StatusMessage = TotalGeneratorsProcessed.ToString() + " generators processed: " + GeneratorDescriptions;
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenNode::GetUpdatedItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

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

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtGenNode

}//namespace ChemSW.Nbt.Sched
