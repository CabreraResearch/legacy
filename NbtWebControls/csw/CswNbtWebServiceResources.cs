using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Data;
using System.Configuration;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using ChemSW.Nbt.Config;
using ChemSW.Audit;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Security;
using ChemSW.Nbt.Security;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.CswWebControls;
using ChemSW.Nbt.Statistics;
using ChemSW.Nbt.Config;
using ChemSW.Session; 

namespace ChemSW.Nbt
{
    public enum EndSessionMode { esmCommit, esmRollback, esmRelease };
    public class CswNbtWebServiceResources
    {
        private CswNbtResources _CswNbtResources = null;
        public CswNbtResources CswNbtResources { get { return ( _CswNbtResources ); } }

        private CswAuthenticator _CswAuthenticator = null;

        private CswSessionStorageDb _CswSessionStorageDb = null; 

        public CswNbtWebServiceResources( HttpApplicationState HttpApplicationState, HttpSessionState HttpSessionState, HttpRequest HttpRequest, HttpResponse HttpResponse, string LoginAccessId, string FilesPath, SetupMode SetupMode )
        {
            CswSetupVblsNbt CswSetupVbls = new CswSetupVblsNbt( SetupMode.Web );
            CswDbCfgInfoNbt CswDbCfgInfo = new CswDbCfgInfoNbt( SetupMode.Web );
            string ConfigurationFilePath = CswTools.getConfigurationFilePath( SetupMode.Web );
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( CswSetupVbls, CswDbCfgInfo, ConfigurationFilePath );
            _CswAuthenticator = new CswAuthenticator( _CswNbtResources, new CswNbtAuthenticator( _CswNbtResources ), _CswNbtResources.MD5Seed );
            _CswSessionStorageDb = new Session.CswSessionStorageDb( AppType.Nbt, CswSetupVbls, CswDbCfgInfo, false );
        }//ctor




        public AuthenticationStatus authenticate( string AccessId, string UserName, string Password, ref string EuphemisticStatus, ref string SessionId )
        {
            AuthenticationStatus ReturnVal = AuthenticationStatus.Unknown;

            Int32 RoleTimeout = Int32.MinValue;
            CswPrimaryKey UserId = null;
            _CswNbtResources.AccessId = AccessId;


            ReturnVal = _CswAuthenticator.Authenticate( AccessId, UserName, Password, CswNbtWebTools.getIpAddress(), 0, ref RoleTimeout, ref UserId );
            EuphemisticStatus = _CswAuthenticator.euphemizeAuthenticationStatus( ReturnVal );

            if( AuthenticationStatus.Authenticated == ReturnVal )
            {
                CswSessionsListEntry CswSessionsListEntry = new CswSessionsListEntry( SessionId = System.Guid.NewGuid().ToString() );
                CswSessionsListEntry.AccessId = AccessId;
                CswSessionsListEntry.IPAddress = CswNbtWebTools.getIpAddress();
                CswSessionsListEntry.LoginDate = DateTime.Now;
                CswSessionsListEntry.UserName = UserName;
                
                _CswSessionStorageDb.save( CswSessionsListEntry ); 

                _CswNbtResources.CurrentUser = _CswAuthenticator.makeUser( UserName );

                //CswTableUpdate CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtWebServiceResources.makeSession()", "sessionlist" );
                //DataTable DataTableSessionListUpdate = CswTableUpdate.getEmptyTable();
                //DataRow NewRow = DataTableSessionListUpdate.NewRow();
                //NewRow["AccessId"] = AccessId;
                //NewRow["UserName"] = UserName;
                //NewRow["IpAddress"] = CswNbtWebTools.getIpAddress();
                //NewRow["SessionId"] = SessionId = System.Guid.NewGuid().ToString();
                //NewRow["LoginDate"] = DateTime.Now;
                //DataTableSessionListUpdate.Rows.Add( NewRow );
                //CswTableUpdate.update( DataTableSessionListUpdate );

            }

            return ( ReturnVal );

        }//authenticate()

        public void deAuthenticate( string SessionId )
        {

            _CswSessionStorageDb.remove( SessionId ); 
            //CswTableUpdate CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtWebServiceResources.deAuthenticate()", "sessionlist" );
            //DataTable DataTableDeleteSessionList = CswTableUpdate.getTable( " where sessionid = '" + SessionId + "'", true );

            ////there could be stale records as well
            //if( 1 == DataTableDeleteSessionList.Rows.Count  )
            //{
            //    DataTableDeleteSessionList.Rows[0].Delete(); 
            //    CswTableUpdate.update( DataTableDeleteSessionList );
            //}


        }//deAuthenticate()

        public AuthenticationStatus startSession( string SessionId , ref string EuphemisticAuthenticationStatus )
        {

            AuthenticationStatus ReturnVal = AuthenticationStatus.Unknown;

            CswSessionsListEntry CswSessionsListEntry  = null; 
            if( null != ( CswSessionsListEntry = _CswSessionStorageDb.get( SessionId ) ) )
            {
                ReturnVal = AuthenticationStatus.Authenticated;
                _CswNbtResources.AccessId = CswSessionsListEntry.AccessId;
                _CswNbtResources.CurrentUser = _CswAuthenticator.makeUser( CswSessionsListEntry.UserName);
            }
            else
            {
                ReturnVal = AuthenticationStatus.NonExistentSession; 
            }//if-else

            //CswArbitrarySelect CswArbitrarySelectSessionList = _CswNbtResources.makeCswArbitrarySelect( "CswNbtWebServiceResources.resumeSession()", "select username,accessid,sessionlistid from sessionlist order by logindate asc" );
            //DataTable DataTable = CswArbitrarySelectSessionList.getTable();

            //if( DataTable.Rows.Count > 0 )
            //{
            //    ReturnVal = AuthenticationStatus.Authenticated;

            //    if( DataTable.Rows.Count > 1 )
            //    {
            //        string DeleteIds = string.Empty;
            //        for( int idx = 0; idx < ( DataTable.Rows.Count - 1 ); idx++ )
            //        {
            //            if( string.Empty != DeleteIds )
            //            {
            //                DeleteIds += ",";
            //            }

            //            DeleteIds += DataTable.Rows[idx]["sessionlistid"].ToString();
            //        }//

            //        CswTableUpdate CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtWebServiceResources.resumeSession()", "sessionlist" );
            //        DataTable DataTableUpdateSessionList = CswTableUpdate.getTable( " where sessionlistid in (" + DeleteIds + ")" );

            //        foreach( DataRow CurrentRow in DataTableUpdateSessionList.Rows )
            //        {
            //            CurrentRow.Delete();

            //        }//iterate rows

            //        CswTableUpdate.update( DataTableUpdateSessionList );

            //    }//if we have stale sessionlist records
            //}
            //else
            //{
            //    ReturnVal = AuthenticationStatus.NonExistentSession;
            //}//if-else we have session records

            EuphemisticAuthenticationStatus = _CswAuthenticator.euphemizeAuthenticationStatus( ReturnVal ); 

            return ( ReturnVal );

        }//startSession()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EndSessionMode">esmCommit and esmRollback also release; esmRelease is only when there were no changes to data</param>
        public void endSession( EndSessionMode EndSessionMode )
        {
            if( EndSessionMode.esmCommit == EndSessionMode )
            {
                CswNbtResources.finalize();
            }
            else if( EndSessionMode.esmRollback == EndSessionMode )
            {
                CswNbtResources.Rollback();
            }

            CswNbtResources.release();

        }//endSession()




    }//CswNbtWebServiceResources

}//ChemSW.CswWebControls