using ChemSW.Core;
using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtMailReportFormatOptions : IEquatable<CswEnumNbtMailReportFormatOptions>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                {
                                                                    { Link, Link },
                                                                    { CSV, CSV }
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
        public CswEnumNbtMailReportFormatOptions(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtMailReportFormatOptions(string Val)
        {
            return new CswEnumNbtMailReportFormatOptions(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtMailReportFormatOptions item)
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

        public const string Link = "Link";
        public const string CSV = "CSV";

        #endregion Enum members

        #region IEquatable (CswEnumNbtMailReportFormatOptions)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtMailReportFormatOptions ft1, CswEnumNbtMailReportFormatOptions ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtMailReportFormatOptions ft1, CswEnumNbtMailReportFormatOptions ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtMailReportFormatOptions))
            {
                return false;
            }
            return this == (CswEnumNbtMailReportFormatOptions)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtMailReportFormatOptions obj)
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

        #endregion IEquatable (CswEnumNbtMailReportFormatOptions)

    };

}//namespace ChemSW.Nbt.ObjClasses
