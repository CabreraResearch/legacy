﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.Security;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Scheduler;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Scheduler
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary> 
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "get" )]
        [Description( "Get all scheduled rules as a Grid" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtScheduledRulesReturn getScheduledRulesGrid( string AccessId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtScheduledRulesReturn Ret = new CswNbtScheduledRulesReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtScheduledRulesReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNbtManager.getScheduledRulesGrid,
                ParamObj: AccessId
                );

            SvcDriver.run();

            return ( Ret );

        }//getScheduledRulesGrid

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "save" )]
        [Description( "Save changes to scheduled rules" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtScheduledRulesReturn updateAllScheduledRules( CswNbtScheduledRulesReturn.Ret Request  )
        {
            //delegate has to be static because you can't creat e an instance yet: you don't have resources until the delegate is actually called
            CswNbtScheduledRulesReturn Ret = new CswNbtScheduledRulesReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtScheduledRulesReturn, CswNbtScheduledRulesReturn.Ret>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNbtManager.updateAllScheduledRules,
                ParamObj: Request
                );

            SvcDriver.run();

            return ( Ret );

        }//updateAllScheduledRules

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "getTimeline" )]
        [Description( "Get a timeline of scheduled rules" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtSchedServiceTimeLineReturn getTimeline( CswNbtSchedServiceTimeLineRequest Request )
        {
            CswNbtSchedServiceTimeLineReturn Ret = new CswNbtSchedServiceTimeLineReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtSchedServiceTimeLineReturn, CswNbtSchedServiceTimeLineRequest>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceNbtManager.getTimelines,
                ParamObj : Request
                );

            SvcDriver.run();

            return ( Ret );

        }//getTimeline

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "getTimelineFilters" )]
        [Description( "Get the filter data for a timeline of scheduled rules" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtSchedServiceTimeLineReturn getTimelineFilters( string FileName )
        {
            CswNbtSchedServiceTimeLineReturn Ret = new CswNbtSchedServiceTimeLineReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtSchedServiceTimeLineReturn, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceNbtManager.getTimelineFilters,
                ParamObj : FileName
                );

            SvcDriver.run();

            return ( Ret );

        }//getTimeline

        [OperationContract]
        [WebGet()]
        [Description( "Get staus of Schedule Service rules" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtScheduledRuleStatusReturn getScheduledRuleStatus()
        {
            CswNbtScheduledRuleStatusReturn Ret = new CswNbtScheduledRuleStatusReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtScheduledRuleStatusReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNbtManager.getScheduledRuleStatus,
                ParamObj: null
                );

            //This Webservice call does not require authentication
            SvcDriver.AuthenticationStatus = CswEnumAuthenticationStatus.Ignore;

            SvcDriver.run();
            return ( Ret );
        }

        //If you need to test CswDictionary, this'll help
            //[OperationContract]
            //[WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json)]
            //[FaultContract( typeof( FaultException ) )]
            //public CswDictionary dictionaryTest( CswDictionary Dict   )
            //{
            //    CswDictionary Ret = new CswDictionary();
            //    foreach( KeyValuePair<string, dynamic> Pair in Dict )
            //    {
            //        Ret.Add(Pair.Key, Pair.Value);
            //    }
            //    Ret.Add("Success", "True");

            //    return ( Ret );

            //}
        

    }//Scheduler 
}
