using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropQuantity : CswNbtNodeProp
    {
        public CswNbtNodePropQuantity( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Quantity )
            {
                throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
                                            "CswNbtNodePropQuantity() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            // Get the units

            // get the Unit of Measure objectclassid
            CswNbtMetaDataObjectClass Unit_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );

            // generate the view
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtNodePropQuantity()";
            View.AddViewRelationship( Unit_ObjectClass, true );

            // generate the tree
            ICswNbtTree UnitsTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );

            // get the list of units
            _UnitNodes = new Collection<CswNbtNode>();
            UnitsTree.goToRoot();
            for( int i = 0; i < UnitsTree.getChildNodeCount(); i++ )
            {
                UnitsTree.goToNthChild( i );
                _UnitNodes.Add( UnitsTree.getNodeForCurrentPosition() );
                UnitsTree.goToParentNode();
            }

            _QuantitySubField = ( (CswNbtFieldTypeRuleQuantity) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).QuantitySubField;
            _UnitsSubField = ( (CswNbtFieldTypeRuleQuantity) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).UnitsSubField;

        }//CswNbtNodePropQuantity()


        private CswNbtSubField _QuantitySubField;
        private CswNbtSubField _UnitsSubField;


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
            }
        }//Gestalt

        public double Quantity
        {
            get
            {
                string Value = _CswNbtNodePropData.GetPropRowValue( _QuantitySubField.Column );
                if( CswTools.IsFloat( Value ) )
                    return Convert.ToDouble( Value );
                else
                    return Double.NaN;
            }
            set
            {
                string StringVal = string.Empty;
                if( Double.IsNaN( value ) )
                {
                    _CswNbtNodePropData.SetPropRowValue( _QuantitySubField.Column, Double.NaN );
                }
                else
                {
                    StringVal = Math.Round( value, Precision, MidpointRounding.AwayFromZero ).ToString();
                    _CswNbtNodePropData.SetPropRowValue( _QuantitySubField.Column, StringVal );
                }
                _SynchGestalt();
            }
        }

        public string Units
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _UnitsSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _UnitsSubField.Column, value );
                _SynchGestalt();
            }
        }

        private void _SynchGestalt()
        {
            string GestaltValue = _CswNbtNodePropData.GetPropRowValue( _QuantitySubField.Column ) + " " + _CswNbtNodePropData.GetPropRowValue( _UnitsSubField.Column );
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, GestaltValue );
        }


        public Int32 Precision
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.NumberPrecision != Int32.MinValue )
                    return _CswNbtMetaDataNodeTypeProp.NumberPrecision;
                else
                    return 0;
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

        private Collection<CswNbtNode> _UnitNodes;
        public Collection<CswNbtNode> UnitNodes
        {
            get
            {
                return _UnitNodes;
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode QtyNode = CswXmlDocument.AppendXmlNode( ParentNode, _QuantitySubField.ToXmlNodeName() );
            CswXmlDocument.AppendXmlAttribute( QtyNode, "minvalue", MinValue.ToString() );
            CswXmlDocument.AppendXmlAttribute( QtyNode, "maxvalue", MaxValue.ToString() );
            CswXmlDocument.AppendXmlAttribute( QtyNode, "precision", Precision.ToString() );
            if( !Double.IsNaN( Quantity ) )
            {
                QtyNode.InnerText = Quantity.ToString();
            }

            XmlNode UnitsNode = CswXmlDocument.AppendXmlNode( ParentNode, _UnitsSubField.ToXmlNodeName(), Units );
            foreach( CswNbtNode UnitNode in _UnitNodes )
            {
                XmlNode UnitOptionNode = CswXmlDocument.AppendXmlNode( UnitsNode, "option" );
                CswXmlDocument.AppendXmlAttribute( UnitOptionNode, "value", UnitNode.NodeName );
            }
        } // ToXml()

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _QuantitySubField.ToXmlNodeName( true ), ( !Double.IsNaN( Quantity ) ) ? Quantity.ToString() : string.Empty,
                new XAttribute( "minvalue", MinValue.ToString() ),
                new XAttribute( "maxvalue", MaxValue.ToString() ),
                new XAttribute( "precision", Precision.ToString() ) ) );

            XElement UnitsNode = new XElement( _UnitsSubField.ToXmlNodeName( true ), Units );
            ParentNode.Add( UnitsNode );

            foreach( CswNbtNode UnitNode in _UnitNodes )
            {
                UnitsNode.Add( new XElement( "option",
                    new XAttribute( "value", UnitNode.NodeName ) ) );
            }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_QuantitySubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Quantity ) ) ? Quantity.ToString() : string.Empty;

            ParentObject["minvalue"] = MinValue.ToString();
            ParentObject["maxvalue"] = MaxValue.ToString();
            ParentObject["precision"] = Precision.ToString();

            JArray UnitsNodeObj = new JArray();
            ParentObject[_UnitsSubField.ToXmlNodeName( true )] = UnitsNodeObj;

            foreach( CswNbtNode UnitNode in _UnitNodes )
            {
                UnitsNodeObj.Add( UnitNode.NodeName );
            }
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Quantity = CswXmlDocument.ChildXmlNodeValueAsDouble( XmlNode, _QuantitySubField.ToXmlNodeName() );
            Units = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _UnitsSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _QuantitySubField.ToXmlNodeName( true ) ) )
            {
                Quantity = CswConvert.ToDouble( XmlNode.Element( _QuantitySubField.ToXmlNodeName( true ) ).Value );
            }
            if( null != XmlNode.Element( _UnitsSubField.ToXmlNodeName( true ) ) )
            {
                Units = XmlNode.Element( _UnitsSubField.ToXmlNodeName( true ) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string StringVal = CswTools.XmlRealAttributeName( PropRow[_QuantitySubField.ToXmlNodeName()].ToString() );
            if( CswTools.IsFloat( StringVal ) )
                Quantity = Convert.ToDouble( StringVal );
            Units = CswTools.XmlRealAttributeName( PropRow[_UnitsSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject.Property( _QuantitySubField.ToXmlNodeName( true ) ) )
            {
                Quantity = CswConvert.ToDouble( JObject.Property( _QuantitySubField.ToXmlNodeName( true ) ).Value );
            }
            if( null != JObject.Property( _UnitsSubField.ToXmlNodeName( true ) ) )
            {
                Units = (string) JObject.Property( _UnitsSubField.ToXmlNodeName( true ) ).Value;
            }
        }
    }//CswNbtNodePropQuantity

}//namespace 
