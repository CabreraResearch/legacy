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
    public class CswNbtNodePropStatic : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropStatic( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsStatic;
        }

        public CswNbtNodePropStatic( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleStatic) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _TextSubField = _FieldTypeRule.TextSubField;
        }

        private CswNbtFieldTypeRuleStatic _FieldTypeRule;
        private CswNbtSubField _TextSubField;

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
            }
        }//Gestalt

        public string StaticText
        {
            get
            {
                string PossibleValue = _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
                if( PossibleValue != string.Empty )
                    return PossibleValue;
                else
                    return _CswNbtMetaDataNodeTypeProp.StaticText;
            }
            set
            {
                if( value != _CswNbtMetaDataNodeTypeProp.StaticText )
                {
                    _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, value );
                    _CswNbtNodePropData.Gestalt = value;
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, string.Empty );
                    _CswNbtNodePropData.Gestalt = string.Empty;
                }
            }
        }

        public Int32 Rows
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaRows != Int32.MinValue )
                    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
                else
                    return Int32.MinValue;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaRows = value;
            //}
        }

        public Int32 Columns
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaColumns != Int32.MinValue )
                    return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
                else
                    return Int32.MinValue;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaColumns = value;
            //}
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_TextSubField.ToXmlNodeName( true )] = StaticText;
            ParentObject["rows"] = Rows.ToString();
            ParentObject["columns"] = Columns.ToString();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            StaticText = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_TextSubField.ToXmlNodeName( true )] )
            {
                StaticText = JObject[_TextSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }//CswNbtNodePropStatic

}//namespace ChemSW.Nbt.PropTypes
