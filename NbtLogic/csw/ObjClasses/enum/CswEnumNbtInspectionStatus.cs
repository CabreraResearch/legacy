using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtInspectionStatus : IEquatable<CswEnumNbtInspectionStatus>
    {
        #region Internals

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
                {Pending, Pending},
                {Overdue, Overdue},
                {ActionRequired, ActionRequired},
                {Missed, Missed},
                {Completed, Completed},
                {CompletedLate, CompletedLate},
                {Cancelled, Cancelled}
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
        public CswEnumNbtInspectionStatus( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtInspectionStatus( string Val )
        {
            return new CswEnumNbtInspectionStatus( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtInspectionStatus item )
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
        /// No action has been taken, not yet due
        /// </summary>
        public const string Pending = "Pending";

        /// <summary>
        /// No action has been taken, past due
        /// </summary>
        public const string Overdue = "Overdue";

        /// <summary>
        /// Inspection finished, some answers Deficient
        /// </summary>
        public const string ActionRequired = "Action Required";

        /// <summary>
        /// Inspection was never finished, past missed date
        /// </summary>
        public const string Missed = "Missed";

        /// <summary>
        /// Inspection complete, all answers OK
        /// </summary>
        public const string Completed = "Completed";

        /// <summary>
        /// Inspection completed late, all answers OK
        /// </summary>
        public const string CompletedLate = "Completed Late";

        /// <summary>
        /// Admin has cancelled the Inspection
        /// </summary>
        public const string Cancelled = "Cancelled";

        #endregion Enum members

        #region IEquatable (InspectionStatus)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtInspectionStatus ft1, CswEnumNbtInspectionStatus ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtInspectionStatus ft1, CswEnumNbtInspectionStatus ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtInspectionStatus ) )
            {
                return false;
            }
            return this == (CswEnumNbtInspectionStatus) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtInspectionStatus obj )
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

        #endregion IEquatable (InspectionStatus)

    }
} // namespace