using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropBlob: CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropBlob( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsBlob;
        }

        public CswNbtNodePropBlob( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FileNameSubField = ( (CswNbtFieldTypeRuleBlob) _FieldTypeRule ).FileNameSubField;
            _ContentTypeSubField = ( (CswNbtFieldTypeRuleBlob) _FieldTypeRule ).ContentTypeSubField;
            _DateModifiedSubField = ( (CswNbtFieldTypeRuleBlob) _FieldTypeRule ).DateModifiedSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _FileNameSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => FileName, x => FileName = CswConvert.ToString(x) ) );
            _SubFieldMethods.Add( _ContentTypeSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => ContentType, x => ContentType = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _DateModifiedSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => DateModified, x => DateModified = CswConvert.ToDateTime( x ) ) );
        }

        private CswNbtSubField _FileNameSubField;
        private CswNbtSubField _ContentTypeSubField;
        private CswNbtSubField _DateModifiedSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == FileName.Length );
            }
        }

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
                CswNbtSdBlobData sdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                return sdBlobData.GetFileName( this );
            }
            set
            {
                CswNbtSdBlobData sdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                sdBlobData.SetFileName( value, this );
                SyncGestalt();
            }
        }
        public string ContentType
        {
            get
            {
                return GetPropRowValue( _ContentTypeSubField );
            }
            set
            {
                SetPropRowValue( _ContentTypeSubField, value );
            }
        }


        public DateTime DateModified
        {
            get
            {
                return GetPropRowValueDate( _DateModifiedSubField );
            }
            set
            {
                if( DateTime.MinValue != value )
                {
                    SetPropRowValue( _DateModifiedSubField, value );
                }
                else
                {
                    SetPropRowValue( _DateModifiedSubField, DateTime.MinValue );
                }
            }
        } // DateModified

        public string Href
        {
            get { return getLink( JctNodePropId, NodeId ); }
        }

        public static string getLink( Int32 JctNodePropId, CswPrimaryKey NodeId, Int32 BlobDataId = Int32.MinValue, bool UseNodeTypeAsPlaceholder = false )
        {
            string ret = string.Empty;
            if( JctNodePropId != Int32.MinValue && NodeId != null )
            {
                ret = "Services/BlobData/getBlob?jctnodepropid=" + JctNodePropId + "&nodeid=" + NodeId.ToString() + "&blobdataid=" + BlobDataId + "&usenodetypeasplaceholder=" + UseNodeTypeAsPlaceholder.ToString();
            }
            return ret;
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ContentTypeSubField.ToXmlNodeName( true )] = ContentType;
            ParentObject[_FileNameSubField.ToXmlNodeName( true )] = FileName;
            ParentObject[CswEnumNbtSubFieldName.Href.ToString().ToLower()] = Href;
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
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, FileName );
        }

    }

}//namespace ChemSW.Nbt.PropTypes

