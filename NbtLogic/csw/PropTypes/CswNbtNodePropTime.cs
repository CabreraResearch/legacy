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

    public class CswNbtNodePropTime: CswNbtNodeProp
    {

        public CswNbtNodePropTime(CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp)
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp  )
        {
            _TimeValueSubField = ( (CswNbtFieldTypeRuleTime) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).TimeValueSubField;

        }//generic

        private CswNbtSubField _TimeValueSubField;

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
                if( TimeValue != DateTime.MinValue )
                    return TimeValue.ToShortTimeString();
                else
                    return String.Empty;
            }//

        }//Gestalt

        public DateTime TimeValue
        {
            get
            {
                string StringValue = _CswNbtNodePropData.GetPropRowValue( _TimeValueSubField.Column );
                DateTime ReturnVal = DateTime.MinValue;
                if( StringValue != string.Empty )
                    ReturnVal = Convert.ToDateTime( StringValue );
                return ( ReturnVal );
            }

            set
            {
                if( DateTime.MinValue != value )
                {
                    _CswNbtNodePropData.SetPropRowValue( _TimeValueSubField.Column, value.ToShortTimeString() );
                    _CswNbtNodePropData.Gestalt = value.ToShortTimeString();
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _TimeValueSubField.Column, DateTime.MinValue );
                    _CswNbtNodePropData.Gestalt = string.Empty;
                }
            }

        }//DateTime

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode TimeNode = CswXmlDocument.AppendXmlNode( ParentNode, _TimeValueSubField.ToXmlNodeName(), TimeValue.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            TimeValue = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _TimeValueSubField.ToXmlNodeName() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string Val = CswTools.XmlRealAttributeName( PropRow[_TimeValueSubField.ToXmlNodeName()].ToString() );
            if( Val != string.Empty )
                TimeValue = Convert.ToDateTime( Val );
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
