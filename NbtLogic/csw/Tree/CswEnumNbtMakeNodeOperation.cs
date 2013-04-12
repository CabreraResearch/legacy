using System;
using System.Collections.Generic;

namespace ChemSW.Nbt
{

    /// <summary>
    /// Specifies operation to take on database when using makeNodeFromNodeTypeId
    /// </summary>
    public sealed class CswEnumNbtMakeNodeOperation : IEquatable<CswEnumNbtMakeNodeOperation>
    {

        #region Enum Member

        /// <summary>
        /// Write the new node to the database
        /// </summary>
        public const string WriteNode = "WriteNode";

        /// <summary>
        /// Just set the primary key and the default property values, but make
        /// no changes to the database
        /// </summary>
        public const string JustSetPk = "JustSetPk";

        /// <summary>
        /// Just get an empty node with meta data from the nodetype
        /// </summary>
        public const string DoNothing = "DoNothing";

        /// <summary>
        /// Write the new temporary node to the database.
        /// </summary>
        public const string MakeTemp = "MakeTemp";
        #endregion

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { WriteNode, WriteNode },
                                                                       { JustSetPk, JustSetPk },
                                                                       { DoNothing, DoNothing },
                                                                       { MakeTemp, MakeTemp }
                                                                   };
        /// <summary>
        /// Instance value
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
        /// Constructor
        /// </summary>
        public CswEnumNbtMakeNodeOperation( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit enum cast
        /// </summary>
        public static implicit operator CswEnumNbtMakeNodeOperation( string Val )
        {
            return new CswEnumNbtMakeNodeOperation( Val );
        }

        /// <summary>
        /// Implicit string cast
        /// </summary>
        public static implicit operator string( CswEnumNbtMakeNodeOperation item )
        {
            return item.Value;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #region IEquatable (MakeNodeOperation)

        /// <summary>
        /// ==
        /// </summary>
        public static bool operator ==( CswEnumNbtMakeNodeOperation ft1, CswEnumNbtMakeNodeOperation ft2 )
        {
            //do a string comparison on the fieldtypes
            return ft1.ToString() == ft2.ToString();
        }

        /// <summary>
        /// !=
        /// </summary>
        public static bool operator !=( CswEnumNbtMakeNodeOperation ft1, CswEnumNbtMakeNodeOperation ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtMakeNodeOperation ) )
                return false;
            return this == (CswEnumNbtMakeNodeOperation) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtMakeNodeOperation obj )
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

        #endregion IEquatable (MakeNodeOperation)

    }
} // namespace ChemSW.Nbt


