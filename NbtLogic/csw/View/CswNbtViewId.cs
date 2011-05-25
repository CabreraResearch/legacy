using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;

namespace ChemSW.Nbt
{
	/// <summary>
	/// Represents a View Id
	/// </summary>
	/// <remarks>
	/// This class exists to distinguish this Int32 from the SessionViewId Int32, 
	/// and to prevent developers from using them interchangably
	/// </remarks>
	public class CswNbtViewId : IEquatable<CswNbtViewId>
	{
		private static char _delimiter = '_';
		private static string _StringPrefix = "ViewId";
		private Int32 _ViewId = Int32.MinValue;

		public CswNbtViewId()
		{
		}
		public CswNbtViewId( Int32 value )
		{
			_ViewId = value;
		}
		public CswNbtViewId(string ViewIdString )
		{
			CswDelimitedString delimstr = new CswDelimitedString( _delimiter );
			delimstr.FromString( ViewIdString );
			_ViewId = CswConvert.ToInt32( delimstr[1] );
		}
		
		public static bool isViewIdString(string TestString)
		{
			CswDelimitedString delimstr = new CswDelimitedString( _delimiter );
			delimstr.FromString( TestString );
			
			return (delimstr.Count == 2 &&
					delimstr[0] == _StringPrefix &&
					CswTools.IsInteger(delimstr[1]));
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
			CswDelimitedString delimstr = new CswDelimitedString( _delimiter );
			if( isSet() )
			{
				delimstr[0] = _StringPrefix;
				delimstr[1] = _ViewId.ToString();
			}
			return delimstr.ToString();
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
