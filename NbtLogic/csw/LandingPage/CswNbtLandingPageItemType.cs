using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.LandingPage
{
    /// <summary>
    /// Represents a LandingPage ItemType
    /// </summary>
    public sealed class CswNbtLandingPageItemType : IEquatable<CswNbtLandingPageItemType>, IComparable<CswNbtLandingPageItemType>
    {
        #region Internals
        private static readonly Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            { Text  , Text  },
            { Add   , Add   },
            { Link  , Link  },
            { Tab   , Tab   },
            { Button, Button}
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
        public CswNbtLandingPageItemType( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswNbtLandingPageItemType( string Val )
        {
            return new CswNbtLandingPageItemType( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswNbtLandingPageItemType item )
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

        /// <summary>
        /// Enum member 1
        /// </summary>
        public const string Text = "Text";
        public const string Add = "Add";
        public const string Link = "Link";
        public const string Tab = "Tab";
        public const string Button = "Button";

        #endregion Enum members


        #region IComparable

        public int CompareTo( CswNbtLandingPageItemType other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
}

        #endregion IComparable

        #region IEquatable (NbtObjectClass)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswNbtLandingPageItemType item1, CswNbtLandingPageItemType item2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( item1 ) == CswConvert.ToString( item2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswNbtLandingPageItemType item1, CswNbtLandingPageItemType item2 )
        {
            return !( item1 == item2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtLandingPageItemType ) )
            {
                return false;
            }
            return this == (CswNbtLandingPageItemType) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswNbtLandingPageItemType obj )
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

        #endregion IEquatable (NbtObjectClass)

    };
}
