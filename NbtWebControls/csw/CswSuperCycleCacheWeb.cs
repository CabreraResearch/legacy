using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Caching;
using System.Threading;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Log;
using ChemSW.RscAdo;
using ChemSW.Security;

namespace ChemSW.Nbt
{
    public class CswSuperCycleCacheWeb : ICswSuperCycleCache
    {
        private enum CachedItems { CacheDirtyThreshold }
        struct DatedObject
        {
            public Object Object;
            public DateTime DateTime;
        }//struct DatedObject


        /// <summary>
        /// We don't know who else might have put things in the cache, so we can only clear what we're responsible for.
        /// </summary>

        ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim();
        private Cache _Cache = null;
        public CswSuperCycleCacheWeb( Cache Cache )
        {
            _Cache = Cache;
        }//ctor


        public DateTime CacheDirtyThreshold
        {

            set
            {
                string Key = CachedItems.CacheDirtyThreshold.ToString() ;
                Object DateTimeObj = _Cache.Get(Key );
                if( null != DateTimeObj )
                {
                    _Cache.Remove( Key );
                }

                _Cache.Add( Key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null );

            }//set


            get
            {
                DateTime ReturnVal = DateTime.Now;
                Object Object = _Cache.Get( CachedItems.CacheDirtyThreshold.ToString() );
                if( null != Object )
                {
                    ReturnVal = (DateTime) Object;
                }

                return ( ReturnVal );
            }//get

        }//CacheDirtyThreshold


        string _SessionId = string.Empty;
        public string SessionId { set { _SessionId = value; } }

        public void delete( string[] Names )
        {
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                foreach( string CurrentName in Names )
                {
                    string Key = _makeRealKey( CurrentName );
                    _Cache.Remove( Key );
                }
            }

            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }

        }//delete()

        public void delete( string Name )
        {
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                string Key = _makeRealKey( Name );
                _Cache.Remove( Key );
            }

            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }

        }//delete()

        public void put( string Name, object Object )
        {

            if( null != Object )
            {
                DatedObject DatedObject = new DatedObject();
                DatedObject.Object = Object;
                DatedObject.DateTime = DateTime.Now;
                try
                {
                    _ReaderWriterLockSlim.EnterWriteLock();

                    string Key = _makeRealKey( Name );
                    _Cache.Add( Key, DatedObject, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null );
                }

                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }

            }//if object is not null

        }//put() 


        public object get( string Name )
        {
            object ReturnVal = null;

            Object Object = null;
            string Key = string.Empty; 
            try
            {
                _ReaderWriterLockSlim.EnterReadLock();
                Key = _makeRealKey( Name );
                Object = _Cache.Get( Key );

            }//try

            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }

            if( null != Object )
            {
                DatedObject DatedObject = (DatedObject) Object;
                if( DatedObject.DateTime >= CacheDirtyThreshold )
                {
                    ReturnVal = DatedObject.Object;
                }
                else
                {
                    try
                    {
                        _ReaderWriterLockSlim.EnterWriteLock();
                        _Cache.Remove( Key ); //ReturnVal will be null by default
                    }
                    finally
                    {
                        _ReaderWriterLockSlim.ExitWriteLock();

                    }

                }//if-else the object is stale

            }//if there is an object for that key



            return ( ReturnVal );
        }


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


    } // class ICswSuperCycleCache

} // namespace ChemSW

