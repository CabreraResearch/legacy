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

        public CswNbtNodePropLink( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _TextSubField = ( (CswNbtFieldTypeRuleLink) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).TextSubField;
            _HrefSubField = ( (CswNbtFieldTypeRuleLink) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).HrefSubField;
        }

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
                _CswNbtNodePropData.Gestalt = value;
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
            ParentNode.Add( new XElement( _TextSubField.ToXmlNodeName(), Text ),
                new XElement( _HrefSubField.ToXmlNodeName(), Href ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject.Add( new JProperty( _HrefSubField.ToXmlNodeName(), Href ) );
            ParentObject.Add( new JProperty( _TextSubField.ToXmlNodeName(), Text ) );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _TextSubField.ToXmlNodeName() );
            Href = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _HrefSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _TextSubField.ToXmlNodeName() ) )
            {
                Text = XmlNode.Element( _TextSubField.ToXmlNodeName() ).Value;
            }
            if( null != XmlNode.Element( _HrefSubField.ToXmlNodeName() ) )
            {
                Href = XmlNode.Element( _HrefSubField.ToXmlNodeName() ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
            Href = CswTools.XmlRealAttributeName( PropRow[_HrefSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject.Property( _HrefSubField.ToXmlNodeName() ) )
            {
                Href = (string) JObject.Property( _HrefSubField.ToXmlNodeName() ).Value;
            }
            if( null != JObject.Property( _TextSubField.ToXmlNodeName() ) )
            {
                Text = (string) JObject.Property( _TextSubField.ToXmlNodeName() ).Value;
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
