using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropList : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropList( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsList;
        }

        public CswNbtNodePropList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _ValueSubField = ( (CswNbtFieldTypeRuleList) _FieldTypeRule ).ValueSubField;
            _TextSubField = ( (CswNbtFieldTypeRuleList) _FieldTypeRule ).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _ValueSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Value, x => Value = x ) );
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, null ) );
        }

        private CswNbtSubField _ValueSubField;
        private CswNbtSubField _TextSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length && string.Empty == Value );
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt

        public string Value
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _ValueSubField.Column );
            }
            set
            {
                CswNbtNodeTypePropListOption SelectedOption = Options.FindByValue( value );
                if( null != SelectedOption )
                {
                    _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, SelectedOption.Value );
                    _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, SelectedOption.Text );
                    _CswNbtNodePropData.Gestalt = SelectedOption.Text;
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, value );
                }
            }
        }

        public string Text
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
            }
            // set value, not text
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        private CswNbtNodeTypePropListOptions _CswNbtNodeTypePropListOptions = null;
        public CswNbtNodeTypePropListOptions Options
        {
            get
            {
                if( null == _CswNbtNodeTypePropListOptions )
                {
                    _CswNbtNodeTypePropListOptions = new CswNbtNodeTypePropListOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                }

                return ( _CswNbtNodeTypePropListOptions );

            }//get

        }//Options

        public static string OptionTextField = "Text";
        public static string OptionValueField = "Value";

        override public void onNodePropRowFilled()
        {
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ValueSubField.ToXmlNodeName( true )] = Value;
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;

            // Make sure the selected value is in the list of options (originally case 28020)
            JArray OptionsArr = new JArray();
            bool foundValue = false;
            foreach( CswNbtNodeTypePropListOption o in Options.Options )
            {
                foundValue = foundValue || ( o.Value == Value );
                JObject Opt = new JObject();
                Opt["text"] = o.Text;
                Opt["value"] = o.Value;
                OptionsArr.Add( Opt );
            }
            if( false == foundValue )
            {
                JObject Opt = new JObject();
                Opt["text"] = Text;
                Opt["value"] = Value;
                OptionsArr.Add( Opt );
            }
            ParentObject["options"] = OptionsArr;
        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Value = CswTools.XmlRealAttributeName( PropRow[_ValueSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_ValueSubField.ToXmlNodeName( true )] )
            {
                string selText = JObject[_ValueSubField.ToXmlNodeName( true )].ToString();

                // Decode the actual value from the option selected
                CswNbtNodeTypePropListOption selOption = Options.FindByText( selText );
                if( null != selOption )
                {
                    Value = selOption.Value;
                }
                else
                {
                    Value = selText;
                }
            }
        }

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Value );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
