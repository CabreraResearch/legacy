using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Session;
using ChemSW.Session;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtPurgeSessionData : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.PurgeSessionData ); }
        }

        private bool _ExpiredSessionListRecsExist = false;
        private CswNbtResources _MasterSchemaResources = null;
        private CswSessions _CswSessions = null;


        //if we don't filter session list for the curent rule's accessid,
        //another accessid's rule will nuke the session record and this
        //accessid's temp nodes won't get nuked 
        private string _SessionListWhere( string AccessId )
        {
            return ( " where lower(accessid) = '" + AccessId.ToLower() + "' and timeoutdate < sysdate" );
        }//SessionListWhere


        public Int32 getLoadCount( ICswResources CswResources )
        {

            Int32 ReturnVal = 0;

            try
            {
                _MasterSchemaResources = _getMasterSchemaResources( (CswNbtResources) CswResources );

                if( null != _MasterSchemaResources )
                {

                    CswArbitrarySelect CswArbitrarySelectSessionList = _MasterSchemaResources.makeCswArbitrarySelect( "expired_session_list_query", "select count(*) as \"cnt\" from sessionlist " + _SessionListWhere( CswResources.AccessId ) );
                    DataTable SessionListTable = CswArbitrarySelectSessionList.getTable();
                    Int32 ExpiredSessionRecordCount = CswConvert.ToInt32( SessionListTable.Rows[0]["cnt"] );
                    if( ExpiredSessionRecordCount > 0 )
                    {

                        ReturnVal = ExpiredSessionRecordCount;
                        _ExpiredSessionListRecsExist = true;



                        //The higher level classes that CswSessions is used require http reponse and request objects, 
                        //so we have to use th bare CswSessions class directly.
                        CswSessionsFactory CswSessionsFactory = new CswSessionsFactory( CswEnumAppType.Nbt, _MasterSchemaResources.SetupVbls, _MasterSchemaResources.CswDbCfgInfo, false, _MasterSchemaResources.CswLogger );
                        _CswSessions = CswSessionsFactory.make( CswEnumSessionsStorageType.DbStorage, _MasterSchemaResources );

                    }

                    _CswScheduleLogicDetail.LoadCount = ReturnVal;
                }
                else
                {
                    CswResources.CswLogger.reportError( new CswDniException( "Unable to get load count of sessionlist records: The master schmea resource object is null" ) );
                }
            }

            catch( Exception Exception )
            {
                CswResources.CswLogger.reportError( Exception );

            }//catch()

            return ( ReturnVal );

        }//getLoadCount() 

        private CswNbtResources _getMasterSchemaResources( CswNbtResources CswResources )
        {
            CswNbtResources ReturnVal = CswNbtResourcesFactory.makeCswNbtResources( CswResources );

            ReturnVal.AccessId = ReturnVal.CswDbCfgInfo.MasterAccessId;

            return ( ReturnVal );

        }//_getMasterSchemaResources()

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
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CurrentSchemaResources = (CswNbtResources) CswResources;

            CurrentSchemaResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( null != _MasterSchemaResources )
                    {
                        if( _ExpiredSessionListRecsExist )
                        {

                            //We must get and delete the session data from the master schema, 
                            //but delete expired temp nodes from the current schema
                            CswTableSelect SessionListSelect = _MasterSchemaResources.makeCswTableSelect( "delete_expired_sessionlist_records", "sessionlist" );
                            DataTable SessionListTable = SessionListSelect.getTable( _SessionListWhere( CurrentSchemaResources.AccessId ) );

                            foreach( DataRow CurrentRow in SessionListTable.Rows )
                            {
                                //Step # 1: Remove stranded temp nodes in the _current_ schema using session id we got from master schema session list
                                CswNbtSessionDataMgr CswNbtSessionDataMgr = new CswNbtSessionDataMgr( CurrentSchemaResources );
                                string CurrentSessionId = CurrentRow["sessionid"].ToString();
                                CswNbtSessionDataMgr.removeAllSessionData( CurrentSessionId );


                                //Step # 2: Remove Session Record from master schema
                                //CswSessionsFactory CswSessionsListFactory = new CswSessionsListFactory();
                                CswSessionsListEntry CswSessionsListEntry = new CswSessionsListEntry( CurrentSessionId );
                                _CswSessions.remove( CswSessionsListEntry );

                            } //iterate session records

                        } //_ExpiredSessionListRecsExist

                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                        _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                    }
                    else
                    {
                        CswResources.CswLogger.reportError( new CswDniException( "Unable to process sessionlist records: The master schmea resource object is null" ) );
                    }

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtPurgeSessionData exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CurrentSchemaResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

                finally
                {
                    _ExpiredSessionListRecsExist = false;
                    _MasterSchemaResources.release();

                    //These must be marked null so that they get garbage collected
                    _MasterSchemaResources = null;
                    _CswSessions = null;

                }

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
    }//CswScheduleLogicNbtPurgeSessionData

}//namespace ChemSW.Nbt.Sched
