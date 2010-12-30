using System;
using System.Collections;
using ChemSW.Core;

namespace ChemSW.Nbt.Statistics
{
    public class CswNbtStatisticsEntry
    {

        public bool StatisticsDataWasLost = false;

        public Int32 Stats_count_actionloads;
        public Int32 Stats_count_multiedit;
        public Int32 Stats_count_reportruns;
        public Int32 Stats_count_viewloads;
        public Int32 Stats_count_viewsedited;
        public Int32 Stats_count_nodessaved;
        public Int32 Stats_count_nodesadded;
        public Int32 Stats_count_nodescopied;
        public Int32 Stats_count_nodesdeleted;
        public Int32 Stats_errors;
        public Int32 Stats_count_searches;
        public Int32 Stats_count_viewfiltermod;

        public Int32 Stats_servertime_count;
        public Double Stats_servertime_total;
        public bool Stats_LoggedOut;

        public Hashtable ActionsLoaded;
        public Hashtable ViewsLoaded;
        public Hashtable ViewsMultiEdited;
        public Hashtable ViewsEdited;
        public Hashtable NodeTypesSaved;
        public Hashtable NodeTypesCopied;
        public Hashtable NodeTypesDeleted;
        public Hashtable NodeTypesAdded;
        public Hashtable ReportsLoaded;
        public Hashtable NodeTypePropsSearched;
        public Hashtable ObjectClassPropsSearched;
        public Hashtable NodeTypePropsFilterMod;
        public Hashtable ObjectClassPropsFilterMod;

        public string SessionId = string.Empty;
        public CswPrimaryKey UserId = null;
        public string UserName = string.Empty;
        public DateTime LoginDate = DateTime.Now;
        public Double RoleTimeoutMinutes = Double.NaN;
        public DateTime TimeoutDate;
        public string IPAddress = string.Empty;
        public string AccessId = string.Empty;

        //public CswStatisticsListEntry( string InSessionId, string InIPAddress, string InAccessId, CswPrimaryKey InUserId, string InUsername, double InRoleTimeoutMinutes )
        public CswNbtStatisticsEntry( string InSessionId )
        {
            SessionId = InSessionId;
            //SessionId = InSessionId;
            //AccessId = InAccessId;
            //UserName = InUsername;
            //UserId = InUserId;
            //RoleTimeoutMinutes = InRoleTimeoutMinutes;
            //if( Double.IsNaN( RoleTimeoutMinutes ) )
            //    TimeoutDate = DateTime.MinValue;
            //else
            //    TimeoutDate = LoginDate.AddMinutes( RoleTimeoutMinutes );
            //IPAddress = InIPAddress;

            ClearStatistics();
        }

        public void IncrementHash( Hashtable HT, object Key )
        {
            if ( HT.Contains( Key ) )
                HT[ Key ] = CswConvert.ToInt32( HT[ Key ] ) + 1;
            else
                HT[ Key ] = 1;
        }

        public void ClearStatistics()
        {
            Stats_count_actionloads = 0;
            Stats_count_multiedit = 0;
            Stats_count_reportruns = 0;
            Stats_count_viewloads = 0;
            Stats_count_viewsedited = 0;
            Stats_count_nodessaved = 0;
            Stats_count_nodesadded = 0;
            Stats_count_nodescopied = 0;
            Stats_count_nodesdeleted = 0;
            Stats_servertime_count = 0;
            Stats_servertime_total = 0;
            Stats_errors = 0;
            Stats_count_searches = 0;
            Stats_count_viewfiltermod = 0;
            Stats_LoggedOut = false;

            ViewsEdited = new Hashtable();
            ViewsLoaded = new Hashtable();
            ViewsMultiEdited = new Hashtable();
            NodeTypesSaved = new Hashtable();
            NodeTypesCopied = new Hashtable();
            NodeTypesDeleted = new Hashtable();
            NodeTypesAdded = new Hashtable();
            ReportsLoaded = new Hashtable();
            ActionsLoaded = new Hashtable();
            NodeTypePropsSearched = new Hashtable();
            ObjectClassPropsSearched = new Hashtable();
            NodeTypePropsFilterMod = new Hashtable();
            ObjectClassPropsFilterMod = new Hashtable();
        }

    }//class CswSessionListEntry

}//ChemSW.Session