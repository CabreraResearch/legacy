using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignNodeType : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AuditLevel = "Audit Level";
            public const string Category = "Category";
            public const string DeferSearchTo = "Defer Search To";
            public const string Enabled = "Enabled";
            public const string HasLabel = "Has Label";
            public const string IconFileName = "Icon File Name";
            public const string Locked = "Locked";
            public const string NameTemplateText = "Name Template";
            public const string NameTemplateValue = "Name Template Value";
            public const string NameTemplateAdd = "Add to Name Template";
            public const string NodeTypeName = "NodeType Name";
            public const string ObjectClass = "Object Class";
            public const string Searchable = "Searchable";
            public const string ViewNodesButton = "View Nodes";
        }

        private bool _overrideNodeConversionCheck = false;

        //private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeType( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            //_CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        //ctor()

        /// <summary>
        /// This is the object class that OWNS this property (DesignNodeType)
        /// If you want the object class property value, look for ObjectClassProperty
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeType
        /// </summary>
        public static implicit operator CswNbtObjClassDesignNodeType( CswNbtNode Node )
        {
            CswNbtObjClassDesignNodeType ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignNodeTypeClass ) )
            {
                ret = (CswNbtObjClassDesignNodeType) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// The NodeType that this node represents
        /// </summary>
        public CswNbtMetaDataNodeType RelationalNodeType
        {
            get
            {
                CswNbtMetaDataNodeType ret = null;
                if( CswTools.IsPrimaryKey( RelationalId ) )
                {
                    ret = _CswNbtResources.MetaData.getNodeType( RelationalId.PrimaryKey );
                }
                return ret;
            }
        }

        #region Inherited Events

        /// <summary>
        /// True if the create is a result of an internal process (like Copy)
        /// </summary>
        public bool InternalCreate = false;

        private bool _requiresSync = false;

        protected override void beforePromoteNodeLogic( bool OverrideUniqueValidation = false )
        {
            if( false == OverrideUniqueValidation &&
                null != _CswNbtResources.MetaData.getNodeType( NodeTypeName.Text ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Node Type Name must be unique", "Attempted to create a new nodetype with the same name as an existing nodetype" );
            }
        }

        protected override void afterPromoteNodeLogic()
        {
            // ------------------------------------------------------------
            // This logic from makeNewNodeType in CswNbtMetaData.cs
            // ------------------------------------------------------------

            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                Int32 NodeTypeId = RelationalId.PrimaryKey;

                //CswTableUpdate NodeTypesUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeType_afterCreateNode_NodeTypesUpdate", "nodetypes" );
                //DataTable NodeTypesTable = NodeTypesUpdate.getTable( "nodetypeid", NodeTypeId );
                //if( NodeTypesTable.Rows.Count > 0 )
                //{
                // Set values not controlled by the node
                //DataRow NodeTypesRow = NodeTypesTable.Rows[0];
                if( null != RelationalNodeType )
                {
                    DataRow NodeTypesRow = RelationalNodeType._DataRow;

                    NodeTypesRow["versionno"] = "1";
                    NodeTypesRow["tablename"] = "nodes";
                    NodeTypesRow["nodecount"] = 0;
                    NodeTypesRow["firstversionid"] = NodeTypeId.ToString();
                    _CswNbtResources.MetaData._CswNbtMetaDataResources.NodeTypeTableUpdate.update( NodeTypesRow.Table );

                    //CswNbtMetaDataNodeType NewNodeType = RelationalNodeType;

                    if( false == InternalCreate )
                    {
                        // Now can create nodetype_props and tabset records
                        //CswTableUpdate NodeTypePropTableUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeType_afterCreateNode_NodeTypePropUpdate", "nodetypes" );
                        //DataTable NodeTypeProps = NodeTypePropTableUpdate.getTable( "nodetypeid", NodeTypeId );

                        // Make an initial tab
                        //CswNbtMetaDataObjectClass DesignNodeTypeOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass );
                        CswNbtMetaDataObjectClass DesignNodeTypeTabOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass );
                        //CswNbtMetaDataObjectClass DesignNodeTypePropOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );

                        //CswNbtMetaDataNodeType DesignNodeTypeNT = DesignNodeTypeOC.FirstNodeType;
                        CswNbtMetaDataNodeType DesignNodeTypeTabNT = DesignNodeTypeTabOC.FirstNodeType;
                        //CswNbtMetaDataNodeType DesignNodeTypePropNT = DesignNodeTypePropOC.FirstNodeType;

                        if( null != DesignNodeTypeTabNT )
                        {
                            _CswNbtResources.Nodes.makeNodeFromNodeTypeId( DesignNodeTypeTabNT.NodeTypeId, delegate( CswNbtNode NewNode )
                                {
                                    CswNbtObjClassDesignNodeTypeTab IdentityTab = NewNode;
                                    IdentityTab.NodeTypeValue.RelatedNodeId = this.NodeId;
                                    IdentityTab.TabName.Text = CswNbtMetaData.IdentityTabName;
                                    IdentityTab.Order.Value = 0;
                                    //IdentityTab.ServerManaged.Checked = CswEnumTristate.True;
                                    //IdentityTab.postChanges( false );
                                } );

                            _CswNbtResources.Nodes.makeNodeFromNodeTypeId( DesignNodeTypeTabNT.NodeTypeId, delegate( CswNbtNode NewNode )
                                {
                                    CswNbtObjClassDesignNodeTypeTab FirstTab = NewNode;
                                    FirstTab.NodeTypeValue.RelatedNodeId = this.NodeId;
                                    FirstTab.TabName.Text = NodeTypeName.Text;
                                    FirstTab.Order.Value = 1;
                                    // FirstTab.postChanges( false );
                                } );
                        } // if( null != DesignNodeTypeTabNT )


                        // Make initial props
                        _setPropertyValuesFromObjectClass();
                        _requiresSync = false;

                    } // if( false == InternalCreate )

                    //if( OnMakeNewNodeType != null )
                    //    OnMakeNewNodeType( NewNodeType, false );

                    //refreshAll();

                    //will need to refresh auto-views
                    //_RefreshViewForNodetypeId.Add( NodeTypeId );

                } // if( null != RelationalNodeType )


                // Give the current user's role full permissions to the new nodetype
                CswEnumNbtNodeTypePermission[] AllPerms = new CswEnumNbtNodeTypePermission[]
                    {
                        CswEnumNbtNodeTypePermission.Delete,
                        CswEnumNbtNodeTypePermission.Create,
                        CswEnumNbtNodeTypePermission.Edit,
                        CswEnumNbtNodeTypePermission.View
                    };

                if( null != _CswNbtResources.CurrentNbtUser.RoleId )
                {
                    CswNbtNode RoleNode = _CswNbtResources.Nodes[_CswNbtResources.CurrentNbtUser.RoleId];
                    CswNbtObjClassRole RoleNodeAsRole = (CswNbtObjClassRole) RoleNode;

                    // case 23185 - reset permission options
                    RoleNodeAsRole.triggerAfterPopulateProps();

                    _CswNbtResources.Permit.set( AllPerms, RelationalId.PrimaryKey, RoleNodeAsRole, true );

                }//if we have a current user
                else if( _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
                {
                    // Grant permission to Administrator, unless it's a Design nodetype
                    if( RelationalNodeType.getObjectClass().ObjectClass != CswEnumNbtObjectClass.DesignNodeTypeClass &&
                        RelationalNodeType.getObjectClass().ObjectClass != CswEnumNbtObjectClass.DesignNodeTypeTabClass &&
                        RelationalNodeType.getObjectClass().ObjectClass != CswEnumNbtObjectClass.DesignNodeTypePropClass &&
                        RelationalNodeType.getObjectClass().ObjectClass != CswEnumNbtObjectClass.DesignSequenceClass )
                    {
                        CswNbtObjClassRole RoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( "Administrator" );
                        if( RoleNode != null )
                        {
                            _CswNbtResources.Permit.set( AllPerms, RelationalNodeType, RoleNode, true );
                        }
                    }
                    // Grant permission to chemsw_admin
                    CswNbtObjClassRole RoleNode2 = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                    if( RoleNode2 != null )
                    {
                        _CswNbtResources.Permit.set( AllPerms, RelationalNodeType, RoleNode2, true );
                    }
                }

                if( ObjectClassPropertyValue.ObjectClass == CswEnumNbtObjectClass.InspectionDesignClass )
                {
                    _OnMakeNewInspectionDesignNodeType( RelationalNodeType );
                }

            } // if( CswTools.IsPrimaryKey( RelationalId ) )

        } // afterPromoteNode()



        private void _OnMakeNewInspectionDesignNodeType( CswNbtMetaDataNodeType NewNodeType )
        {
            CswNbtMetaDataNodeTypeProp NameProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Name );
            CswNbtMetaDataNodeTypeProp DateProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate );
            CswNbtMetaDataNodeTypeProp SetPreferredProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.SetPreferred );
            CswNbtMetaDataNodeTypeProp FinishedProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Finish );
            CswNbtMetaDataNodeTypeProp CancelledProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Cancel );
            CswNbtMetaDataNodeTypeProp CancelReasonProp = NewNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.CancelReason );

            // CIS-52810 - properties can be null if the nodetype is being copied instead of created
            if( null != NameProp )
            {
                // Set 'Name' default value = nodetypename
                NameProp.getDefaultValue( true ).AsText.Text = NewNodeType.NodeTypeName;

                // The following changes for new forms only
                if( NewNodeType.VersionNo == 1 && false == InternalCreate )
                {
                    // Set nametemplate = Name + Date
                    this.NameTemplateText.Text = CswNbtMetaData.MakeTemplateEntry( NameProp.PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( DateProp.PropName );

                    // Set first tab to be "Details"
                    CswNbtMetaDataNodeTypeTab FirstTab = NewNodeType.getNodeTypeTab( NewNodeType.NodeTypeName );
                    if( null != FirstTab )
                    {
                        //FirstTab = NewNodeType.getSecondNodeTypeTab();
                        FirstTab.DesignNode.TabName.Text = "Details";
                        FirstTab.DesignNode.Order.Value = 10;
                        FirstTab.DesignNode.IncludeInReport.Checked = CswEnumTristate.False;
                        FirstTab.DesignNode.postChanges( false );
                    }

                    // case 20951 - Add an Action tab
                    CswNbtMetaDataNodeTypeTab ActionTab = NewNodeType.getNodeTypeTab( "Action" );
                    if( ActionTab == null )
                    {
                        ActionTab = _CswNbtResources.MetaData.makeNewTab( NewNodeType, "Action", 9 );
                    }

                    _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, SetPreferredProp, true, ActionTab.TabId, 1, 1 );
                    _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, FinishedProp, true, ActionTab.TabId, 2, 1 );
                    _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, CancelledProp, true, ActionTab.TabId, 3, 1 );
                    _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewNodeType.NodeTypeId, CancelReasonProp, true, ActionTab.TabId, 4, 1 );

                } // if( NewNodeType.VersionNo == 1 && !IsCopy )
            } // if( null != NameProp )
        } // OnMakeNewInspectionDesignNodeType()



        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            if( null != RelationalNodeType )
            {
                _syncNameTemplate();
            }
        } //beforeWriteNode()

        protected override void afterWriteNodeLogic()
        {
            if( _requiresSync )
            {
                _setPropertyValuesFromObjectClass();
                _requiresSync = false;
            }
            base.afterWriteNodeLogic();
        }

        protected override void beforeDeleteNodeLogic() // bool DeleteAllRequiredRelatedNodes = false )
        {
            if( false == IsTemp )
            {
                // If the nodetype is a prior version, prevent delete
                if( false == RelationalNodeType.IsLatestVersion() )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "This NodeType cannot be deleted", "User attempted to delete a nodetype that was not the latest version" );
                }

                // Delete Nodes
                foreach( CswNbtNode Node in getNodes() )
                {
                    Node.delete( true, true );
                }

                // Delete Props
                foreach( CswNbtObjClassDesignNodeTypeProp PropNode in getPropNodes() )
                {
                    PropNode.InternalDelete = true;
                    PropNode.Node.delete( true, true );
                }

                // Delete Tabs
                foreach( CswNbtObjClassDesignNodeTypeTab TabNode in getTabNodes() )
                {
                    TabNode.InternalDelete = true;
                    TabNode.Node.delete( true, true );
                }

                //// Delete Views
                //CswTableUpdate ViewsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "DeleteNodeType_viewupdate", "node_views" );
                //CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
                //SelectCols.Add( "nodeviewid" );
                //DataTable ViewsTable = ViewsUpdate.getTable( SelectCols );
                //foreach( DataRow CurrentRow in ViewsTable.Rows )
                //{
                //    //CswNbtView CurrentView = new CswNbtView(_CswNbtResources);
                //    //CurrentView.LoadXml(CswConvert.ToInt32(CurrentRow["nodeviewid"].ToString()));
                //    CswNbtView CurrentView = _CswNbtMetaDataResources.CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) ) );
                //    if( CurrentView.ContainsNodeType( NodeType ) )
                //    {
                //        CurrentView.Delete();
                //    }
                //}
                //ViewsUpdate.update( ViewsTable );
            } // if( false == IsTemp )
            //_CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        } //beforeDeleteNode()

        protected override void afterDeleteNodeLogic()
        {
            if( false == IsTemp )
            {
                //validate role nodetype permissions
                foreach( CswNbtObjClassRole roleNode in _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass ).getNodes( false, true ) )
                {
                    roleNode.NodeTypePermissions.ValidateValues();
                    roleNode.postChanges( false );
                }

                //_CswNbtResources.MetaData.DeleteNodeType( RelationalNodeType );
            }
        } // afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            NodeTypeName.SetOnPropChange( _NodeTypeName_Change );
            NameTemplateAdd.SetOnPropChange( _NameTemplateAdd_Change );
            ObjectClassProperty.SetOnPropChange( _ObjectClassProperty_Change );

            // Prevent renaming "Design" nodetypes
            if( null != RelationalNodeType &&
                null != RelationalNodeType.getObjectClass() &&
                ( RelationalNodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.DesignNodeTypeClass ||
                  RelationalNodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.DesignNodeTypeTabClass ||
                  RelationalNodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.DesignNodeTypePropClass ||
                  RelationalNodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.DesignSequenceClass ) )
            {
                NodeTypeName.setReadOnly( true, true );
            }

            // Options for 'Object Class' property
            SortedList<string, CswNbtNodeTypePropListOption> ObjectClassOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
            Dictionary<Int32, CswEnumNbtObjectClass> ObjectClassIds = _CswNbtResources.MetaData.getObjectClassIds();
            foreach( Int32 ObjectClassId in ObjectClassIds.Keys )
            {
                string thisObjectClassName = ObjectClassIds[ObjectClassId];
                ObjectClassOptions.Add( thisObjectClassName, new CswNbtNodeTypePropListOption( thisObjectClassName, ObjectClassId.ToString() ) );
            }
            Collection<CswNbtNodeTypePropListOption> SortedOptions = new Collection<CswNbtNodeTypePropListOption>( ObjectClassOptions.Values.ToList() );
            ObjectClassProperty.Options.Override( SortedOptions );

            // Only allowed to edit Object Class on Add, or convert Generics
            if( _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add && CswEnumNbtObjectClass.GenericClass != ObjectClassProperty.Value )
            {
                ObjectClassProperty.ServerManaged = true;
            }
            else
            {
                ObjectClassProperty.ServerManaged = false;
            }

            // Options for 'Icon File Name' property
            Dictionary<string, string> IconOptions = new Dictionary<string, string>();
            if( null != HttpContext.Current )
            {
                DirectoryInfo d = new DirectoryInfo( HttpContext.Current.Request.PhysicalApplicationPath + CswNbtMetaDataObjectClass.IconPrefix16 );
                FileInfo[] IconFiles = d.GetFiles();
                foreach( FileInfo IconFile in IconFiles )
                {
                    IconOptions.Add( IconFile.Name, IconFile.Name );
                }
                IconFileName.ImagePrefix = CswNbtMetaDataObjectClass.IconPrefix16;
                IconFileName.Options = IconOptions;
            }

            // Options for 'DeferSearchTo' - all my Relationships or Locations
            CswNbtMetaDataObjectClass NodeTypePropOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            CswNbtMetaDataObjectClassProp NTPNodeTypeOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );
            CswNbtMetaDataObjectClassProp NTPFieldTypeOCP = NodeTypePropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType );

            CswNbtView DeferView = new CswNbtView( _CswNbtResources );
            DeferView.ViewName = "Defer Search To Options";
            // my nodetype (me)
            CswNbtViewRelationship DeferViewRel1 = DeferView.AddViewRelationship( this.ObjectClass, false );
            DeferViewRel1.NodeIdsToFilterIn.Add( this.NodeId );
            // and all my properties
            CswNbtViewRelationship DeferViewRel2 = DeferView.AddViewRelationship( DeferViewRel1, CswEnumNbtViewPropOwnerType.Second, NTPNodeTypeOCP, false );
            // ... that are relationships
            DeferView.AddViewPropertyAndFilter( DeferViewRel2,
                                                NTPFieldTypeOCP,
                                                Conjunction: CswEnumNbtFilterConjunction.And,
                                                SubFieldName: CswNbtFieldTypeRuleList.SubFieldName.Value,
                                                FilterMode: CswEnumNbtFilterMode.Equals,
                                                Value: _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Relationship ).FieldTypeId.ToString() );
            // ... or locations
            DeferView.AddViewPropertyAndFilter( DeferViewRel2,
                                                NTPFieldTypeOCP,
                                                Conjunction: CswEnumNbtFilterConjunction.Or,
                                                FilterMode: CswEnumNbtFilterMode.Equals,
                                                SubFieldName: CswNbtFieldTypeRuleList.SubFieldName.Value,
                                                Value: _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Location ).FieldTypeId.ToString() );
            DeferSearchTo.OverrideView( DeferView );

        } //afterPopulateProps()

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                string OCPPropName = ButtonData.NodeTypeProp.getObjectClassPropName();
                switch( OCPPropName )
                {
                    case PropertyName.ViewNodesButton:
                        CswNbtView DefaultView = RelationalNodeType.CreateDefaultView( includeDefaultFilters: true );
                        DefaultView.ViewName = NodeTypeName.Text + " Default View";
                        DefaultView.SaveToCache( IncludeInQuickLaunch: true );

                        ButtonData.Action = CswEnumNbtButtonAction.loadView;
                        ButtonData.Data["viewid"] = DefaultView.SessionViewId.ToString();
                        ButtonData.Data["viewmode"] = DefaultView.ViewMode.ToString();
                        ButtonData.Data["type"] = "view";
                        break;
                }
            }
            return true;
        }


        public override CswNbtNode CopyNode( bool IsNodeTemp = false, Action<CswNbtNode> OnCopy = null )
        {
            // Copy NodeType
            string NewNodeTypeName = "Copy Of " + NodeTypeName.Text;
            Int32 CopyInt = 1;
            while( null != _CswNbtResources.MetaData.getNodeType( NewNodeTypeName ) )
            {
                CopyInt++;
                NewNodeTypeName = "Copy " + CopyInt.ToString() + " Of " + NodeTypeName.Text;
            }

            CswNbtObjClassDesignNodeType NodeTypeCopy = base.CopyNodeImpl( IsNodeTemp, delegate( CswNbtNode NewNode )
            {
                ( (CswNbtObjClassDesignNodeType) NewNode ).InternalCreate = true;
                ( (CswNbtObjClassDesignNodeType) NewNode ).NodeTypeName.Text = NewNodeTypeName;
                if( null != OnCopy )
                {
                    OnCopy( NewNode );
                }
            } );

            // Copy Tabs
            Dictionary<Int32, CswNbtObjClassDesignNodeTypeTab> TabMap = new Dictionary<Int32, CswNbtObjClassDesignNodeTypeTab>();
            foreach( CswNbtObjClassDesignNodeTypeTab TabNode in getTabNodes() )
            {
                CswNbtObjClassDesignNodeTypeTab TabCopy = TabNode.CopyNode( IsNodeTemp, delegate( CswNbtNode CopiedNode )
                    {
                        ( (CswNbtObjClassDesignNodeTypeTab) CopiedNode ).NodeTypeValue.RelatedNodeId = NodeTypeCopy.NodeId;
                        ( (CswNbtObjClassDesignNodeTypeTab) CopiedNode ).TabName.Text = TabNode.TabName.Text;
                    } );

                TabMap.Add( TabNode.RelationalId.PrimaryKey, TabCopy );
            }

            // case 31518 - props won't be able to see the tab if we don't do this
            _CswNbtResources.MetaData.refreshAll();

            // Copy Props
            Collection<CswNbtObjClassDesignNodeTypeProp> PropNodes = getPropNodes();
            Dictionary<Int32, CswNbtObjClassDesignNodeTypeProp> PropMap = new Dictionary<Int32, CswNbtObjClassDesignNodeTypeProp>();
            foreach( CswNbtObjClassDesignNodeTypeProp PropNode in PropNodes )
            {
                CswNbtObjClassDesignNodeTypeProp PropCopy = PropNode.CopyNode( IsNodeTemp, delegate( CswNbtNode CopiedNode )
                    {
                        ( (CswNbtObjClassDesignNodeTypeProp) CopiedNode ).NodeTypeValue.RelatedNodeId = NodeTypeCopy.NodeId;
                        ( (CswNbtObjClassDesignNodeTypeProp) CopiedNode ).PropName.Text = PropNode.PropName.Text;
                    } );
                PropMap.Add( PropNode.RelationalId.PrimaryKey, PropCopy );

                // Fix layout
                if( PropCopy.PropName.Text.Equals( CswNbtObjClass.PropertyName.Save ) )
                {
                    foreach( CswNbtObjClassDesignNodeTypeTab NewTab in TabMap.Values )
                    {
                        //Case 29181 - Save prop on all tabs except identity
                        if( NewTab.RelationalId.PrimaryKey != NodeTypeCopy.RelationalNodeType.getIdentityTab().TabId )
                        {
                            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NodeTypeCopy.RelationalId.PrimaryKey, PropCopy.RelationalNodeTypeProp, false, NewTab.RelationalId.PrimaryKey, Int32.MaxValue, 1 );
                        }
                    }
                }
                else
                {
                    foreach( CswEnumNbtLayoutType LayoutType in CswEnumNbtLayoutType._All )
                    {
                        Dictionary<Int32, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout> OriginalLayouts = _CswNbtResources.MetaData.NodeTypeLayout.getLayout( LayoutType, PropNode.RelationalNodeTypeProp, null );
                        foreach( CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout OriginalLayout in OriginalLayouts.Values )
                        {
                            if( OriginalLayout != null )
                            {
                                Int32 NewTabId = Int32.MinValue;
                                if( LayoutType == CswEnumNbtLayoutType.Edit )
                                {
                                    NewTabId = TabMap[OriginalLayout.TabId].RelationalId.PrimaryKey;
                                }
                                _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( LayoutType, NodeTypeCopy.RelationalId.PrimaryKey, PropCopy.RelationalNodeTypeProp, true, NewTabId, OriginalLayout.DisplayRow, OriginalLayout.DisplayColumn );
                            }
                        }
                    } // foreach( CswEnumNbtLayoutType LayoutType in CswEnumNbtLayoutType._All )
                }
            } // foreach( CswNbtObjClassDesignNodeTypeProp PropNode in getPropNodes() )


            // Fix Conditional Props (case 22328)
            foreach( CswNbtObjClassDesignNodeTypeProp PropNode in PropNodes )
            {
                if( CswTools.IsPrimaryKey( PropNode.DisplayConditionProperty.RelatedNodeId ) )
                {
                    CswNbtObjClassDesignNodeTypeProp PropCopy = PropMap[PropNode.RelationalId.PrimaryKey];
                    CswNbtObjClassDesignNodeTypeProp DisplayConditionProp = PropNodes.FirstOrDefault( p => p.NodeId == PropNode.DisplayConditionProperty.RelatedNodeId );
                    if( null != DisplayConditionProp )
                    {
                        CswNbtObjClassDesignNodeTypeProp DisplayConditionPropCopy = PropMap[DisplayConditionProp.RelationalId.PrimaryKey];
                        PropCopy.DisplayConditionProperty.RelatedNodeId = DisplayConditionPropCopy.NodeId;
                    }
                }
            }

            // Fix the name template, now that properties are in place
            NodeTypeCopy.postChanges( true );

            //if( OnCopyNodeType != null )
            //    OnCopyNodeType( OldNodeType, NewNodeType );

            return NodeTypeCopy.Node;
        } // CopyNode()

        #endregion

        private void _syncNameTemplate()
        {
            // Set NameTemplateValue from NameTemplateText
            NameTemplateValue.Text = CswNbtMetaData.TemplateTextToTemplateValue( RelationalNodeType.getNodeTypeProps(), NameTemplateText.Text );
        }

        #region Object class specific properties

        public CswNbtNodePropList AuditLevel { get { return ( _CswNbtNode.Properties[PropertyName.AuditLevel] ); } }
        public CswNbtNodePropText Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }
        public CswNbtNodePropRelationship DeferSearchTo { get { return ( _CswNbtNode.Properties[PropertyName.DeferSearchTo] ); } }
        public CswNbtNodePropLogical HasLabel { get { return ( _CswNbtNode.Properties[PropertyName.HasLabel] ); } }
        public CswNbtNodePropImageList IconFileName { get { return ( _CswNbtNode.Properties[PropertyName.IconFileName] ); } }
        public CswNbtNodePropLogical Locked { get { return ( _CswNbtNode.Properties[PropertyName.Locked] ); } }
        public CswNbtNodePropLogical Enabled { get { return ( _CswNbtNode.Properties[PropertyName.Enabled] ); } }
        public CswNbtNodePropText NameTemplateText { get { return ( _CswNbtNode.Properties[PropertyName.NameTemplateText] ); } }
        public CswNbtNodePropText NameTemplateValue { get { return ( _CswNbtNode.Properties[PropertyName.NameTemplateValue] ); } }

        public CswNbtNodePropRelationship NameTemplateAdd { get { return ( _CswNbtNode.Properties[PropertyName.NameTemplateAdd] ); } }
        private void _NameTemplateAdd_Change( CswNbtNodeProp Prop, bool Creating )
        {
            // Add the selected value to the name template
            CswNbtObjClassDesignNodeTypeProp SelectedProp = _CswNbtResources.Nodes[NameTemplateAdd.RelatedNodeId];
            if( null != SelectedProp )
            {
                string newTemplate = NameTemplateText.Text;
                if( false == string.IsNullOrEmpty( newTemplate ) )
                {
                    newTemplate += " ";
                }
                newTemplate += CswNbtMetaData.MakeTemplateEntry( SelectedProp.PropName.Text );
                NameTemplateText.Text = newTemplate;
                _syncNameTemplate();

                // Clear the selected value
                NameTemplateAdd.RelatedNodeId = null;
                NameTemplateAdd.CachedNodeName = string.Empty;
                NameTemplateAdd.PendingUpdate = false;
            } // if( null != SelectedProp )
        } // _NameTemplateAdd_Change()

        public CswNbtNodePropText NodeTypeName { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeName] ); } }
        public void _NodeTypeName_Change( CswNbtNodeProp Prop, bool Creating )
        {
            //if( null != RelationalNodeType && RelationalNodeType.getObjectClass().ObjectClass == CswEnumNbtObjectClass.InspectionDesignClass )
            if( null != RelationalNodeType &&
                null != ObjectClassPropertyValue &&
                ObjectClassPropertyValue.ObjectClass == CswEnumNbtObjectClass.InspectionDesignClass )
            {
                // Set 'Name' default value = nodetypename
                CswNbtMetaDataNodeTypeProp NameProp = RelationalNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Name );
                NameProp.getDefaultValue( true ).AsText.Text = RelationalNodeType.NodeTypeName;
            }
            if( false == CswTools.IsValidName( NodeTypeName.Text ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "NodeType Name must start with a letter, and can only contain alphanumeric characters and spaces.", "Attempted to name the NodeType '" + NodeTypeName.Text + "'" );
            }
        } // _NodeTypeName_Change()

        public CswNbtNodePropList ObjectClassProperty { get { return ( _CswNbtNode.Properties[PropertyName.ObjectClass] ); } }

        public CswNbtMetaDataObjectClass ObjectClassPropertyValue { get { return _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( ObjectClassProperty.Value ) ); } }
        private void _ObjectClassProperty_Change( CswNbtNodeProp Prop, bool Creating )
        {
            CswEnumNbtObjectClass OriginalOC = ObjectClassProperty.GetOriginalPropRowValue( CswNbtFieldTypeRuleList.SubFieldName.Text );
            if( false == string.IsNullOrEmpty( OriginalOC ) &&
                OriginalOC != CswNbtResources.UnknownEnum &&
                ObjectClassPropertyValue.ObjectClass != CswEnumNbtObjectClass.GenericClass &&
                ObjectClassPropertyValue.ObjectClass != OriginalOC )
            {
                if( OriginalOC == CswEnumNbtObjectClass.GenericClass )
                {
                    // Convert NodeType

                    //NodeType = CheckVersioning( RelationalNodeType );

                    IconFileName.Value.FromString( ObjectClassPropertyValue.IconFileName );

                    // Sync properties with new object class
                    // see case 31678
                    _requiresSync = true;

                    ObjectClassProperty.ServerManaged = true;
                }
                else if( _overrideNodeConversionCheck )
                {
                    _requiresSync = true;
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot convert this NodeType", "Nodetype " + RelationalNodeType.NodeTypeName + " (" + RelationalNodeType.NodeTypeId + ") cannot be converted because it is not Generic" );
                }
            }
        } // _ObjectClassProperty_Change

        public CswNbtNodePropLogical Searchable { get { return ( _CswNbtNode.Properties[PropertyName.Searchable] ); } }
        public CswNbtNodePropButton ViewNodesButton { get { return ( _CswNbtNode.Properties[PropertyName.ViewNodesButton] ); } }

        #endregion

        /// <summary>
        /// Create properties based from the object class
        /// </summary>
        private void _setPropertyValuesFromObjectClass()
        {
            Dictionary<Int32, CswNbtObjClassDesignNodeTypeProp> NewNTPropsByOCPId = new Dictionary<Int32, CswNbtObjClassDesignNodeTypeProp>();
            //int DisplayRow = 1;

            // Create/convert object class props
            foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassPropertyValue.getObjectClassProps() )
            {
                CswNbtObjClassDesignNodeTypeProp PropNode = null;
                if( null != RelationalNodeType )
                {
                    PropNode = ( from Prop in RelationalNodeType.getNodeTypeProps()
                                 where ( Prop.PropName == OCProp.PropName || Prop.getObjectClassPropName() == OCProp.PropName ) && Prop.FieldTypeId == OCProp.FieldTypeId
                                 select Prop.DesignNode
                               ).FirstOrDefault();
                }
                // If converting, need to detect existing properties
                if( null != PropNode )
                {
                    PropNode.ObjectClassPropName.Value = OCProp.PropId.ToString();
                    NewNTPropsByOCPId.Add( OCProp.PropId, PropNode );
                }
                else
                {
                    string NewPropName = OCProp.PropName;
                    while( null != RelationalNodeType &&
                           null != RelationalNodeType.getNodeTypeProp( NewPropName ) )
                    {
                        NewPropName = NewPropName + " (new)";
                    }

                    CswNbtMetaDataNodeType DesignNodeTypePropNT = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassDesignNodeTypeProp.getNodeTypeName( OCProp.getFieldTypeValue() ) );
                    PropNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( DesignNodeTypePropNT.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).NodeTypeValue.RelatedNodeId = this.NodeId;
                            ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).ObjectClassPropName.Value = OCProp.PropId.ToString();
                            ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).FieldType.Value = OCProp.FieldTypeId.ToString();
                            ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).PropName.Text = NewPropName;
                        } );

                    NewNTPropsByOCPId.Add( OCProp.ObjectClassPropId, PropNode );
                } // if-else( null != PropNode )

                //only update the layout when we're not overriding an existing nodetype
                PropNode.syncFromObjectClassProp( false == _overrideNodeConversionCheck );

            } // foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassPropertyValue.getObjectClassProps() )

            // Now that we're done with all object class props, we can handle filters
            foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassPropertyValue.getObjectClassProps() )
            {
                if( OCProp.hasFilter() )
                {
                    CswNbtObjClassDesignNodeTypeProp NTProp = NewNTPropsByOCPId[OCProp.ObjectClassPropId];
                    if( null != NTProp )
                    {
                        CswNbtObjClassDesignNodeTypeProp TargetOfFilter = NewNTPropsByOCPId[OCProp.FilterObjectClassPropId];
                        if( TargetOfFilter != null )
                        {
                            CswNbtSubField SubField = null;
                            CswEnumNbtFilterMode FilterMode = CswEnumNbtFilterMode.Unknown;
                            string FilterValue = string.Empty;
                            OCProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );

                            // We don't have to worry about versioning in this function
                            NTProp.DisplayConditionProperty.RelatedNodeId = TargetOfFilter.NodeId;
                            NTProp.DisplayConditionSubfield.Value = SubField.Name.ToString();
                            NTProp.DisplayConditionFilterMode.Value = FilterMode.ToString();
                            NTProp.DisplayConditionValue.Text = FilterValue;
                            NTProp.postChanges( false );
                        } // if( TargetOfFilter != null )
                    } // if( null != NTProp )
                } // if( OCProp.hasFilter() )
            } // foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassProps )


            // Handle search defer inheritance from object classes
            if( Int32.MinValue != ObjectClassPropertyValue.SearchDeferPropId )
            {
                //if( CswNbtMetaDataObjectClass.NotSearchableValue != ObjectClassPropertyValue.SearchDeferPropId )
                //{
                CswNbtObjClassDesignNodeTypeProp SearchDeferProp = NewNTPropsByOCPId[ObjectClassPropertyValue.SearchDeferPropId];
                this.DeferSearchTo.RelatedNodeId = SearchDeferProp.NodeId;
                //}
                //else
                //{
                //    this.DeferSearchTo.RelatedNodeId = null;
                //}
            }

        } // _setPropertyValuesFromObjectClass()


        public Collection<CswNbtObjClassDesignNodeTypeProp> getPropNodes()
        {
            Collection<CswNbtObjClassDesignNodeTypeProp> ret = new Collection<CswNbtObjClassDesignNodeTypeProp>();

            CswNbtMetaDataObjectClass DesignPropOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            CswNbtMetaDataObjectClassProp NtpNodeTypeOCP = DesignPropOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );

            CswNbtView PropsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship ocRel = PropsView.AddViewRelationship( this.ObjectClass, false );
            ocRel.NodeIdsToFilterIn.Add( this.NodeId );
            PropsView.AddViewRelationship( ocRel, CswEnumNbtViewPropOwnerType.Second, NtpNodeTypeOCP, false );

            ICswNbtTree PropsTree = _CswNbtResources.Trees.getTreeFromView( PropsView, false, true, true );
            for( Int32 nt = 0; nt < PropsTree.getChildNodeCount(); nt++ )
            {
                PropsTree.goToNthChild( nt );
                for( Int32 p = 0; p < PropsTree.getChildNodeCount(); p++ )
                {
                    PropsTree.goToNthChild( p );
                    ret.Add( PropsTree.getCurrentNode() );
                    PropsTree.goToParentNode();
                }
                PropsTree.goToParentNode();
            } // for( Int32 nt = 0; nt < PropsTree.getChildNodeCount(); nt++ )
            return ret;
        } // PropNodes()

        public Collection<CswNbtObjClassDesignNodeTypeTab> getTabNodes()
        {
            Collection<CswNbtObjClassDesignNodeTypeTab> ret = new Collection<CswNbtObjClassDesignNodeTypeTab>();

            CswNbtMetaDataObjectClass DesignTabOCP = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass );
            CswNbtMetaDataObjectClassProp NttNodeTypeOCP = DesignTabOCP.getObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue );

            CswNbtView TabsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship ocRel = TabsView.AddViewRelationship( this.ObjectClass, false );
            ocRel.NodeIdsToFilterIn.Add( this.NodeId );
            TabsView.AddViewRelationship( ocRel, CswEnumNbtViewPropOwnerType.Second, NttNodeTypeOCP, false );

            ICswNbtTree TabsTree = _CswNbtResources.Trees.getTreeFromView( TabsView, false, true, true );
            for( Int32 nt = 0; nt < TabsTree.getChildNodeCount(); nt++ )
            {
                TabsTree.goToNthChild( nt );
                for( Int32 t = 0; t < TabsTree.getChildNodeCount(); t++ )
                {
                    TabsTree.goToNthChild( t );
                    ret.Add( TabsTree.getCurrentNode() );
                    TabsTree.goToParentNode();
                }
                TabsTree.goToParentNode();
            } // for( Int32 nt = 0; nt < TabsTree.getChildNodeCount(); nt++ )
            return ret;
        } // TabNodes()

        public Collection<CswNbtNode> getNodes()
        {
            Collection<CswNbtNode> ret = new Collection<CswNbtNode>();

            CswNbtView NodesView = new CswNbtView( _CswNbtResources );
            NodesView.AddViewRelationship( this.RelationalNodeType, false );

            ICswNbtTree NodesTree = _CswNbtResources.Trees.getTreeFromView( NodesView, false, true, true, true );
            for( Int32 n = 0; n < NodesTree.getChildNodeCount(); n++ )
            {
                NodesTree.goToNthChild( n );
                ret.Add( NodesTree.getCurrentNode() );
                NodesTree.goToParentNode();
            }
            return ret;
        } // Nodes()


        public void RecalculateQuestionNumbers()
        {
            foreach( CswNbtObjClassDesignNodeTypeTab Tab in this.getTabNodes() )
            {
                Int32 CurrentQuestionNo = 1;
                // Do non-conditional ones first
                Dictionary<CswPrimaryKey, Int32> PropQuestionNumbers = new Dictionary<CswPrimaryKey, Int32>();
                Collection<CswNbtObjClassDesignNodeTypeProp> Props = Tab.getPropNodesByDisplayOrder( NumberedOnly: true );
                foreach( CswNbtObjClassDesignNodeTypeProp Prop in Props )
                {
                    if( //Prop.UseNumbering.Checked == CswEnumTristate.True && 
                        Prop.DisplayConditionProperty.Empty )
                    {
                        Prop.QuestionNo.Value = CurrentQuestionNo;
                        Prop.SubQuestionNo.Value = Int32.MinValue;
                        Prop.postOnlyChanges( ForceUpdate: false, SkipEvents: true );
                        PropQuestionNumbers[Prop.NodeId] = CurrentQuestionNo;
                        CurrentQuestionNo++;
                    }
                } // foreach( CswNbtObjClassDesignNodeTypeProp Prop in Tab.getPropNodesByDisplayOrder() )

                // Now do the conditional ones (with numbered parents)
                Int32[] SubQuestionNos = new Int32[CurrentQuestionNo + 1];
                for( Int32 i = 1; i <= CurrentQuestionNo; i++ )
                {
                    SubQuestionNos[i] = 1;
                }

                foreach( CswNbtObjClassDesignNodeTypeProp Prop in Props )
                {
                    if( //Prop.UseNumbering.Checked == CswEnumTristate.True &&
                        false == Prop.DisplayConditionProperty.Empty )
                    {
                        //CswNbtMetaDataNodeTypeProp ParentProp = NodeTypePropsCollection.getNodeTypeProp( Prop.FilterNodeTypePropId ).getNodeTypePropLatestVersion();
                        if( PropQuestionNumbers.ContainsKey( Prop.DisplayConditionProperty.RelatedNodeId ) )
                        {
                            Int32 ParentPropQuestionNo = PropQuestionNumbers[Prop.DisplayConditionProperty.RelatedNodeId];
                            Prop.QuestionNo.Value = ParentPropQuestionNo;
                            Prop.SubQuestionNo.Value = SubQuestionNos[ParentPropQuestionNo];
                            Prop.postOnlyChanges( ForceUpdate: false, SkipEvents: true );
                            SubQuestionNos[ParentPropQuestionNo] += 1;
                        }
                    }
                }
            } // foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
        } // RecalculateQuestionNumbers()

        /// <summary>
        /// STOP. IS THIS REALLY WHAT YOU WANT TO DO? 
        /// Change the object class of the nodetype referenced by this DNT node. Make sure you update all the props manually
        /// </summary>
        /// <param name="NewOC">the object class you want to apply to this NT</param>
        public void changeParentObjectClass( CswNbtMetaDataObjectClass NewOC )
        {
            _overrideNodeConversionCheck = true;

            Node.Properties[PropertyName.ObjectClass].SetSubFieldValue( CswEnumNbtSubFieldName.Value, NewOC.ObjectClassId );

            Node.postChanges( true );

            _overrideNodeConversionCheck = false;
        }




    }//CswNbtObjClassDesignNodeType

}//namespace ChemSW.Nbt.ObjClasses
