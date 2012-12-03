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
            _FieldTypeRule = (CswNbtFieldTypeRuleNumber) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _ValueSubField = _FieldTypeRule.ValueSubField;
        }
        private CswNbtFieldTypeRuleNumber _FieldTypeRule;
        private CswNbtSubField _ValueSubField;

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
            }//

        }//Gestalt

        public double Value
        {
            get
            {
                string StringValue = _CswNbtNodePropData.GetPropRowValue( _ValueSubField.Column );
                if( CswTools.IsFloat( StringValue ) )
                    return Convert.ToDouble( StringValue );
                else
                    return Double.NaN;
            }
            set
            {
                if( Double.IsNaN( value ) )
                {
                    _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, Double.NaN );
                    _CswNbtNodePropData.Gestalt = string.Empty;
                }
                else
                {
                    // Round the value to the precision
                    double RoundedValue = value;
                    if( Precision > Int32.MinValue )
                        RoundedValue = Math.Round( value, Precision, MidpointRounding.AwayFromZero );

                    _CswNbtNodePropData.SetPropRowValue( _ValueSubField.Column, RoundedValue );
                    _CswNbtNodePropData.Gestalt = RoundedValue.ToString();
                }
            }
        }

        public Int32 Precision
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.NumberPrecision;
            }
        }

        private double _MinValue = Double.NaN;
        public double MinValue
        {
            get
            {
                if( Double.IsNaN( _MinValue ) )
                {
                    _MinValue = _CswNbtMetaDataNodeTypeProp.MinValue;
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
                    _MaxValue = _CswNbtMetaDataNodeTypeProp.MaxValue;
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
                return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute1 );
            }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ValueSubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Value ) ) ? Value.ToString() : string.Empty;
            ParentObject["minvalue"] = MinValue.ToString();
            ParentObject["maxvalue"] = MaxValue.ToString();
            ParentObject["precision"] = Precision.ToString();
            ParentObject["excludeRangeLimits"] = ExcludeRangeLimits.ToString();
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
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
