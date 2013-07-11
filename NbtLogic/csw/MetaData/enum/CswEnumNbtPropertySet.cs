using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Nbt Object Class Name
    /// </summary>
    public sealed class CswEnumNbtPropertySetName : IEquatable<CswEnumNbtPropertySetName>, IComparable<CswEnumNbtPropertySetName>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            { GeneratorTargetSet      , GeneratorTargetSet    },
            { InspectionParentSet     , InspectionParentSet   },
            { RequestItemSet          , RequestItemSet        },
            { SchedulerSet            , SchedulerSet          },
            { MaterialSet             , MaterialSet           },
            { DocumentSet             , DocumentSet           },
            { PermissionSet           , PermissionSet         }
        };

        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        public static IEnumerable<string> All
        {
            get { return _Enums.Values; }
        }

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
        public CswEnumNbtPropertySetName( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtPropertySetName( string Val )
        {
            return new CswEnumNbtPropertySetName( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtPropertySetName item )
        {
            return ( null != item ) ? item.Value : CswNbtResources.UnknownEnum;
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

        public const string GeneratorTargetSet = "GeneratorTargetSet";
        public const string InspectionParentSet = "InspectionParentSet";
        public const string RequestItemSet = "RequestItemSet";
        public const string SchedulerSet = "SchedulerSet";
        public const string MaterialSet = "MaterialSet";
        public const string DocumentSet = "DocumentSet";
        public const string PermissionSet = "PermissionSet";

        #endregion Enum members


        #region IComparable

        public int CompareTo( CswEnumNbtPropertySetName other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
        }

        #endregion IComparable

        #region IEquatable (CswEnumNbtPropertySet)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtPropertySetName ft1, CswEnumNbtPropertySetName ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtPropertySetName ft1, CswEnumNbtPropertySetName ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtPropertySetName ) )
            {
                return false;
            }
            return this == (CswEnumNbtPropertySetName) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtPropertySetName obj )
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

        #endregion IEquatable (CswEnumNbtPropertySet)

    };
}
