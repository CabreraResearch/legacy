using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    [DataContract]
    public class CswNbtNodePropImage : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropImage( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsImage;
        }

        public CswNbtNodePropImage()
        {
        }

        public CswNbtNodePropImage( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            //_FileNameSubField = ( (CswNbtFieldTypeRuleImage) _FieldTypeRule ).FileNameSubField;
            //_ContentTypeSubField = ( (CswNbtFieldTypeRuleImage) _FieldTypeRule ).ContentTypeSubField;

            // No subfields
        }
        //private CswNbtSubField _FileNameSubField;
        //private CswNbtSubField _ContentTypeSubField;

        override public bool Empty
        {
            //TODO: check if there is any blob data here - if there are no blob_data rows for this prop, return true
            get
            {
                //return ( string.Empty == GetPropRowValue( _FileNameSubField.Column ) ||
                //          string.Empty == GetPropRowValue( _ContentTypeSubField.Column ) );
                return Images.Count > 0;
            }
        }

        private Collection<CswNbtSdBlobData.CswNbtBlob> _Images = null;
        [DataMember]
        public Collection<CswNbtSdBlobData.CswNbtBlob> Images
        {
            get
            {
                if( null == _Images || WasModified )
                {
                    _Images = new Collection<CswNbtSdBlobData.CswNbtBlob>();
                    if( null != _CswNbtResources ) //WCF getters must always be null safe
                    {
                        CswNbtSdBlobData sdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                        _Images = sdBlobData.GetImages( NodeId, JctNodePropId );
                    }
                }
                return _Images;
            }
            set
            {
                Collection<CswNbtSdBlobData.CswNbtBlob> IDoNothing = value; //have to use this to use the [DataContract] decoration...
            }
        }

        public void SetImages( string Date = "" )
        {
            CswNbtSdBlobData sdBlobData = new CswNbtSdBlobData( _CswNbtResources );
            _Images = sdBlobData.GetImages( NodeId, JctNodePropId, Date );
        }

        [DataMember]
        public Int32 Height
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( null != _CswNbtMetaDataNodeTypeProp )
                {
                    ret = _CswNbtMetaDataNodeTypeProp.TextAreaRows;
                }
                return ret;
            }
            set
            {
                int IDoNothing = value; //we have to have a setter to have a [DataMember] decoration
            }
        }

        [DataMember]
        public Int32 Width
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( null != _CswNbtMetaDataNodeTypeProp )
                {
                    ret = _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
                }
                return ret;
            }
            set
            {
                int IDoNothing = value; //we have to have a setter to have a [DataMember] decoration
            }
        }

        public Double MaxFiles
        {
            get { return CswConvert.ToDouble( _CswNbtMetaDataNodeTypeProp.MaxValue ); }
            set { _CswNbtMetaDataNodeTypeProp.MaxValue = value; }
        }

        public static string getLink( Int32 JctNodePropId, CswPrimaryKey NodeId, Int32 BlobDataId = Int32.MinValue )
        {
            return CswNbtNodePropBlob.getLink( JctNodePropId, NodeId, BlobDataId ); ;
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void ToJSON( JObject ParentObject )
        {
            ParentObject["width"] = ( Width > 0 ) ? Width : 0;
            ParentObject["height"] = ( Height > 0 ) ? Height : 0;
            ParentObject["placeholder"] = "Images/icons/300/_placeholder.gif";
            ParentObject["maxfiles"] = ( MaxFiles >= 1 ) ? MaxFiles : 1;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //ContentType = CswTools.XmlRealAttributeName( PropRow[_ContentTypeSubField.ToXmlNodeName()].ToString() );
            //FileName = CswTools.XmlRealAttributeName( PropRow[_FileNameSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //if( null != JObject[_ContentTypeSubField.ToXmlNodeName( true )] )
            //{
            //    ContentType = JObject[_ContentTypeSubField.ToXmlNodeName( true )].ToString();
            //}
            //if( null != JObject[_FileNameSubField.ToXmlNodeName( true )] )
            //{
            //    FileName = JObject[_FileNameSubField.ToXmlNodeName( true )].ToString();
            //}
        }

        public override void SyncGestalt()
        {
            CswCommaDelimitedString imageNames = new CswCommaDelimitedString();
            foreach( CswNbtSdBlobData.CswNbtBlob Image in Images )
            {
                imageNames.Add(Image.FileName);
            }
            SetPropRowValue( CswEnumNbtPropColumn.Gestalt, imageNames.ToString() );
        }
    }

}//namespace ChemSW.Nbt.PropTypes
