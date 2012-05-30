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

    public class CswNbtNodePropScientific : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropScientific( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsScientific;
        }

        public CswNbtNodePropScientific( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleScientific) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _BaseSubField = _FieldTypeRule.BaseSubField;
            _ExponentSubField = _FieldTypeRule.ExponentSubField;
        }

        private CswNbtFieldTypeRuleScientific _FieldTypeRule;
        private CswNbtSubField _BaseSubField;
        private CswNbtSubField _ExponentSubField;

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

        private void _setGestalt()
        {
            if( false == Double.IsNaN( Base ) )
            {
                if( Exponent != Int32.MinValue && Exponent != 0 )
                {
                    _CswNbtNodePropData.Gestalt = Base.ToString() + "E" + Exponent.ToString();
                }
                else
                {
                    _CswNbtNodePropData.Gestalt = Base.ToString();
                }
            }
        }

        public double RealValue
        {
            get
            {
                if( false == Double.IsNaN( Base ) )
                {
                    return Base * Math.Pow( 10, Exponent );
                }
                else
                {
                    return Double.NaN;
                }
            }
        } // RealValue



        public double Base
        {
            get
            {
                string StringValue = _CswNbtNodePropData.GetPropRowValue( _BaseSubField.Column );
                return CswConvert.ToDouble( StringValue );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _BaseSubField.Column, value );
                if( false == Double.IsNaN( value ) && Exponent == Int32.MinValue )
                {
                    Exponent = 0;
                }
                _setGestalt();
            } // set
        } // Base

        public Int32 Exponent
        {
            get
            {
                string StringValue = _CswNbtNodePropData.GetPropRowValue( _ExponentSubField.Column );
                return CswConvert.ToInt32( StringValue );
            }
            set
            {
                Int32 ExpValue = value;
                if( false == Double.IsNaN( Base ) && value == Int32.MinValue )
                {
                    ExpValue = 0;
                }
                _CswNbtNodePropData.SetPropRowValue( _ExponentSubField.Column, ExpValue );
                _setGestalt();
            }
        } // Exponent

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


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode BaseNode = CswXmlDocument.AppendXmlNode( ParentNode, _BaseSubField.ToXmlNodeName( true ) );
            if( false == Double.IsNaN( Base ) )
            {
                BaseNode.InnerText = Base.ToString();
            }
            CswXmlDocument.AppendXmlAttribute( BaseNode, "minvalue", MinValue.ToString() );
            CswXmlDocument.AppendXmlAttribute( BaseNode, "precision", Precision.ToString() );

            XmlNode ExponentNode = CswXmlDocument.AppendXmlNode( ParentNode, _ExponentSubField.ToXmlNodeName( true ) );
            if( Int32.MinValue != Exponent )
            {
                ExponentNode.InnerText = Exponent.ToString();
            }
            CswXmlDocument.AppendXmlAttribute( ExponentNode, "minvalue", MinValue.ToString() );

        } // ToXml()

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Base = CswXmlDocument.ChildXmlNodeValueAsDouble( XmlNode, _BaseSubField.ToXmlNodeName( true ) );
            Exponent = CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _ExponentSubField.ToXmlNodeName( true ) );
        }

        public override void ToXElement( XElement ParentNode )
        {
            throw new NotImplementedException();
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            throw new NotImplementedException();
        }

        public override void ToJSON( JObject ParentObject )
        {
            if( false == Double.IsNaN( Base ) )
            {
                ParentObject[_BaseSubField.ToXmlNodeName( true )] = Base.ToString();
            }
            else
            {
                ParentObject[_BaseSubField.ToXmlNodeName( true )] = string.Empty;
            }
            if( Int32.MinValue != Exponent )
            {
                ParentObject[_ExponentSubField.ToXmlNodeName( true )] = Exponent.ToString();
            }
            else
            {
                ParentObject[_ExponentSubField.ToXmlNodeName( true )] = string.Empty;
            }
            ParentObject["minvalue"] = MinValue.ToString();
            ParentObject["precision"] = Precision.ToString();
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Base = CswConvert.ToDouble( JObject[_BaseSubField.ToXmlNodeName( true )] );
            Exponent = CswConvert.ToInt32( JObject[_ExponentSubField.ToXmlNodeName( true )] );
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( PropRow.Table.Columns.Contains( _BaseSubField.ToXmlNodeName( true ) ) )
            {
                Base = CswConvert.ToDouble( PropRow[_BaseSubField.ToXmlNodeName( true )].ToString() );
            }
            if( PropRow.Table.Columns.Contains( _ExponentSubField.ToXmlNodeName( true ) ) )
            {
                Base = CswConvert.ToInt32( PropRow[_ExponentSubField.ToXmlNodeName( true )].ToString() );
            }
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
