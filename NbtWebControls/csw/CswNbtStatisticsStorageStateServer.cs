using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
//using System.Web.UI;

//using System.Data;
//using System.Configuration;
//using System.Web;
//using System.Web.Security;
using System.Web.UI;
//using System.Web.SessionState;
using ChemSW.Session;
/*
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
 */


namespace ChemSW.Nbt.Statistics
{
    public class CswNbtStatisticsStorageStateServer
    {


        //private string _StatisticsKey = "CswStatistics";
        public CswNbtStatisticsStorageStateServer(  )
        {
        }


        public CswNbtStatisticsEntry CswNbtStatisticsEntry
        {
            get
            {
                return ( null );
            }
        }

        #region ICswSessionStorage Members


        public void save( CswNbtStatisticsEntry CswNbtStatisticsEntry )
        {
            ;
        }

        public void remove()
        {
            ;
        }

        //private string _StatisticsKey = "CswStatistics";
        //private HttpSessionState _HttpSessionState = null;
        //public CswNbtStatisticsStorageStateServer( HttpSessionState HttpSessionState )
        //{
        //    _HttpSessionState = HttpSessionState;
        //}


        //public CswNbtStatisticsEntry CswNbtStatisticsEntry
        //{
        //    get
        //    {
        //        //Retrns NULL if the statistics key does not exist
        //        return ( _HttpSessionState[_StatisticsKey] as CswNbtStatisticsEntry );
        //    }
        //}

        //#region ICswSessionStorage Members


        //public void save( CswNbtStatisticsEntry CswNbtStatisticsEntry )
        //{
        //    //_HttpSessionState.Remove (_StatisticsKey );
        //    //_HttpSessionState.Add( _StatisticsKey, CswNbtStatisticsEntry );

        //    _HttpSessionState[_StatisticsKey] = CswNbtStatisticsEntry;
        //}

        //public void remove()
        //{
        //    if ( null != _HttpSessionState[_StatisticsKey] )
        //    {
        //        _HttpSessionState[_StatisticsKey] = null;
        //    }
        //}

        #endregion
    }//CswNbtStatisticsStorageStateServer

}//ChemSW.CswWebControls