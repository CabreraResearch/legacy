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


    public class CswNbtNodePropNFPA : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropNFPA( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsNFPA;
        }

        public CswNbtNodePropNFPA( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _RedSubField = ( (CswNbtFieldTypeRuleNFPA) _FieldTypeRule ).RedSubField;
            _YellowSubField = ( (CswNbtFieldTypeRuleNFPA) _FieldTypeRule ).YellowSubField;
            _BlueSubField = ( (CswNbtFieldTypeRuleNFPA) _FieldTypeRule ).BlueSubField;
            _WhiteSubField = ( (CswNbtFieldTypeRuleNFPA) _FieldTypeRule ).WhiteSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _RedSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Red, x => Red = x ) );
            _SubFieldMethods.Add( _YellowSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Yellow, x => Yellow = x ) );
            _SubFieldMethods.Add( _BlueSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Blue, x => Blue = x ) );
            _SubFieldMethods.Add( _WhiteSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => White, x => White = x ) );
        }

        private CswNbtSubField _RedSubField;
        private CswNbtSubField _YellowSubField;
        private CswNbtSubField _BlueSubField;
        private CswNbtSubField _WhiteSubField;
        private CswEnumNbtNFPADisplayMode _DisplayMode;

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
            }//

        }//Gestalt

        public string Red
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _RedSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _RedSubField.Column, value );
                SyncGestalt();
            }
        }
        public string Yellow
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _YellowSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _YellowSubField.Column, value );
                SyncGestalt();
            }
        }
        public string Blue
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _BlueSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _BlueSubField.Column, value );
                SyncGestalt();
            }
        }
        public string White
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _WhiteSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _WhiteSubField.Column, value );
                SyncGestalt();
            }
        }

        public CswEnumNbtNFPADisplayMode DisplayMode
        {
            get
            {
                if( null == _DisplayMode )
                {
                    _DisplayMode = (CswEnumNbtNFPADisplayMode) _CswNbtMetaDataNodeTypeProp.Attribute1;
                }
                return _DisplayMode;
            }
        }

        public bool HideSpecial
        {
            get
            {
                return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute2 );
            }
        }

        public override void SyncGestalt()
        {
            string newGestalt = "Flammability: " + Red + ", ";
            newGestalt += "Reactivity: " + Yellow + ", ";
            newGestalt += "Health: " + Blue;
            if( White != string.Empty )
            {
                newGestalt += ", Special: " + White;
            }

            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, newGestalt );
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_RedSubField.ToXmlNodeName( true )] = Red;
            ParentObject[_YellowSubField.ToXmlNodeName( true )] = Yellow;
            ParentObject[_BlueSubField.ToXmlNodeName( true )] = Blue;
            ParentObject[_WhiteSubField.ToXmlNodeName( true )] = White;
            ParentObject["displaymode"] = DisplayMode.ToString();
            ParentObject["hidespecial"] = HideSpecial;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Red = CswTools.XmlRealAttributeName( PropRow[_RedSubField.ToXmlNodeName()].ToString() );
            Yellow = CswTools.XmlRealAttributeName( PropRow[_YellowSubField.ToXmlNodeName()].ToString() );
            Blue = CswTools.XmlRealAttributeName( PropRow[_BlueSubField.ToXmlNodeName()].ToString() );
            White = CswTools.XmlRealAttributeName( PropRow[_WhiteSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_RedSubField.ToXmlNodeName( true )] )
            {
                Red = JObject[_RedSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_YellowSubField.ToXmlNodeName( true )] )
            {
                Yellow = JObject[_YellowSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_BlueSubField.ToXmlNodeName( true )] )
            {
                Blue = JObject[_BlueSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_WhiteSubField.ToXmlNodeName( true )] )
            {
                White = JObject[_WhiteSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }//CswNbtNodePropNFPA

}//namespace ChemSW.Nbt.PropTypes
