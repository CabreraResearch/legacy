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

    public class CswNbtNodePropMemo : CswNbtNodeProp
    {

        public CswNbtNodePropMemo( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _TextSubField = ( (CswNbtFieldTypeRuleMemo) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).TextSubField;
        }

        private CswNbtSubField _TextSubField;


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
                return _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
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
            }
        }

        public Int32 Rows
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaRows == Int32.MinValue )
                    return 4;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaRows = value;
            //}
        }
        public Int32 Columns
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaColumns == Int32.MinValue )
                    return 40;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaColumns = value;
            //}
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode TextNode = CswXmlDocument.AppendXmlNode( ParentNode, _TextSubField.ToXmlNodeName() );
            CswXmlDocument.AppendXmlAttribute( TextNode, "rows", Rows.ToString() );
            CswXmlDocument.AppendXmlAttribute( TextNode, "columns", Columns.ToString() );
            CswXmlDocument.SetInnerTextAsCData( TextNode, Text );
        }

        public override void ToXElement( XElement ParentNode )
        {
            XElement TextNode = new XElement( _TextSubField.ToXmlNodeName( true ),
                                              new XElement( "rows", Rows.ToString() ),
                                              new XElement( "columns", Columns.ToString() ) ) { Value = Text };
            ParentNode.Add( TextNode );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject.Add( new JProperty( _TextSubField.ToXmlNodeName( true ), Text ) );
            ParentObject.Add( new JProperty( "rows", Rows.ToString() ) );
            ParentObject.Add( new JProperty( "columns", Columns.ToString() ) );
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
            if( null != JObject.Property( _TextSubField.ToXmlNodeName( true ) ) )
            {
                Text = (string) JObject.Property( _TextSubField.ToXmlNodeName( true ) ).Value;
            }
        }
    }//CswNbtNodePropMemo

}//namespace ChemSW.Nbt.PropTypes
