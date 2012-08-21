using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Grid.ExtJs
{
    public class CswNbtGridExtJsDataIndex : IEquatable<CswNbtGridExtJsDataIndex>
    {
        private string _prefix;
        private string _dataIndex;

        public CswNbtGridExtJsDataIndex( string UniquePrefix, string dataIndex )
        {
            _prefix = UniquePrefix.Replace( " ", "_" ).ToLower();
            _dataIndex = dataIndex.Replace( " ", "_" ).ToLower();
        }

        /// <summary>
        /// Returns prefix + dataIndex as a string
        /// </summary>
        public string safeString()
        {
            return _prefix + _dataIndex;
        }

        /// <summary>
        /// Returns dataIndex as a string
        /// </summary>
        public override string ToString()
        {
            return _dataIndex;
        }

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtGridExtJsDataIndex dataIndex1, CswNbtGridExtJsDataIndex dataIndex2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( dataIndex1, dataIndex2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) dataIndex1 == null ) || ( (object) dataIndex2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            return ( dataIndex1._dataIndex == dataIndex2._dataIndex );
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtGridExtJsDataIndex dataIndex1, CswNbtGridExtJsDataIndex dataIndex2 )
        {
            return !( dataIndex1 == dataIndex2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtGridExtJsDataIndex ) )
                return false;
            return this == (CswNbtGridExtJsDataIndex) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtGridExtJsDataIndex obj )
        {
            return this == (CswNbtGridExtJsDataIndex) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return _dataIndex.GetHashCode();
        }

        #endregion IEquatable


    } // class CswNbtGridExtJsDataIndex

} // namespace ChemSW.Nbt.Grid.ExtJs
