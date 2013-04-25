using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropButton: CswNbtNodeProp
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
            _DisplayNameSubField = _FieldTypeRule.DisplayNameField;
            _IconSubField = _FieldTypeRule.IconSubField;
        }

        private CswNbtFieldTypeRuleButton _FieldTypeRule;
        private CswNbtSubField _StateSubField;
        private CswNbtSubField _MenuOptionsSubField;
        private CswNbtSubField _DisplayNameSubField;
        private CswNbtSubField _IconSubField;

        /// <summary>
        /// Prevent inadvertant attempts to "ReadOnlify" a button. Buttons are either visible or not.
        /// </summary>
        public new void setReadOnly( bool value, bool SaveToDb )
        {
            setHidden( value: value, SaveToDb: SaveToDb );
        }

        /// <summary>
        /// Prevent inadvertant gets on "ReadOnly" on a button. Buttons are either visible or not.
        /// </summary>
        public new bool ReadOnly
        {
            get { return Hidden; }
        }

        public string Text
        {
            get
            {
                string Ret = _CswNbtMetaDataNodeTypeProp.StaticText;
                if( string.IsNullOrEmpty( Ret ) )
                {
                    Ret = PropName;
                }
                return Ret;
            }
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

        private bool isThisTheSaveButton()
        {
            bool Ret = false;
            if( this.ObjectClassPropId != Int32.MinValue )
            {
                CswNbtMetaDataObjectClassProp Ocp = _CswNbtResources.MetaData.getObjectClassProp( this.ObjectClassPropId );
                if( null != Ocp && Ocp.PropName == CswNbtObjClass.PropertyName.Save )
                {
                    Ret = true;
                }
            }
            return Ret;
        }

        public string DisplayName
        {
            get
            {
                string Ret = _CswNbtNodePropData.GetPropRowValue( _DisplayNameSubField.Column );
                if( string.IsNullOrEmpty( Ret ) )
                {
                    if( isThisTheSaveButton() )
                    {
                        Ret = "Save Changes";
                    }
                    else
                    {
                        Ret = Text;
                    }
                    DisplayName = Ret;
                }
                return Ret;
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _DisplayNameSubField.Column, value );
            }
        }


        public string Icon 
        {
            get
            {
                string Ret = _CswNbtNodePropData.GetPropRowValue( _IconSubField.Column );
                if( string.IsNullOrEmpty( Ret ) && isThisTheSaveButton() )
                {
                    Ret = "save";
                    Icon = Ret;
                }
                return Ret;
            }
            set { _CswNbtNodePropData.SetPropRowValue( _IconSubField.Column, value ); }
        }



        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            //ParentObject.Add( new JProperty( "text", Text ) );
            //ParentObject.Add( new JProperty( "mode", Mode.ToString().ToLower() ) );
            AsJSON( NodeTypeProp, ParentObject, MenuOptions, State );
            ParentObject["confirmmessage"] = ConfirmationDialogMessage;
            ParentObject["displayText"] = DisplayName;
            ParentObject["icon"] = Icon;
        }

        public static void AsJSON( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ParentObject, string MenuOptions, string SelectedText )
        {
            ParentObject["text"] = NodeTypeProp.StaticText;
            ParentObject["mode"] = NodeTypeProp.Extended.ToLower();
            ParentObject["buttonname"] = NodeTypeProp.PropName;
            if( NodeTypeProp.Extended.ToLower() == ButtonMode.menu )
            {
                ParentObject["menuoptions"] = MenuOptions;
            }
            else
            {
                ParentObject["menuoptions"] = string.Empty;
            }
            
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
