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
    public class CswNbtNodePropSequence : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropSequence( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsSequence;
        }

        public CswNbtNodePropSequence( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _SequenceSubField = ( (CswNbtFieldTypeRuleSequence) _FieldTypeRule ).SequenceSubField;
            _SequenceNumberSubField = ( (CswNbtFieldTypeRuleSequence) _FieldTypeRule ).SequenceNumberSubField;

            _SequenceValue = new CswNbtSequenceValue( NodeTypePropId, _CswNbtResources );

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _SequenceSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Sequence, x => setSequenceValueOverride( CswConvert.ToString( x ), true ) ) );
            _SubFieldMethods.Add( _SequenceNumberSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => SequenceNumber, null ) );
        }

        private CswNbtSequenceValue _SequenceValue;
        private CswNbtSubField _SequenceSubField;
        private CswNbtSubField _SequenceNumberSubField;


        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }//Empty

        public string Sequence
        {
            get
            {
                return GetPropRowValue( _SequenceSubField );
            }
        }//Sequence

        public string SequenceNumber
        {
            get
            {
                return GetPropRowValue( _SequenceNumberSubField );
            }
        }//Sequence

        /// <summary>
        /// Sets Sequence to the next sequence value
        /// </summary>
        public void setSequenceValue()
        {
            if( Sequence.Trim() == string.Empty )
            {
                string value = _SequenceValue.getNext();
                setSequenceValueOverride( value, false );
            }
        }

        /// <summary>
        /// Sets Sequence to the provided value.  
        /// This allows manually overriding automatically generated sequence values.  Use carefully.
        /// </summary>
        /// <param name="SeqValue">Value to set for Sequence</param>
        /// <param name="ResetSequence">True if the sequence needs to be reset to this value 
        /// (set true if the value was not just generated from the sequence)</param>
        public void setSequenceValueOverride( string SeqValue, bool ResetSequence )
        {
            SetPropRowValue( _SequenceSubField, SeqValue );
            Int32 ThisSeqValue = _SequenceValue.deformatSequence( SeqValue );
            SetPropRowValue( _SequenceNumberSubField, ThisSeqValue );
            Gestalt = SeqValue;

            if( ResetSequence )
            {
                // Keep the sequence up to date
                _SequenceValue.reSync( ThisSeqValue );
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        override public void onBeforeUpdateNodePropRow()
        {
            setSequenceValue();
        }//onBeforeUpdateNodePropRow()

        public override void Copy( CswNbtNodePropData Source )
        {
            // BZ 10498 - Don't copy, just generate a new value
            //base.onCopy();
            setSequenceValue();
        }

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST

            ParentObject[_SequenceSubField.ToXmlNodeName( true )] = Sequence;
            ParentObject[_SequenceNumberSubField.ToXmlNodeName( true )] = SequenceNumber;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveSequence = CswTools.XmlRealAttributeName( PropRow[_SequenceSubField.ToXmlNodeName()].ToString() );
            if( ProspectiveSequence != string.Empty )
                setSequenceValueOverride( ProspectiveSequence, false );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_SequenceSubField.ToXmlNodeName( true )] )
            {
                _saveProp( JObject[_SequenceSubField.ToXmlNodeName( true )].ToString() );
            }
        }

        private void _saveProp( string ProspectiveSequence )
        {
            if( ProspectiveSequence != string.Empty )
            {
                setSequenceValueOverride( ProspectiveSequence, false );
            }
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, SequenceNumber );
        }

        public override bool onBeforeSetDefault()
        {
            return DefaultValue.AsSequence.Sequence != CswNbtNodePropBarcode.AutoSignal;
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
