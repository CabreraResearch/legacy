using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents a Session View Id
    /// </summary>
	/// <remarks>
	/// This class exists solely to distinguish this Int32 from the ViewId Int32, 
	/// and to prevent developers from using them interchangably
	/// </remarks>
    public class CswNbtSessionViewId : IEquatable<CswNbtSessionViewId>
    {
		Int32 _SessionViewId = Int32.MinValue;

		public CswNbtSessionViewId()
		{
		}

		public CswNbtSessionViewId( Int32 Value )
		{
			_SessionViewId = Value;
		}

		public Int32 get()
		{
			return _SessionViewId;
		}
		public void set(Int32 value)
		{
			_SessionViewId = value;
		}

		public bool isSet()
		{
			return ( _SessionViewId != Int32.MinValue );
		}

		public override string ToString()
		{
			return CswConvert.ToString( _SessionViewId );
		}
		#region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
		public static bool operator ==( CswNbtSessionViewId viewid1, CswNbtSessionViewId viewid2 )
        {
            // If both are null, or both are same instance, return true.
			if( System.Object.ReferenceEquals( viewid1, viewid2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
			if( ( (object) viewid1 == null ) || ( (object) viewid2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
			if( viewid1.get() == viewid2.get() )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
		public static bool operator !=( CswNbtSessionViewId viewid1, CswNbtSessionViewId viewid2 )
        {
			return !( viewid1 == viewid2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
			if( !( obj is CswNbtSessionViewId ) )
                return false;
			return this == (CswNbtSessionViewId) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
		public bool Equals( CswNbtSessionViewId obj )
        {
			return this == (CswNbtSessionViewId) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
			return this.get();
        }
        #endregion IEquatable

    } // class CswNbtView


} // namespace ChemSW.Nbt



