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

    public class CswNbtNodePropButton : CswNbtNodeProp
    {
        public sealed class ButtonMode
        {
            public const string button = "button";
            public const string link = "link";
            public const string menu = "menu";
        };

        public static implicit operator CswNbtNodePropButton( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsButton;
        }

        public CswNbtNodePropButton( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule  = (CswNbtFieldTypeRuleButton) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _StateSubField = _FieldTypeRule.StateSubField;
        }

        private CswNbtFieldTypeRuleButton _FieldTypeRule;
        private CswNbtSubField _StateSubField;

        public string Text
        {
            get { return _CswNbtMetaDataNodeTypeProp.StaticText; }
        }

        public string Mode
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.Extended;
            }
        }

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        private string _MenuOptions = string.Empty;
        /// <summary>
        /// Returns the ListOptions of the button or the in-memory MenuOptions. set{} only affects the in-memory object.
        /// </summary>
        public string MenuOptions
        {
            get
            {
                if( string.IsNullOrEmpty( _MenuOptions ) )
                {
                    _MenuOptions = _CswNbtMetaDataNodeTypeProp.ListOptions;
                }
                return _MenuOptions;
            }
            set { _MenuOptions = value; }
        }

        private string _SelectedMenuOption = string.Empty;
        
        /// <summary>
        /// Returns the StaticText of the button or the in-memory SelectedMenuOption. set{} only affects the in-memory object.
        /// </summary>
        public string SelectedMenuOption
        {
            get
            {
                if( string.IsNullOrEmpty(_SelectedMenuOption) )
                {
                    _SelectedMenuOption = Text;
                }
                return _SelectedMenuOption;
            }
            set { _SelectedMenuOption = value; }
        }

        public string State
        {
            get { return _CswNbtNodePropData.GetPropRowValue( _StateSubField.Column ); }
            set { _CswNbtNodePropData.SetPropRowValue( _StateSubField.Column, value ); }
        }

        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode TextNode = CswXmlDocument.AppendXmlNode( ParentNode, "text", Text );
            CswXmlDocument.AppendXmlAttribute( TextNode, "mode", Mode.ToString() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( "text", Text,
                new XAttribute( "mode", Mode.ToString() ) ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            //ParentObject.Add( new JProperty( "text", Text ) );
            //ParentObject.Add( new JProperty( "mode", Mode.ToString().ToLower() ) );
            AsJSON( NodeTypeProp, ParentObject );
            ParentObject["state"] = State;
        }

        public static void AsJSON( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ParentObject )
        {
            ParentObject["text"] = NodeTypeProp.StaticText;
            ParentObject["mode"] = NodeTypeProp.Extended.ToLower();
            ParentObject["menuoptions"] = NodeTypeProp.ListOptions;
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing to do here
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            //nothing to do here
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing        
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
