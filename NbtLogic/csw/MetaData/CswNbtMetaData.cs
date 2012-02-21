using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Represents NodeType, ObjectClass, Tab, Property, and FieldType data
    /// </summary>
    public class CswNbtMetaData
    {
        protected CswNbtResources CswNbtResources;
        public CswNbtMetaDataResources _CswNbtMetaDataResources;

        public CswNbtMetaDataNodeTypeLayoutMgr NodeTypeLayout;

        protected bool _ExcludeDisabledModules = true;
        public Collection <Int32> _RefreshViewForNodetypeId = new Collection<Int32>();
        protected bool _ResetAllViews = false;

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtMetaData( CswNbtResources Resources, bool ExcludeDisabledModules )
        {
            CswNbtResources = Resources;
            _ExcludeDisabledModules = ExcludeDisabledModules;
            _CswNbtMetaDataResources = new CswNbtMetaDataResources( CswNbtResources, this );
            NodeTypeLayout = new CswNbtMetaDataNodeTypeLayoutMgr( _CswNbtMetaDataResources );
            refreshAll();
        }

        /// <summary>
        /// Constructor, for CswNbtMetaDataForSchemaUpdater
        /// </summary>
        public CswNbtMetaData( CswNbtResources Resources, CswNbtMetaDataResources MetaDataResources, bool ExcludeDisabledModules )
        {
            CswNbtResources = Resources;
            _ExcludeDisabledModules = ExcludeDisabledModules;
            _CswNbtMetaDataResources = MetaDataResources;
            NodeTypeLayout = new CswNbtMetaDataNodeTypeLayoutMgr( _CswNbtMetaDataResources );
            refreshAll();
        }

        /// <summary>
        /// Refresh the contents of the Meta Data object from the database
        /// </summary>
        public virtual void refreshAll()
        {
            _CswNbtMetaDataResources.refreshAll(); // _ExcludeDisabledModules );
        }//refreshAll()


        #endregion Initialization

        #region Selectors

        /// <summary>
        /// Collection of Node Type primary keys (Int32)
        /// </summary>
        public Collection<Int32> getNodeTypeIds()
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypeIds();
        }

        /// <summary>
        /// Collection of NodeType primary keys, filtered by object class
        /// </summary>
        /// <param name="ObjectClassId"></param>
        /// <returns></returns>
        public Collection<Int32> getNodeTypeIds( Int32 ObjectClassId )
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypeIds( ObjectClassId );
        }

        /// <summary>
        /// Collection of CswNbtMetaDataNodeType objects
        /// </summary>
        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes()
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypes();
        }

        /// <summary>
        /// Collection of CswNbtMetaDataNodeType objects that have the provided object class
        /// </summary>
        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass )
        {
            CswNbtMetaDataObjectClass theObjectClass = this.getObjectClass( ObjectClass );
            return getNodeTypes( theObjectClass.ObjectClassId );
        }

        /// <summary>
        /// Collection of CswNbtMetaDataNodeType objects that have the provided object class
        /// </summary>
        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes( Int32 ObjectClassId )
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypes( ObjectClassId );
        }

        /// <summary>
        /// Collection of Node Types, latest versions only
        /// </summary>
        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypesLatestVersion()
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypesLatestVersion();
        }


        

        /// <summary>
        /// Collection of Object Class primary keys (Int32)
        /// </summary>
        public Dictionary<CswNbtMetaDataObjectClass.NbtObjectClass, Int32> getObjectClassIds()
        {
            return _CswNbtMetaDataResources.ObjectClassesCollection.getObjectClassIds();
        }

        /// <summary>
        /// Collection of CswNbtMetaDataObjectClass objects
        /// </summary>
        public IEnumerable<CswNbtMetaDataObjectClass> getObjectClasses()
        {
            return _CswNbtMetaDataResources.ObjectClassesCollection.getObjectClasses();
        }

        /// <summary>
        /// Collection of Field Type primary keys
        /// </summary>
        public Dictionary<CswNbtMetaDataFieldType.NbtFieldType, Int32> getFieldTypeIds()
        {
            return _CswNbtMetaDataResources.FieldTypesCollection.getFieldTypeIds();
        }

        /// <summary>
        /// Collection of CswNbtMetaDataFieldType objects
        /// </summary>
        public IEnumerable<CswNbtMetaDataFieldType> getFieldTypes()
        {
            return _CswNbtMetaDataResources.FieldTypesCollection.getFieldTypes();
        }

        /// <summary>
        /// Returns a CswNbtMetaDataObjectClass based on the given NbtObjectClass parameter
        /// </summary>
        public CswNbtMetaDataObjectClass getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass )
        {
            return _CswNbtMetaDataResources.ObjectClassesCollection.getObjectClass( ObjectClass );
        }

        /// <summary>
        /// Returns a CswNbtMetaDataObjectClass based on the object class primary key parameter
        /// </summary>
        public CswNbtMetaDataObjectClass getObjectClass( Int32 ObjectClassId )
        {
            return _CswNbtMetaDataResources.ObjectClassesCollection.getObjectClass( ObjectClassId );
        }

        /// <summary>
        /// Finds the object class of a nodetype, from the nodetype's primary key
        /// </summary>
        public CswNbtMetaDataObjectClass getObjectClassByNodeTypeId( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.ObjectClassesCollection.getObjectClassByNodeTypeId( NodeTypeId );
        }

        /// <summary>
        /// Returns the first version of a particular nodetype
        /// </summary>
        public CswNbtMetaDataNodeType getNodeTypeFirstVersion( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypeFirstVersion( NodeTypeId );
        }
        
        /// <summary>
        /// Returns the latest version of a particular nodetype
        /// </summary>
        public CswNbtMetaDataNodeType getNodeTypeLatestVersion( CswNbtMetaDataNodeType NodeType )
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypeLatestVersion( NodeType );
        }
        /// <summary>
        /// Returns the latest version of a particular nodetype
        /// </summary>
        public CswNbtMetaDataNodeType getNodeTypeLatestVersion( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypeLatestVersion( NodeTypeId );
        }

        /// <summary>
        /// Returns the CswNbtMetaDataNodeType record for the latest version of a nodetype, by name
        /// </summary>
        public CswNbtMetaDataNodeType getNodeType( string NodeTypeName )
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeTypeLatestVersion( NodeTypeName );
        }

        /// <summary>
        /// Returns the CswNbtMetaDataNodeType record by primary key
        /// </summary>
        public CswNbtMetaDataNodeType getNodeType( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.NodeTypesCollection.getNodeType( NodeTypeId );
        }

        /// <summary>
        /// Returns a CswNbtMetaDataFieldType based on the NbtFieldType provided
        /// </summary>
        public CswNbtMetaDataFieldType getFieldType( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return _CswNbtMetaDataResources.FieldTypesCollection.getFieldType( FieldType );
        }

        /// <summary>
        /// Returns a CswNbtMetaDataFieldType based on the Field Type primary key provided
        /// </summary>
        public CswNbtMetaDataFieldType getFieldType( Int32 FieldTypeId )
        {
            return _CswNbtMetaDataResources.FieldTypesCollection.getFieldType( FieldTypeId );
        }

        /// <summary>
        /// Returns the NbtFieldType value for a property
        /// </summary>
        public CswNbtMetaDataFieldType.NbtFieldType getFieldTypeValueForNodeTypePropId( Int32 NodeTypePropId )
        {
            return _CswNbtMetaDataResources.FieldTypesCollection.getFieldTypeValueForNodeTypePropId( NodeTypePropId );
        }

        /// <summary>
        /// Returns the NbtFieldType value for a property
        /// </summary>
        public CswNbtMetaDataFieldType.NbtFieldType getFieldTypeValueForObjectClassPropId( Int32 ObjectClassPropId )
        {
            return _CswNbtMetaDataResources.FieldTypesCollection.getFieldTypeValueForObjectClassPropId( ObjectClassPropId );
        }

        /// <summary>
        /// Fetches a NodeType Property based on the primary key (all nodetypes)
        /// </summary>
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypePropId )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypeProp( NodeTypePropId );
        }

        /// <summary>
        /// Fetches a NodeType Property based on the primary key of the property from any version, for the given nodetypeid version
        /// </summary>
        public CswNbtMetaDataNodeTypeProp getNodeTypePropVersion( Int32 NodeTypeId, Int32 NodeTypePropId )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropVersion( NodeTypeId, NodeTypePropId );
        }

        /// <summary>
        /// Fetches a NodeType Property based on the nodetype and property name
        /// </summary>
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypeId, string PropName )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypeProp( NodeTypeId, PropName );
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypePropLatestVersion( Int32 NodeTypePropId )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropLatestVersion( NodeTypePropId );
        }

        /// <summary>
        /// Fetches a NodeType Property based on the nodetype and object class prop primary key)
        /// </summary>
        public CswNbtMetaDataNodeTypeProp getNodeTypePropByObjectClassProp( Int32 NodeTypeId, Int32 ObjectClassPropId )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropByObjectClassProp( NodeTypeId, ObjectClassPropId );
        }

        /// <summary>
        /// Fetches a NodeType Property based on the nodetype and the object class property name
        /// </summary>
        public CswNbtMetaDataNodeTypeProp getNodeTypePropByObjectClassProp( Int32 NodeTypeId, string ObjectClassPropName )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropByObjectClassProp( NodeTypeId, ObjectClassPropName );
        }

        /// <summary>
        /// Fetches a NodeType Tab based on the primary key (all nodetypes)
        /// </summary>
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeTabId )
        {
            return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTab( NodeTypeTabId );
        }
        /// <summary>
        /// Fetches a NodeType Tab based on the primary key (all nodetypes)
        /// </summary>
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeId, string TabName )
        {
            return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTab( NodeTypeId, TabName );
        }

        /// <summary>
        /// Fetches a NodeType Tab based on the primary key (all nodetypes)
        /// </summary>
        public CswNbtMetaDataNodeTypeTab getNodeTypeTabVersion( Int32 NodeTypeId, Int32 NodeTypeTabId )
        {
            return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTabVersion( NodeTypeId, NodeTypeTabId );
        }

        

        /// <summary>
        /// Fetches an Object Class Property based on the primary key (all object classes)
        /// </summary>
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassProp( ObjectClassPropId );
        }
        /// <summary>
        /// Fetches an Object Class Property based on the primary key (all object classes)
        /// </summary>
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassId, string ObjectClassPropName )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassProp( ObjectClassId, ObjectClassPropName );
        }
        /// <summary>
        /// Fetches Object Class Properties based on the object class primary key
        /// </summary>
        public IEnumerable<CswNbtMetaDataObjectClassProp> getObjectClassProps( Int32 ObjectClassId )
        {
            return _CswNbtMetaDataResources.ObjectClassPropsCollection.getObjectClassPropsByObjectClass( ObjectClassId );
        }

        public ICswNbtFieldTypeRule getFieldTypeRule( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return _CswNbtMetaDataResources.makeFieldTypeRule( FieldType );
        }

        public Collection<Int32> getNodeTypeTabIds( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTabIds( NodeTypeId );
        }

        public IEnumerable<CswNbtMetaDataNodeTypeTab> getNodeTypeTabs( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.NodeTypeTabsCollection.getNodeTypeTabs( NodeTypeId );
        }

        public Collection<Int32> getNodeTypePropIds( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropIds( NodeTypeId );
        }

        public Int32 getNodeTypePropId( Int32 NodeTypeId, string PropName )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropId( NodeTypeId, PropName );
        }

        public Int32 getNodeTypePropIdByObjectClassProp( Int32 NodeTypeId, string ObjectClassPropName )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropIdByObjectClassProp( NodeTypeId, ObjectClassPropName );
        }

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps( Int32 NodeTypeId )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypeProps( NodeTypeId );
        }

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypeProps( FieldType );
        }

        #endregion Selectors

        #region XML

        public static string _Element_MetaData = "CswNbtMetaData";

        #endregion XML

        #region Events

        /// <summary>
        /// Defines events that occur when a property is renamed
        /// </summary>
        public delegate void EditPropNameEventHandler( CswNbtMetaDataNodeTypeProp EditedProp );
        /// <summary>
        /// Event handle for editing nodetype property names
        /// </summary>
        [NonSerialized]
        public EditPropNameEventHandler OnEditNodeTypePropName = null;

        /// <summary>
        /// Defines events that occur when a nodetype is renamed
        /// </summary>
        public delegate void EditNodeTypeNameEventHandler( CswNbtMetaDataNodeType EditedNodeType );
        /// <summary>
        /// Event handle for editing nodetype names
        /// </summary>
        [NonSerialized]
        public EditNodeTypeNameEventHandler OnEditNodeTypeName = null;

        /// <summary>
        /// Defines events that occur when a property is deleted
        /// </summary>
        public delegate void DeletePropEventHandler( CswNbtMetaDataNodeTypeProp DeletedProp );
        /// <summary>
        /// Event handle for deleting nodetype properties
        /// </summary>
        [NonSerialized]
        public DeletePropEventHandler OnDeleteNodeTypeProp = null;
        /// <summary>
        /// Internal event that occurs when a property is deleted
        /// </summary>

        /// <summary>
        /// Defines events that occur when a nodetype is copied
        /// </summary>
        public delegate void CopyNodeTypeEventHandler( CswNbtMetaDataNodeType OriginalNodeType, CswNbtMetaDataNodeType CopyNodeType );
        /// <summary>
        /// Event handle for copying nodetypes
        /// </summary>
        [NonSerialized]
        public CopyNodeTypeEventHandler OnCopyNodeType = null;

        /// <summary>
        /// Defines events that occur when a new nodetype is created
        /// </summary>
        public delegate void NewNodeTypeEventHandler( CswNbtMetaDataNodeType NewNodeType, bool IsCopy );
        /// <summary>
        /// Event handle for creating new nodetypes
        /// </summary>
        [NonSerialized]
        public NewNodeTypeEventHandler OnMakeNewNodeType = null;

        /// <summary>
        /// Defines events that occur on new property creation
        /// </summary>
        public delegate void NewNodeTypePropEventHandler( CswNbtMetaDataNodeTypeProp NewProp );
        /// <summary>
        /// Event handle for creating new nodetype properties
        /// </summary>
        [NonSerialized]
        public NewNodeTypePropEventHandler OnMakeNewNodeTypeProp = null;


        #endregion Events

        #region Make New...

        public CswNbtMetaDataFieldType makeNewFieldType( CswNbtMetaDataFieldType.NbtFieldType FieldType, CswNbtMetaDataFieldType.DataType DataType, string FieldPrecision = "", string Mask = "" )
        {
            CswNbtMetaDataFieldType RetFieldType = null;
            if( FieldType != CswNbtMetaDataFieldType.NbtFieldType.Unknown && DataType != CswNbtMetaDataFieldType.DataType.UNKNOWN )
            {
                RetFieldType = getFieldType( FieldType );
                if( null == RetFieldType )
                {
                    DataTable FieldTypeTable = _CswNbtMetaDataResources.FieldTypeTableUpdate.getEmptyTable();
                    DataRow Row = FieldTypeTable.NewRow();
                    Row["datatype"] = CswConvert.ToDbVal( DataType.ToString().ToLower() );
                    Row["fieldtype"] = FieldType.ToString();
                    Row["fieldprecision"] = CswConvert.ToDbVal( FieldPrecision );
                    Row["mask"] = CswConvert.ToDbVal( Mask );
                    FieldTypeTable.Rows.Add( Row );
                    _CswNbtMetaDataResources.FieldTypeTableUpdate.update( FieldTypeTable );

                    // Keep MetaData up to date
                    //RetFieldType = _CswNbtMetaDataResources.FieldTypesCollection.RegisterNew( Row ) as CswNbtMetaDataFieldType;
                    refreshAll();
                }
            }

            return RetFieldType;
        }//makeNewFieldType()


        /// <summary>
        /// Creates a brand new NodeType in the database and in the MetaData collection
        /// </summary>
        /// <param name="ObjectClassName"></param>
        /// <param name="NodeTypeName"></param>
        /// <param name="Category"></param>
        /// <returns></returns>
        public CswNbtMetaDataNodeType makeNewNodeType( string ObjectClassName, string NodeTypeName, string Category )
        {
            //Enum.Parse(
            if( !Enum.IsDefined( typeof( CswNbtMetaDataObjectClass.NbtObjectClass ), ObjectClassName ) )
                throw ( new CswDniException( "No such object class: " + ObjectClassName ) );

            CswNbtMetaDataObjectClass.NbtObjectClass NbtObjectClass = (CswNbtMetaDataObjectClass.NbtObjectClass) Enum.Parse( typeof( CswNbtMetaDataObjectClass.NbtObjectClass ), ObjectClassName, true );

            Int32 ObjectClassId = getObjectClass( NbtObjectClass ).ObjectClassId;


            return ( makeNewNodeType( ObjectClassId, NodeTypeName, Category ) );

        }//makeNewNodeType()

        /// <summary>
        /// Creates a brand new NodeType in the database and in the MetaData collection
        /// </summary>
        /// <param name="NodeTypeRowFromXml">A DataRow derived from exported XML</param>
        public CswNbtMetaDataNodeType makeNewNodeType( DataRow NodeTypeRowFromXml )
        {
            CswNbtMetaDataNodeType NewNodeType = makeNewNodeType( CswConvert.ToInt32( NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_ObjectClassId] ),
                                                                  NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_NodeTypeName].ToString(),
                                                                  NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_Category].ToString() );
            NewNodeType.IconFileName = NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_IconFileName].ToString();
            NewNodeType.TableName = NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_TableName].ToString();
            // can't do this here since we have no properties yet
            //NewNodeType.NameTemplateText = NodeTypeRowFromXml[CswNbtMetaDataNodeType._Attribute_NameTemplate].ToString();
            return NewNodeType;
        }

        /// <summary>
        /// Creates a brand new NodeType in the database and in the MetaData collection
        /// </summary>
        /// <param name="ObjectClassId">Primary key of Object Class</param>
        /// <param name="NodeTypeName">Name of New NodeType</param>
        /// <param name="Category">Category to assign NodeType; can be empty</param>
        /// <returns>CswNbtMetaDataNodeType object for new NodeType</returns>
        public CswNbtMetaDataNodeType makeNewNodeType( Int32 ObjectClassId, string NodeTypeName, string Category )
        {
            if( NodeTypeName == string.Empty )
                throw new CswDniException( ErrorType.Warning, "Node Type Name is required", "Attempted to create a new nodetype with a null nodetypename" );

            // Only new versions of the same nodetype can reuse the name
            if( getNodeType( NodeTypeName ) != null )
                throw new CswDniException( ErrorType.Warning, "Node Type Name must be unique", "Attempted to create a new nodetype with the same name as an existing nodetype" );

            CswNbtMetaDataObjectClass ObjectClass = getObjectClass( ObjectClassId );

            DataTable NodeTypesTable = _CswNbtMetaDataResources.NodeTypeTableUpdate.getEmptyTable();

            DataRow InsertedNodeTypesRow = NodeTypesTable.NewRow();
            InsertedNodeTypesRow["objectclassid"] = ObjectClass.ObjectClassId.ToString();
            InsertedNodeTypesRow["iconfilename"] = ObjectClass.IconFileName;
            InsertedNodeTypesRow["nodetypename"] = NodeTypeName;
            InsertedNodeTypesRow["category"] = Category;
            InsertedNodeTypesRow["versionno"] = "1";
            InsertedNodeTypesRow["islocked"] = "0";
            InsertedNodeTypesRow["tablename"] = "nodes";
            NodeTypesTable.Rows.Add( InsertedNodeTypesRow );
            Int32 NodeTypeId = CswConvert.ToInt32( InsertedNodeTypesRow["nodetypeid"] );
            InsertedNodeTypesRow["firstversionid"] = NodeTypeId.ToString();
            _CswNbtMetaDataResources.NodeTypeTableUpdate.update( NodeTypesTable );

            // Update MetaData Collection
            //_CswNbtMetaDataResources.NodeTypesCollection.RegisterNew( InsertedNodeTypesRow );

            CswNbtMetaDataNodeType NewNodeType = getNodeType( NodeTypeId );

            // Now can create nodetype_props and tabset records

            DataTable NodeTypeProps = _CswNbtMetaDataResources.NodeTypePropTableUpdate.getTable( "nodetypeid", NodeTypeId );

            // Make an initial tab
            CswNbtMetaDataNodeTypeTab FirstTab = makeNewTab( NewNodeType, InsertedNodeTypesRow["nodetypename"].ToString(), 1 );

            // Make initial props
            Dictionary<Int32, CswNbtMetaDataNodeTypeProp> NewNTPropsByOCPId = new Dictionary<Int32, CswNbtMetaDataNodeTypeProp>(); 
            int DisplayRow = 1;
            IEnumerable<CswNbtMetaDataObjectClassProp> ObjectClassProps = ObjectClass.ObjectClassProps;
            foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassProps )
            {
                DataRow NewNodeTypePropRow = NodeTypeProps.NewRow();

                // Set default initial values for this prop
                // (basic info needed for creating the NodeTypeProp)
                NewNodeTypePropRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                //NewNodeTypePropRow["nodetypetabsetid"] = CswConvert.ToDbVal(FirstTab.TabId);
                //NewNodeTypePropRow["display_row"] = CswConvert.ToDbVal(DisplayRow);
                //NewNodeTypePropRow["display_col"] = CswConvert.ToDbVal(1);
                //NewNodeTypePropRow["display_row_add"] = CswConvert.ToDbVal(DisplayRow);
                //NewNodeTypePropRow["display_col_add"] = CswConvert.ToDbVal(1);
                NewNodeTypePropRow["fieldtypeid"] = CswConvert.ToDbVal( OCProp.FieldTypeId );
                NewNodeTypePropRow["objectclasspropid"] = CswConvert.ToDbVal( OCProp.PropId );
                NewNodeTypePropRow["propname"] = CswConvert.ToDbVal( OCProp.PropName );
                NodeTypeProps.Rows.Add( NewNodeTypePropRow );
                NewNodeTypePropRow["firstpropversionid"] = NewNodeTypePropRow["nodetypepropid"].ToString();

                // Now copy information from the Object Class Prop
                CopyNodeTypePropFromObjectClassProp( OCProp, NewNodeTypePropRow );

                //CswNbtMetaDataNodeTypeProp NewProp = (CswNbtMetaDataNodeTypeProp) _CswNbtMetaDataResources.NodeTypePropsCollection.RegisterNew( NewNodeTypePropRow );
                CswNbtMetaDataNodeTypeProp NewProp = new CswNbtMetaDataNodeTypeProp( _CswNbtMetaDataResources, NewNodeTypePropRow );
                _CswNbtMetaDataResources.NodeTypePropsCollection.AddToCache( NewProp );
		        NewNTPropsByOCPId.Add( OCProp.ObjectClassPropId, NewProp );

                // Handle default values
                CopyNodeTypePropDefaultValueFromObjectClassProp( OCProp, NewProp );

                NewProp.IsQuickSearch = NewProp.getFieldTypeRule().SearchAllowed;

                NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, NewProp.NodeTypeId, NewProp.PropId, FirstTab.TabId, DisplayRow, 1 );
                if( OCProp.IsRequired && false == OCProp.HasDefaultValue() )
                {
                    NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, NewProp.NodeTypeId, NewProp.PropId, FirstTab.TabId, Int32.MinValue, Int32.MinValue );
                }
                DisplayRow++;
            }//iterate object class props

            if( NodeTypeProps.Rows.Count > 0 )
            {
                _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( NodeTypeProps );
            }

            // Now that we're done with all object class props, we can handle filters
            foreach( CswNbtMetaDataObjectClassProp OCProp in ObjectClassProps )
            {
                if( OCProp.hasFilter() )
                {
                    //CswNbtMetaDataNodeTypeProp NTProp = NewNodeType.getNodeTypePropByObjectClassProp( OCProp.PropName );
                    CswNbtMetaDataNodeTypeProp NTProp = NewNTPropsByOCPId[OCProp.ObjectClassPropId];
                    if( null != NTProp )
                    {
                        //CswNbtMetaDataNodeTypeProp TargetOfFilter = NewNodeType.getNodeTypePropByObjectClassProp( ObjectClass.getObjectClassProp( OCProp.FilterObjectClassPropId ).PropName );
                        CswNbtMetaDataNodeTypeProp TargetOfFilter = NewNTPropsByOCPId[OCProp.FilterObjectClassPropId];
                        if( TargetOfFilter != null )
                        {
                            //NTProp.FilterNodeTypePropId = TargetOfFilter.FirstPropVersionId;
                            CswNbtSubField SubField = null;
                            CswNbtPropFilterSql.PropertyFilterMode FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Undefined;
                            string FilterValue = string.Empty;
                            OCProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );
                            // We don't have to worry about versioning in this function
                            NTProp.setFilter( TargetOfFilter.FirstPropVersionId, SubField, FilterMode, FilterValue );
                        }
                    }
                }
            }//iterate object class props

            if( OnMakeNewNodeType != null )
                OnMakeNewNodeType( NewNodeType, false );

            refreshAll();

            //will need to refresh auto-views
            _RefreshViewForNodetypeId.Add(NodeTypeId);

            return NewNodeType;
        } // makeNewNodeType()


        /// <summary>
        /// Creates a brand new Tab in the database and in the MetaData collection
        /// </summary>
        /// <param name="NodeType">NodeType to which to assign the new tab</param>
        /// <param name="NodeTypeTabRowFromXml">A DataRow derived from exported XML</param>
        public CswNbtMetaDataNodeTypeTab makeNewTab( CswNbtMetaDataNodeType NodeType, DataRow NodeTypeTabRowFromXml )
        {
            CswNbtMetaDataNodeTypeTab NewTab = makeNewTab( NodeType,
                                                           NodeTypeTabRowFromXml[CswNbtMetaDataNodeTypeTab._Attribute_TabName].ToString(),
                                                           CswConvert.ToInt32( NodeTypeTabRowFromXml[CswNbtMetaDataNodeTypeTab._Attribute_Order] ) );
            return NewTab;
        }

        /// <summary>
        /// Creates a brand new Tab in the database and in the MetaData collection
        /// </summary>
        /// <param name="NodeType">Node Type for new tab</param>
        /// <param name="TabName">Name of new tab</param>
        /// <param name="TabOrder">Order value for new tab</param>
        public CswNbtMetaDataNodeTypeTab makeNewTab( CswNbtMetaDataNodeType NodeType, string TabName, Int32 TabOrder )
        {
            if( TabName == "" )
                throw new CswDniException( ErrorType.Warning, "New Tabs must have a non-blank name",
                                          "Attempted to create a new nodetype_tabset record with a null tabname field" );

            // Only new versions of the same nodetype can reuse the name
            if( NodeType.getNodeTypeTab( TabName ) != null )
                throw new CswDniException( ErrorType.Warning, "Tab Name must be unique (per NodeType)", "Attempted to create a new nodetypetab with the same name as an existing nodetypetab on the same nodetype" );

            // Version, if necessary
            NodeType = CheckVersioning( NodeType );

            //CswTableCaddy TabsTableCaddy = _CswNbtResources.makeCswTableCaddy("nodetype_tabset");
            DataTable TabsTable = _CswNbtMetaDataResources.NodeTypeTabTableUpdate.getEmptyTable();
            DataRow Row = TabsTable.NewRow();
            Row["nodetypeid"] = CswConvert.ToDbVal( NodeType.NodeTypeId );
            Row["tabname"] = TabName;
            Row["taborder"] = CswConvert.ToDbVal( TabOrder );
            Row["includeinnodereport"] = CswConvert.ToDbVal( true );
            TabsTable.Rows.Add( Row );
            Row["firsttabversionid"] = Row["nodetypetabsetid"];
            _CswNbtMetaDataResources.NodeTypeTabTableUpdate.update( TabsTable );

            refreshAll();

            // Keep MetaData up to date
            //CswNbtMetaDataNodeTypeTab NewTab = _CswNbtMetaDataResources.NodeTypeTabsCollection.RegisterNew( Row ) as CswNbtMetaDataNodeTypeTab;
            CswNbtMetaDataNodeTypeTab NewTab = new CswNbtMetaDataNodeTypeTab( _CswNbtMetaDataResources, Row );
            _CswNbtMetaDataResources.NodeTypeTabsCollection.AddToCache( NewTab );
            return NewTab;

        }//makeNewTab()

        /// <summary>
        /// Creates a new property in the database and in the MetaData collection.
        /// </summary>
        public CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeProp InsertAfterProp, CswNbtMetaDataFieldType.NbtFieldType FieldType, string PropName )
        {
            return makeNewProp( NodeType, InsertAfterProp, getFieldType( FieldType ), PropName, Int32.MinValue, false, null );
        }
        /// <summary>
        /// Creates a new property in the database and in the MetaData collection.
        /// </summary>
        public CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataFieldType.NbtFieldType FieldType, string PropName, string NodeTypeTabName )
        {
            CswNbtMetaDataNodeTypeTab CswNbtMetaDataNodeTypeTab = null;
            if( string.Empty == NodeTypeTabName )
                CswNbtMetaDataNodeTypeTab = NodeType.getFirstNodeTypeTab();
            else
                CswNbtMetaDataNodeTypeTab = NodeType.getNodeTypeTab( NodeTypeTabName );
            if( null == CswNbtMetaDataNodeTypeTab )
                throw ( new CswDniException( ErrorType.Error, "No such Nodetype Tab: " + NodeTypeTabName, "NodeType " + NodeType.NodeTypeName + " (" + NodeType.NodeTypeId.ToString() + ") does not contain a tab with name: " + NodeTypeTabName ) );

            return makeNewProp( NodeType, null, getFieldType( FieldType ), PropName, CswNbtMetaDataNodeTypeTab.TabId, false, null );
        }

        /// <summary>
        /// Creates a new property in the database and in the MetaData collection.
        /// </summary>
        public CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataFieldType.NbtFieldType FieldType, string PropName, Int32 TabId )
        {
            return makeNewProp( NodeType, null, getFieldType( FieldType ), PropName, TabId, false, null );
        }

        /// <summary>
        /// Creates a new property in the database and in the MetaData collection.
        /// </summary>
        public CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, Int32 FieldTypeId, string PropName, Int32 TabId )
        {
            return makeNewProp( NodeType, null, FieldTypeId, PropName, TabId, false, null );
        }

        /// <summary>
        /// Creates a new property in the database and in the MetaData collection.
        /// </summary>
        public CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeProp InsertAfterProp, Int32 FieldTypeId, string PropName )
        {
            return makeNewProp( NodeType, InsertAfterProp, FieldTypeId, PropName, Int32.MinValue, false, null );
        }

        /// <summary>
        /// Creates a new property in the database and in the MetaData collection.
        /// </summary>
        /// <param name="NodeType">NodeType to which to assign the new property</param>
        /// <param name="Tab">Tab to which to assign the new property</param>
        /// <param name="NodeTypePropRowFromXml">A DataRow derived from exported XML</param>
        public CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeTab Tab, DataRow NodeTypePropRowFromXml )
        {
            CswNbtMetaDataFieldType FieldType = getFieldType( (CswNbtMetaDataFieldType.NbtFieldType) Enum.Parse( typeof( CswNbtMetaDataFieldType.NbtFieldType ), NodeTypePropRowFromXml[CswNbtMetaDataNodeTypeProp._Attribute_fieldtype].ToString() ) );
            CswNbtMetaDataNodeTypeProp NewProp = makeNewProp( NodeType,
                                                              null,
                                                              FieldType,
                                                              NodeTypePropRowFromXml[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName].ToString(),
                                                              Tab.TabId,
                                                              true,
                                                              null );
            NewProp.SetFromXmlDataRow( NodeTypePropRowFromXml );
            return NewProp;
        }


        protected CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeProp InsertAfterProp, Int32 FieldTypeId, string PropName, Int32 TabId, bool PreventVersioning, CswNbtMetaDataObjectClassProp ObjectClassPropToCopy )
        {
            CswNbtMetaDataFieldType FieldType = getFieldType( FieldTypeId );
            return makeNewProp( NodeType, InsertAfterProp, FieldType, PropName, TabId, PreventVersioning, ObjectClassPropToCopy );
        }

        protected CswNbtMetaDataNodeTypeProp makeNewProp( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeProp InsertAfterProp, CswNbtMetaDataFieldType FieldType, string PropName, Int32 TabId, bool PreventVersioning, CswNbtMetaDataObjectClassProp ObjectClassPropToCopy )
        {
            bool OldPreventVersioning = _CswNbtMetaDataResources._PreventVersioning;
            if( PreventVersioning )
                _CswNbtMetaDataResources._PreventVersioning = true;

            //CswNbtMetaDataFieldType FieldType = getFieldType( FieldTypeId );
            //ICswNbtFieldTypeRule CswNbtFieldTypeRule = FieldType.FieldTypeRule;

            if( PropName == string.Empty )
                throw new CswDniException( ErrorType.Warning, "Property Name is required", "Attempted to create a new nodetype prop with a null propname" );

            // Make sure propname is unique for this nodetype
            //bz # 6157: Calculate displayrowadd
            if( NodeType.getNodeTypeProp( PropName ) != null )
                throw new CswDniException( ErrorType.Warning, "Property Name must be unique per nodetype", "Attempted to save a propname which is equal to a propname of another property in this nodetype" );

            // Version, if necessary
            string OriginalTabName;
            if( TabId != Int32.MinValue )
                OriginalTabName = getNodeTypeTab( TabId ).TabName;
            else if( InsertAfterProp != null && InsertAfterProp.EditLayout.TabId != Int32.MinValue )
            {
                CswNbtMetaDataNodeTypeTab OriginalTab = getNodeTypeTab( InsertAfterProp.EditLayout.TabId );
                //OriginalTabName = InsertAfterProp.EditLayout.Tab.TabName;
                OriginalTabName = OriginalTab.TabName;
            }
            else
                OriginalTabName = NodeType.getFirstNodeTypeTab().TabName;
            NodeType = CheckVersioning( NodeType );
            CswNbtMetaDataNodeTypeTab Tab = NodeType.getNodeTypeTab( OriginalTabName );

            // Create row
            DataTable NodeTypePropsTable = _CswNbtMetaDataResources.NodeTypePropTableUpdate.getEmptyTable();
            DataRow InsertedRow = NodeTypePropsTable.NewRow();

            //Apply parameter values
            Int32 NodeTypeId = NodeType.NodeTypeId;
            InsertedRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
            InsertedRow["fieldtypeid"] = CswConvert.ToDbVal( FieldType.FieldTypeId );

            //InsertedRow["nodetypetabsetid"] = CswConvert.ToDbVal(Tab.TabId);
            if( NodeType.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass &&
                FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Static )
            {
                InsertedRow["usenumbering"] = CswConvert.ToDbVal( true );
            }
            else
            {
                InsertedRow["usenumbering"] = CswConvert.ToDbVal( false );
            }

            //InsertedRow["setvalonadd"] = CswConvert.ToDbVal(false);
            //InsertedRow[ "isquicksearch" ] = CswConvert.ToDbVal( FieldType.FieldTypeRule.SearchAllowed );

            if( FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect )
                InsertedRow["multi"] = "0";
            else
                InsertedRow["multi"] = DBNull.Value;

            if( FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode ||
                FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence )
            {
                // BZ 7987
                InsertedRow["readonly"] = CswConvert.ToDbVal( true );
                InsertedRow["isunique"] = CswConvert.ToDbVal( true );
            }
            else
            {
                InsertedRow["readonly"] = CswConvert.ToDbVal( false );
                InsertedRow["isunique"] = CswConvert.ToDbVal( false );
            }

            //Set display_col and display_row values
            //CswTableCaddy TabCaddy = _CswNbtResources.makeCswTableCaddy("nodetype_tabset");
            //DataTable NodeTypeTabs = null;
            //if( InsertedRow.IsNull( "nodetypetabsetid" ) )
            //{
            //    NodeTypeTabs = _CswNbtMetaDataResources.NodeTypeTabTableUpdate.getTable( "nodetypeid", NodeType.NodeTypeId, new Collection<OrderByClause> { new OrderByClause( "taborder", OrderByType.Ascending ) } );
            //}
            //else
            //{
            //    NodeTypeTabs = _CswNbtMetaDataResources.NodeTypeTabTableUpdate.getTable( "nodetypetabsetid",
            //                       CswConvert.ToInt32( InsertedRow["nodetypetabsetid"] ),
            //                       new Collection<OrderByClause> { new OrderByClause( "taborder", OrderByType.Ascending ) } );
            //}

            //Do actual update
            NodeTypePropsTable.Rows.Add( InsertedRow );

            InsertedRow["firstpropversionid"] = InsertedRow["nodetypepropid"];

            // Copy values from ObjectClassProp
            if( ObjectClassPropToCopy != null )
            {
                CopyNodeTypePropFromObjectClassProp( ObjectClassPropToCopy, InsertedRow );
            }

            //Case 24135: If we specified a unique prop name, keep it
            InsertedRow["propname"] = PropName;

            _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( NodeTypePropsTable );


            // Keep MetaData up to date
            //CswNbtMetaDataNodeTypeProp NewProp = _CswNbtMetaDataResources.NodeTypePropsCollection.RegisterNew( InsertedRow ) as CswNbtMetaDataNodeTypeProp;
            CswNbtMetaDataNodeTypeProp NewProp = new CswNbtMetaDataNodeTypeProp( _CswNbtMetaDataResources, InsertedRow );
            _CswNbtMetaDataResources.NodeTypePropsCollection.AddToCache( NewProp );
            NewProp.IsQuickSearch = NewProp.getFieldTypeRule().SearchAllowed;

            NewProp.getFieldTypeRule().afterCreateNodeTypeProp( NewProp );

            _CswNbtMetaDataResources.RecalculateQuestionNumbers( NodeType );    // this could cause versioning

            if( ObjectClassPropToCopy != null )
                CopyNodeTypePropDefaultValueFromObjectClassProp( ObjectClassPropToCopy, NewProp );

            if( OnMakeNewNodeTypeProp != null )
                OnMakeNewNodeTypeProp( NewProp );

            _CswNbtMetaDataResources._PreventVersioning = OldPreventVersioning;


            if( InsertAfterProp != null )
            {
                NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, NewProp, InsertAfterProp );
                NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, NewProp, InsertAfterProp );
            }
            else //if( NodeTypeTabs.Rows.Count > 0 )
            {
                NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, NewProp.NodeTypeId, NewProp.PropId, Tab.TabId, Int32.MinValue, Int32.MinValue );
                NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, NewProp.NodeTypeId, NewProp.PropId, Int32.MinValue, Int32.MinValue, Int32.MinValue );
            }

            refreshAll();

            //will need to refresh auto-views
            _RefreshViewForNodetypeId.Add( NodeTypeId );

            return NewProp;

        }// makeNewProp()

        #endregion Make New...

        #region Mutators


        /// <summary>
        /// Before making changes to a nodetype (or its tab or properties), call this function to handle whether the nodetype should version
        /// </summary>
        public CswNbtMetaDataNodeType CheckVersioning( CswNbtMetaDataNodeType NodeType )
        {
            CswNbtMetaDataNodeType ret = NodeType;
            if( !_CswNbtMetaDataResources._PreventVersioning )
            {
                // Version, if necessary
                if( NodeType.IsLocked )
                {
                    if( NodeType.IsLatestVersion() )
                    {
                        CswNbtMetaDataNodeType NewNodeTypeVersion = MakeNewVersion( NodeType );
                        ret = NewNodeTypeVersion;
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot modify locked nodetype", "Nodetype " + NodeType.NodeTypeName + " (" + NodeType.NodeTypeId.ToString() + ") cannot be modified because it is locked" );
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Converts a Generic nodetype to another Object Class
        /// Returns either the same nodetype or a new version of the nodetype if versioning was required
        /// </summary>
        public CswNbtMetaDataNodeType ConvertObjectClass( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataObjectClass NewObjectClass )
        {
            CswNbtMetaDataNodeType ret = NodeType;
            Int32 OriginalObjectClassId = CswConvert.ToInt32( NodeType._DataRow["objectclassid"] );
            if( OriginalObjectClassId != NewObjectClass.ObjectClassId )
            {
                if( getObjectClass( OriginalObjectClassId ).ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass )
                    throw new CswDniException( ErrorType.Warning, "Cannot convert this nodetype", "Nodetype " + NodeType.NodeTypeName + " cannot be converted because it is not Generic" );

                NodeType = CheckVersioning( NodeType );

                NodeType._DataRow["objectclassid"] = CswConvert.ToDbVal( NewObjectClass.ObjectClassId );
                NodeType.IconFileName = NewObjectClass.IconFileName;
                Collection<CswNbtMetaDataNodeTypeProp> MatchingProps = new Collection<CswNbtMetaDataNodeTypeProp>();
                // Synchronize Object Class Props from the new object class
                foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in getObjectClassProps( NodeType.ObjectClassId ) )
                {
                    bool FoundMatch = false;
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps() )
                    {
                        if( Prop.PropName == ObjectClassProp.PropName && Prop.FieldTypeId == ObjectClassProp.FieldTypeId )
                        {
                            MatchingProps.Add( Prop );
                        }
                    }

                    foreach( CswNbtMetaDataNodeTypeProp Match in MatchingProps )
                    {
                        Match._DataRow["objectclasspropid"] = ObjectClassProp.PropId;
                        FoundMatch = true;

                        _CswNbtMetaDataResources.NodeTypePropsCollection.clearCache();
                    }

                    if( !FoundMatch )
                    {
                        // Because we handle versioning above, we don't have to worry about it here
                        CswNbtMetaDataNodeTypeProp NewNodeTypeProp = makeNewProp( NodeType, null, ObjectClassProp.FieldTypeId, ObjectClassProp.PropName, Int32.MinValue, false, ObjectClassProp );

                    }
                } // foreach (CswNbtMetaDataObjectClassProp ObjectClassProp in this.ObjectClass.ObjectClassProps)

            } // if (_NodeTypeRow["objectclassid"] != NewObjectClass.ObjectClassId)

            return ret;

        } // ConvertObjectClass

        public CswNbtMetaDataNodeType MakeNewVersion( CswNbtMetaDataNodeType NodeType )
        {
            return CopyNodeType( NodeType, NodeType.NodeTypeName, true );
        }
        public CswNbtMetaDataNodeType CopyNodeType( CswNbtMetaDataNodeType NodeType )
        {
            return CopyNodeType( NodeType, string.Empty, false );
        }
        public CswNbtMetaDataNodeType CopyNodeType( CswNbtMetaDataNodeType NodeType, string NewNodeTypeName )
        {
            return CopyNodeType( NodeType, NewNodeTypeName, false );
        }
        private CswNbtMetaDataNodeType CopyNodeType( CswNbtMetaDataNodeType NodeType, string NewNodeTypeName, bool IsVersioning )
        {
            Int32 OriginalNodeTypeId = NodeType.NodeTypeId;

            if( NewNodeTypeName == String.Empty )
                NewNodeTypeName = "Copy Of " + NodeType.NodeTypeName;

            if( !IsVersioning && getNodeType( NewNodeTypeName ) != null )
                throw new CswDniException( ErrorType.Warning, "Copy failed: Name is already in use", "User tried to copy nodetype: " + NodeType.NodeTypeName + " to name: " + NewNodeTypeName + ", which is already in use" );

            // Copy nodetype info
            DataTable NewNodeTypeTable = _CswNbtMetaDataResources.NodeTypeTableUpdate.getEmptyTable();
            DataRow InsertedNodeTypeRow = NewNodeTypeTable.NewRow();
            InsertedNodeTypeRow["objectclassid"] = CswConvert.ToDbVal( NodeType.ObjectClassId );
            InsertedNodeTypeRow["iconfilename"] = NodeType.IconFileName;
            InsertedNodeTypeRow["nodetypename"] = NewNodeTypeName;
            InsertedNodeTypeRow["category"] = NodeType.Category;
            InsertedNodeTypeRow["islocked"] = CswConvert.ToDbVal( false );
            NewNodeTypeTable.Rows.Add( InsertedNodeTypeRow );
            Int32 NewNodeTypeId = CswConvert.ToInt32( InsertedNodeTypeRow["nodetypeid"].ToString() );
            if( IsVersioning )
            {
                // new version of this nodetype
                InsertedNodeTypeRow["versionno"] = ( NodeType.VersionNo + 1 ).ToString();
                InsertedNodeTypeRow["priorversionid"] = CswConvert.ToDbVal( NodeType.NodeTypeId );
                InsertedNodeTypeRow["firstversionid"] = CswConvert.ToDbVal( NodeType.FirstVersionNodeTypeId );
            }
            else
            {
                // first version of new nodetype
                InsertedNodeTypeRow["versionno"] = "1";
                InsertedNodeTypeRow["firstversionid"] = CswConvert.ToDbVal( NewNodeTypeId );
            }
            _CswNbtMetaDataResources.NodeTypeTableUpdate.update( NewNodeTypeTable );

            _CswNbtMetaDataResources.refreshAll();

            CswNbtMetaDataNodeType NewNodeType = null;
            CswNbtMetaDataNodeType OldNodeType = null;
            //if( IsVersioning )
            //{
            //    // This swaps the existing reference to the new reference!
            //    //_CswNbtMetaDataResources.NodeTypesCollection.RegisterNew( InsertedNodeTypeRow, OriginalNodeTypeId );
            //    // "NodeType" is now the new nodetype copy
            //    NewNodeType = NodeType;
            //    // get the old one again
            //    OldNodeType = getNodeType( OriginalNodeTypeId );
            //}
            //else
            //{
                // No swapping
                //_CswNbtMetaDataResources.NodeTypesCollection.RegisterNew( InsertedNodeTypeRow );
                // "NodeType" is still the original nodetype
                OldNodeType = NodeType;
                // get the new one
                NewNodeType = getNodeType( NewNodeTypeId );
            //}

            // Copy tabs
            Hashtable TabMap = new Hashtable();
            DataTable NewTabsTable = _CswNbtMetaDataResources.NodeTypeTabTableUpdate.getEmptyTable();
            Collection<CswNbtMetaDataNodeTypeTab> TabsToCopy = new Collection<CswNbtMetaDataNodeTypeTab>();
            foreach( CswNbtMetaDataNodeTypeTab NodeTypeTab in OldNodeType.getNodeTypeTabs() )
                TabsToCopy.Add( NodeTypeTab );
            foreach( CswNbtMetaDataNodeTypeTab NodeTypeTab in TabsToCopy )
            {
                DataRow NewTabRow = NewTabsTable.NewRow();
                NodeTypeTab.CopyTabToNewNodeTypeTabRow( NewTabRow );
                NewTabRow["nodetypeid"] = NewNodeTypeId.ToString();
                NewTabsTable.Rows.Add( NewTabRow );
                Int32 NewTabId = CswConvert.ToInt32( NewTabRow["nodetypetabsetid"].ToString() );
                if( IsVersioning )
                {
                    // new version of this nodetypetab
                    NewTabRow["priortabversionid"] = CswConvert.ToDbVal( NodeTypeTab.TabId );
                    NewTabRow["firsttabversionid"] = CswConvert.ToDbVal( NodeTypeTab.FirstTabVersionId );
                }
                else
                {
                    // first version of new nodetypetab
                    NewTabRow["priortabversionid"] = CswConvert.ToDbVal( Int32.MinValue );
                    NewTabRow["firsttabversionid"] = CswConvert.ToDbVal( NewTabId );
                }
                _CswNbtMetaDataResources.NodeTypeTabTableUpdate.update( NewTabsTable );
                TabMap.Add( NodeTypeTab.TabId, NewTabId );

                //_CswNbtMetaDataResources.NodeTypeTabsCollection.RegisterNew( NewTabRow, NodeTypeTab.TabId );
            }

            // Copy props 
            DataTable NewPropsTable = _CswNbtMetaDataResources.NodeTypePropTableUpdate.getEmptyTable();
            Collection<CswNbtMetaDataNodeTypeProp> PropsToCopy = new Collection<CswNbtMetaDataNodeTypeProp>();
            foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in OldNodeType.getNodeTypeProps() )
                PropsToCopy.Add( NodeTypeProp );
            foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in PropsToCopy )
            {
                DataRow NewPropRow = NewPropsTable.NewRow();
                NewPropRow["nodetypeid"] = CswConvert.ToDbVal( NewNodeTypeId );
                //NewPropRow["nodetypetabsetid"] = CswConvert.ToDbVal( CswConvert.ToInt32( TabMap[NodeTypeProp.EditLayout.Tab.TabId] ) );
                NewPropsTable.Rows.Add( NewPropRow );
                Int32 NewPropId = CswConvert.ToInt32( NewPropRow["nodetypepropid"].ToString() );
                if( IsVersioning )
                {
                    NewPropRow["priorpropversionid"] = CswConvert.ToDbVal( NodeTypeProp.PropId );
                    NewPropRow["firstpropversionid"] = CswConvert.ToDbVal( NodeTypeProp.FirstPropVersionId );
                }
                else
                {
                    NewPropRow["priorpropversionid"] = CswConvert.ToDbVal( Int32.MinValue );
                    NewPropRow["firstpropversionid"] = CswConvert.ToDbVal( NewPropId );
                }
                _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( NewPropsTable );

                // BZ 10242 forces this to happen after the row is inserted, so we'll have to update it twice
                NodeTypeProp.CopyPropToNewNodeTypePropRow( NewPropRow );
                _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( NewPropsTable );

                // Fix layout
                foreach( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType in Enum.GetValues( typeof( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType ) ) )
                {
                    CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout OriginalLayout = NodeTypeLayout.getLayout( LayoutType, NodeTypeProp.PropId );
                    if( OriginalLayout != null )
                    {
                        Int32 NewTabId = Int32.MinValue;
                        if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
                        {
                            NewTabId = CswConvert.ToInt32( TabMap[OriginalLayout.TabId] );
                        }
                        NodeTypeLayout.updatePropLayout( LayoutType, NewNodeType.NodeTypeId, NewPropId, NewTabId, OriginalLayout.DisplayRow, OriginalLayout.DisplayColumn );
                    }
                }

                //_CswNbtMetaDataResources.NodeTypePropsCollection.RegisterNew( NewPropRow, NodeTypeProp.PropId );
            }

            // Fix Conditional Props (case 22328)
            Collection<CswNbtMetaDataNodeTypeProp> NewProps = new Collection<CswNbtMetaDataNodeTypeProp>();
            foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NewNodeType.getNodeTypeProps() )
            {
                NewProps.Add( NodeTypeProp );
            }
            foreach( CswNbtMetaDataNodeTypeProp NewProp in NewProps )
            {
                if( NewProp.FilterNodeTypePropId != Int32.MinValue )
                {
                    CswNbtMetaDataNodeTypeProp OldFilter = OldNodeType.getNodeTypeProp( NewProp.FilterNodeTypePropId );
                    if( OldFilter != null )
                    {
                        CswNbtMetaDataNodeTypeProp NewFilter = NewNodeType.getNodeTypeProp( OldFilter.PropName );
                        NewProp.setFilter( NewFilter.PropId, NewProp.getFilterString() );
                    }
                }
            }

            // Fix the name template
            NewNodeType.setNameTemplateText( OldNodeType.getNameTemplateText() );

            if( OnCopyNodeType != null )
                OnCopyNodeType( OldNodeType, NewNodeType );

            return NewNodeType;

        }// CopyNodeType()

        protected void CopyNodeTypePropFromObjectClassProp( CswNbtMetaDataObjectClassProp ObjectClassProp, DataRow NodeTypePropRow ) //, CswNbtMetaDataNodeTypeProp NodeTypeProp)
        {
            if( CswConvert.ToInt32( NodeTypePropRow["fieldtypeid"] ) != ObjectClassProp.FieldTypeId )
                throw new CswDniException( ErrorType.Error, "Illegal property assignment", "Attempting to assign an ObjectClassProperty (" + ObjectClassProp.PropId.ToString() + ") to a NodeTypeProperty (" + NodeTypePropRow["nodetypepropid"].ToString() + ") where their fieldtypes do not match" );

            ObjectClassProp.CopyPropToNewPropRow( NodeTypePropRow );

            // Handle the object class prop's viewxml
            if( ObjectClassProp.ViewXml != string.Empty )
            {
                CswTableUpdate NodeViewsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "CopyNodeTypePropFromObjectClassProp_update", "node_views" );
                DataTable NodeViewsTable = NodeViewsUpdate.getEmptyTable();

                DataRow NewViewRow = NodeViewsTable.NewRow();
                NewViewRow["viewname"] = NodeTypePropRow["propname"].ToString();
                NewViewRow["viewxml"] = ObjectClassProp.ViewXml;
                NewViewRow["visibility"] = NbtViewVisibility.Property.ToString();
                NodeViewsTable.Rows.Add( NewViewRow );
                NodeViewsUpdate.update( NodeViewsTable );

                NodeTypePropRow["nodeviewid"] = NewViewRow["nodeviewid"];
            }
        }

        // Handle the object class prop's default value
        protected void CopyNodeTypePropDefaultValueFromObjectClassProp( CswNbtMetaDataObjectClassProp ObjectClassProp, CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            if( ObjectClassProp.HasDefaultValue() )
            {
                NodeTypeProp.DefaultValue.copy( ObjectClassProp.DefaultValue );
            }
        }

        //uses the oracle-specific CreateNTview() and CreateOBJview() procedures
        protected void RefreshNodetypeView(  int nodetypeid)
        {
            //ALWAYS do nodetype views first, then objectclass views second
            //nodetype
            List<CswStoredProcParam> myParams = new List<CswStoredProcParam> ();
            myParams.Add(new CswStoredProcParam("ntid",nodetypeid,DataDictionaryPortableDataType.Long));
            _CswNbtMetaDataResources.CswNbtResources.execStoredProc("CreateNTview",myParams);
        }

        protected void RefreshAllNodetypeViews( )
        {
            //ALWAYS do nodetype views first, then objectclass views second
            //nodetype
            List<CswStoredProcParam> myParams = new List<CswStoredProcParam> ();
            _CswNbtMetaDataResources.CswNbtResources.execStoredProc("CreateNTview",myParams);
        }


        #endregion Mutators


        #region Delete

        /// <summary>
        /// Deletes a nodetype from the database and meta data collection
        /// </summary>
        /// <param name="NodeType">Node Type to delete</param>
        public void DeleteNodeType( CswNbtMetaDataNodeType NodeType )
        {
            // If the nodetype is a prior version, prevent delete
            if( !NodeType.IsLatestVersion() )
            {
                throw new CswDniException( ErrorType.Warning, "This NodeType cannot be deleted", "User attempted to delete a nodetype that was not the latest version" );
            }

            // Delete Props
            Collection<CswNbtMetaDataNodeTypeProp> PropsToDelete = new Collection<CswNbtMetaDataNodeTypeProp>();
            foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps() )
                PropsToDelete.Add( Prop );
            foreach( CswNbtMetaDataNodeTypeProp Prop in PropsToDelete )
                DeleteNodeTypeProp( Prop, true );

            // Delete Tabs
            Collection<CswNbtMetaDataNodeTypeTab> TabsToDelete = new Collection<CswNbtMetaDataNodeTypeTab>();
            foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
                TabsToDelete.Add( Tab );
            foreach( CswNbtMetaDataNodeTypeTab Tab in TabsToDelete )
                DeleteNodeTypeTab( Tab, false );

            // Delete Nodes
            CswTableUpdate NodesUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "deletenodetype_NodesUpdate", "nodes" );
            DataTable NodesTable = NodesUpdate.getTable( "nodetypeid", NodeType.NodeTypeId );
            foreach( DataRow CurrentRow in NodesTable.Rows )
            {
                CurrentRow.Delete();
            }
            NodesUpdate.update( NodesTable );

            // Delete Views
            CswTableUpdate ViewsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "DeleteNodeType_viewupdate", "node_views" );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
            SelectCols.Add( "nodeviewid" );
            DataTable ViewsTable = ViewsUpdate.getTable( SelectCols );
            foreach( DataRow CurrentRow in ViewsTable.Rows )
            {
                //CswNbtView CurrentView = new CswNbtView(_CswNbtResources);
                //CurrentView.LoadXml(CswConvert.ToInt32(CurrentRow["nodeviewid"].ToString()));
                CswNbtView CurrentView = _CswNbtMetaDataResources.CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) ) );
                if( CurrentView.ContainsNodeType( NodeType ) )
                    CurrentView.Delete();
            }
            ViewsUpdate.update( ViewsTable );

            // Update MetaData
            _CswNbtMetaDataResources.NodeTypesCollection.clearCache();

            // Delete the NodeType
            NodeType._DataRow.Delete();
            _CswNbtMetaDataResources.NodeTypeTableUpdate.update( NodeType._DataRow.Table );

            refreshAll();
            _ResetAllViews = true;

        }//DeleteNodeType()


        /// <summary>
        /// Delete all versions of a nodetype
        /// </summary>
        public void DeleteNodeTypeAllVersions( CswNbtMetaDataNodeType NodeType )
        {
            List<CswNbtMetaDataNodeType> AllVersions = new List<CswNbtMetaDataNodeType>();
            CswNbtMetaDataNodeType CurrentNodeType = NodeType.getNodeTypeLatestVersion();
            do
            {
                AllVersions.Add( CurrentNodeType );
                CurrentNodeType = CurrentNodeType.getPriorVersionNodeType();
            } while( null != CurrentNodeType );

            foreach( CswNbtMetaDataNodeType CurrentVersion in AllVersions )
            {
                DeleteNodeType( CurrentVersion );
            }
        }//DeleteNodeTypeAllVersions()



        /// <summary>
        /// Deletes a property from the database and metadata collection
        /// </summary>
        /// <param name="NodeTypeProp">Prop to Delete</param>
        /// <returns>Returns the NodeTypeTab of this property (or the new version of the tab, if versioning was necessary)</returns>
        public CswNbtMetaDataNodeTypeTab DeleteNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            return DeleteNodeTypeProp( NodeTypeProp, false );
        }

        /// <summary>
        /// Deletes a property from the database and metadata collection
        /// </summary>
        /// <param name="NodeTypeProp">Prop to delete</param>
        /// <param name="Internal">If true, allow deleting object class props, and don't version or recalculate question numbers</param>
        /// <returns>Tab of deleted property (for UI to select)</returns>
        protected CswNbtMetaDataNodeTypeTab DeleteNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp, bool Internal )
        {
            CswNbtMetaDataNodeTypeTab ret = getNodeTypeTab( NodeTypeProp.EditLayout.TabId );
            if( !Internal )
            {
                if( !NodeTypeProp.IsDeletable() )
                    throw new CswDniException( ErrorType.Warning, "Cannot delete property", "Property is not allowed to be deleted: PropId = " + NodeTypeProp.PropId );

                //string OriginalPropName = NodeTypeProp.PropName;
                CswNbtMetaDataNodeType NodeType = CheckVersioning( NodeTypeProp.getNodeType() );
                NodeTypeProp = getNodeTypePropVersion( NodeType.NodeTypeId, NodeTypeProp.PropId );
            }

            // Delete Jct_Nodes_Props records
            CswTableUpdate JctNodesPropsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "DeleteNodeTypeProp_jct_update", "jct_nodes_props" );
            DataTable JctNodesPropsTable = JctNodesPropsUpdate.getTable( "nodetypepropid", NodeTypeProp.PropId );
            foreach( DataRow CurrentJctNodesPropsRow in JctNodesPropsTable.Rows )
            {
                CurrentJctNodesPropsRow.Delete();
            }
            JctNodesPropsUpdate.update( JctNodesPropsTable );

            // Delete nodetype_layout records
            foreach( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType in Enum.GetValues( typeof( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType ) ) )
            {
                NodeTypeLayout.removePropFromLayout( LayoutType, NodeTypeProp );
            }

            // Delete Views
            // This has to come after because nodetype_props has an fk to node_views.
            CswTableUpdate ViewsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "DeleteNodeTypeProp_nodeview_update", "node_views" );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
            SelectCols.Add( "nodeviewid" );
            SelectCols.Add( "viewxml" );
            DataTable ViewsTable = ViewsUpdate.getTable( SelectCols );
            foreach( DataRow CurrentRow in ViewsTable.Rows )
            {
                if( CurrentRow.RowState != DataRowState.Deleted )
                {
                    CswNbtView CurrentView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
                    CurrentView.LoadXml( CurrentRow["viewxml"].ToString() );
                    CurrentView.ViewId = new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) );

                    if( CurrentView.ContainsNodeTypeProp( NodeTypeProp ) || CurrentView.ViewId == NodeTypeProp.ViewId )
                        CurrentView.Delete();
                }
            }
            ViewsUpdate.update( ViewsTable );

            // BZ 8745
            // Update nodename template
            CswNbtMetaDataNodeType UpdateNodeType = NodeTypeProp.getNodeType();

            string NodeTypeTemp = UpdateNodeType.NameTemplateValue;
            NodeTypeTemp = NodeTypeTemp.Replace( " " + MakeTemplateEntry( NodeTypeProp.PropId.ToString() ), "" );
            NodeTypeTemp = NodeTypeTemp.Replace( MakeTemplateEntry( NodeTypeProp.PropId.ToString() ), "" );
            UpdateNodeType.NameTemplateValue = NodeTypeTemp;

            if( OnDeleteNodeTypeProp != null )
                OnDeleteNodeTypeProp( NodeTypeProp );

            // Update MetaData
            _CswNbtMetaDataResources.NodeTypePropsCollection.clearCache();

            // Delete NodeType Prop record
            NodeTypeProp._DataRow.Delete();
            _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( NodeTypeProp._DataRow.Table );

            if( !Internal )
                _CswNbtMetaDataResources.RecalculateQuestionNumbers( ret.getNodeType() );

            //refresh the views
            _RefreshViewForNodetypeId.Add( UpdateNodeType.NodeTypeId );

            refreshAll();
            return ret;
        } // DeleteNodeTypeProp()

        /// <summary>
        /// Deletes a tab from the database and metadata collection
        /// </summary>
        /// <param name="NodeTypeTab">Tab to Delete</param>
        /// <returns>Returns the NodeType of this tab (or the new version of the tab, if versioning was necessary)</returns>
        public CswNbtMetaDataNodeType DeleteNodeTypeTab( CswNbtMetaDataNodeTypeTab NodeTypeTab )
        {
            return DeleteNodeTypeTab( NodeTypeTab, true );
        }

        private CswNbtMetaDataNodeType DeleteNodeTypeTab( CswNbtMetaDataNodeTypeTab NodeTypeTab, bool CauseVersioning )
        {
            CswNbtMetaDataNodeType ret = NodeTypeTab.getNodeType();
            if( CauseVersioning )
            {
                string OriginalTabName = NodeTypeTab.TabName;
                CswNbtMetaDataNodeType NodeType = CheckVersioning( NodeTypeTab.getNodeType() );
                NodeTypeTab = NodeType.getNodeTypeTab( OriginalTabName );
            }

            // This breaks deleting nodetypes.  Instead, the menu doesn't display the delete button.
            //if (this.NodeType.NodeTypeTabs.Count <= 1)
            //    throw new CswDniException("You cannot delete the last tab of a nodetype", "User attempted to delete the only tab on a nodetype");

            // Move the properties to another tab
            CswNbtMetaDataNodeTypeTab NewTab = NodeTypeTab.getNodeType().getFirstNodeTypeTab();
            if( NewTab == NodeTypeTab )  // BZ 8353
                NewTab = NodeTypeTab.getNodeType().getSecondNodeTypeTab();

            Collection<CswNbtMetaDataNodeTypeProp> PropsToReassign = new Collection<CswNbtMetaDataNodeTypeProp>();
            foreach( CswNbtMetaDataNodeTypeProp Prop in NodeTypeTab.NodeTypeProps )
                PropsToReassign.Add( Prop );

            foreach( CswNbtMetaDataNodeTypeProp Prop in PropsToReassign )
            {
                Prop.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, NewTab.TabId, Int32.MinValue, Int32.MinValue );
                // BZ 8353 - To avoid constraint errors, post this change immediately
                _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( Prop._DataRow.Table );
            }


            // Update MetaData
            refreshAll();
            //_CswNbtMetaDataResources.NodeTypeTabsCollection.clearCache();

            // Delete NodeType Tab record
            NodeTypeTab._DataRow.Delete();
            _CswNbtMetaDataResources.NodeTypeTabTableUpdate.update( NodeTypeTab._DataRow.Table );

            return ret;
        } // DeleteNodeTypeTab

        #endregion Delete




        /// <summary>
        /// Saves all changes to all NodeTypes, NodeType Tabs, and NodeType Properties to the database
        /// Note that changes to object classes, object class properties, and field types will not be saved.
        /// </summary>
        public void finalize()
        {
            _CswNbtMetaDataResources.finalize();
        }

        public void afterFinalize()
        {
            if( _ResetAllViews )
            {
                RefreshAllNodetypeViews();
                _ResetAllViews = false;
            }
            else
            {
                foreach( Int32 ntid in _RefreshViewForNodetypeId )
                {
                    RefreshNodetypeView( ntid );
                }
                _RefreshViewForNodetypeId.Clear();
            }
        }


        #region Templates (Node name, Composite)

        /// <summary>
        /// Left delimiter of template values
        /// </summary>
        public static char _TemplateLeftDelimiter = '{';
        /// <summary>
        /// Right delimiter of template values
        /// </summary>
        public static char _TemplateRightDelimiter = '}';

        /// <summary>
        /// Makes an entry for a template, using the delimiters
        /// </summary>
        public static string MakeTemplateEntry( string TemplateValue )
        {
            return _TemplateLeftDelimiter.ToString() + TemplateValue + _TemplateRightDelimiter.ToString();
        }

        /// <summary>
        /// Change a Template Value (with Property IDs) to a Template Text (with Property Names)
        /// </summary>
        public static string TemplateValueToTemplateText( IEnumerable<CswNbtMetaDataNodeTypeProp> PropsCollection, string Template )
        {
            string Text = Template;
            foreach( CswNbtMetaDataNodeTypeProp Prop in PropsCollection )
            {
                Text = Text.Replace( MakeTemplateEntry( Prop.PropId.ToString() ), MakeTemplateEntry( Prop.PropName ) );
            }
            return Text;
        }

        /// <summary>
        /// Change a Template Text (with Property Names) to a Template Value (with Property IDs)
        /// </summary>
        public static string TemplateTextToTemplateValue( IEnumerable<CswNbtMetaDataNodeTypeProp> PropsCollection, string Text )
        {
            string Template = Text;
            foreach( CswNbtMetaDataNodeTypeProp Prop in PropsCollection )
            {
                Template = Template.Replace( MakeTemplateEntry( Prop.PropName ), MakeTemplateEntry( Prop.PropId.ToString() ) );
            }
            return Template;
        }

        /// <summary>
        /// Change a Template Value (with Property IDs) to an instance value (using the values of those properties)
        /// </summary>
        public static string TemplateValueToDisplayValue( IEnumerable<CswNbtMetaDataNodeTypeProp> PropsCollection, string Template, CswNbtNodePropData PropData )
        {
            string Value = Template;
            foreach( CswNbtMetaDataNodeTypeProp Prop in PropsCollection )
            {
                if( Value.Contains( MakeTemplateEntry( Prop.PropId.ToString() ) ) )
                {
                    Value = Value.Replace( MakeTemplateEntry( Prop.PropId.ToString() ), PropData.OtherPropGestalt( Prop.PropId ) );
                }
            }
            return Value;
        }

        #endregion Templates (Node name, Composite)

    }
}
