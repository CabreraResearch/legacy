using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Web.SessionState;   // This is why this class is in the web controls package
using System.Web;
using ChemSW.Exceptions;
using ChemSW.Session;
using ChemSW.Nbt;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Security;
using ChemSW.Config;

namespace ChemSW.Nbt.Security
{
    /// <summary>
    /// Implements the ICswSession interface. 
    /// Note that some of the properties go directly to HttpSessionState whilst others 
    /// use the session list. The properties that use the session list have to check to 
    /// make sure that there exists an entry for the session list and, if not, to use local
    /// variables instead. This approach is necessary to support the flow of operations
    /// in NBT -- the role timeout , for example, is initially set before a user list entry
    /// has been established. Notice also that, with respect to the session list, all that 
    /// gets stored is the hashtable at the core of the list. It's possible that it would 
    /// work to store CswSessionList but I suspect that it would have to implement a serializeable
    /// interface in order for that to work. 
    /// </summary>
    public class CswNbtSession : ICswSession
    {
        private HttpSessionState _HttpSessionState;
        private HttpApplicationState _HttpApplicationState;
        private HttpRequest _HttpRequest;
        private HttpResponse _HttpResponse;
        private CswWebSession _CswWebSession = null;



        public CswNbtSession( HttpApplicationState HttpApplicationState, HttpSessionState HttpSessionState, HttpRequest HttpRequest, HttpResponse HttpResponse )
        {
            _HttpApplicationState = HttpApplicationState;
            _HttpSessionState = HttpSessionState;
            _HttpRequest = HttpRequest;
            _HttpResponse = HttpResponse;

        } //ctor


        public void configure( ICswSetupVbls inCswSetupVbls, ICswDbCfgInfo inCswDbCfgInfo, bool IsDeleteModeLogical )
        {
            _CswWebSession = new CswWebSession( _HttpApplicationState, _HttpSessionState, _HttpRequest, _HttpResponse, AppType.Nbt, inCswSetupVbls, inCswDbCfgInfo, IsDeleteModeLogical );
        }//configure



        //public string SessionCookieName
        //{
        //    get
        //    {
        //        return ( _CswWebSession.SessionCookieName );
        //    }
        //}


        //From iface
        private ICswResources _CswResources;
        public ICswResources CswResources
        {

            set
            {
                _CswResources = value;
                //_CswWebSession.CswResources = _CswResources;
            }
        }



        //*** SET SESSION ID BY EITHER MAKING A NEW SESSION OR LOADING AN EXISGING ONE
        public string makeNew()
        {
            return ( _CswWebSession.makeNew() );
        }//


        public bool loadExisting( string SessionId )
        {
            return ( _CswWebSession.loadExisting( SessionId ) );
        }


        //public bool load( string SessionId )
        //{
        //    return ( _CswWebSession.load( SessionId ) );
        //}//load() 


        public string SessionId
        {
            get
            {
                return ( _CswWebSession.SessionId );
            }
        }




        //private string _AccessId = string.Empty;
        public string AccessId
        {
            set
            {
                //_AccessId = value;
                _CswWebSession.AccessId = value;
            }

            get
            {
                return ( _CswWebSession.AccessId );
            }
        }

        //private string _UserName = string.Empty;
        public string UserName
        {
            set
            {
                _CswWebSession.UserName = value;
            }

            get
            {
                return ( _CswWebSession.UserName );
            }
        }//UserName


        public CswSessionsList SessionsList
        {
            get
            {
                return ( _CswWebSession.SessionsList );
            }

        }//SessionsList

        public void purgeExpiredSessions()
        {
            _CswWebSession.purgeExpiredSessions();

        }//purgeExpiredSessions()



        public string IPAddress
        {
            set
            {
                _CswWebSession.IPAddress = value;

            }
            get
            {
                return ( _CswWebSession.IPAddress );
            }
        } // IPAddress

        public CswPrimaryKey UserId
        {
            set
            {
                _CswWebSession.UserId = value;
            }
            get
            {

                return ( _CswWebSession.UserId );
            }
        } // UserId


