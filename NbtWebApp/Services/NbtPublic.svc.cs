﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic;
using NbtWebApp.WebSvc.Logic.Labels;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class NbtPublic
    {
        /*
         * READ ME!
         * 
         * This svc is for web service methods that are available for external use.
         * The interface for these web service methods should not change.
         * 
         * NOTE: You may not use RESTful services here (WebGet) 
         * because they prevent the services from being used within the context of a Windows Forms application.
         * 
         */

        private HttpContext _Context = HttpContext.Current;

        #region Session

        /// <summary>
        /// Initiate a new session
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Initiate a new session" )]
        public CswNbtWebServiceSession.CswNbtAuthReturn SessionInit( CswWebSvcSessionAuthenticateData.Authentication.Request Request )
        {
            Request.Parameters = Request.Parameters ?? new CswWebSvcSessionAuthenticateData.Authentication.Parameters();
            Request.Parameters.IsIncludedInLoginData = false;
            Session Session = new Session();
            return Session.Init( Request );
        }

        /// <summary>
        /// Terminate the current session
        /// </summary>
        [OperationContract]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Terminate the current session" )]
        public void SessionEnd()
        {
            Session Session = new Session();
            Session.End();
            //CswWebSvcResourceInitializerNbt Resource = new CswWebSvcResourceInitializerNbt( _Context, null );
            //Resource.initResources();
            //Resource.deauthenticate();
            //Resource.deInitResources();
        }

        #endregion Session

        #region Print Labels

        /// <summary>
        /// Register a label printer
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Register a label printer" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelPrinterReg LpcRegister( LabelPrinter Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelPrinterReg Ret = new CswNbtLabelPrinterReg();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelPrinterReg, LabelPrinter>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.registerLpc,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// Retrieve the next job for a label printer
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Retrieve the next job for a label printer" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelJobResponse LpcGetNextJob( CswNbtLabelJobRequest Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelJobResponse Ret = new CswNbtLabelJobResponse();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelJobResponse, CswNbtLabelJobRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.nextLabelJob,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// Retrieve a particular label for a label printer
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Retrieve a particular label for a label printer" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelEpl LpcGetLabel( NbtPrintLabel.Request.Get Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelEpl Ret = new CswNbtLabelEpl();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelEpl, NbtPrintLabel.Request.Get>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.getEPLText,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// Update the state of a label printing job
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Update the state of a label printing job" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelJobUpdateResponse LpcUpdateJob( CswNbtLabelJobUpdateRequest Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelJobUpdateResponse Ret = new CswNbtLabelJobUpdateResponse();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelJobUpdateResponse, CswNbtLabelJobUpdateRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.updateLabelJob,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        #endregion Print Labels

        #region Balances


        /// <summary>
        /// Register a serial balance
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Register a serial balance" )]
        [FaultContract( typeof( FaultException ) )]
        public void UpdateBalanceData( SerialBalance Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtBalanceReturn Ret = new CswNbtBalanceReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtBalanceReturn, SerialBalance>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSerialBalance.UpdateBalanceData,
                ParamObj: Request
                );

            SvcDriver.run();
        }


        /// <summary>
        /// fetch existing balance configurations in NBT
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "fetch existing balance configurations in NBT" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtBalanceReturn ListBalanceConfigurations()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtBalanceReturn Ret = new CswNbtBalanceReturn();
            object Request = null;
            var SvcDriver = new CswWebSvcDriver<CswNbtBalanceReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSerialBalance.listBalanceConfigurations,
                ParamObj: Request
                );

            SvcDriver.run();

            return Ret;
        }



        /// <summary>
        /// update or create a new balance configuration on the server
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "update or create a new balance configuration on the server" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtBalanceReturn registerBalanceConfiguration( BalanceConfiguration Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtBalanceReturn Ret = new CswNbtBalanceReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtBalanceReturn, BalanceConfiguration>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSerialBalance.registerBalanceConfiguration,
                ParamObj: Request
                );

            SvcDriver.run();

            return Ret;
        }



        #endregion
    }
}
