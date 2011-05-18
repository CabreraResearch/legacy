using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemSW.Nbt
{
	public class CswNbtViewId : IEquatable<CswNbtViewId>
	{
		private Int32 _ViewId;
		public CswNbtViewId()
		{
		}
		public CswNbtViewId( Int32 value )
		{
			_ViewId = value;
		}

		public Int32 get()
		{
			return _ViewId;
		}
		public void set(Int32 value)
		{
			_ViewId = value;
		}
		public bool isSet()
		{
			return ( _ViewId != Int32.MinValue );
		}

		public override string ToString()
		{
			string ret = string.Empty;
			if(isSet())
				ret = _ViewId.ToString();
			return ret;
		}

		#region IEquatable
		/// <summary>
		/// IEquatable: ==
		/// </summary>
		public static bool operator ==( CswNbtViewId viewid1, CswNbtViewId viewid2 )
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
		public static bool operator !=( CswNbtViewId viewid1, CswNbtViewId viewid2 )
		{
			return !( viewid1 == viewid2 );
		}

		/// <summary>
		/// IEquatable: Equals
		/// </summary>
		public override bool Equals( object obj )
		{
			if( !( obj is CswNbtViewId ) )
				return false;
			return this == (CswNbtViewId) obj;
		}

		/// <summary>
		/// IEquatable: Equals
		/// </summary>
		public bool Equals( CswNbtViewId obj )
		{
			return this == (CswNbtViewId) obj;
		}

		/// <summary>
		/// IEquatable: GetHashCode
		/// </summary>
		public override int GetHashCode()
		{
			return this.get();
		}

		#endregion IEquatable

	}
}
