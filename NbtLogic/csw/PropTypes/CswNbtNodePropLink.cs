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

    public class CswNbtNodePropLink : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropLink( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsLink;
        }
        public CswNbtNodePropLink( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleLink) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _TextSubField = _FieldTypeRule.TextSubField;
            _HrefSubField = _FieldTypeRule.HrefSubField;
        }

        private CswNbtFieldTypeRuleLink _FieldTypeRule;
        private CswNbtSubField _TextSubField;
        private CswNbtSubField _HrefSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        public string Text
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, value );
                _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, value );
            }
        }
        public string Href
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _HrefSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _HrefSubField.Column, value );
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, _TextSubField.ToXmlNodeName(), Text );
            CswXmlDocument.AppendXmlNode( ParentNode, _HrefSubField.ToXmlNodeName(), Href );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _TextSubField.ToXmlNodeName( true ), Text ),
                new XElement( _HrefSubField.ToXmlNodeName( true ), Href ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_HrefSubField.ToXmlNodeName( true )] = Href;
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _TextSubField.ToXmlNodeName() );
            Href = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _HrefSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _TextSubField.ToXmlNodeName( true ) ) )
            {
                Text = XmlNode.Element( _TextSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _HrefSubField.ToXmlNodeName( true ) ) )
            {
                Href = XmlNode.Element( _HrefSubField.ToXmlNodeName( true ) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
            Href = CswTools.XmlRealAttributeName( PropRow[_HrefSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_HrefSubField.ToXmlNodeName( true )] )
            {
                Href = JObject[_HrefSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_TextSubField.ToXmlNodeName( true )] )
            {
                Text = JObject[_TextSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
