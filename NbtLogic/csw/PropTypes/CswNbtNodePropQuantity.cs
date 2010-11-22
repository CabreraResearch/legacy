using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropQuantity : CswNbtNodeProp
    {
        public CswNbtNodePropQuantity( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Quantity )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
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
            _UnitNodes = new ArrayList();
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
                    _CswNbtNodePropData.SetPropRowValue( _QuantitySubField.Column, CswConvert.ToDbVal( Double.NaN ) );
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
                _CswNbtNodePropData.SetPropRowValue( _UnitsSubField.Column, value.ToString() );
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

        private ArrayList _UnitNodes;
        public ArrayList UnitNodes
        {
            get
            {
                return _UnitNodes;
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode QtyNode = CswXmlDocument.AppendXmlNode( ParentNode, _QuantitySubField.ToXmlNodeName(), Quantity.ToString() );
            XmlNode UnitsNode = CswXmlDocument.AppendXmlNode( ParentNode, _UnitsSubField.ToXmlNodeName(), Units );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Quantity = CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _QuantitySubField.ToXmlNodeName() );
            Units = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _UnitsSubField.ToXmlNodeName() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string StringVal = CswTools.XmlRealAttributeName( PropRow[_QuantitySubField.ToXmlNodeName()].ToString() );
            if( CswTools.IsFloat( StringVal ) )
                Quantity = Convert.ToDouble( StringVal );
            Units = CswTools.XmlRealAttributeName( PropRow[_UnitsSubField.ToXmlNodeName()].ToString() );
        }


    }//CswNbtNodePropQuantity

}//namespace 
