
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{

    [DataContract]
    public class CswNbtTreeNodeProp
    {
        public CswNbtTreeNodeProp( CswNbtMetaDataFieldType.NbtFieldType NbtFieldType, string NbtPropName,
                                   int NbtNodeTypePropId, int NbtJctNodePropId, CswNbtTreeNode TreeNode )
        {
            FieldType = NbtFieldType;
            PropName = NbtPropName;
            NodeTypePropId = NbtNodeTypePropId;
            JctNodePropId = NbtJctNodePropId;
            PropOwner = TreeNode;
        }

        public CswNbtTreeNode PropOwner = null;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Field1", Order = 1 )]
        public string Field1 = string.Empty;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Field1Fk", Order = 2 )]
        public int Field1_Fk = int.MinValue;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Field1Numeric", Order = 3 )]
        public double Field1_Numeric = double.PositiveInfinity;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Field2", Order = 4 )]
        public string Field2 = string.Empty;

        private CswNbtMetaDataFieldType.NbtFieldType _FieldType = CswNbtResources.UnknownEnum;
        public CswNbtMetaDataFieldType.NbtFieldType NbtFieldType
        {
            get { return _FieldType; }
        }

        [DataMember( EmitDefaultValue = true, IsRequired = true, Name = "FieldType", Order = 5 )]
        public string FieldType
        {
            get { return _FieldType; }
            set { _FieldType = value; }
        }

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Gestalt", Order = 6 )]
        public string Gestalt = string.Empty;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Hidden", Order = 7 )]
        public bool Hidden = false;

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "JctNodePropId", Order = 8 )]
        public int JctNodePropId = int.MinValue;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Name", Order = 9 )]
        public string ElementName = string.Empty;

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "NodeTypePropId", Order = 10 )]
        public int NodeTypePropId = int.MinValue;

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "PropName", Order = 11 )]
        public string PropName = string.Empty;

        #region Helpers

        public string getPropColumnValue( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            string ret = Gestalt;
            CswNbtSubField.PropColumn Column = NodeTypeProp.getFieldTypeRule().SubFields[CswNbtSubField.SubFieldName.Name].Column;
            if( Column == CswNbtSubField.PropColumn.Field1 )
            {
                ret = Field1;
            }
            else if( Column == CswNbtSubField.PropColumn.Field2 )
            {
                ret = Field2;
            }
            else if( Column == CswNbtSubField.PropColumn.Field1_FK )
            {
                ret = Field1_Fk.ToString();
            }
            else if( Column == CswNbtSubField.PropColumn.Field1_Numeric )
            {
                ret = Field1_Numeric.ToString();
            }
            return ret;
        }

        #endregion

    }

    [DataContract]
    public class CswNbtTreeNode
    {
        public CswNbtTreeNode( string NbtNodeId, string NbtNodeName, int NbtNodeTypeId, int NbtObjectClassId, CswNbtTreeNode ParentNode = null )
        {
            NodePk = NbtNodeId;
            NodeName = NbtNodeName;
            NodeTypeId = NbtNodeTypeId;
            ObjectClassId = NbtObjectClassId;
            this.ParentNode = ParentNode;
        }

        public CswNbtTreeNode( CswPrimaryKey NbtNodeId, string NbtNodeName, int NbtNodeTypeId, int NbtObjectClassId, CswNbtTreeNode ParentNode = null )
        {
            if( null != NbtNodeId )
            {
                NodePk = NbtNodeId.ToString();
            }
            NodeName = NbtNodeName;
            NodeTypeId = NbtNodeTypeId;
            ObjectClassId = NbtObjectClassId;
            this.ParentNode = ParentNode;
        }

        public CswNbtTreeNode ParentNode = null;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Children", Order = 1 )]
        public Collection<CswNbtTreeNode> ChildNodes = new Collection<CswNbtTreeNode>();

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Properties", Order = 2 )]
        public Collection<CswNbtTreeNodeProp> ChildProps = new Collection<CswNbtTreeNodeProp>();

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "IconFileName", Order = 3 )]
        public string IconFileName = string.Empty;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Included", Order = 4 )]
        public bool Included = true;

        private CswNbtNodeKey _NodeKey = null;
        public CswNbtNodeKey NodeKey
        {
            get { return _NodeKey; }
            set { _NodeKey = value; }
        }
        
        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "Key", Order = 5 )]
        public string Key
        {
            get
            {
                string Ret = string.Empty;
                if( null != _NodeKey )
                {
                    Ret = _NodeKey.ToString();
                }
                return Ret;
            }
        }

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Locked", Order = 6 )]
        public bool Locked = false;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Name", Order = 7 )]
        public string ElementName = string.Empty;

        private CswPrimaryKey _NodeId = new CswPrimaryKey();
        public CswPrimaryKey CswNodeId { get { return _NodeId; } }

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "NodePk", Order = 8 )]
        public string NodePk
        {
            get
            {
                string ret = string.Empty;
                if( null != _NodeId )
                {
                    ret = _NodeId.ToString();
                }
                return ret;
            }
            set { _NodeId = CswConvert.ToPrimaryKey( value ); }
        }

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "NodeId", Order = 9 )]
        public int NodeId
        {
            get
            {
                int Ret = 0;
                if( CswTools.IsPrimaryKey( _NodeId ) )
                {
                    if( _NodeId.PrimaryKey > 0 )
                    {
                        Ret = _NodeId.PrimaryKey;
                    }
                }
                return Ret;
            }
        }

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "NodeName", Order = 10 )]
        public string NodeName = string.Empty;

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "ObjectClassId", Order = 11 )]
        public int ObjectClassId = int.MinValue;

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "NodeTypeId", Order = 12 )]
        public int NodeTypeId = int.MinValue;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Selectable", Order = 13 )]
        public bool Selectable = true;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "ShowInTree", Order = 14 )]
        public bool ShowInTree = true;
        
        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "TreeName", Order = 12 )]
        public string TreeName;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "ExpandMode", Order = 13 )]
        public string ExpandMode;
        
        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Truncated", Order = 14 )]
        public bool Truncated;
        
    }

}//namespace ChemSW.Nbt

