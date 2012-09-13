using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataNodeTypeProp : ICswNbtMetaDataObject, ICswNbtMetaDataProp, IEquatable<CswNbtMetaDataNodeTypeProp>, IComparable
    {
        public enum NodeTypePropAttributes
        {
            unknown,
            append,
            auditlevel,
            datetoday,
            //display_col,
            //display_row,
            fieldtypeid,
            //fkvalue, 
            isbatchentry,
            //isfk, 
            isrequired,
            isunique,
            iscompoundunique,
            length,
            //nodetypeid, 
            //nodetypepropid, 
            //nodetypetabsetid, 
            //objectclasspropid, 
            servermanaged,
            textareacols,
            textarearows,
            textlength,
            url,
            valuepropid,
            width,
            sequenceid,
            numberprecision,
            listoptions,
            compositetemplate,
            //fktype, 
            valueproptype,
            statictext,
            multi,
            nodeviewid,
            readOnly,
            //display_col_add,
            //display_row_add,
            //setvalonadd,
            numberminvalue,
            numbermaxvalue,
            usenumbering,
            questionno,
            subquestionno,
            filter,
            filterpropid,
            //firstpropversionid, 
            //priorpropversionid, 
            valueoptions,
            //defaultvalue, 
            helptext,
            propname,
            //defaultvalueid, 
            isquicksearch,
            extended,
            hideinmobile,
            mobilesearch,
            attribute1,
            attribute2,
            attribute3,
            attribute4,
            attribute5
        }

        public static NodeTypePropAttributes getNodeTypePropAttributesFromString( string AttributeName )
        {
            NodeTypePropAttributes ReturnVal = NodeTypePropAttributes.unknown;
            AttributeName = AttributeName.Replace( "_", "" );
            if( Enum.IsDefined( typeof( NodeTypePropAttributes ), AttributeName ) )
            {
                ReturnVal = (NodeTypePropAttributes) Enum.Parse( typeof( NodeTypePropAttributes ), AttributeName, true );
            }
            return ( ReturnVal );
        }

        public static String getNodeTypePropAttributesAsString( NodeTypePropAttributes Attribute )
        {
            String ReturnVal = String.Empty;
            if( Attribute != NodeTypePropAttributes.unknown )
                ReturnVal = Attribute.ToString().Replace( "_", "" );
            return ( ReturnVal );
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _NodeTypePropRow;
        public CswNbtMetaDataNodeTypeProp( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            Reassign( Row );
        }

        #region Layout

        /// <summary>
        /// Returns the layout for this property.  If edit, be sure to supply a valid TabId
        /// </summary>
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType, Int32 TabId = Int32.MinValue )
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.getLayout( LayoutType, this.PropId, TabId );
        }

        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getEditLayout( Int32 TabId )
        {
            return getLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, TabId );
        }
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getAddLayout()
        {
            return getLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
        }
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getTableLayout()
        {
            return getLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table );
        }
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getPreviewLayout()
        {
            return getLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview );
        }
        public Dictionary<Int32, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout> getEditLayouts()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.getLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, this );
        }

        public void updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType, bool DoMove, Int32 TabId = Int32.MinValue, Int32 DisplayRow = Int32.MinValue, Int32 DisplayColumn = Int32.MinValue, string TabGroup = "" )
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.updatePropLayout( LayoutType, NodeTypeId, PropId, DoMove, TabId, DisplayRow, DisplayColumn, TabGroup );
        }
        public void updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType, CswNbtMetaDataNodeTypeProp InsertAfterProp, bool DoMove )
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.updatePropLayout( LayoutType, this, InsertAfterProp, DoMove );
        }
        public void removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType, Int32 TabId = Int32.MinValue )
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( LayoutType, this, TabId );
        }
        public void removeFromAllLayouts( Int32 TabId = Int32.MinValue )
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, this, TabId );
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, this, TabId );
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, this, TabId );
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, this, TabId );
        }


        public void clearCachedLayouts()
        {
            _FirstEditLayout = null;
            _AddLayout = null;
        }

        private CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout _FirstEditLayout = null;
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout FirstEditLayout
        {
            get
            {
                if( _FirstEditLayout == null )
                {
                    Dictionary<Int32, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout> EditLayouts = getEditLayouts();
                    if( EditLayouts.Values.Count > 0 )
                    {
                        _FirstEditLayout = EditLayouts.Values.First();
                    }
                }
                return _FirstEditLayout;
            }
        } // FirstEditLayout
        private CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout _AddLayout = null;
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout AddLayout
        {
            get
            {
                if( _AddLayout == null )
                {
                    _AddLayout = getAddLayout();
                }
                return _AddLayout;
            }
        } // AddLayout

        #endregion Layout

        public DataRow _DataRow
        {
            get { return _NodeTypePropRow; }
            //set { _NodeTypePropRow = value; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
            //set { _UniqueId = value; }
        }

        public string UniqueIdFieldName { get { return "nodetypepropid"; } }

        public void Reassign( DataRow NewRow )
        {
            _NodeTypePropRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }

        private void _checkVersioningProp()
        {
            CswNbtMetaDataNodeType NewNodeType = _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this.getNodeType() );
            if( NewNodeType.NodeTypeId != NodeTypeId )
            {
                // Get the new property and reassign myself
                CswNbtMetaDataNodeTypeProp NewProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypePropVersion( NewNodeType.NodeTypeId, this.PropId );
                this._NodeTypePropRow = NewProp._DataRow;
            }
        }

        /// <summary>
        /// This function sets the value of the field to the provided value, checking for nodetype versioning 
        /// and re-registering the property with the collection.
        /// We only want to check for versioning if the value is indeed changing.
        /// </summary>
        /// <returns>True if the value was changed</returns>
        private bool _setAttribute( string FieldName, object value, bool ReRegister )
        {
            bool ret = false;
            if( value is double )
            {
                if( CswConvert.ToDouble( _NodeTypePropRow[FieldName] ) != (double) value )
                {
                    _checkVersioningProp();
                    _NodeTypePropRow[FieldName] = CswConvert.ToDbVal( (double) value );
                    ret = true;
                    if( ReRegister )
                    {
                        _CswNbtMetaDataResources.NodeTypePropsCollection.clearCache();
                    }
                }
            }
            else if( value is Int32 )
            {
                if( CswConvert.ToInt32( _NodeTypePropRow[FieldName] ) != (Int32) value )
                {
                    _checkVersioningProp();
                    _NodeTypePropRow[FieldName] = CswConvert.ToDbVal( (Int32) value );
                    ret = true;
                    if( ReRegister )
                    {
                        _CswNbtMetaDataResources.NodeTypePropsCollection.clearCache();
                    }
                }
            }
            else if( value is bool )
            {
                if( CswConvert.ToBoolean( _NodeTypePropRow[FieldName] ) != (bool) value )
                {
                    _checkVersioningProp();
                    _NodeTypePropRow[FieldName] = CswConvert.ToDbVal( (bool) value );
                    ret = true;
                    if( ReRegister )
                    {
                        _CswNbtMetaDataResources.NodeTypePropsCollection.clearCache();
                    }
                }
            }
            else if( value is string )
            {
                if( _NodeTypePropRow[FieldName].ToString() != (string) value )
                {
                    _checkVersioningProp();
                    _NodeTypePropRow[FieldName] = (string) value;
                    ret = true;
                    if( ReRegister )
                    {
                        _CswNbtMetaDataResources.NodeTypePropsCollection.clearCache();
                    }
                }
            }
            else if( value is DBNull )
            {
                if( _NodeTypePropRow[FieldName].ToString() != string.Empty )
                {
                    _checkVersioningProp();
                    _NodeTypePropRow[FieldName] = value;
                    ret = true;
                    if( ReRegister )
                    {
                        _CswNbtMetaDataResources.NodeTypePropsCollection.clearCache();
                    }
                }
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Unrecognized attribute type", "CswNbtMetaDataNodeTypeProp._setAttribute encountered an unrecognized attribute type" );
            }
            return ret;
        } // _setAttribute()

        private ICswNbtFieldTypeRule _FieldTypeRule = null;
        public ICswNbtFieldTypeRule getFieldTypeRule()
        {
            if( _FieldTypeRule == null )
                _FieldTypeRule = _CswNbtMetaDataResources.makeFieldTypeRule( this.getFieldType().FieldType );
            return _FieldTypeRule;
        }

        public Int32 PropId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["nodetypepropid"].ToString() ); }
        }
        public string PropName
        {
            get
            {
                return _NodeTypePropRow["propname"].ToString();
            }
            set
            {
                // BZ 5492: Make sure this is unique for this nodetype

                if( _NodeTypePropRow["propname"].ToString() != value &&
                    _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypePropId( this.NodeTypeId, value ) != Int32.MinValue )
                {
                    throw new CswDniException( ErrorType.Warning, "Property Name must be unique per nodetype", "Attempted to save a propname which is equal to a propname of another property in this nodetype" );
                }
                _setAttribute( "propname", value, true );

                if( _CswNbtMetaDataResources.CswNbtMetaData.OnEditNodeTypePropName != null )
                    _CswNbtMetaDataResources.CswNbtMetaData.OnEditNodeTypePropName( this );
            }
        } // PropName

        public string PropNameWithQuestionNo
        {
            get
            {
                string name = string.Empty;
                if( UseNumbering && QuestionNo != Int32.MinValue )
                {
                    name += FullQuestionNo;
                    name += " ";
                }
                name += PropName.ToString();
                return name;
            }
        }

        //public delegate void EditPropEventHandler( CswNbtMetaDataNodeTypeProp EditedProp );
        //public event EditPropEventHandler OnEditNodeTypePropName = null;
        //public event EditPropEventHandler OnBeforeEditNodeTypePropOrder = null;
        //public event EditPropEventHandler OnAfterEditNodeTypePropOrder = null;

        /// <summary>
        /// Set the ViewId for Relationships.  Editing this does not cause versioning.
        /// </summary>
        public CswNbtViewId ViewId
        {
            get
            {
                CswNbtViewId ret = new CswNbtViewId( CswConvert.ToInt32( _NodeTypePropRow["nodeviewid"] ) );
                if( !ret.isSet() )
                {
                    // This prop is missing a view, so make one
                    CswNbtView ThisView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
                    ThisView.makeNew( this.PropName, NbtViewVisibility.Property, null, null, Int32.MinValue );
                    if( this.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Grid )
                    {
                        // BZ 9203 - View starts with this property's nodetype at root
                        ThisView.AddViewRelationship( this.getNodeType(), true );
                        ThisView.SetViewMode( NbtViewRenderingMode.Grid );
                    }
                    ThisView.save();
                    _NodeTypePropRow["nodeviewid"] = CswConvert.ToDbVal( ThisView.ViewId.get() );
                    ret = ThisView.ViewId;
                }
                return ret;
            }
            set { _NodeTypePropRow["nodeviewid"] = CswConvert.ToDbVal( value.get() ); }
        }
        public bool IsRequired
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["isrequired"] ); }
            set
            {
                _setAttribute( "isrequired", value, false );

                if( value )
                {
                    //clearFilter();
                    _NodeTypePropRow["filter"] = string.Empty;
                    _NodeTypePropRow["filterpropid"] = CswConvert.ToDbVal( Int32.MinValue );
                    //SetValueOnAdd = true;
                    if( this.DefaultValue.Empty )
                    {
                        //_NodeTypePropRow["setvalonadd"] = CswConvert.ToDbVal( true );
                        updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, Int32.MinValue, Int32.MinValue );
                    }
                }
            }
        }
        public bool IsRequiredEnabled()
        {
            return ( false == hasFilter() );
        }


        //bz # 6686
        public bool IsUnique()
        {
            return ( CswConvert.ToBoolean( _NodeTypePropRow["isunique"] ) || IsGlobalUnique() );
        }

        public void setIsUnique( bool value )
        {
            _setAttribute( "isunique", value, false );
        }

        //case 24979
        public bool IsCompoundUnique()
        {
            return ( CswConvert.ToBoolean( _NodeTypePropRow["iscompoundunique"] ) );
        }

        public void setIsCompoundUnique( bool value )
        {
            _setAttribute( "iscompoundunique", value, false );
        }


        // BZ 9754
        public bool IsGlobalUnique()
        {
            bool ret = false;
            CswNbtMetaDataObjectClassProp OCP = this.getObjectClassProp();
            if( OCP != null )
            {
                ret = OCP.IsGlobalUnique();
            }
            return ret;
        }

        /// <summary>
        /// Returns whether any property values have been saved on nodetype prop
        /// </summary>
        public bool InUse
        {
            get
            {
                CswTableSelect JctNodesPropsTableSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswTableSelect( "InUse_select", "jct_nodes_props" );
                return ( JctNodesPropsTableSelect.getRecordCount( "where nodetypepropid = " + this.PropId.ToString() + " and nodeid is not null" ) > 0 );
            }
        }

        public PropertySelectMode Multi
        {
            get
            {
                PropertySelectMode ret = PropertySelectMode.Blank;
                switch( _NodeTypePropRow["multi"].ToString() )
                {
                    case "":
                        ret = PropertySelectMode.Blank;
                        break;
                    case "0":
                        ret = PropertySelectMode.Single;
                        break;
                    case "1":
                        ret = PropertySelectMode.Multiple;
                        break;
                }
                return ret;
            }
            set
            {
                switch( value )
                {
                    case PropertySelectMode.Blank:
                        _setAttribute( "multi", DBNull.Value, false );
                        break;
                    case PropertySelectMode.Single:
                        _setAttribute( "multi", "0", false );
                        break;
                    case PropertySelectMode.Multiple:
                        _setAttribute( "multi", "1", false );
                        break;
                }
            }
        }

        public bool EditProp( CswNbtNode Node, ICswNbtUser User, bool InPopUp )
        {
            //CswNbtMetaDataNodeTypeProp Prop = this;
            bool IsOnAdd = ( ( IsRequired && DefaultValue.Empty ) ||
                             Node.Properties[this].TemporarilyRequired ||
                             AddLayout != null );
            var ret = ( ( false == InPopUp || IsOnAdd ) &&
                FilterNodeTypePropId == Int32.MinValue && /* Keep these out */
                        false == Node.Properties[this].Hidden &&
                        _CswNbtMetaDataResources.CswNbtResources.Permit.canNode( CswNbtPermit.NodeTypePermission.Edit, this.getNodeType(), Node.NodeId, this ) );
            return ret;
        }

        public bool ShowProp( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType, CswNbtNode Node, ICswNbtUser User, Int32 TabId )
        {
            //CswNbtMetaDataNodeTypeProp Prop = this;
            CswNbtMetaDataNodeTypeTab Tab = null;
            if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
            {
                CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout EditLayout = getEditLayout( TabId );
                if( EditLayout != null )
                {
                    Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( EditLayout.TabId );
                }
            }
            var ret = ( false == hasFilter() && false == Node.Properties[this].Hidden &&
                        (
                           _CswNbtMetaDataResources.CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, this.getNodeType() ) ||
                           _CswNbtMetaDataResources.CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.View, this.getNodeType(), Tab ) ||
                           _CswNbtMetaDataResources.CswNbtResources.Permit.canNode( CswNbtPermit.NodeTypePermission.View, this.getNodeType(), Node.NodeId )
                        )
                      );

            //_CswNbtMetaDataResources.CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, this.getNodeType(), false, Tab, User, Node.NodeId, this ) );
            return ret;
        }

        private void _doSetFk( string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            FKType = inFKType;
            FKValue = inFKValue;
            ValuePropId = inValuePropId;
            ValuePropType = inValuePropType;
            IsFK = Int32.MinValue != FKValue;
        }

        public delegate void doSetFk( string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue );

        /// <summary>
        /// Set the FK for relationship props
        /// </summary>
        /// <param name="inFKType">Either NodeTypeId or ObjectClassId</param>
        /// <param name="inFKValue">FK Value</param>
        /// <param name="inValuePropType">Optional (for Property Reference)</param>
        /// <param name="inValuePropId">Optional  (for Property Reference)</param>
        public void SetFK( string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            doSetFk setFk = _doSetFk;
            getFieldTypeRule().setFk( this, setFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public string FKType
        {
            get { return _NodeTypePropRow["fktype"].ToString(); }
            private set { _setAttribute( "fktype", value, false ); }
        }
        public Int32 FKValue
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["fkvalue"] ); }
            private set { _setAttribute( "fkvalue", value, false ); }
        }

        public bool IsFK
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["isfk"] ); }
            private set { _setAttribute( "isfk", value, false ); }
        }

        public Int32 ValuePropId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["valuepropid"] ); }
            private set
            {
                if( CswConvert.ToInt32( _NodeTypePropRow["valuepropid"] ) != value )
                    getNodeType().SetNodesToPendingUpdate();

                _setAttribute( "valuepropid", value, false );
            }
        }

        public string ValuePropType
        {
            get { return _NodeTypePropRow["valueproptype"].ToString(); }
            private set { _setAttribute( "valueproptype", value, false ); }
        }

        public string getCompositeTemplateText()
        {
            return CswNbtMetaData.TemplateValueToTemplateText( getOtherNodeTypeProps(), CompositeTemplateValue );
        }
        public void setCompositeTemplateText( string value )
        {
            CompositeTemplateValue = CswNbtMetaData.TemplateTextToTemplateValue( getOtherNodeTypeProps(), value );
        }
        public string CompositeTemplateValue
        {
            get { return _NodeTypePropRow["compositetemplate"].ToString(); }
            set { _setAttribute( "compositetemplate", value, false ); }
        }
        public Int32 Length
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["length"] ); }
            set { _setAttribute( "length", value, false ); }
        }
        public bool DateToday
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["datetoday"] ); }
            set { _setAttribute( "datetoday", value, false ); }
        }
        public bool IsBatchEntry
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["isbatchentry"] ); }
            set { _setAttribute( "isbatchentry", value, false ); }
        }
        public bool ServerManaged
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["servermanaged"] ); }
            set { _setAttribute( "servermanaged", value, false ); }
        }
        public bool ReadOnly
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["readonly"] ); }
            set { _setAttribute( "readonly", value, false ); }
        }
        public Int32 TextAreaRows
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["textarearows"] ); }
            set { _setAttribute( "textarearows", value, false ); }
        }
        public Int32 TextAreaColumns
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["textareacols"] ); }
            set { _setAttribute( "textareacols", value, false ); }
        }
        public string ListOptions
        {
            get { return _NodeTypePropRow["listoptions"].ToString(); }
            set { _setAttribute( "listoptions", value, false ); }
        }
        public string ValueOptions
        {
            get { return _NodeTypePropRow["valueoptions"].ToString(); }
            set { _setAttribute( "valueoptions", value, false ); }
        }
        public Int32 NumberPrecision
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["numberprecision"] ); }
            set { _setAttribute( "numberprecision", value, false ); }
        }
        public double MinValue
        {
            get { return CswConvert.ToDouble( _NodeTypePropRow["numberminvalue"] ); }
            set { _setAttribute( "numberminvalue", value, false ); }
        }
        public double MaxValue
        {
            get { return CswConvert.ToDouble( _NodeTypePropRow["numbermaxvalue"] ); }
            set { _setAttribute( "numbermaxvalue", value, false ); }
        }
        public string StaticText
        {
            get { return _NodeTypePropRow["statictext"].ToString(); }
            set { _setAttribute( "statictext", value, false ); }
        }
        public string Extended
        {
            get { return _NodeTypePropRow["extended"].ToString(); }
            set { _setAttribute( "extended", value, false ); }
        }
        //public string DefaultValue
        //{
        //    get { return _NodeTypePropRow["defaultvalue"].ToString(); }
        //    set { _setAttribute( "defaultvalue", value, false ); }
        //}

        public string AuditLevel
        {
            get
            {
                return ( ChemSW.Audit.AuditLevel.Parse( _NodeTypePropRow[NodeTypePropAttributes.auditlevel.ToString()].ToString() ) );
            }
            set
            {
                _setAttribute( NodeTypePropAttributes.auditlevel.ToString(), value.ToString(), false );
            }
        }

        private CswNbtNodePropWrapper _DefaultValue = null;
        private DataRow _DefaultValueRow = null;

        private void _initDefaultValue( bool CreateMissingRow )
        {
            if( _DefaultValue == null )
            {
                if( _DefaultValueRow == null )
                {
                    if( _NodeTypePropRow.Table.Columns.Contains( "defaultvalueid" ) )
                    {
                        if( _NodeTypePropRow["defaultvalueid"] != null && CswTools.IsInteger( _NodeTypePropRow["defaultvalueid"] ) )
                        {
                            DataTable DefaultValueTable = _CswNbtMetaDataResources.JctNodesPropsTableUpdate.getTable( "jctnodepropid", CswConvert.ToInt32( _NodeTypePropRow["defaultvalueid"] ) );
                            if( DefaultValueTable.Rows.Count > 0 )
                                _DefaultValueRow = DefaultValueTable.Rows[0];
                        }
                        else if( CreateMissingRow )
                        {
                            DataTable NewDefaultValueTable = _CswNbtMetaDataResources.JctNodesPropsTableUpdate.getEmptyTable();
                            _DefaultValueRow = NewDefaultValueTable.NewRow();
                            _DefaultValueRow["nodetypepropid"] = CswConvert.ToDbVal( this.PropId );
                            NewDefaultValueTable.Rows.Add( _DefaultValueRow );
                            _NodeTypePropRow["defaultvalueid"] = _DefaultValueRow["jctnodepropid"];
                        }
                    } // if( _NodeTypePropRow.Table.Columns.Contains( "defaultvalueid" ) )
                } // if( _DefaultValueRow == null )

                if( _DefaultValueRow != null )
                {
                    _DefaultValue = CswNbtNodePropFactory.makeNodeProp( _CswNbtMetaDataResources.CswNbtResources, _DefaultValueRow, _DefaultValueRow.Table, null, this, null );
                }

            } // if( _DefaultValue == null )
        }

        public bool HasDefaultValue()
        {
            _initDefaultValue( false );
            return !( _DefaultValue == null || _DefaultValue.Empty );
        }

        public CswNbtNodePropWrapper DefaultValue
        {
            get
            {
                _initDefaultValue( true );
                return _DefaultValue;
            }
            // NO SET...interact with the CswNbtNodePropWrapper instead
        }

        public Int32 FirstPropVersionId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["firstpropversionid"] ); }
            set { _setAttribute( "firstpropversionid", value, false ); }
        }

        public Int32 PriorPropVersionId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["priorpropversionid"] ); }
            set { _setAttribute( "priorpropversionid", value, false ); }
        }

        // This should not trigger versioning
        public string HelpText
        {
            get { return _NodeTypePropRow["helptext"].ToString(); }
            set { _NodeTypePropRow["helptext"] = value; }
        }
        // This should not trigger versioning
        public bool IsQuickSearch
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["isquicksearch"] ); }
            set { _NodeTypePropRow["isquicksearch"] = CswConvert.ToDbVal( value ); }
        }

        public Int32 NodeTypeId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["nodetypeid"] ); }
        }
        public Int32 ObjectClassPropId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["objectclasspropid"] ); }
        }
        public Int32 FieldTypeId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["fieldtypeid"] ); }
        }

        public CswNbtMetaDataFieldType getFieldType()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getFieldType( FieldTypeId );
        }

        public CswNbtMetaDataNodeType getNodeType()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeType( NodeTypeId );
        }

        public CswNbtMetaDataNodeType getNodeTypeLatestVersion()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeLatestVersion( NodeTypeId );
        }

        public CswNbtMetaDataObjectClassProp getObjectClassProp()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassProp( ObjectClassPropId );
        }

        public void CopyPropToNewNodeTypePropRow( DataRow NewPropRow )
        {
            foreach( DataColumn PropColumn in NewPropRow.Table.Columns )
            {
                if( _NodeTypePropRow[PropColumn.ColumnName].ToString() != String.Empty )
                {
                    if( PropColumn.ColumnName.ToLower() == "nodeviewid" )
                    {
                        // BZ 10172
                        // We can't point to the same view.  We need to copy the view, and refer to the copy.
                        CswNbtView View = _CswNbtMetaDataResources.CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( _NodeTypePropRow[PropColumn.ColumnName] ) ) );
                        CswNbtView ViewCopy = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
                        ViewCopy.makeNew( View.ViewName, View.Visibility, View.VisibilityRoleId, View.VisibilityUserId, View );
                        ViewCopy.save();
                        NewPropRow[PropColumn.ColumnName] = CswConvert.ToDbVal( ViewCopy.ViewId.get() );
                    }
                    else if( PropColumn.ColumnName.ToLower() == "defaultvalueid" && CswTools.IsInteger( _NodeTypePropRow[PropColumn.ColumnName].ToString() ) )
                    {
                        // BZ 10242
                        // Same problem -- copy the default value record
                        CswTableUpdate JctNodesPropsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "CopyPropToNewNodeTypePropRow_JNP_Update", "jct_nodes_props" );
                        JctNodesPropsUpdate.AllowBlobColumns = true;
                        DataTable JNPTable = JctNodesPropsUpdate.getTable( "jctnodepropid", CswConvert.ToInt32( _NodeTypePropRow[PropColumn.ColumnName] ) );
                        if( JNPTable.Rows.Count > 0 )
                        {
                            DataRow ExistingJNPRow = JNPTable.Rows[0];
                            DataRow NewJNPRow = JNPTable.NewRow();
                            NewJNPRow["nodetypepropid"] = NewPropRow["nodetypepropid"];
                            foreach( DataColumn JctColumn in JNPTable.Columns )
                            {
                                if( JctColumn.ColumnName != "jctnodepropid" &&
                                    JctColumn.ColumnName != "nodetypepropid" )
                                {
                                    NewJNPRow[JctColumn] = ExistingJNPRow[JctColumn];
                                }
                            }
                            JNPTable.Rows.Add( NewJNPRow );
                            JctNodesPropsUpdate.update( JNPTable );
                            NewPropRow[PropColumn.ColumnName] = NewJNPRow["jctnodepropid"];
                        }
                    }
                    else if( PropColumn.ColumnName.ToLower() != "nodetypeid" &&
                             PropColumn.ColumnName.ToLower() != "nodetypetabsetid" &&
                             PropColumn.ColumnName.ToLower() != "nodetypepropid" &&
                             PropColumn.ColumnName.ToLower() != "priorpropversionid" &&
                             PropColumn.ColumnName.ToLower() != "firstpropversionid" )
                    {
                        NewPropRow[PropColumn.ColumnName] = _NodeTypePropRow[PropColumn.ColumnName].ToString();
                    }

                } // if( _NodeTypePropRow[PropColumn.ColumnName].ToString() != String.Empty )
            } // foreach( DataColumn PropColumn in NewPropRow.Table.Columns )
        } // CopyPropToNewNodeTypePropRow()

        //public Int32 DisplayColumn
        //{
        //    get { return CswConvert.ToInt32( _NodeTypePropRow["display_col"] ); }
        //    set
        //    {
        //        if( _setAttribute( "display_col", value, true ) )
        //            _CswNbtMetaDataResources.RecalculateQuestionNumbers( NodeType );
        //    }
        //}
        //public Int32 DisplayRow
        //{
        //    get { return CswConvert.ToInt32( _NodeTypePropRow["display_row"] ); }
        //    set
        //    {
        //        if( _setAttribute( "display_row", value, true ) )
        //            _CswNbtMetaDataResources.RecalculateQuestionNumbers( NodeType );
        //    }
        //}

        public string FullQuestionNo
        {
            get
            {
                string ret = "Q";
                CswNbtMetaDataNodeTypeTab Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( FirstEditLayout.TabId );
                if( Tab.SectionNo != Int32.MinValue )
                {
                    ret += Tab.SectionNo.ToString() + ".";
                }
                ret += QuestionNo.ToString();
                if( SubQuestionNo != Int32.MinValue )
                {
                    ret += "." + SubQuestionNo.ToString();
                }
                return ret;
            }
        }

        // This should not trigger versioning
        public Int32 QuestionNo
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["questionno"] ); }
            set { _DataRow["questionno"] = CswConvert.ToDbVal( value ); }
        }

        // This should not trigger versioning
        public Int32 SubQuestionNo
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["subquestionno"] ); }
            set { _DataRow["subquestionno"] = CswConvert.ToDbVal( value ); }
        }
        // This should not trigger versioning
        public bool UseNumbering
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["usenumbering"] ); }
            set
            {
                _DataRow["usenumbering"] = CswConvert.ToDbVal( value );
                if( !value )
                {
                    QuestionNo = Int32.MinValue;
                    SubQuestionNo = Int32.MinValue;
                }
            }
        }//UseNumbering

        // This should not trigger versioning
        public bool HideInMobile
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["hideinmobile"] ); }
            set { _DataRow["hideinmobile"] = CswConvert.ToDbVal( value ); }
        }

        // This should not trigger versioning
        public bool MobileSearch
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["mobilesearch"] ); }
            set { _DataRow["mobilesearch"] = CswConvert.ToDbVal( value ); }
        }

        //public Int32 DisplayColAdd
        //{
        //    get { return CswConvert.ToInt32( _NodeTypePropRow["display_col_add"] ); }
        //    set { _setAttribute( "display_col_add", value, true ); }
        //}

        //public Int32 DisplayRowAdd
        //{
        //    get { return CswConvert.ToInt32( _NodeTypePropRow["display_row_add"] ); }
        //    set { _setAttribute( "display_row_add", value, true ); }
        //}


        //public bool SetValueOnAdd
        //{
        //    get { return CswConvert.ToBoolean( _NodeTypePropRow["setvalonadd"] ); }
        //    set
        //    {
        //        if( IsRequired && !value && this.DefaultValue.Empty )
        //            throw new CswDniException( ErrorType.Warning, "Required properties must have 'set value on add' enabled unless a default value is present", "User attempted to set SetValueOnAdd = false on a required property with an empty default value" );
        //        if( hasFilter() && value )
        //            throw new CswDniException( ErrorType.Warning, "Conditional properties cannot have 'set value on add' enabled", "User attempted to set SetValueOnAdd = true on a conditional property" );

        //        _setAttribute( "setvalonadd", value, true );
        //    }
        //}
        public bool AllowReadOnlyAdd
        {
            get
            {
                return _CswNbtMetaDataResources.CswNbtResources.EditMode == NodeEditMode.Add &&
                       null != AddLayout;
            }
        }

        public bool SetValueOnAddEnabled
        {
            get
            {
                // Case 20480
                return ( !( IsRequired && DefaultValue.Empty ) ); //&& !hasFilter() );
            }
        }

        public string Attribute1
        {
            get
            {
                return _NodeTypePropRow[NodeTypePropAttributes.attribute1.ToString()].ToString();
            }
            set
            {
                _NodeTypePropRow[NodeTypePropAttributes.attribute1.ToString()] = value;
            }
        }

        public string Attribute2
        {
            get
            {
                return _NodeTypePropRow[NodeTypePropAttributes.attribute2.ToString()].ToString();
            }
            set
            {
                _NodeTypePropRow[NodeTypePropAttributes.attribute2.ToString()] = value;
            }
        }

        public string Attribute3
        {
            get
            {
                return _NodeTypePropRow[NodeTypePropAttributes.attribute3.ToString()].ToString();
            }
            set
            {
                _NodeTypePropRow[NodeTypePropAttributes.attribute3.ToString()] = value;
            }
        }

        public string Attribute4
        {
            get
            {
                return _NodeTypePropRow[NodeTypePropAttributes.attribute4.ToString()].ToString();
            }
            set
            {
                _NodeTypePropRow[NodeTypePropAttributes.attribute4.ToString()] = value;
            }
        }

        public string Attribute5
        {
            get
            {
                return _NodeTypePropRow[NodeTypePropAttributes.attribute5.ToString()].ToString();
            }
            set
            {
                _NodeTypePropRow[NodeTypePropAttributes.attribute5.ToString()] = value;
            }
        }

        public bool HasSubProps()
        {
            bool ret = false;
            foreach( CswNbtMetaDataNodeTypeProp OtherProp in getOtherNodeTypeProps() )
            {
                if( OtherProp.FilterNodeTypePropId == this.FirstPropVersionId )
                {
                    ret = true;
                }
            }
            return ret;
        } // HasSubProps()

        /// <summary>
        /// Returns all nodetype props for this nodetype (self included)
        /// </summary>
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getOtherNodeTypeProps()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProps( this.NodeTypeId );
        }

        public Int32 FilterNodeTypePropId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["filterpropid"] ); }
        }

        private char FilterDelimiter = CswNbtMetaDataObjectClassProp.FilterDelimiter;
        public void setFilter( Int32 FilterNodeTypePropId, CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode, object FilterValue )
        {
            string FilterString = SubField.Column.ToString() + FilterDelimiter + FilterMode + FilterDelimiter + FilterValue;
            CswNbtMetaDataNodeTypeProp FilterProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( FilterNodeTypePropId );
            _setFilter( FilterProp, FilterString );
        }

        public void setFilter( CswNbtMetaDataNodeTypeProp FilterProp, CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode, object FilterValue )
        {
            string FilterString = SubField.Column.ToString() + FilterDelimiter + FilterMode + FilterDelimiter + FilterValue;
            _setFilter( FilterProp, FilterString );
        }

        public void setFilter( Int32 FilterNodeTypePropId, string FilterString )
        {
            CswNbtMetaDataNodeTypeProp FilterProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( FilterNodeTypePropId );
            _setFilter( FilterProp, FilterString );
        }

        private void _setFilter( CswNbtMetaDataNodeTypeProp FilterProp, string FilterString )
        {
            if( IsRequired )
            {
                throw new CswDniException( ErrorType.Warning, "Required properties cannot be conditional", "User attempted to set a conditional filter on a required property" );
            }

            bool changed = false;

            if( FilterProp != null )
            {
                changed = _setAttribute( "filterpropid", FilterProp.FirstPropVersionId, true );
            }
            else
            {
                FilterString = "";
                _CswNbtMetaDataResources.CswNbtResources.logMessage( "Attempted to create a conditional property filter with based upon a null NodeTypeProperty." );
            }

            changed = _setAttribute( "filter", FilterString, true ) || changed;

            if( changed )
            {
                _CswNbtMetaDataResources.RecalculateQuestionNumbers( getNodeType() );
            }

            // BZ 7363
            //SetValueOnAdd = false;
            //removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
        }

        public void clearFilter()
        {
            bool changed = _setAttribute( "filter", string.Empty, true );
            changed = _setAttribute( "filterpropid", Int32.MinValue, true ) || changed;
            if( changed )
                _CswNbtMetaDataResources.RecalculateQuestionNumbers( getNodeType() );
        }

        public void getFilter( ref CswNbtSubField SubField, ref CswNbtPropFilterSql.PropertyFilterMode FilterMode, ref string FilterValue )
        {
            if( _NodeTypePropRow["filter"].ToString() != string.Empty )
            {
                string[] filter = _NodeTypePropRow["filter"].ToString().Split( FilterDelimiter );
                CswNbtMetaDataNodeTypeProp FilterNodeTypeProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( FilterNodeTypePropId );
                if( FilterNodeTypeProp != null )
                {
                    //CswNbtSubField.PropColumn Column = (CswNbtSubField.PropColumn) Enum.Parse( typeof( CswNbtSubField.PropColumn ), filter[0] );
                    CswNbtSubField.PropColumn Column = (CswNbtSubField.PropColumn) filter[0];
                    SubField = FilterNodeTypeProp.getFieldTypeRule().SubFields[Column];
                    //FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterMode ), filter[1] );
                    FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) filter[1];
                    if( filter.GetUpperBound( 0 ) > 1 )
                        FilterValue = filter[2];
                }
            }
        }
        public string getFilterString()
        {
            return _NodeTypePropRow["filter"].ToString();
        }
        public bool hasFilter()
        {
            return ( _NodeTypePropRow["filter"].ToString() != string.Empty &&
                     _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( FilterNodeTypePropId ) != null );
        }


        public bool CheckFilter( CswNbtNode Node )
        {
            bool FilterMatches = false;

            CswNbtMetaDataNodeTypeProp FilterMetaDataProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( this.FilterNodeTypePropId );

            CswNbtSubField SubField = FilterMetaDataProp.getFieldTypeRule().SubFields.Default;
            CswNbtPropFilterSql.PropertyFilterMode FilterMode = SubField.DefaultFilterMode;
            string FilterValue = null;
            getFilter( ref SubField, ref FilterMode, ref FilterValue );

            CswNbtNodePropWrapper FilterProp = Node.Properties[FilterMetaDataProp];

            // Logical needs a special case
            if( FilterMetaDataProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Logical )
            {
                if( SubField.Name == CswNbtSubField.SubFieldName.Checked )
                {
                    if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                    {
                        FilterMatches = ( CswConvert.ToTristate( FilterValue ) == FilterProp.AsLogical.Checked );
                    }
                    else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
                    {
                        FilterMatches = ( CswConvert.ToTristate( FilterValue ) != FilterProp.AsLogical.Checked );
                    }
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "Invalid filter condition", "CswNbtMetaDataNodeTypeProp only supports 'Checked Equality' filters on Logical properties" );
                }
            }
            else
            {
                string ValueToCompare = string.Empty;

                switch( FilterMetaDataProp.getFieldType().FieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.List:
                        ValueToCompare = FilterProp.AsList.Value;
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.Static:
                        ValueToCompare = FilterProp.AsStatic.StaticText;
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.Text:
                        ValueToCompare = FilterProp.AsText.Text;
                        break;
                    default:
                        throw new CswDniException( ErrorType.Error, "Invalid filter condition", "CswPropertyTable does not support field type: " + FilterMetaDataProp.getFieldType().FieldType.ToString() );
                } // switch( FilterMetaDataProp.FieldType.FieldType )

                if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                {
                    FilterMatches = ( ValueToCompare.ToLower() == FilterValue.ToLower() );
                }
                else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
                {
                    FilterMatches = ( ValueToCompare.ToLower() != FilterValue.ToLower() );
                }
                else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
                {
                    FilterMatches = ( ValueToCompare == string.Empty );
                }
                else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull )
                {
                    FilterMatches = ( ValueToCompare != string.Empty );
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "Invalid filter condition", "CswPropertyTable does not support filter mode){ " + FilterMode.ToString() );
                } // switch( FilterMode )

            } // if-else( FilterMetaDataProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Logical )

            return FilterMatches;

        } // _CheckFilter()

        public bool FilterEnabled
        {
            get { return false == IsRequired; }
        }

        public bool EditTabEnabled()
        {
            return ( false == HasConditionalProperties() );
        }

        /// <summary>
        /// Returns whether other properties are conditional on this property
        /// </summary>
        public bool HasConditionalProperties()
        {
            bool ret = false;
            foreach( CswNbtMetaDataNodeTypeProp OtherProp in getOtherNodeTypeProps() )
            {
                if( OtherProp.hasFilter() && OtherProp.FilterNodeTypePropId == this.PropId )
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypePropLatestVersion()
        {
            //CswNbtMetaDataNodeTypeProp ret = null;
            //if( this.getNodeType().IsLatestVersion() )
            //{
            //    ret = this;
            //}
            //else
            //{
            //    foreach( CswNbtMetaDataNodeTypeProp OtherProp in this.getNodeTypeLatestVersion().getNodeTypeProps() )
            //    {
            //        if( OtherProp.FirstPropVersionId == this.FirstPropVersionId )
            //            ret = OtherProp;
            //    }
            //}
            //return ret;
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypePropLatestVersion( PropId );
        }


        /// <summary>
        /// Returns whether the value of this property should be allowed to be copied to other nodes
        /// </summary>
        public bool IsCopyable()
        {
            return ( false == IsUnique() && !ReadOnly && getFieldType().IsCopyable() );
        }
        /// <summary>
        /// Returns whether this property can be deleted
        /// </summary>
        public bool IsDeletable()
        {
            return ( ObjectClassPropId == Int32.MinValue );
        }

        private static string SequenceIdColumn = "sequenceid";
        public Int32 SequenceId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow[SequenceIdColumn] ); }
        }

        public void setSequence( Int32 SequenceId )
        {
            if( CswConvert.ToInt32( _NodeTypePropRow[SequenceIdColumn] ) != SequenceId )
            {
                _checkVersioningProp();

                _NodeTypePropRow[SequenceIdColumn] = CswConvert.ToDbVal( SequenceId );

                CswNbtMetaDataFieldType FT = getFieldType();
                if( SequenceId > 0 && ( FT.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence ||
                                        FT.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode ) )
                {
                    // Update nodes
                    //ICswNbtTree TreeOfNodesOfType = _CswNbtMetaDataResources.CswNbtResources.Trees.getTreeFromNodeTypeId( NodeType.NodeTypeId );
                    //TreeOfNodesOfType.goToRoot();
                    //int TotalNodes = TreeOfNodesOfType.getChildNodeCount();
                    //if( TotalNodes > 0 )
                    //{
                    //    TreeOfNodesOfType.goToNthChild( 0 );
                    //for( int idx = 0; idx < TotalNodes; idx++ )
                    //{
                    //TreeOfNodesOfType.goToParentNode();
                    //TreeOfNodesOfType.goToNthChild( idx );
                    //CswNbtNode CurrentNode = TreeOfNodesOfType.getNodeForCurrentPosition();
                    foreach( CswNbtNode CurrentNode in getNodeType().getNodes( false, false ) )
                    {
                        CswNbtNodePropWrapper CurrentProp = CurrentNode.Properties[this];

                        if( CurrentProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence && CurrentProp.AsSequence.Empty )
                        {
                            CurrentNode.Properties[this].AsSequence.setSequenceValue();
                        }
                        else if( CurrentProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode && CurrentProp.AsBarcode.Empty )
                        {
                            CurrentNode.Properties[this].AsBarcode.setBarcodeValue();
                        }
                    }
                    //} // for( int idx = 0; idx < TotalNodes; idx++ )

                    // need to post this change immediately for resync to work
                    _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( _NodeTypePropRow.Table );

                    // Resync Sequence to next new value
                    CswNbtSequenceValue SeqValue = new CswNbtSequenceValue( _CswNbtMetaDataResources.CswNbtResources, SequenceId );
                    SeqValue.reSync();

                    //} // if( TotalNodes > 0 )
                } // if prop is sequence or barcode
            } // if( CswConvert.ToInt32( _NodeTypePropRow[SequenceIdColumn] ) != SequenceId )
        } // setSequence()

        public static string _Element_MetaDataNodeTypeProp = "MetaDataNodeTypeProp";
        public static string _Element_DefaultValue = "DefaultValue";
        public static string _Element_SubFieldMap = "SubFieldMap";
        public static string _Attribute_NodeTypePropId = "nodetypepropid";
        public static string _Attribute_JctNodePropId = "jctnodepropid";
        public static string _Attribute_NodeTypePropName = "nodetypepropname";
        //public static string _Attribute_order = "order";
        public static string _Attribute_nodetypeid = "nodetypeid";
        public static string _Attribute_nodetypetabid = "nodetypetabid";
        public static string _Attribute_usenumbering = "usenumbering";
        public static string _Attribute_questionno = "questionno";
        public static string _Attribute_subquestionNo = "subquestionno";
        public static string _Attribute_fieldtype = "fieldtype";
        public static string _Attribute_readonly = "readonly";
        public static string _Attribute_isrequired = "isrequired";
        public static string _Attribute_isunique = "isunique";
        public static string _Attribute_length = "length";
        public static string _Attribute_datetoday = "datetoday";
        public static string _Attribute_listoptions = "listoptions";
        public static string _Attribute_numberprecision = "numberprecision";
        public static string _Attribute_minvalue = "minvalue";
        public static string _Attribute_maxvalue = "maxvalue";
        public static string _Attribute_statictext = "statictext";
        //public static string _Attribute_defaultvalue = "defaultvalue";
        public static string _Attribute_filterpropid = "filterpropid";
        public static string _Attribute_filter = "filter";
        //public static string _Attribute_displayrow = "displayrow";
        //public static string _Attribute_displaycolumn = "displaycolumn";
        //public static string _Attribute_displayrowadd = "displayrowadd";
        //public static string _Attribute_displaycoladd = "displaycoladd";
        public static string _Attribute_firstpropversionid = "firstpropversionid";
        public static string _Attribute_SubFieldName = "SubFieldName";
        public static string _Attribute_RelationalTable = "RelationalTable";
        public static string _Attribute_RelationalColumn = "RelationalColumn";

        public XmlNode ToXml( XmlDocument XmlDoc, bool ForMobile )
        {
            CswNbtMetaDataFieldType thisFieldType = getFieldType();

            XmlNode PropNode = XmlDoc.CreateNode( XmlNodeType.Element, "MetaDataNodeTypeProp", "" );

            XmlAttribute PropIdAttr = XmlDoc.CreateAttribute( _Attribute_NodeTypePropId );
            PropIdAttr.Value = PropId.ToString();
            PropNode.Attributes.Append( PropIdAttr );

            XmlAttribute NameAttr = XmlDoc.CreateAttribute( _Attribute_NodeTypePropName );
            if( ForMobile )
                NameAttr.Value = PropNameWithQuestionNo;
            else    // BZ 8644
                NameAttr.Value = PropName;
            PropNode.Attributes.Append( NameAttr );

            //XmlAttribute OrderAttr = XmlDoc.CreateAttribute( _Attribute_order );
            //CswNbtMetaDataNodeTypeTab Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( EditLayout.TabId );
            //OrderAttr.Value = Tab.GetPropDisplayOrder( this ).ToString();
            //PropNode.Attributes.Append( OrderAttr );

            //XmlAttribute DisplayRowAttr = XmlDoc.CreateAttribute( _Attribute_displayrow );
            //DisplayRowAttr.Value = DisplayRow.ToString();
            //PropNode.Attributes.Append( DisplayRowAttr );

            //XmlAttribute DisplayColumnAttr = XmlDoc.CreateAttribute( _Attribute_displaycolumn );
            //DisplayColumnAttr.Value = DisplayColumn.ToString();
            //PropNode.Attributes.Append( DisplayColumnAttr );

            //XmlAttribute DisplayRowAddAttr = XmlDoc.CreateAttribute( _Attribute_displayrowadd );
            //DisplayRowAddAttr.Value = DisplayRowAdd.ToString();
            //PropNode.Attributes.Append( DisplayRowAddAttr );

            //XmlAttribute DisplayColAddAttr = XmlDoc.CreateAttribute( _Attribute_displaycoladd );
            //DisplayColAddAttr.Value = DisplayColAdd.ToString();
            //PropNode.Attributes.Append( DisplayColAddAttr );

            XmlAttribute NodeTypeIdAttr = XmlDoc.CreateAttribute( _Attribute_nodetypeid );
            NodeTypeIdAttr.Value = NodeTypeId.ToString();
            PropNode.Attributes.Append( NodeTypeIdAttr );

            //XmlAttribute NodeTypeTabIdAttr = XmlDoc.CreateAttribute( _Attribute_nodetypetabid );
            //NodeTypeTabIdAttr.Value = NodeTypeTab.TabId.ToString();
            //PropNode.Attributes.Append( NodeTypeTabIdAttr );

            XmlAttribute FirstPropVersionIdAttr = XmlDoc.CreateAttribute( _Attribute_firstpropversionid ); //bz # 8016
            FirstPropVersionIdAttr.Value = FirstPropVersionId.ToString();
            PropNode.Attributes.Append( FirstPropVersionIdAttr );

            XmlAttribute UseNumberingAttr = XmlDoc.CreateAttribute( _Attribute_usenumbering );
            UseNumberingAttr.Value = UseNumbering.ToString();
            PropNode.Attributes.Append( UseNumberingAttr );
            XmlAttribute QuestionNoAttr = XmlDoc.CreateAttribute( _Attribute_questionno );
            PropNode.Attributes.Append( QuestionNoAttr );
            XmlAttribute SubQuestionNoAttr = XmlDoc.CreateAttribute( _Attribute_subquestionNo );
            PropNode.Attributes.Append( SubQuestionNoAttr );
            if( UseNumbering )
            {
                QuestionNoAttr.Value = QuestionNo.ToString();
                SubQuestionNoAttr.Value = SubQuestionNo.ToString();
            }
            else
            {
                QuestionNoAttr.Value = string.Empty;
                SubQuestionNoAttr.Value = string.Empty;
            }

            XmlAttribute FieldTypeAttr = XmlDoc.CreateAttribute( _Attribute_fieldtype );
            FieldTypeAttr.Value = thisFieldType.FieldType.ToString();
            PropNode.Attributes.Append( FieldTypeAttr );

            //bz #7632: Locations should be editable
            XmlAttribute ReadOnlyAttr = XmlDoc.CreateAttribute( _Attribute_readonly );
            if( ForMobile && ( ( CswNbtMetaDataFieldType.NbtFieldType.Location != thisFieldType.FieldType ) && false == thisFieldType.IsSimpleType() ) )
                ReadOnlyAttr.Value = "true";
            else
                ReadOnlyAttr.Value = ReadOnly.ToString().ToLower();
            PropNode.Attributes.Append( ReadOnlyAttr );

            XmlAttribute IsRequiredAttr = XmlDoc.CreateAttribute( _Attribute_isrequired );
            IsRequiredAttr.Value = IsRequired.ToString().ToLower();
            PropNode.Attributes.Append( IsRequiredAttr );

            XmlAttribute IsUniqueAttr = XmlDoc.CreateAttribute( _Attribute_isunique );
            IsUniqueAttr.Value = IsUnique().ToString().ToLower();
            PropNode.Attributes.Append( IsUniqueAttr );

            XmlAttribute LengthAttr = XmlDoc.CreateAttribute( _Attribute_length );
            PropNode.Attributes.Append( LengthAttr );
            if( Length > 0 )
                LengthAttr.Value = Length.ToString();
            else
                LengthAttr.Value = Int32.MinValue.ToString();

            XmlAttribute DateTodayAttr = XmlDoc.CreateAttribute( _Attribute_datetoday );
            DateTodayAttr.Value = DateToday.ToString();
            PropNode.Attributes.Append( DateTodayAttr );

            XmlAttribute ListOptionsAttr = XmlDoc.CreateAttribute( _Attribute_listoptions );
            ListOptionsAttr.Value = ListOptions;
            PropNode.Attributes.Append( ListOptionsAttr );

            XmlAttribute NumberPrecisionAttr = XmlDoc.CreateAttribute( _Attribute_numberprecision );
            PropNode.Attributes.Append( NumberPrecisionAttr );
            if( NumberPrecision > 0 )
                NumberPrecisionAttr.Value = NumberPrecision.ToString();
            else
                NumberPrecisionAttr.Value = Int32.MinValue.ToString();

            XmlAttribute MinValueAttr = XmlDoc.CreateAttribute( _Attribute_minvalue );
            PropNode.Attributes.Append( MinValueAttr );
            if( MinValue != Int32.MinValue )
                MinValueAttr.Value = MinValue.ToString();
            else
                MinValueAttr.Value = Int32.MinValue.ToString();

            XmlAttribute MaxValueAttr = XmlDoc.CreateAttribute( _Attribute_maxvalue );
            PropNode.Attributes.Append( MaxValueAttr );
            if( MaxValue != Int32.MinValue )
                MaxValueAttr.Value = MaxValue.ToString();
            else
                MaxValueAttr.Value = Int32.MinValue.ToString();

            XmlAttribute StaticTextAttr = XmlDoc.CreateAttribute( _Attribute_statictext );
            StaticTextAttr.Value = StaticText;
            PropNode.Attributes.Append( StaticTextAttr );

            // Default value is a subnode, not an attribute
            if( HasDefaultValue() )
            {
                XmlNode DefaultValueNode = XmlDoc.CreateElement( _Element_DefaultValue );
                DefaultValue.ToXml( DefaultValueNode );
                PropNode.AppendChild( DefaultValueNode );
            }

            // Jct_dd_ntp value is a set of subnodes, not attributes
            foreach( CswNbtSubField SubField in _CswNbtMetaDataResources.CswNbtMetaData.getFieldTypeRule( thisFieldType.FieldType ).SubFields )
            {
                if( SubField.RelationalTable != string.Empty )
                {
                    XmlNode SubFieldMapNode = XmlDoc.CreateElement( _Element_SubFieldMap );
                    PropNode.AppendChild( SubFieldMapNode );

                    XmlAttribute SubFieldNameAttr = XmlDoc.CreateAttribute( _Attribute_SubFieldName );
                    SubFieldNameAttr.Value = SubField.ToXmlNodeName();
                    SubFieldMapNode.Attributes.Append( SubFieldNameAttr );

                    XmlAttribute RTableAttr = XmlDoc.CreateAttribute( _Attribute_RelationalTable );
                    RTableAttr.Value = SubField.RelationalTable;
                    SubFieldMapNode.Attributes.Append( RTableAttr );

                    XmlAttribute RColumnAttr = XmlDoc.CreateAttribute( _Attribute_RelationalColumn );
                    RColumnAttr.Value = SubField.RelationalColumn;
                    SubFieldMapNode.Attributes.Append( RColumnAttr );

                } // if( SubField.RelationalTable != string.Empty )
            } // foreach( CswNbtSubField SubField in this.FieldTypeRule.SubFields )

            XmlAttribute FilterAttr = XmlDoc.CreateAttribute( _Attribute_filter );
            PropNode.Attributes.Append( FilterAttr );
            XmlAttribute FilterPropIdAttr = XmlDoc.CreateAttribute( _Attribute_filterpropid );
            PropNode.Attributes.Append( FilterPropIdAttr );
            if( hasFilter() )
            {
                FilterPropIdAttr.Value = FilterNodeTypePropId.ToString();
                FilterAttr.Value = getFilterString();
            }
            else
            {
                FilterPropIdAttr.Value = string.Empty;
                FilterAttr.Value = string.Empty;
            }

            return PropNode;
        }

        public void SetFromXmlDataRow( DataRow PropXmlRow )
        {
            this.DateToday = CswConvert.ToBoolean( PropXmlRow[_Attribute_datetoday] );
            //NewProp.DefaultValue = NodeTypePropRowFromXml[_Attribute_defaultvalue].ToString();
            this.IsRequired = CswConvert.ToBoolean( PropXmlRow[_Attribute_isrequired] );
            if( PropXmlRow.Table.Columns.Contains( _Attribute_isunique ) )
            {
                this.setIsUnique( CswConvert.ToBoolean( PropXmlRow[_Attribute_isunique] ) );
            }
            this.Length = CswConvert.ToInt32( PropXmlRow[_Attribute_length] );
            this.ListOptions = PropXmlRow[_Attribute_listoptions].ToString();
            this.MaxValue = CswConvert.ToDouble( PropXmlRow[_Attribute_maxvalue] );
            this.MinValue = CswConvert.ToDouble( PropXmlRow[_Attribute_minvalue] );
            this.NumberPrecision = CswConvert.ToInt32( PropXmlRow[_Attribute_numberprecision] );
            //this.DisplayRow = CswConvert.ToInt32( PropXmlRow[_Attribute_displayrow] );
            //this.DisplayColumn = CswConvert.ToInt32( PropXmlRow[_Attribute_displaycolumn] );
            //this.DisplayRowAdd = CswConvert.ToInt32( PropXmlRow[_Attribute_displayrowadd] );
            //this.DisplayColAdd = CswConvert.ToInt32( PropXmlRow[_Attribute_displaycoladd] );
            this.UseNumbering = CswConvert.ToBoolean( PropXmlRow[_Attribute_usenumbering] );
            this.QuestionNo = CswConvert.ToInt32( PropXmlRow[_Attribute_questionno] );
            this.SubQuestionNo = CswConvert.ToInt32( PropXmlRow[_Attribute_subquestionNo] );
            this.ReadOnly = CswConvert.ToBoolean( PropXmlRow[_Attribute_readonly] );
            this.StaticText = PropXmlRow[_Attribute_statictext].ToString();
            //NewProp._setFilter(NodeTypePropRowFromXml[_Attribute_filterpropid],
            //                  NodeTypePropRowFromXml[_Attribute_filter]
        }


        /// <summary>
        /// Returns whether the property table should show the property name as a label next to the value
        /// </summary>
        public bool ShowLabel
        {
            get { return getFieldType().ShowLabel(); }
        }

        #region IEquatable

        public static bool operator ==( CswNbtMetaDataNodeTypeProp ntp1, CswNbtMetaDataNodeTypeProp ntp2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( ntp1, ntp2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) ntp1 == null ) || ( (object) ntp2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( ntp1.UniqueId == ntp2.UniqueId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtMetaDataNodeTypeProp ntp1, CswNbtMetaDataNodeTypeProp ntp2 )
        {
            return !( ntp1 == ntp2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtMetaDataNodeTypeProp ) )
                return false;
            return this == (CswNbtMetaDataNodeTypeProp) obj;
        }

        public bool Equals( CswNbtMetaDataNodeTypeProp obj )
        {
            return this == (CswNbtMetaDataNodeTypeProp) obj;
        }

        public override int GetHashCode()
        {
            return this.PropId;
        }

        #endregion IEquatable

        #region IComparable

        public int CompareTo( object obj )
        {
            if( !( obj is CswNbtMetaDataNodeTypeProp ) )
                throw new ArgumentException( "object is not a CswNbtMetaDataNodeTypeProp" );

            CswNbtMetaDataNodeTypeProp OtherNodeTypeProp = (CswNbtMetaDataNodeTypeProp) obj;
            return CompareTo( OtherNodeTypeProp );
        }

        /// <summary>
        /// Comparison method that puts properties in display order, top to bottom, left to right, and then alphabetical.
        /// </summary>
        public int CompareTo( CswNbtMetaDataNodeTypeProp OtherNodeTypeProp )
        {
            int ret = 0;
            if( FirstEditLayout.DisplayRow == OtherNodeTypeProp.FirstEditLayout.DisplayRow )
            {
                if( FirstEditLayout.DisplayColumn == OtherNodeTypeProp.FirstEditLayout.DisplayColumn )
                {
                    ret = this.PropName.CompareTo( OtherNodeTypeProp.PropName );
                }
                else
                {
                    ret = this.FirstEditLayout.DisplayColumn.CompareTo( OtherNodeTypeProp.FirstEditLayout.DisplayColumn );
                }
            }
            else
            {
                ret = this.FirstEditLayout.DisplayRow.CompareTo( OtherNodeTypeProp.FirstEditLayout.DisplayRow );
            }
            return ret;
        }


        #endregion IComparable



        public bool IsUserRelationship()
        {
            bool ret = false;
            if( this.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
            {
                if( FKType != string.Empty )
                {
                    //NbtViewRelatedIdType TargetType = (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), FKType, true );
                    NbtViewRelatedIdType TargetType = (NbtViewRelatedIdType) FKType;

                    if( TargetType == NbtViewRelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType TargetNodeType = _CswNbtMetaDataResources.CswNbtResources.MetaData.getNodeType( FKValue );
                        if( null != TargetNodeType )
                        {
                            ret = ( TargetNodeType.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                        }
                    }
                    else if( TargetType == NbtViewRelatedIdType.ObjectClassId )
                    {
                        CswNbtMetaDataObjectClass TargetObjectClass = _CswNbtMetaDataResources.CswNbtResources.MetaData.getObjectClass( FKValue );
                        if( null != TargetObjectClass )
                        {
                            ret = ( TargetObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                        }
                    }
                }
            }
            return ret;
        }
    }
}
