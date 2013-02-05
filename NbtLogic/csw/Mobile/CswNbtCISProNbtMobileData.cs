using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ChemSW.Nbt.csw.Mobile
{
    public class CswNbtCISProNbtMobileData
    {

        [DataContract]
        public class LegacyMobileResponse
        {
            [DataMember]
            public string Error;

            [DataMember]
            public string FileContents;

            [DataMember]
            public string TreeData;

        }

        [DataContract]
        public class MobileResponse
        {
            [DataMember]
            public bool Success;
        }


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

                // Used when uploading data from CISPro/NBT CORE mobile
                [DataMember]
                public string username = string.Empty;

                // Used when uploading data from legacy mobile
                [DataMember]
                public string programname = string.Empty;

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