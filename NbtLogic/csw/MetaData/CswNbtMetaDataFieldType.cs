using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataFieldType : ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataFieldType>, IComparable
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        private DataRow _FieldTypeRow;
        //public ICswNbtFieldTypeRule FieldTypeRule = null;

        public CswNbtMetaDataFieldType( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;

            Reassign( Row );

            //CswNbtFieldTypeRuleFactory CswNbtFieldTypeRuleFactory = new CswNbtFieldTypeRuleFactory( CswNbtMetaDataResources.CswNbtResources );
            //FieldTypeRule = CswNbtFieldTypeRuleFactory.makeRule( FieldType );
        }

        public static CswEnumNbtFieldType getFieldTypeFromString( string FieldTypeName )
        {
            return FieldTypeName;
        }

        public DataRow _DataRow
        {
            get { return _FieldTypeRow; }
            //set { _FieldTypeRow = value; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
            //set { _UniqueId = value; }
        }

        public string UniqueIdFieldName { get { return "fieldtypeid"; } }

        public void Reassign( DataRow NewRow )
        {
            _FieldTypeRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        public Int32 FieldTypeId
        {
            get { return CswConvert.ToInt32( _FieldTypeRow["fieldtypeid"].ToString() ); }
        }

        public CswEnumNbtFieldType FieldType
        {
            get
            {
                return CswConvert.ToString( _FieldTypeRow["fieldtype"] );
            }
        }

        /// <summary>
        /// Returns whether Universal Search is enabled for this field type
        /// </summary>
        public bool Searchable
        {
            get { return CswConvert.ToBoolean( _FieldTypeRow["searchable"] ); }
        }

        /// <summary>
        /// Returns whether the field type should be considered a "simple" type,
        /// meaning that it is not a fancy-schmancy UI control
        /// </summary>
        public bool IsSimpleType()
        {
            return ( FieldType == CswEnumNbtFieldType.DateTime ||
                //FieldType == NbtFieldType.Time ||
                     FieldType == CswEnumNbtFieldType.List ||
                     FieldType == CswEnumNbtFieldType.Logical ||
                     FieldType == CswEnumNbtFieldType.Memo ||
                     FieldType == CswEnumNbtFieldType.Number ||
                     FieldType == CswEnumNbtFieldType.Static ||
                     FieldType == CswEnumNbtFieldType.Text );
        }

        /// <summary>
        /// Returns whether the field type only displays a computed value, 
        /// rather than storing a user-specified value
        /// </summary>
        public bool IsDisplayType()
        {
            return ( FieldType == CswEnumNbtFieldType.Composite ||
                     FieldType == CswEnumNbtFieldType.External ||
                     FieldType == CswEnumNbtFieldType.Grid ||
                     FieldType == CswEnumNbtFieldType.LocationContents ||
                     FieldType == CswEnumNbtFieldType.PropertyReference ||
                     FieldType == CswEnumNbtFieldType.Static );
        }

        /// <summary>
        /// Indicates whether the field type's value should be allowed to be copied
        /// </summary>
        public bool IsCopyable()
        {
            return ( !IsDisplayType() &&
                     FieldType != CswEnumNbtFieldType.File &&
                     FieldType != CswEnumNbtFieldType.Image );
        }

        /// <summary>
        /// Indicates whether the field type can have a default value
        /// </summary>
        public bool CanHaveDefaultValue()
        {
            return ( !IsDisplayType() &&
                     FieldType != CswEnumNbtFieldType.ViewPickList &&
                     FieldType != CswEnumNbtFieldType.File &&
                     FieldType != CswEnumNbtFieldType.Image &&
                     FieldType != CswEnumNbtFieldType.Comments );
        }

        /// <summary>
        /// Returns whether the field type should show the property name as a label next to the value
        /// </summary>
        public bool ShowLabel()
        {
            return ( FieldType != CswEnumNbtFieldType.Grid &&
                     FieldType != CswEnumNbtFieldType.LocationContents &&
                     FieldType != CswEnumNbtFieldType.Static );
        }

        public bool IsLayoutCompatible( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            bool ret = true;
            if( LayoutType != CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
            {
                ret = ( FieldType != CswEnumNbtFieldType.Grid );
            }
            return ret;
        } // IsLayoutCompatible()


        #region IComparable

        public int CompareTo( object obj )
        {
            if( !( obj is CswNbtMetaDataFieldType ) )
                throw new ArgumentException( "object is not a CswNbtMetaDataFieldType" );

            CswNbtMetaDataFieldType CswNbtMetaDataFieldType = (CswNbtMetaDataFieldType) obj;
            return ( string.Compare( FieldType.ToString(), CswNbtMetaDataFieldType.FieldType.ToString() ) );

        }//CompareTo() 

        #endregion IComparable

        #region IEquatable

        public static bool operator ==( CswNbtMetaDataFieldType ft1, CswNbtMetaDataFieldType ft2 )
        {
            // If both are null, or both are same instance, return true.
            if( ReferenceEquals( ft1, ft2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) ft1 == null ) || ( (object) ft2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( ft1.UniqueId == ft2.UniqueId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtMetaDataFieldType ft1, CswNbtMetaDataFieldType ft2 )
        {
            return !( ft1 == ft2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtMetaDataFieldType ) )
                return false;
            return this == (CswNbtMetaDataFieldType) obj;
        }

        public bool Equals( CswNbtMetaDataFieldType obj )
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            return FieldType.GetHashCode();
        }

        #endregion IEquatable

    }
}
