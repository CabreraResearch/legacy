using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewSelect
    {

        /// <summary>
        /// Structure for requesting a View Select
        /// </summary>
        [DataContract]
        public class Request
        {
            /// <summary>
            /// If <c>true</c>, constrain return object to searchable views
            /// </summary>
            [DataMember]
            public bool IsSearchable { get; set; }

            /// <summary>
            /// If <c>true</c>, include recently selected views
            /// </summary>
            [DataMember]
            public bool IncludeRecent { get; set; }

            /// <summary>
            /// If <c>true</c>, only include recently selected views
            /// </summary>
            [DataMember]
            public bool LimitToRecent { get; set; }

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
                categories = new Collection<Category>();
            }

            /// <summary>
            /// Base collection of Categories containing Items
            /// </summary>
            [DataMember]
            public Collection<Category> categories;

            [DataContract]
            public class Category
            {
                public Category( string Name )
                {
                    category = Name;
                    items = new Collection<Item>();
                }

                [DataMember]
                public string category = "";

                [DataMember]
                public Collection<Item> items;
            }

            [DataContract]
            public class Item
            {
                public Item( CswEnumNbtViewItemType ItemType )
                {
                    if( ItemType != ChemSW.CswResources.UnknownEnum )
                    {
                        type = ItemType;
                    }
                }

                [DataMember( EmitDefaultValue = true, IsRequired = true )]
                public string itemid = "";

                [DataMember( EmitDefaultValue = true, IsRequired = true )]
                public string name = "";

                [DataMember( EmitDefaultValue = true, IsRequired = true )]
                public readonly string type = "";

                [DataMember( EmitDefaultValue = false, IsRequired = true )]
                public string url = "";

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public string iconurl = "";

                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public string mode = "";
            }
        }
    }

    [DataContract]
    public class View
    {
        public CswNbtSessionDataId SessionViewId = null;
        public CswNbtViewId ViewId = null;

        [DataMember( IsRequired = true, EmitDefaultValue = true, Name = "ViewId" )]
        public string NbtViewId
        {
            get
            {
                string Ret = string.Empty;
                if( null != ViewId && ViewId.isSet() )
                {
                    Ret = ViewId.ToString();
                    SessionViewId = null;
                }
                else if( null != SessionViewId && SessionViewId.isSet() )
                {
                    Ret = SessionViewId.ToString();
                    ViewId = null;
                }
                return Ret;
            }
            set
            {
                string ViewIdString = value;
                if( CswNbtViewId.isViewIdString( ViewIdString ) )
                {
                    ViewId = new CswNbtViewId( ViewIdString );
                    SessionViewId = null;
                }
                else if( CswNbtSessionDataId.isSessionDataIdString( ViewIdString ) )
                {
                    SessionViewId = new CswNbtSessionDataId( ViewIdString );
                    ViewId = null;
                }
            }
        }
    }
}