using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

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
            _DateValueSubField = ( (CswNbtFieldTypeRuleDateTime) _FieldTypeRule ).DateValueSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _DateValueSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => DateTimeValue, x => DateTimeValue = CswConvert.ToDateTime( x ) ) );
        }

        private CswNbtSubField _DateValueSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }

        public DateTime DateTimeValue
        {
            get
            {
                //string StringValue = GetPropRowValue( _DateValueSubField.Column );
                //DateTime ReturnVal = DateTime.MinValue;
                //if( StringValue != string.Empty )
                //    ReturnVal = Convert.ToDateTime( StringValue );
                //return ( ReturnVal.Date );
                return GetPropRowValueDate( _DateValueSubField );
            }

            set
            {
                if( DateTime.MinValue != value )
                {
                    SetPropRowValue( _DateValueSubField, value );
                }
                else
                {
                    SetPropRowValue( _DateValueSubField, DateTime.MinValue );
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
                    case CswEnumNbtDateDisplayMode.Date:
                        SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Value.ToShortDateString() );
                        break;
                    case CswEnumNbtDateDisplayMode.Time:
                        SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Value.ToLongTimeString() );
                        break;
                    case CswEnumNbtDateDisplayMode.DateTime:
                        SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Value.ToShortDateString() + " " + Value.ToLongTimeString() );
                        break;
                }
            }
            else
            {
                Gestalt = string.Empty;
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
        /// Determines whether dates, times, or datetimes are displayed
        /// </summary>
        private CswEnumNbtDateDisplayMode _DisplayMode = CswResources.UnknownEnum;
        public CswEnumNbtDateDisplayMode DisplayMode
        {
            get
            {
                if (_DisplayMode == CswResources.UnknownEnum)
                {
                    if( _CswNbtMetaDataNodeTypeProp.Extended != string.Empty )
                        _DisplayMode = _CswNbtMetaDataNodeTypeProp.Extended;
                    else
                        _DisplayMode = CswEnumNbtDateDisplayMode.Date;
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
