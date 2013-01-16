using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
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

            [DataContract]
            public class Ret
            {
                [DataMember( IsRequired = false )]
                public bool Succeeded { get; set; }

                [DataMember]
                public string RequestId
                {
                    get { return CswTools.IsPrimaryKey( CswRequestId ) ? CswRequestId.ToString() : ""; }
                    set
                    {
                        CswRequestId = new CswPrimaryKey();
                        CswRequestId.FromString( value );
                    }
                }

                [DataMember( IsRequired = false )]
                public string RequestName
                {
                    get { return ( null != CswRequestName ) ? CswRequestName.ToString() : ""; }
                    set { CswRequestName = new CswPropIdAttr( value ); }
                }

                [DataMember( IsRequired = false )]
                public Collection<CswNbtNode.Node> RequestItems = new Collection<CswNbtNode.Node>();

                [IgnoreDataMember]
                public CswPrimaryKey CswRequestId { get; set; }
                [IgnoreDataMember]
                public CswPropIdAttr CswRequestName { get; set; }
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
                Data = new CswNbtActRequesting.Cart();
            }

            [DataMember]
            public CswNbtActRequesting.Cart Data;



        }

    }
}