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

    public class CswNbtNodePropList : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropList( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsList;
        }

        public CswNbtNodePropList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _ValueSubField = ( (CswNbtFieldTypeRuleList) _FieldTypeRule ).ValueSubField;
            _TextSubField = ( (CswNbtFieldTypeRuleList) _FieldTypeRule ).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _ValueSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Value, x => Value = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, null ) );

            _SearchThreshold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.relationshipoptionlimit.ToString() ) );
            if( _SearchThreshold <= 0 )
            {
                _SearchThreshold = 100;
            }
        }

        private CswNbtSubField _ValueSubField;
        private CswNbtSubField _TextSubField;

        private Int32 _SearchThreshold;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length && string.Empty == Value );
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt

        public string Value
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _ValueSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, value );
                CswNbtNodeTypePropListOption SelectedOption = Options.FindByValue( value );
                if( null != SelectedOption )
                {
                    Text = SelectedOption.Text;
                }
            }
        }

        /// <summary>
        /// Overwrites the selected text.  Consider setting the Value instead (which will set the Text)
        /// </summary>
        public string Text
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public delegate CswNbtNodeTypePropListOptions InitOptionsHandler();
        public InitOptionsHandler InitOptions = null;

        private CswNbtNodeTypePropListOptions _Options = null;
        public CswNbtNodeTypePropListOptions Options
        {
            get
            {

                if( _Options == null )
                {
                    if( InitOptions != null )
                    {
                        // Override, usually from CswNbtObjClass*
                        _Options = InitOptions();
                    }
                    if( _Options == null )
                    {
                        // Default
                        _Options = new CswNbtNodeTypePropListOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                    }
                }
                return ( _Options );
            }//get
        }//Options

        public delegate void FilterOptionsHandler( string SearchTerm, Int32 SearchThreshold );
        public FilterOptionsHandler OnBeforeFilterOptions = null;

        public void filterOptions( string SearchTerm )
        {
            // If the delegate isn't null, then execute it!
            if( null != OnBeforeFilterOptions )
            {
                OnBeforeFilterOptions( SearchTerm, _SearchThreshold );
            }

            for( int i = Options.Options.Count - 1; i >= 0; i-- )
            {
                if( false == Options.Options[i].Text.ToLower().Contains( SearchTerm.ToLower() ) )
                {
                    Options.Options.RemoveAt( i );
                }
            }

        }//filterOptions()


        public static string OptionTextField = "Text";
        public static string OptionValueField = "Value";

        override public void onNodePropRowFilled()
        {
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
            ParentObject[_ValueSubField.ToXmlNodeName( true )] = Value;
            ParentObject["search"] = false;

            if( Options.Options.Count <= _SearchThreshold )
            {
                // Make sure the selected value is in the list of options (originally case 28020)
                // TODO: When we use WCF, we can just serialize Options directly
                JArray OptionsArr = new JArray();
                bool foundValue = false;
                foreach( CswNbtNodeTypePropListOption o in Options.Options )
                {
                    foundValue = foundValue || ( o.Value == Value );
                    JObject Opt = new JObject();
                    Opt["Text"] = o.Text;
                    Opt["Value"] = o.Value;
                    OptionsArr.Add( Opt );
                }
                if( false == foundValue )
                {
                    JObject Opt = new JObject();
                    Opt["Text"] = Text;
                    Opt["Value"] = Value;
                    OptionsArr.Add( Opt );
                }
                ParentObject["options"] = OptionsArr;

                // To search or not to search
                if( Options.Options.Count == 1 && ( string.IsNullOrEmpty( Options.Options[0].Text ) && string.IsNullOrEmpty( Options.Options[0].Value ) ) )
                {
                    ParentObject["search"] = true;
                }
            }
            else
            {
                ParentObject["search"] = true;
                ParentObject["options"] = "";
            }
        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Text is replacing value
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
            //Value = CswTools.XmlRealAttributeName( PropRow[_ValueSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {

            if( null != JObject[_TextSubField.ToXmlNodeName( true )] )
            {
                Text = JObject[_TextSubField.ToXmlNodeName( true )].ToString();
            }

            if( null != JObject[_ValueSubField.ToXmlNodeName( true )] )
            {
                string selText = JObject[_ValueSubField.ToXmlNodeName( true )].ToString();

                // Decode the actual value from the option selected
                CswNbtNodeTypePropListOption selOption = Options.FindByText( selText );
                if( null != selOption )
                {
                    Value = selOption.Value;
                }
                else
                {
                    Value = selText;
                }
            }
        }

        public override void SyncGestalt()
        {
            // Text is replacing value
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Text );
            //_CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Value );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
