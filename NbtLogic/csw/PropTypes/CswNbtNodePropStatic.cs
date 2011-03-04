using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropStatic : CswNbtNodeProp
    {
        public CswNbtNodePropStatic( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _TextSubField = ( (CswNbtFieldTypeRuleStatic) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).TextSubField;
        }

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
            }
        }//Gestalt

        public string StaticText
        {
            get
            {
                string PossibleValue = _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
                if( PossibleValue != string.Empty )
                    return PossibleValue;
                else
                    return _CswNbtMetaDataNodeTypeProp.StaticText;
            }
            set
            {
                if( value != _CswNbtMetaDataNodeTypeProp.StaticText )
                {
                    _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, value );
                    _CswNbtNodePropData.Gestalt = value;
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, string.Empty );
                    _CswNbtNodePropData.Gestalt = string.Empty;
                }
            }
        }

        public Int32 Rows
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaRows != Int32.MinValue )
                    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
                else
                    return Int32.MinValue;
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
                if( _CswNbtMetaDataNodeTypeProp.TextAreaColumns != Int32.MinValue )
                    return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
                else
                    return Int32.MinValue;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaColumns = value;
            //}
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode TextNode = CswXmlDocument.AppendXmlNode( ParentNode, _TextSubField.ToXmlNodeName(), StaticText );
            CswXmlDocument.AppendXmlAttribute( TextNode, "rows", Rows.ToString() );
            CswXmlDocument.AppendXmlAttribute( TextNode, "columns", Columns.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            StaticText = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _TextSubField.ToXmlNodeName() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            throw new NotImplementedException();
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            throw new NotImplementedException();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            StaticText = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
        }

    }//CswNbtNodePropStatic

}//namespace ChemSW.Nbt.PropTypes
