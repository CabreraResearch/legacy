using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;
using System.Xml;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropBarcode : CswNbtNodeProp
    {
        public const string AutoSignal = "[auto]";

        public CswNbtNodePropBarcode( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Barcode )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropBarcode() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _BarcodeSubField = ( (CswNbtFieldTypeRuleBarCode) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).BarcodeSubField;
            _SequenceNumberSubField = ( (CswNbtFieldTypeRuleBarCode) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SequenceNumberSubField;

            _SequenceValue = new CswNbtSequenceValue( _CswNbtMetaDataNodeTypeProp.PropId, _CswNbtResources );

        }//CswNbtNodePropBarcode()
        private CswNbtSequenceValue _SequenceValue;
        private CswNbtSubField _BarcodeSubField;
        private CswNbtSubField _SequenceNumberSubField;
          
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

        public string SequenceNumber
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _SequenceNumberSubField.Column );
            }
        }//SequenceNumber

        /// <summary>
        /// Sets Barcode to the next sequence value
        /// </summary>
        public bool setBarcodeValue()
        {
            bool Succeeded = false;
            if( Barcode.Trim() == string.Empty )
            {
                string value = _SequenceValue.Next;
                Succeeded = setBarcodeValueOverride( value, false );
            }
            return Succeeded;
        }

        /// <summary>
        /// Sets Barcode to the provided value.  
        /// This allows manually overriding automatically generated sequence values.  Use carefully.
        /// </summary>
        /// <param name="SeqValue">Value to set for Barcode</param>
        /// <param name="ResetSequence">True if the sequence needs to be reset to this value 
        /// (set true if the value was not just generated from the sequence)</param>
        public bool setBarcodeValueOverride( string SeqValue, bool ResetSequence )
        {
            bool Succeeded = _CswNbtNodePropData.SetPropRowValue( _BarcodeSubField.Column, SeqValue );
            Succeeded = ( Succeeded && _CswNbtNodePropData.SetPropRowValue( _SequenceNumberSubField.Column, _SequenceValue.deformatSequence( SeqValue ) ) );
            _CswNbtNodePropData.Gestalt = SeqValue;

            if( ResetSequence )
            {
                // Keep the sequence up to date
                Int32 ThisSeqValue = CswConvert.ToInt32( SeqValue );
                _SequenceValue.reSync( ThisSeqValue );
            }
            return Succeeded;
        }

        override public void onBeforeUpdateNodePropRow( bool IsCopy )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Barcode )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropBarcode.onBeforeUpdateNodePropRow() was called on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType.ToString() ) );
            }

            // Automatically generate a value.  This will not overwrite existing values.
            setBarcodeValue();

            base.onBeforeUpdateNodePropRow( IsCopy );
        }//onBeforeUpdateNodePropRow()

        public override void Copy( CswNbtNodePropData Source )
        {
            // BZ 10498 - Don't copy, just generate a new value
            // Waiting on disucssion w/ TDU
            //String Barcode = Source.NodeTypeProp.DefaultValue.AsBarcode.Barcode.ToString();
            //if( string.IsNullOrEmpty( Barcode ) )
            //{
                setBarcodeValue();
            //}
            //else // Case 20784
            //{
            //    setBarcodeValueOverride( Barcode, false );
            //}
        }
    


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode BarcodeNode = CswXmlDocument.AppendXmlNode( ParentNode, _BarcodeSubField.ToXmlNodeName(), Barcode );
            XmlNode SequenceNumberNode = CswXmlDocument.AppendXmlNode( ParentNode, _SequenceNumberSubField.ToXmlNodeName(), SequenceNumber );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveBarcode = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _BarcodeSubField.ToXmlNodeName() );
            if( ProspectiveBarcode != string.Empty )
                setBarcodeValueOverride( ProspectiveBarcode, false );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveBarcode = CswTools.XmlRealAttributeName( PropRow[_BarcodeSubField.ToXmlNodeName()].ToString() );
            if( ProspectiveBarcode != string.Empty )
                setBarcodeValueOverride( ProspectiveBarcode, false );
        }

    }//CswNbtNodePropQuantity

}//namespace 
