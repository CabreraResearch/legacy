using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtFieldType : IEquatable<CswEnumNbtFieldType>, IComparable<CswEnumNbtFieldType>
    {

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
                { Barcode,                   Barcode },                        
                { Button,                    Button },                          
                { CASNo,                     CASNo },                            
                { ChildContents,             ChildContents },
                { Comments,                  Comments },                      
                { Composite,                 Composite },                    
                { DateTime,                  DateTime },                      
                { External,                  External },                      
                { File,                      File },                              
                { Grid,                      Grid },                              
                { Image,                     Image },                            
                { ImageList,                 ImageList },                    
                { Link,                      Link },                              
                { List,                      List },                              
                { Location,                  Location },                      
                { LocationContents,          LocationContents },      
                { Logical,                   Logical },                        
                { LogicalSet,                LogicalSet },                  
                { Memo,                      Memo },                              
                { MetaDataList,              MetaDataList },
                { MOL,                       MOL },                                
                { MTBF,                      MTBF },                              
                { MultiList,                 MultiList },                    
                { NFPA,                      NFPA },                              
                { NodeTypeSelect,            NodeTypeSelect },          
                { Number,                    Number },                          
                { Password,                  Password },                      
                { PropertyReference,         PropertyReference },    
                { Quantity,                  Quantity },                      
                { Question,                  Question },                      
                { Relationship,              Relationship },              
                { Scientific,                Scientific },                  
                { Sequence,                  Sequence },                      
                { Static,                    Static },                          
                { Text,                      Text },                              
                { TimeInterval,              TimeInterval },              
                { UserSelect,                UserSelect },                  
                { ViewPickList,              ViewPickList },              
                { ViewReference,             ViewReference }             
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
        public const string LocationContents = "LocationContents";
        public const string Logical = "Logical";
        public const string LogicalSet = "LogicalSet";
        public const string Memo = "Memo";
        public const string MetaDataList = "MetaDataList";
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
