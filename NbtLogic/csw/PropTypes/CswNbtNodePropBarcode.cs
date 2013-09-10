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

    public class CswNbtNodePropBarcode : CswNbtNodeProp
    {
        public const string AutoSignal = "[auto]";

        public static implicit operator CswNbtNodePropBarcode( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsBarcode;
        }

        public CswNbtNodePropBarcode( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _BarcodeSubField = ( (CswNbtFieldTypeRuleBarCode) _FieldTypeRule ).BarcodeSubField;
            _SequenceNumberSubField = ( (CswNbtFieldTypeRuleBarCode) _FieldTypeRule ).SequenceNumberSubField;

            _SequenceValue = new CswNbtSequenceValue( _CswNbtMetaDataNodeTypeProp.PropId, _CswNbtResources );


            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _BarcodeSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Barcode, x => setBarcodeValueOverride( CswConvert.ToString( x ), true ) ) );
            _SubFieldMethods.Add( _SequenceNumberSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => SequenceNumber, null ) );
        } //CswNbtNodePropBarcode()

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
        
        public string Barcode
        {
            get
            {
                return GetPropRowValue( _BarcodeSubField.Column );
            }
        }//Barcode

        public Int32 SequenceNumber
        {
            get
            {
                return CswConvert.ToInt32( GetPropRowValue( _SequenceNumberSubField.Column ) );
            }
        }//SequenceNumber

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        /// <summary>
        /// Sets Barcode to the next sequence value
        /// </summary>
        public bool setBarcodeValue( bool OverrideExisting = false )
        {
            bool Succeeded = false;
            if( Barcode.Trim() == string.Empty || OverrideExisting )
            {
                string value = _SequenceValue.Next;
                Succeeded = setBarcodeValueOverride( value, false );
            }
            return Succeeded;
        }

        /// <summary>
        /// Resets SequenceNumber to the numeric portion of the Barcode
        /// </summary>
        public void resetSequenceNumber()
        {
            // Fix missing sequence number
            Int32 ThisSeqValue = _SequenceValue.deformatSequence( Barcode );
            SetPropRowValue( _SequenceNumberSubField.Column, ThisSeqValue );
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
            bool Succeeded = SetPropRowValue( _BarcodeSubField.Column, SeqValue );
            Int32 ThisSeqValue = _SequenceValue.deformatSequence( SeqValue );
            Succeeded = ( Succeeded && SetPropRowValue( _SequenceNumberSubField.Column, ThisSeqValue ) );
            Gestalt = SeqValue;

            if( ResetSequence )
            {
                // Keep the sequence up to date
                _SequenceValue.reSync( ThisSeqValue );
            }
            return Succeeded;
        }

        override public void onBeforeUpdateNodePropRow( CswNbtNode Node, bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            //if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswEnumNbtFieldType.Barcode )
            //{
            //    throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
            //                                "CswNbtNodePropBarcode.onBeforeUpdateNodePropRow() was called on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType.ToString() ) );
            //}

            // Automatically generate a value.  This will not overwrite existing values.
            setBarcodeValue();

            base.onBeforeUpdateNodePropRow( Node, IsCopy, OverrideUniqueValidation, Creating );
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

        // ReadXml()

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_SequenceNumberSubField.ToXmlNodeName( true )] = SequenceNumber;
            ParentObject[_BarcodeSubField.ToXmlNodeName( true )] = Barcode;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            _setPropVals( CswTools.XmlRealAttributeName( PropRow[_BarcodeSubField.ToXmlNodeName()].ToString() ) );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_BarcodeSubField.ToXmlNodeName( true )] )
            {
                _setPropVals( JObject[_BarcodeSubField.ToXmlNodeName( true )].ToString() );
            }
        }

        private void _setPropVals( string ProspectiveBarcode )
        {
            if( ProspectiveBarcode != string.Empty )
            {
                setBarcodeValueOverride( ProspectiveBarcode, false );
            }
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Barcode );
        }
    }//CswNbtNodePropQuantity

}//namespace 