        private Double _RoleTimeout = Double.NaN;
        public Double RoleTimeout
        {
            set
            {
                _CswWebSession.RoleTimeout = value;
            }
            get
            {
                return ( _CswWebSession.RoleTimeout );
            }
        }//RoleTimeout

        public DateTime LoginDate
        {
            get
            {
                return ( _CswWebSession.LoginDate );
            }

        }//LoginDate


        public Int32 SessionCount
        {
            get
            {
                return ( _CswWebSession.SessionCount );
            }//get

        }//SessionCount 

        public void save()
        {
            _CswWebSession.save();
        }//save()

        public void clear()
        {
            SaveStatistics( SessionId );
            _CswWebSession.clear();
            //_HttpSessionState.RemoveAll();

        }//clear()

        public bool InSessionList
        {
            get
            {
                return ( _CswWebSession.InSessionList );
            }//get
        }//InSessionList

        public bool TimedOut
        {
            get
            {
                return ( _CswWebSession.TimedOut );
            }//
        }//TimedOut

        public void updateLastAccess()
        {
            _CswWebSession.updateLastAccess();
        }//updateLastAccess()

        public Int32 getSessionCount( string AccessId )
        {

            return ( _CswWebSession.getSessionCount( AccessId ) );
        }//getSessionCount()


        public CswSessionsListEntry SessionsListEntry
        {
            get { return ( _CswWebSession.SessionsListEntry ); }
        }

        //#endregion Cached CswNbtMetaData

        // ------------------------------------------------------------------
        // Statistics Gathering

        // Events

