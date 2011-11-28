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
        /// <summary>
        /// We don't know who else might have put things in the cache, so we can only clear what we're responsible for.
        /// </summary>


        private Cache _Cache = null;
        private List<string> _CachedItemNames = new List<string>();
        public CswSuperCycleCacheWeb( Cache Cache )
        {
            _Cache = Cache;
        }//ctor


        public void delete( string[] Names )
        {
            lock( this )
            {
                foreach( string CurrentName in Names )
                {
                    _Cache.Remove( CurrentName.ToLower() );
                }
            }

        }//delete()


        public void put( string Name, object Object )
        {
            lock( this )
            {
                string Key = Name.ToLower();

                _Cache.Add( Key, Object, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null );
                _CachedItemNames.Add( Key );
            }

        }//put() 


        public object get( string Name )
        {
            lock( this )
            {
                string Key = Name.ToLower();
                return ( _Cache.Get( Key ) );
            }
        }

    } // class ICswSuperCycleCache

} // namespace ChemSW

