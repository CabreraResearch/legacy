using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropPropertyReference : CswNbtNodeProp
    {
        private CswNbtSubField _CachedValueSubField;

        private CswNbtSequenceValue _SequenceValue;
        private CswNbtSubField _SequenceSubField;
        private CswNbtSubField _SequenceNumberSubField;

        public static implicit operator CswNbtNodePropPropertyReference( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsPropertyReference;
        }

        public CswNbtNodePropPropertyReference( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _CachedValueSubField = ( (CswNbtFieldTypeRulePropertyReference) _FieldTypeRule ).CachedValueSubField;
            _SequenceSubField = ( (CswNbtFieldTypeRulePropertyReference) _FieldTypeRule ).SequenceSubField;
            _SequenceNumberSubField = ( (CswNbtFieldTypeRulePropertyReference) _FieldTypeRule ).SequenceNumberSubField;

            _SequenceValue = new CswNbtSequenceValue( NodeTypePropId, _CswNbtResources );

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _CachedValueSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => CachedValue, null ) );
            _SubFieldMethods.Add( _SequenceSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Sequence, x => setSequenceValueOverride( CswConvert.ToString( x ), true ) ) );
            _SubFieldMethods.Add( _SequenceNumberSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => SequenceNumber, null ) );
        }

        #region Generic Properties

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        private void _setGestalt( string PropRefVal, string SeqVal )
        {
            string NewGestalt = PropRefVal;
            if( UseSequence && false == String.IsNullOrEmpty( PropRefVal ) )
            {
                NewGestalt = PropRefVal + "-" + SeqVal;
            }
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, NewGestalt );
        }

        public string CachedValue
        {
            get
            {
                return GetPropRowValue( _CachedValueSubField );
            }
        }

        public void ClearCachedValue()
        {
            SetPropRowValue( _CachedValueSubField, DBNull.Value );
        }

        #endregion Generic Properties

        #region Relationship Properties and Functions

        public Int32 RelationshipId
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.FKValue;
                return CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRulePropertyReference.AttributeName.Relationship] );
            }
        }

        public CswEnumNbtViewPropIdType RelationshipType
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.FKType;
                return _CswNbtNodePropData[CswNbtFieldTypeRulePropertyReference.AttributeName.FKType];
            }
        }

        public Int32 RelatedPropId
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.ValuePropId;
                return CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedProperty] );
            }
        }

        public CswEnumNbtViewPropIdType RelatedPropType
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.ValuePropType;
                return _CswNbtNodePropData[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedPropType];
            }
        }

        public string RecalculateReferenceValue()
        {
            string Value = String.Empty;

            if( RelationshipId > 0 && RelatedPropId > 0 )
            {
                CswNbtMetaDataNodeTypeProp RelationshipdNtp = _getRelationshipProp();
                if( null != RelationshipdNtp )
                {
                    CswPrimaryKey RelatedNodeId = _Node.Properties[RelationshipdNtp].AsRelationship.RelatedNodeId;
                    CswNbtNode RelatedNode = _CswNbtResources.Nodes[RelatedNodeId];
                    if( null != RelatedNode )
                    {
                        CswNbtMetaDataNodeTypeProp ToReferenceNtp = null;

                        if( RelatedPropType == CswEnumNbtViewPropIdType.NodeTypePropId )
                        {
                            ToReferenceNtp = _CswNbtResources.MetaData.getNodeTypeProp( RelatedPropId );
                        }
                        else if( RelatedPropType == CswEnumNbtViewPropIdType.ObjectClassPropId )
                        {
                            ToReferenceNtp = RelatedNode.getNodeType().getNodeTypePropByObjectClassProp( RelatedPropId );
                        }
                        if( null != ToReferenceNtp )
                        {
                            Value = RelatedNode.Properties[ToReferenceNtp].Gestalt;
                        }
                    }
                }
            } // if (RelationshipId > 0 && RelatedPropId > 0)

            SetPropRowValue( _CachedValueSubField, Value );
            _setGestalt( Value, Sequence );
            PendingUpdate = false;

            return Value;
        }

        private CswNbtMetaDataNodeTypeProp _getRelationshipProp()
        {
            CswNbtMetaDataNodeTypeProp RelationshipProp = null;
            Int32 NodeTypeId = this.NodeTypeProp.getNodeType().NodeTypeId;
            if( RelationshipType == CswEnumNbtViewPropIdType.NodeTypePropId )
            {
                RelationshipProp = _CswNbtResources.MetaData.getNodeTypePropVersion( NodeTypeId, RelationshipId );
            }
            else if( RelationshipType == CswEnumNbtViewPropIdType.ObjectClassPropId )
            {
                RelationshipProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( NodeTypeId, RelationshipId );
            }
            if( RelationshipProp == null )
            {
                throw new CswDniException( "RecalculateReferenceValue(): RelationshipId is not valid:" + RelationshipId.ToString() );
            }
            return RelationshipProp;
        }

        #endregion Relationship Properties and Functions

        #region Sequence Properties and Functions

        public string Sequence
        {
            get
            {
                return GetPropRowValue( _SequenceSubField );
            }
        }

        public string SequenceNumber
        {
            get
            {
                return GetPropRowValue( _SequenceNumberSubField );
            }
        }

        /// <summary>
        /// When set to true, display Sequence alongside PropertyReference value
        /// </summary>
        public bool UseSequence
        {
            get
            {
                //return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                return CswConvert.ToBoolean( _CswNbtNodePropData[CswNbtFieldTypeRulePropertyReference.AttributeName.UseSequence] );
            }
        }

        /// <summary>
        /// Sets Sequence to the next sequence value
        /// </summary>
        public void setSequenceValue()
        {
            if( UseSequence && Sequence.Trim() == string.Empty )
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
            if( UseSequence )
            {
                SetPropRowValue( _SequenceSubField, SeqValue );
                Int32 ThisSeqValue = _SequenceValue.deformatSequence( SeqValue );
                SetPropRowValue( _SequenceNumberSubField, ThisSeqValue );
                _setGestalt( CachedValue, SeqValue );

                if( ResetSequence )
                {
                    // Keep the sequence up to date
                    _SequenceValue.reSync( ThisSeqValue );
                }
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void onBeforeUpdateNodePropRow()
        {
            setSequenceValue();
        }

        public override void Copy( CswNbtNodePropData Source )
        {
            // Don't copy, just generate a new value
            setSequenceValue();
        }

        #endregion Sequence Properties and Functions

        #region Serialization

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST

            ParentObject[_CachedValueSubField.ToXmlNodeName( true )] = CachedValue;
            ParentObject["useSequence"] = UseSequence.ToString();
            ParentObject[_SequenceSubField.ToXmlNodeName( true )] = Sequence;
            ParentObject[_SequenceNumberSubField.ToXmlNodeName( true )] = SequenceNumber;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing to restore
            PendingUpdate = true;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_SequenceSubField.ToXmlNodeName( true )] && false == String.IsNullOrEmpty( JObject[_SequenceSubField.ToXmlNodeName( true )].ToString() ) )
            {
                setSequenceValueOverride( JObject[_SequenceSubField.ToXmlNodeName( true )].ToString(), false );
            }
            PendingUpdate = true;
        }

        #endregion Serialization

        public override void SyncGestalt()
        {

        }

    }//CswNbtNodePropPropertyReference

}//namespace ChemSW.Nbt.PropTypes
