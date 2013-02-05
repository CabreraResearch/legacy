using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{

    #region ItemType

    /// <summary>
    /// Template for new ItemType class
    /// </summary>
    public sealed class ItemType : IEquatable<ItemType>
    {
        #region Internals

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
                {View, View},
                {Category, Category},
                {Action, Action},
                {Report, Report},
                {Search, Search},
                {RecentView, RecentView},
                {Root, Root}
            };

        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public ItemType( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator ItemType( string Val )
        {
            return new ItemType( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( ItemType item )
        {
            return item.Value;
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #endregion Internals

        #region Enum members

        public const string View = "View";
        public const string Category = "Category";
        public const string Action = "Action";
        public const string Report = "Report";
        public const string Search = "Search";
        public const string RecentView = "RecentView";
        public const string Root = "Root";
        public const string Unknown = CswResources.UnknownEnum;

        #endregion Enum members

        #region IEquatable (ItemType)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( ItemType ft1, ItemType ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( ItemType ft1, ItemType ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is ItemType ) )
            {
                return false;
            }
            return this == (ItemType) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( ItemType obj )
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = ( ret * prime ) + Value.GetHashCode();
            ret = ( ret * prime ) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (ItemType)

    };

    #endregion ItemType


    #region Requests

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
                public Item( ItemType ItemType )
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

    #endregion Requests

}