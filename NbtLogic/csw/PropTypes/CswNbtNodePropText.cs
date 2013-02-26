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


    public class CswNbtNodePropText : CswNbtNodeProp
    {

        public static implicit operator CswNbtNodePropText( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsText;
        }

        public CswNbtNodePropText( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleText) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _TextSubField = _FieldTypeRule.TextSubField;
        }

        private CswNbtFieldTypeRuleText _FieldTypeRule;
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
            }//

        }//Gestalt

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
        public Int32 Size
        {
            get
            {
                if( false == String.IsNullOrEmpty( _CswNbtMetaDataNodeTypeProp.Attribute1 ) )
                    return CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                else
                    return 25;
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
                if( false == String.IsNullOrEmpty( _CswNbtMetaDataNodeTypeProp.Attribute2 ) )
                    return CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute2 );
                else
                    return 255;
            }
        }

        public string RegEx
        {
            get
            {
                return ( _CswNbtMetaDataNodeTypeProp.Attribute3 ); 
            }
        }


        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        //private string _ElemName_Value = "Value";

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
            ParentObject["size"] = Size.ToString();
            ParentObject["maxlength"] = MaxLength.ToString();
            ParentObject["regex"] = RegEx;
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
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Text );
        }

    }//CswNbtNodePropText

}//namespace ChemSW.Nbt.PropTypes
