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

    public class CswNbtNodePropDateTime : CswNbtNodeProp
    {

        public CswNbtNodePropDateTime( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _DateValueSubField = ( (CswNbtFieldTypeRuleDateTime) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).DateValueSubField;

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
                //if( DateTimeValue != DateTime.MinValue )
                //    return DateTimeValue.ToShortDateString();
                //else
                //    return String.Empty;
                _setGestalt();
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt


        public DateTime DateTimeValue
        {
            get
            {
                //string StringValue = _CswNbtNodePropData.GetPropRowValue( _DateValueSubField.Column );
                //DateTime ReturnVal = DateTime.MinValue;
                //if( StringValue != string.Empty )
                //    ReturnVal = Convert.ToDateTime( StringValue );
                //return ( ReturnVal.Date );
                return _CswNbtNodePropData.GetPropRowValueDate( _DateValueSubField.Column );
            }

            set
            {
                if( DateTime.MinValue != value )
                {
                    _CswNbtNodePropData.SetPropRowValue( _DateValueSubField.Column, value );
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _DateValueSubField.Column, DateTime.MinValue );
                }
                _setGestalt();
            }

        }//DateTime

        private void _setGestalt()
        {
            DateTime Value = DateTimeValue;
            if( Value != DateTime.MinValue )
            {
                switch( DisplayMode )
                {
                    case DateDisplayMode.Date:
                        _CswNbtNodePropData.Gestalt = Value.ToShortDateString();
                        break;
                    case DateDisplayMode.Time:
                        _CswNbtNodePropData.Gestalt = Value.ToLongTimeString();
                        break;
                    case DateDisplayMode.DateTime:
                        _CswNbtNodePropData.Gestalt = Value.ToShortDateString() + " " + Value.ToLongTimeString();
                        break;
                }
            }
            else
            {
                _CswNbtNodePropData.Gestalt = string.Empty;
            }
        } // _setGestalt()

        public bool DefaultToToday
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.DateToday;
            }
        }

        /// <summary>
        /// Possible display modes for dates
        /// </summary>
        public enum DateDisplayMode
        {
            /// <summary>
            /// unknown display mode
            /// </summary>
            Unknown,
            /// <summary>
            /// display date only
            /// </summary>
            Date,
            /// <summary>
            /// display time only
            /// </summary>
            Time,
            /// <summary>
            /// display date and time
            /// </summary>
            DateTime
        }

        /// <summary>
        /// Determines whether dates, times, or datetimes are displayed
        /// </summary>
        private DateDisplayMode _DisplayMode = DateDisplayMode.Unknown;
        public DateDisplayMode DisplayMode
        {
            get
            {
                if( _DisplayMode == DateDisplayMode.Unknown )
                {
                    if( _CswNbtMetaDataNodeTypeProp.Extended != string.Empty )
                        _DisplayMode = (DateDisplayMode) Enum.Parse( typeof( DateDisplayMode ), _CswNbtMetaDataNodeTypeProp.Extended, true );
                    else
                        _DisplayMode = DateDisplayMode.Date;
                }
                return _DisplayMode;
            }
        } // DisplayMode

        public override void ToXml( XmlNode ParentNode )
        {
            //CswXmlDocument.AppendXmlNode( ParentNode, _DateValueSubField.ToXmlNodeName(), DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat ) );

            XmlNode DateValueNode = CswXmlDocument.AppendXmlNode( ParentNode, _DateValueSubField.ToXmlNodeName() );
            CswXmlDocument.AppendXmlAttribute( DateValueNode, "displaymode", DisplayMode.ToString() );

            CswDateTime CswDate = new CswDateTime( _CswNbtResources, DateTimeValue );
            CswXmlDocument.AppendXmlAttribute( DateValueNode, "datetime", CswDate.ToClientAsDateTimeString() );
            CswXmlDocument.AppendXmlAttribute( DateValueNode, "dateformat", CswDate.ClientDateFormat );
            CswXmlDocument.AppendXmlAttribute( DateValueNode, "timeformat", CswDate.ClientTimeFormat );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _DateValueSubField.ToXmlNodeName( true ), DateTimeValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat ) ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            //ParentObject.Add( new JProperty( _DateValueSubField.ToXmlNodeName( true ), DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat ) ) );
            //ParentObject[_DateValueSubField.ToXmlNodeName( true )] = DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat );

            CswDateTime CswDate = new CswDateTime( _CswNbtResources, DateTimeValue );
            ParentObject[_DateValueSubField.ToXmlNodeName( true )] = CswDate.ToClientAsDateTimeJObject();
            ParentObject["displaymode"] = DisplayMode.ToString();

        } // ToJSON()

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //DateValue = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _DateValueSubField.ToXmlNodeName() );
            XmlNode DateValueNode = CswXmlDocument.ChildXmlNode( XmlNode, _DateValueSubField.ToXmlNodeName() );
            CswDateTime CswDateTime = new CswDateTime( _CswNbtResources, DateValueNode.Attributes["dateformat"].Value, DateValueNode.Attributes["timeformat"].Value );
            CswDateTime.FromClientDateTimeString( DateValueNode.Attributes["datetime"].Value );
            DateTimeValue = CswDateTime.ToDateTime();
        } // ReadXml()

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _DateValueSubField.ToXmlNodeName( true ) ) )
            {
                DateTimeValue = CswConvert.ToDateTime( XmlNode.Element( _DateValueSubField.ToXmlNodeName( true ) ).Value );
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string Val = CswTools.XmlRealAttributeName( PropRow[_DateValueSubField.ToXmlNodeName()].ToString() );
            if( Val != string.Empty )
                DateTimeValue = Convert.ToDateTime( Val );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject.Property( _DateValueSubField.ToXmlNodeName( true ) ) )
            {
                //DateValue = CswConvert.ToDateTime( JObject.Property( _DateValueSubField.ToXmlNodeName(true) ).Value );
                //DateValue = CswConvert.ToDateTime( JObject.Property( _DateValueSubField.ToXmlNodeName( true ) ).Value );

                CswDateTime CswDate = new CswDateTime( _CswNbtResources );
                CswDate.FromClientDateTimeJObject( (JObject) JObject[_DateValueSubField.ToXmlNodeName( true )] );
                DateTimeValue = CswDate.ToDateTime();
            }
        }
    }//class CswNbtNodePropDateTime

}//namespace ChemSW.Nbt.PropTypes
