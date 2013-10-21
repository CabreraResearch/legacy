using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtFieldType : IEquatable<CswEnumNbtFieldType>, IComparable<CswEnumNbtFieldType>
    {

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
                {CswEnumNbtFieldType.Barcode, CswEnumNbtFieldType.Barcode},
                {CswEnumNbtFieldType.Button, CswEnumNbtFieldType.Button},
                {CswEnumNbtFieldType.CASNo, CswEnumNbtFieldType.CASNo},
                {CswEnumNbtFieldType.ChildContents, CswEnumNbtFieldType.ChildContents},
                {CswEnumNbtFieldType.Comments, CswEnumNbtFieldType.Comments},
                {CswEnumNbtFieldType.Composite, CswEnumNbtFieldType.Composite},
                {CswEnumNbtFieldType.DateTime, CswEnumNbtFieldType.DateTime},
                {CswEnumNbtFieldType.External, CswEnumNbtFieldType.External},
                {CswEnumNbtFieldType.File, CswEnumNbtFieldType.File},
                {CswEnumNbtFieldType.Formula, CswEnumNbtFieldType.Formula},
                {CswEnumNbtFieldType.Grid, CswEnumNbtFieldType.Grid},
                {CswEnumNbtFieldType.Image, CswEnumNbtFieldType.Image},
                {CswEnumNbtFieldType.ImageList, CswEnumNbtFieldType.ImageList},
                {CswEnumNbtFieldType.Link, CswEnumNbtFieldType.Link},
                {CswEnumNbtFieldType.List, CswEnumNbtFieldType.List},
                {CswEnumNbtFieldType.Location, CswEnumNbtFieldType.Location},
                {CswEnumNbtFieldType.Logical, CswEnumNbtFieldType.Logical},
                {CswEnumNbtFieldType.LogicalSet, CswEnumNbtFieldType.LogicalSet},
                {CswEnumNbtFieldType.Memo, CswEnumNbtFieldType.Memo},
                {CswEnumNbtFieldType.MOL, CswEnumNbtFieldType.MOL},
                {CswEnumNbtFieldType.MTBF, CswEnumNbtFieldType.MTBF},
                {CswEnumNbtFieldType.MultiList, CswEnumNbtFieldType.MultiList},
                {CswEnumNbtFieldType.NFPA, CswEnumNbtFieldType.NFPA},
                {CswEnumNbtFieldType.NodeTypeSelect, CswEnumNbtFieldType.NodeTypeSelect},
                {CswEnumNbtFieldType.Number, CswEnumNbtFieldType.Number},
                {CswEnumNbtFieldType.Password, CswEnumNbtFieldType.Password},
                {CswEnumNbtFieldType.PropertyReference, CswEnumNbtFieldType.PropertyReference},
                {CswEnumNbtFieldType.Quantity, CswEnumNbtFieldType.Quantity},
                {CswEnumNbtFieldType.Question, CswEnumNbtFieldType.Question},
                {CswEnumNbtFieldType.Relationship, CswEnumNbtFieldType.Relationship},
                {CswEnumNbtFieldType.ReportLink, CswEnumNbtFieldType.ReportLink},
                {CswEnumNbtFieldType.Scientific, CswEnumNbtFieldType.Scientific},
                {CswEnumNbtFieldType.Sequence, CswEnumNbtFieldType.Sequence},
                {CswEnumNbtFieldType.Static, CswEnumNbtFieldType.Static},
                {CswEnumNbtFieldType.Text, CswEnumNbtFieldType.Text},
                {CswEnumNbtFieldType.TimeInterval, CswEnumNbtFieldType.TimeInterval},
                {CswEnumNbtFieldType.UserSelect, CswEnumNbtFieldType.UserSelect},
                {CswEnumNbtFieldType.ViewPickList, CswEnumNbtFieldType.ViewPickList},
                {CswEnumNbtFieldType.ViewReference, CswEnumNbtFieldType.ViewReference}
            };

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

        public CswEnumNbtFieldType( string ItemName = CswNbtResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        public static implicit operator CswEnumNbtFieldType( string Val )
        {
            return new CswEnumNbtFieldType( Val );
        }

        public static implicit operator string( CswEnumNbtFieldType item )
        {
            return item.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public const string Barcode = "Barcode";
        public const string Button = "Button";
        public const string CASNo = "CASNo";
        public const string ChildContents = "ChildContents";
        public const string Comments = "Comments";
        public const string Composite = "Composite";
        public const string DateTime = "DateTime";
        public const string External = "External";
        public const string File = "File";
        public const string Formula = "Formula";
        public const string Grid = "Grid";
        public const string Image = "Image";
        public const string ImageList = "ImageList";
        public const string Link = "Link";
        public const string List = "List";
        public const string Location = "Location";
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
        public const string ReportLink = "ReportLink";
        public const string Scientific = "Scientific";
        public const string Sequence = "Sequence";
        public const string Static = "Static";
        public const string Text = "Text";
        public const string TimeInterval = "TimeInterval";
        public const string UserSelect = "UserSelect";
        public const string ViewPickList = "ViewPickList";
        public const string ViewReference = "ViewReference";

        #region IEquatable (NbtFieldType)

        public static bool operator ==( CswEnumNbtFieldType ft1, CswEnumNbtFieldType ft2 )
        {
            //do a string comparison on the fieldtypes
            return ft1.ToString() == ft2.ToString();
        }

        public static bool operator !=( CswEnumNbtFieldType ft1, CswEnumNbtFieldType ft2 )
        {
            return !( ft1 == ft2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtFieldType ) )
                return false;
            return this == (CswEnumNbtFieldType) obj;
        }

        public bool Equals( CswEnumNbtFieldType obj )
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

        #region IComparable (NbtFieldType)

        public int CompareTo( CswEnumNbtFieldType other )
        {
            return this.ToString().CompareTo( other.ToString() );
        }

        #endregion IComparable (NbtFieldType)
    }
}
