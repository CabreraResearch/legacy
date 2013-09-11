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
            _TextSubField = ((CswNbtFieldTypeRuleStatic) _FieldTypeRule).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => StaticText, x => StaticText = CswConvert.ToString(x) ) );
        }

        private CswNbtSubField _TextSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }
        
        public string StaticText
        {
            get
            {
                string PossibleValue = GetPropRowValue( _TextSubField );
                if( PossibleValue != string.Empty )
                    return PossibleValue;
                else
                    return _CswNbtMetaDataNodeTypeProp.StaticText;
            }
            set
            {
                if( value != _CswNbtMetaDataNodeTypeProp.StaticText )
                {
                    SetPropRowValue( _TextSubField, value );
                    Gestalt = value;
                }
                else
                {
                    SetPropRowValue( _TextSubField, string.Empty );
                    Gestalt = string.Empty;
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

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_TextSubField.ToXmlNodeName( true )] = StaticText;
            ParentObject["rows"] = Rows;
            ParentObject["columns"] = Columns;
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

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, StaticText );
        }
    }//CswNbtNodePropStatic

}//namespace ChemSW.Nbt.PropTypes
