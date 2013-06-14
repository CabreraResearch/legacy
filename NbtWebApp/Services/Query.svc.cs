using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Mail Report operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Query
    {
        private HttpContext _Context = HttpContext.Current;

        [DataContract]
        public class Tables : CswWebSvcReturn
        {
            public Tables()
            {
                Data = new CswNbtSdDbQueries.Tables();
            }

            [DataMember]
            public CswNbtSdDbQueries.Tables Data;
        }

        [DataContract]
        public class Columns: CswWebSvcReturn
        {
            public Columns()
            {
                Data = new CswNbtSdDbQueries.Columns();
            }

            [DataMember]
            public CswNbtSdDbQueries.Columns Data;
        }
        
        [OperationContract]
        [WebGet( UriTemplate = "tables" )]
        [Description( "Get the tables from which SQL queries can be composed" )]
        [FaultContract( typeof( FaultException ) )]
        public Tables getTables()
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            Tables Ret = new Tables();

            var SvcDriver = new CswWebSvcDriver<Tables, string>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : ( Resources, Response, Request ) => CswNbtSdDbQueries.getTables( Resources, Response.Data, Request ),
                ParamObj : ""
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebGet( UriTemplate = "columns?ObjectClassId={ObjectClassId}&NodeTypeId={NodeTypeId}" )]
        [Description( "Get the columns for a table from which SQL queries can be composed" )]
        [FaultContract( typeof( FaultException ) )]
        public Columns getColumns(Int32 ObjectClassId, Int32 NodeTypeId)
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            Columns Ret = new Columns();

            var SvcDriver = new CswWebSvcDriver<Columns, Int32>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr: delegate( ICswResources Resources, Columns Response, int Request )
                    {
                        if( Int32.MinValue != ObjectClassId )
                        {
                            CswNbtSdDbQueries.getOcColumns( Resources, Response.Data, ObjectClassId );
                        }
                        else
                        {
                            
                        }
                    },  
                ParamObj : NodeTypeId
                );

            SvcDriver.run();
            return ( Ret );
        }
        
    }
}
