using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropMultiList : CswNbtNodeProp
    {

        public CswNbtNodePropMultiList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            //if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.MultiList )
            //{
            //    throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
            //                                "CswNbtNodePropMultiList() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            //}
            _FieldTypeRule = (CswNbtFieldTypeRuleMultiList) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _ValueSubField = _FieldTypeRule.ValueSubField;

        }//generic
        private CswNbtFieldTypeRuleMultiList _FieldTypeRule;
        private CswNbtSubField _ValueSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length && Value.IsEmpty );
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt

        private CswCommaDelimitedString _Value = null;
        public CswCommaDelimitedString Value
        {
            get
            {
                if( _Value == null )
                {
                    _Value = new CswCommaDelimitedString();
                    _Value.FromString( _CswNbtNodePropData.GetPropRowValue( _ValueSubField.Column ) );
                }
                return _Value;
            }
            set
            {
                _Value = value;
                string ValString = value.ToString();
                _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, ValString );
                _setGestalt();
            }
        }

        /// <summary>
        /// Add a value to the set of selected values
        /// </summary>
        public void AddValue( string ValueToAdd )
        {
            CswCommaDelimitedString myValue = Value;
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
        /// Checks to be sure all values assigned are unique and valid against possible options
        /// </summary>
        public void ValidateValues()
        {
            CswCommaDelimitedString newVals = new CswCommaDelimitedString();

            foreach( string aval in Value )
            {
                if( Options.ContainsKey( aval ) && false == newVals.Contains( aval ) )
                {
                    newVals.Add( aval );
                }
            }
            Value = newVals;

        } // ValidateValues() 

        /// <summary>
        /// Remove a value from the set of selected values
        /// </summary>
        public void RemoveValue( string ValueToRemove )
        {
            CswCommaDelimitedString myValue = Value;
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
                    CswCommaDelimitedString ListOptions = new CswCommaDelimitedString();
                    ListOptions.FromString( _CswNbtMetaDataNodeTypeProp.ListOptions );
                    foreach( string ListOption in ListOptions )
                    {
                        ret.Add( ListOption, ListOption );
                    }
                }
                return ret;
            } // get
            set
            {
                _Options = value;
            }
        } // Options

        private void _setGestalt()
        {
            CswCommaDelimitedString NewGestalt = new CswCommaDelimitedString();
            foreach( string Key in this.Value )
            {
                if( Options.ContainsKey( Key ) )
                {
                    NewGestalt.Add( Options[Key] );
                }
            }
            _CswNbtNodePropData.Gestalt = NewGestalt.ToString();
        } // _setGestalt()

        public static string OptionTextField = "Text";
        public static string OptionValueField = "Value";

        override public void onNodePropRowFilled()
        {
        }


        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, _ValueSubField.ToXmlNodeName(), Value.ToString() );
            XmlNode OptionsNode = CswXmlDocument.AppendXmlNode( ParentNode, "Options" );
            foreach( string Key in this.Options.Keys )
            {
                XmlNode OptionNode = CswXmlDocument.AppendXmlNode( OptionsNode, "Option" );
                CswXmlDocument.AppendXmlAttribute( OptionNode, "value", Key );
                CswXmlDocument.AppendXmlAttribute( OptionNode, "text", Options[Key] );
                CswXmlDocument.AppendXmlAttribute( OptionNode, "selected", Value.Contains( Key ).ToString().ToLower() );
            }
        }

        public override void ToXElement( XElement ParentNode )
        {
            //ParentNode.Add( new XElement( _ValueSubField.ToXmlNodeName(true), Value ),
            //    new XElement( "options", Options.ToString() ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ValueSubField.ToXmlNodeName( true )] = Value.ToString();

            JObject OptionsObj = new JObject();
            ParentObject["options"] = OptionsObj;

            foreach( string Key in Options.Keys )
            {
                OptionsObj[Key] = new JObject();
                OptionsObj[Key]["text"] = Options[Key];
                OptionsObj[Key]["value"] = Key;
                OptionsObj[Key]["selected"] = Value.Contains( Key ).ToString().ToLower();
            }
        } // ToJSON()

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswCommaDelimitedString NewValue = new CswCommaDelimitedString();
            NewValue.FromString( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _ValueSubField.ToXmlNodeName() ) );
            Value = NewValue;
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            //if( null != XmlNode.Element( _ValueSubField.ToXmlNodeName(true) ) )
            //{
            //    Value = XmlNode.Element( _ValueSubField.ToXmlNodeName(true) ).Value;
            //}
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswCommaDelimitedString NewValue = new CswCommaDelimitedString();
            NewValue.FromString( CswTools.XmlRealAttributeName( PropRow[_ValueSubField.ToXmlNodeName()].ToString() ) );
            Value = NewValue;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_ValueSubField.ToXmlNodeName( true )] )
            {
                CswCommaDelimitedString NewValue = new CswCommaDelimitedString();
                NewValue.FromString( JObject[_ValueSubField.ToXmlNodeName( true )].ToString() );
                Value = NewValue;
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
