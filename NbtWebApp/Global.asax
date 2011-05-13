<%@ Application Language="C#" %>
<%@ Assembly Name="CswCommon" %>
<%@ Assembly Name="NbtWebControls" %>
<%@ Import Namespace="ChemSW.Config" %>
<%@ Import Namespace="ChemSW.Session" %>
<%@ Import Namespace="ChemSW.Nbt" %>
<%@ Import Namespace="ChemSW.Nbt.Config" %>
<%@ Import Namespace="ChemSW.Core" %>
<%@ Import Namespace="ChemSW.Nbt.TreeEvents" %>
<%@ Import Namespace="ChemSW.Nbt.Statistics" %>
<%@ Import Namespace="ChemSW.Exceptions" %>

<script RunAt="server">

    void Application_Start( object sender, EventArgs e )
    {
        // Code that runs on application startup
    }

    void Application_End( object sender, EventArgs e )
    {
    }

    void Application_Error( object sender, EventArgs e )
    {
        // Code that runs when an unhandled error occurs
    }

    void Session_Start( object sender, EventArgs e )
    {
        // Code that runs when a new session is started
    }




    /// <summary>
    /// The firing of this event is dependent upon this setting in web.config
    ///      <sessionState mode="InProc" timeout="30"/>
    /// The "InProc" mode is the only one with which Session_End is fired
    /// The timeout setting indicates how long after user inactivity the 
    /// event will fire. 
    /// Note that timeout is a global setting for all sessions and that if 
    /// it is less than the role timeout for any particular user, that user's
    /// session will get summarily nuked by this event before it gets nuked 
    /// by the web app.
    /// See bz # 9278 for further details.
    /// 
    /// We don't use the standard session management and factory classes here because 
    /// they assume they can delete the inproc session record, whereas here the event 
    /// signals the record has already been deleted -- so we dont' want to mess with it. 
    /// As for polymorphisizing for our own DSM mechanism: if we do that then this 
    /// code is irrelevant anyway
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //void Session_End( object sender, EventArgs e )
    //{

    //    CswNbtResources CswNbtResources = null;
    //    string FilesPath = string.Empty;
    //    CswDbCfgInfoNbt CswDbCfgInfoNbt = null;
    //    CswSetupVblsNbt CswSetupVblsNbt = null;


    //    //we have to do this in two phases, because if the first phase doesn't work, 
    //    //we don't even have a known way to log the error
    //    //PHASE I : Establish logging resources
    //    try
    //    {
    //        //FilesPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc";
    //        CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.Web );
    //        CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode.Web );
    //        //CswNbtResources = new CswNbtResources( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt, true, false );
    //        CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt, CswTools.getConfigurationFilePath( SetupMode.Executable ), true, false );
    //    }
    //    catch( Exception Exception )
    //    {
    //        //This should show up in the event viewer
    //        throw ( new Exception( "ChemSW, Inc., NBT could not set set up resources in the global Session_End event: " + Exception.Message ) );
    //    }

    //    //PHASE II : Connect to schema and do the work
    //    try
    //    {
    //        CswNbtResources.SetDbResources( new CswNbtTreeFactory( FilesPath ) );

    //        string SessionListEntryKey = "CswSessionListEntry";
    //        CswSessionsListEntry CswSessionsListEntry = Session[SessionListEntryKey] as CswSessionsListEntry;


    //        //CswSessionsFactory CswSessionsListFactory = new CswSessionsFactory( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt, false, new CswSessionStorageStateServer( Session ) );
    //        //CswSessions CswSessions = CswSessionsListFactory.make( CswSessionsFactory.SessionsStorageType.DbStorage );


    //        //If the user hits any page, he'll have a session, but he won't necessarily have authenticated 
    //        CswSessionStorageDb CswSessionStorageDb = new CswSessionStorageDb( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt, false );
    //        if( null != CswSessionsListEntry )
    //        {
    //            if( CswSessionsListEntry.TimedOut ) //bz # 9940
    //            {

    //                CswSessionStorageDb.remove( CswSessionsListEntry.SessionId );

    //                CswNbtStatisticsStorageStateServer CswNbtStatisticsStorageStateServer = new CswNbtStatisticsStorageStateServer();
    //                if( null != CswNbtStatisticsStorageStateServer.CswNbtStatisticsEntry )
    //                {
    //                    CswNbtResources.AccessId = CswNbtStatisticsStorageStateServer.CswNbtStatisticsEntry.AccessId;
    //                    CswNbtStatisticsStorageDb CswNbtStatisticsStorageDb = new CswNbtStatisticsStorageDb( CswNbtResources );
    //                    CswNbtStatisticsStorageDb.save( CswNbtStatisticsStorageStateServer.CswNbtStatisticsEntry );
    //                    CswNbtResources.finalize();
    //                }

    //            }
    //            else
    //            {
    //                CswNbtResources.logError( new CswDniException( "Session_End was called for user " + CswSessionsListEntry.UserName + "'s session before the NBT role timeout: either a system event forced the InProc session to end, or the InProc timeout is less than the role timeout" ) );

    //            }//if else-sesson timed out

    //            //bz # 10001: purge expired sessions
    //            if( ( false == CswSetupVblsNbt.doesSettingExist( "PurgeExpiredSessionsOnEndSession" ) ) || ( "1" == CswSetupVblsNbt.readSetting( "PurgeExpiredSessionsOnEndSession" ) ) )
    //            {
    //                ArrayList SessionIdsToRemove = new ArrayList();
    //                SortedList<string, CswSessionsListEntry> CurrentSessions = CswSessionStorageDb.AllSessions;
    //                foreach( CswSessionsListEntry Entry in CurrentSessions.Values )
    //                {
    //                    if( false == Entry.IsMobile && Entry.TimeoutDate < DateTime.Now )
    //                    {
    //                        SessionIdsToRemove.Add( Entry.SessionId );
    //                    }
    //                }

    //                foreach( string SessionId in SessionIdsToRemove )
    //                {
    //                    CswSessionStorageDb.remove( SessionId );
    //                }

    //            }//if we're purging

    //        }

    //    }//try

    //    catch( Exception Excepton )
    //    {
    //        CswNbtResources.logError( Excepton );
    //    }//catch
    //}
	   
</script>
