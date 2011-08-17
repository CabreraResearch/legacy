using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropTime : CswNbtNodeProp
    {

        public CswNbtNodePropTime( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
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
                    return TimeValue.ToLongTimeString();
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
                    _CswNbtNodePropData.SetPropRowValue( _TimeValueSubField.Column, value.ToLongTimeString() );
					_CswNbtNodePropData.Gestalt = value.ToLongTimeString();
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
            XmlNode TimeNode = CswXmlDocument.AppendXmlNode( ParentNode, _TimeValueSubField.ToXmlNodeName() );
            if( TimeValue != DateTime.MinValue )
            {
				TimeNode.InnerText = TimeValue.ToLongTimeString();
            }
        } // ToXml()

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _TimeValueSubField.ToXmlNodeName( true ), ( TimeValue != DateTime.MinValue ) ?
				TimeValue.ToLongTimeString() : string.Empty ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
			//ParentObject[_TimeValueSubField.ToXmlNodeName( true )] = ( TimeValue != DateTime.MinValue ) ?
			//    TimeValue.ToLongTimeString() : string.Empty;
			CswDateTime CswTime = new CswDateTime( _CswNbtResources, TimeValue );
			ParentObject.Add( new JProperty( _TimeValueSubField.ToXmlNodeName( true ),
											 CswTime.ToClientAsTimeJObject() ) );
		}

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            TimeValue = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _TimeValueSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _TimeValueSubField.ToXmlNodeName( true ) ) )
            {
                TimeValue = CswConvert.ToDateTime( XmlNode.Element( _TimeValueSubField.ToXmlNodeName( true ) ).Value );
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string Val = CswTools.XmlRealAttributeName( PropRow[_TimeValueSubField.ToXmlNodeName()].ToString() );
            if( Val != string.Empty )
                TimeValue = Convert.ToDateTime( Val );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject.Property( _TimeValueSubField.ToXmlNodeName( true ) ) )
            {
                //TimeValue = CswConvert.ToDateTime( JObject.Property( _TimeValueSubField.ToXmlNodeName( true ) ).Value );
				CswDateTime CswTime = new CswDateTime( _CswNbtResources );
				CswTime.FromClientTimeJObject( (JObject) JObject[_TimeValueSubField.ToXmlNodeName( true )] );
				TimeValue = CswTime.ToDateTime();
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
