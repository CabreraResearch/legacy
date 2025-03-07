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

    public class CswNbtNodePropNumber : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropNumber( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsNumber;
        }

        public CswNbtNodePropNumber( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _ValueSubField = ( (CswNbtFieldTypeRuleNumber) _FieldTypeRule ).ValueSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _ValueSubField.Name, new Tuple<Func<dynamic>, Action<dynamic>>( () => Value, x => Value = CswConvert.ToDouble( x ) ) );
        }

        private CswNbtSubField _ValueSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        public double Value
        {
            get
            {
                string StringValue = GetPropRowValue( _ValueSubField );
                if( CswTools.IsFloat( StringValue ) )
                    return Convert.ToDouble( StringValue );
                else
                    return Double.NaN;
            }
            set
            {
                if( Double.IsNaN( value ) )
                {
                    SetPropRowValue( _ValueSubField, Double.NaN );
                    Gestalt = string.Empty;
                }
                else
                {
                    // Round the value to the precision
                    double RoundedValue = value;
                    if( Precision > Int32.MinValue )
                        RoundedValue = Math.Round( value, Precision, MidpointRounding.AwayFromZero );

                    SetPropRowValue( _ValueSubField, RoundedValue );
                    SyncGestalt();
                }
            }
        }

        public Int32 Precision
        {
            get
            {
                //Int32 Ret = _CswNbtMetaDataNodeTypeProp.NumberPrecision;
                Int32 Ret = CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleNumber.AttributeName.Precision] );
                if( Ret < 0 )
                {
                    Ret = 6;
                }
                return Ret;
            }
        }

        private double _MinValue = Double.NaN;
        public double MinValue
        {
            get
            {
                if( Double.IsNaN( _MinValue ) )
                {
                    //_MinValue = _CswNbtMetaDataNodeTypeProp.MinValue;
                    _MinValue = CswConvert.ToDouble( _CswNbtNodePropData[CswNbtFieldTypeRuleNumber.AttributeName.MinimumValue] );
                }
                return _MinValue;
            }
            set { _MinValue = value; }
        } // MinValue


        private double _MaxValue = Double.NaN;
        public double MaxValue
        {
            get
            {
                if( Double.IsNaN( _MaxValue ) )
                {
                    //_MaxValue = _CswNbtMetaDataNodeTypeProp.MaxValue;
                    _MaxValue = CswConvert.ToDouble( _CswNbtNodePropData[CswNbtFieldTypeRuleNumber.AttributeName.MaximumValue] );
                }
                return _MaxValue;
            }
            set { _MaxValue = value; }
        } // MaxValue

        /// <summary>
        /// When set to true, the MinValue and MaxValue limits are not included in the allowed number range.
        /// </summary>
        public bool ExcludeRangeLimits
        {
            get
            {
                //return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                return CswConvert.ToBoolean( _CswNbtNodePropData[CswNbtFieldTypeRuleNumber.AttributeName.ExcludeRangeLimits] );
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ValueSubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Value ) ) ? Value.ToString() : string.Empty;
            ParentObject["minvalue"] = MinValue.ToString();
            ParentObject["maxvalue"] = MaxValue.ToString();
            ParentObject["precision"] = Precision;
            ParentObject["excludeRangeLimits"] = ExcludeRangeLimits;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( PropRow.Table.Columns.Contains( _ValueSubField.ToXmlNodeName() ) )
            {
                string Val = CswTools.XmlRealAttributeName( PropRow[_ValueSubField.ToXmlNodeName()].ToString() );
                if( Val != string.Empty )
                    Value = Convert.ToDouble( PropRow[_ValueSubField.ToXmlNodeName()].ToString() );
            }
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_ValueSubField.ToXmlNodeName( true )] )
            {
                Value = CswConvert.ToDouble( JObject[_ValueSubField.ToXmlNodeName( true )] );
            }
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Value.ToString() );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
