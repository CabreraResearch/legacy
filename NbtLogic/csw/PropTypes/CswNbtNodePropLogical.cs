using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropLogical : CswNbtNodeProp
    {

        public CswNbtNodePropLogical( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _CheckedSubField = ( (CswNbtFieldTypeRuleLogical) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CheckedSubField;
        }

        private CswNbtSubField _CheckedSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        public Tristate Checked
        {
            get
            {
                return CswConvert.ToTristate( _CswNbtNodePropData.GetPropRowValue( _CheckedSubField.Column ) );
            }
            set
            {
                object val = CswConvert.ToDbVal( value );
                if( val != DBNull.Value )
                {
                    _CswNbtNodePropData.SetPropRowValue( _CheckedSubField.Column, val.ToString() );
                    _CswNbtNodePropData.Gestalt = CswConvert.ToDisplayString( value );
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _CheckedSubField.Column, string.Empty );
                    _CswNbtNodePropData.Gestalt = string.Empty;
                }
            }
        }


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode CheckedNode = CswXmlDocument.AppendXmlNode( ParentNode, _CheckedSubField.ToXmlNodeName(), Checked.ToString().ToLower() );
            XmlNode RequiredNode = CswXmlDocument.AppendXmlNode( ParentNode, CswNbtSubField.SubFieldName.Required.ToString(), Required.ToString().ToLower() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Checked = CswConvert.ToTristate( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _CheckedSubField.ToXmlNodeName() ) );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Checked = CswConvert.ToTristate( PropRow[_CheckedSubField.ToXmlNodeName()] );
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
