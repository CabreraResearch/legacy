using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using System.Xml;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropBarcode : CswNbtNodeProp
    {
        public static string AutoSignal = "[auto]";

        public CswNbtNodePropBarcode( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Barcode )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropBarcode() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _BarcodeSubField = ( (CswNbtFieldTypeRuleBarCode) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).BarcodeSubField;

        }//CswNbtNodePropBarcode()

        private CswNbtSubField _BarcodeSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }//Empty

        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }
        }//Gestalt

        public string Barcode
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _BarcodeSubField.Column );
            }
        }//Barcode

        /// <summary>
        /// Sets Barcode to the next sequence value
        /// </summary>
        public void SetBarcodeValue()
        {
            if( Barcode.Trim() == string.Empty )
            {
                CswNbtSequenceValue CswNbtSequenceValue = new CswNbtSequenceValue( _CswNbtResources );
                CswNbtSequenceValue.NodeTypePropId = _CswNbtMetaDataNodeTypeProp.PropId;
                string value = CswNbtSequenceValue.Next;
                SetBarcodeValueOverride( value, false );
            }
        }

        /// <summary>
        /// Sets Barcode to the provided value.  
        /// This allows manually overriding automatically generated sequence values.  Use carefully.
        /// </summary>
        /// <param name="value">Value to set for Barcode</param>
        /// <param name="ResetSequence">True if the sequence needs to be reset to this value 
        /// (set true if the value was not just generated from the sequence)</param>
        public void SetBarcodeValueOverride( string value, bool ResetSequence )
        {
            _CswNbtNodePropData.SetPropRowValue( _BarcodeSubField.Column, value );
            _CswNbtNodePropData.Gestalt = value;

            if( ResetSequence )
            {
                // Keep the sequence up to date
                CswNbtSequenceValue CswNbtSequenceValue = new CswNbtSequenceValue( _CswNbtResources );
                CswNbtSequenceValue.NodeTypePropId = _CswNbtMetaDataNodeTypeProp.PropId;
                CswNbtSequenceValue.Reset( value );
            }
        }

        override public void onBeforeUpdateNodePropRow( bool IsCopy )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Barcode )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropBarcode.onBeforeUpdateNodePropRow() was called on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType.ToString() ) );
            }

            // Automatically generate a value.  This will not overwrite existing values.
            SetBarcodeValue();

            base.onBeforeUpdateNodePropRow( IsCopy );
        }//onBeforeUpdateNodePropRow()

        public override void Copy( CswNbtNodePropData Source )
        {
            // BZ 10498 - Don't copy, just generate a new value
            //base.onCopy();
            SetBarcodeValue();
        }


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode BarcodeNode = CswXmlDocument.AppendXmlNode( ParentNode, _BarcodeSubField.ToXmlNodeName(), Barcode );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveBarcode = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _BarcodeSubField.ToXmlNodeName() );
            if( ProspectiveBarcode != string.Empty )
                SetBarcodeValueOverride( ProspectiveBarcode, false );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveBarcode = CswTools.XmlRealAttributeName( PropRow[_BarcodeSubField.ToXmlNodeName()].ToString() );
            if( ProspectiveBarcode != string.Empty )
                SetBarcodeValueOverride( ProspectiveBarcode, false );
        }

    }//CswNbtNodePropQuantity

}//namespace 
