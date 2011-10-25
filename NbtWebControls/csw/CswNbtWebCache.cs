using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using ChemSW.Core;

namespace ChemSW.Nbt
{
	/// <summary>
	/// Abstraction of a Session-specific Cache
	/// </summary>
	public class CswNbtWebCache
	{
		private Cache _Cache;
		private string _SessionId;
		public string SessionId
		{
			set { _SessionId = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public CswNbtWebCache( HttpContext Context, string SessionId )
		{
			_Cache = Context.Cache;
			_SessionId = SessionId;
		}

		/// <summary>
		/// Add an object to the cache, for this session
		/// </summary>
		public void Add( string Key, object Value )
		{
			if( Value != null )
			{
				//Context.Cache.Add( "CswNbtMetaData_" + SessionId, _CswNbtResources.MetaData, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null );
				_Cache[_makeRealKey( Key )] = Value;
			}
		} // Add()

		/// <summary>
		/// Get an object from the cache, for this session
		/// </summary>
		public object Get( string Key )
		{
			object ret = null;
			if( Key != string.Empty )
			{
				ret = _Cache[_makeRealKey( Key )];
			}
			return ret;
		} // Get()

		/// <summary>
		/// Clear the cache, for this session
		/// </summary>
		public void Clear()
		{
			foreach( string Key in _Cache )
			{
				CswDelimitedString RealKey = _parseRealKey( Key );
				if( RealKey[1] == _SessionId )
				{
					_Cache.Remove( Key );
				}
			}
		} // Clear()

		private string _makeRealKey( string InKey )
		{
			CswDelimitedString RealKey = new CswDelimitedString( '_' );
			RealKey.Add( InKey );
			RealKey.Add( _SessionId );
			return RealKey.ToString();
		}

		private CswDelimitedString _parseRealKey( string InRealKey )
		{
			CswDelimitedString RealKey = new CswDelimitedString( '_' );
			RealKey.FromString( InRealKey );
			return RealKey;
		}

	} // class CswNbtWebServiceCache
} // namespace ChemSW.WebServices