using ChemSW.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Security
{

    public sealed class CswEnumNbtNodeTypeTabPermission : IEquatable<CswEnumNbtNodeTypeTabPermission>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                {
                                                                    { Edit, Edit },
                                                                    { View, View }
                                                                };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        public static Collection<CswEnumNbtNodeTypeTabPermission> Members
        {
            get { return new Collection<CswEnumNbtNodeTypeTabPermission> { View, Edit }; }
        }

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
        public CswEnumNbtNodeTypeTabPermission(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtNodeTypeTabPermission(string Val)
        {
            return new CswEnumNbtNodeTypeTabPermission(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtNodeTypeTabPermission item)
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
        /// NodeTypePermission to view the tab
        /// </summary>
        public const string View = "View";

        /// <summary>
        /// NodeTypePermission to edit property values on this tab
        /// </summary>
        public const string Edit = "Edit";



        #endregion Enum members

        #region IEquatable (CswEnumNbtNodeTypeTabPermission)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtNodeTypeTabPermission ft1, CswEnumNbtNodeTypeTabPermission ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtNodeTypeTabPermission ft1, CswEnumNbtNodeTypeTabPermission ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtNodeTypeTabPermission))
            {
                return false;
            }
            return this == (CswEnumNbtNodeTypeTabPermission)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtNodeTypeTabPermission obj)
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

        #endregion IEquatable (CswEnumNbtNodeTypeTabPermission)

    };


} // namespace ChemSW.Nbt.Security
