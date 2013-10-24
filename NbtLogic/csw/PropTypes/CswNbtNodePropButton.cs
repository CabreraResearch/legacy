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
            CswNbtFieldTypeRuleButton FieldTypeRule = (CswNbtFieldTypeRuleButton) _FieldTypeRule;
            _StateSubField = FieldTypeRule.StateSubField;
            _MenuOptionsSubField = FieldTypeRule.MenuOptionsSubField;
            _DisplayNameSubField = FieldTypeRule.DisplayNameField;
            _IconSubField = FieldTypeRule.IconSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _StateSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => State, x => State = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _MenuOptionsSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => MenuOptions, x => MenuOptions = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _DisplayNameSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => DisplayName, x => DisplayName = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _IconSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Icon, x => Icon = CswConvert.ToString( x ) ) );
        }

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
            get { return GetPropRowValue( _MenuOptionsSubField ); }
            set { SetPropRowValue( _MenuOptionsSubField, value ); }
        }

        public string State
        {
            get { return GetPropRowValue( _StateSubField ); }
            set { SetPropRowValue( _StateSubField, value ); }
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
                string Ret = GetPropRowValue( _DisplayNameSubField );
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
                    //SetPropRowValue( _DisplayNameSubField, Ret );
                }
                return Ret;
            }
            set
            {
                SetPropRowValue( _DisplayNameSubField, value );
            }
        }


        public string Icon
        {
            get
            {
                string Ret = GetPropRowValue( _IconSubField );
                if( string.IsNullOrEmpty( Ret ) && isThisTheSaveButton() )
                {
                    Ret = "save";
                    //Icon = Ret;
                }
                return Ret;
            }
            set { SetPropRowValue( _IconSubField, value ); }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST

            //TODO: when Case 27516 is complete, merge these two "JSON" methods
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
            //Case 29681, collolary to Case 27516. In table/grid views we don't have the full node. 
            //Guarantee that at least the same property structure is sent to the client.
            //TODO: when Case 27516 is complete, merge these two "JSON" methods
            ParentObject["confirmmessage"] = "";
            ParentObject["displayText"] = NodeTypeProp.PropName;
            ParentObject["icon"] = "";

            ParentObject["issaveprop"] = NodeTypeProp.IsSaveProp;
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
