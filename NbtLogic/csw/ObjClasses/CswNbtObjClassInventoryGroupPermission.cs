using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroupPermission : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string InventoryGroup = "Inventory Group";
            public const string WorkUnit = "WorkUnit";
            public const string Role = "Role";
            public const string View = "View";
            public const string Edit = "Edit";
            public const string Dispense = "Dispense";
            public const string Dispose = "Dispose";
            public const string Undispose = "Undispose";
            public const string Request = "Request";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInventoryGroupPermission( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInventoryGroupPermission
        /// </summary>
        public static implicit operator CswNbtObjClassInventoryGroupPermission( CswNbtNode Node )
        {
            CswNbtObjClassInventoryGroupPermission ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InventoryGroupPermissionClass ) )
            {
                ret = (CswNbtObjClassInventoryGroupPermission) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            //case 27692 - uniqueness rule based on InventoryGroup + Role + WorkUnit
            if( false == IsTemp )
            {
                CswNbtView matchingPermissionsView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClassProp WorkUnitOCP = this.ObjectClass.getObjectClassProp( WorkUnit.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp RoleOCP = this.ObjectClass.getObjectClassProp( Role.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp InvGroupOCP = this.ObjectClass.getObjectClassProp( InventoryGroup.ObjectClassPropId );

                CswNbtViewRelationship parent = matchingPermissionsView.AddViewRelationship( this.ObjectClass, false ); //add the InventoryGroupPermission OC to the root of the view
                matchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: WorkUnitOCP,
                    Value: WorkUnit.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                matchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: RoleOCP,
                    Value: Role.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                matchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: InvGroupOCP,
                    Value: InventoryGroup.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                parent.NodeIdsToFilterOut.Add( this.NodeId );

                ICswNbtTree matchingPermissionsTree = _CswNbtResources.Trees.getTreeFromView( matchingPermissionsView, false, false, false );
                matchingPermissionsTree.goToRoot();
                if( matchingPermissionsTree.getChildNodeCount() > 0 )
                {
                    matchingPermissionsTree.goToNthChild( 0 );
                    CswPrimaryKey duplicateNodeId = matchingPermissionsTree.getNodeIdForCurrentPosition();
                    throw new CswDniException(
                        CswEnumErrorType.Warning,
                        "An InventoryGroupPermission with this Role, WorkUnit and InventoryGroup already exists",
                        "A node of nodeid " + duplicateNodeId.ToString() + " already exists with Role: \"" + Role.CachedNodeName + "\", WorkUnit: \"" + WorkUnit.CachedNodeName + "\", and InventoryGroup: \"" + InventoryGroup.CachedNodeName + "\" already exists." );
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

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Public static helper methods

        /// <summary>
        /// Get all Inventory Group Ids to which the current User has Edit permission, according to the User's WorkUnit and Role
        /// </summary>
        public static Collection<CswPrimaryKey> getInventoryGroupIdsForCurrentUser( CswNbtResources Resources )
        {
            Collection<CswPrimaryKey> Ret = new Collection<CswPrimaryKey>();
            if( null != Resources && 
                null != Resources.CurrentNbtUser && 
                CswTools.IsPrimaryKey( Resources.CurrentNbtUser.RoleId ) && 
                CswTools.IsPrimaryKey( Resources.CurrentNbtUser.WorkUnitId ) )
            {
                CswNbtView View = new CswNbtView( Resources );

                CswNbtMetaDataObjectClass InventoryGroupPermOc = Resources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );
                CswNbtViewRelationship IgpVr = View.AddViewRelationship( InventoryGroupPermOc, IncludeDefaultFilters: false );

                CswNbtMetaDataObjectClassProp RoleOcp = InventoryGroupPermOc.getObjectClassProp( PropertyName.Role );
                View.AddViewPropertyAndFilter( IgpVr, RoleOcp, Value: Resources.CurrentNbtUser.RoleId.PrimaryKey.ToString(), SubFieldName: CswEnumNbtSubFieldName.NodeID );

                CswNbtMetaDataObjectClassProp WorkUnitOcp = InventoryGroupPermOc.getObjectClassProp( PropertyName.WorkUnit );
                View.AddViewPropertyAndFilter( IgpVr, WorkUnitOcp, Value: Resources.CurrentNbtUser.WorkUnitId.PrimaryKey.ToString(), SubFieldName: CswEnumNbtSubFieldName.NodeID );

                CswNbtMetaDataObjectClassProp EditOcp = InventoryGroupPermOc.getObjectClassProp( PropertyName.Edit );
                View.AddViewPropertyAndFilter( IgpVr, EditOcp, Value: CswEnumTristate.True.ToString() );

                ICswNbtTree Tree = Resources.Trees.getTreeFromView( View, RequireViewPermissions: false, IncludeHiddenNodes: false, IncludeSystemNodes: false );
                Int32 Results = Tree.getChildNodeCount();
                if( Results > 0 )
                {
                    for( int R = 0; R < Results; R++ )
                    {
                        Tree.goToNthChild(R);

                        CswNbtObjClassInventoryGroupPermission Perm = Tree.getCurrentNode();
                        if( CswTools.IsPrimaryKey( Perm.InventoryGroup.RelatedNodeId ) )
                        {
                            Ret.Add( Perm.InventoryGroup.RelatedNodeId );
                        }

                        Tree.goToParentNode();
                    }
                }

            }
            return Ret;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup]; } }
        public CswNbtNodePropRelationship WorkUnit { get { return _CswNbtNode.Properties[PropertyName.WorkUnit]; } }
        public CswNbtNodePropRelationship Role { get { return _CswNbtNode.Properties[PropertyName.Role]; } }
        public CswNbtNodePropLogical View { get { return _CswNbtNode.Properties[PropertyName.View]; } }
        public CswNbtNodePropLogical Edit { get { return _CswNbtNode.Properties[PropertyName.Edit]; } }
        public CswNbtNodePropLogical Dispense { get { return _CswNbtNode.Properties[PropertyName.Dispense]; } }
        public CswNbtNodePropLogical Dispose { get { return _CswNbtNode.Properties[PropertyName.Dispose]; } }
        public CswNbtNodePropLogical Undispose { get { return _CswNbtNode.Properties[PropertyName.Undispose]; } }
        public CswNbtNodePropLogical Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }

        #endregion

    }//CswNbtObjClassInventoryGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
