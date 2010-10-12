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
    public class CswNbtNodePropSequence : CswNbtNodeProp
    {
        public CswNbtNodePropSequence( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Sequence )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropSequence() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _SequenceSubField = ( (CswNbtFieldTypeRuleSequence) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SequenceSubField;
        }

        private CswNbtSubField _SequenceSubField;


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

        /// <summary>
        /// Sets Sequence to the next sequence value
        /// </summary>
        public void SetSequenceValue()
        {
            if( Sequence.Trim() == string.Empty )
            {
                CswNbtSequenceValue CswNbtSequenceValue = new CswNbtSequenceValue( _CswNbtResources );
                CswNbtSequenceValue.NodeTypePropId = _CswNbtMetaDataNodeTypeProp.PropId;
                string value = CswNbtSequenceValue.Next;
                SetSequenceValueOverride( value, false );
            }
        }

        /// <summary>
        /// Sets Sequence to the provided value.  
        /// This allows manually overriding automatically generated sequence values.  Use carefully.
        /// </summary>
        /// <param name="value">Value to set for Sequence</param>
        /// <param name="ResetSequence">True if the sequence needs to be reset to this value 
        /// (set true if the value was not just generated from the sequence)</param>
        public void SetSequenceValueOverride( string value, bool ResetSequence )
        {
            _CswNbtNodePropData.SetPropRowValue( _SequenceSubField.Column, value );
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
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Sequence )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropSequence.onBeforeUpdateNodePropRow() was called on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType.ToString() ) );
            }

            // Automatically generate a value.  This will not overwrite existing values.
            SetSequenceValue();

            base.onBeforeUpdateNodePropRow( IsCopy );
        }//onBeforeUpdateNodePropRow()

        public override void Copy( CswNbtNodePropData Source )
        {
            // BZ 10498 - Don't copy, just generate a new value
            //base.onCopy();
            SetSequenceValue();
        }


        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SequenceNode = CswXmlDocument.AppendXmlNode( ParentNode, _SequenceSubField.Name.ToString(), Sequence );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveSequence = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SequenceSubField.Name.ToString() );
            if( ProspectiveSequence != string.Empty )
                SetSequenceValueOverride( ProspectiveSequence, false );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string ProspectiveSequence = CswTools.XmlRealAttributeName( PropRow[_SequenceSubField.Name.ToString()].ToString() );
            if( ProspectiveSequence != string.Empty )
                SetSequenceValueOverride( ProspectiveSequence, false );
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
