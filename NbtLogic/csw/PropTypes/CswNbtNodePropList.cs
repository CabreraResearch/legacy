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
            //if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswEnumNbtFieldType.List )
            //{
            //    throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
            //                                "CswNbtNodePropList() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            //}
            _FieldTypeRule = (CswNbtFieldTypeRuleList) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _ValueSubField = _FieldTypeRule.ValueSubField;
            _TextSubField = _FieldTypeRule.TextSubField;

            _SearchThreshold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.relationshipoptionlimit.ToString() ) );
            if( _SearchThreshold <= 0 )
            {
                _SearchThreshold = 100;
            }

        }//generic

        private CswNbtFieldTypeRuleList _FieldTypeRule;
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

        // Text is replacing what was previously known as value so that these subfields correspond
        // to the fields in CswNbtNodeTypePropListOption
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

        public string Value
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _ValueSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, value );
                //_CswNbtNodePropData.Gestalt = value;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        private CswNbtNodeTypePropListOptions _CswNbtNodeTypePropListOptions = null;
        public CswNbtNodeTypePropListOptions Options
        {
            get
            {
                if( null == _CswNbtNodeTypePropListOptions )
                {
                    _CswNbtNodeTypePropListOptions = new CswNbtNodeTypePropListOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                }

                return ( _CswNbtNodeTypePropListOptions );

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
            ParentObject["options"] = Options.Options.Count > _SearchThreshold ? "" : Options.ToString();
        }

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
                Value = JObject[_ValueSubField.ToXmlNodeName( true )].ToString();
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
