using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropImageList: CswNbtNodeProp
    {
        public static char Delimiter = '\n';

        public static implicit operator CswNbtNodePropImageList( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsImageList;
        }

        public CswNbtNodePropImageList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _ValueSubField = ( (CswNbtFieldTypeRuleImageList) _FieldTypeRule ).ValueSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _ValueSubField.Name, new Tuple<Func<dynamic>, Action<dynamic>>( () => Value, x => Value = new CswDelimitedString( x.ToString(), Delimiter ) ) );
        }

        private CswNbtSubField _ValueSubField;

        public bool AllowMultiple
        {
            //get { return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Extended ); }
            get { return CswConvert.ToBoolean( _CswNbtNodePropData[CswNbtFieldTypeRuleImageList.AttributeName.AllowMultipleValues] ); }
        }

        private string _ImagePrefix;
        public string ImagePrefix
        {
            get
            {
                if( string.IsNullOrEmpty( _ImagePrefix ) )
                {
                    //_ImagePrefix = CswConvert.ToString( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                    _ImagePrefix = _CswNbtNodePropData[CswNbtFieldTypeRuleImageList.AttributeName.Prefix];
                }
                return _ImagePrefix;
            }
            set { _ImagePrefix = value; }
        }

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length && Value.IsEmpty );
            }//
        }

        private CswDelimitedString _Value = null;
        public CswDelimitedString Value
        {
            get
            {
                if( _Value == null )
                {
                    _Value = new CswDelimitedString( Delimiter );
                    _Value.FromString( GetPropRowValue( _ValueSubField ) );
                }
                return _Value;
            }
            set
            {
                _Value = value;
                string ValString = value.ToString();
                SetPropRowValue( _ValueSubField, ValString );
                SyncGestalt();
            }
        }

        /// <summary>
        /// Add a value to the set of selected values
        /// </summary>
        public void AddValue( string ValueToAdd )
        {
            CswDelimitedString myValue = Value;
            myValue.Add( ValueToAdd );
            Value = myValue;
        }

        /// <summary>
        /// Check to see if a value is present
        /// </summary>
        public bool CheckValue( string ValueToCheck )
        {
            return Value.Contains( ValueToCheck );
        }

        /// <summary>
        /// Remove a value from the set of selected values
        /// </summary>
        public void RemoveValue( string ValueToRemove )
        {
            CswDelimitedString myValue = Value;
            myValue.Remove( ValueToRemove );
            Value = myValue;
        }




        private Dictionary<string, string> _Options = null;
        public Dictionary<string, string> Options
        {
            get
            {
                Dictionary<string, string> ret = null;
                if( _Options != null )
                {
                    ret = _Options;
                }
                else
                {
                    ret = new Dictionary<string, string>();
                    CswDelimitedString NameOptions = new CswDelimitedString( Delimiter );
                    CswDelimitedString ValueOptions = new CswDelimitedString( Delimiter );
                    //NameOptions.FromString( _CswNbtMetaDataNodeTypeProp.ListOptions.Trim() );
                    //ValueOptions.FromString( _CswNbtMetaDataNodeTypeProp.ValueOptions.Trim() );
                    NameOptions.FromString( _CswNbtNodePropData[CswNbtFieldTypeRuleImageList.AttributeName.ImageNames].Trim() );
                    ValueOptions.FromString( _CswNbtNodePropData[CswNbtFieldTypeRuleImageList.AttributeName.ImageUrls].Trim() );

                    for( Int32 i = 0; i < ValueOptions.Count; i++ )
                    {
                        string thisValue = ValueOptions[i];
                        string thisName = thisValue;
                        if( NameOptions.Count > i )
                        {
                            thisName = NameOptions[i];
                        }
                        ret.Add( thisValue, thisName );
                    }
                }
                return ret;
            } // get
            set
            {
                _Options = value;
            }
        } // Options

        public Int32 Height
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
                return CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleImageList.AttributeName.HeightInPixels] );
            }
        }

        public Int32 Width
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
                return CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleImageList.AttributeName.WidthInPixels] );
            }
        }

        public override void SyncGestalt()
        {
            CswDelimitedString NewGestalt = new CswDelimitedString( Delimiter );
            foreach( string Key in Value )
            {
                if( Options.ContainsKey( Key ) )
                {
                    NewGestalt.Add( Options[Key] );
                }
            }
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, NewGestalt.ToString() );
        } // _setGestalt()

        public static string OptionTextField = "Text";
        public static string OptionValueField = "Value";

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        override public void onNodePropRowFilled()
        {
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ValueSubField.ToXmlNodeName( true )] = Value.ToString();
            ParentObject["width"] = Width;
            ParentObject["height"] = Height;
            ParentObject["allowmultiple"] = AllowMultiple;
            ParentObject["imageprefix"] = ImagePrefix;

                // To preserve order of the displaying/printing of pictorgrams,
                // this must be an array.
                JArray OptionsArr = new JArray();
                ParentObject["options"] = OptionsArr;
            foreach( string Key in Options.Keys )
            {
                bool isSelected = Value.Contains( Key );
                if( IsEditModeEditable || isSelected )
                {
                    JObject Opt = new JObject();
                    Opt["text"] = Options[Key];
                    Opt["value"] = Key;
                    Opt["selected"] = isSelected;
                        
                    OptionsArr.Add( Opt );

                }
            }
        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswDelimitedString NewValue = new CswDelimitedString( Delimiter );
            NewValue.FromString( CswTools.XmlRealAttributeName( PropRow[_ValueSubField.ToXmlNodeName()].ToString() ) );
            Value = NewValue;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_ValueSubField.ToXmlNodeName( true )] )
            {
                CswDelimitedString NewValue = new CswDelimitedString( Delimiter );
                NewValue.FromString( JObject[_ValueSubField.ToXmlNodeName( true )].ToString() );

                if( false == AllowMultiple )
                {
                    string SingleValue = NewValue[0];
                    NewValue.Clear();
                    NewValue.Add( SingleValue );
                }
                Value = NewValue;
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
