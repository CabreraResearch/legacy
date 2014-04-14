using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;

namespace ChemSW.Nbt.MetaData
{
    [DataContract]
    public class CswNbtMetaDataNodeTypeProp : ICswNbtMetaDataObject, ICswNbtMetaDataProp, IEquatable<CswNbtMetaDataNodeTypeProp>, IComparable
    {
        private CswDateTime _Date;
        /// <summary>
        /// For auditing: the date for which the property was initialized
        /// </summary>
        public CswDateTime Date
        {
            get { return _Date; }
        }

        public static CswEnumNbtNodeTypePropAttributes getCswEnumNbtNodeTypePropAttributesFromString( string AttributeName )
        {
            CswEnumNbtNodeTypePropAttributes ReturnVal = CswResources.UnknownEnum;
            AttributeName = AttributeName.Replace( "_", "" );
            ReturnVal = AttributeName;

            return ( ReturnVal );
        }

        public static String getCswEnumNbtNodeTypePropAttributesAsString( CswEnumNbtNodeTypePropAttributes Attribute )
        {
            String ReturnVal = String.Empty;
            if( Attribute != CswResources.UnknownEnum )
                ReturnVal = Attribute.ToString().Replace( "_", "" );
            return ( ReturnVal );
        }

        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _NodeTypePropRow;
        public CswNbtMetaDataNodeTypeProp( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row, CswDateTime Date = null )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _Date = Date;
            Reassign( Row );
        }

        #region Layout

