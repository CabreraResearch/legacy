using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtDisableCswAdmin : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.DisableChemSwAdmin ); }
        }

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }


        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }//initScheduleLogicDetail()

        //In the case where the rule always has 'work' to do, the rule should only have load when the rule is scheduled to run.
        //This is necessary to stop the rule from running once it has completed its job.
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = _CswScheduleLogicDetail.doesItemRunNow() ? 1 : 0;
            return _CswScheduleLogicDetail.LoadCount;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    CswNbtNode ChemSWAdminUserNode = CswNbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
                    CswNbtObjClassUser CswAdminAsUser = (CswNbtObjClassUser) ChemSWAdminUserNode;
                    if( false == CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.NBTManager ) )
                    {
                        CswAdminAsUser.AccountLocked.Checked = Tristate.True;
                        CswAdminAsUser.PasswordProperty.ChangedDate = DateTime.MinValue;
                    }
                    else
                    {
                        CswAdminAsUser.AccountLocked.Checked = Tristate.False;
                        CswAdminAsUser.FailedLoginCount.Value = 0;
                        CswNbtResources.ConfigVbls.setConfigVariableValue( CswNbtResources.ConfigurationVariables.password_length.ToString(), "16" );
                        CswNbtResources.ConfigVbls.setConfigVariableValue( CswNbtResources.ConfigurationVariables.passwordexpiry_days.ToString(), "30" );
                    }
                    ChemSWAdminUserNode.postChanges( ForceUpdate: true );

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line


                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtDisableCswAdmin::GetUpdatedItems() exception: " + Exception.Message;
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
            _LogicRunStatus = LogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtDisableCswAdmin

}//namespace ChemSW.Nbt.Sched
