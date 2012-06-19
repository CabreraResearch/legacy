using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataFieldType : ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataFieldType>, IComparable
    {

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                               {
                                                                   { NbtFieldType.Barcode, NbtFieldType.Barcode },
                                                                    { NbtFieldType.Button, NbtFieldType.Button }   ,
                                                                    { NbtFieldType.Comments, NbtFieldType.Comments },
                                                                    { NbtFieldType.Composite, NbtFieldType.Composite },
                                                                    { NbtFieldType.DateTime, NbtFieldType.DateTime },
                                                                    { NbtFieldType.External, NbtFieldType.External },
                                                                    { NbtFieldType.File, NbtFieldType.File },
                                                                    { NbtFieldType.Grid, NbtFieldType.Grid },
                                                                    { NbtFieldType.Image, NbtFieldType.Image },
                                                                    { NbtFieldType.ImageList, NbtFieldType.ImageList },
                                                                    { NbtFieldType.Link, NbtFieldType.Link },
                                                                    { NbtFieldType.List, NbtFieldType.List },
                                                                    { NbtFieldType.Location, NbtFieldType.Location },
                                                                    { NbtFieldType.LocationContents, NbtFieldType.LocationContents},
                                                                    { NbtFieldType.Logical, NbtFieldType.Logical },
                                                                    { NbtFieldType.LogicalSet, NbtFieldType.LogicalSet },
                                                                    { NbtFieldType.Memo, NbtFieldType.Memo },
                                                                    { NbtFieldType.MOL, NbtFieldType.MOL },
                                                                    { NbtFieldType.MTBF, NbtFieldType.MTBF },
                                                                    { NbtFieldType.MultiList, NbtFieldType.MultiList },
                                                                    { NbtFieldType.NFPA,NbtFieldType. NFPA },
                                                                    { NbtFieldType.NodeTypeSelect, NbtFieldType.NodeTypeSelect },
                                                                    { NbtFieldType.Number, NbtFieldType.Number },
                                                                    { NbtFieldType.Password, NbtFieldType.Password },
                                                                    { NbtFieldType.PropertyReference, NbtFieldType.PropertyReference },
                                                                    { NbtFieldType.Quantity, NbtFieldType.Quantity },
                                                                    { NbtFieldType.Question, NbtFieldType.Question },
                                                                    { NbtFieldType.Relationship, NbtFieldType.Relationship },
                                                                    { NbtFieldType.Scientific, NbtFieldType.Scientific },
                                                                    { NbtFieldType.Sequence, NbtFieldType.Sequence },
                                                                    { NbtFieldType.Static, NbtFieldType.Static },
                                                                    { NbtFieldType.Text, NbtFieldType.Text },
                                                                    { NbtFieldType.TimeInterval, NbtFieldType.TimeInterval },
                                                                    { NbtFieldType.UserSelect, NbtFieldType.UserSelect },
                                                                    { NbtFieldType.ViewPickList, NbtFieldType.ViewPickList },
                                                                    { NbtFieldType.ViewReference,NbtFieldType.ViewReference}
                                                                    
                                                               };


        public sealed class NbtFieldType
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
            //public const string MultiRelationship= "";
            public const string MultiList = "MultiList";
            //public const string //NodeTypePermissions= "";
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
            //public const string //Time= "";
            public const string TimeInterval = "TimeInterval";
            public const string UserSelect = "UserSelect";
            public const string ViewPickList = "ViewPickList";
            public const string ViewReference = "ViewReference";

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
            switch( LayoutType )
            {
                case CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add:
                    ret = ( FieldType != NbtFieldType.Grid );
                    break;
                case CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit:
                    break;
                case CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview:
                    ret = ( FieldType != NbtFieldType.Grid );
                    break;
                case CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table:
                    ret = ( FieldType != NbtFieldType.Grid );
                    break;
                default:
                    throw new CswDniException( ErrorType.Error, "Unrecognized Layout Type", "CswNbtMetaDataFieldType.IsLayoutCompatible has not been updated with the following layout: " + LayoutType.ToString() );
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
