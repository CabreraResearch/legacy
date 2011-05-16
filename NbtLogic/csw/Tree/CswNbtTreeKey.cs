using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Exceptions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Uniquely identifies a Tree.
    /// </summary>
    /// <remarks>
    /// This is really just a thin wrapper around CswNbtView at this point
    /// </remarks>
    [Serializable()]
    public class CswNbtTreeKey : System.IEquatable<CswNbtTreeKey>
    {
        private CswNbtResources _CswNbtResources;
        
        private void _RealConstructor(CswNbtResources CswNbtResources)
        {
            _CswNbtResources = CswNbtResources;
            //View = new CswNbtView(_CswNbtResources);
        }

        /// <summary>
        /// Constructor: empty
        /// </summary>
        public CswNbtTreeKey(CswNbtResources CswNbtResources)//Int32 ArbitraryTreeId)
        {
            _RealConstructor(CswNbtResources);
        }

        ///// <summary>
        ///// Constructor: from string
        ///// </summary>
        //public CswNbtTreeKey(CswNbtResources CswNbtResources, string StringKey)
        //{
        //    _RealConstructor(CswNbtResources);
        //    if (string.Empty != StringKey)
        //    {
        //        View.LoadXml(StringKey);
        //    }
        //}//ctor

        ///// <summary>
        ///// Constructor: from View
        ///// </summary>
        //public CswNbtTreeKey(CswNbtResources CswNbtResources, CswNbtView TheView)
        //{
        //    _RealConstructor(CswNbtResources);
        //    View = TheView;
        //}//ctor

		public CswNbtTreeKey( CswNbtResources CswNbtResources, CswNbtSessionViewId TheSessionViewId )
        {
            _RealConstructor(CswNbtResources);
            SessionViewId = TheSessionViewId;
        }

        /// <summary>
        /// Convert a tree key into a string
        /// </summary>
        public override string ToString()
        {
            string ret = "";
            //ret += View.ToString();
            ret += SessionViewId.ToString();
            return ret;
        }//ToString()


        //private CswNbtView _View;
        ///// <summary>
        ///// Primary key of view used to create this tree
        ///// </summary>
        //public CswNbtView View
        //{
        //    get { return _View; }
        //    set { _View = value; }
        //}

        private CswNbtSessionViewId _SessionViewId = null;
        /// <summary>
        /// Session-specific ViewId for View used to create this tree
        /// </summary>
        public CswNbtSessionViewId SessionViewId
        {
            get { return _SessionViewId; }
            set { _SessionViewId = value; }
        }


        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==(CswNbtTreeKey key1, CswNbtTreeKey key2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(key1, key2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)key1 == null) || ((object)key2 == null))
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if (key1.SessionViewId == key2.SessionViewId)
                return true;
            else
                return false;
                    
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=(CswNbtTreeKey key1, CswNbtTreeKey key2)
        {
            return !(key1 == key2);
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is CswNbtTreeKey))
                return false;
            return this == (CswNbtTreeKey)obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals(CswNbtTreeKey obj)
        {
            return this == (CswNbtTreeKey)obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return SessionViewId.get();
        }
        #endregion IEquatable

    }//CswNbtTreeKey

}//namespace ChemSW.Nbt
