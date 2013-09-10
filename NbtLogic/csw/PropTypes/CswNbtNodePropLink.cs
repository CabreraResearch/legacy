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
            _TextSubField = ( (CswNbtFieldTypeRuleLink) _FieldTypeRule ).TextSubField;
            _HrefSubField = ( (CswNbtFieldTypeRuleLink) _FieldTypeRule ).HrefSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, x => Text = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _HrefSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Href, x => Href = CswConvert.ToString( x ) ) );
        }

        private CswNbtSubField _TextSubField;
        private CswNbtSubField _HrefSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        public string Text
        {
            get
            {
                return GetPropRowValue( _TextSubField.Column );
            }
            set
            {
                SetPropRowValue( _TextSubField.Column, value );
                SetPropRowValue( CswEnumNbtPropColumn.Gestalt, value );
            }
        }

        public string Href
        {
            get
            {
                return GetPropRowValue( _HrefSubField.Column );
            }
            set
            {
                SetPropRowValue( _HrefSubField.Column, value );
            }
        }

        public static string GetFullURL( string Prefix, string HrefBody, string Suffix )
        {
            string fullUrl = HrefBody;
            if( false == String.IsNullOrEmpty( fullUrl ) )
            {
                fullUrl = Prefix + HrefBody + Suffix;
                if( false == Regex.IsMatch( fullUrl, @"^https?://.*" ) ) //if the hyperlink doesn't contain http:// or https://
                {
                    fullUrl = "http://" + fullUrl;
                }
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
            SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Text );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