        /// <summary>
        /// Returns the layout for this property.  If edit, be sure to supply a valid TabId
        /// </summary>
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getLayout( CswEnumNbtLayoutType LayoutType, Int32 TabId = Int32.MinValue )
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.getLayout( LayoutType, this.NodeTypeId, this.PropId, TabId, _Date );
        }

        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getEditLayout( Int32 TabId )
        {
            return getLayout( CswEnumNbtLayoutType.Edit, TabId );
        }
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getAddLayout()
        {
            return getLayout( CswEnumNbtLayoutType.Add );
        }
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getTableLayout()
        {
            return getLayout( CswEnumNbtLayoutType.Table );
        }
        public CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout getPreviewLayout()
        {
            return getLayout( CswEnumNbtLayoutType.Preview );
        }
        public Dictionary<Int32, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout> getEditLayouts()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.getLayout( CswEnumNbtLayoutType.Edit, this, _Date );
        }

        public void updateLayout( CswEnumNbtLayoutType LayoutType, bool DoMove, Int32 TabId = Int32.MinValue, Int32 DisplayRow = Int32.MinValue, Int32 DisplayColumn = Int32.MinValue, string TabGroup = "" )
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.updatePropLayout( LayoutType, NodeTypeId, this, DoMove, TabId, DisplayRow, DisplayColumn, TabGroup );
        }
        public void updateLayout( CswEnumNbtLayoutType LayoutType, CswNbtMetaDataNodeTypeProp InsertAfterProp, bool DoMove )
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.updatePropLayout( LayoutType, this, InsertAfterProp, DoMove );
        }
        public void removeFromLayout( CswEnumNbtLayoutType LayoutType, Int32 TabId = Int32.MinValue )
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( LayoutType, this, TabId );
        }
        public void removeFromAllLayouts()
        {
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswEnumNbtLayoutType.Add, this, Int32.MinValue );
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswEnumNbtLayoutType.Edit, this, Int32.MinValue );
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswEnumNbtLayoutType.Preview, this, Int32.MinValue );
            _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.removePropFromLayout( CswEnumNbtLayoutType.Table, this, Int32.MinValue );
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

        private CswNbtObjClassDesignNodeTypeProp _DesignNode = null;
        public CswNbtObjClassDesignNodeTypeProp DesignNode
        {
            get
            {
                if( null == _DesignNode )
                {
                    _DesignNode = _CswNbtMetaDataResources.CswNbtResources.Nodes.getNodeByRelationalId( new CswPrimaryKey( "nodetype_props", PropId ) );
                    if( null != _DesignNode )
                    {
                        _CswNbtMetaDataResources.addDesignNodeForFinalization( _DesignNode.Node );
                    }
                }
                return _DesignNode;
            }
        }

        public CswPrimaryKey DesignNodeId
        {
            get { return _CswNbtMetaDataResources.CswNbtResources.Nodes.getNodeIdByRelationalId( new CswPrimaryKey( "nodetype_props", PropId ) ); }
        }

        public object this[CswEnumNbtPropertyAttributeColumn Column]
        {
            get { return _DataRow[Column.ToString()]; }
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

        //private void _checkVersioningProp()
        //{
        //    CswNbtMetaDataNodeType NewNodeType = _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioningDeprecated( this.getNodeType() );
        //    if( NewNodeType.NodeTypeId != NodeTypeId )
        //    {
        //        // Get the new property and reassign myself
        //        CswNbtMetaDataNodeTypeProp NewProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypePropVersion( NewNodeType.NodeTypeId, this.PropId );
        //        this._NodeTypePropRow = NewProp._DataRow;
        //    }
        //}

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
                    //_checkVersioningProp();
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
                    //_checkVersioningProp();
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
                    //_checkVersioningProp();
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
                    //_checkVersioningProp();
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
                    //_checkVersioningProp();
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
                throw new CswDniException( CswEnumErrorType.Error, "Unrecognized attribute type", "CswNbtMetaDataNodeTypeProp._setAttribute encountered an unrecognized attribute type" );
            }
            return ret;
        } // _setAttribute()

        private ICswNbtFieldTypeRule _FieldTypeRule = null;
        public ICswNbtFieldTypeRule getFieldTypeRule()
        {
            if( _FieldTypeRule == null )
                _FieldTypeRule = _CswNbtMetaDataResources.makeFieldTypeRule( this.getFieldTypeValue() );
            return _FieldTypeRule;
        }

        public Int32 PropId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["nodetypepropid"].ToString() ); }
        }

        [DataMember]
        public string PropName
        {
            get
            {
                return _NodeTypePropRow["propname"].ToString();
            }
            private set
            {
                // BZ 5492: Make sure this is unique for this nodetype

                if( _NodeTypePropRow["propname"].ToString() != value &&
                    _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypePropId( this.NodeTypeId, value ) != Int32.MinValue )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Property Name must be unique per nodetype", "Attempted to save a propname which is equal to a propname of another property in this nodetype" );
                }

                _setAttribute( "propname", value, true );

                resetDbViewColumnName();

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

        [DataMember( Name = "ColumnName" )]
        public Collection<CswNbtSdDbQueries.Column> DbViewColumns
        {
            get
            {
                Collection<CswNbtSdDbQueries.Column> Ret = new Collection<CswNbtSdDbQueries.Column>();

                string DbName = "P" + PropId;
                CswNbtSdDbQueries.Column Gestalt = new CswNbtSdDbQueries.Column();
                Gestalt.Name = PropName;
                Gestalt.Type = "clob";
                Gestalt.DbName = DbName;
                Ret.Add( Gestalt );

                if( this.getFieldType().FieldType == CswEnumNbtFieldType.Relationship )
                {
                    CswNbtSdDbQueries.Column Relationship = new CswNbtSdDbQueries.Column();
                    //if( this.FKType == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() )
                    //{

                    //} else if
                    //{

                    //}

                    Relationship.Name = PropName + " Fk";
                    Relationship.Type = "number(12,0)";
                    Relationship.DbName = DbName + "_fk";
                    Ret.Add( Relationship );
                }
                else if( this.getFieldType().FieldType == CswEnumNbtFieldType.Location )
                {

                }
                else
                {

                }

                return Ret;
            }
            private set { var KeepSerializerHappy = value; }
        }
        [DataMember( Name = "ViewName" )]
        public string DbViewColumnName
        {
            get
            {
                return CswConvert.ToString( _NodeTypePropRow["oraviewcolname"] );
            }
        }

        public void resetDbViewColumnName()
        {
            //use objectclasspropname if we have it
            if( this.ObjectClassPropId != Int32.MinValue )
            {
                _NodeTypePropRow["oraviewcolname"] = this.getObjectClassProp().DbViewColumnName;
            }
            else
            {
                if( UseNumbering && QuestionNo != Int32.MinValue )
                {
                    _NodeTypePropRow["oraviewcolname"] = CswFormat.MakeOracleCompliantIdentifier( CswNbtMetaData.OraViewColNamePrefix + FullQuestionNo.Replace( ".", "x" ) );
                }
                else
                {
                    _NodeTypePropRow["oraviewcolname"] = CswFormat.MakeOracleCompliantIdentifier( CswNbtMetaData.OraViewColNamePrefix + PropName );
                }
            }
        }

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
                    ThisView.saveNew( this.PropName, CswEnumNbtViewVisibility.Property, null, null, Int32.MinValue );
                    if( this.getFieldTypeValue() == CswEnumNbtFieldType.Grid )
                    {
                        // BZ 9203 - View starts with this property's nodetype at root
                        ThisView.AddViewRelationship( this.getNodeType(), true );
                        ThisView.SetViewMode( CswEnumNbtViewRenderingMode.Grid );
                    }
                    ThisView.save();
                    _NodeTypePropRow["nodeviewid"] = CswConvert.ToDbVal( ThisView.ViewId.get() );
                    ret = ThisView.ViewId;
                }
                return ret;
            }
            private set { _NodeTypePropRow["nodeviewid"] = CswConvert.ToDbVal( value.get() ); }
        }
        public bool IsRequired
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["isrequired"] ); }
            private set
            {
                _setAttribute( "isrequired", value, false );

                if( value )
                {
                    _NodeTypePropRow["filtersubfield"] = string.Empty;
                    _NodeTypePropRow["filtermode"] = string.Empty;
                    _NodeTypePropRow["filtervalue"] = string.Empty;
                    _NodeTypePropRow["filterpropid"] = CswConvert.ToDbVal( Int32.MinValue );
                    if( HasDefaultValue() )
                    {
                        //If the prop isn't on the Add layout, Add it.
                        if( false == ExistsOnLayout( CswEnumNbtLayoutType.Add ) )
                        {
                            updateLayout( CswEnumNbtLayoutType.Add, TabId: Int32.MinValue, TabGroup: string.Empty, DisplayRow: Int32.MinValue, DisplayColumn: Int32.MinValue, DoMove: false );
                        }
                    }
                }
            }
        }

        private bool _ExistsInLayout( CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout Layout )
        {
            bool Ret = false;
            if( null != Layout )
            {
                if( Int32.MinValue != Layout.PropId &&
                    ( Layout.PropId == this.PropId ||
                      Layout.PropId == this.FirstPropVersionId ||
                      Layout.PropId == this.getNodeTypePropLatestVersion().PropId ) )
                {
                    Ret = true;
                }
            }
            return Ret;
        }

        public bool ExistsOnLayout( CswEnumNbtLayoutType Type )
        {
            bool Ret = false;

            if( Type == CswEnumNbtLayoutType.Edit )
            {
                foreach( KeyValuePair<int, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout> KeyValuePair in getEditLayouts() )
                {
                    Ret = Ret || _ExistsInLayout( getLayout( Type, KeyValuePair.Key ) );
                }
            }
            else
            {
                Ret = _ExistsInLayout( getLayout( Type ) );
            }
            return Ret;
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

        private void setIsUnique( bool value )
        {
            _setAttribute( "isunique", value, false );
        }

        //case 24979
        public bool IsCompoundUnique()
        {
            return ( CswConvert.ToBoolean( _NodeTypePropRow["iscompoundunique"] ) );
        }

        private void setIsCompoundUnique( bool value )
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

        private CswEnumTristate _IsSaveProp = CswEnumTristate.Null;
        public bool IsSaveProp
        {
            get
            {
                if( CswEnumTristate.Null == _IsSaveProp )
                {
                    if( Int32.MinValue != ObjectClassPropId &&
                        null != getObjectClassProp() &&
                        getObjectClassProp().getFieldType().FieldType == CswEnumNbtFieldType.Button &&
                        getObjectClassProp().PropName == CswNbtObjClass.PropertyName.Save )
                    {
                        _IsSaveProp = CswEnumTristate.True;
                    }
                    else
                    {
                        _IsSaveProp = CswEnumTristate.False;
                    }
                }
                return CswConvert.ToBoolean( _IsSaveProp );
            }
        }

        public bool ShowSaveProp( CswEnumNbtLayoutType Layout, bool IsConfigMode, bool HasEditableProps )
        {
            return IsSaveProp &&
                   false == IsConfigMode &&
                   ( _CswNbtMetaDataResources.CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add ||
                     _CswNbtMetaDataResources.CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Edit ) &&
                   ( Layout == CswEnumNbtLayoutType.Add ||
                     ( Layout == CswEnumNbtLayoutType.Edit && HasEditableProps ) );
        
        } // ShowSaveProp()


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

        //public CswEnumNbtPropertySelectMode Multi
        //{
        //    get
        //    {
        //        CswEnumNbtPropertySelectMode ret = CswEnumNbtPropertySelectMode.Blank;
        //        switch( _NodeTypePropRow["multi"].ToString() )
        //        {
        //            case "":
        //                ret = CswEnumNbtPropertySelectMode.Blank;
        //                break;
        //            case "0":
        //                ret = CswEnumNbtPropertySelectMode.Single;
        //                break;
        //            case "1":
        //                ret = CswEnumNbtPropertySelectMode.Multiple;
        //                break;
        //        }
        //        return ret;
        //    }
        //    private set
        //    {
        //        switch( value )
        //        {
        //            case CswEnumNbtPropertySelectMode.Blank:
        //                _setAttribute( "multi", DBNull.Value, false );
        //                break;
        //            case CswEnumNbtPropertySelectMode.Single:
        //                _setAttribute( "multi", "0", false );
        //                break;
        //            case CswEnumNbtPropertySelectMode.Multiple:
        //                _setAttribute( "multi", "1", false );
        //                break;
        //        }
        //    }
        //}

        public bool IsSaveable
        {
            get
            {
                bool Ret = true;
                CswEnumNbtFieldType Ft = getFieldType().FieldType;
                if( Ft == CswEnumNbtFieldType.Button ||
                    Ft == CswEnumNbtFieldType.PropertyReference ||
                    Ft == CswEnumNbtFieldType.Static ||
                    Ft == CswEnumNbtFieldType.ReportLink ||
                    Ft == CswEnumNbtFieldType.ChildContents )
                {
                    Ret = false;
                }

                return Ret;
            }
        }

        public bool ShowProp( CswEnumNbtLayoutType LayoutType, CswNbtNode Node, Int32 TabId, bool IsConfigMode, bool HasEditableProps )
        {
            bool ret = true;
            CswNbtMetaDataNodeTypeTab Tab = null;

            // 1: throw away properties incompatable with layouts
            if( LayoutType == CswEnumNbtLayoutType.Add )
            {
                ret = ret && ( IsSaveProp || getFieldType().FieldType != CswEnumNbtFieldType.Button ) &&
                             ( ( IsRequired && false == HasDefaultValue() ) ||
                               CswConvert.ToBoolean( Node.Properties[this][CswEnumNbtPropertyAttributeName.Required] ) ||
                               AddLayout != null );
            }
            if( LayoutType == CswEnumNbtLayoutType.Edit )
            {
                CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout EditLayout = getEditLayout( TabId );
                if( EditLayout != null )
                {
                    Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( EditLayout.TabId );
                }
            }
            if( _CswNbtMetaDataResources.CswNbtResources.EditMode == CswEnumNbtNodeEditMode.PrintReport ||
                _CswNbtMetaDataResources.CswNbtResources.EditMode == CswEnumNbtNodeEditMode.AuditHistoryInPopup )
            {
                ret = ret && ( getFieldType().FieldType != CswEnumNbtFieldType.Button );
            }

            // 2: Validate orthogonal use cases
            if( IsConfigMode )
            {
                ret = ret && ( false == IsSaveProp );
            }
            else if( IsSaveProp )
            {
                ret = ret && ShowSaveProp( LayoutType, false, HasEditableProps ) && ( false == Node.ReadOnly || _CswNbtMetaDataResources.CswNbtResources.CurrentNbtUser.IsAdministrator() );
            }
            else
            {
                ret = ret && ( false == hasFilter() && false == Node.Properties[this].Hidden && false == Hidden );
            }

            // 3: Permissions
            ret = ret && (
                           _CswNbtMetaDataResources.CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, this.getNodeType() ) ||
                           _CswNbtMetaDataResources.CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.View, this.getNodeType(), Tab ) ||
                           _CswNbtMetaDataResources.CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.View, this.getNodeType(), Node.NodeId )
                        );


            return ret;
        }

        //private void _doSetFkDeprecated( string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        //{
        //    FKType = inFKType;
        //    FKValue = inFKValue;
        //    ValuePropId = inValuePropId;
        //    ValuePropType = inValuePropType;
        //    IsFK = Int32.MinValue != inFKValue;
        //}

        public delegate void doSetFk( string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue );

        ///// <summary>
        ///// Set the FK for relationship props
        ///// </summary>
        ///// <param name="inFKType">Either NodeTypeId or ObjectClassId</param>
        ///// <param name="inFKValue">FK Value</param>
        ///// <param name="inValuePropType">Optional (for Property Reference)</param>
        ///// <param name="inValuePropId">Optional  (for Property Reference)</param>
        //public void SetFKDeprecated( string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        //{
        //    //getFieldTypeRule().onSetFk( this, inFKType, inFKValue, inValuePropType, inValuePropId );
        //    _doSetFkDeprecated( inFKType, inFKValue, inValuePropType, inValuePropId );
        //}

        public string FKType
        {
            get { return DesignNode.getAttributeValueByColumn( CswEnumNbtPropertyAttributeColumn.Fktype ); }
            private set { _setAttribute( "fktype", value, false ); }
        }

        public Int32 FKValue
        {
            get { return CswConvert.ToInt32( DesignNode.getAttributeValueByColumn( CswEnumNbtPropertyAttributeColumn.Fkvalue ) ); }
            private set { _setAttribute( "fkvalue", value, false ); }
        }

        public bool IsFK
        {
            get { return CswConvert.ToBoolean( DesignNode.getAttributeValueByColumn( CswEnumNbtPropertyAttributeColumn.Isfk ) ); }
            private set { _setAttribute( "isfk", value, false ); }
        }

        private Int32 ValuePropId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["valuepropid"] ); }
            set
            {
                if( CswConvert.ToInt32( _NodeTypePropRow["valuepropid"] ) != value )
                    getNodeType().SetNodesToPendingUpdate();

                _setAttribute( "valuepropid", value, false );
            }
        }

        private string ValuePropType
        {
            get { return _NodeTypePropRow["valueproptype"].ToString(); }
            set { _setAttribute( "valueproptype", value, false ); }
        }

        public bool Hidden
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["hidden"] ); }
            private set { _setAttribute( "hidden", value, true ); }
        }

        #region FK Matching

        public bool FkMatches( CswNbtMetaDataNodeType CompareNT, bool IgnoreVersions = false )
        {
            return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, FKType, FKValue, CompareNT, IgnoreVersions );
        }

        public bool FkMatches( CswNbtMetaDataObjectClass CompareOC )
        {
            return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, FKType, FKValue, CompareOC );
        }

        public bool FkMatches( CswNbtMetaDataPropertySet ComparePS )
        {
            return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, FKType, FKValue, ComparePS );
        }

        //public bool FkMatchesDeprecated( CswNbtMetaDataNodeType CompareNT, bool IgnoreVersions = false )
        //{
        //    return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, _NodeTypePropRow["fktype"].ToString(), CswConvert.ToInt32( _NodeTypePropRow["fkvalue"] ), CompareNT, IgnoreVersions );
        //}

        //public bool FkMatchesDeprecated( CswNbtMetaDataObjectClass CompareOC )
        //{
        //    return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, _NodeTypePropRow["fktype"].ToString(), CswConvert.ToInt32( _NodeTypePropRow["fkvalue"] ), CompareOC );
        //}

        //public bool FkMatchesDeprecated( CswNbtMetaDataPropertySet ComparePS )
        //{
        //    return CswNbtViewRelationship.Matches( _CswNbtMetaDataResources.CswNbtResources, _NodeTypePropRow["fktype"].ToString(), CswConvert.ToInt32( _NodeTypePropRow["fkvalue"] ), ComparePS );
        //}


        #endregion FK Matching

        public string getCompositeTemplateText()
        {
            return CswNbtMetaData.TemplateValueToTemplateText( getOtherNodeTypeProps(), CompositeTemplateValue );
        }
        public void setCompositeTemplateText( string value )
        {
            CompositeTemplateValue = CswNbtMetaData.TemplateTextToTemplateValue( getOtherNodeTypeProps(), value );
        }
        private string CompositeTemplateValue
        {
            get { return _NodeTypePropRow["compositetemplate"].ToString(); }
            set { _setAttribute( "compositetemplate", value, false ); }
        }
        private bool DateToday
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["datetoday"] ); }
            set { _setAttribute( "datetoday", value, false ); }
        }
        public bool IsBatchEntry
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["isbatchentry"] ); }
            private set { _setAttribute( "isbatchentry", value, false ); }
        }
        public bool ServerManaged
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["servermanaged"] ); }
            private set { _setAttribute( "servermanaged", value, false ); }
        }
        public bool ReadOnly
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["readonly"] ); }
            private set { _setAttribute( "readonly", value, false ); }
        }
        private Int32 TextAreaRows
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["textarearows"] ); }
            set { _setAttribute( "textarearows", value, false ); }
        }
        private Int32 TextAreaColumns
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["textareacols"] ); }
            set { _setAttribute( "textareacols", value, false ); }
        }
        private string ListOptions
        {
            get { return _NodeTypePropRow["listoptions"].ToString(); }
            set { _setAttribute( "listoptions", value, false ); }
        }
        private string ValueOptions
        {
            get { return _NodeTypePropRow["valueoptions"].ToString(); }
            set { _setAttribute( "valueoptions", value, false ); }
        }
        private Int32 NumberPrecision
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["numberprecision"] ); }
            set { _setAttribute( "numberprecision", value, false ); }
        }
        private double MinValue
        {
            get { return CswConvert.ToDouble( _NodeTypePropRow["numberminvalue"] ); }
            set { _setAttribute( "numberminvalue", value, false ); }
        }
        private double MaxValue
        {
            get { return CswConvert.ToDouble( _NodeTypePropRow["numbermaxvalue"] ); }
            set { _setAttribute( "numbermaxvalue", value, false ); }
        }
        private string StaticText
        {
            get { return _NodeTypePropRow["statictext"].ToString(); }
            set { _setAttribute( "statictext", value, false ); }
        }
        private string Extended
        {
            get { return _NodeTypePropRow["extended"].ToString(); }
            set { _setAttribute( "extended", value, false ); }
        }

        public CswEnumAuditLevel AuditLevel
        {
            get { return _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.auditlevel.ToString()].ToString(); }
            private set
            {
                _setAttribute( CswEnumNbtNodeTypePropAttributes.auditlevel.ToString(), value.ToString(), false );
                _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.updateLayoutAuditLevel( this, value );
            }
        }

        //private CswNbtNodePropWrapper _DefaultValue = null;
        //private DataRow _DefaultValueRow = null;

        //private CswNbtNodePropWrapper _initDefaultValueDeprecated( bool CreateMissingRow )
        //{
        //    if( _DefaultValue == null )
        //    {
        //        if( _DefaultValueRow == null )
        //        {
        //            if( _NodeTypePropRow.Table.Columns.Contains( "defaultvalueid" ) )
        //            {
        //                if( _NodeTypePropRow["defaultvalueid"] != null && CswTools.IsInteger( _NodeTypePropRow["defaultvalueid"] ) )
        //                {
        //                    DataTable DefaultValueTable = _CswNbtMetaDataResources.JctNodesPropsTableUpdate.getTable( "jctnodepropid", CswConvert.ToInt32( _NodeTypePropRow["defaultvalueid"] ) );
        //                    if( DefaultValueTable.Rows.Count > 0 )
        //                        _DefaultValueRow = DefaultValueTable.Rows[0];
        //                }
        //                else if( CreateMissingRow )
        //                {
        //                    DataTable NewDefaultValueTable = _CswNbtMetaDataResources.JctNodesPropsTableUpdate.getEmptyTable();
        //                    _DefaultValueRow = NewDefaultValueTable.NewRow();
        //                    _DefaultValueRow["nodetypepropid"] = CswConvert.ToDbVal( this.PropId );
        //                    NewDefaultValueTable.Rows.Add( _DefaultValueRow );
        //                    _NodeTypePropRow["defaultvalueid"] = _DefaultValueRow["jctnodepropid"];
        //                }
        //            } // if( _NodeTypePropRow.Table.Columns.Contains( "defaultvalueid" ) )
        //        } // if( _DefaultValueRow == null )
        //        if( _DefaultValueRow != null )
        //        {
        //            _DefaultValue = CswNbtNodePropFactory.makeNodeProp( _CswNbtMetaDataResources.CswNbtResources, _DefaultValueRow, _DefaultValueRow.Table, null, this, null );
        //        }
        //    } // if( _DefaultValue == null )
        //    return _DefaultValue;
        //}

        //public bool HasDefaultValue()
        //{
        //    _initDefaultValue( false );
        //    return !( _DefaultValue == null || _DefaultValue.Empty );
        //}

        //public CswNbtNodePropWrapper DefaultValue
        //{
        //    get
        //    {
        //        _initDefaultValue( true );
        //        return _DefaultValue;
        //    }
        //    // NO SET...interact with the CswNbtNodePropWrapper instead
        //}

        public CswNbtNodePropWrapper getDefaultValue( bool CreateIfMissing )
        {
            CswNbtNodePropWrapper ret = null;
            if( null != DesignNode )
            {
                if( DesignNode.AttributeProperty.ContainsKey( CswEnumNbtPropertyAttributeName.DefaultValue ) )
                {
                    ret = DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.DefaultValue];
                }
            }

            //if( AllowDeprecated && ( ret == null || ret.Empty ) )
            //{
            //    // DEPRECATED support of old default values.  Should be able to be removed in Larch.
            //    ret = _initDefaultValueDeprecated( CreateIfMissing );
            //}
            return ret;
        } // getDefaultValue()

        public bool HasDefaultValue()
        {
            bool ret = false;
            CswNbtNodePropWrapper defval = getDefaultValue( false );
            if( null != defval )
            {
                ret = ( false == defval.Empty );
            }
            return ret;
        } // HasDefaultValue()

        public Int32 FirstPropVersionId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["firstpropversionid"] ); }
            private set { _setAttribute( "firstpropversionid", value, false ); }
        }

        public Int32 PriorPropVersionId
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["priorpropversionid"] ); }
            private set { _setAttribute( "priorpropversionid", value, false ); }
        }

        // This should not trigger versioning
        public string HelpText
        {
            get { return _NodeTypePropRow["helptext"].ToString(); }
            private set { _NodeTypePropRow["helptext"] = value; }
        }
        // This should not trigger versioning
        public bool IsQuickSearch
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["isquicksearch"] ); }
            private set { _NodeTypePropRow["isquicksearch"] = CswConvert.ToDbVal( value ); }
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

        public CswEnumNbtFieldType getFieldTypeValue()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getFieldTypeValue( FieldTypeId );
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

        public string getObjectClassPropName()
        {
            string ret = string.Empty;
            if( Int32.MinValue != ObjectClassPropId )
            {
                ret = _CswNbtMetaDataResources.CswNbtMetaData.getObjectClassPropName( ObjectClassPropId, _Date );
            }
            return ret;
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
                        ViewCopy.saveNew( View.ViewName, View.Visibility, View.VisibilityRoleId, View.VisibilityUserId, View );
                        //ViewCopy.save();
                        NewPropRow[PropColumn.ColumnName] = CswConvert.ToDbVal( ViewCopy.ViewId.get() );
                    }
                    //else if( PropColumn.ColumnName.ToLower() == "defaultvalueid" && CswTools.IsInteger( _NodeTypePropRow[PropColumn.ColumnName].ToString() ) )
                    //{
                    //    // BZ 10242
                    //    // Same problem -- copy the default value record
                    //    CswTableUpdate JctNodesPropsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "CopyPropToNewNodeTypePropRow_JNP_Update", "jct_nodes_props" );
                    //    JctNodesPropsUpdate.AllowBlobColumns = true;
                    //    DataTable JNPTable = JctNodesPropsUpdate.getTable( "jctnodepropid", CswConvert.ToInt32( _NodeTypePropRow[PropColumn.ColumnName] ) );
                    //    if( JNPTable.Rows.Count > 0 )
                    //    {
                    //        DataRow ExistingJNPRow = JNPTable.Rows[0];
                    //        DataRow NewJNPRow = JNPTable.NewRow();
                    //        NewJNPRow["nodetypepropid"] = NewPropRow["nodetypepropid"];
                    //        foreach( DataColumn JctColumn in JNPTable.Columns )
                    //        {
                    //            if( JctColumn.ColumnName != "jctnodepropid" &&
                    //                JctColumn.ColumnName != "nodetypepropid" )
                    //            {
                    //                NewJNPRow[JctColumn] = ExistingJNPRow[JctColumn];
                    //            }
                    //        }
                    //        JNPTable.Rows.Add( NewJNPRow );
                    //        JctNodesPropsUpdate.update( JNPTable );
                    //        NewPropRow[PropColumn.ColumnName] = NewJNPRow["jctnodepropid"];
                    //    }
                    //}
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

        public string FullQuestionNo
        {
            get
            {
                string ret = "Q";
                CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout First = FirstEditLayout;
                if( null != FirstEditLayout )
                {
                    CswNbtMetaDataNodeTypeTab Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( First.TabId );
                    if( Tab.SectionNo != Int32.MinValue )
                    {
                        ret += Tab.SectionNo.ToString() + ".";
                    }
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
            private set
            {
                _DataRow["questionno"] = CswConvert.ToDbVal( value );
                resetDbViewColumnName();
            }
        }

        // This should not trigger versioning
        public Int32 SubQuestionNo
        {
            get { return CswConvert.ToInt32( _NodeTypePropRow["subquestionno"] ); }
            private set { _DataRow["subquestionno"] = CswConvert.ToDbVal( value ); }
        }
        // This should not trigger versioning
        public bool UseNumbering
        {
            get { return CswConvert.ToBoolean( _NodeTypePropRow["usenumbering"] ); }
            private set
            {
                _DataRow["usenumbering"] = CswConvert.ToDbVal( value );
                if( !value )
                {
                    QuestionNo = Int32.MinValue;
                    SubQuestionNo = Int32.MinValue;
                }
            }
        }//UseNumbering

        //public string Attribute1
        //{
        //    get
        //    {
        //        return _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute1.ToString()].ToString();
        //    }
        //    private set
        //    {
        //        _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute1.ToString()] = value;
        //    }
        //}

        //public string Attribute2
        //{
        //    get
        //    {
        //        return _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute2.ToString()].ToString();
        //    }
        //    private set
        //    {
        //        _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute2.ToString()] = value;
        //    }
        //}

        //public string Attribute3
        //{
        //    get
        //    {
        //        return _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute3.ToString()].ToString();
        //    }
        //    private set
        //    {
        //        _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute3.ToString()] = value;
        //    }
        //}

        //public string Attribute4
        //{
        //    get
        //    {
        //        return _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute4.ToString()].ToString();
        //    }
        //    private set
        //    {
        //        _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute4.ToString()] = value;
        //    }
        //}

        //public string Attribute5
        //{
        //    get
        //    {
        //        return _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute5.ToString()].ToString();
        //    }
        //    private set
        //    {
        //        _NodeTypePropRow[CswEnumNbtNodeTypePropAttributes.attribute5.ToString()] = value;
        //    }
        //}

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

        public void setFilter( CswNbtMetaDataNodeTypeProp FilterProp, CswEnumNbtSubFieldName SubFieldName, CswEnumNbtFilterMode FilterMode, object FilterValue )
        {
            if( IsRequired )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Required properties cannot be conditional", "User attempted to set a conditional filter on a required property" );
            }
            DesignNode.DisplayConditionProperty.RelatedNodeId = FilterProp.DesignNode.NodeId;
            DesignNode.DisplayConditionSubfield.Value = SubFieldName.ToString();
            DesignNode.DisplayConditionFilterMode.Value = FilterMode.ToString();
            DesignNode.DisplayConditionValue.Text = FilterValue.ToString();
            DesignNode.postChanges( false );
        }

        public void getFilter( ref CswNbtSubField SubField, ref CswEnumNbtFilterMode FilterMode, ref string FilterValue )
        {
            CswNbtMetaDataNodeTypeProp FilterNodeTypeProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( FilterNodeTypePropId );
            if( FilterNodeTypeProp != null )
            {
                SubField = FilterNodeTypeProp.getFieldTypeRule().SubFields[(CswEnumNbtSubFieldName) _NodeTypePropRow["filtersubfield"].ToString()];
                FilterMode = (CswEnumNbtFilterMode) _NodeTypePropRow["filtermode"].ToString();
                FilterValue = _NodeTypePropRow["filtervalue"].ToString();
            }
        }

        public bool hasFilter()
        {
            return ( _NodeTypePropRow["filtersubfield"].ToString() != string.Empty &&
                     _NodeTypePropRow["filtermode"].ToString() != string.Empty &&
                     _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( FilterNodeTypePropId ) != null );
        }


        public bool CheckFilter( CswNbtNode Node )
        {
            bool FilterMatches = false;

            CswNbtMetaDataNodeTypeProp FilterMetaDataProp = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeProp( this.FilterNodeTypePropId );
            if( null != FilterMetaDataProp )
            {
                CswNbtSubField SubField = FilterMetaDataProp.getFieldTypeRule().SubFields.Default;
                CswEnumNbtFilterMode FilterMode = SubField.DefaultFilterMode;
                string FilterValue = null;
                getFilter( ref SubField, ref FilterMode, ref FilterValue );
                if( FilterMode != CswEnumNbtFilterMode.Unknown )
                {
                    CswNbtNodePropWrapper FilterProp = Node.Properties[FilterMetaDataProp];

                    // Logical needs a special case
                    if( FilterMetaDataProp.getFieldTypeValue() == CswEnumNbtFieldType.Logical )
                    {
                        if( SubField.Name == CswNbtFieldTypeRuleLogical.SubFieldName.Checked )
                        {
                            if( FilterMode == CswEnumNbtFilterMode.Equals )
                            {
                                FilterMatches = ( CswConvert.ToTristate( FilterValue ) == FilterProp.AsLogical.Checked );
                            }
                            else if( FilterMode == CswEnumNbtFilterMode.NotEquals )
                            {
                                FilterMatches = ( CswConvert.ToTristate( FilterValue ) != FilterProp.AsLogical.Checked );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "Invalid filter condition", "CswNbtMetaDataNodeTypeProp only supports 'Checked Equality' filters on Logical properties" );
                        }
                    }
                    else
                    {
                        string ValueToCompare = string.Empty;
                        //switch( FilterMetaDataProp.getFieldTypeValue() )
                        //{
                        //    case CswEnumNbtFieldType.List:
                        //        ValueToCompare = FilterProp.AsList.Value;
                        //        break;
                        //    case CswEnumNbtFieldType.Static:
                        //        ValueToCompare = FilterProp.AsStatic.StaticText;
                        //        break;
                        //    case CswEnumNbtFieldType.Text:
                        //        ValueToCompare = FilterProp.AsText.Text;
                        //        break;
                        //    case CswEnumNbtFieldType.Relationship:
                        //        if( null != FilterProp.AsRelationship.RelatedNodeId )
                        //        {
                        //            ValueToCompare = FilterProp.AsRelationship.RelatedNodeId.PrimaryKey.ToString();
                        //        }
                        //        break;
                        //    case CswEnumNbtFieldType.MetaDataList:
                        //        ValueToCompare = FilterProp.AsMetaDataList.Text;
                        //        break;
                        //    default:
                        //        throw new CswDniException( CswEnumErrorType.Error, "Invalid filter condition", "CheckFilter does not support field type: " + FilterMetaDataProp.getFieldTypeValue().ToString() );
                        //} // switch( FilterMetaDataProp.FieldType.FieldType )

                        dynamic SubfieldVal = FilterProp.GetSubFieldValue( SubField.Name );
                        if( null != SubfieldVal )
                        {
                            ValueToCompare = CswConvert.ToString( SubfieldVal );
                        }

                        if( FilterMode == CswEnumNbtFilterMode.Equals )
                        {
                            FilterMatches = ( ValueToCompare.ToLower() == FilterValue.ToLower() );
                        }
                        else if( FilterMode == CswEnumNbtFilterMode.NotEquals )
                        {
                            FilterMatches = ( ValueToCompare.ToLower() != FilterValue.ToLower() );
                        }
                        else if( FilterMode == CswEnumNbtFilterMode.Null )
                        {
                            FilterMatches = ( ValueToCompare == string.Empty );
                        }
                        else if( FilterMode == CswEnumNbtFilterMode.NotNull )
                        {
                            FilterMatches = ( ValueToCompare != string.Empty );
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "Invalid filter condition", "CheckFilter does not support filter mode: " + FilterMode.ToString() );
                        } // switch( FilterMode )

                    } // if-else( FilterMetaDataProp.FieldType.FieldType == CswEnumNbtFieldType.Logical )
                } // if(FilterMode != CswEnumNbtFilterMode.Unknown)
            } // if( null != FilterMetaDataProp )
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
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypePropLatestVersion( PropId );
        }


        /// <summary>
        /// Returns whether the value of this property should be allowed to be copied to other nodes
        /// </summary>
        public bool IsCopyable()
        {
            return ( false == IsUnique() &&
                false == ReadOnly &&
                false == IsCompoundUnique() &&
                getFieldType().IsCopyable() );
        }
        /// <summary>
        /// Returns whether this property can be deleted
        /// </summary>
        public bool IsDeletable()
        {
            return ( ObjectClassPropId == Int32.MinValue );
        }

        public CswNbtObjClassDesignSequence Sequence
        {
            get
            {
                CswNbtObjClassDesignSequence ret = null;
                if( null != this.DesignNode &&
                    this.DesignNode.AttributeProperty.ContainsKey( CswEnumNbtPropertyAttributeName.Sequence ) )
                {
                    CswPrimaryKey SequenceId = this.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.Sequence].AsRelationship.RelatedNodeId;
                    if( CswTools.IsPrimaryKey( SequenceId ) )
                    {
                        ret = _CswNbtMetaDataResources.CswNbtResources.Nodes[SequenceId];
                    }
                }
                return ret;
            }
        } // Sequence

        //public static string _Element_MetaDataNodeTypeProp = "MetaDataNodeTypeProp";
        //public static string _Element_DefaultValue = "DefaultValue";
        //public static string _Element_SubFieldMap = "SubFieldMap";
        //public static string _Attribute_NodeTypePropId = "nodetypepropid";
        //public static string _Attribute_JctNodePropId = "jctnodepropid";
        //public static string _Attribute_NodeTypePropName = "nodetypepropname";
        //public static string _Attribute_nodetypeid = "nodetypeid";
        //public static string _Attribute_nodetypetabid = "nodetypetabid";
        //public static string _Attribute_usenumbering = "usenumbering";
        //public static string _Attribute_questionno = "questionno";
        //public static string _Attribute_subquestionNo = "subquestionno";
        //public static string _Attribute_fieldtype = "fieldtype";
        //public static string _Attribute_readonly = "readonly";
        //public static string _Attribute_isrequired = "isrequired";
        //public static string _Attribute_isunique = "isunique";
        //public static string _Attribute_datetoday = "datetoday";
        //public static string _Attribute_listoptions = "listoptions";
        //public static string _Attribute_numberprecision = "numberprecision";
        //public static string _Attribute_minvalue = "minvalue";
        //public static string _Attribute_maxvalue = "maxvalue";
        //public static string _Attribute_statictext = "statictext";
        //public static string _Attribute_filterpropid = "filterpropid";
        //public static string _Attribute_filter = "filter";
        //public static string _Attribute_firstpropversionid = "firstpropversionid";
        //public static string _Attribute_SubFieldName = "SubFieldName";
        //public static string _Attribute_RelationalTable = "RelationalTable";
        //public static string _Attribute_RelationalColumn = "RelationalColumn";
        //public static string _Attribute_attribute1 = "attribute1";
        //public static string _Attribute_attribute2 = "attribute2";
        //public static string _Attribute_attribute3 = "attribute3";
        //public static string _Attribute_attribute4 = "attribute4";
        //public static string _Attribute_attribute5 = "attribute5";

        //public XmlNode ToXml( XmlDocument XmlDoc, bool UseQuestionNo )
        //{
        //    CswNbtMetaDataFieldType thisFieldType = getFieldType();

        //    XmlNode PropNode = XmlDoc.CreateNode( XmlNodeType.Element, "MetaDataNodeTypeProp", "" );

        //    XmlAttribute PropIdAttr = XmlDoc.CreateAttribute( _Attribute_NodeTypePropId );
        //    PropIdAttr.Value = PropId.ToString();
        //    PropNode.Attributes.Append( PropIdAttr );

        //    XmlAttribute NameAttr = XmlDoc.CreateAttribute( _Attribute_NodeTypePropName );
        //    if( UseQuestionNo )
        //    {
        //        NameAttr.Value = PropNameWithQuestionNo;
        //    }
        //    else    // BZ 8644
        //    {
        //        NameAttr.Value = PropName;
        //    }
        //    PropNode.Attributes.Append( NameAttr );

        //    XmlAttribute NodeTypeIdAttr = XmlDoc.CreateAttribute( _Attribute_nodetypeid );
        //    NodeTypeIdAttr.Value = NodeTypeId.ToString();
        //    PropNode.Attributes.Append( NodeTypeIdAttr );

        //    XmlAttribute FirstPropVersionIdAttr = XmlDoc.CreateAttribute( _Attribute_firstpropversionid ); //bz # 8016
        //    FirstPropVersionIdAttr.Value = FirstPropVersionId.ToString();
        //    PropNode.Attributes.Append( FirstPropVersionIdAttr );

        //    XmlAttribute UseNumberingAttr = XmlDoc.CreateAttribute( _Attribute_usenumbering );
        //    UseNumberingAttr.Value = UseNumbering.ToString();
        //    PropNode.Attributes.Append( UseNumberingAttr );
        //    XmlAttribute QuestionNoAttr = XmlDoc.CreateAttribute( _Attribute_questionno );
        //    PropNode.Attributes.Append( QuestionNoAttr );
        //    XmlAttribute SubQuestionNoAttr = XmlDoc.CreateAttribute( _Attribute_subquestionNo );
        //    PropNode.Attributes.Append( SubQuestionNoAttr );
        //    if( UseNumbering )
        //    {
        //        QuestionNoAttr.Value = QuestionNo.ToString();
        //        SubQuestionNoAttr.Value = SubQuestionNo.ToString();
        //    }
        //    else
        //    {
        //        QuestionNoAttr.Value = string.Empty;
        //        SubQuestionNoAttr.Value = string.Empty;
        //    }

        //    XmlAttribute FieldTypeAttr = XmlDoc.CreateAttribute( _Attribute_fieldtype );
        //    FieldTypeAttr.Value = thisFieldType.FieldType.ToString();
        //    PropNode.Attributes.Append( FieldTypeAttr );

        //    //bz #7632: Locations should be editable
        //    XmlAttribute ReadOnlyAttr = XmlDoc.CreateAttribute( _Attribute_readonly );
        //    if( UseQuestionNo && ( ( CswEnumNbtFieldType.Location != thisFieldType.FieldType ) && false == thisFieldType.IsSimpleType() ) )
        //        ReadOnlyAttr.Value = "true";
        //    else
        //        ReadOnlyAttr.Value = ReadOnly.ToString().ToLower();
        //    PropNode.Attributes.Append( ReadOnlyAttr );

        //    XmlAttribute IsRequiredAttr = XmlDoc.CreateAttribute( _Attribute_isrequired );
        //    IsRequiredAttr.Value = IsRequired.ToString().ToLower();
        //    PropNode.Attributes.Append( IsRequiredAttr );

        //    XmlAttribute IsUniqueAttr = XmlDoc.CreateAttribute( _Attribute_isunique );
        //    IsUniqueAttr.Value = IsUnique().ToString().ToLower();
        //    PropNode.Attributes.Append( IsUniqueAttr );

        //    XmlAttribute DateTodayAttr = XmlDoc.CreateAttribute( _Attribute_datetoday );
        //    DateTodayAttr.Value = DateToday.ToString();
        //    PropNode.Attributes.Append( DateTodayAttr );

        //    XmlAttribute ListOptionsAttr = XmlDoc.CreateAttribute( _Attribute_listoptions );
        //    ListOptionsAttr.Value = ListOptions;
        //    PropNode.Attributes.Append( ListOptionsAttr );

        //    XmlAttribute NumberPrecisionAttr = XmlDoc.CreateAttribute( _Attribute_numberprecision );
        //    PropNode.Attributes.Append( NumberPrecisionAttr );
        //    if( NumberPrecision > 0 )
        //        NumberPrecisionAttr.Value = NumberPrecision.ToString();
        //    else
        //        NumberPrecisionAttr.Value = Int32.MinValue.ToString();

        //    XmlAttribute MinValueAttr = XmlDoc.CreateAttribute( _Attribute_minvalue );
        //    PropNode.Attributes.Append( MinValueAttr );
        //    if( MinValue != Int32.MinValue )
        //        MinValueAttr.Value = MinValue.ToString();
        //    else
        //        MinValueAttr.Value = Int32.MinValue.ToString();

        //    XmlAttribute MaxValueAttr = XmlDoc.CreateAttribute( _Attribute_maxvalue );
        //    PropNode.Attributes.Append( MaxValueAttr );
        //    if( MaxValue != Int32.MinValue )
        //        MaxValueAttr.Value = MaxValue.ToString();
        //    else
        //        MaxValueAttr.Value = Int32.MinValue.ToString();

        //    XmlAttribute StaticTextAttr = XmlDoc.CreateAttribute( _Attribute_statictext );
        //    StaticTextAttr.Value = StaticText;
        //    PropNode.Attributes.Append( StaticTextAttr );

        //    XmlAttribute Attribute1Attr = XmlDoc.CreateAttribute( _Attribute_attribute1 );
        //    Attribute1Attr.Value = Attribute1;
        //    PropNode.Attributes.Append( Attribute1Attr );

        //    XmlAttribute Attribute2Attr = XmlDoc.CreateAttribute( _Attribute_attribute2 );
        //    Attribute2Attr.Value = Attribute2;
        //    PropNode.Attributes.Append( Attribute2Attr );

        //    XmlAttribute Attribute3Attr = XmlDoc.CreateAttribute( _Attribute_attribute3 );
        //    Attribute3Attr.Value = Attribute3;
        //    PropNode.Attributes.Append( Attribute3Attr );

        //    XmlAttribute Attribute4Attr = XmlDoc.CreateAttribute( _Attribute_attribute4 );
        //    Attribute4Attr.Value = Attribute4;
        //    PropNode.Attributes.Append( Attribute4Attr );

        //    XmlAttribute Attribute5Attr = XmlDoc.CreateAttribute( _Attribute_attribute5 );
        //    Attribute5Attr.Value = Attribute5;
        //    PropNode.Attributes.Append( Attribute5Attr );

        //    // Default value is a subnode, not an attribute
        //    if( HasDefaultValue() )
        //    {
        //        XmlNode DefaultValueNode = XmlDoc.CreateElement( _Element_DefaultValue );
        //        DefaultValue.ToXml( DefaultValueNode );
        //        PropNode.AppendChild( DefaultValueNode );
        //    }

        //    //// Jct_dd_ntp value is a set of subnodes, not attributes
        //    //foreach( CswNbtSubField SubField in _CswNbtMetaDataResources.CswNbtMetaData.getFieldTypeRule( thisFieldType.FieldType ).SubFields )
        //    //{
        //    //    if( SubField.RelationalTable != string.Empty )
        //    //    {
        //    //        XmlNode SubFieldMapNode = XmlDoc.CreateElement( _Element_SubFieldMap );
        //    //        PropNode.AppendChild( SubFieldMapNode );

        //    //        XmlAttribute SubFieldNameAttr = XmlDoc.CreateAttribute( _Attribute_SubFieldName );
        //    //        SubFieldNameAttr.Value = SubField.ToXmlNodeName();
        //    //        SubFieldMapNode.Attributes.Append( SubFieldNameAttr );

        //    //        XmlAttribute RTableAttr = XmlDoc.CreateAttribute( _Attribute_RelationalTable );
        //    //        RTableAttr.Value = SubField.RelationalTable;
        //    //        SubFieldMapNode.Attributes.Append( RTableAttr );

        //    //        XmlAttribute RColumnAttr = XmlDoc.CreateAttribute( _Attribute_RelationalColumn );
        //    //        RColumnAttr.Value = SubField.RelationalColumn;
        //    //        SubFieldMapNode.Attributes.Append( RColumnAttr );

        //    //    } // if( SubField.RelationalTable != string.Empty )
        //    //} // foreach( CswNbtSubField SubField in this.FieldTypeRule.SubFields )

        //    XmlAttribute FilterAttr = XmlDoc.CreateAttribute( _Attribute_filter );
        //    PropNode.Attributes.Append( FilterAttr );
        //    XmlAttribute FilterPropIdAttr = XmlDoc.CreateAttribute( _Attribute_filterpropid );
        //    PropNode.Attributes.Append( FilterPropIdAttr );
        //    if( hasFilter() )
        //    {
        //        FilterPropIdAttr.Value = FilterNodeTypePropId.ToString();
        //        FilterAttr.Value = getFilterString();
        //    }
        //    else
        //    {
        //        FilterPropIdAttr.Value = string.Empty;
        //        FilterAttr.Value = string.Empty;
        //    }

        //    return PropNode;
        //}

        //public void SetFromXmlDataRow( DataRow PropXmlRow )
        //{
        //    this.DateToday = CswConvert.ToBoolean( PropXmlRow[_Attribute_datetoday] );
        //    this.IsRequired = CswConvert.ToBoolean( PropXmlRow[_Attribute_isrequired] );
        //    if( PropXmlRow.Table.Columns.Contains( _Attribute_isunique ) )
        //    {
        //        this.setIsUnique( CswConvert.ToBoolean( PropXmlRow[_Attribute_isunique] ) );
        //    }
        //    this.ListOptions = PropXmlRow[_Attribute_listoptions].ToString();
        //    this.MaxValue = CswConvert.ToDouble( PropXmlRow[_Attribute_maxvalue] );
        //    this.MinValue = CswConvert.ToDouble( PropXmlRow[_Attribute_minvalue] );
        //    this.NumberPrecision = CswConvert.ToInt32( PropXmlRow[_Attribute_numberprecision] );
        //    this.UseNumbering = CswConvert.ToBoolean( PropXmlRow[_Attribute_usenumbering] );
        //    this.QuestionNo = CswConvert.ToInt32( PropXmlRow[_Attribute_questionno] );
        //    this.SubQuestionNo = CswConvert.ToInt32( PropXmlRow[_Attribute_subquestionNo] );
        //    this.ReadOnly = CswConvert.ToBoolean( PropXmlRow[_Attribute_readonly] );
        //    this.StaticText = PropXmlRow[_Attribute_statictext].ToString();
        //}


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
            if( null != FirstEditLayout && null != OtherNodeTypeProp.FirstEditLayout )
            {
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
            }
            return ret;
        }


        #endregion IComparable

        public bool IsNodeReference()
        {
            return ( ( this.getFieldTypeValue() == CswEnumNbtFieldType.Relationship ) ||
                     ( this.getFieldTypeValue() == CswEnumNbtFieldType.Location ) ||
                     ( this.getFieldTypeValue() == CswEnumNbtFieldType.Quantity ) );
        }

        public bool IsUserRelationship()
        {
            bool ret = false;
            if( this.getFieldTypeValue() == CswEnumNbtFieldType.Relationship )
            {
                CswNbtMetaDataObjectClass UserOC = _CswNbtMetaDataResources.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
                ret = FkMatches( UserOC );
            }
            return ret;
        }

        //public bool IsUserRelationshipDeprecated()
        //{
        //    bool ret = false;
        //    if( this.getFieldTypeValue() == CswEnumNbtFieldType.Relationship )
        //    {
        //        CswNbtMetaDataObjectClass UserOC = _CswNbtMetaDataResources.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
        //        ret = FkMatchesDeprecated( UserOC );
        //    }
        //    return ret;
        //}

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> Attributes = new Collection<CswNbtFieldTypeAttribute>();
            foreach( CswNbtFieldTypeAttribute attribute in getFieldTypeRule().getAttributes() )
            {
                if( _DataRow.Table.Columns.Contains( attribute.Column ) )
                {
                    attribute.Value = _DataRow[attribute.Column].ToString();
                }
                Attributes.Add( attribute );
            }
            return Attributes;
        } // getAttributes()
    }
}
