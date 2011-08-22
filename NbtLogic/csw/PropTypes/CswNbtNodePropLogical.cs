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

    public class CswNbtNodePropLogical : CswNbtNodeProp
    {

        public CswNbtNodePropLogical( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _CheckedSubField = ( (CswNbtFieldTypeRuleLogical) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CheckedSubField;
        }

        private CswNbtSubField _CheckedSubField;

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

        public Tristate Checked
        {
            get
            {
                return CswConvert.ToTristate( _CswNbtNodePropData.GetPropRowValue( _CheckedSubField.Column ) );
            }
            set
            {
                object val = CswConvert.ToDbVal( value );
                if( val != DBNull.Value )
                {
                    _CswNbtNodePropData.SetPropRowValue( _CheckedSubField.Column, val );
                    _CswNbtNodePropData.Gestalt = CswConvert.ToDisplayString( value );
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _CheckedSubField.Column, string.Empty );
                    _CswNbtNodePropData.Gestalt = string.Empty;
                }
            }
        }


        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, _CheckedSubField.ToXmlNodeName(), Checked.ToString().ToLower() );
            CswXmlDocument.AppendXmlNode( ParentNode, CswNbtSubField.SubFieldName.Required.ToString(), Required.ToString().ToLower() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _CheckedSubField.ToXmlNodeName( true ), Checked.ToString().ToLower() ),
                new XElement( CswNbtSubField.SubFieldName.Required.ToString(), Required.ToString().ToLower() ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_CheckedSubField.ToXmlNodeName( true )] = Checked.ToString().ToLower();
            ParentObject[CswNbtSubField.SubFieldName.Required.ToString()] = Required.ToString().ToLower();
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Checked = CswConvert.ToTristate( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _CheckedSubField.ToXmlNodeName() ) );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _CheckedSubField.ToXmlNodeName( true ) ) )
            {
                Checked = CswConvert.ToTristate( XmlNode.Element( _CheckedSubField.ToXmlNodeName( true ) ).Value );
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Checked = CswConvert.ToTristate( PropRow[_CheckedSubField.ToXmlNodeName()] );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject.Property( _CheckedSubField.ToXmlNodeName( true ) ) )
            {
                Checked = CswConvert.ToTristate( JObject.Property( _CheckedSubField.ToXmlNodeName( true ) ).Value );
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
