using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
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
                CswEnumNbtObjectClass.InventoryGroupPermissionClass,
                CswEnumNbtObjectClass.ReportGroupPermissionClass,
                CswEnumNbtObjectClass.MailReportGroupPermissionClass
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
        /// ObjectClass-specific logic for determining Action Permissions
        /// </summary>
        public abstract bool canAction( CswNbtAction Action );

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
            WorkUnit.SetOnPropChange( OnWorkUnitPropChange );
            ApplyToAllWorkUnits.SetOnPropChange( OnApplyToAllWorkUnitsPropChange );
            Role.SetOnPropChange( OnRolePropChange );
            ApplyToAllRoles.SetOnPropChange( OnApplyToAllRolesPropChange );
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
                CswNbtMetaDataObjectClassProp ApplyToAllWorkUnitsOCP = ObjectClass.getObjectClassProp( ApplyToAllWorkUnits.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp WorkUnitOCP = ObjectClass.getObjectClassProp( WorkUnit.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp ApplyToAllRolesOCP = ObjectClass.getObjectClassProp( ApplyToAllRoles.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp RoleOCP = ObjectClass.getObjectClassProp( Role.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp GroupOCP = ObjectClass.getObjectClassProp( Group.ObjectClassPropId );

                CswNbtViewRelationship parent = MatchingPermissionsView.AddViewRelationship( ObjectClass, false );
                if( ApplyToAllWorkUnits.Checked == CswEnumTristate.True )
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: ApplyToAllWorkUnitsOCP,
                    Value: CswEnumTristate.True.ToString(),
                    SubFieldName: CswEnumNbtSubFieldName.Checked,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
                else
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: WorkUnitOCP,
                    Value: WorkUnit.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
                if( ApplyToAllRoles.Checked == CswEnumTristate.True )
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: ApplyToAllRolesOCP,
                    Value: CswEnumTristate.True.ToString(),
                    SubFieldName: CswEnumNbtSubFieldName.Checked,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
                else
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: RoleOCP,
                    Value: Role.CachedNodeName,
                    SubFieldName: CswEnumNbtSubFieldName.Name,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
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
                        "A Permission with this Role, WorkUnit and " + Group.PropName + " already exists",
                        "A node of nodeid " + DuplicateNodeId + " already exists with Role: \"" + Role.CachedNodeName +
                        "\", WorkUnit: \"" + WorkUnit.CachedNodeName + "\", and " + Group.PropName + ": \"" + Group.CachedNodeName + "\"." );
                }
            }
        }

        //lame
        public static string getGroupPropName( CswEnumNbtObjectClass PermissionClass )
        {
            string GroupPropName = string.Empty;
            switch( PermissionClass )
            {
                case CswEnumNbtObjectClass.InventoryGroupPermissionClass:
                    GroupPropName = CswNbtObjClassInventoryGroupPermission.PropertyName.InventoryGroup;
                    break;
                case CswEnumNbtObjectClass.MailReportGroupPermissionClass:
                    GroupPropName = CswNbtObjClassMailReportGroupPermission.PropertyName.MailReportGroup;
                    break;
                case CswEnumNbtObjectClass.ReportGroupPermissionClass:
                    GroupPropName = CswNbtObjClassReportGroupPermission.PropertyName.ReportGroup;
                    break;
            }
            return GroupPropName;
        }

        #endregion Custom Logic

        #region Property Set specific properties

        /// <summary>
        /// The Group with which all Targets shall have these Permissions applied
        /// </summary>
        public abstract CswNbtNodePropRelationship Group { get; }
        public CswNbtNodePropRelationship WorkUnit { get { return _CswNbtNode.Properties[PropertyName.WorkUnit]; } }
        private void OnWorkUnitPropChange( CswNbtNodeProp NodeProp )
        {
            if( null == WorkUnit.RelatedNodeId )
            {
                ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
            }
        }
        public CswNbtNodePropLogical ApplyToAllWorkUnits { get { return _CswNbtNode.Properties[PropertyName.ApplyToAllWorkUnits]; } }
        private void OnApplyToAllWorkUnitsPropChange( CswNbtNodeProp NodeProp )
        {
            if( ApplyToAllWorkUnits.Checked == CswEnumTristate.True )
            {
                WorkUnit.RelatedNodeId = null;
                WorkUnit.RefreshNodeName();
            }
            else if( false == IsTemp && ApplyToAllWorkUnits.Checked == CswEnumTristate.False && null == WorkUnit.RelatedNodeId )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "A Work Unit must be selected if 'Apply To All Work Units' is unchecked.", "User did not supply a valid Work Unit." );
            }
        }
        public CswNbtNodePropRelationship Role { get { return _CswNbtNode.Properties[PropertyName.Role]; } }
        private void OnRolePropChange( CswNbtNodeProp NodeProp )
        {
            if( null == Role.RelatedNodeId )
            {
                ApplyToAllRoles.Checked = CswEnumTristate.True;
            }
        }
        public CswNbtNodePropLogical ApplyToAllRoles { get { return _CswNbtNode.Properties[PropertyName.ApplyToAllRoles]; } }
        private void OnApplyToAllRolesPropChange( CswNbtNodeProp NodeProp )
        {
            if( ApplyToAllRoles.Checked == CswEnumTristate.True )
            {
                Role.RelatedNodeId = null;
                Role.RefreshNodeName();
            }
            else if( false == IsTemp && ApplyToAllRoles.Checked == CswEnumTristate.False && null == Role.RelatedNodeId )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "A Role must be selected if 'Apply To All Roles' is unchecked.", "User did not supply a valid Role." );
            }
        }
        public CswNbtNodePropLogical View { get { return _CswNbtNode.Properties[PropertyName.View]; } }
        public CswNbtNodePropLogical Edit { get { return _CswNbtNode.Properties[PropertyName.Edit]; } }

        #endregion Property Set specific properties

    }//CswNbtPropertySetPermission

}//namespace ChemSW.Nbt.ObjClasses