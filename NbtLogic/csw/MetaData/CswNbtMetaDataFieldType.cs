using System;
using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataFieldType : ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataFieldType>, IComparable
    {
        public enum NbtFieldType
        {
            Unknown,
            Barcode,
            Composite,
            DateTime,
            External,
            File,
            Grid,
            Image,
            Link,
            List,
            Location,
            LocationContents,
            Logical,
            LogicalSet,
            Memo,
            MOL,
            MTBF,
            //MultiRelationship,
            MultiList,
            //NodeTypePermissions,
            NodeTypeSelect,
            Number,
            Password,
            PropertyReference,
            Quantity,
            Question,
            Relationship,
            Scientific,
            Sequence,
            Static,
            Text,
            //Time,
            TimeInterval,
            UserSelect,
            ViewPickList,
            ViewReference
        };

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

        public static NbtFieldType getFieldTypeFromString( string FieldTypeName )
        {
            NbtFieldType Ret = NbtFieldType.Unknown;
            NbtFieldType.TryParse( FieldTypeName, true, out Ret );
            return Ret;
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

        public NbtFieldType FieldType
        {
            get
            {
                NbtFieldType Ret = NbtFieldType.Unknown;
                NbtFieldType.TryParse( _FieldTypeRow["fieldtype"].ToString(), true, out Ret );
                return Ret;
            }
        }

        /// <summary>
        /// Returns whether the field type should be considered a "simple" type,
        /// meaning that it is not a fancy-schmancy UI control
        /// </summary>
        public bool IsSimpleType()
        {
            return ( FieldType == NbtFieldType.DateTime ||
                //FieldType == NbtFieldType.Time ||
                     FieldType == NbtFieldType.List ||
                     FieldType == NbtFieldType.Logical ||
                     FieldType == NbtFieldType.Memo ||
                     FieldType == NbtFieldType.Number ||
                     FieldType == NbtFieldType.Static ||
                     FieldType == NbtFieldType.Text );
        }

        /// <summary>
        /// Returns whether the field type only displays a computed value, 
        /// rather than storing a user-specified value
        /// </summary>
        public bool IsDisplayType()
        {
            return ( FieldType == NbtFieldType.Composite ||
                     FieldType == NbtFieldType.External ||
                     FieldType == NbtFieldType.Grid ||
                     FieldType == NbtFieldType.LocationContents ||
                     FieldType == NbtFieldType.PropertyReference ||
                     FieldType == NbtFieldType.Static );
        }

        /// <summary>
        /// Indicates whether the field type's value should be allowed to be copied
        /// </summary>
        public bool IsCopyable()
        {
            return ( !IsDisplayType() &&
                     FieldType != CswNbtMetaDataFieldType.NbtFieldType.File &&
                     FieldType != CswNbtMetaDataFieldType.NbtFieldType.Image );
        }

        /// <summary>
        /// Indicates whether the field type can have a default value
        /// </summary>
        public bool CanHaveDefaultValue()
        {
            return ( !IsDisplayType() &&
                     FieldType != CswNbtMetaDataFieldType.NbtFieldType.ViewPickList &&
                     FieldType != CswNbtMetaDataFieldType.NbtFieldType.File &&
                     FieldType != CswNbtMetaDataFieldType.NbtFieldType.Image );
        }

        /// <summary>
        /// Returns whether the field type should show the property name as a label next to the value
        /// </summary>
        public bool ShowLabel()
        {
            return ( FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid &&
                     FieldType != CswNbtMetaDataFieldType.NbtFieldType.LocationContents &&
                     FieldType != CswNbtMetaDataFieldType.NbtFieldType.Static );
        }

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
            if( System.Object.ReferenceEquals( ft1, ft2 ) )
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
            return this == (CswNbtMetaDataFieldType) obj;
        }

        public override int GetHashCode()
        {
            return this.FieldType.GetHashCode();
        }

        #endregion IEquatable

    }
}
