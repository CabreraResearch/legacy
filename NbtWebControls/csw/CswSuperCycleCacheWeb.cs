using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Caching;
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
        Cache _Cache = null;
        public CswSuperCycleCacheWeb( Cache Cache )
        {
            _Cache = Cache;
        }//ctor

        public void put( string Name, object Object )
        {

           string Key = Name.ToLower();

           _Cache.Add( Key, Object, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null );

        }//put() 

        public object get( string Name )
        {
            object Returnval = null;

            string Key = Name.ToLower();

            _Cache.Get( Key ); 

            return ( Returnval );
        }

    } // class ICswSuperCycleCache

} // namespace ChemSW

