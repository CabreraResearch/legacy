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
            get { return ( CswEnumNbtScheduleRuleNames.DisableChemSwAdmin ); }
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

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }//initScheduleLogicDetail()

        //In the case where the rule always has 'work' to do, the rule should only have load when the rule is scheduled to run.
        //This is necessary to stop the rule from running once it has completed its job.
        public Int32 getLoadCount( ICswResources CswResources )
        {
            CswNbtResources NbtResources = ( CswNbtResources ) CswResources;
            CswNbtObjClassUser ChemSWAdminUser = NbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
            _CswScheduleLogicDetail.LoadCount = 
                false == NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.NBTManager ) && 
                ChemSWAdminUser.AccountLocked.Checked == CswEnumTristate.True ? 0 : 1;
            //_CswScheduleLogicDetail.LoadCount = _CswScheduleLogicDetail.doesItemRunNow() ? 1 : 0;
            return _CswScheduleLogicDetail.LoadCount;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    CswNbtNode ChemSWAdminUserNode = CswNbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
                    CswNbtObjClassUser CswAdminAsUser = (CswNbtObjClassUser) ChemSWAdminUserNode;
                    if( false == CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.NBTManager ) )
                    {
                        CswAdminAsUser.AccountLocked.Checked = CswEnumTristate.True;
                        CswAdminAsUser.PasswordProperty.ChangedDate = DateTime.MinValue;
                    }
                    else
                    {
                        CswAdminAsUser.AccountLocked.Checked = CswEnumTristate.False;
                        CswAdminAsUser.FailedLoginCount.Value = 0;
                        CswNbtResources.ConfigVbls.setConfigVariableValue( CswEnumNbtConfigurationVariables.password_length.ToString(), "16" );
                        CswNbtResources.ConfigVbls.setConfigVariableValue( CswEnumNbtConfigurationVariables.passwordexpiry_days.ToString(), "30" );
                    }
                    ChemSWAdminUserNode.postChanges( ForceUpdate: true );

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line


                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtDisableCswAdmin::GetUpdatedItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;

                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtDisableCswAdmin

}//namespace ChemSW.Nbt.Sched
