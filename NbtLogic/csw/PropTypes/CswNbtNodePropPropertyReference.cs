using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropPropertyReference : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropPropertyReference( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsPropertyReference;
        }

        public CswNbtNodePropPropertyReference( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRulePropertyReference) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _CachedValueSubField = _FieldTypeRule.CachedValueSubField;
        }
        private CswNbtFieldTypeRulePropertyReference _FieldTypeRule;
        private CswNbtSubField _CachedValueSubField;

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

        public Int32 RelationshipId
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.FKValue;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.FKValue = value;
            //}
        }
        public NbtViewPropIdType RelationshipType
        {
            get
            {
                return (NbtViewPropIdType) _CswNbtMetaDataNodeTypeProp.FKType;
            }
        }

        public Int32 RelatedPropId
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.ValuePropId;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.ValuePropId = value;
            //}
        }
        public NbtViewPropIdType RelatedPropType
        {
            get
            {
                //if( _CswNbtMetaDataNodeTypeProp.ValuePropType != String.Empty )
                //    return (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), _CswNbtMetaDataNodeTypeProp.ValuePropType, true );
                //else
                //    return NbtViewPropIdType.Unknown;
                return (NbtViewPropIdType) _CswNbtMetaDataNodeTypeProp.ValuePropType;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.ValuePropType = value.ToString();
            //}
        }


        public string RecalculateReferenceValue()
        {
            string Value = String.Empty;

            if( RelationshipId > 0 && RelatedPropId > 0 )
            {
                if( _CswNbtResources == null )
                    throw new CswDniException( "RecalculateReferenceValue(): _CswNbtResources is null" );

                if( _CswNbtMetaDataNodeTypeProp == null )
                    throw new CswDniException( "RecalculateReferenceValue(): _CswNbtMetaDataNodeTypeProp is null" );

                Int32 NodeTypeId = this.NodeTypeProp.getNodeType().NodeTypeId;
                CswNbtMetaDataNodeTypeProp RelationshipProp = null;
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

                //ICswNbtMetaDataProp RelatedProp = null;
                //if( RelatedPropType == NbtViewPropIdType.NodeTypePropId )
                //    RelatedProp = _CswNbtResources.MetaData.getNodeTypeProp( RelatedPropId );
                //else
                //    RelatedProp = _CswNbtResources.MetaData.getObjectClassProp( RelatedPropId );

                //if( RelatedProp == null )
                //    throw new CswDniException( "RecalculateReferenceValue(): RelatedPropId is not valid:" + RelatedPropId.ToString() );

                CswNbtView ReferenceView = new CswNbtView( _CswNbtResources );
                ReferenceView.Root.Selectable = false;
                ReferenceView.ViewName = "CswNbtNodePropPropertyReference.RecalculateReferenceValue()";

                CswNbtViewRelationship ThisNodeRelationship = ReferenceView.AddViewRelationship( _CswNbtMetaDataNodeTypeProp.getNodeType(), false );
                ThisNodeRelationship.NodeIdsToFilterIn.Add( _CswNbtNodePropData.NodeId );

                ReferenceView.AddViewRelationship( ThisNodeRelationship, NbtViewPropOwnerType.First, RelationshipProp, false );

                //ReferenceView.Root.NodeIdsToFilterIn.Add(_CswNbtNodePropData.NodeId);
                //ReferenceView.Root.FilterInNodesRecursively = false;
                //ICswNbtTree ReferenceTree = _CswNbtResources.Trees.getTreeFromView(ReferenceView, _CswNbtNodePropData.NodeId, true);
                ICswNbtTree ReferenceTree = _CswNbtResources.Trees.getTreeFromView( ReferenceView, true, true, false, false );
                if( ReferenceTree == null )
                    throw new CswDniException( "RecalculateReferenceValue(): ReferenceTree is null" );

                ReferenceTree.goToRoot();

                if( ReferenceTree.getChildNodeCount() > 0 )
                {
                    ReferenceTree.goToNthChild( 0 );
                    if( ReferenceTree.getChildNodeCount() > 0 )
                    {
                        ReferenceTree.goToNthChild( 0 );
                        CswNbtNode RelatedNode = ReferenceTree.getNodeForCurrentPosition();
                        //if( RelatedPropType == NbtViewPropIdType.NodeTypePropId )
                        //{
                        //    Value = ( (CswNbtNodePropWrapper) RelatedNode.Properties[_CswNbtResources.MetaData.getNodeTypePropVersion( RelatedNode.NodeTypeId, RelatedPropId )] ).Gestalt;
                        //}
                        //else
                        //{
                        //    foreach( CswNbtNodePropWrapper Prop in RelatedNode.Properties )
                        //    {
                        //        if( Prop.ObjectClassPropId == RelatedPropId )
                        //            Value = Prop.Gestalt;
                        //    }
                        //}

                        // Match by propname

                        CswNbtMetaDataNodeTypeProp StoredRelatedProp = null;
                        if( RelatedPropType == NbtViewPropIdType.NodeTypePropId )
                        {
                            StoredRelatedProp = _CswNbtResources.MetaData.getNodeTypeProp( RelatedPropId );
                        }
                        else if( RelatedPropType == NbtViewPropIdType.ObjectClassPropId )
                        {
                            StoredRelatedProp = RelatedNode.getNodeType().getNodeTypePropByObjectClassProp( RelatedPropId );
                        }

                        if( null != StoredRelatedProp )
                        {
                            CswNbtMetaDataNodeTypeProp ActualRelatedProp = RelatedNode.getNodeType().getNodeTypeProp( StoredRelatedProp.PropName );
                            if( ActualRelatedProp != null )
                            {
                                Value = RelatedNode.Properties[ActualRelatedProp].Gestalt;
                            }
                        }
                    }
                }
            } // if (RelationshipId > 0 && RelatedPropId > 0)

            _CswNbtNodePropData.SetPropRowValue( _CachedValueSubField.Column, Value );
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, Value );
            _CswNbtNodePropData.PendingUpdate = false;

            return Value;
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_CachedValueSubField.ToXmlNodeName( true )] = CachedValue;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing to restore
            PendingUpdate = true;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing to restore
            PendingUpdate = true;
        }
    }//CswNbtNodePropPropertyReference

}//namespace ChemSW.Nbt.PropTypes
