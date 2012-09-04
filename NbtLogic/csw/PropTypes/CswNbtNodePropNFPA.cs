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


    public class CswNbtNodePropNFPA : CswNbtNodeProp
    {

        public sealed class NFPADisplayMode : CswEnum<NFPADisplayMode>
        {
            private NFPADisplayMode( string mode ) : base( mode ) { }
            public static IEnumerable<NFPADisplayMode> all { get { return All; } }
            public static explicit operator NFPADisplayMode( string str )
            {
                NFPADisplayMode ret = Parse( str );
                return ret ?? Diamond; //return the selected value, or Diamond if none
            }
            public static readonly NFPADisplayMode Linear = new NFPADisplayMode( "Linear" );
            public static readonly NFPADisplayMode Diamond = new NFPADisplayMode( "Diamond" );
        }

        public static implicit operator CswNbtNodePropNFPA( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsNFPA;
        }

        public CswNbtNodePropNFPA( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleNFPA) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _RedSubField = _FieldTypeRule.RedSubField;
            _YellowSubField = _FieldTypeRule.YellowSubField;
            _BlueSubField = _FieldTypeRule.BlueSubField;
            _WhiteSubField = _FieldTypeRule.WhiteSubField;
        }
        private CswNbtFieldTypeRuleNFPA _FieldTypeRule;
        private CswNbtSubField _RedSubField;
        private CswNbtSubField _YellowSubField;
        private CswNbtSubField _BlueSubField;
        private CswNbtSubField _WhiteSubField;
        private NFPADisplayMode _DisplayMode;
        private bool _DisplaySpecial;

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

        public string Red
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _RedSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _RedSubField.Column, value );
                _setGestalt();
            }
        }
        public string Yellow
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _YellowSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _YellowSubField.Column, value );
                _setGestalt();
            }
        }
        public string Blue
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _BlueSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _BlueSubField.Column, value );
                _setGestalt();
            }
        }
        public string White
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _WhiteSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _WhiteSubField.Column, value );
                _setGestalt();
            }
        }

        public NFPADisplayMode DisplayMode
        {
            get
            {
                if( null == _DisplayMode )
                {
                    _DisplayMode = (NFPADisplayMode) _CswNbtMetaDataNodeTypeProp.Attribute1;
                }
                return _DisplayMode;
            }
        }

        public bool HideSpecial
        {
            get
            {
                return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute2 );
            }
        }

        private void _setGestalt()
        {
            string newGestalt = "Flammability: " + Red + ", ";
            newGestalt += "Reactivity: " + Yellow + ", ";
            newGestalt += "Health: " + Blue + ", ";
            newGestalt += "Special: " + White;

            _CswNbtNodePropData.Gestalt = newGestalt;
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode RedNode = CswXmlDocument.AppendXmlNode( ParentNode, _RedSubField.ToXmlNodeName(), Red );
            XmlNode YellowNode = CswXmlDocument.AppendXmlNode( ParentNode, _YellowSubField.ToXmlNodeName(), Yellow );
            XmlNode BlueNode = CswXmlDocument.AppendXmlNode( ParentNode, _BlueSubField.ToXmlNodeName(), Blue );
            XmlNode WhiteNode = CswXmlDocument.AppendXmlNode( ParentNode, _WhiteSubField.ToXmlNodeName(), White );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _RedSubField.ToXmlNodeName( true ), Red ) );
            ParentNode.Add( new XElement( _YellowSubField.ToXmlNodeName( true ), Yellow ) );
            ParentNode.Add( new XElement( _BlueSubField.ToXmlNodeName( true ), Blue ) );
            ParentNode.Add( new XElement( _WhiteSubField.ToXmlNodeName( true ), White ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_RedSubField.ToXmlNodeName( true )] = Red;
            ParentObject[_YellowSubField.ToXmlNodeName( true )] = Yellow;
            ParentObject[_BlueSubField.ToXmlNodeName( true )] = Blue;
            ParentObject[_WhiteSubField.ToXmlNodeName( true )] = White;
            ParentObject["displaymode"] = DisplayMode.ToString();
            ParentObject["hidespecial"] = HideSpecial;
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Red = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _RedSubField.ToXmlNodeName() );
            Yellow = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _YellowSubField.ToXmlNodeName() );
            Blue = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _BlueSubField.ToXmlNodeName() );
            White = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _WhiteSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _RedSubField.ToXmlNodeName( true ) ) )
            {
                Red = XmlNode.Element( _RedSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _YellowSubField.ToXmlNodeName( true ) ) )
            {
                Yellow = XmlNode.Element( _YellowSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _BlueSubField.ToXmlNodeName( true ) ) )
            {
                Blue = XmlNode.Element( _BlueSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _WhiteSubField.ToXmlNodeName( true ) ) )
            {
                White = XmlNode.Element( _WhiteSubField.ToXmlNodeName( true ) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Red = CswTools.XmlRealAttributeName( PropRow[_RedSubField.ToXmlNodeName()].ToString() );
            Yellow = CswTools.XmlRealAttributeName( PropRow[_YellowSubField.ToXmlNodeName()].ToString() );
            Blue = CswTools.XmlRealAttributeName( PropRow[_BlueSubField.ToXmlNodeName()].ToString() );
            White = CswTools.XmlRealAttributeName( PropRow[_WhiteSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_RedSubField.ToXmlNodeName( true )] )
            {
                Red = JObject[_RedSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_YellowSubField.ToXmlNodeName( true )] )
            {
                Yellow = JObject[_YellowSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_BlueSubField.ToXmlNodeName( true )] )
            {
                Blue = JObject[_BlueSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_WhiteSubField.ToXmlNodeName( true )] )
            {
                White = JObject[_WhiteSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }//CswNbtNodePropNFPA

}//namespace ChemSW.Nbt.PropTypes
