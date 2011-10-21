using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents a Session View Id
    /// </summary>
    /// <remarks>
    /// This class exists to distinguish this Int32 from the ViewId Int32, 
    /// and to prevent developers from using them interchangably
    /// </remarks>
    public class CswNbtSessionDataId : IEquatable<CswNbtSessionDataId>
    {
        private static char _delimiter = '_';
        private static string _StringPrefix = "SessionDataId";
        private Int32 _SessionDataId = Int32.MinValue;

        public CswNbtSessionDataId()
        {
        }

        public CswNbtSessionDataId( Int32 Value )
        {
            _SessionDataId = Value;
        }
        public CswNbtSessionDataId( string SessionDataIdString )
        {
            CswDelimitedString delimstr = new CswDelimitedString( _delimiter );
            delimstr.FromString( SessionDataIdString );
            _SessionDataId = CswConvert.ToInt32( delimstr[1] );
        }

        public static bool isSessionDataIdString( string TestString )
        {
            CswDelimitedString delimstr = new CswDelimitedString( _delimiter );
            delimstr.FromString( TestString );

            return ( delimstr.Count == 2 &&
                    delimstr[0] == _StringPrefix &&
                    CswTools.IsInteger( delimstr[1] ) );
        }

        public Int32 get()
        {
            return _SessionDataId;
        }
        public void set( Int32 value )
        {
            _SessionDataId = value;
        }

        public bool isSet()
        {
            return ( _SessionDataId != Int32.MinValue );
        }

        public override string ToString()
        {
            CswDelimitedString delimstr = new CswDelimitedString( _delimiter );
            if( isSet() )
            {
                delimstr[0] = _StringPrefix;
                delimstr[1] = _SessionDataId.ToString();
            }
            return delimstr.ToString();
        }

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtSessionDataId sdid1, CswNbtSessionDataId sdid2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( sdid1, sdid2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) sdid1 == null ) || ( (object) sdid2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( sdid1.get() == sdid2.get() )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtSessionDataId sdid1, CswNbtSessionDataId sdid2 )
        {
            return !( sdid1 == sdid2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtSessionDataId ) )
                return false;
            return this == (CswNbtSessionDataId) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtSessionDataId obj )
        {
            return this == (CswNbtSessionDataId) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return this.get();
        }

        #endregion IEquatable

    } // class CswNbtSessionDataId
} // namespace ChemSW.Nbt
