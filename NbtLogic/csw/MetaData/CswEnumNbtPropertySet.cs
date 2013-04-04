using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Nbt Object Class Name
    /// </summary>
    public sealed class CswEnumNbtPropertySet : IEquatable<CswEnumNbtPropertySet>, IComparable<CswEnumNbtPropertySet>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            { GeneratorTarget      , GeneratorTarget    },
            { InspectionParent     , InspectionParent   },
            { RequestItem          , RequestItem        },
            { Scheduler            , Scheduler          }
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
        public CswEnumNbtPropertySet( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtPropertySet( string Val )
        {
            return new CswEnumNbtPropertySet( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtPropertySet item )
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

        public const string GeneratorTarget = "GeneratorTarget";  
        public const string InspectionParent = "InspectionParent"; 
        public const string RequestItem = "RequestItem";      
        public const string Scheduler = "Scheduler";        

        #endregion Enum members


        #region IComparable

        public int CompareTo( CswEnumNbtPropertySet other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
        }

        #endregion IComparable

        #region IEquatable (CswEnumNbtPropertySet)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtPropertySet ft1, CswEnumNbtPropertySet ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtPropertySet ft1, CswEnumNbtPropertySet ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtPropertySet ) )
            {
                return false;
            }
            return this == (CswEnumNbtPropertySet) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtPropertySet obj )
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
