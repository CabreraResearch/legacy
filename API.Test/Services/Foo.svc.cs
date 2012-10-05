using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace API.Test.Services
{
    [ServiceContract( Namespace = "API.Test.Services" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Foo
    {
        public class FooData
        {
            public Int32 Id = 1;
            public string Name = "Foo";
            public bool IsFoo = true;
            public DateTime Time = DateTime.Now;
        }

        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public FooData getFoo()
        {
            // Add your operation implementation here
            return new FooData();
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
