using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtDisableCswAdmin : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.DisableChemSwAdmin.ToString() ); }
        }

        public bool doesItemRunNow()
        {
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrence, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
        }

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }


        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        private CswNbtResources _CswNbtResources = null;
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            _CswNbtResources.AuditContext = "Scheduler Task: Disable ChemSW Admin User";

        }//init()

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    CswNbtNode ChemSWAdminUserNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
                    CswNbtObjClassUser CswAdminAsUser = CswNbtNodeCaster.AsUser( ChemSWAdminUserNode );
                    if( false == _CswNbtResources.ModulesEnabled().Contains( CswNbtResources.CswNbtModule.NBTManager ) )
                    {
                        CswAdminAsUser.AccountLocked.Checked = Tristate.True;
                        CswAdminAsUser.PasswordProperty.ChangedDate = DateTime.MinValue;
                    }
                    else
                    {
                        CswAdminAsUser.AccountLocked.Checked = Tristate.False;
                        CswAdminAsUser.FailedLoginCount.Value = 0;
                        _CswNbtResources.ConfigVbls.setConfigVariableValue( CswNbtResources.ConfigurationVariables.password_length.ToString(), "16" );
                        _CswNbtResources.ConfigVbls.setConfigVariableValue( CswNbtResources.ConfigurationVariables.passwordexpiry_days.ToString(), "30" );
                    }
                    ChemSWAdminUserNode.postChanges( true );

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line


                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtDisableCswAdmin::GetUpdatedItems() exception: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
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
            _LogicRunStatus = LogicRunStatus.Idle;
        }

        public void releaseResources()
        {
            _CswNbtResources.release();
        }

    }//CswScheduleLogicNbtDisableCswAdmin


}//namespace ChemSW.Nbt.Sched
