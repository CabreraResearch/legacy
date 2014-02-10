
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{

    [DataContract]
    public class CswNbtTreeNodeProp
    {
        public CswNbtTreeNodeProp( CswEnumNbtFieldType NbtFieldType, string NbtPropName, string NbtObjectClassPropName, int NbtObjectClassPropId,
                                   int NbtNodeTypePropId, int NbtJctNodePropId, CswNbtTreeNode TreeNode )
        {
            FieldType = NbtFieldType;
            PropName = NbtPropName;
            ObjectClassPropName = NbtObjectClassPropName;
            ObjectClassPropId = NbtObjectClassPropId;
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

        private CswEnumNbtFieldType _FieldType = CswNbtResources.UnknownEnum;
        public CswEnumNbtFieldType NbtFieldType
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

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "ObjectClassPropName", Order = 12 )]
        public string ObjectClassPropName = string.Empty;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "ObjectClassPropId", Order = 13 )]
        public int ObjectClassPropId = int.MinValue;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Field1Big", Order = 14 )]
        public string Field1_Big = string.Empty;

        public string this[CswEnumNbtPropColumn column]
        {
            get
            {
                string ret = string.Empty;
                if( CswEnumNbtPropColumn.Field1 == column )
                {
                    ret = this.Field1;
                }
                else if( CswEnumNbtPropColumn.Field1_FK == column )
                {
                    ret = this.Field1_Fk.ToString();
                }
                else if( CswEnumNbtPropColumn.Field1_Numeric == column )
                {
                    ret = this.Field1_Numeric.ToString();
                }
                else if( CswEnumNbtPropColumn.Field2 == column )
                {
                    ret = this.Field2;
                }
                else if( CswEnumNbtPropColumn.Gestalt == column )
                {
                    ret = this.Gestalt;
                }
                else if( CswEnumNbtPropColumn.Field1_Big == column )
                {
                    ret = this.Field1_Big;
                }
                return ret;
            }
        } // this[]
    } // class CswNbtTreeNodeProp

    [DataContract]
    public class CswNbtTreeNode
    {
        public CswNbtTreeNode( CswPrimaryKey NbtNodeId, string NbtNodeName, int NbtNodeTypeId, int NbtObjectClassId, CswPrimaryKey NbtRelationalId, CswNbtTreeNode ParentNode = null )
        {
            if( null != NbtNodeId )
            {
                NodePk = NbtNodeId.ToString();
            }
            NodeName = NbtNodeName;
            NodeTypeId = NbtNodeTypeId;
            ObjectClassId = NbtObjectClassId;
            this.ParentNode = ParentNode;
            RelationalId = NbtRelationalId;
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

        public CswPrimaryKey CswNodeId = new CswPrimaryKey();
        public CswPrimaryKey RelationalId = null;

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "NodePk", Order = 8 )]
        public string NodePk
        {
            get
            {
                string ret = string.Empty;
                if( null != CswNodeId )
                {
                    ret = CswNodeId.ToString();
                }
                return ret;
            }
            set { CswNodeId = CswConvert.ToPrimaryKey( value ); }
        }

        [DataMember( EmitDefaultValue = false, IsRequired = true, Name = "NodeId", Order = 9 )]
        public int NodeId
        {
            get
            {
                int Ret = 0;
                if( CswTools.IsPrimaryKey( CswNodeId ) )
                {
                    if( CswNodeId.PrimaryKey > 0 )
                    {
                        Ret = CswNodeId.PrimaryKey;
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

        [DataMember( EmitDefaultValue = false, IsRequired = false, Name = "Favorited", Order = 15 )]
        public bool Favorited;
    }

}//namespace ChemSW.Nbt

