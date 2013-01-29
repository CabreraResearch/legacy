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
        private CswNbtFieldTypeRulePropertyReference _FieldTypeRule;
        private CswNbtSubField _CachedValueSubField;

        private CswNbtSequenceValue _SequenceValue;
        private CswNbtSubField _SequenceSubField;
        private CswNbtSubField _SequenceNumberSubField;
        private CswNbtNode _Node;

        public static implicit operator CswNbtNodePropPropertyReference( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsPropertyReference;
        }

        public CswNbtNodePropPropertyReference( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _Node = Node;

            _FieldTypeRule = (CswNbtFieldTypeRulePropertyReference) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _CachedValueSubField = _FieldTypeRule.CachedValueSubField;

            _SequenceSubField = _FieldTypeRule.SequenceSubField;
            _SequenceNumberSubField = _FieldTypeRule.SequenceNumberSubField;
            _SequenceValue = new CswNbtSequenceValue( _CswNbtMetaDataNodeTypeProp.PropId, _CswNbtResources );
        }

        #region Generic Properties

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
        }

        private void _setGestalt( string PropRefVal, string SeqVal )
        {
            string NewGestalt = PropRefVal;
            if( UseSequence && false == String.IsNullOrEmpty( PropRefVal ) )
            {
                NewGestalt = PropRefVal + "-" + SeqVal;
            }
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, NewGestalt );
        }

        public string CachedValue
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _CachedValueSubField.Column );
            }
        }

        public void ClearCachedValue()
        {
            _CswNbtNodePropData.SetPropRowValue( _CachedValueSubField.Column, DBNull.Value );
        }

        #endregion Generic Properties

        #region Relationship Properties and Functions

        public Int32 RelationshipId
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.FKValue;
            }
        }

        public NbtViewPropIdType RelationshipType
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.FKType;
            }
        }

        public Int32 RelatedPropId
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.ValuePropId;
            }
        }

        public NbtViewPropIdType RelatedPropType
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.ValuePropType;
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

                        if( RelatedPropType == NbtViewPropIdType.NodeTypePropId )
                        {
                            ToReferenceNtp = _CswNbtResources.MetaData.getNodeTypeProp( RelatedPropId );
                        }
                        else if( RelatedPropType == NbtViewPropIdType.ObjectClassPropId )
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

            _CswNbtNodePropData.SetPropRowValue( _CachedValueSubField.Column, Value );
            _setGestalt( Value, Sequence );
            _CswNbtNodePropData.PendingUpdate = false;

            return Value;
        }

        private CswNbtMetaDataNodeTypeProp _getRelationshipProp()
        {
            CswNbtMetaDataNodeTypeProp RelationshipProp = null;
            Int32 NodeTypeId = this.NodeTypeProp.getNodeType().NodeTypeId;
            if( RelationshipType == NbtViewPropIdType.NodeTypePropId )
            {
                RelationshipProp = _CswNbtResources.MetaData.getNodeTypePropVersion( NodeTypeId, RelationshipId );
            }
            else if( RelationshipType == NbtViewPropIdType.ObjectClassPropId )
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
                return _CswNbtNodePropData.GetPropRowValue( _SequenceSubField.Column );
            }
        }

        public string SequenceNumber
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _SequenceNumberSubField.Column );
            }
        }

        /// <summary>
        /// When set to true, display Sequence alongside PropertyReference value
        /// </summary>
        public bool UseSequence
        {
            get
            {
                return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute1 );
            }
        }

        /// <summary>
        /// Sets Sequence to the next sequence value
        /// </summary>
        public void setSequenceValue()
        {
            if( UseSequence && Sequence.Trim() == string.Empty )
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
            if( UseSequence )
            {
                _CswNbtNodePropData.SetPropRowValue( _SequenceSubField.Column, SeqValue );
                Int32 ThisSeqValue = _SequenceValue.deformatSequence( SeqValue );
                _CswNbtNodePropData.SetPropRowValue( _SequenceNumberSubField.Column, ThisSeqValue );
                _setGestalt( CachedValue, SeqValue );

                if( ResetSequence )
                {
                    // Keep the sequence up to date
                    _SequenceValue.reSync( ThisSeqValue );
                }
            }
        }

        override public void onBeforeUpdateNodePropRow( bool IsCopy, bool OverrideUniqueValidation )
        {
            // Automatically generate a value.  This will not overwrite existing values.
            setSequenceValue();
            base.onBeforeUpdateNodePropRow( IsCopy, OverrideUniqueValidation );
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
