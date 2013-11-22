using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActMerge
    {
        private CswNbtResources _CswNbtResources = null;

        public CswNbtActMerge( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }


        #region WCF

        [DataContract]
        public class MergeInfoData
        {
            public MergeInfoData()
            {
                Properties = new Collection<MergeInfoProperty>();
            }

            [DataMember]
            public Collection<MergeInfoProperty> Properties;

            [DataContract]
            public class MergeInfoProperty
            {
                [DataMember]
                public string PropName;
                [DataMember]
                public Int32 NodeTypePropId;
                [DataMember]
                public string Node1Value;
                [DataMember]
                public string Node2Value;
                [DataMember]
                public Int32 Choice = 1;
            }
        }

        #endregion WCF

        public MergeInfoData getMergeInfo( CswPrimaryKey NodeId1, CswPrimaryKey NodeId2 )
        {
            MergeInfoData ret = new MergeInfoData();

            CswNbtNode Node1 = _CswNbtResources.Nodes[NodeId1];
            CswNbtNode Node2 = _CswNbtResources.Nodes[NodeId2];

            if( null != Node1 && null != Node2 )
            {
                foreach( CswNbtNodePropWrapper Prop1 in Node1.Properties )
                {
                    CswNbtNodePropWrapper Prop2 = Node2.Properties[Prop1.NodeTypePropId];
                    if( null == Prop2 || Prop1.Gestalt != Prop2.Gestalt )
                    {
                        ret.Properties.Add( new MergeInfoData.MergeInfoProperty()
                            {
                                PropName = Prop1.PropName,
                                NodeTypePropId = Prop1.NodeTypePropId,
                                Node1Value = Prop1.Gestalt,
                                Node2Value = Prop2.Gestalt
                            } );
                    }
                }
            }
            return ret;
        }

        public CswNbtNode applyMergeChoices( CswPrimaryKey NodeId1, CswPrimaryKey NodeId2, MergeInfoData Choices )
        {
            CswNbtNode ret = null;

            CswNbtNode Node1 = _CswNbtResources.Nodes[NodeId1];
            CswNbtNode Node2 = _CswNbtResources.Nodes[NodeId2];
            if( null != Node1 && null != Node2 )
            {
                ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( Node1.NodeTypeId, IsTemp: true, OverrideUniqueValidation: true, OnAfterMakeNode: delegate( CswNbtNode newNode )
                    {
                        newNode.copyPropertyValues( Node1 );
                        foreach( MergeInfoData.MergeInfoProperty mergeProp in Choices.Properties )
                        {
                            if( mergeProp.Choice == 2 )
                            {
                                newNode.Properties[mergeProp.NodeTypePropId].copy( Node2.Properties[mergeProp.NodeTypePropId] );
                            }
                        }
                    } );
            }
            return ret;
        }
    }
}
