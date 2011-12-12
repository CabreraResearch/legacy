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

    public class CswNbtNodePropButton : CswNbtNodeProp
    {
        public enum ButtonMode { button, link };
        private CswNbtSubField _TextSubField;

        public CswNbtNodePropButton( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _TextSubField = ( (CswNbtFieldTypeRuleButton) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).TextSubField;
        }

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

        public string Mode
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.Extended.ToString();
            }
        }

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


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode TextNode = CswXmlDocument.AppendXmlNode( ParentNode, "text", Text );
            CswXmlDocument.AppendXmlAttribute( TextNode, "mode", Mode.ToString() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( "text", Text,
                new XAttribute( "mode", Mode.ToString() ) ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject.Add( new JProperty( "text", Text ) );
            ParentObject.Add( new JProperty( "mode", Mode.ToString().ToLower() ) );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing to do here
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            //nothing to do here
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing        
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
