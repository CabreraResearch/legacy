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

namespace ChemSW.Nbt
{
    public enum EndSessionMode { esmCommit, esmRollback, esmRelease };
    public class CswNbtWebServiceResources
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtResources CswNbtResources { get { return ( _CswNbtResources ); } }

        private CswSessionResourcesNbt _CswInitialization;
        public CswSessionResourcesNbt CswInitialization { get { return ( _CswInitialization ); } }

        private CswSessionManager _CswSessionManager;
        public CswSessionManager CswSessionManager { get { return ( _CswSessionManager ); } }

        public CswNbtWebServiceResources( HttpApplicationState HttpApplicationState, HttpSessionState HttpSessionState, HttpRequest HttpRequest, HttpResponse HttpResponse, string LoginAccessId, string FilesPath, SetupMode SetupMode )
        {
            _CswInitialization = new CswSessionResourcesNbt( HttpApplicationState, HttpSessionState, HttpRequest, HttpResponse, string.Empty, FilesPath, SetupMode );
            _CswNbtResources = _CswInitialization.CswNbtResources;
            _CswSessionManager = _CswInitialization.CswSessionManager;
        }//ctor

        public void startSession()
        {
            ;//do nothing for now, but may want this hook someday
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
            CswSessionManager.release();

        }//endSession()




    }//CswNbtWebServiceResources

}//ChemSW.CswWebControls