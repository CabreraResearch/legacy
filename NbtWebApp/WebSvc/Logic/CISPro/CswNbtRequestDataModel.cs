using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.CISPro
{
    public class CswNbtRequestDataModel
    {
        /// <summary>
        /// Requesting Return Object
        /// </summary>
        [DataContract]
        public class CswNbtRequestMaterialCreateReturn : CswWebSvcReturn
        {
            /// <summary>
            /// Ctor
            /// </summary>
            public CswNbtRequestMaterialCreateReturn()
            {
                Data = new RequestType();
            }

            [DataMember]
            public RequestType Data;
        }

        /// <summary>
        /// Requesting Return Object
        /// </summary>
        [DataContract]
        public class CswNbtRequestMaterialDispenseReturn : CswWebSvcReturn
        {
            /// <summary>
            /// Ctor
            /// </summary>
            public CswNbtRequestMaterialDispenseReturn()
            {
                Data = new RequestResponse();
            }

            [DataMember]
            public RequestResponse Data;
        }

        // <summary>
        /// Represents a RequestCreateMaterial NodeTypeId
        /// </summary>
        public class RequestType
        {
            public Int32 NodeTypeId { get; set; }
        }

        public class RequestResponse
        {
            public bool Succeeded { get; set; }
        }

        public class RequestFulfill
        {
            public string RequestItemId { get; set; }
            public Collection<string> ContainerIds { get; set; }
        }

        /// <summary>
        /// Requesting Return Object
        /// </summary>
        [DataContract]
        public class RequestCart : CswWebSvcReturn
        {
            /// <summary>
            /// Ctor
            /// </summary>
            public RequestCart()
            {
                Data = new Ret();
            }

            [DataMember]
            public Ret Data;

            public class Ret
            {
                public string PendingItemsViewId = string.Empty;
                public string FavoriteItemsViewId = string.Empty;
                public string SubmittedItemsViewId = string.Empty;
            }

        }

    }
}