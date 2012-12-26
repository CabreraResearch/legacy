using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace NbtWebApp.WebSvc.Logic.CoreMobile
{
    public class CswNbtCoreMobileData
    {

        [DataContract]
        public class MobileRequest
        {
            public MobileRequest()
            {
                data = new Data();
            }

            [DataMember]
            public string apptype { get; set; }

            [DataMember]
            public Data data = null;

            [DataContract]
            public class Data
            {
                public Data()
                {
                    MultiOpRows = new Collection<Operation>();
                }

                [DataMember]
                public string username = string.Empty;

                [DataMember]
                public Collection<Operation> MultiOpRows;
            }//Data()

            [DataContract]
            public class Operation
            {
                public Operation()
                {
                    update = new Update();
                }

                [DataMember]
                public string op { get; set; }

                [DataMember]
                public string barcode = string.Empty;

                [DataMember]
                public Update update;
            }//Operation()

            [DataContract]
            public class Update
            {
                [DataMember]
                public string location = string.Empty;

                [DataMember]
                public string user = string.Empty;

                [DataMember]
                public string qty = string.Empty;

                [DataMember]
                public string uom = string.Empty;

            }//Update()

        }//MobileRequest()

    }//CswNbtWcfMobileDataModel()

}//NbtWebAppServices.Response