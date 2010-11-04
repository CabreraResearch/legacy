using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropList : CswNbtNodeProp
    {

        public CswNbtNodePropList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.List )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropList() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _ValueSubField = ( (CswNbtFieldTypeRuleList) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).ValueSubField;

        }//generic

        private CswNbtSubField _ValueSubField;

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

        public string Value
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _ValueSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        private CswNbtNodeTypePropListOptions _CswNbtNodeTypePropListOptions = null;
        public CswNbtNodeTypePropListOptions Options
        {
            get
            {
                if( null == _CswNbtNodeTypePropListOptions )
                {
                    _CswNbtNodeTypePropListOptions = new CswNbtNodeTypePropListOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp.PropId );
                }

                return ( _CswNbtNodeTypePropListOptions );

            }//get

        }//Options

        public static string OptionTextField = "Text";
        public static string OptionValueField = "Value";

        override public void onNodePropRowFilled()
        {
        }


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode ValueNode = CswXmlDocument.AppendXmlNode( ParentNode, _ValueSubField.Name.ToString(), Value );
            XmlNode OptionsNode = CswXmlDocument.AppendXmlNode( ParentNode, "Options", Options.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Value = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _ValueSubField.Name.ToString() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Value = CswTools.XmlRealAttributeName( PropRow[_ValueSubField.Name.ToString()].ToString() );
        }


    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
