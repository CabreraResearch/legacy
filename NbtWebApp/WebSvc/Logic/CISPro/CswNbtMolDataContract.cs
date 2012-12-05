/// <summary>
    /// Generic Return Object, which inherits from CswWebSvcReturn
    /// </summary>
    [DataContract]
    public class CswNbtGenericReturn : CswWebSvcReturn
    {
        public CswNbtGenericReturn()
        {
            Data = new GenericReturn.Response();
        }
        [DataMember]
        public GenericReturn.Response Data;
    }


    /// <summary>
    /// Generic Data Contract with Request and Response classes
    /// </summary>
    public class GenericContract
    {

        /// <summary>
        /// Structure for requesting a View Select
        /// </summary>
        [DataContract]
        public class Request
        {
            /// <summary>
            /// If <c>true</c>, instance is enabled
            /// </summary>
            [DataMember]
            public bool Enabled { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataContract]
        public class Response
        {

            /// <summary>
            /// Data contract constructor
            /// </summary>
            public Response()
            {
                Members = new Collection<Member>();
            }

            /// <summary>
            /// Base collection of Members
            /// </summary>
            [DataMember]
            public Collection<Member> Members;

            [DataContract]
            public class Member
            {
                [DataMember]
                public string Name = "";
            }

            
        }
    }