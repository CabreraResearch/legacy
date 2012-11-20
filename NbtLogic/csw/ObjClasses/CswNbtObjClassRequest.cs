using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequest : CswNbtObjClass
    {

        public sealed class PropertyName
        {
            public const string Requestor = "Requestor";
            public const string Name = "Name";
            public const string SubmittedDate = "Submitted Date";
            public const string CompletedDate = "Completed Date";
            public const string IsFavorite = "Completed Date";
        }

        public static implicit operator CswNbtObjClassRequest( CswNbtNode Node )
        {
            CswNbtObjClassRequest ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RequestClass ) )
            {
                ret = (CswNbtObjClassRequest) Node.ObjClass;
            }
            return ret;
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRequest( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass ); }
        }

        private void _setDefaultValues()
        {
            if( false == CswTools.IsPrimaryKey( Requestor.RelatedNodeId ) )
            {
                Requestor.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
            }
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _setDefaultValues();
            if( SubmittedDate.WasModified && DateTime.MinValue != SubmittedDate.DateTimeValue )
            {
                ICswNbtTree Tree = _getRelatedRequestItemsTree( FilterByPending: true );
                Int32 RequestItemNodeCount = Tree.getChildNodeCount();
                if( RequestItemNodeCount > 0 )
                {
                    for( Int32 N = 0; N < RequestItemNodeCount; N += 1 )
                    {
                        Tree.goToNthChild( N );
                        CswNbtPropertySetRequestItem NodeAsPropSet = _CswNbtResources.Nodes.GetNode( Tree.getNodeIdForCurrentPosition() );
                        NodeAsPropSet.Status.Value = CswNbtPropertySetRequestItem.Statuses.Submitted;
                        NodeAsPropSet.postChanges( true );
                        Tree.goToParentNode();
                    }
                }
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataObjectClassProp RequestorOcp = ObjectClass.getObjectClassProp( PropertyName.Requestor );
            CswNbtMetaDataObjectClassProp IsFavoriteOcp = ObjectClass.getObjectClassProp( PropertyName.IsFavorite );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, RequestorOcp, "me" );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, IsFavoriteOcp, Tristate.False.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Custom Logic

        public void setCompletedDate()
        {
            ICswNbtTree RequestItemTree = _getRelatedRequestItemsTree();
            if( _allRequestItemsCompleted( RequestItemTree ) )
            {
                this.CompletedDate.DateTimeValue = DateTime.Today;
                this.postChanges( false );
            }
        }

        private ICswNbtTree _getRelatedRequestItemsTree( bool FilterByPending = false )
        {
            CswNbtView RequestItemView = new CswNbtView( _CswNbtResources );

            foreach( NbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                CswNbtViewRelationship RiRelationship = RequestItemView.AddViewRelationship( MemberOc, false );
                if( FilterByPending )
                {
                    RequestItemView.AddViewPropertyAndFilter( RiRelationship,
                                                              MemberOc.getObjectClassProp(
                                                                  CswNbtPropertySetRequestItem.PropertyName.Status ),
                                                              CswNbtPropertySetRequestItem.Statuses.Pending,
                                                              FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                }
                RequestItemView.AddViewPropertyAndFilter( RiRelationship, RequestOcp, SubFieldName: CswNbtSubField.SubFieldName.NodeID, Value: NodeId.PrimaryKey.ToString() );
            }


            ICswNbtTree RequestItemTree = _CswNbtResources.Trees.getTreeFromView( RequestItemView, IncludeSystemNodes: false, RequireViewPermissions: false, IncludeHiddenNodes: false );
            return RequestItemTree;
        }

        private bool _allRequestItemsCompleted( ICswNbtTree RequestItemTree )
        {
            bool allRequestItemsCompleted = true;
            if( RequestItemTree.getChildNodeCount() > 0 )
            {
                for( Int32 N = 0; N < RequestItemTree.getChildNodeCount(); N += 1 )
                {
                    RequestItemTree.goToNthChild( N );
                    CswNbtPropertySetRequestItem NodeAsPropSet = RequestItemTree.getNodeForCurrentPosition();
                    if( null != NodeAsPropSet )
                    {
                        if( CswNbtPropertySetRequestItem.Statuses.Completed != NodeAsPropSet.Status.Value )
                        {
                            allRequestItemsCompleted = false;
                        }
                    }
                    RequestItemTree.goToParentNode();
                }
            }
            return allRequestItemsCompleted;
        }

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropRelationship Requestor
        {
            get { return _CswNbtNode.Properties[PropertyName.Requestor]; }
        }

        public CswNbtNodePropText Name
        {
            get { return _CswNbtNode.Properties[PropertyName.Name]; }
        }

        public CswNbtNodePropDateTime SubmittedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.SubmittedDate]; }
        }

        public CswNbtNodePropDateTime CompletedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.CompletedDate]; }
        }

        public CswNbtNodePropDateTime IsFavorite
        {
            get { return _CswNbtNode.Properties[PropertyName.IsFavorite]; }
        }

        #endregion
    }//CswNbtObjClassRequest

}//namespace ChemSW.Nbt.ObjClasses
