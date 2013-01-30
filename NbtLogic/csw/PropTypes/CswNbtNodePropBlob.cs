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

    public class CswNbtNodePropBlob : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropBlob( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsBlob;
        }

        public CswNbtNodePropBlob( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleBlob) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _FileNameSubField = _FieldTypeRule.FileNameSubField;
            _ContentTypeSubField = _FieldTypeRule.ContentTypeSubField;
        }
        private CswNbtFieldTypeRuleBlob _FieldTypeRule;
        private CswNbtSubField _FileNameSubField;
        private CswNbtSubField _ContentTypeSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == FileName.Length );
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

        public string Href
        {
            get { return getLink( JctNodePropId, NodeId, NodeTypePropId ); }
        }

        public static string getLink( Int32 JctNodePropId, CswPrimaryKey NodeId, Int32 NodeTypePropId )
        {
            string ret = string.Empty;
            if( JctNodePropId != Int32.MinValue && NodeId != null && NodeTypePropId != Int32.MinValue )
            {
                ret = "wsNBT.asmx/getBlob?mode=doc&jctnodepropid=" + JctNodePropId + "&nodeid=" + NodeId + "&propid=" + NodeTypePropId;
            }
            return ret;
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ContentTypeSubField.ToXmlNodeName( true )] = ContentType;
            ParentObject[_FileNameSubField.ToXmlNodeName( true )] = FileName;
            ParentObject[CswNbtSubField.SubFieldName.Href.ToString().ToLower()] = Href;
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

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, FileName );
        }

    }

}//namespace ChemSW.Nbt.PropTypes

