using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Type of Node 
    /// </summary>
    public sealed class CswEnumNbtNodeSpecies : IEquatable<CswEnumNbtNodeSpecies>
    {
        /// <summary>
        /// Regular, run-of-the-mill Node
        /// </summary>
        public const string Plain = "Plain";

        /// <summary>
        /// Audit Node
        /// </summary>
        public const string Audit = "Audit";

        /// <summary>
        /// Group
        /// </summary>
        public const string Group = "Group";

        /// <summary>
        /// Root Node
        /// </summary>
        public const string Root = "Root";

        /// <summary>
        /// More Node
        /// </summary>
        public const string More = "More";

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Plain, Plain },
                                                                       { Audit, Audit },
                                                                       { Group, Group },
                                                                       { More, More },
                                                                       { Root, Root }
                                                                   };

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
        public CswEnumNbtNodeSpecies( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        public static implicit operator CswEnumNbtNodeSpecies( string Val )
        {
            return new CswEnumNbtNodeSpecies( Val );
        }
        public static implicit operator string( CswEnumNbtNodeSpecies item )
        {
            return item.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        #region IEquatable (CswEnum)

        public static bool operator ==( CswEnumNbtNodeSpecies ft1, CswEnumNbtNodeSpecies ft2 )
        {
            //do a string comparison on the fieldtypes
            return ft1.ToString() == ft2.ToString();
        }

        public static bool operator !=( CswEnumNbtNodeSpecies ft1, CswEnumNbtNodeSpecies ft2 )
        {
            return !( ft1 == ft2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtNodeSpecies ) )
                return false;
            return this == (CswEnumNbtNodeSpecies) obj;
        }

        public bool Equals( CswEnumNbtNodeSpecies obj )
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

        #endregion IEquatable (NodeSpecies)

    }

}//namespace ChemSW.Nbt.ObjClasses
