using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropImage: CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropImage( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsImage;
        }

        public CswNbtNodePropImage( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
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

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
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
