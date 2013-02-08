using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropLink : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropLink( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsLink;
        }
        public CswNbtNodePropLink( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleLink) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _TextSubField = _FieldTypeRule.TextSubField;
            _HrefSubField = _FieldTypeRule.HrefSubField;
        }

        private CswNbtFieldTypeRuleLink _FieldTypeRule;
        private CswNbtSubField _TextSubField;
        private CswNbtSubField _HrefSubField;

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

        public string Text
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, value );
                _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, value );
            }
        }

        public string Href
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _HrefSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _HrefSubField.Column, value );
            }
        }

        public static string GetFullURL( string Prefix, string HrefBody, string Suffix )
        {
            string fullUrl = Prefix + HrefBody + Suffix;
            if( false == String.IsNullOrEmpty( fullUrl ) && false == Regex.IsMatch( fullUrl, @"^https?://.*" ) ) //if the hyperlink doesn't contain http:// or https://
            {
                fullUrl = "http://" + fullUrl;
            }
            return fullUrl;
        }

        public string GetFullURL()
        {
            return GetFullURL( Prefix, Href, Suffix );
        }

        public string Prefix
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.Attribute1;
            }
            set
            {
                _CswNbtMetaDataNodeTypeProp.Attribute1 = value;
            }
        }

        public string Suffix
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.Attribute2;
            }
            set
            {
                _CswNbtMetaDataNodeTypeProp.Attribute2 = value;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_HrefSubField.ToXmlNodeName( true )] = Href;
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
            ParentObject["url"] = GetFullURL( Prefix, Href, Suffix );
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
            Href = CswTools.XmlRealAttributeName( PropRow[_HrefSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_HrefSubField.ToXmlNodeName( true )] )
            {
                Href = JObject[_HrefSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_TextSubField.ToXmlNodeName( true )] )
            {
                Text = JObject[_TextSubField.ToXmlNodeName( true )].ToString();
            }
        }

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Text );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
