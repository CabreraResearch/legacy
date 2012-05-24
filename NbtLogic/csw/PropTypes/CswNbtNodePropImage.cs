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

    public class CswNbtNodePropImage : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropImage( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsImage;
        }

        public CswNbtNodePropImage( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleImage) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _FileNameSubField = _FieldTypeRule.FileNameSubField;
            _ContentTypeSubField = _FieldTypeRule.ContentTypeSubField;
        }
        private CswNbtFieldTypeRuleImage _FieldTypeRule;
        private CswNbtSubField _FileNameSubField;
        private CswNbtSubField _ContentTypeSubField;

        override public bool Empty
        {
            get
            {
                return ( string.Empty == _CswNbtNodePropData.GetPropRowValue( _FileNameSubField.Column ) ||
                         string.Empty == _CswNbtNodePropData.GetPropRowValue( _ContentTypeSubField.Column ) );
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        //public byte[] BlobData
        //{
        //    get
        //    {
        //        return _CswNbtNodePropData.Row["blobdata"] as byte[];
        //    }
        //    set
        //    {
        //        _CswNbtNodePropData.Row["blobdata"] = value;
        //    }
        //}
        public string FileName
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _FileNameSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _FileNameSubField.Column, value );
                _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, value );
            }
        }
        public string ContentType
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _ContentTypeSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ContentTypeSubField.Column, value );
            }
        }

        public Int32 Height
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaRows = value;
            //}
        }

        public Int32 Width
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaColumns = value;
            //}
        }

        public string ImageUrl
        {
            get
            {
                return getLink( JctNodePropId, NodeId, NodeTypePropId );
            }
        }

        public static string getLink( Int32 JctNodePropId, CswPrimaryKey NodeId, Int32 NodeTypePropId )
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
            CswXmlDocument.AppendXmlNode( ParentNode, _FileNameSubField.ToXmlNodeName(), FileName );
            CswXmlDocument.AppendXmlNode( ParentNode, _ContentTypeSubField.ToXmlNodeName(), ContentType );
            XmlNode ImageUrlNode = CswXmlDocument.AppendXmlNode( ParentNode, CswNbtSubField.SubFieldName.Href.ToString(), ImageUrl );
            CswXmlDocument.AppendXmlAttribute( ImageUrlNode, "width", Width.ToString() );
            CswXmlDocument.AppendXmlAttribute( ImageUrlNode, "height", Height.ToString() );

            // TODO: We need to figure out how we want to do this, for binary data
            // Handle blob data
            //CswTableCaddy JctCaddy = _CswNbtXmlDocServices.CswNbtResources.makeCswTableCaddy( "jct_nodes_props" );
            //JctCaddy.RequireOneRow = true;
            //JctCaddy.AllowBlobColumns = true;
            //JctCaddy.FilterColumn = "jctnodepropid";
            //DataTable JctTable = JctCaddy[CswNbtNodePropWrapper.JctNodePropId].Table;

            //if( !JctTable.Rows[0].IsNull( "blobdata" ) )
            //{
            //    byte[] BlobData = new byte[0];
            //    BlobData = JctTable.Rows[0]["blobdata"] as byte[];

            //    int ArraySize = BlobData.GetUpperBound( 0 );
            //    FileStream fs = new FileStream( _CswNbtXmlDocServices.DocumentPath + CswNbtNodePropWrapper.AsImage.FileName, FileMode.OpenOrCreate, FileAccess.Write );
            //    fs.Write( BlobData, 0, ArraySize );
            //    fs.Close();
            //}
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _FileNameSubField.ToXmlNodeName( true ), FileName ),
                            new XElement( _ContentTypeSubField.ToXmlNodeName( true ), ContentType ),
                            new XElement( CswNbtSubField.SubFieldName.Href.ToString(), ImageUrl,
                                new XElement( "width", Width.ToString() ),
                                new XElement( "height", Height.ToString() ) ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_FileNameSubField.ToXmlNodeName( true )] = FileName;
            ParentObject[_ContentTypeSubField.ToXmlNodeName( true )] = ContentType;
            ParentObject[CswNbtSubField.SubFieldName.Href.ToString().ToLower()] = ImageUrl;
            if( 0 < Width )
            {
                ParentObject["width"] = Width.ToString();
            }
            if( 0 < Height )
            {
                ParentObject["height"] = Height.ToString();
            }
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            ContentType = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _ContentTypeSubField.ToXmlNodeName( true ) );
            FileName = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _FileNameSubField.ToXmlNodeName( true ) );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _ContentTypeSubField.ToXmlNodeName( true ) ) )
            {
                ContentType = XmlNode.Element( _ContentTypeSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _FileNameSubField.ToXmlNodeName( true ) ) )
            {
                FileName = XmlNode.Element( _FileNameSubField.ToXmlNodeName( true ) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            ContentType = CswTools.XmlRealAttributeName( PropRow[_ContentTypeSubField.ToXmlNodeName()].ToString() );
            FileName = CswTools.XmlRealAttributeName( PropRow[_FileNameSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_ContentTypeSubField.ToXmlNodeName( true )] )
            {
                ContentType = JObject[_ContentTypeSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_FileNameSubField.ToXmlNodeName( true )] )
            {
                FileName = JObject[_FileNameSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }

}//namespace ChemSW.Nbt.PropTypes
