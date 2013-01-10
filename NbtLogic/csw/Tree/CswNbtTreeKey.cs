using System;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Uniquely identifies a Tree.
    /// </summary>
    /// <remarks>
    /// This is a thin wrapper around a CswNbtViewId or CswNbtSessionDataId
    /// </remarks>
    [Serializable()]
    public class CswNbtTreeKey : System.IEquatable<CswNbtTreeKey>
    {
        private CswNbtResources _CswNbtResources;
        private string _KeyString = string.Empty;

        /// <summary>
        /// Constructor: from View
        /// </summary>
        public CswNbtTreeKey( CswNbtResources CswNbtResources, CswNbtView TheView )
        {
            _CswNbtResources = CswNbtResources;

            if( TheView != null )
            {
                if( TheView.ViewId != null && TheView.ViewId.isSet() )
                {
                    _KeyString = TheView.ViewId.ToString();
                }
                else if( TheView.SessionViewId != null && TheView.SessionViewId.isSet() )
                {
                    //TheView.SaveToCache(false);
                    _KeyString = TheView.SessionViewId.ToString();
                }
            }
            else
            {
                // stay empty
            }
        }//ctor

        /// <summary>
        /// Constructor: from String
        /// </summary>
        public CswNbtTreeKey( CswNbtResources CswNbtResources, string KeyString )
        {
            _CswNbtResources = CswNbtResources;

            if( CswNbtViewId.isViewIdString( KeyString ) )
            {
                _KeyString = new CswNbtViewId( KeyString ).ToString();
            }
            else if( CswNbtSessionDataId.isSessionDataIdString( KeyString) )
            {
                _KeyString = new CswNbtSessionDataId( KeyString ).ToString();
            }
        }//ctor

        /// <summary>
        /// Convert a tree key into a string
        /// </summary>
        public override string ToString()
        {
            return _KeyString;
        }//ToString()

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtTreeKey key1, CswNbtTreeKey key2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( key1, key2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) key1 == null ) || ( (object) key2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( key1._KeyString == key2._KeyString )
                return true;
            else
                return false;

        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtTreeKey key1, CswNbtTreeKey key2 )
        {
            return !( key1 == key2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtTreeKey ) )
                return false;
            return this == (CswNbtTreeKey) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtTreeKey obj )
        {
            return this == (CswNbtTreeKey) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return _KeyString.GetHashCode();
        }
        #endregion IEquatable

    }//CswNbtTreeKey

}//namespace ChemSW.Nbt
