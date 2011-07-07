using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using System.Text.RegularExpressions;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Type of Node 
    /// </summary>
    public enum NodeSpecies
    {
        /// <summary>
        /// Unknown
        /// </summary>
        UnKnown,
        /// <summary>
        /// Regular, run-of-the-mill Node
        /// </summary>
        Plain,
        /// <summary>
        /// Audit Node
        /// </summary>
        Audit,
        /// <summary>
        /// Group
        /// </summary>
        Group,
        /// <summary>
        /// Root Node
        /// </summary>
        Root,
        /// <summary>
        /// More Node
        /// </summary>
        More
    };

    //bz # 5943
    public enum NodeModificationState
    {
        Unknown,
        /// <summary>
        /// Unknown
        /// </summary>
        Empty,
        /// <summary>
        /// The node contains no data
        /// </summary>
        Unchanged,
        /// <summary>
        /// The node and its properties have been read from the database
        /// </summary>
        Modified,
        //Set,
        /// <summary>
        /// The value one of the node's selectors or of its properties has been modified
        /// </summary>
        Posted,
        /// <summary>
        /// The Node's data has been written to the database, but not yet committed
        /// </summary>
        //Committed,
        ///// <summary>
        ///// The data written in the Posted phase has been committed
        ///// </summary>
        ///// 
        Deleted
        /// <summary>
        /// The node has been removed from the database
        /// </summary>

    };


    //public enum NodeState { Insert, Update, Delete, Unchanged };
    public class CswNbtNode //: System.IEquatable<CswNbtNode>
    {
        public delegate void OnSetNodeIdHandler( CswNbtNode Node, CswPrimaryKey OldNodeId, CswPrimaryKey NewNodeId );
        public delegate void OnRequestWriteNodeHandler( CswNbtNode Node, bool ForceUpdate, bool IsCopy );
        public delegate void OnRequestDeleteNodeHandler( CswNbtNode Node );
        public delegate void OnRequestFillHandler( CswNbtNode Node, DateTime Date );
        public delegate void OnRequestFillFromNodeTypeIdHandler( CswNbtNode Node, Int32 NodeTypeId );
        public event OnSetNodeIdHandler OnAfterSetNodeId = null;
        public event OnRequestWriteNodeHandler OnRequestWriteNode = null;
        public event OnRequestDeleteNodeHandler OnRequestDeleteNode = null;
        public event OnRequestFillHandler OnRequestFill = null;
        public event OnRequestFillFromNodeTypeIdHandler OnRequestFillFromNodeTypeId = null;

        private void OnAfterSetNodeIdHandler( CswPrimaryKey OldNodeId, CswPrimaryKey NewNodeId )
        {
            if( OnAfterSetNodeId != null )
                OnAfterSetNodeId( this, OldNodeId, NewNodeId );
        }

        private CswNbtNodePropColl _CswNbtNodePropColl = null;
        //private ICswNbtObjClassFactory _CswNbtObjClassFactory = null;
        private CswNbtObjClass _CswNbtObjClass = null;
        private CswNbtResources _CswNbtResources;
        public CswNbtNode( CswNbtResources CswNbtResources, Int32 NodeTypeId, NodeSpecies NodeSpecies, CswPrimaryKey NodeId, Int32 UniqueId ) //, ICswNbtObjClassFactory ICswNbtObjClassFactory )
        {
            _CswNbtResources = CswNbtResources;
            _UniqueId = UniqueId;
            _NodeId = NodeId;
            _NodeTypeId = NodeTypeId;
            _CswNbtNodePropColl = new CswNbtNodePropColl( CswNbtResources, this ); //, ICswNbtObjClassFactory);
            //_CswNbtObjClassFactory = ICswNbtObjClassFactory; // new CswNbtObjClassFactory(CswNbtResources, this);
            _NodeSpecies = NodeSpecies;

            if( NodeType != null )
                ObjectClassId = NodeType.ObjectClass.ObjectClassId;

        }//ctor()

        //private NodeState _NodeState = NodeState.Unchanged;
        //public NodeState 

        #region Core Properties

        //bz # 5908
        //We need this because in c# you can't take the address of an object
        private Int32 _UniqueId = Int32.MinValue;
        public Int32 UniqueId
        {
            get
            {
                return ( _UniqueId );
            }//
        }//UniqueId

        //bz # 5943
        private NodeModificationState _NodeModificationState = NodeModificationState.Unknown;
        public NodeModificationState ModificationState
        {
            get
            {

                if( ( NodeModificationState.Unchanged == _NodeModificationState ||
                       NodeModificationState.Posted == _NodeModificationState ) &&
                    _CswNbtNodePropColl.Modified )
                {
                    _NodeModificationState = NodeModificationState.Modified;
                }

                return ( _NodeModificationState );
            }//get

        }//ModificationState

		private bool _ReadOnly = false;
		public bool ReadOnly
		{
			get { return _ReadOnly; }
			set { _ReadOnly = value; }
		}


        //bz # 5943
        //private bool _Modified = false;
        //public bool Modified
        //{
        //    get { return ( _Modified || _CswNbtNodePropColl.Modified ); }
        //    set { _Modified = value; }
        //}//Modified

        //public void clearModifiedFlag()
        //{
        //    _CswNbtNodePropColl.clearModifiedFlag();
        //    _Modified = false;
        //}

        public bool Filled
        {
            get
            {
                return ( _CswNbtNodePropColl.Filled );
            }//get
        }//Filled

        public bool SuspendModifyTracking
        {
            get { return _CswNbtNodePropColl.SuspendModifyTracking; }
            set { _CswNbtNodePropColl.SuspendModifyTracking = value; }
        }

        public bool DisableSave = false;

        public bool New
        {
            get
            {
                return ( _CswNbtNodePropColl.CreatedFromNodeTypeId );
            }
        }//New


        private NodeSpecies _NodeSpecies = NodeSpecies.UnKnown;
        public NodeSpecies NodeSpecies { get { return ( _NodeSpecies ); } }


        private CswPrimaryKey _NodeId = null;
        public CswPrimaryKey NodeId
        {
            get
            {
                return ( _NodeId );
            }//get

            set
            {
                CswPrimaryKey OldNodeId = _NodeId;
                _NodeId = value;

                // fix properties
                Properties._NodePk = _NodeId;

                OnAfterSetNodeIdHandler( OldNodeId, _NodeId );
            }//set

        }//NodeId

        //private CswNbtNodeKey _NodeKey = null;
        //public CswNbtNodeKey NodeKey
        //{
        //    get { return ( _NodeKey ); }

        //    set
        //    {
        //        _NodeKey = value;
        //        if( null != onAfterSetNodeKey )
        //        {
        //            onAfterSetNodeKey( this );
        //        }//
        //    }//set

        //}//NodeKey

        //private Int32 _ParentNodeId = 0;
        //public Int32 ParentNodeId { get { return ( _ParentNodeId ); } set { _ParentNodeId = value; } }

        //private CswNbtNode _ParentNode = null;
        //public CswNbtNode ParentNode 
        //{ 
        //    get { return ( _ParentNode ); } 

        //    set { 

        //        _ParentNode = value;
        //        if( null != _ParentNode.NodeKey && null != NodeKey )
        //        {
        //            NodeKey.ParentNodeKey = _ParentNode.NodeKey;
        //        }
        //    } 
        //}

        private Int32 _NodeTypeId = 0;
        public Int32 NodeTypeId
        {
            get
            {
                return ( _NodeTypeId );
            }
            set
            {
                _NodeTypeId = value;
            }
        }


        private CswNbtMetaDataNodeType _NodeType = null;
        public CswNbtMetaDataNodeType NodeType
        {
            get { return _CswNbtResources.MetaData.getNodeType( NodeTypeId ); }
        }//NodeType

        //private string _NodeTypeName = "";
        //public string NodeTypeName { get { return ( _NodeTypeName ); } set { _NodeTypeName = value; } }

        private Int32 _ObjectClassId = 0;
        public Int32 ObjectClassId
        {
            get { return ( _ObjectClassId ); }
            set
            {
                _ObjectClassId = value;
                if( _CswNbtObjClass == null || _CswNbtObjClass.ObjectClass.ObjectClassId != _ObjectClassId )
                    _CswNbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, _ObjectClassId, this );
            }
        }//ObjectClassId

        public CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( ObjectClassId ); }
        }//ObjectClass

        // For CswNbtNodeCaster
        public CswNbtObjClass ObjClass
        {
            get { return _CswNbtObjClass; }
        }

        private void _clear()
        {
            _NodeName = string.Empty;
            _PendingUpdate = false;
        }//clear()

        private string _NodeName = string.Empty;
        public string NodeName
        {
            get { return ( _NodeName ); }
            set
            {
                // case 20781 - only mark modified if we're changing the name, not assigning it from DB
                if( _NodeName != value && _NodeName != string.Empty)
                {
                    //bz # 5943
                    //_Modified = true;
                    _NodeModificationState = NodeModificationState.Modified;
                }
                _NodeName = value;
            }
        }
        private bool _PendingUpdate = false;
        public bool PendingUpdate
        {
            get { return ( _PendingUpdate ); }
            set
            {
                if( _PendingUpdate != value )
                {
                    _PendingUpdate = value;
                    //bz # 5943
                    //_Modified = true;
                    _NodeModificationState = NodeModificationState.Modified;
                }
            }
        }

        private ArrayList _TemplatePropsAl = new ArrayList();
        //private string _NameTemplate = "";
        //public string NameTemplate
        //{
        //    get { return ( _NameTemplate ); }

        //    set
        //    {
        //        _NameTemplate = value;
        //    }

        //}//NameTemplate 

        //public IEnumerable TemplateProps
        //{
        //    get
        //    {
        //        if( NodeType.NameTemplate.Length > 0 && 0 == _TemplatePropsAl.Count )
        //        {
        //            RegexOptions RegExOpts = new RegexOptions();
        //            RegExOpts |= RegexOptions.IgnoreCase;
        //            string TemplateRegEx = @"\[(.*?)\]";
        //            Regex RegEx = new Regex( TemplateRegEx, RegExOpts );
        //            Match RegExMatch = RegEx.Match( _NameTemplate );
        //            while( RegExMatch.Success )
        //            {
        //                string TemplateParameter = RegExMatch.Groups[ 1 ].ToString();
        //                _TemplatePropsAl.Add( TemplateParameter );
        //                RegExMatch = RegExMatch.NextMatch();
        //            }//iterate matches
        //        }//

        //        return ( _TemplatePropsAl );
        //    }//

        //}//TemplateProps



        private string _IconFileName = "";
        public string IconFileName { get { return ( _IconFileName ); } set { _IconFileName = value; } }

        private bool _Selectable = true;
        public bool Selectable { get { return ( _Selectable ); } set { _Selectable = value; } }

        //private bool _ShowInGrid = true;
        //public bool ShowInGrid { get { return ( _ShowInGrid ); } set { _ShowInGrid = value; } }
        private bool _ShowInTree = true;
        public bool ShowInTree { get { return ( _ShowInTree ); } set { _ShowInTree = value; } }

        //private CswNbtView.AddChildrenSetting _AddChildren = CswNbtView.AddChildrenSetting.None;
        //public CswNbtView.AddChildrenSetting AddChildren { get { return ( _AddChildren ); } set { _AddChildren = value; } }

        public CswNbtNodePropColl Properties { get { return ( _CswNbtNodePropColl ); } }

        //private CswNbtViewNode _ViewNode;
        //public CswNbtViewNode ViewNode { get { return _ViewNode; } set { _ViewNode = value; } }

        public override int GetHashCode()
        {
            return this.NodeId.PrimaryKey;
        }
        #endregion

        #region Methods


        //bz # 5943
        public void postChanges( bool ForceUpdate )
        {
            postChanges( ForceUpdate, false );
        }

        public void postChanges( bool ForceUpdate, bool IsCopy )
        {
            if( NodeModificationState.Modified == ModificationState || ForceUpdate )
            {
                if( null == OnRequestWriteNode )
                    throw ( new CswDniException( "There is no write handler" ) );

                bool IsNew = ( this.NodeId == null || this.NodeId.PrimaryKey == Int32.MinValue );
                if( null != _CswNbtObjClass )
                {
                    if( IsNew )
                        _CswNbtObjClass.beforeCreateNode();
                    else
                        _CswNbtObjClass.beforeWriteNode();
                }

                OnRequestWriteNode( this, ForceUpdate, IsCopy );

                if( null != _CswNbtObjClass )
                {
                    if( IsNew )
                        _CswNbtObjClass.afterCreateNode();
                    else
                        _CswNbtObjClass.afterWriteNode();
                }

                _NodeModificationState = NodeModificationState.Posted;

                //reset(); //bz # 6713
                // But see BZ 9650 and BZ 8517
            }
        }//postChanges()

        //bz # 5943
        //public void beforeWriteNode()
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClass.beforeWriteNode();
        //    }//
        //}//beforeWriteNode()

        //bz # 5943
        //public void afterWriteNode()
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClass.afterWriteNode();
        //    }//
        //}//afterWriteNode()


        //bz # 5943
        public void delete()
        {
			if( null == OnRequestDeleteNode )
			{
				throw ( new CswDniException( "There is no delete handler" ) );
			}

			if( !_CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Delete, this.NodeType ) )
			{
				throw ( new CswDniException( ErrorType.Warning, "You do not have permission to delete this " + this.NodeType.NodeTypeName, "User attempted to delete a " + this.NodeType.NodeTypeName + " without Delete permissions" ) );
			}

            if( null != _CswNbtObjClass )
            {
                _CswNbtObjClass.beforeDeleteNode();
            }

            OnRequestDeleteNode( this );

            if( null != _CswNbtObjClass )
            {
                _CswNbtObjClass.afterDeleteNode();
            }

            _NodeModificationState = NodeModificationState.Deleted;

        }//delete()

        ////bz # 6713
        // But see BZ 8517 and BZ 9650
        //public void reset()
        //{
        //    if( ModificationState == NodeModificationState.Modified )
        //        throw( new CswDniException( "There are pending changes -- reset not allowed" ) );

        //    _clear();
        //    fill();
        //    Properties.clearModifiedFlag();
        //    _NodeModificationState = NodeModificationState.Unchanged;

        //}//reset()

        //bz # 5943
        //public void beforeDeleteNode()
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClass.beforeDeleteNode();
        //    }//
        //}//beforeDeleteNode()


        //bz # 5943
        //public void afterDeleteNode()
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClass.afterDeleteNode();
        //    }//
        //}//afterDeleteNode()


        //bz # 5943

        public void fill(DateTime Date)
        {
            if( null == OnRequestFill )
                throw ( new CswDniException( "There is no fill handler" ) );

            OnRequestFill( this, Date );

            _NodeModificationState = NodeModificationState.Unchanged;

        }//fill() 


        public void fillFromNodeTypeId( Int32 NodeTypeId )
        {
            if( null == OnRequestFillFromNodeTypeId )
                throw ( new CswDniException( "There is no fill handler" ) );

            OnRequestFillFromNodeTypeId( this, NodeTypeId );

            _NodeModificationState = NodeModificationState.Unchanged;

        }//fillFromNodeTypeId()


        public void cancelChanges()
        {
            Int32 NodeTypeId = _NodeTypeId;

            OnRequestFillFromNodeTypeId( this, NodeTypeId );

            _NodeModificationState = NodeModificationState.Unchanged;

        }//cancelChanges()


        /// <summary>
        /// Copies all matching properties (by name and field type) from another node. 
        /// </summary>
        /// <param name="SourceNode">Node from which to copy property values</param>
        public void copyPropertyValues( CswNbtNode SourceNode )
        {
            foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
            {
                foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
                {
                    if( ThisProp.PropName == SourceProp.PropName && ThisProp.FieldType == SourceProp.FieldType )
                    {
                        ThisProp.copy( SourceProp );
                    } // if( ThisProp.PropName == SourceProp.PropName && ThisProp.FieldType == SourceProp.FieldType )
                } // foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
            } // foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
        } // copyPropertyValues()

        /// <summary>
        /// Sets the values of all relationships whose target matches 
        /// the ParentNode's nodetypeid or objectclassid to the ParentNode's nodeid.
        /// </summary>
        public void RelateToNode( CswNbtNode ParentNode, CswNbtView View )
        {
            CswNbtNodePropWrapper Prop = null;
            // BZ 10372 - Iterate all relationships
            foreach( CswNbtViewRelationship ViewRelationship in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
            {
                // BZ 8355 - Set relationships on children pointing to parents, not the other way
                if( ViewRelationship.PropOwner == CswNbtViewRelationship.PropOwnerType.Second )
                {
                    if( ( ( ViewRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId && ViewRelationship.SecondId == this.NodeTypeId ) ||
                          ( ViewRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId && ViewRelationship.SecondId == this.ObjectClassId ) ) &&
                        ( ( ViewRelationship.FirstType == CswNbtViewRelationship.RelatedIdType.NodeTypeId && ViewRelationship.FirstId == ParentNode.NodeTypeId ) ||
                          ( ViewRelationship.FirstType == CswNbtViewRelationship.RelatedIdType.ObjectClassId && ViewRelationship.FirstId == ParentNode.ObjectClassId ) ) )
                    {
                        if( ViewRelationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                            Prop = this.Properties[_CswNbtResources.MetaData.getNodeTypeProp( ViewRelationship.PropId )];
                        else if( ViewRelationship.PropType == CswNbtViewRelationship.PropIdType.ObjectClassPropId )
                            Prop = this.Properties[_CswNbtResources.MetaData.getObjectClassProp( ViewRelationship.PropId ).PropName];

                        if( Prop != null )
                        {
                            if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                            {
                                Prop.AsRelationship.RelatedNodeId = ParentNode.NodeId;
                                Prop.AsRelationship.RefreshNodeName();
                            }
                            if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Location )
                            {
                                Prop.AsLocation.SelectedNodeId = ParentNode.NodeId;
                                Prop.AsLocation.RefreshNodeName();
                            }
                        }
                    }
                } // if( ViewRelationship.PropOwner == CswNbtViewRelationship.PropOwnerType.Second )
            } // foreach( CswNbtViewRelationship ViewRelationship in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
        } // RelateToNode()

        #endregion Methods


    }//CswNbtNode

}//namespace ChemSW.Nbt.ObjClasses
