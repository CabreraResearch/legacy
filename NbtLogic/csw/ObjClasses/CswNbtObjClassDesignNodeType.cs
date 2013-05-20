using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignNodeType : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AuditLevel = "Audit Level";
            public const string Category = "Category";
            public const string DeferSearchTo = "Defer Search To";
            public const string IconFileName = "Icon File Name";
            public const string Locked = "Locked";
            public const string NameTemplate = "Name Template";
            public const string NameTemplateAdd = "Add to Name Template";
            public const string NodeTypeName = "NodeType Name";
            public const string ObjectClass = "Object Class";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeType( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
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
            get { return _CswNbtResources.MetaData.getNodeType( RelationalId.PrimaryKey ); }
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( null != _CswNbtResources.MetaData.getNodeType( NodeTypeName.Text ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Node Type Name must be unique", "Attempted to create a new nodetype with the same name as an existing nodetype" );
            }
        }

        // beforeCreateNode()

        public override void afterCreateNode()
        {
            // ------------------------------------------------------------
            // This logic from makeNewNodeType in CswNbtMetaData.cs
            // ------------------------------------------------------------

            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                Int32 NodeTypeId = RelationalId.PrimaryKey;

                CswTableUpdate NodeTypesUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeType_afterCreateNode_NodeTypesUpdate", "nodetypes" );
                DataTable NodeTypesTable = NodeTypesUpdate.getTable( "nodetypeid", NodeTypeId );
                if( NodeTypesTable.Rows.Count > 0 )
                {
                    // Set values not controlled by the node
                    DataRow NodeTypesRow = NodeTypesTable.Rows[0];
                    NodeTypesRow["versionno"] = "1";
                    NodeTypesRow["tablename"] = "nodes";
                    NodeTypesRow["enabled"] = CswConvert.ToDbVal( true );
                    NodeTypesRow["nodecount"] = 0;
                    NodeTypesRow["firstversionid"] = NodeTypeId.ToString();
                    NodeTypesUpdate.update( NodeTypesTable );

                    //CswNbtMetaDataNodeType NewNodeType = RelationalNodeType;


                    // Now can create nodetype_props and tabset records
                    CswTableUpdate NodeTypePropTableUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeType_afterCreateNode_NodeTypePropUpdate", "nodetypes" );
                    DataTable NodeTypeProps = NodeTypePropTableUpdate.getTable( "nodetypeid", NodeTypeId );

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

                    //if( OnMakeNewNodeType != null )
                    //    OnMakeNewNodeType( NewNodeType, false );

                    //refreshAll();

                    //will need to refresh auto-views
                    //_RefreshViewForNodetypeId.Add( NodeTypeId );

                } // if( NodeTypeTable.Rows.Count > 0 )
            } // if( CswTools.IsPrimaryKey( RelationalId ) )
        }

        // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
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
            // If the nodetype is a prior version, prevent delete
            if( false == RelationalNodeType.IsLatestVersion() )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "This NodeType cannot be deleted", "User attempted to delete a nodetype that was not the latest version" );
            }

            // Delete Nodes
            {
                CswNbtView NodesView = new CswNbtView( _CswNbtResources );
                NodesView.AddViewRelationship( this.RelationalNodeType, false );

                ICswNbtTree NodesTree = _CswNbtResources.Trees.getTreeFromView( NodesView, false, true, true );
                for( Int32 n = 0; n < NodesTree.getChildNodeCount(); n++ )
                {
                    NodesTree.goToNthChild( n );
                    NodesTree.getCurrentNode().delete( true, true );
                    NodesTree.goToParentNode();
                }
            }

            // Delete Props
            {
                CswNbtMetaDataObjectClass DesignPropOCP = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
                CswNbtMetaDataObjectClassProp NtpNodeTypeOCP = DesignPropOCP.getObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue );

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

                        CswNbtObjClassDesignNodeTypeProp DoomedProp = PropsTree.getCurrentNode();
                        DoomedProp.InternalDelete = true;
                        DoomedProp.Node.delete( true, true );

                        PropsTree.goToParentNode();
                    }
                    PropsTree.goToParentNode();
                }
            } // end: Delete Props

            // Delete Tabs
            {
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
                        TabsTree.getCurrentNode().delete( true, true );
                        TabsTree.goToParentNode();
                    }
                    TabsTree.goToParentNode();
                } // for( Int32 nt = 0; nt < TabsTree.getChildNodeCount(); nt++ )
            } // end: Delete Tabs

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

            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            //validate role nodetype permissions
            foreach( CswNbtObjClassRole roleNode in _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass ).getNodes( false, true ) )
            {
                roleNode.NodeTypePermissions.ValidateValues();
                roleNode.postChanges( false );
            }

            _CswNbtResources.MetaData.DeleteNodeType( RelationalNodeType );
            _CswNbtObjClassDefault.afterDeleteNode();
        }

        // afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            NameTemplateAdd.SetOnPropChange( _NameTemplateAdd_Change );
            ObjectClassProperty.SetOnPropChange( _ObjectClassProperty_Change );

            // Options for Object Class property
            SortedList<string, CswNbtNodeTypePropListOption> ObjectClassOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
            Dictionary<Int32, CswEnumNbtObjectClass> ObjectClassIds = _CswNbtResources.MetaData.getObjectClassIds();
            foreach( Int32 ObjectClassId in ObjectClassIds.Keys )
            {
                string thisObjectClassName = ObjectClassIds[ObjectClassId];
                ObjectClassOptions.Add( thisObjectClassName, new CswNbtNodeTypePropListOption( thisObjectClassName, ObjectClassId.ToString() ) );
            }
            ObjectClassProperty.Options.Override( ObjectClassOptions.Values );

            // Only allowed to edit Object Class on Add, or convert Generics
            if( _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add && CswEnumNbtObjectClass.GenericClass != ObjectClassProperty.Value )
            {
                ObjectClassProperty.ServerManaged = true;
            }
            else
            {
                ObjectClassProperty.ServerManaged = false;
            }

            // Options for Icon File Name property
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

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }

        //afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
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

        public override CswNbtNode CopyNode()
        {
            CswNbtMetaDataNodeType NewNodeType = _CswNbtResources.MetaData.CopyNodeType( RelationalNodeType, string.Empty );
            return _CswNbtResources.Nodes.getNodeByRelationalId( new CswPrimaryKey( "nodetypes", NewNodeType.NodeTypeId ) );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList AuditLevel
        {
            get { return ( _CswNbtNode.Properties[PropertyName.AuditLevel] ); }
        }

        public CswNbtNodePropText Category
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Category] ); }
        }

        public CswNbtNodePropRelationship DeferSearchTo
        {
            get { return ( _CswNbtNode.Properties[PropertyName.DeferSearchTo] ); }
        }

        public CswNbtNodePropImageList IconFileName
        {
            get { return ( _CswNbtNode.Properties[PropertyName.IconFileName] ); }
        }

        public CswNbtNodePropLogical Locked
        {
            get { return ( _CswNbtNode.Properties[PropertyName.Locked] ); }
        }

        public CswNbtNodePropText NameTemplate
        {
            get { return ( _CswNbtNode.Properties[PropertyName.NameTemplate] ); }
        }

        public CswNbtNodePropRelationship NameTemplateAdd
        {
            get { return ( _CswNbtNode.Properties[PropertyName.NameTemplateAdd] ); }
        }

        private void _NameTemplateAdd_Change( CswNbtNodeProp Prop )
        {
            // Add the selected value to the name template
            CswNbtObjClassDesignNodeTypeProp SelectedProp = _CswNbtResources.Nodes[NameTemplateAdd.RelatedNodeId];
            if( null != SelectedProp )
            {
                string newTemplate = NameTemplate.Text;
                if( false == string.IsNullOrEmpty( newTemplate ) )
                {
                    newTemplate += " ";
                }
                newTemplate += CswNbtMetaData.MakeTemplateEntry( SelectedProp.PropName.Text );
                NameTemplate.Text = newTemplate;

                // Clear the selected value
                NameTemplateAdd.RelatedNodeId = null;
                NameTemplateAdd.CachedNodeName = string.Empty;
                NameTemplateAdd.PendingUpdate = false;
            }
        }

        // _NameTemplateAdd_Change()
        public CswNbtNodePropText NodeTypeName
        {
            get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeName] ); }
        }

        public CswNbtNodePropList ObjectClassProperty
        {
            get { return ( _CswNbtNode.Properties[PropertyName.ObjectClass] ); }
        }

        public CswNbtMetaDataObjectClass ObjectClassPropertyValue
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( ObjectClassProperty.Value ) ); }
        }

        private void _ObjectClassProperty_Change( CswNbtNodeProp Prop )
        {
            CswEnumNbtObjectClass OriginalOC = ObjectClassProperty.GetOriginalPropRowValue( CswEnumNbtSubFieldName.Text );
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
                    _setPropertyValuesFromObjectClass();

                    ObjectClassProperty.ServerManaged = true;
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot convert this NodeType", "Nodetype " + RelationalNodeType.NodeTypeName + " (" + RelationalNodeType.NodeTypeId + ") cannot be converted because it is not Generic" );
                }
            }
        } // _ObjectClassProperty_Change

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
                CswNbtObjClassDesignNodeTypeProp PropNode = ( from Prop in RelationalNodeType.getNodeTypeProps()
                                                              where Prop.PropName == OCProp.PropName && Prop.FieldTypeId == OCProp.FieldTypeId
                                                              select _CswNbtResources.Nodes.getNodeByRelationalId( new CswPrimaryKey( "nodetype_props", Prop.PropId ) )
                                                            ).FirstOrDefault();
                // If converting, need to detect existing properties
                if( null != PropNode )
                {
                    PropNode.ObjectClassPropName.Value = OCProp.PropId.ToString();
                    PropNode.syncFromObjectClassProp();

                    NewNTPropsByOCPId.Add( OCProp.PropId, PropNode );
                }
                else
                {
                    string NewPropName = OCProp.PropName;
                    while( null != RelationalNodeType.getNodeTypeProp( NewPropName ) )
                    {
                        NewPropName = NewPropName + " (new)";
                    }

                    CswNbtMetaDataNodeType DesignNodeTypePropNT = _CswNbtResources.MetaData.getNodeType( "Design " + OCProp.getFieldTypeValue().ToString() + " NodeTypeProp" );
                    PropNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( DesignNodeTypePropNT.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).NodeTypeValue.RelatedNodeId = this.NodeId;
                            ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).ObjectClassPropName.Value = OCProp.PropId.ToString();
                            ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).PropName.Text = NewPropName;
                        } );
                    
                    NewNTPropsByOCPId.Add( OCProp.ObjectClassPropId, PropNode );
                } // if-else( null != PropNode )
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
                            NTProp.DisplayConditionFilter.Value = FilterMode.ToString();
                            NTProp.DisplayConditionValue.Text = FilterValue;

                        } // if( TargetOfFilter != null )
                    } // if( null != NTProp )
                } // if( OCProp.hasFilter() )
            } // foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassProps )


            // Handle search defer inheritance from object classes
            if( Int32.MinValue != ObjectClassPropertyValue.SearchDeferPropId )
            {
                if( CswNbtMetaDataObjectClass.NotSearchableValue != ObjectClassPropertyValue.SearchDeferPropId )
                {
                    CswNbtObjClassDesignNodeTypeProp SearchDeferProp = NewNTPropsByOCPId[ObjectClassPropertyValue.SearchDeferPropId];
                    this.DeferSearchTo.RelatedNodeId = SearchDeferProp.NodeId;
                }
                else
                {
                    //NewNodeType.SearchDeferPropId = CswNbtMetaDataObjectClass.NotSearchableValue;
                    this.DeferSearchTo.RelatedNodeId = null;
                }
            }

        } // _setPropertyValuesFromObjectClass()


    }//CswNbtObjClassDesignNodeType

}//namespace ChemSW.Nbt.ObjClasses
