using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Session;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtPurgeSessionData: ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.PurgeSessionData ); }
        }

        private bool _StaleDataExists = false;
        private CswNbtResources _MasterSchemaResources = null;


        //if we don't filter session list for the curent rule's accessid,
        //another accessid's rule will nuke the session record and this
        //accessid's temp nodes won't get nuked 
        private string _SessionListWhere( string AccessId )
        {
            return ( " where lower(sl.accessid) = '" + AccessId.ToLower() + "' and ( sl.timeoutdate + 1/24 ) < sysdate" );
        }//SessionListWhere

        private string _makeLoadCountSql( string AccessId )
        {
            string sql = @"with expiredcounts as (select count(*) as expired_cnt from sessionlist sl + " + _SessionListWhere( AccessId ) + @",
                          orphancounts as (select count(*) as orphan_cnt from session_data sd where sd.sessionid not in (select sl.sessionid from sessionlist sl))
                          select expiredcounts.expired_cnt, orphancounts.orphan_cnt from expiredcounts, orphancounts";

            return sql;
        }

        public Int32 getLoadCount( ICswResources CswResources )
        {

            Int32 ReturnVal = 0;

            try
            {
                _MasterSchemaResources = _getMasterSchemaResources( (CswNbtResources) CswResources );

                if( null != _MasterSchemaResources )
                {

                    string sql = @"se";
                    CswArbitrarySelect CswArbitrarySelectSessionList = _MasterSchemaResources.makeCswArbitrarySelect( "expired_session_list_query", _makeLoadCountSql( CswResources.AccessId ) );
                    DataTable SessionListTable = CswArbitrarySelectSessionList.getTable();
                    Int32 ExpiredSessionRecordCount = CswConvert.ToInt32( SessionListTable.Rows[0]["cnt"] );
                    Int32 OrhpanSessionDataCount = CswConvert.ToInt32( SessionListTable.Rows[0]["cnt"] );
                    if( ExpiredSessionRecordCount > 0 || OrhpanSessionDataCount > 0 )
                    {

                        ReturnVal = ExpiredSessionRecordCount + OrhpanSessionDataCount;
                        _StaleDataExists = true;

                    }

                    _CswScheduleLogicDetail.LoadCount = ReturnVal;
                }
                else
                {
                    CswResources.CswLogger.reportError( new CswDniException( "Unable to get load count of sessionlist records: The master schema resource object is null" ) );
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

                    if( null == _MasterSchemaResources )
                    {
                        _MasterSchemaResources = _getMasterSchemaResources( (CswNbtResources) CswResources );
                    }

                    if( null != _MasterSchemaResources )
                    {
                        if( _StaleDataExists )
                        {

                            //The higher level classes that CswSessions is used require http reponse and request objects, 
                            //so we have to use th bare CswSessions class directly.
                            CswSessionsFactory CswSessionsFactory = new CswSessionsFactory( CswEnumAppType.Nbt, _MasterSchemaResources.SetupVbls, _MasterSchemaResources.CswDbCfgInfo, false, _MasterSchemaResources.CswLogger );
                            CswSessions CswSessions = CswSessionsFactory.make( CswEnumSessionsStorageType.DbStorage, _MasterSchemaResources );


                            //We must get and delete the session data from the master schema, 
                            //but delete expired temp nodes from the current schema
                            CswTableSelect SessionListSelect = _MasterSchemaResources.makeCswTableSelect( "delete_expired_sessionlist_records", "sessionlist" );
                            DataTable SessionListTable = SessionListSelect.getTable( _SessionListWhere( CurrentSchemaResources.AccessId ) );
                            CswNbtSessionDataMgr CswNbtSessionDataMgr = new CswNbtSessionDataMgr( CurrentSchemaResources );

                            foreach( DataRow CurrentRow in SessionListTable.Rows )
                            {
                                //Step # 1: Remove stranded temp nodes in the _current_ schema using session id we got from master schema session list
                                string CurrentSessionId = CurrentRow["sessionid"].ToString();
                                CswNbtSessionDataMgr.removeAllSessionData( CurrentSessionId );


                                //Step # 2: Remove Session Record from master schema
                                //If our session management code were organized differently, we would be calling 
                                //CswSessionManager::clearSession() instead of rolloing our own here. In the future
                                //CswSessionManager::clearSession() could acquire functionality that we would miss. 
                                //Moreover, it calls an OnDeathenticate() event that is passsed in from 
                                //CswSessionResourcesNbt. Using the aforementioned chain of classes here would be 
                                //problematic because of said classes deep-endencies on, for example, various http
                                //classes. So, if we add something in one place that the other place should also be 
                                //doing, we'll have to add it manually. 
                                CswSessionsListEntry CswSessionsListEntry = new CswSessionsListEntry( CurrentSessionId );
                                CswSessions.remove( CswSessionsListEntry );

                            } //iterate session records

                            //Case 30266 - remove all session data with no corresponding session id in SessionList
                            CswNbtSessionDataMgr.removeOrphanedSessionData();

                        } //_StaleDataExists

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
                    _StaleDataExists = false;
                    _MasterSchemaResources.release();

                    //These must be marked null so that they get garbage collected
                    _MasterSchemaResources = null;
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
