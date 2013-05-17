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
            _TextSubField = ((CswNbtFieldTypeRuleText) _FieldTypeRule).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, x => Text = CswConvert.ToString(x) ) );
        }

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
                Int32 Ret = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute1 );
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
                Int32 Ret = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute2 );
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
                return ( _CswNbtMetaDataNodeTypeProp.Attribute3 ); 
            }
        }

        public string RegExMsg
        {
            get
            {
                return ( _CswNbtMetaDataNodeTypeProp.Attribute4 ); 
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
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Text );
        }

    }//CswNbtNodePropText

}//namespace ChemSW.Nbt.PropTypes
