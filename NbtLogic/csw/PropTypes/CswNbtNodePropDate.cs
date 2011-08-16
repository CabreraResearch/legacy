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
                    _CswNbtNodePropData.SetPropRowValue( _DateValueSubField.Column, DateTime.MinValue );
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
			//CswXmlDocument.AppendXmlNode( ParentNode, _DateValueSubField.ToXmlNodeName(), DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat ) );
			XmlNode DateValueNode = CswXmlDocument.AppendXmlNode( ParentNode, _DateValueSubField.ToXmlNodeName() );
			CswDateTime CswDate = new CswDateTime( _CswNbtResources, DateValue );
			CswXmlDocument.AppendXmlAttribute( DateValueNode, "date", CswDate.ToClientAsString() );
			CswXmlDocument.AppendXmlAttribute( DateValueNode, "dateformat", CswDate.ClientDateFormat );
		}

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _DateValueSubField.ToXmlNodeName( true ), DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat ) ) );
        }

		public override void ToJSON( JObject ParentObject )
		{
			//ParentObject.Add( new JProperty( _DateValueSubField.ToXmlNodeName( true ), DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat ) ) );
            //ParentObject[_DateValueSubField.ToXmlNodeName( true )] = DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat );
			CswDateTime CswDate = new CswDateTime( _CswNbtResources, DateValue );
			ParentObject.Add( new JProperty( _DateValueSubField.ToXmlNodeName( true ), 
											 CswDate.ToClientAsJObject() ) );
		}

		public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
		{
			//DateValue = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _DateValueSubField.ToXmlNodeName() );
			XmlNode DateValueNode = CswXmlDocument.ChildXmlNode( XmlNode, _DateValueSubField.ToXmlNodeName() );
			CswDateTime CswDate = new CswDateTime( _CswNbtResources, DateValueNode.Attributes["dateformat"].Value, string.Empty );
			CswDate.FromClientDateString( DateValueNode.Attributes["date"].Value );
			DateValue = CswDate.ToDateTime();
		} // ReadXml()

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _DateValueSubField.ToXmlNodeName( true ) ) )
            {
                DateValue = CswConvert.ToDateTime( XmlNode.Element( _DateValueSubField.ToXmlNodeName( true ) ).Value );
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string Val = CswTools.XmlRealAttributeName( PropRow[_DateValueSubField.ToXmlNodeName()].ToString() );
            if( Val != string.Empty )
                DateValue = Convert.ToDateTime( Val );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
			if( null != JObject.Property( _DateValueSubField.ToXmlNodeName( true ) ) )
			{
				//DateValue = CswConvert.ToDateTime( JObject.Property( _DateValueSubField.ToXmlNodeName(true) ).Value );
                //DateValue = CswConvert.ToDateTime( JObject.Property( _DateValueSubField.ToXmlNodeName( true ) ).Value );
				CswDateTime CswDate = new CswDateTime( _CswNbtResources );
				CswDate.FromClientJObject( (JObject) JObject[_DateValueSubField.ToXmlNodeName( true )] );
				DateValue = CswDate.ToDateTime();
			}
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
