using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenNode : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.GenNode.ToString() ); }
        }

        public bool hasLoad( ICswResources CswResources )
        {
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
            return ( true );
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
        }

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }



        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;
        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;


        }//initScheduleLogicDetail()


        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( CswNbtResources );


            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    Int32 GeneratorLimit = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.generatorlimit.ToString() ) );
                    if( Int32.MinValue == GeneratorLimit )
                    {
                        GeneratorLimit = 1;
                    }

                    Collection<CswNbtObjClassGenerator> ObjectGenerators = _CswScheduleLogicNodes.getGenerators();

                    Int32 TotalGeneratorsProcessed = 0;
                    string GeneratorDescriptions = string.Empty;

                    for( Int32 idx = 0; ( idx < ObjectGenerators.Count && TotalGeneratorsProcessed < GeneratorLimit ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassGenerator CurrentGenerator = ObjectGenerators[idx];

                        if( CurrentGenerator.Enabled.Checked == Tristate.True )
                        {

                            try
                            {


                                DateTime ThisDueDateValue = CurrentGenerator.NextDueDate.DateTimeValue.Date;
                                DateTime InitialDueDateValue = CurrentGenerator.DueDateInterval.getStartDate().Date;
                                if( InitialDueDateValue == DateTime.MinValue )
                                {
                                    InitialDueDateValue = ThisDueDateValue;
                                }
                                DateTime FinalDueDateValue = CurrentGenerator.FinalDueDate.DateTimeValue.Date;

                                // BZ 7866
                                if( ThisDueDateValue != DateTime.MinValue )
                                {
                                    // BZ 7124 - set runtime
                                    if( CurrentGenerator.RunTime.DateTimeValue != DateTime.MinValue &&
                                        CswRateInterval.RateIntervalType.Hourly != CurrentGenerator.DueDateInterval.RateInterval.RateType ) // Ignore runtime for hourly generators
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
                                    if( ( DateTime.Now.Date >= InitialDueDateValue ) &&
                                        ( DateTime.Now.Date <= FinalDueDateValue || DateTime.MinValue.Date == FinalDueDateValue ) &&
                                        ( DateTime.Now >= ThisDueDateValue ) )
                                    {
                                        // case 28069
                                        // It should not be possible to make more than 24 nodes per parent in a single day, 
                                        // since the fastest interval is 1 hour, and we're not creating things into the past anymore.
                                        // Therefore, disable anything that is erroneously spewing things.
                                        if( CurrentGenerator.GeneratedNodeCount( DateTime.Today ) >= ( 24*CurrentGenerator.TargetParents.Count ) )
                                        {
                                            CurrentGenerator.Enabled.Checked = Tristate.False;
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

                                            GeneratorDescriptions += CurrentGenerator.Description + "; ";
                                        } // if-else( CurrentGenerator.GeneratedNodeCount( DateTime.Today ) >= 24 )
                                    } // if due
                                } // if( ThisDueDateValue != DateTime.MinValue )

                            } //try

                            catch( Exception Exception )
                            {
                                string Message = "Unable to process generator " + CurrentGenerator.Description + ", which will now be disabled, due to the following exception: " + Exception.Message;
                                GeneratorDescriptions += Message;
                                CurrentGenerator.Enabled.Checked = Tristate.False;
                                CurrentGenerator.RunStatus.AddComment( "Disabled due do exception: " + Exception.Message );
                                CurrentGenerator.postChanges( false );
                                CswNbtResources.logError( new CswDniException( Message ) );


                            } //catch
                            finally
                            {
                                //Review K4566: always increment https://fogbugz.chemswlive.com/kiln/Review/K4566
                                TotalGeneratorsProcessed += 1;                                
                            }

                        } // if( CurrentGenerator.Enabled.Checked == Tristate.True )



                    }//iterate generators

                    _CswScheduleLogicDetail.StatusMessage = TotalGeneratorsProcessed.ToString() + " generators processed: " + GeneratorDescriptions;
                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenNode::GetUpdatedItems() exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
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
    }//CswScheduleLogicNbtGenNode


}//namespace ChemSW.Nbt.Sched
