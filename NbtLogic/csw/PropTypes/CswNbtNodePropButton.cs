using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
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

        public CswNbtNodePropButton( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleButton) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _StateSubField = _FieldTypeRule.StateSubField;
            _MenuOptionsSubField = _FieldTypeRule.MenuOptionsSubField;

        }

        private CswNbtFieldTypeRuleButton _FieldTypeRule;
        private CswNbtSubField _StateSubField;
        private CswNbtSubField _MenuOptionsSubField;


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

        /// <summary>
        /// The message to display in the confirmation dialog.
        /// </summary>
        public string ConfirmationDialogMessage
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.ValueOptions;
            }
        }

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        /// <summary>
        /// Returns the MenuOptions of the button.
        /// </summary>
        public string MenuOptions
        {
            get { return _CswNbtNodePropData.GetPropRowValue( _MenuOptionsSubField.Column ); }
            set { _CswNbtNodePropData.SetPropRowValue( _MenuOptionsSubField.Column, value ); }
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

        public override void ToJSON( JObject ParentObject )
        {
            //ParentObject.Add( new JProperty( "text", Text ) );
            //ParentObject.Add( new JProperty( "mode", Mode.ToString().ToLower() ) );
            AsJSON( NodeTypeProp, ParentObject, MenuOptions, State );
            ParentObject["confirmmessage"] = ConfirmationDialogMessage;
        }

        public static void AsJSON( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ParentObject, string MenuOptions, string SelectedText )
        {
            ParentObject["text"] = NodeTypeProp.StaticText;
            ParentObject["mode"] = NodeTypeProp.Extended.ToLower();
            ParentObject["menuoptions"] = MenuOptions;
            ParentObject["selectedText"] = SelectedText;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing        
        }

        public override void SyncGestalt()
        {

        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
