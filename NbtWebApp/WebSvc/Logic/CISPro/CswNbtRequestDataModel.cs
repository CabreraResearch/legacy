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
                Data = new Ret();
            }

            [DataMember]
            public Ret Data;

            public class Ret
            {
                public Int32 NodeTypeId { get; set; }
            }

        }

        /// <summary>
        /// Requesting Return Object
        /// </summary>
        [DataContract]
        public class CswRequestReturn : CswWebSvcReturn
        {
            /// <summary>
            /// Ctor
            /// </summary>
            public CswRequestReturn()
            {
                Data = new Ret();
            }

            [DataMember]
            public Ret Data;

            public class Ret
            {
                public bool Succeeded { get; set; }
            }
        }

        // <summary>
        /// Represents a RequestCreateMaterial NodeTypeId
        /// </summary>
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