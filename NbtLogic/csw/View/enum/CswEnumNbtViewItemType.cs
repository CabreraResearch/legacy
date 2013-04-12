using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{

    /// <summary>
    /// View ItemType
    /// </summary>
    public sealed class CswEnumNbtViewItemType : IEquatable<CswEnumNbtViewItemType>
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
        public CswEnumNbtViewItemType( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtViewItemType( string Val )
        {
            return new CswEnumNbtViewItemType( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtViewItemType item )
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
        public static bool operator ==( CswEnumNbtViewItemType ft1, CswEnumNbtViewItemType ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtViewItemType ft1, CswEnumNbtViewItemType ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtViewItemType ) )
            {
                return false;
            }
            return this == (CswEnumNbtViewItemType) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtViewItemType obj )
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

    }
} // namespace