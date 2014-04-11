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

    public class CswNbtNodePropNumericRange : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropNumericRange( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsNumericRange;
        }

        public CswNbtNodePropNumericRange( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _LowerSubField = ( (CswNbtFieldTypeRuleNumericRange) _FieldTypeRule ).LowerSubField;
            _TargetSubField = ( (CswNbtFieldTypeRuleNumericRange) _FieldTypeRule ).TargetSubField;
            _UpperSubField = ( (CswNbtFieldTypeRuleNumericRange) _FieldTypeRule ).UpperSubField;
            _LowerInclusiveSubField = ( (CswNbtFieldTypeRuleNumericRange) _FieldTypeRule ).LowerInclusiveSubField;
            _UpperInclusiveSubField = ( (CswNbtFieldTypeRuleNumericRange) _FieldTypeRule ).UpperInclusiveSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _LowerSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Lower, x => Lower = CswConvert.ToDouble( x ) ) );
            _SubFieldMethods.Add( _TargetSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Target, x => Target = CswConvert.ToDouble( x ) ) );
            _SubFieldMethods.Add( _UpperSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Upper, x => Upper = CswConvert.ToDouble( x ) ) );
            _SubFieldMethods.Add( _LowerInclusiveSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => LowerInclusive, x => LowerInclusive = x ) );
            _SubFieldMethods.Add( _UpperInclusiveSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => UpperInclusive, x => UpperInclusive = x ) );
        }

        private CswNbtSubField _LowerSubField;
        private CswNbtSubField _TargetSubField;
        private CswNbtSubField _UpperSubField;
        private CswNbtSubField _LowerInclusiveSubField;
        private CswNbtSubField _UpperInclusiveSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        public double Lower
        {
            get { return CswConvert.ToDouble( GetPropRowValue( _LowerSubField ) ); }
            set
            {
                if( Double.IsNaN( value ) )
                {
                    SetPropRowValue( _LowerSubField, Double.NaN );
                }
                else
                {
                    // Round the value to the precision
                    double RoundedValue = value;
                    if( Precision > Int32.MinValue )
                    {
                        RoundedValue = Math.Round( value, Precision, MidpointRounding.AwayFromZero );
                    }
                    SetPropRowValue( _LowerSubField, RoundedValue );
                }
                SyncGestalt();
            }
        }

        public double Target
        {
            get { return CswConvert.ToDouble( GetPropRowValue( _TargetSubField ) ); }
            set
            {
                if( Double.IsNaN( value ) )
                {
                    SetPropRowValue( _TargetSubField, Double.NaN );
                }
                else
                {
                    // Round the value to the precision
                    double RoundedValue = value;
                    if( Precision > Int32.MinValue )
                    {
                        RoundedValue = Math.Round( value, Precision, MidpointRounding.AwayFromZero );
                    }
                    SetPropRowValue( _TargetSubField, RoundedValue );
                }
                SyncGestalt();
            }
        }

        public double Upper
        {
            get { return CswConvert.ToDouble( GetPropRowValue( _UpperSubField ) ); }
            set
            {
                if( Double.IsNaN( value ) )
                {
                    SetPropRowValue( _UpperSubField, Double.NaN );
                }
                else
                {
                    // Round the value to the precision
                    double RoundedValue = value;
                    if( Precision > Int32.MinValue )
                    {
                        RoundedValue = Math.Round( value, Precision, MidpointRounding.AwayFromZero );
                    }
                    SetPropRowValue( _UpperSubField, RoundedValue );
                }
                SyncGestalt();
            }
        }

        public bool LowerInclusive
        {
            get { return CswConvert.ToBoolean( GetPropRowValue( _LowerInclusiveSubField ) ); }
            set
            {
                SetPropRowValue( _LowerInclusiveSubField, value );
                SyncGestalt();
            }
        }

        public bool UpperInclusive
        {
            get { return CswConvert.ToBoolean( GetPropRowValue( _UpperInclusiveSubField ) ); }
            set
            {
                SetPropRowValue( _UpperInclusiveSubField, value );
                SyncGestalt();
            }
        }

        public Int32 Precision
        {
            get
            {
                //Int32 Ret = _CswNbtMetaDataNodeTypeProp.NumericRangePrecision;
                Int32 Ret = CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleNumericRange.AttributeName.Precision] );
                if( Ret < 0 )
                {
                    Ret = 6;
                }
                return Ret;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_LowerSubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Lower ) ) ? Lower.ToString() : string.Empty;
            ParentObject[_TargetSubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Target ) ) ? Target.ToString() : string.Empty;
            ParentObject[_UpperSubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Upper ) ) ? Upper.ToString() : string.Empty;
            ParentObject[_LowerInclusiveSubField.ToXmlNodeName( true )] = LowerInclusive;
            ParentObject[_UpperInclusiveSubField.ToXmlNodeName( true )] = UpperInclusive;
            ParentObject["precision"] = Precision;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( PropRow.Table.Columns.Contains( _LowerSubField.ToXmlNodeName() ) )
            {
                string Val = CswTools.XmlRealAttributeName( PropRow[_LowerSubField.ToXmlNodeName()].ToString() );
                if( Val != string.Empty )
                {
                    Lower = Convert.ToDouble( PropRow[_LowerSubField.ToXmlNodeName()].ToString() );
                }
            }
            if( PropRow.Table.Columns.Contains( _TargetSubField.ToXmlNodeName() ) )
            {
                string Val = CswTools.XmlRealAttributeName( PropRow[_TargetSubField.ToXmlNodeName()].ToString() );
                if( Val != string.Empty )
                {
                    Target = Convert.ToDouble( PropRow[_TargetSubField.ToXmlNodeName()].ToString() );
                }
            }
            if( PropRow.Table.Columns.Contains( _UpperSubField.ToXmlNodeName() ) )
            {
                string Val = CswTools.XmlRealAttributeName( PropRow[_UpperSubField.ToXmlNodeName()].ToString() );
                if( Val != string.Empty )
                {
                    Upper = Convert.ToDouble( PropRow[_UpperSubField.ToXmlNodeName()].ToString() );
                }
            }
            if( PropRow.Table.Columns.Contains( _LowerInclusiveSubField.ToXmlNodeName() ) )
            {
                LowerInclusive = CswConvert.ToBoolean( CswTools.XmlRealAttributeName( PropRow[_LowerInclusiveSubField.ToXmlNodeName()].ToString() ) );
            }
            if( PropRow.Table.Columns.Contains( _UpperInclusiveSubField.ToXmlNodeName() ) )
            {
                UpperInclusive = CswConvert.ToBoolean( CswTools.XmlRealAttributeName( PropRow[_UpperInclusiveSubField.ToXmlNodeName()].ToString() ) );
            }
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_LowerSubField.ToXmlNodeName( true )] )
            {
                Lower = CswConvert.ToDouble( JObject[_LowerSubField.ToXmlNodeName( true )] );
            }
            if( null != JObject[_TargetSubField.ToXmlNodeName( true )] )
            {
                Target = CswConvert.ToDouble( JObject[_TargetSubField.ToXmlNodeName( true )] );
            }
            if( null != JObject[_UpperSubField.ToXmlNodeName( true )] )
            {
                Upper = CswConvert.ToDouble( JObject[_UpperSubField.ToXmlNodeName( true )] );
            }
            if( null != JObject[_LowerInclusiveSubField.ToXmlNodeName( true )] )
            {
                LowerInclusive = CswConvert.ToBoolean( JObject[_LowerInclusiveSubField.ToXmlNodeName( true )] );
            }
            if( null != JObject[_UpperInclusiveSubField.ToXmlNodeName( true )] )
            {
                UpperInclusive = CswConvert.ToBoolean( JObject[_UpperInclusiveSubField.ToXmlNodeName( true )] );
            }
        }

        public override void SyncGestalt()
        {
            string newGestalt = string.Empty;
            if( false == Double.IsNaN( Lower ) )
            {
                newGestalt += Lower.ToString();
                if( LowerInclusive )
                {
                    newGestalt += " <= ";
                }
                else
                {
                    newGestalt += " < ";
                }
            }
            if( false == Double.IsNaN( Target ) )
            {
                newGestalt += "[" + Target.ToString() + "]";
            }
            if( false == Double.IsNaN( Upper ) )
            {
                if( UpperInclusive )
                {
                    newGestalt += " <= ";
                }
                else
                {
                    newGestalt += " < ";
                }
                newGestalt += Upper.ToString();
            }
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, newGestalt );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
