using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
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
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.MultiList )
            {
                throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
											"CswNbtNodePropMultiList() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

			_ValueSubField = ( (CswNbtFieldTypeRuleMultiList) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).ValueSubField;

        }//generic

        private CswNbtSubField _ValueSubField;

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
                _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        private CswNbtNodeTypePropListOptions _CswNbtNodeTypePropListOptions = null;
        public CswNbtNodeTypePropListOptions Options
        {
            get
            {
                if( null == _CswNbtNodeTypePropListOptions )
                {
                    _CswNbtNodeTypePropListOptions = new CswNbtNodeTypePropListOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp.PropId );
                }

                return ( _CswNbtNodeTypePropListOptions );

            }//get

        }//Options

        public static string OptionTextField = "Text";
        public static string OptionValueField = "Value";

        override public void onNodePropRowFilled()
        {
        }


        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, _ValueSubField.ToXmlNodeName(), Value );
            CswXmlDocument.AppendXmlNode( ParentNode, "Options", Options.ToString() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _ValueSubField.ToXmlNodeName(true), Value ),
                new XElement( "options", Options.ToString() ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject.Add( new JProperty( _ValueSubField.ToXmlNodeName(true), Value ) );
            ParentObject.Add( new JProperty( "options", Options.ToString() ) );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Value = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _ValueSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _ValueSubField.ToXmlNodeName(true) ) )
            {
                Value = XmlNode.Element( _ValueSubField.ToXmlNodeName(true) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Value = CswTools.XmlRealAttributeName( PropRow[_ValueSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject.Property( _ValueSubField.ToXmlNodeName(true) ) )
            {
                Value = (string) JObject.Property( _ValueSubField.ToXmlNodeName(true) ).Value;
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
