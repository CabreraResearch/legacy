using ChemSW.Core;
using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtPropertySelectMode : IEquatable<CswEnumNbtPropertySelectMode>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                {
                                                                    { Single, Single },
                                                                    { Multiple, Multiple },
                                                                    { Blank, Blank }
                                                                };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// All values
        /// </summary>
        public static IEnumerable<string> _All { get { return _Enums.Values; } }

        private static string _Parse( string Val )
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
        public CswEnumNbtPropertySelectMode(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtPropertySelectMode(string Val)
        {
            return new CswEnumNbtPropertySelectMode(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtPropertySelectMode item)
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
        /// Only allow selecting a single item
        /// </summary>
        public const string Single = "Single";

        /// <summary>
        /// Allow selecting multiple items
        /// </summary>
        public const string Multiple = "Multiple";

        /// <summary>
        /// Not Applicable
        /// </summary>
        public const string Blank = "Blank";

        #endregion Enum members

        #region IEquatable (CswEnumNbtPropertySelectMode)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtPropertySelectMode ft1, CswEnumNbtPropertySelectMode ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtPropertySelectMode ft1, CswEnumNbtPropertySelectMode ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtPropertySelectMode))
            {
                return false;
            }
            return this == (CswEnumNbtPropertySelectMode)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtPropertySelectMode obj)
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

        #endregion IEquatable (CswEnumNbtPropertySelectMode)

    };

}
