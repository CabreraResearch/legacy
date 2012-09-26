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


    public class CswNbtNodePropText : CswNbtNodeProp
    {

        public static implicit operator CswNbtNodePropText( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsText;
        }

        public CswNbtNodePropText( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleText) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _TextSubField = _FieldTypeRule.TextSubField;
        }

        private CswNbtFieldTypeRuleText _FieldTypeRule;
        private CswNbtSubField _TextSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

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
        public Int32 Size
        {
            get
            {
                if( false == String.IsNullOrEmpty( _CswNbtMetaDataNodeTypeProp.Attribute1 ) )
                    return CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                else
                    return 25;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.Length = value;
            //}
        }

        //private string _ElemName_Value = "Value";

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode TextNode = CswXmlDocument.AppendXmlNode( ParentNode, _TextSubField.ToXmlNodeName(), Text );
            CswXmlDocument.AppendXmlAttribute( TextNode, "size", Size.ToString() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _TextSubField.ToXmlNodeName( true ), Text,
                new XAttribute( "size", Size.ToString() ) ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
            ParentObject["size"] = Size.ToString();
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _TextSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _TextSubField.ToXmlNodeName( true ) ) )
            {
                Text = XmlNode.Element( _TextSubField.ToXmlNodeName( true ) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_TextSubField.ToXmlNodeName( true )] )
            {
                Text = JObject[_TextSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }//CswNbtNodePropText

}//namespace ChemSW.Nbt.PropTypes
