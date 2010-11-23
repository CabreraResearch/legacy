using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Nbt.MetaData;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropMemo : CswNbtNodeProp
    {

        public CswNbtNodePropMemo(CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp)
            : base(CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp)
        {
            _TextSubField = ( (CswNbtFieldTypeRuleMemo) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).TextSubField;
        }

        private CswNbtSubField _TextSubField;


        override public bool Empty
        {
            get
            {
                return (0 == Gestalt.Length);
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
                if (_CswNbtMetaDataNodeTypeProp.TextAreaRows == Int32.MinValue)
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
                if (_CswNbtMetaDataNodeTypeProp.TextAreaColumns == Int32.MinValue)
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
            CswXmlDocument.SetInnerTextAsCData( TextNode, Text );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _TextSubField.ToXmlNodeName() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
        }

    }//CswNbtNodePropMemo

}//namespace ChemSW.Nbt.PropTypes
