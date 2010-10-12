using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Nbt.MetaData;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropDate : CswNbtNodeProp
    {

        public CswNbtNodePropDate( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _DateValueSubField = ( (CswNbtFieldTypeRuleDate) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).DateValueSubField;

        }//generic

        private CswNbtSubField _DateValueSubField;

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
                if( DateValue != DateTime.MinValue )
                    return DateValue.ToShortDateString();
                else
                    return String.Empty;
            }//

        }//Gestalt


        public DateTime DateValue
        {
            get
            {
                string StringValue = _CswNbtNodePropData.GetPropRowValue( _DateValueSubField.Column );
                DateTime ReturnVal = DateTime.MinValue;
                if( StringValue != string.Empty )
                    ReturnVal = Convert.ToDateTime( StringValue );
                return ( ReturnVal.Date );
            }

            set
            {
                if( DateTime.MinValue != value )
                {
                    _CswNbtNodePropData.SetPropRowValue( _DateValueSubField.Column, value.Date.ToShortDateString() );
                    _CswNbtNodePropData.Gestalt = value.Date.ToShortDateString();
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _DateValueSubField.Column, CswConvert.ToDbVal( DateTime.MinValue ) );
                    _CswNbtNodePropData.Gestalt = string.Empty;
                }
            }

        }//DateTime

        public bool DefaultToToday
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.DateToday;
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode DateNode = CswXmlDocument.AppendXmlNode( ParentNode, _DateValueSubField.Name.ToString(), DateValue.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            DateValue = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _DateValueSubField.Name.ToString() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string Val = CswTools.XmlRealAttributeName( PropRow[_DateValueSubField.Name.ToString()].ToString() );
            if( Val != string.Empty )
                DateValue = Convert.ToDateTime( Val );
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