        public void OnError( Exception ex )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_errors++;
            }
        }

        //*************************************************************
        //*************** NBT specific
        private CswNbtResources _CswNbtResources;
        public CswNbtResources CswNbtResources
        {
            get { return _CswNbtResources; }
            set
            {
                _CswNbtResources = value;
                CswResources = value.CswResources;
            }
        }


        public void OnAfterLogin( ICswSession Me )
        {

            _CswWebSession.OnAfterLogin( Me );
            //HttpCookie SessionCookie = _HttpRequest.Cookies.Get( SessionCookieName );
            //if ( null == SessionCookie )
            //{
            //    SessionCookie = new HttpCookie( SessionCookieName );
            //    _HttpResponse.Cookies.Add( SessionCookie );
            //}

            //SessionCookie.Value = SessionId;
        }

        public void OnAddNode( CswNbtNode Node )
        {
            if ( Node != null )
            {
                SessionsList.Statistics[SessionId].Stats_count_nodesadded++;
                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].NodeTypesAdded, Node.NodeTypeId.ToString() );
            }
        }
        public void OnCopyNode( CswNbtNode OldNode, CswNbtNode NewNode )
        {
            if ( NewNode != null )
            {
                SessionsList.Statistics[SessionId].Stats_count_nodescopied++;
                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].NodeTypesCopied, NewNode.NodeTypeId.ToString() );
            }
        }
        public void OnWriteNode( CswNbtNode Node, bool ForceSave )
        {
            if ( Node != null )
            {
                SessionsList.Statistics[SessionId].Stats_count_nodessaved++;
                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].NodeTypesSaved, Node.NodeTypeId.ToString() );
            }
        }
        public void OnDeleteNode( CswNbtNode Node )
        {
            if ( Node != null )
            {
                SessionsList.Statistics[SessionId].Stats_count_nodesdeleted++;
                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].NodeTypesDeleted, Node.NodeTypeId.ToString() );
            }
        }
        public void OnLoadView( CswNbtView View )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_count_viewloads++;
                if ( View.ViewId > 0 )
                    SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].ViewsLoaded, View.ViewId.ToString() );
            }
        }
        public void OnLoadSearch( CswNbtViewProperty ViewProp )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_count_searches++;
                if ( ViewProp.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                    SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].NodeTypePropsSearched, ViewProp.NodeTypePropId.ToString() );
                else
                    SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].ObjectClassPropsSearched, ViewProp.ObjectClassPropId.ToString() );
            }
        }
        public void OnModifyViewFilters( CswNbtView OldView, CswNbtView NewView )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_count_viewfiltermod++;
                foreach ( CswNbtViewPropertyFilter OldFilter in OldView.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewPropertyFilter ) )
                {
                    foreach ( CswNbtViewPropertyFilter NewFilter in NewView.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewPropertyFilter ) )
                    {
                        if ( OldFilter.ArbitraryId == NewFilter.ArbitraryId &&
                             OldFilter.Value != NewFilter.Value )
                        {
                            CswNbtViewProperty ParentProp = ( CswNbtViewProperty )NewFilter.Parent;
                            if ( ParentProp.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId )
                                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].NodeTypePropsFilterMod, ParentProp.NodeTypePropId.ToString() );
                            else
                                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].ObjectClassPropsFilterMod, ParentProp.ObjectClassPropId.ToString() );
                        }
                    }
                }
            }
        }


        public void OnLoadReport( CswPrimaryKey ReportId )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_count_reportruns++;
                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].ReportsLoaded, ReportId );
            }
        }

        public void OnLoadAction( Int32 ActionId )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_count_actionloads++;
                SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].ActionsLoaded, ActionId.ToString() );
            }
        }

        public void OnMultiModeEnabled( CswNbtView View )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_count_multiedit++;
                if ( View.ViewId > 0 )
                    SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].ViewsMultiEdited, View.ViewId.ToString() );
            }
        }

        public void OnFinishEditingView( CswNbtView View )
        {
            if ( InSessionList )
            {
                SessionsList.Statistics[SessionId].Stats_count_viewsedited++;
                if ( View.ViewId > 0 )
                    SessionsList.Statistics[SessionId].IncrementHash( SessionsList.Statistics[SessionId].ViewsEdited, View.ViewId.ToString() );
            }
        }

        public void OnEndOfPageLifeCycle( CswTimer Timer )
        {
            if ( InSessionList )
            {
                // Store numbers to determine average page lifecycle
                double ElapsedTime = Timer.ElapsedDurationInMilliseconds;
                SessionsList.Statistics[SessionId].Stats_servertime_total += ElapsedTime;
                SessionsList.Statistics[SessionId].Stats_servertime_count++;
            }
        }

        public void OnBeforeLogout( ICswSession Me )
        {
            // we don't do this here, since clear() will do this after
            //SaveStatistics(this.SessionId);
            SessionsList.Statistics[SessionId].Stats_LoggedOut = true;
        }

        /// <summary>
        /// Writes statistics data for the given SessionId 
        /// (which is not necessarily the same as this object's SessionId)
        /// </summary>
        /// <param name="SessionId"></param>
        public void SaveStatistics( string SessionId )
        {
            SessionsList.SaveStatistics( SessionId );
        }

        public void finalize()
        {
            //if( "1" == _CswResources.SetupVbls["cachemetadata"] )
            //{
            //    if( _CswResources.MetaData != null && _CswResources.IsInitializedForDbAccess )
            //    {
            //        CachedMetaData_DateCached = DateTime.Now;
            //        CachedMetaData_AccessId = _CswResources.AccessId;
            //    }
            //}
        }//finalize()

        public void SetCache()
        {
            if ( "1" == CswNbtResources.SetupVbls["cachemetadata"] )
            {
                if ( CswNbtResources != null )
                {
                    //Session[ "ViewCache" ] = CswNbtResources.ViewCache.ToString();
                    CswNbtResources.BeforeStoreInCache();
                    _HttpSessionState["CswNbtResources"] = CswNbtResources;
                }
            }
            else
            {
                _HttpSessionState["CswNbtResources"] = null;
            }
        }

        public ICswResources CachedResources
        {
            get
            {
                ICswResources ret = null;
                if ( _HttpSessionState["CswNbtResources"] != null )
                    ret = _HttpSessionState["CswNbtResources"] as ICswResources;
                return ret;
            }
        }
    }//CswNbtSession


}//ChemSW.Core
