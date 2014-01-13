using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{


    public class CswNbtNodePropText : CswNbtNodeProp
    {

        public static implicit operator CswNbtNodePropText( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsText;
        }

        public CswNbtNodePropText( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _TextSubField = ( (CswNbtFieldTypeRuleText) _FieldTypeRule ).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, x => Text = CswConvert.ToString( x ) ) );
        }

        private CswNbtSubField _TextSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }

        public string Text
        {
            get
            {
                return GetPropRowValue( _TextSubField );
            }
            set
            {
                SetPropRowValue( _TextSubField, value );
                Gestalt = value;
            }
        }
        public Int32 Size
        {
            get
            {
                //Int32 Ret = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                Int32 Ret = CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleText.AttributeName.Size] );
                if( Ret <= 0 )
                {
                    Ret = 25;
                }
                return Ret;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.Length = value;
            //}
        }

        public Int32 MaxLength
        {
            get
            {
                //Int32 Ret = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute2 );
                Int32 Ret = CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleText.AttributeName.MaximumLength] );
                if( Ret <= 0 )
                {
                    Ret = 255;
                }
                return Ret;
            }
        }

        public string RegEx
        {
            get
            {
                //return ( _CswNbtMetaDataNodeTypeProp.Attribute3 ); 
                return _CswNbtNodePropData[CswNbtFieldTypeRuleText.AttributeName.ValidationRegex];
            }
        }

        public string RegExMsg
        {
            get
            {
                //return ( _CswNbtMetaDataNodeTypeProp.Attribute4 );
                return _CswNbtNodePropData[CswNbtFieldTypeRuleText.AttributeName.RegexMessage];
            }
        }


        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        //private string _ElemName_Value = "Value";

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST

            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
            ParentObject["size"] = Size;
            ParentObject["maxlength"] = MaxLength;
            ParentObject["regex"] = RegEx;
            ParentObject["regexmsg"] = RegExMsg;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_TextSubField.ToXmlNodeName( true )] )
            {
                Text = JObject[_TextSubField.ToXmlNodeName( true )].ToString();
            }
        }
        
        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Text );
        }

        /// <summary>
        /// Compares this prop's Text value with all existing values that share the same NodeTypePropId and changes it until it becomes unqiue. 
        /// </summary>
        public void makeUnique()
        {
            bool isUnique = false;
            CswTableSelect JctNodePropSelect = _CswNbtResources.makeCswTableSelect( NodeTypePropId + "_matching select", "jct_nodes_props" );
            int PropNum = 1;
            while( false == isUnique )
            {
                DataTable MatchingPropsTable = JctNodePropSelect.getTable( "where nodetypepropid = " + NodeTypePropId + " and " + _TextSubField.Column + " = '" + Text + "'" );
                if( MatchingPropsTable.Rows.Count > 1 )
                {
                    Text = Text + PropNum;
                    PropNum++;
                }
                else
                {
                    isUnique = true;
                }
            }
        }

    }//CswNbtNodePropText

}//namespace ChemSW.Nbt.PropTypes
