using ChemSW.Core;
using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.PropTypes
{
    public sealed class CswEnumNbtDateDisplayMode : IEquatable<CswEnumNbtDateDisplayMode>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                {
                                                                    { Date, Date },
                                                                    { DateTime, DateTime },
                                                                    { Time, Time }
                                                                };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse(string Val)
        {
            string ret = CswResources.UnknownEnum;
            if (_Enums.ContainsKey(Val))
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public CswEnumNbtDateDisplayMode(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtDateDisplayMode(string Val)
        {
            return new CswEnumNbtDateDisplayMode(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtDateDisplayMode item)
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
        /// display date only
        /// </summary>
        public const string Date = "Date";

        /// <summary>
        /// display time only
        /// </summary>
        public const string Time = "Time";

        /// <summary>
        /// display date and time
        /// </summary>
        public const string DateTime = "DateTime";

        #endregion Enum members

        #region IEquatable (CswEnumNbtDateDisplayMode)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtDateDisplayMode ft1, CswEnumNbtDateDisplayMode ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtDateDisplayMode ft1, CswEnumNbtDateDisplayMode ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtDateDisplayMode))
            {
                return false;
            }
            return this == (CswEnumNbtDateDisplayMode)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtDateDisplayMode obj)
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = (ret * prime) + Value.GetHashCode();
            ret = (ret * prime) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (CswEnumNbtDateDisplayMode)

    };

}//namespace ChemSW.Nbt.PropTypes
