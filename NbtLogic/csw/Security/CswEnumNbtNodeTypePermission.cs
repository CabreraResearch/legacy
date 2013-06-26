using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.Security
{
    public sealed class CswEnumNbtNodeTypePermission : IEquatable<CswEnumNbtNodeTypePermission>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                {
                                                                    { Create, Create },
                                                                    { View, View },
                                                                    { Edit, Edit },
                                                                    { Delete, Delete }
                                                                };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        public static Collection<CswEnumNbtNodeTypePermission> Members
        {
            get 
            {
                return new Collection<CswEnumNbtNodeTypePermission> { Create, View, Edit, Delete };
            }
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
        public CswEnumNbtNodeTypePermission(string ItemName = CswResources.UnknownEnum)
        {
            Value = _Parse(ItemName);
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtNodeTypePermission(string Val)
        {
            return new CswEnumNbtNodeTypePermission(Val);
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string(CswEnumNbtNodeTypePermission item)
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
        /// NodeTypePermission to view nodes of this type
        /// </summary>
        public const string View = "View";

        /// <summary>
        /// NodeTypePermission to create new nodes of this type
        /// </summary>
        public const string Create = "Create";

        /// <summary>
        /// NodeTypePermission to delete nodes of this type
        /// </summary>
        public const string Delete = "Delete";

        /// <summary>
        /// NodeTypePermission to edit property values of nodes of this type
        /// </summary>
        public const string Edit = "Edit";

        #endregion Enum members

        #region IEquatable (CswEnumNbtNodeTypePermission)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==(CswEnumNbtNodeTypePermission ft1, CswEnumNbtNodeTypePermission ft2)
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString(ft1) == CswConvert.ToString(ft2);
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=(CswEnumNbtNodeTypePermission ft1, CswEnumNbtNodeTypePermission ft2)
        {
            return !(ft1 == ft2);
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswEnumNbtNodeTypePermission))
            {
                return false;
            }
            return this == (CswEnumNbtNodeTypePermission)obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals(CswEnumNbtNodeTypePermission obj)
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

        #endregion IEquatable (CswEnumNbtNodeTypePermission)

    };

} // namespace ChemSW.Nbt.Security
