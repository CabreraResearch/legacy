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

    public class CswNbtNodePropNumber : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropNumber( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsNumber;
        }

        public CswNbtNodePropNumber( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
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
        public double MinValue
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.MinValue;
            }
        }
        public double MaxValue
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.MaxValue;
            }
        }

        /// <summary>
        /// When set to true, minValue and MaxValue are not included in the number range.
        /// </summary>
        public bool IsOpenSet
        {
            get
            {
                return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute1 );
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode ValueNode = CswXmlDocument.AppendXmlNode( ParentNode, _ValueSubField.ToXmlNodeName() );
            if( !Double.IsNaN( Value ) )
            {
                ValueNode.InnerText = Value.ToString();
            }
            CswXmlDocument.AppendXmlAttribute( ValueNode, "minvalue", MinValue.ToString() );
            CswXmlDocument.AppendXmlAttribute( ValueNode, "maxvalue", MaxValue.ToString() );
            CswXmlDocument.AppendXmlAttribute( ValueNode, "precision", Precision.ToString() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _ValueSubField.ToXmlNodeName( true ), ( !Double.IsNaN( Value ) ) ? Value.ToString() : string.Empty,
                                          new XAttribute( "minvalue", MinValue.ToString() ),
                                          new XAttribute( "maxvalue", MaxValue.ToString() ),
                                          new XAttribute( "precision", Precision.ToString() ) ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_ValueSubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Value ) ) ? Value.ToString() : string.Empty;
            ParentObject["minvalue"] = MinValue.ToString();
            ParentObject["maxvalue"] = MaxValue.ToString();
            ParentObject["precision"] = Precision.ToString();
            ParentObject["isOpenSet"] = IsOpenSet.ToString();
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Value = CswXmlDocument.ChildXmlNodeValueAsDouble( XmlNode, _ValueSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _ValueSubField.ToXmlNodeName( true ) ) )
            {
                Value = CswConvert.ToDouble( XmlNode.Element( _ValueSubField.ToXmlNodeName( true ) ).Value );
            }
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
