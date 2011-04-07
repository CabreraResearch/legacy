using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using ChemSW.Exceptions;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Stores View information in Session
    /// </summary>
    public class CswNbtViewCache
    {
        private CswNbtResources _CswNbtResources;
        private Hashtable ViewHash;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtViewCache(CswNbtResources CswNbtResources)
        {
            _CswNbtResources = CswNbtResources;
            ViewHash = new Hashtable();

            if (delim1 == CswNbtView.delimiter.ToString() ||
                delim2 == CswNbtView.delimiter.ToString())
            {
                throw new CswDniException("CswNbtViewCache cannot use CswNbtViewBase's delimiter for either of its delimiters");
            }
        }

        /// <summary>
        /// Fetch a view from the Cache
        /// </summary>
        public CswNbtView getView(Int32 SessionViewId)
        {
            if( ViewHash.ContainsKey( SessionViewId ) )
                return CswNbtViewFactory.restoreView( _CswNbtResources, ViewHash[SessionViewId].ToString() );
            else
                return null;
            //throw new CswDniException("View Not Found", "Could not find View matching SessionViewId: " + SessionViewId.ToString());
        }

        /// <summary>
        /// Put a view into the Cache
        /// </summary>
        public Int32 putView(CswNbtView View)
        {
            CswTimer Timer = new CswTimer();

            Int32 key = View.GetHashCode();
            while (ViewHash.ContainsKey(key) && ViewHash[key].ToString() != View.ToString())
                key++;
            if(!ViewHash.ContainsKey(key))
                ViewHash.Add(key, View.ToString());

            _CswNbtResources.logTimerResult("CswNbtViewCache.putView()", Timer.ElapsedDurationInSecondsAsString);
            return key;
        }

        ///// <summary>
        ///// Updates an existing view in the cache (for example, if the view is altered and saved)
        ///// </summary>
        ///// <param name="Key">View Cache Hash Key</param>
        ///// <param name="View">Updated View</param>
        ///// <remarks>We may want to add this as a new view into the cache, rather than updating, if it causes problems.</remarks>
        //public void updateCachedView(Int32 Key, CswNbtViewBase View)
        //{
        //    if (ViewHash.ContainsKey(Key))
        //        ViewHash[Key] = View.ToString();
        //}

        /// <summary>
        /// Removes all instances of a view from the cache
        /// </summary>
        /// <param name="View">View to remove</param>
        public void clearFromCache( CswNbtView View )
        {
			// case 21181 - this did not clear changed views
			//string ViewToClear = View.ToString();
			//ArrayList KeysToRemove = new ArrayList();
			//foreach( Int32 Key in ViewHash.Keys )
			//{
			//    string ThisView = ViewHash[Key].ToString();
			//    if( ThisView == ViewToClear )
			//        KeysToRemove.Add( Key );
			//}
			//foreach( Int32 Key in KeysToRemove )
			//    ViewHash.Remove( Key );

			if( View.SessionViewId != Int32.MinValue && ViewHash.ContainsKey( View.SessionViewId ) )
			{
				ViewHash.Remove( View.SessionViewId );
			}
			if( View.ViewId != Int32.MinValue && ViewHash.ContainsKey( View.ViewId ) )
			{
				ViewHash.Remove( View.ViewId );
			}
		} // clearFromCache()


        private string delim1 = "^^^";    // it's important that these delimiters are not the same delimiter
        private string delim2 = "###";    // as CswNbtViewBase.delimiter

        /// <summary>
        /// Saves all views and their indexes to a string
        /// </summary>
        public override string ToString()
        {
            string ret = "";
            foreach(Int32 Key in ViewHash.Keys)
            {
                ret += Key.ToString() + delim1 + ViewHash[Key].ToString() + delim2;
            }
            return ret;
        }

        /// <summary>
        /// Restores all views and their indexes from a string
        /// </summary>
        public void FromString(string ViewCacheAsString)
        {
            ViewHash.Clear();
            string[] delim2arr = new string[1];
            delim2arr[0] = delim2;
            string[] Split1 = ViewCacheAsString.Split(delim2arr, StringSplitOptions.RemoveEmptyEntries);
            foreach (string elm in Split1)
            {
                if( elm != string.Empty )
                {
                    string[] delim1arr = new string[1];
                    delim1arr[0] = delim1;
                    string[] Split2 = elm.Split( delim1arr, StringSplitOptions.RemoveEmptyEntries );
                    Int32 key = CswConvert.ToInt32( Split2[0] );
                    string ViewString = Split2[1].ToString();
                    if( !ViewHash.ContainsKey( key ) )
                        ViewHash.Add( key, ViewString );
                    else
                        throw new CswDniException( "Invalid ViewCache Restore", "Found redundant keys in ViewCache" );
                }
            }
        }
    }
}
