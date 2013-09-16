using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtFieldType : IEquatable<CswEnumNbtFieldType>, IComparable<CswEnumNbtFieldType>
    {

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
                {CswEnumNbtFieldType.Barcode, CswEnumNbtFieldType.Barcode}, //public const string Barcode = "Barcode";
                {CswEnumNbtFieldType.Button, CswEnumNbtFieldType.Button}, //public const string Button = "Button";
                {CswEnumNbtFieldType.CASNo, CswEnumNbtFieldType.CASNo}, //public const string CASNo = "CASNo";
                {CswEnumNbtFieldType.ChildContents, CswEnumNbtFieldType.ChildContents},
                {CswEnumNbtFieldType.Comments, CswEnumNbtFieldType.Comments}, //public const string Comments = "Comments";
                {CswEnumNbtFieldType.Composite, CswEnumNbtFieldType.Composite}, //public const string Composite = "Composite";
                {CswEnumNbtFieldType.DateTime, CswEnumNbtFieldType.DateTime}, //public const string DateTime = "DateTime";
                {CswEnumNbtFieldType.External, CswEnumNbtFieldType.External}, //public const string External = "External";
                {CswEnumNbtFieldType.File, CswEnumNbtFieldType.File}, //public const string File = "File";
                {CswEnumNbtFieldType.Grid, CswEnumNbtFieldType.Grid}, //public const string Grid = "Grid";
                {CswEnumNbtFieldType.Image, CswEnumNbtFieldType.Image}, //public const string Image = "Image";
                {CswEnumNbtFieldType.ImageList, CswEnumNbtFieldType.ImageList}, //public const string ImageList = "ImageList";
                {CswEnumNbtFieldType.Link, CswEnumNbtFieldType.Link}, //public const string Link = "Link";
                {CswEnumNbtFieldType.List, CswEnumNbtFieldType.List}, //public const string List = "List";
                {CswEnumNbtFieldType.Location, CswEnumNbtFieldType.Location}, //public const string Location = "Location";
                {CswEnumNbtFieldType.Logical, CswEnumNbtFieldType.Logical}, //public const string Logical = "Logical";
                {CswEnumNbtFieldType.LogicalSet, CswEnumNbtFieldType.LogicalSet}, //public const string LogicalSet = "LogicalSet";
                {CswEnumNbtFieldType.Memo, CswEnumNbtFieldType.Memo}, //public const string Memo = "Memo";
                {CswEnumNbtFieldType.MOL, CswEnumNbtFieldType.MOL}, //public const string MOL = "MOL";
                {CswEnumNbtFieldType.MTBF, CswEnumNbtFieldType.MTBF}, //public const string MTBF = "MTBF";
                {CswEnumNbtFieldType.MultiList, CswEnumNbtFieldType.MultiList}, //public const string MultiList = "MultiList";
                {CswEnumNbtFieldType.NFPA, CswEnumNbtFieldType.NFPA}, //public const string NFPA = "NFPA";
                {CswEnumNbtFieldType.NodeTypeSelect, CswEnumNbtFieldType.NodeTypeSelect}, //public const string NodeTypeSelect = "NodeTypeSelect";
                {CswEnumNbtFieldType.Number, CswEnumNbtFieldType.Number}, //public const string Number = "Number";
                {CswEnumNbtFieldType.Password, CswEnumNbtFieldType.Password}, //public const string Password = "Password";
                {CswEnumNbtFieldType.PropertyReference, CswEnumNbtFieldType.PropertyReference}, //public const string PropertyReference = "PropertyReference";
                {CswEnumNbtFieldType.Quantity, CswEnumNbtFieldType.Quantity}, //public const string Quantity = "Quantity";
                {CswEnumNbtFieldType.Question, CswEnumNbtFieldType.Question}, //public const string Question = "Question";
                {CswEnumNbtFieldType.Relationship, CswEnumNbtFieldType.Relationship}, //public const string Relationship = "Relationship";
                {CswEnumNbtFieldType.Scientific, CswEnumNbtFieldType.Scientific}, //public const string Scientific = "Scientific";
                {CswEnumNbtFieldType.Sequence, CswEnumNbtFieldType.Sequence}, //public const string Sequence = "Sequence";
                {CswEnumNbtFieldType.Static, CswEnumNbtFieldType.Static}, //public const string Static = "Static";
                {CswEnumNbtFieldType.Text, CswEnumNbtFieldType.Text}, //public const string Text = "Text";
                {CswEnumNbtFieldType.TimeInterval, CswEnumNbtFieldType.TimeInterval}, //public const string TimeInterval = "TimeInterval";
                {CswEnumNbtFieldType.UserSelect, CswEnumNbtFieldType.UserSelect}, //public const string UserSelect = "UserSelect";
                {CswEnumNbtFieldType.ViewPickList, CswEnumNbtFieldType.ViewPickList}, //public const string ViewPickList = "ViewPickList";
                {CswEnumNbtFieldType.ViewReference, CswEnumNbtFieldType.ViewReference} //public const string ViewReference = "ViewReference";

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
