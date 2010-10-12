using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropPropertyReference : CswNbtNodeProp
    {
        public CswNbtNodePropPropertyReference(CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp)
            : base(CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp)
        {
            _CachedValueSubField = ( (CswNbtFieldTypeRulePropertyReference) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CachedValueSubField;
        }

        private CswNbtSubField _CachedValueSubField;

        override public bool Empty
        {
            get
            {
                return (0 == Gestalt.Length);
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
                return _CswNbtNodePropData.GetPropRowValue(_CachedValueSubField.Column);
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
        public CswNbtViewRelationship.PropIdType RelationshipType
        {
            get
            {
                return CswNbtViewRelationship.PropIdType.NodeTypePropId;
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
        public CswNbtViewRelationship.PropIdType RelatedPropType
        {
            get
            {
                if (_CswNbtMetaDataNodeTypeProp.ValuePropType != String.Empty)
                    return (CswNbtViewRelationship.PropIdType)Enum.Parse(typeof(CswNbtViewRelationship.PropIdType), _CswNbtMetaDataNodeTypeProp.ValuePropType, true);
                else
                    return CswNbtViewRelationship.PropIdType.Unknown;
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

                CswNbtMetaDataNodeTypeProp RelationshipProp = _CswNbtResources.MetaData.getNodeTypeProp( RelationshipId );
                if( RelationshipProp == null )
                    throw new CswDniException( "RecalculateReferenceValue(): RelationshipId is not valid:" + RelationshipId.ToString() );

                ICswNbtMetaDataProp RelatedProp = null;
                if( RelatedPropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                    RelatedProp = _CswNbtResources.MetaData.getNodeTypeProp( RelatedPropId );
                else
                    RelatedProp = _CswNbtResources.MetaData.getObjectClassProp( RelatedPropId );

                if( RelatedProp == null )
                    throw new CswDniException( "RecalculateReferenceValue(): RelatedPropId is not valid:" + RelatedPropId.ToString() );

                CswNbtView ReferenceView = new CswNbtView( _CswNbtResources );
                ReferenceView.Root.Selectable = false;
                ReferenceView.ViewName = "CswNbtNodePropPropertyReference.RecalculateReferenceValue()";

                CswNbtViewRelationship ThisNodeRelationship = ReferenceView.AddViewRelationship( _CswNbtMetaDataNodeTypeProp.NodeType, false );
                ThisNodeRelationship.NodeIdsToFilterIn.Add( _CswNbtNodePropData.NodeId );

                if( RelationshipType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                    ReferenceView.AddViewRelationship( ThisNodeRelationship, CswNbtViewRelationship.PropOwnerType.First, _CswNbtResources.MetaData.getNodeTypeProp( RelationshipId ), false );
                else
                    ReferenceView.AddViewRelationship( ThisNodeRelationship, CswNbtViewRelationship.PropOwnerType.First, _CswNbtResources.MetaData.getObjectClassProp( RelationshipId ), false ); 

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
                        if( RelatedPropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                        {
                            Value = ( (CswNbtNodePropWrapper) RelatedNode.Properties[_CswNbtResources.MetaData.getNodeTypeProp( RelatedPropId )] ).Gestalt;
                        }
                        else
                        {
                            foreach( CswNbtNodePropWrapper Prop in RelatedNode.Properties )
                            {
                                if( Prop.ObjectClassPropId == RelatedPropId )
                                    Value = Prop.Gestalt;
                            }
                        }
                    }
                }
            } // if (RelationshipId > 0 && RelatedPropId > 0)

            _CswNbtNodePropData.Field1 = Value;
            _CswNbtNodePropData.Gestalt = Value;
            _CswNbtNodePropData.PendingUpdate = false;

            return Value;
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode ValueNode = CswXmlDocument.AppendXmlNode( ParentNode, _CachedValueSubField.Name.ToString(), CachedValue );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing to restore
            PendingUpdate = true;
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //nothing to restore
            PendingUpdate = true;
        }

    }//CswNbtNodePropPropertyReference

}//namespace ChemSW.Nbt.PropTypes
