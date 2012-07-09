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
    public class CswNbtNodePropMol : CswNbtNodeProp
    {

        public static implicit operator CswNbtNodePropMol( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsMol;
        }

        public CswNbtNodePropMol( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleMol) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _MolSubField = _FieldTypeRule.MolSubField;
        }
        private CswNbtFieldTypeRuleMol _FieldTypeRule;
        private CswNbtSubField _MolSubField;

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

        public string Mol
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _MolSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _MolSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        public string getLink()
        {
            string ret = string.Empty;
            if( JctNodePropId != Int32.MinValue && NodeId != null && NodeTypePropId != Int32.MinValue )
            {
                ret = "wsNBT.asmx/getBlob?mode=image&jctnodepropid=" + JctNodePropId + "&nodeid=" + NodeId + "&propid=" + NodeTypePropId;
            }
            return ret;
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode MolNode = CswXmlDocument.AppendXmlNode( ParentNode, _MolSubField.ToXmlNodeName() );
            CswXmlDocument.SetInnerTextAsCData( MolNode, Mol );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _MolSubField.ToXmlNodeName( true ), Mol ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_MolSubField.ToXmlNodeName( true )] = Mol;
            ParentObject["column"] = _MolSubField.Column.ToString().ToLower();
            ParentObject[CswNbtSubField.SubFieldName.Href.ToString().ToLower()] = getLink();
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Mol = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _MolSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _MolSubField.ToXmlNodeName( true ) ) )
            {
                Mol = XmlNode.Element( _MolSubField.ToXmlNodeName( true ) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Mol = CswTools.XmlRealAttributeName( PropRow[_MolSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_MolSubField.ToXmlNodeName( true )] )
            {
                Mol = JObject[_MolSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }//CswNbtNodePropMol

}//namespace ChemSW.Nbt.PropTypes
