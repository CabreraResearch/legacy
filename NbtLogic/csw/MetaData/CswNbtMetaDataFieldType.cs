using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataFieldType : ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataFieldType>, IComparable
    {

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                               {
                                                                   { NbtFieldType.Barcode, NbtFieldType.Barcode },                          //public const string Barcode = "Barcode";
                                                                    { NbtFieldType.Button, NbtFieldType.Button }   ,                        //public const string Button = "Button";
                                                                    { NbtFieldType.Comments, NbtFieldType.Comments },                       //public const string Comments = "Comments";
                                                                    { NbtFieldType.Composite, NbtFieldType.Composite },                     //public const string Composite = "Composite";
                                                                    { NbtFieldType.DateTime, NbtFieldType.DateTime },                       //public const string DateTime = "DateTime";
                                                                    { NbtFieldType.External, NbtFieldType.External },                       //public const string External = "External";
                                                                    { NbtFieldType.File, NbtFieldType.File },                               //public const string File = "File";
                                                                    { NbtFieldType.Grid, NbtFieldType.Grid },                               //public const string Grid = "Grid";
                                                                    { NbtFieldType.Image, NbtFieldType.Image },                             //public const string Image = "Image";
                                                                    { NbtFieldType.ImageList, NbtFieldType.ImageList },                     //public const string ImageList = "ImageList";
                                                                    { NbtFieldType.Link, NbtFieldType.Link },                               //public const string Link = "Link";
                                                                    { NbtFieldType.List, NbtFieldType.List },                               //public const string List = "List";
                                                                    { NbtFieldType.Location, NbtFieldType.Location },                       //public const string Location = "Location";
                                                                    { NbtFieldType.LocationContents, NbtFieldType.LocationContents},        //public const string LocationContents = "LocationContents";
                                                                    { NbtFieldType.Logical, NbtFieldType.Logical },                         //public const string Logical = "Logical";
                                                                    { NbtFieldType.LogicalSet, NbtFieldType.LogicalSet },                   //public const string LogicalSet = "LogicalSet";
                                                                    { NbtFieldType.Memo, NbtFieldType.Memo },                               //public const string Memo = "Memo";
                                                                    { NbtFieldType.MOL, NbtFieldType.MOL },                                 //public const string MOL = "MOL";
                                                                    { NbtFieldType.MTBF, NbtFieldType.MTBF },                               //public const string MTBF = "MTBF";
                                                                    { NbtFieldType.MultiList, NbtFieldType.MultiList },                     //public const string MultiList = "MultiList";
                                                                    { NbtFieldType.NFPA,NbtFieldType. NFPA },                               //public const string NFPA = "NFPA";
                                                                    { NbtFieldType.NodeTypeSelect, NbtFieldType.NodeTypeSelect },           //public const string NodeTypeSelect = "NodeTypeSelect";
                                                                    { NbtFieldType.Number, NbtFieldType.Number },                           //public const string Number = "Number";
                                                                    { NbtFieldType.Password, NbtFieldType.Password },                       //public const string Password = "Password";
                                                                    { NbtFieldType.PropertyReference, NbtFieldType.PropertyReference },     //public const string PropertyReference = "PropertyReference";
                                                                    { NbtFieldType.Quantity, NbtFieldType.Quantity },                       //public const string Quantity = "Quantity";
                                                                    { NbtFieldType.Question, NbtFieldType.Question },                       //public const string Question = "Question";
                                                                    { NbtFieldType.Relationship, NbtFieldType.Relationship },               //public const string Relationship = "Relationship";
                                                                    { NbtFieldType.Scientific, NbtFieldType.Scientific },                   //public const string Scientific = "Scientific";
                                                                    { NbtFieldType.Sequence, NbtFieldType.Sequence },                       //public const string Sequence = "Sequence";
                                                                    { NbtFieldType.Static, NbtFieldType.Static },                           //public const string Static = "Static";
                                                                    { NbtFieldType.Text, NbtFieldType.Text },                               //public const string Text = "Text";
                                                                    { NbtFieldType.TimeInterval, NbtFieldType.TimeInterval },               //public const string TimeInterval = "TimeInterval";
                                                                    { NbtFieldType.UserSelect, NbtFieldType.UserSelect },                   //public const string UserSelect = "UserSelect";
                                                                    { NbtFieldType.ViewPickList, NbtFieldType.ViewPickList },               //public const string ViewPickList = "ViewPickList";
                                                                    { NbtFieldType.ViewReference,NbtFieldType.ViewReference}                //public const string ViewReference = "ViewReference";
                                                                   
                                                               };


        public sealed class NbtFieldType : IEquatable<NbtFieldType>
        {
            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswNbtResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }
            public NbtFieldType( string ItemName = CswNbtResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            public static implicit operator NbtFieldType( string Val )
            {
                return new NbtFieldType( Val );
            }
            public static implicit operator string( NbtFieldType item )
            {
                return item.Value;
            }

            public override string ToString()
            {
                return Value;
            }

            public const string Barcode = "Barcode";
            public const string Button = "Button";
            public const string Comments = "Comments";
            public const string Composite = "Composite";
            public const string DateTime = "DateTime";
            public const string External = "External";
            public const string File = "File";
            public const string Grid = "Grid";
            public const string Image = "Image";
            public const string ImageList = "ImageList";
            public const string Link = "Link";
            public const string List = "List";
            public const string Location = "Location";
            public const string LocationContents = "LocationContents";
            public const string Logical = "Logical";
            public const string LogicalSet = "LogicalSet";
            public const string Memo = "Memo";
            public const string MOL = "MOL";
            public const string MTBF = "MTBF";
            public const string MultiList = "MultiList";
            public const string NFPA = "NFPA";
            public const string NodeTypeSelect = "NodeTypeSelect";
            public const string Number = "Number";
            public const string Password = "Password";
            public const string PropertyReference = "PropertyReference";
            public const string Quantity = "Quantity";
            public const string Question = "Question";
            public const string Relationship = "Relationship";
            public const string Scientific = "Scientific";
            public const string Sequence = "Sequence";
            public const string Static = "Static";
            public const string Text = "Text";
            public const string TimeInterval = "TimeInterval";
            public const string UserSelect = "UserSelect";
            public const string ViewPickList = "ViewPickList";
            public const string ViewReference = "ViewReference";

            #region IEquatable (NbtFieldType)

            public static bool operator ==( NbtFieldType ft1, NbtFieldType ft2 )
            {
                //do a string comparison on the fieldtypes
                return ft1.ToString() == ft2.ToString();
            }

            public static bool operator !=( NbtFieldType ft1, NbtFieldType ft2 )
            {
                return !( ft1 == ft2 );
            }

            public override bool Equals( object obj )
            {
                if( !( obj is NbtFieldType ) )
                    return false;
                return this == (NbtFieldType) obj;
            }

            public bool Equals( NbtFieldType obj )
            {
                return this == obj;
            }

            /// <summary>
            /// Get Hash Code
            /// </summary>
            public override int GetHashCode()
            {
                int ret = 23, prime = 37;
                ret = ( ret * prime ) + Value.GetHashCode();
                ret = ( ret * prime ) + _Enums.GetHashCode();
                return ret;
            }

            #endregion IEquatable (NbtFieldType)

        };

        public enum DataType
        {
            UNKNOWN,
            XML,
            DOUBLE,
            INTEGER,
            BLOB,
            DATETIME,
            TEXT,
            STRING,
            CLOB,
            BOOLEAN
        }

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

        public NbtFieldType FieldType
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
                     FieldType != NbtFieldType.File &&
                     FieldType != NbtFieldType.Image );
        }

        /// <summary>
        /// Indicates whether the field type can have a default value
        /// </summary>
        public bool CanHaveDefaultValue()
        {
            return ( !IsDisplayType() &&
                     FieldType != NbtFieldType.ViewPickList &&
                     FieldType != NbtFieldType.File &&
                     FieldType != NbtFieldType.Image &&
                     FieldType != NbtFieldType.Comments );
        }

        /// <summary>
        /// Returns whether the field type should show the property name as a label next to the value
        /// </summary>
        public bool ShowLabel()
        {
            return ( FieldType != NbtFieldType.Grid &&
                     FieldType != NbtFieldType.LocationContents &&
                     FieldType != NbtFieldType.Static );
        }

        public bool IsLayoutCompatible( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            bool ret = true;
            if( LayoutType != CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
            {
                ret = ( FieldType != NbtFieldType.Grid );
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
