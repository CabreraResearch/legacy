using ChemSW.Core;
using System;
using System.Collections.Generic;

namespace ChemSW.Nbt
{
    public sealed class CswEnumNbtSessionDataType : IEquatable<CswEnumNbtSessionDataType>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                {
                                                                    { View, View },
                                                                    { Action, Action },
                                                                    { Search, Search }
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
        public CswEnumNbtSessionDataType(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtSessionDataType(string Val)
        {
            return new CswEnumNbtSessionDataType(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtSessionDataType item)
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
        /// Link to a View
        /// </summary>
        public const string View = "View";

        /// <summary>
        /// Link to a Search
        /// </summary>
        public const string Search = "Search";

        /// <summary>
        /// Link to an Action
        /// </summary>
        public const string Action = "Action";

        #endregion Enum members

        #region IEquatable (CswEnumNbtSessionDataType)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtSessionDataType ft1, CswEnumNbtSessionDataType ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtSessionDataType ft1, CswEnumNbtSessionDataType ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtSessionDataType))
            {
                return false;
            }
            return this == (CswEnumNbtSessionDataType)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtSessionDataType obj)
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

        #endregion IEquatable (CswEnumNbtSessionDataType)

    };

} // namespace ChemSW.Nbt
