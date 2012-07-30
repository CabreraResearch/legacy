using System;
using ChemSW.Core;

namespace ChemSW.Nbt.Grid.ExtJs
{
    public class CswNbtGridExtJsDataIndex : IEquatable<CswNbtGridExtJsDataIndex>
    {
        private string _dataIndex;

        public CswNbtGridExtJsDataIndex( CswNbtView ViewForPrefix, string dataIndex )
        {
            CswDelimitedString Idx = new CswDelimitedString( '_' );
            if( null != ViewForPrefix )
            {
                if( ViewForPrefix.IsSessionViewIdSet() )
                {
                    Idx.Add( ViewForPrefix.SessionViewId.ToString() );
                }
                else if( ViewForPrefix.IsViewIdSet() )
                {
                    Idx.Add( ViewForPrefix.ViewId.ToString() );
                }
            }
            Idx.Add( dataIndex.Replace( " ", "" ) );
            _dataIndex = Idx.ToString( EscapeDelimiterInstances: false );
        }

        public CswNbtGridExtJsDataIndex( string UniquePrefix, string dataIndex )
        {
            CswDelimitedString Idx = new CswDelimitedString( '_' );
            Idx.Add( UniquePrefix.Replace( " ", "" ) );
            Idx.Add( dataIndex.Replace( " ", "" ) );
            _dataIndex = Idx.ToString( EscapeDelimiterInstances: false );
        }

        public override string ToString()
        {
            return _dataIndex.ToLower();
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
