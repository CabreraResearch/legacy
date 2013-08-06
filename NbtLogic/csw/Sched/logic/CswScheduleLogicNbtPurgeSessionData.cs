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
            return ( " where lower(accessid) = '" + AccessId.ToLower() + "' and ( timeoutdate + 1/24 ) < sysdate" );
        }//SessionListWhere

        public Int32 getLoadCount( ICswResources CswResources )
        {

            Int32 ReturnVal = 0;

            _MasterSchemaResources = _getMasterSchemaResources( (CswNbtResources) CswResources );

            if( null != _MasterSchemaResources )
            {

                CswArbitrarySelect CswArbitrarySelectSessionList = _MasterSchemaResources.makeCswArbitrarySelect( "expired_session_list_query",
                    "select count(*) as cnt from sessionlist" + _SessionListWhere( CswResources.AccessId ) );

                DataTable SessionListTable = CswArbitrarySelectSessionList.getTable();
                Int32 ExpiredSessionRecordCount = CswConvert.ToInt32( SessionListTable.Rows[0]["cnt"] );
                Int32 OrhpanSessionDataCount = _getOrphanRowCount( (CswNbtResources) CswResources );
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

            return ( ReturnVal );

        }//getLoadCount() 

        private CswNbtResources _getMasterSchemaResources( CswNbtResources CswResources )
        {
            CswNbtResources ReturnVal = CswNbtResourcesFactory.makeCswNbtResources( CswResources );

            ReturnVal.AccessId = ReturnVal.CswDbCfgInfo.MasterAccessId;

            return ( ReturnVal );

        }//_getMasterSchemaResources()

        /// <summary>
        /// Get the number of rows in the current schema's session_data that do not have a corresponding sessionid in Master's sessionlist
        /// </summary>
        /// <remarks>
        /// This assumes you have initialized _MasterSchemaResources. 
        /// Note that the first part of the threadCallBack() might generate more orphans, so this select needs to be re-done after we remove session ids
        /// </remarks>
        private Int32 _getOrphanRowCount( CswNbtResources CswNbtResources )
        {
            CswTableSelect DoomedSessionDataTS = CswNbtResources.makeCswTableSelect( "purge_doomed_session_data", "session_data" );
            DataTable DoomedSessionDataDT = DoomedSessionDataTS.getTable( _getMasterSessionIdsWhere() );
            return DoomedSessionDataDT.Rows.Count;
        }

        /// <summary>
        /// Assumes the _MasterSchemaResources is initialized
        /// </summary>
        /// <returns></returns>
        private string _getMasterSessionIdsWhere()
        {
            string ret = "";
            bool first = true;
            CswTableSelect SessionListIdsTS = _MasterSchemaResources.makeCswTableSelect( "get_all_session_ids", "sessionlist" );
            DataTable RemainingSessionListIdsDT = SessionListIdsTS.getTable( new CswCommaDelimitedString { "sessionid" } );
            CswCommaDelimitedString SessionListIds = new CswCommaDelimitedString( 0, "'" );
            foreach( DataRow Row in RemainingSessionListIdsDT.Rows )
            {
                if( SessionListIds.Count == 999 ) //can't have more than 1000 literals in an "in" clause"
                {
                    if( first )
                    {
                        ret = " where sessionid not in (" + SessionListIds.ToString() + ") ";
                        first = false;
                    }
                    else
                    {
                        ret += " and sessionid not in (" + SessionListIds.ToString() + ") ";
                    }
                    SessionListIds.Clear();
                }
                SessionListIds.Add( Row["sessionid"].ToString() );
            }

            if( SessionListIds.Count > 0 ) //if we never reached the max in the latest iteration
            {
                if( first ) //if we never reached the max period
                {
                    ret = " where sessionid not in (" + SessionListIds.ToString() + ") ";
                }
                else //if we never reached the max on any iteration other than the first
                {
                    ret += " and sessionid not in (" + SessionListIds.ToString() + ") ";
                }
            }

            return ret;
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

                            //Case 30266 - remove all rows in the current schema's session_data with no corresponding session id in the master schema's SessionList
                            CswTableUpdate DoomedSessionDataTU = CurrentSchemaResources.makeCswTableUpdate( "purge_doomed_session_data", "session_data" );
                            DataTable DoomedSessionDataDT = DoomedSessionDataTU.getTable( _getMasterSessionIdsWhere() );
                            foreach( DataRow DoomedRow in DoomedSessionDataDT.Rows )
                            {
                                DoomedRow.Delete();
                            }
                            DoomedSessionDataTU.update( DoomedSessionDataDT );

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
