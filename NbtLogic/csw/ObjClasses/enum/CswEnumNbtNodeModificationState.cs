using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    //bz # 5943
    /// <summary>
    /// State of node
    /// </summary>
    public sealed class CswEnumNbtNodeModificationState : IEquatable<CswEnumNbtNodeModificationState>
    {
        /// <summary>
        /// Unknown
        /// </summary>
        public const string Unknown = CswNbtResources.UnknownEnum;

        /// <summary>
        /// The node contains no data
        /// </summary>
        public const string Empty = "Empty";

        /// <summary>
        /// The node and its properties have been read from the database
        /// </summary>
        public const string Unchanged = "Unchanged";

        /// <summary>
        /// The value one of the node's selectors or of its properties has been modified
        /// </summary>
        public const string Modified = "Modified";
        //Set,
        /// <summary>
        /// The Node's data has been written to the database, but not yet committed
        /// </summary>
        public const string Posted = "Posted";
        /// <summary>
        /// The node has been removed from the database
        /// </summary>
        public const string Deleted = "Deleted";

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Empty, Empty },
                                                                       { Unchanged, Unchanged },
                                                                       { Modified, Modified },
                                                                       { Posted, Posted },
                                                                       { Deleted, Deleted }
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
        public CswEnumNbtNodeModificationState( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        public static implicit operator CswEnumNbtNodeModificationState( string Val )
        {
            return new CswEnumNbtNodeModificationState( Val );
        }
        public static implicit operator string( CswEnumNbtNodeModificationState item )
        {
            return item.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        #region IEquatable (NodeModificationState)

        public static bool operator ==( CswEnumNbtNodeModificationState ft1, CswEnumNbtNodeModificationState ft2 )
        {
            //do a string comparison on the fieldtypes
            return ft1.ToString() == ft2.ToString();
        }

        public static bool operator !=( CswEnumNbtNodeModificationState ft1, CswEnumNbtNodeModificationState ft2 )
        {
            return !( ft1 == ft2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtNodeModificationState ) )
                return false;
            return this == (CswEnumNbtNodeModificationState) obj;
        }

        public bool Equals( CswEnumNbtNodeModificationState obj )
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

        #endregion IEquatable (NodeModificationState)

    }
}//namespace ChemSW.Nbt.ObjClasses
