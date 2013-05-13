using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
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
        }//ctor()

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
        }
        
        public override void afterCreateNode()
        {
            _CswNbtResources.MetaData.makeNewNodeType
            //#region ...NodeType

            ///// <summary>
            ///// Creates a brand new NodeType in the database and in the MetaData collection
            ///// </summary>
            ///// <param name="ObjectClassName"></param>
            ///// <param name="NodeTypeName"></param>
            ///// <param name="Category"></param>
            ///// <returns></returns>
            //public CswNbtMetaDataNodeType makeNewNodeType( string ObjectClassName, string NodeTypeName, string Category )
            //{
            //    CswEnumNbtObjectClass NbtObjectClass = ObjectClassName;
            //    if( NbtObjectClass == CswNbtResources.UnknownEnum )
            //    {
            //        throw ( new CswDniException( "No such object class: " + ObjectClassName ) );
            //    }

            //    Int32 ObjectClassId = getObjectClass( NbtObjectClass ).ObjectClassId;


            //    return ( makeNewNodeType( ObjectClassId, NodeTypeName, Category ) );

            //}//makeNewNodeType()

            ///// <summary>
            ///// Creates a brand new NodeType in the database and in the MetaData collection
            ///// </summary>
            ///// <param name="NodeTypeRowFromXml">A DataRow derived from exported XML</param>
            //public CswNbtMetaDataNodeType makeNewNodeType( DataRow NodeTypeRowFromXml )
            //{
            //    CswNbtMetaDataNodeType NewNodeType = makeNewNodeType( CswConvert.ToInt32( NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_ObjectClassId] ),
            //                                                          NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_NodeTypeName].ToString(),
            //                                                          NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_Category].ToString() );
            //    NewNodeType.IconFileName = NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_IconFileName].ToString();
            //    NewNodeType.TableName = NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_TableName].ToString();
            //    // can't do this here since we have no properties yet
            //    //NewNodeType.NameTemplateText = NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_NameTemplate].ToString();
            //    return NewNodeType;
            //}

            ///// <summary>
            ///// Creates a brand new NodeType in the database and in the MetaData collection
            ///// </summary>
            ///// <param name="ObjectClassId">Primary key of Object Class</param>
            ///// <param name="NodeTypeName">Name of New NodeType</param>
            ///// <param name="Category">Category to assign NodeType; can be empty</param>
            ///// <returns>CswNbtMetaDataNodeType object for new NodeType</returns>
            //public CswNbtMetaDataNodeType makeNewNodeType( Int32 ObjectClassId, string NodeTypeName, string Category )
            //{
            //    if( NodeTypeName == string.Empty )
            //    { throw new CswDniException( CswEnumErrorType.Warning, "Node Type Name is required", "Attempted to create a new nodetype with a null nodetypename" ); }

            //    // Only new versions of the same nodetype can reuse the name
            //    if( getNodeType( NodeTypeName ) != null )
            //    { throw new CswDniException( CswEnumErrorType.Warning, "Node Type Name must be unique", "Attempted to create a new nodetype with the same name as an existing nodetype" ); }

            //    CswNbtMetaDataObjectClass ObjectClass = getObjectClass( ObjectClassId );

            //    return makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( ObjectClass )
            //                               {
            //                                   Category = Category,
            //                                   NodeTypeName = NodeTypeName
            //                               }
            //                           );


            //} // makeNewNodeType()

            //public CswNbtMetaDataNodeType makeNewNodeType( CswNbtWcfMetaDataModel.NodeType NtModel )
            //{
            //    if( NtModel.NodeTypeName == string.Empty )
            //    { throw new CswDniException( CswEnumErrorType.Warning, "Node Type Name is required", "Attempted to create a new nodetype with a null nodetypename" ); }

            //    // Only new versions of the same nodetype can reuse the name
            //    if( getNodeType( NtModel.NodeTypeName ) != null )
            //    { throw new CswDniException( CswEnumErrorType.Warning, "Node Type Name must be unique", "Attempted to create a new nodetype with the same name as an existing nodetype" ); }

            //    DataTable NodeTypesTable = _CswNbtMetaDataResources.NodeTypeTableUpdate.getEmptyTable();

            //    DataRow InsertedNodeTypesRow = NodeTypesTable.NewRow();
            //    InsertedNodeTypesRow["objectclassid"] = NtModel.ObjectClassId;
            //    InsertedNodeTypesRow["iconfilename"] = NtModel.IconFileName;
            //    InsertedNodeTypesRow["nodetypename"] = NtModel.NodeTypeName;
            //    InsertedNodeTypesRow["category"] = NtModel.Category;
            //    InsertedNodeTypesRow["versionno"] = "1";
            //    InsertedNodeTypesRow["islocked"] = CswConvert.ToDbVal( false );
            //    InsertedNodeTypesRow["tablename"] = "nodes";
            //    InsertedNodeTypesRow["enabled"] = CswConvert.ToDbVal( true );
            //    InsertedNodeTypesRow["searchdeferpropid"] = CswConvert.ToDbVal( NtModel.SearchDeferNodeTypePropId );    // see below for inheritance from object classes
            //    InsertedNodeTypesRow["nodecount"] = 0;
            //    NodeTypesTable.Rows.Add( InsertedNodeTypesRow );

            //    Int32 NodeTypeId = CswConvert.ToInt32( InsertedNodeTypesRow["nodetypeid"] );
            //    InsertedNodeTypesRow["firstversionid"] = NodeTypeId.ToString();
            //    _CswNbtMetaDataResources.NodeTypeTableUpdate.update( NodeTypesTable );

            //    // Update MetaData Collection
            //    //_CswNbtMetaDataResources.NodeTypesCollection.RegisterNew( InsertedNodeTypesRow );

            //    CswNbtMetaDataNodeType NewNodeType = getNodeType( NodeTypeId );

            //    // Now can create nodetype_props and tabset records

            //    DataTable NodeTypeProps = _CswNbtMetaDataResources.NodeTypePropTableUpdate.getTable( "nodetypeid", NodeTypeId );

            //    // Make an initial tab
            //    CswNbtMetaDataNodeTypeTab IdentityTab = makeNewTab( NewNodeType, IdentityTabName, 0 );
            //    IdentityTab.ServerManaged = true;

            //    CswNbtMetaDataNodeTypeTab FirstTab = makeNewTab( NewNodeType, InsertedNodeTypesRow["nodetypename"].ToString(), 1 );

            //    // Make initial props
            //    Dictionary<Int32, CswNbtMetaDataNodeTypeProp> NewNTPropsByOCPId = new Dictionary<Int32, CswNbtMetaDataNodeTypeProp>();
            //    int DisplayRow = 1;
            //    IEnumerable<CswNbtMetaDataObjectClassProp> ObjectClassProps = NtModel.ObjectClass.getObjectClassProps();
            //    foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassProps )
            //    {
            //        DataRow NewNodeTypePropRow = NodeTypeProps.NewRow();

            //        // Set default initial values for this prop
            //        // (basic info needed for creating the NodeTypeProp)
            //        NewNodeTypePropRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
            //        NewNodeTypePropRow["fieldtypeid"] = CswConvert.ToDbVal( OCProp.FieldTypeId );
            //        NewNodeTypePropRow["objectclasspropid"] = CswConvert.ToDbVal( OCProp.PropId );
            //        NewNodeTypePropRow["propname"] = CswConvert.ToDbVal( OCProp.PropName );
            //        NodeTypeProps.Rows.Add( NewNodeTypePropRow );
            //        NewNodeTypePropRow["firstpropversionid"] = NewNodeTypePropRow["nodetypepropid"].ToString();

            //        // Now copy information from the Object Class Prop
            //        CopyNodeTypePropFromObjectClassProp( OCProp, NewNodeTypePropRow );

            //        //CswNbtMetaDataNodeTypeProp NewProp = (CswNbtMetaDataNodeTypeProp) _CswNbtMetaDataResources.NodeTypePropsCollection.RegisterNew( NewNodeTypePropRow );
            //        CswNbtMetaDataNodeTypeProp NewProp = new CswNbtMetaDataNodeTypeProp( _CswNbtMetaDataResources, NewNodeTypePropRow );
            //        _CswNbtMetaDataResources.NodeTypePropsCollection.AddToCache( NewProp );
            //        NewNTPropsByOCPId.Add( OCProp.ObjectClassPropId, NewProp );

            //        // Handle setFk()
            //        if( OCProp.FKValue != Int32.MinValue )
            //        {
            //            NewProp.SetFK( OCProp.FKType, OCProp.FKValue, OCProp.ValuePropType, OCProp.ValuePropId );
            //        }

            //        // Handle default values
            //        CopyNodeTypePropDefaultValueFromObjectClassProp( OCProp, NewProp );

            //        NewProp.IsQuickSearch = NewProp.getFieldTypeRule().SearchAllowed;

            //        if( OCProp.PropName.Equals( CswNbtObjClass.PropertyName.Save ) ) //case 29181 - Save prop on Add/Edit layouts at the bottom of tab
            //        {
            //            NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NewProp.NodeTypeId, NewProp, true, FirstTab.TabId, Int32.MaxValue, 1 );
            //            NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewProp.NodeTypeId, NewProp, true, FirstTab.TabId, Int32.MaxValue, 1 );
            //        }
            //        else
            //        {
            //            NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NewProp.NodeTypeId, NewProp, true, FirstTab.TabId, DisplayRow, 1 );
            //            if( OCProp.getFieldType().IsLayoutCompatible( CswEnumNbtLayoutType.Add ) &&
            //                ( ( OCProp.IsRequired &&
            //                    false == OCProp.HasDefaultValue() ) ||
            //                  ( OCProp.SetValueOnAdd ||
            //                    ( Int32.MinValue != OCProp.DisplayColAdd &&
            //                      Int32.MinValue != OCProp.DisplayRowAdd ) ) ) )
            //            {
            //                NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NewProp.NodeTypeId, NewProp, true, FirstTab.TabId, OCProp.DisplayRowAdd, OCProp.DisplayColAdd );
            //            }
            //            DisplayRow++;
            //        }
            //    }//iterate object class props

            //    if( NodeTypeProps.Rows.Count > 0 )
            //    {
            //        _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( NodeTypeProps );
            //    }

            //    // Now that we're done with all object class props, we can handle filters
            //    foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassProps )
            //    {
            //        if( OCProp.hasFilter() )
            //        {
            //            //CswNbtMetaDataNodeTypeProp NTProp = NewNodeType.getNodeTypePropByObjectClassProp( OCProp.PropName );
            //            CswNbtMetaDataNodeTypeProp NTProp = NewNTPropsByOCPId[OCProp.ObjectClassPropId];
            //            if( null != NTProp )
            //            {
            //                //CswNbtMetaDataNodeTypeProp TargetOfFilter = NewNodeType.getNodeTypePropByObjectClassProp( ObjectClass.getObjectClassProp( OCProp.FilterObjectClassPropId ).PropName );
            //                CswNbtMetaDataNodeTypeProp TargetOfFilter = NewNTPropsByOCPId[OCProp.FilterObjectClassPropId];
            //                if( TargetOfFilter != null )
            //                {
            //                    //NTProp.FilterNodeTypePropId = TargetOfFilter.FirstPropVersionId;
            //                    CswNbtSubField SubField = null;
            //                    CswEnumNbtFilterMode FilterMode = CswEnumNbtFilterMode.Unknown;
            //                    string FilterValue = string.Empty;
            //                    OCProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );
            //                    // We don't have to worry about versioning in this function
            //                    NTProp.setFilter( TargetOfFilter, SubField, FilterMode, FilterValue );
            //                }
            //            }
            //        }
            //    }//iterate object class props

            //    // Handle search defer inheritance from object classes
            //    if( Int32.MinValue != NtModel.SearchDeferObjectClassPropId )
            //    {
            //        if( CswNbtMetaDataObjectClass.NotSearchableValue != NtModel.SearchDeferObjectClassPropId )
            //        {
            //            NewNodeType.SearchDeferPropId = NewNodeType.getNodeTypePropByObjectClassProp( NtModel.SearchDeferObjectClassPropId ).PropId;
            //        }
            //        else
            //        {
            //            NewNodeType.SearchDeferPropId = CswNbtMetaDataObjectClass.NotSearchableValue;
            //        }
            //    }

            //    if( OnMakeNewNodeType != null )
            //        OnMakeNewNodeType( NewNodeType, false );

            //    refreshAll();

            //    //will need to refresh auto-views
            //    _RefreshViewForNodetypeId.Add( NodeTypeId );

            //    return NewNodeType;
            //} // makeNewNodeType()

            //#endregion ...NodeType

        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
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
            _CswNbtResources.MetaData.DeleteNodeType( RelationalNodeType );
            _CswNbtObjClassDefault.afterDeleteNode();
        } // afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            NameTemplateAdd.SetOnPropChange( _NameTemplateAdd_Change );
            ObjectClassProperty.SetOnPropChange( _ObjectClassProperty_Change );

            // Options for Object Class property
            SortedList<string,CswNbtNodeTypePropListOption> ObjectClassOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
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

        public override CswNbtNode CopyNode()
        {
            CswNbtMetaDataNodeType NewNodeType = _CswNbtResources.MetaData.CopyNodeType( RelationalNodeType, string.Empty );
            return _CswNbtResources.Nodes.getNodeByRelationalId( new CswPrimaryKey( "nodetypes", NewNodeType.NodeTypeId ) );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList AuditLevel { get { return ( _CswNbtNode.Properties[PropertyName.AuditLevel] ); } }
        public CswNbtNodePropText Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }
        public CswNbtNodePropRelationship DeferSearchTo { get { return ( _CswNbtNode.Properties[PropertyName.DeferSearchTo] ); } }
        public CswNbtNodePropImageList IconFileName { get { return ( _CswNbtNode.Properties[PropertyName.IconFileName] ); } }
        public CswNbtNodePropLogical Locked { get { return ( _CswNbtNode.Properties[PropertyName.Locked] ); } }
        public CswNbtNodePropText NameTemplate { get { return ( _CswNbtNode.Properties[PropertyName.NameTemplate] ); } }
        public CswNbtNodePropRelationship NameTemplateAdd { get { return ( _CswNbtNode.Properties[PropertyName.NameTemplateAdd] ); } }
        private void _NameTemplateAdd_Change(CswNbtNodeProp Prop)
        {
            // Add the selected value to the name template
            CswNbtObjClassDesignNodeTypeProp SelectedProp = _CswNbtResources.Nodes[NameTemplateAdd.RelatedNodeId];
            if( null != SelectedProp )
            {
                if( false == string.IsNullOrEmpty( NameTemplate.Text ) )
                {
                    NameTemplate.Text += " ";
                }
                NameTemplate.Text += CswNbtMetaData.MakeTemplateEntry( SelectedProp.PropName.Text );
                
                // Clear the selected value
                NameTemplateAdd.RelatedNodeId = null;
                NameTemplateAdd.CachedNodeName = string.Empty;
                NameTemplateAdd.PendingUpdate = false;
            }
        }
        public CswNbtNodePropText NodeTypeName { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeName] ); } }

        public CswNbtNodePropList ObjectClassProperty { get { return ( _CswNbtNode.Properties[PropertyName.ObjectClass] ); } }
        public CswNbtMetaDataObjectClass ObjectClassPropertyValue { get { return _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( ObjectClassProperty.Value ) ); } }
        private void _ObjectClassProperty_Change( CswNbtNodeProp Prop )
        {
            if( ObjectClassPropertyValue.ObjectClass != CswEnumNbtObjectClass.GenericClass &&
                ObjectClassProperty.GetOriginalPropRowValue( CswEnumNbtSubFieldName.Text ) == CswEnumNbtObjectClass.GenericClass )
            {
                // Convert NodeType
                _CswNbtResources.MetaData.ConvertObjectClass( RelationalNodeType, ObjectClassPropertyValue );

                ObjectClassProperty.ServerManaged = true;
            }
        }

        #endregion


    }//CswNbtObjClassDesignNodeType

}//namespace ChemSW.Nbt.ObjClasses
