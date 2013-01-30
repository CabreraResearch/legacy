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

    public class CswNbtNodePropDateTime : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropDateTime( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsDateTime;
        }

        public CswNbtNodePropDateTime( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleDateTime) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _DateValueSubField = _FieldTypeRule.DateValueSubField;
        }//generic
        private CswNbtFieldTypeRuleDateTime _FieldTypeRule;
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
                SyncGestalt();
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
                SyncGestalt();
            }

        }//DateTime

        public override void SyncGestalt()
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

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            //ParentObject.Add( new JProperty( _DateValueSubField.ToXmlNodeName( true ), DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat ) ) );
            //ParentObject[_DateValueSubField.ToXmlNodeName( true )] = DateValue.Date.ToString( _CswNbtResources.CurrentUser.DateFormat );

            CswDateTime CswDate = new CswDateTime( _CswNbtResources, DateTimeValue );
            ParentObject[_DateValueSubField.ToXmlNodeName( true )] = CswDate.ToClientAsDateTimeJObject();
            ParentObject["displaymode"] = DisplayMode.ToString();

        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string Val = CswTools.XmlRealAttributeName( PropRow[_DateValueSubField.ToXmlNodeName()].ToString() );
            if( Val != string.Empty )
                DateTimeValue = Convert.ToDateTime( Val );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_DateValueSubField.ToXmlNodeName( true )] )
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
