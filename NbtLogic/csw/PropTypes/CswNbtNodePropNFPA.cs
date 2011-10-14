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

		public CswNbtNodePropNFPA( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
			: base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
		{
			_RedSubField = ( (CswNbtFieldTypeRuleNFPA) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).RedSubField;
			_YellowSubField = ( (CswNbtFieldTypeRuleNFPA) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).YellowSubField;
			_BlueSubField = ( (CswNbtFieldTypeRuleNFPA) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).BlueSubField;
			_WhiteSubField = ( (CswNbtFieldTypeRuleNFPA) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).WhiteSubField;
		}

		private CswNbtSubField _RedSubField;
		private CswNbtSubField _YellowSubField;
		private CswNbtSubField _BlueSubField;
		private CswNbtSubField _WhiteSubField;

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
			if( null != JObject.Property( _RedSubField.ToXmlNodeName( true ) ) )
			{
				Red = (string) JObject.Property( _RedSubField.ToXmlNodeName( true ) ).Value;
			}
			if( null != JObject.Property( _YellowSubField.ToXmlNodeName( true ) ) )
			{
				Yellow = (string) JObject.Property( _YellowSubField.ToXmlNodeName( true ) ).Value;
			}
			if( null != JObject.Property( _BlueSubField.ToXmlNodeName( true ) ) )
			{
				Blue = (string) JObject.Property( _BlueSubField.ToXmlNodeName( true ) ).Value;
			}
			if( null != JObject.Property( _WhiteSubField.ToXmlNodeName( true ) ) )
			{
				White = (string) JObject.Property( _WhiteSubField.ToXmlNodeName( true ) ).Value;
			}
		}
	}//CswNbtNodePropNFPA

}//namespace ChemSW.Nbt.PropTypes
