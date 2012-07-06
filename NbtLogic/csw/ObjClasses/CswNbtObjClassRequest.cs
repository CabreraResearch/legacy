using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequest : CswNbtObjClass
    {

        public sealed class PropertyName : CswEnum<PropertyName>
        {
            private PropertyName( String Name ) : base( Name ) { }
            public static IEnumerable<PropertyName> all { get { return All; } }
            public static implicit operator PropertyName( string Str )
            {
                PropertyName Ret = Parse( Str );
                return Ret ?? Unknown;
            }
            public static readonly PropertyName Requestor = new PropertyName( "Requestor" );
            public static readonly PropertyName Name = new PropertyName( "Name" );
            public static readonly PropertyName SubmittedDate = new PropertyName( "Submitted Date" );
            public static readonly PropertyName CompletedDate = new PropertyName( "Completed Date" );
            public static readonly PropertyName InventoryGroup = new PropertyName( "Inventory Group" );
            public static readonly PropertyName Unknown = new PropertyName( "Unknown" );
        }

        public static implicit operator CswNbtObjClassRequest( CswNbtNode Node )
        {
            CswNbtObjClassRequest ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass ) )
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
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
            Requestor.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );

            if( SubmittedDate.WasModified && DateTime.MinValue != SubmittedDate.DateTimeValue )
            {
                CswNbtView RequestItemsView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClass RequestItemsOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
                CswNbtMetaDataObjectClassProp RiRequestOcp = RequestItemsOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );

                CswNbtViewRelationship RequestItemVr = RequestItemsView.AddViewRelationship( RequestItemsOc, false );
                RequestItemsView.AddViewPropertyAndFilter( RequestItemVr,
                                                          RequestItemsOc.getObjectClassProp(
                                                              CswNbtObjClassRequestItem.PropertyName.Status ),
                                                          CswNbtObjClassRequestItem.Statuses.Pending,
                                                          FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                RequestItemsView.AddViewPropertyAndFilter( RequestItemVr, RiRequestOcp, NodeId.PrimaryKey.ToString(), SubFieldName: CswNbtSubField.SubFieldName.NodeID );

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestItemsView, false, false );
                Int32 RequestItemNodeCount = Tree.getChildNodeCount();
                if( RequestItemNodeCount > 0 )
                {
                    for( Int32 N = 0; N < RequestItemNodeCount; N += 1 )
                    {
                        Tree.goToNthChild( N );
                        CswNbtObjClassRequestItem NodeAsRequestItem = _CswNbtResources.Nodes.GetNode( Tree.getNodeIdForCurrentPosition() );
                        NodeAsRequestItem.Status.Value = CswNbtObjClassRequestItem.Statuses.Submitted;
                        NodeAsRequestItem.postChanges( true );
                        Tree.goToParentNode();
                    }
                }
            }
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
            CswNbtMetaDataObjectClassProp RequestorOcp = ObjectClass.getObjectClassProp( PropertyName.Requestor.ToString() );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, RequestorOcp, "me" );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out JObject ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = new JObject();
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Requestor
        {
            get { return _CswNbtNode.Properties[PropertyName.Requestor.ToString()]; }
        }

        public CswNbtNodePropRelationship InventoryGroup
        {
            get { return _CswNbtNode.Properties[PropertyName.InventoryGroup.ToString()]; }
        }

        public CswNbtNodePropText Name
        {
            get { return _CswNbtNode.Properties[PropertyName.Name.ToString()]; }
        }

        public CswNbtNodePropDateTime SubmittedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.SubmittedDate.ToString()]; }
        }

        public CswNbtNodePropDateTime CompletedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.CompletedDate.ToString()]; }
        }

        #endregion
    }//CswNbtObjClassRequest

}//namespace ChemSW.Nbt.ObjClasses
