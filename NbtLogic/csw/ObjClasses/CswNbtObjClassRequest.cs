using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequest : CswNbtObjClass
    {

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Requestor = "Requestor";
            public const string Name = "Name";
            public const string SubmittedDate = "Submitted Date";
            public const string CompletedDate = "Completed Date";
            public const string IsFavorite = "Is Favorite";
            public const string IsRecurring = "Is Recurring";
        }

        public static implicit operator CswNbtObjClassRequest( CswNbtNode Node )
        {
            CswNbtObjClassRequest ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RequestClass ) )
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
        }

        //ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass ); }
        }

        private void _setDefaultValues()
        {
            if( false == CswTools.IsPrimaryKey( Requestor.RelatedNodeId ) )
            {
                Requestor.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
            }
            if( string.IsNullOrEmpty( Name.Text ) )
            {
                Name.Text = _CswNbtResources.CurrentNbtUser.Username + " " + DateTime.Now;
            }
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _setDefaultValues();
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }

        //beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }

        //afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        } //afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            IsFavorite.SetOnPropChange( onIsFavortiteChange );
            IsRecurring.SetOnPropChange( onIsRecurringChange );
            Name.SetOnPropChange( onNamePropChange );
            SubmittedDate.SetOnPropChange( onSubmittedDatePropChange );
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        } //afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataObjectClassProp RequestorOcp = ObjectClass.getObjectClassProp( PropertyName.Requestor );
            CswNbtMetaDataObjectClassProp IsFavoriteOcp = ObjectClass.getObjectClassProp( PropertyName.IsFavorite );
            CswNbtMetaDataObjectClassProp IsRecurringOcp = ObjectClass.getObjectClassProp( PropertyName.IsRecurring );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, RequestorOcp, Value: "me", ShowInGrid: false );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, IsFavoriteOcp, FilterMode: CswEnumNbtPropertyFilterMode.NotEquals, Value: CswEnumTristate.True.ToString(), ShowInGrid: false );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, IsRecurringOcp, FilterMode: CswEnumNbtPropertyFilterMode.NotEquals, Value: CswEnumTristate.True.ToString(), ShowInGrid: false );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                /*Do Something*/
            }
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

        private ICswNbtTree _getRelatedRequestItemsTree( string FilterByStatus = null )
        {
            CswNbtView RequestItemView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass RequestOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass );
            CswNbtViewRelationship RootVr = RequestItemView.AddViewRelationship( RequestOc, IncludeDefaultFilters: false );
            RootVr.NodeIdsToFilterIn.Add( this.NodeId );

            foreach( CswEnumNbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                CswNbtViewRelationship RiRelationship = RequestItemView.AddViewRelationship( RootVr, NbtViewPropOwnerType.Second, RequestOcp, IncludeDefaultFilters: false );
                if( false == string.IsNullOrEmpty( FilterByStatus ) )
                {
                    RequestItemView.AddViewPropertyAndFilter( RiRelationship,
                                                              MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ),
                                                              FilterByStatus.ToString(),
                                                              FilterMode: CswEnumNbtFilterMode.Equals );
                }
            }


            ICswNbtTree RequestItemTree = _CswNbtResources.Trees.getTreeFromView( RequestItemView, IncludeSystemNodes: false, RequireViewPermissions: false, IncludeHiddenNodes: false );
            return RequestItemTree;
        }

        private bool _allRequestItemsCompleted( ICswNbtTree RequestItemTree )
        {
            bool allRequestItemsCompleted = true;
            int RequestCount = RequestItemTree.getChildNodeCount();
            if( RequestCount == 1 )
            {
                RequestItemTree.goToNthChild( 0 );

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
        private void onNamePropChange( CswNbtNodeProp NodeProp )
        {
            if( Name.WasModified &&
                string.IsNullOrEmpty( Name.Text ) && 
                false == string.IsNullOrEmpty(Name.GetOriginalPropRowValue()))
            {
                Name.Text = Name.GetOriginalPropRowValue();
            }
        }

        public CswNbtNodePropDateTime SubmittedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.SubmittedDate]; }
        }
        private void onSubmittedDatePropChange( CswNbtNodeProp NodeProp )
        {
            if( SubmittedDate.DateTimeValue != DateTime.MinValue )
            {
                ICswNbtTree Tree = _getRelatedRequestItemsTree( FilterByStatus: CswNbtPropertySetRequestItem.Statuses.Pending );
                int RequestCount = Tree.getChildNodeCount();
                if( RequestCount == 1 )
                {
                    Tree.goToNthChild( 0 );

                    Int32 RequestItemNodeCount = Tree.getChildNodeCount();
                    if( RequestItemNodeCount > 0 )
                    {
                        for( Int32 N = 0; N < RequestItemNodeCount; N += 1 )
                        {
                            Tree.goToNthChild( N );
                            CswNbtPropertySetRequestItem NodeAsPropSet = _CswNbtResources.Nodes[Tree.getNodeIdForCurrentPosition()];
                            NodeAsPropSet.Status.Value = CswNbtPropertySetRequestItem.Statuses.Submitted;
                            NodeAsPropSet.Request.RefreshNodeName();
                            NodeAsPropSet.Name.RecalculateReferenceValue();
                            NodeAsPropSet.postChanges( ForceUpdate: false );
                            Tree.goToParentNode();
                        }
                    }
                }
            }
        }

        public CswNbtNodePropDateTime CompletedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.CompletedDate]; }
        }

        public CswNbtNodePropLogical IsFavorite
        {
            get { return _CswNbtNode.Properties[PropertyName.IsFavorite]; }
        }

        public CswNbtNodePropLogical IsRecurring
        {
            get { return _CswNbtNode.Properties[PropertyName.IsRecurring]; }
        }
        private void onIsRecurringChange( CswNbtNodeProp NodeProp )
        {
            if( IsRecurring.WasModified )
            {
                _toggleProps();
            }
        }

        private bool _IsFakeNode
        {
            get { return IsFavorite.Checked == CswEnumTristate.True || IsRecurring.Checked == CswEnumTristate.True; }
        }

        private void _toggleProps()
        {
            if( _IsFakeNode )
            {
                SubmittedDate.setHidden( value: true, SaveToDb: true );
                CompletedDate.setHidden( value: true, SaveToDb: true );
            }
            else
            {
                SubmittedDate.setHidden( value: false, SaveToDb: true );
                CompletedDate.setHidden( value: false, SaveToDb: true );
            }
        }
        private void onIsFavortiteChange( CswNbtNodeProp NodeProp )
        {
            if( IsFavorite.WasModified )
            {
                _toggleProps();
            }
        }

        #endregion
    }//CswNbtObjClassRequest

}//namespace ChemSW.Nbt.ObjClasses
