using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropSequence : CswNbtNodeProp
    {
        public CswNbtNodePropSequence( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Sequence )
            {
                throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
                                            "CswNbtNodePropSequence() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _SequenceSubField = ( (CswNbtFieldTypeRuleSequence) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SequenceSubField;
            _SequenceNumberSubField = ( (CswNbtFieldTypeRuleSequence) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SequenceNumberSubField;

            _SequenceValue = new CswNbtSequenceValue( _CswNbtMetaDataNodeTypeProp.PropId, _CswNbtResources );
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


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//get

        }//Gestalt

        public string Sequence
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _SequenceSubField.Column );
            }
        }//Sequence

        public string SequenceNumber
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _SequenceNumberSubField.Column );
            }
        }//Sequence

        /// <summary>
        /// Sets Sequence to the next sequence value
        /// </summary>
        public void setSequenceValue()
        {
            if( Sequence.Trim() == string.Empty )
            {
                string value = _SequenceValue.Next;
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
            _CswNbtNodePropData.SetPropRowValue( _SequenceSubField.Column, SeqValue );
            Int32 ThisSeqValue = _SequenceValue.deformatSequence( SeqValue );
            _CswNbtNodePropData.SetPropRowValue( _SequenceNumberSubField.Column, ThisSeqValue );
            _CswNbtNodePropData.Gestalt = SeqValue;

            if( ResetSequence )
            {
                // Keep the sequence up to date
                _SequenceValue.reSync( ThisSeqValue );
            }
        }

        override public void onBeforeUpdateNodePropRow( bool IsCopy )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Sequence )
            {
                throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
                                            "CswNbtNodePropSequence.onBeforeUpdateNodePropRow() was called on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType.ToString() ) );
            }

            // Automatically generate a value.  This will not overwrite existing values.
            setSequenceValue();

            base.onBeforeUpdateNodePropRow( IsCopy );
        }//onBeforeUpdateNodePropRow()

        public override void Copy( CswNbtNodePropData Source )
        {
            // BZ 10498 - Don't copy, just generate a new value
            //base.onCopy();
            setSequenceValue();
        }


        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, _SequenceSubField.ToXmlNodeName(), Sequence );
            CswXmlDocument.AppendXmlNode( ParentNode, _SequenceNumberSubField.ToXmlNodeName(), SequenceNumber );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _SequenceSubField.ToXmlNodeName(true), Sequence ),
                new XElement( _SequenceNumberSubField.ToXmlNodeName(true), SequenceNumber ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject.Add( new JProperty( _SequenceSubField.ToXmlNodeName(true), Sequence ) );
            ParentObject.Add( new JProperty( _SequenceNumberSubField.ToXmlNodeName(true), SequenceNumber ) );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            _saveProp( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SequenceSubField.ToXmlNodeName() ) );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _SequenceSubField.ToXmlNodeName(true) ) )
            {
                _saveProp( XmlNode.Element( _SequenceSubField.ToXmlNodeName(true) ).Value );
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveSequence = CswTools.XmlRealAttributeName( PropRow[_SequenceSubField.ToXmlNodeName()].ToString() );
            if( ProspectiveSequence != string.Empty )
                setSequenceValueOverride( ProspectiveSequence, false );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject.Property( _SequenceSubField.ToXmlNodeName(true) ) )
            {
                _saveProp( (string) JObject.Property( _SequenceSubField.ToXmlNodeName(true) ).Value );
            }
        }

        private void _saveProp( string ProspectiveSequence )
        {
            if( ProspectiveSequence != string.Empty )
            {
                setSequenceValueOverride( ProspectiveSequence, false );
            }
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
