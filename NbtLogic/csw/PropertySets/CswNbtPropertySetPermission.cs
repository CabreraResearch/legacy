using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Permission Property Set
    /// </summary>
    public abstract class CswNbtPropertySetPermission: CswNbtObjClass
    {
        #region Enums

        /// <summary>
        /// Object Class property names
        /// </summary>
        public new class PropertyName: CswNbtObjClass.PropertyName
        {
            /// <summary>
            /// The Work Unit to which this permission applies
            /// </summary>
            public const string WorkUnit = "WorkUnit";
            /// <summary>
            /// When checked, these permissions will apply to all Work Units by default.
            /// Permissions defined elsewhere that apply to specific Work Units will take precedence.
            /// </summary>
            public const string ApplyToAllWorkUnits = "Apply to all Work Units";
            /// <summary>
            /// The Role to which this permission applies
            /// </summary>
            public const string Role = "Role";
            /// <summary>
            /// When checked, these permissions will apply to all Roles by default.
            /// Permissions defined elsewhere that apply to specific Roles will take precedence.
            /// </summary>
            public const string ApplyToAllRoles = "Apply to all Roles";
            /// <summary>
            /// Permission to view the Target
            /// </summary>
            public const string View = "View";
            /// <summary>
            /// Permission to edit the Target
            /// </summary>
            public const string Edit = "Edit";
        }

        /// <summary>
        /// Returns the Group ObjectClass that relates to the Target
        /// </summary>
        public abstract CswEnumNbtObjectClass GroupClass { get; }

        /// <summary>
        /// Returns the Target ObjectClass with which permissions apply
        /// </summary>
        public abstract CswEnumNbtObjectClass TargetClass { get; }

        #endregion Enums

        #region Base

        /// <summary>
        /// Default Object Class for consumption by derived classes
        /// </summary>
        public CswNbtObjClassDefault CswNbtObjClassDefault = null;

        /// <summary>
        /// Property Set ctor
        /// </summary>
        protected CswNbtPropertySetPermission( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtPropertySetPermission
        /// </summary>
        public static implicit operator CswNbtPropertySetPermission( CswNbtNode Node )
        {
            CswNbtPropertySetPermission ret = null;
            if( null != Node && Members().Contains( Node.ObjClass.ObjectClass.ObjectClass ) )
            {
                ret = (CswNbtPropertySetPermission) Node.ObjClass;
            }
            return ret;
        }

        public static Collection<CswEnumNbtObjectClass> Members()
        {
            Collection<CswEnumNbtObjectClass> Ret = new Collection<CswEnumNbtObjectClass>
            {
                CswEnumNbtObjectClass.InventoryGroupPermissionClass//,
                //CswEnumNbtObjectClass.ReportPermissionClass,
                //CswEnumNbtObjectClass.MailReportPermissionClass
            };
            return Ret;
        }

        #endregion Base

        #region Abstract Methods

        /// <summary>
        /// Before write node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation );

        /// <summary>
        /// After write node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetWriteNode();

        /// <summary>
        /// Before delete node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false );

        /// <summary>
        /// After delete node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetDeleteNode();

        /// <summary>
        /// Populate props event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetPopulateProps();

        /// <summary>
        /// Button click event for derived classes to implement
        /// </summary>
        public abstract bool onPropertySetButtonClick( NbtButtonData ButtonData );

        /// <summary>
        /// Mechanism to add default filters in derived classes
        /// </summary>
        public abstract void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship );

        /// <summary>
        /// ObjectClass-specific logic for determining NT (and Action?) Permissions on a given target node
        /// </summary>
        //public abstract void can();
        //todo - migrate canContainer(NT permission) from Container - 
        //this might be a virtual function (to override like the inherited events below)
        //it might also just be generic enough to stay here

        #endregion Abstract Methods

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );
            _validateCompoundUniqueness();
            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterWriteNode()
        {
            afterPropertySetWriteNode();
            CswNbtObjClassDefault.afterWriteNode();
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            beforePropertySetDeleteNode( DeleteAllRequiredRelatedNodes );
            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        public override void afterDeleteNode()
        {
            afterPropertySetDeleteNode();
            CswNbtObjClassDefault.afterDeleteNode();
        }

        protected override void afterPopulateProps()
        {
            afterPropertySetPopulateProps();
            CswNbtObjClassDefault.triggerAfterPopulateProps();
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            onPropertySetAddDefaultViewFilters( ParentRelationship );
            CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion Inherited Events

        #region Custom Logic

        private void _validateCompoundUniqueness()
        {
            if( false == IsTemp )
            {
                CswNbtView MatchingPermissionsView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClassProp WorkUnitOCP = this.ObjectClass.getObjectClassProp( WorkUnit.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp RoleOCP = this.ObjectClass.getObjectClassProp( Role.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp GroupOCP = this.ObjectClass.getObjectClassProp( Group.ObjectClassPropId );

                CswNbtViewRelationship parent = MatchingPermissionsView.AddViewRelationship( this.ObjectClass, false );
                MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: WorkUnitOCP,
                    Value: WorkUnit.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: RoleOCP,
                    Value: Role.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: GroupOCP,
                    Value: Group.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                parent.NodeIdsToFilterOut.Add( this.NodeId );

                ICswNbtTree MatchingPermissionsTree = _CswNbtResources.Trees.getTreeFromView( MatchingPermissionsView, false, false, false );
                MatchingPermissionsTree.goToRoot();
                if( MatchingPermissionsTree.getChildNodeCount() > 0 )
                {
                    MatchingPermissionsTree.goToNthChild( 0 );
                    CswPrimaryKey DuplicateNodeId = MatchingPermissionsTree.getNodeIdForCurrentPosition();
                    throw new CswDniException(
                        CswEnumErrorType.Warning,
                        "A Permission with this Role, WorkUnit and " + GroupClass + " already exists",
                        "A node of nodeid " + DuplicateNodeId + " already exists with Role: \"" + Role.CachedNodeName +
                        "\", WorkUnit: \"" + WorkUnit.CachedNodeName + "\", and " + GroupClass + ": \"" + Group.CachedNodeName + "\"." );
                }
            }
        }

        #endregion Custom Logic

        #region Property Set specific properties

        /// <summary>
        /// The Group with which all Targets shall have these Permissions applied
        /// </summary>
        public abstract CswNbtNodePropRelationship Group { get; }
        public CswNbtNodePropRelationship WorkUnit { get { return _CswNbtNode.Properties[PropertyName.WorkUnit]; } }
        public CswNbtNodePropLogical ApplyToAllWorkUnits { get { return _CswNbtNode.Properties[PropertyName.ApplyToAllWorkUnits]; } }
        public CswNbtNodePropRelationship Role { get { return _CswNbtNode.Properties[PropertyName.Role]; } }
        public CswNbtNodePropLogical ApplyToAllRoles { get { return _CswNbtNode.Properties[PropertyName.ApplyToAllRoles]; } }
        public CswNbtNodePropLogical View { get { return _CswNbtNode.Properties[PropertyName.View]; } }
        public CswNbtNodePropLogical Edit { get { return _CswNbtNode.Properties[PropertyName.Edit]; } }

        #endregion Property Set specific properties

    }//CswNbtPropertySetPermission

}//namespace ChemSW.Nbt.ObjClasses