using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    [Serializable()]
    public class CswNbtViewRelationship : CswNbtViewNode, IEquatable<CswNbtViewRelationship>, IComparable
    {
        public override NbtViewNodeType ViewNodeType { get { return NbtViewNodeType.CswNbtViewRelationship; } }

        // For the Relationship
        public bool Selectable = true;
        //public bool ShowInGrid = true;
        public bool ShowInTree = true;

        public bool AllowView = true;
        public bool AllowEdit = true;
        public bool AllowDelete = true;

        private Int32 _PropId = Int32.MinValue;
        private NbtViewPropIdType _PropType = NbtViewPropIdType.NodeTypePropId;
        private NbtViewPropOwnerType _PropOwner = NbtViewPropOwnerType.First;
        private string _PropName = "";
        private Int32 _FirstId = Int32.MinValue;
        private string _FirstName = "";
        private NbtViewRelatedIdType _FirstType = NbtViewRelatedIdType.NodeTypeId;
        private Int32 _SecondId = Int32.MinValue;
        private string _SecondName = "";
        private NbtViewRelatedIdType _SecondType = NbtViewRelatedIdType.NodeTypeId;
        private string _SecondIconFileName = "blank.gif";
        private Int32 _GroupByPropId = Int32.MinValue;
        private NbtViewPropIdType _GroupByPropType = NbtViewPropIdType.NodeTypePropId;
        private string _GroupByPropName = "";

        private const string _ChildRelationshipsName = "childrelationships";
        private const string _PropertiesName = "properties";

        public NbtViewPropIdType PropType { get { return _PropType; } }
        public NbtViewPropOwnerType PropOwner { get { return _PropOwner; } }
        public Int32 PropId { get { return _PropId; } }
        public string PropName { get { return _PropName; } }
        public Int32 FirstId { get { return _FirstId; } }
        public string FirstName { get { return _FirstName; } }
        public NbtViewRelatedIdType FirstType { get { return _FirstType; } }
        public Int32 SecondId { get { return _SecondId; } }
        public string SecondName { get { return _SecondName; } }
        public NbtViewRelatedIdType SecondType { get { return _SecondType; } }
        public string SecondIconFileName { get { return _SecondIconFileName; } }
        public Int32 GroupByPropId { get { return _GroupByPropId; } }
        public NbtViewPropIdType GroupByPropType { get { return _GroupByPropType; } }
        public string GroupByPropName { get { return _GroupByPropName; } }

        public bool isExpectedMetaDataType( CswNbtMetaDataNodeType NodeType )
        {
            return ( null != NodeType &&
                     SecondType == NbtViewRelatedIdType.NodeTypeId &&
                     SecondId == NodeType.getFirstVersionNodeType().NodeTypeId );
        }

        public ICswNbtMetaDataObject SecondMetaDataObject()
        {
            ICswNbtMetaDataObject Ret = null;
            if( Int32.MinValue != SecondId )
            {
                if( SecondType == NbtViewRelatedIdType.ObjectClassId )
                {
                    Ret = _CswNbtResources.MetaData.getObjectClass( SecondId );
                }
                else if( SecondType == NbtViewRelatedIdType.NodeTypeId )
                {
                    Ret = _CswNbtResources.MetaData.getNodeType( SecondId );
                }
            }
            return Ret;
        }

        public bool isExpectedMetaDataType( CswNbtMetaDataObjectClass ObjectClass )
        {
            bool Ret = ( null != ObjectClass &&
                         SecondType == NbtViewRelatedIdType.ObjectClassId &&
                         SecondId == ObjectClass.ObjectClassId );
            if( false == Ret &&
                null != ObjectClass &&
                SecondType == NbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( SecondId );
                Ret = ( null != NodeType && NodeType.ObjectClassId == ObjectClass.ObjectClassId );
            }
            return Ret;
        }

        #region Relationship internals

        public void overrideFirst( CswNbtMetaDataNodeType NodeType )
        {
            if( NodeType != null )
            {
                setFirst( NbtViewRelatedIdType.NodeTypeId, NodeType.FirstVersionNodeTypeId, NodeType.getNodeTypeLatestVersion().NodeTypeName );
            }
            else
            {
                setFirst( NbtViewRelatedIdType.Unknown, Int32.MinValue, string.Empty );
            }
        }
        public void overrideFirst( CswNbtMetaDataObjectClass ObjectClass )
        {
            if( ObjectClass != null )
            {
                setFirst( NbtViewRelatedIdType.ObjectClassId, ObjectClass.ObjectClassId, ObjectClass.ObjectClass.ToString() );
            }
            else
            {
                setFirst( NbtViewRelatedIdType.Unknown, Int32.MinValue, string.Empty );
            }
        }
        private void setFirst( NbtViewRelatedIdType InFirstType, Int32 InFirstId, string InFirstName )
        {
            _FirstType = InFirstType;
            _FirstId = InFirstId;
            if( InFirstId > 0 && InFirstType == NbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType FirstNodeType = _CswNbtResources.MetaData.getNodeType( InFirstId );
                if( FirstNodeType != null )
                    _FirstId = FirstNodeType.FirstVersionNodeTypeId;
            }
            _FirstName = InFirstName;
        }

        public void overrideSecond( CswNbtMetaDataNodeType NodeType )
        {
            if( null != NodeType ) /* Case 25943 */
            {
                setSecond( NbtViewRelatedIdType.NodeTypeId, NodeType.FirstVersionNodeTypeId, NodeType.getNodeTypeLatestVersion().NodeTypeName, NodeType.getNodeTypeLatestVersion().IconFileName );
            }
            else
            {
                setSecond( NbtViewRelatedIdType.NodeTypeId, Int32.MinValue, string.Empty, string.Empty );
            }
        }
        public void overrideSecond( CswNbtMetaDataObjectClass ObjectClass )
        {
            if( null != ObjectClass ) /* Case 25943 */
            {
                setSecond( NbtViewRelatedIdType.ObjectClassId, ObjectClass.ObjectClassId, ObjectClass.ObjectClass.ToString(), ObjectClass.IconFileName );
            }
            else
            {
                setSecond( NbtViewRelatedIdType.ObjectClassId, Int32.MinValue, string.Empty, string.Empty );
            }
        }
        private void setSecond( NbtViewRelatedIdType InSecondType, Int32 InSecondId, string InSecondName, string InIconFileName )
        {
            _SecondType = InSecondType;
            _SecondId = InSecondId;
            if( InSecondId > 0 && InSecondType == NbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType SecondNodeType = _CswNbtResources.MetaData.getNodeType( InSecondId );
                if( SecondNodeType != null )
                    _SecondId = SecondNodeType.FirstVersionNodeTypeId;
            }
            _SecondName = InSecondName;
            if( InIconFileName.ToLower().StartsWith( "images/" ) )
            {
                //string IconFileNamePrefix = CswNbtMetaDataObjectClass.IconPrefix16;
                //if( InIconFileName.Length > IconFileNamePrefix.Length &&
                //    InIconFileName.Substring( 0, IconFileNamePrefix.Length ) == IconFileNamePrefix )
                //{
                _SecondIconFileName = InIconFileName;
            }
            else if( false == string.IsNullOrEmpty( InIconFileName ) )
            {
                _SecondIconFileName = CswNbtMetaDataObjectClass.IconPrefix16 + InIconFileName;
            }
        }

        public void overrideProp( NbtViewPropOwnerType InOwnerType, CswNbtMetaDataNodeTypeProp Prop )
        {
            setProp( InOwnerType, Prop );
        }

        private void setProp( NbtViewPropOwnerType InOwnerType, CswNbtMetaDataNodeTypeProp Prop )
        {
            CswNbtMetaDataFieldType FieldType = Prop.getFieldType();
            if( FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Location )
            {
                throw new CswDniException( ErrorType.Error, "Illegal view setting", "Views must be built from Relationship or Location properties" );
            }

            setPropValue( InOwnerType, NbtViewPropIdType.NodeTypePropId, Prop.FirstPropVersionId, Prop.getNodeTypePropLatestVersion().PropName );

            if( InOwnerType == NbtViewPropOwnerType.First )
            {
                overrideFirst( Prop.getNodeType() );
                if( Prop.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Prop.FKValue );
                    if( null != NodeType )
                    {
                        overrideSecond( NodeType );
                    }
                }
                else if( Prop.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() )
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Prop.FKValue );
                    if( null != ObjectClass )
                    {
                        overrideSecond( ObjectClass );
                    }
                }
            }
            else if( InOwnerType == NbtViewPropOwnerType.Second )
            {
                if( Prop.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Prop.FKValue );
                    if( null != NodeType )
                    {
                        overrideFirst( NodeType );
                    }
                }
                else if( Prop.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() )
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Prop.FKValue );
                    if( null != ObjectClass )
                    {
                        overrideFirst( ObjectClass );
                    }
                }
                overrideSecond( Prop.getNodeType() );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Illegal view setting", "setProp() got Unknown owner type" );
            }
        }

        private void setProp( NbtViewPropOwnerType InOwnerType, CswNbtMetaDataObjectClassProp Prop )
        {
            CswNbtMetaDataFieldType FieldType = Prop.getFieldType();
            if( FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Location )
            {
                throw new CswDniException( ErrorType.Error, "Illegal view setting", "Views must be built from Relationship or Location properties" );
            }

            setPropValue( InOwnerType, NbtViewPropIdType.ObjectClassPropId, Prop.PropId, Prop.PropName );

            if( InOwnerType == NbtViewPropOwnerType.First )
            {
                overrideFirst( Prop.getObjectClass() );
                overrideSecond( _CswNbtResources.MetaData.getObjectClass( Prop.FKValue ) );
            }
            else if( InOwnerType == NbtViewPropOwnerType.Second )
            {
                overrideFirst( _CswNbtResources.MetaData.getObjectClass( Prop.FKValue ) );
                overrideSecond( Prop.getObjectClass() );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Illegal view setting", "setProp() got Unknown owner type" );
            }
        }

        private void setPropValue( NbtViewPropOwnerType InOwnerType, NbtViewPropIdType InPropType, Int32 InPropId, string InPropName )
        {
            _PropOwner = InOwnerType;
            _PropType = InPropType;
            _PropId = InPropId;
            _PropName = InPropName;
        }

        #endregion Relationship internals


        #region Group By

        public void setGroupByProp( CswNbtMetaDataNodeTypeProp Prop )
        {
            setGroupByProp( NbtViewPropIdType.NodeTypePropId, Prop.FirstPropVersionId, Prop.PropName );
        }
        public void setGroupByProp( CswNbtMetaDataObjectClassProp Prop )
        {
            setGroupByProp( NbtViewPropIdType.ObjectClassPropId, Prop.PropId, Prop.PropName );
        }
        private void setGroupByProp( NbtViewPropIdType InPropType, Int32 InPropId, string InPropName )
        {
            _GroupByPropType = InPropType;
            _GroupByPropId = InPropId;
            _GroupByPropName = InPropName;
        }
        public void clearGroupBy()
        {
            _GroupByPropType = NbtViewPropIdType.Unknown;
            _GroupByPropId = Int32.MinValue;
            _GroupByPropName = string.Empty;
        }

        #endregion Group By


        public Collection<CswPrimaryKey> NodeIdsToFilterIn = new Collection<CswPrimaryKey>();
        public Collection<CswPrimaryKey> NodeIdsToFilterOut = new Collection<CswPrimaryKey>();

        private NbtViewAddChildrenSetting _AddChildren = NbtViewAddChildrenSetting.InView;
        public NbtViewAddChildrenSetting AddChildren
        {
            get { return _AddChildren; }
            set
            {
                // Backwards compatibility
                if( value == NbtViewAddChildrenSetting.True )
                    _AddChildren = NbtViewAddChildrenSetting.InView;
                else if( value == NbtViewAddChildrenSetting.False )
                    _AddChildren = NbtViewAddChildrenSetting.None;
                else
                    _AddChildren = value;
            }
        }

        public override string IconFileName
        {
            get { return SecondIconFileName; }
        }

        #region For the View

        //private string _ArbitraryId = "";
        //public override string ArbitraryId
        //{
        //    get { return _ArbitraryId; }
        //    set { _ArbitraryId = value; }
        //}

        public override string ArbitraryId
        {
            get
            {
                string ArbId = string.Empty;
                if( Parent != null )
                {
                    ArbId += Parent.ArbitraryId + "_";
                }
                if( this.SecondType == NbtViewRelatedIdType.NodeTypeId )
                {
                    ArbId += "NT_";
                }
                else if( this.SecondType == NbtViewRelatedIdType.ObjectClassId )
                {
                    ArbId += "OC_";
                }
                ArbId += SecondId;
                if( Int32.MinValue != this.PropId)
                {
                    ArbId += this.PropId.ToString();
                }
                return ArbId;
            }
        }

        private CswNbtViewNode _Parent;
        public override CswNbtViewNode Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        private Collection<CswNbtViewRelationship> _ChildRelationships = new Collection<CswNbtViewRelationship>();
        public Collection<CswNbtViewRelationship> ChildRelationships
        {
            get { return _ChildRelationships; }
            set { _ChildRelationships = value; }
        }

        private Collection<CswNbtViewProperty> _Properties = new Collection<CswNbtViewProperty>();
        public Collection<CswNbtViewProperty> Properties
        {
            get { return _Properties; }
            set { _Properties = value; }
        }

        public CswNbtViewProperty findProperty( Int32 NodeTypePropId )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewProperty ViewProp in Properties )
            {
                if( ViewProp.NodeTypePropId == NodeTypePropId )
                {
                    ret = ViewProp;
                    break;
                }
            }
            return ret;
        } // findProperty()

        #endregion For the View


        #region Constructors

        /// <summary>
        /// For adding a nodetype to the root level of the view
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, CswNbtMetaDataNodeType NodeType, bool IncludeDefaultFilters )
            : base( CswNbtResources, View )
        {
            overrideSecond( NodeType );

            if( IncludeDefaultFilters )
                _setDefaultFilters();
        }
        /// <summary>
        /// For adding an object class to the root level of the view
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, CswNbtMetaDataObjectClass ObjectClass, bool IncludeDefaultFilters )
            : base( CswNbtResources, View )
        {
            overrideSecond( ObjectClass );

            if( IncludeDefaultFilters )
                _setDefaultFilters();
        }
        /// <summary>
        /// For a relationship below the root level, determined by a nodetype property
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, NbtViewPropOwnerType InOwnerType, CswNbtMetaDataNodeTypeProp Prop, bool IncludeDefaultFilters )
            : base( CswNbtResources, View )
        {
            setProp( InOwnerType, Prop );

            if( IncludeDefaultFilters )
                _setDefaultFilters();
        }
        /// <summary>
        /// For a relationship below the root level, determined by an object class property
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, NbtViewPropOwnerType InOwnerType, CswNbtMetaDataObjectClassProp Prop, bool IncludeDefaultFilters )
            : base( CswNbtResources, View )
        {
            setProp( InOwnerType, Prop );

            if( IncludeDefaultFilters )
                _setDefaultFilters();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, CswNbtViewRelationship CopyFrom, bool IncludeDefaultFilters )
            : base( CswNbtResources, View )
        {
            //ArbitraryId = CopyFrom.ArbitraryId;
            setPropValue( CopyFrom.PropOwner, CopyFrom.PropType, CopyFrom.PropId, CopyFrom.PropName );
            setFirst( CopyFrom.FirstType, CopyFrom.FirstId, CopyFrom.FirstName );
            setSecond( CopyFrom.SecondType, CopyFrom.SecondId, CopyFrom.SecondName, CopyFrom.SecondIconFileName );
            Selectable = CopyFrom.Selectable;
            //ShowInGrid = CopyFrom.ShowInGrid;
            ShowInTree = CopyFrom.ShowInTree;
            AddChildren = CopyFrom.AddChildren;
            AllowView = CopyFrom.AllowView;
            AllowEdit = CopyFrom.AllowEdit;
            AllowDelete = CopyFrom.AllowDelete;
            NodeIdsToFilterIn = CopyFrom.NodeIdsToFilterIn;
            NodeIdsToFilterOut = CopyFrom.NodeIdsToFilterOut;
            setGroupByProp( CopyFrom.GroupByPropType, CopyFrom.GroupByPropId, CopyFrom.GroupByPropName );

            if( IncludeDefaultFilters )
                _setDefaultFilters();
        }

        /// <summary>
        /// Construct from string (made by ToString())
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, CswDelimitedString StringRelationship )
            : base( CswNbtResources, View )
        {
            try
            {
                if( StringRelationship[0] == NbtViewNodeType.CswNbtViewRelationship.ToString() )
                {
                    if( StringRelationship[1] != String.Empty )
                    {
                        //setPropValue( (NbtViewPropOwnerType) Enum.Parse( typeof( NbtViewPropOwnerType ), StringRelationship[4], true ),
                        //              (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), StringRelationship[2], true ),
                        setPropValue( (NbtViewPropOwnerType) StringRelationship[4],
                                      (NbtViewPropIdType) StringRelationship[2],
                                      CswConvert.ToInt32( StringRelationship[1] ),
                                      StringRelationship[3] );
                    }
                    if( StringRelationship[5] != String.Empty )
                    {
                        //setFirst( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), StringRelationship[6], true ),
                        setFirst( (NbtViewRelatedIdType) StringRelationship[6],
                                  CswConvert.ToInt32( StringRelationship[5] ),
                                  StringRelationship[7] );
                    }
                    if( StringRelationship[8] != String.Empty )
                    {
                        //setSecond( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), StringRelationship[9], true ),
                        setSecond( (NbtViewRelatedIdType) StringRelationship[9],
                                   CswConvert.ToInt32( StringRelationship[8] ),
                                   StringRelationship[10],
                                   StringRelationship[11] );
                    }
                    if( StringRelationship[12] != String.Empty )
                        Selectable = Convert.ToBoolean( StringRelationship[12] );
                    //if( StringRelationship[13] != String.Empty )
                    //    ArbitraryId = StringRelationship[13];
                    //if( StringRelationship[14] != String.Empty )
                    //    ShowInGrid = Convert.ToBoolean( StringRelationship[14] );
                    if( StringRelationship[15] != String.Empty )
                    {
                        //AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), StringRelationship[15], true );
                        AddChildren = (NbtViewAddChildrenSetting) StringRelationship[15];
                    }
                    if( StringRelationship[16] != String.Empty )
                        AllowDelete = Convert.ToBoolean( StringRelationship[16] );
                    if( StringRelationship[17] != String.Empty )
                    {
                        string FilterInAttr = StringRelationship[17].ToString();
                        NodeIdsToFilterIn = _commaDelimitedToFilterAttribute( FilterInAttr );
                    }
                    if( StringRelationship[18] != String.Empty )
                    {
                        string FilterOutAttr = StringRelationship[18].ToString();
                        NodeIdsToFilterOut = _commaDelimitedToFilterAttribute( FilterOutAttr );
                    }
                    if( StringRelationship[19] != string.Empty )
                        ShowInTree = Convert.ToBoolean( StringRelationship[19] );
                    if( StringRelationship[20] != string.Empty )
                    {
                        //setGroupByProp( (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), StringRelationship[20], true ),
                        setGroupByProp( (NbtViewPropIdType) StringRelationship[20],
                                        CswConvert.ToInt32( StringRelationship[21] ),
                                        StringRelationship[22] );
                    }
                    if( StringRelationship[23] != String.Empty )
                        AllowView = Convert.ToBoolean( StringRelationship[23] );
                    if( StringRelationship[24] != String.Empty )
                        AllowEdit = Convert.ToBoolean( StringRelationship[24] );
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(string) encountered an invalid property value",
                                          ex );
            }
        }

        /// <summary>
        /// Construct from XML
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, XmlNode RelationshipNode )
            : base( CswNbtResources, View )
        {
            try
            {
                if( RelationshipNode.Attributes[PropIdAttrName] != null )
                {
                    //setPropValue( (NbtViewPropOwnerType) Enum.Parse( typeof( NbtViewPropOwnerType ), RelationshipNode.Attributes[PropOwnerAttrName].Value, true ),
                    //              (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), RelationshipNode.Attributes[PropTypeAttrName].Value, true ),
                    setPropValue( (NbtViewPropOwnerType) RelationshipNode.Attributes[PropOwnerAttrName].Value,
                                  (NbtViewPropIdType) RelationshipNode.Attributes[PropTypeAttrName].Value,
                                  CswConvert.ToInt32( RelationshipNode.Attributes[PropIdAttrName].Value ),
                                  RelationshipNode.Attributes[PropNameAttrName].Value );
                }
                if( RelationshipNode.Attributes[FirstIdAttrName] != null )
                {
                    //setFirst( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), RelationshipNode.Attributes[FirstTypeAttrName].Value, true ),
                    setFirst( (NbtViewRelatedIdType) RelationshipNode.Attributes[FirstTypeAttrName].Value,
                              CswConvert.ToInt32( RelationshipNode.Attributes[FirstIdAttrName].Value ),
                              RelationshipNode.Attributes[FirstNameAttrName].Value );
                }
                if( RelationshipNode.Attributes[SecondIdAttrName] != null )
                {
                    string icon = string.Empty;
                    if( RelationshipNode.Attributes[SecondIconFileNameAttrName] != null )
                        icon = RelationshipNode.Attributes[SecondIconFileNameAttrName].Value;

                    //setSecond( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), RelationshipNode.Attributes[SecondTypeAttrName].Value, true ),
                    setSecond( (NbtViewRelatedIdType) RelationshipNode.Attributes[SecondTypeAttrName].Value,
                               CswConvert.ToInt32( RelationshipNode.Attributes[SecondIdAttrName].Value ),
                               RelationshipNode.Attributes[SecondNameAttrName].Value,
                               icon );
                }
                if( RelationshipNode.Attributes[GroupByPropIdAttrName] != null )
                {
                    if( RelationshipNode.Attributes[GroupByPropTypeAttrName].Value != string.Empty )
                    {
                        //setGroupByProp( (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), RelationshipNode.Attributes[GroupByPropTypeAttrName].Value, true ),
                        setGroupByProp( (NbtViewPropIdType) RelationshipNode.Attributes[GroupByPropTypeAttrName].Value,
                                        CswConvert.ToInt32( RelationshipNode.Attributes[GroupByPropIdAttrName].Value ),
                                        RelationshipNode.Attributes[GroupByPropNameAttrName].Value );
                    }
                    else
                    {
                        clearGroupBy();
                    }
                }

                if( RelationshipNode.Attributes[SelectableAttrName] != null )
                    Selectable = Convert.ToBoolean( RelationshipNode.Attributes[SelectableAttrName].Value );
                //if( RelationshipNode.Attributes[ArbitraryIdAttrName] != null )
                //    ArbitraryId = RelationshipNode.Attributes[ArbitraryIdAttrName].Value;
                //if( RelationshipNode.Attributes[ShowInGridAttrName] != null )
                //    ShowInGrid = Convert.ToBoolean( RelationshipNode.Attributes[ShowInGridAttrName].Value );
                if( RelationshipNode.Attributes[ShowInTreeAttrName] != null )
                    ShowInTree = Convert.ToBoolean( RelationshipNode.Attributes[ShowInTreeAttrName].Value );
                if( RelationshipNode.Attributes[AllowAddChildrenAttrName] != null && RelationshipNode.Attributes[AllowAddChildrenAttrName].Value.ToString() != string.Empty )
                {
                    //AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), RelationshipNode.Attributes[AllowAddChildrenAttrName].Value, true );
                    AddChildren = (NbtViewAddChildrenSetting) RelationshipNode.Attributes[AllowAddChildrenAttrName].Value;
                }

                if( RelationshipNode.Attributes[AllowViewAttrName] != null && RelationshipNode.Attributes[AllowViewAttrName].ToString() != string.Empty )
                    AllowView = Convert.ToBoolean( RelationshipNode.Attributes[AllowViewAttrName].Value );
                if( RelationshipNode.Attributes[AllowEditAttrName] != null && RelationshipNode.Attributes[AllowEditAttrName].ToString() != string.Empty )
                    AllowEdit = Convert.ToBoolean( RelationshipNode.Attributes[AllowEditAttrName].Value );
                if( RelationshipNode.Attributes[AllowDeleteAttrName] != null && RelationshipNode.Attributes[AllowDeleteAttrName].ToString() != string.Empty )
                    AllowDelete = Convert.ToBoolean( RelationshipNode.Attributes[AllowDeleteAttrName].Value );

                if( RelationshipNode.Attributes[NodeIdFilterInAttrName] != null && RelationshipNode.Attributes[NodeIdFilterInAttrName].Value.ToString() != string.Empty )
                {
                    string FilterInAttr = RelationshipNode.Attributes[NodeIdFilterInAttrName].Value.ToString();
                    NodeIdsToFilterIn = _commaDelimitedToFilterAttribute( FilterInAttr );
                }
                if( RelationshipNode.Attributes[NodeIdFilterOutAttrName] != null && RelationshipNode.Attributes[NodeIdFilterOutAttrName].Value.ToString() != string.Empty )
                {
                    string FilterOutAttr = RelationshipNode.Attributes[NodeIdFilterOutAttrName].Value.ToString();
                    NodeIdsToFilterOut = _commaDelimitedToFilterAttribute( FilterOutAttr );
                }

            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                foreach( XmlNode ChildNode in RelationshipNode.ChildNodes )
                {
                    if( ChildNode.Name.ToLower() == NbtViewXmlNodeName.Relationship.ToString().ToLower() )
                    {
                        CswNbtViewRelationship ChildRelationshipNode = new CswNbtViewRelationship( CswNbtResources, _View, ChildNode );
                        this.addChildRelationship( ChildRelationshipNode );
                    }
                    if( ChildNode.Name.ToLower() == NbtViewXmlNodeName.Property.ToString().ToLower() )
                    {
                        CswNbtViewProperty ChildProp = new CswNbtViewProperty( CswNbtResources, _View, ChildNode );
                        this.addProperty( ChildProp );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid child definition",
                                          ex );
            }
        }

        /// <summary>
        /// Construct from JSON
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, JObject RelationshipObj )
            : base( CswNbtResources, View )
        {
            try
            {
                string _PropOwnerAttrName = CswConvert.ToString( RelationshipObj[PropOwnerAttrName] );
                string _PropIdAttrName = CswConvert.ToString( RelationshipObj[PropIdAttrName] );
                string _PropTypeAttrName = CswConvert.ToString( RelationshipObj[PropTypeAttrName] );
                string _PropNameAttrName = CswConvert.ToString( RelationshipObj[PropNameAttrName] );
                if( !string.IsNullOrEmpty( _PropIdAttrName ) )
                {
                    //setPropValue( (NbtViewPropOwnerType) Enum.Parse( typeof( NbtViewPropOwnerType ), _PropOwnerAttrName, true ),
                    //              (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), _PropTypeAttrName, true ),
                    setPropValue( (NbtViewPropOwnerType) _PropOwnerAttrName,
                                  (NbtViewPropIdType) _PropTypeAttrName,
                                  CswConvert.ToInt32( _PropIdAttrName ),
                                  _PropNameAttrName );
                }

                string _FirstTypeAttrName = CswConvert.ToString( RelationshipObj[FirstTypeAttrName] );
                string _FirstNameAttrName = CswConvert.ToString( RelationshipObj[FirstNameAttrName] );
                string _FirstIdAttrName = CswConvert.ToString( RelationshipObj[FirstIdAttrName] );
                if( !string.IsNullOrEmpty( _FirstIdAttrName ) )
                {
                    //setFirst( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), _FirstTypeAttrName, true ),
                    setFirst( (NbtViewRelatedIdType) _FirstTypeAttrName,
                              CswConvert.ToInt32( _FirstIdAttrName ),
                              _FirstNameAttrName );
                }

                string _SecondIdAttrName = CswConvert.ToString( RelationshipObj[SecondIdAttrName] );
                string _SecondIconFileNameAttrName = CswConvert.ToString( RelationshipObj[SecondIconFileNameAttrName] );
                string _SecondTypeAttrName = CswConvert.ToString( RelationshipObj[SecondTypeAttrName] );
                string _SecondNameAttrName = CswConvert.ToString( RelationshipObj[SecondNameAttrName] );
                if( !string.IsNullOrEmpty( _SecondIdAttrName ) )
                {
                    string icon = string.Empty;
                    if( !string.IsNullOrEmpty( _SecondIconFileNameAttrName ) )
                    {
                        icon = _SecondIconFileNameAttrName;
                    }

                    //setSecond( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), _SecondTypeAttrName, true ),
                    setSecond( (NbtViewRelatedIdType) _SecondTypeAttrName,
                               CswConvert.ToInt32( _SecondIdAttrName ),
                               _SecondNameAttrName,
                               icon );
                }

                string _GroupByPropIdAttrName = CswConvert.ToString( RelationshipObj[GroupByPropIdAttrName] );
                string _GroupByPropTypeAttrName = CswConvert.ToString( RelationshipObj[GroupByPropTypeAttrName] );
                string _GroupByPropNameAttrName = CswConvert.ToString( RelationshipObj[GroupByPropNameAttrName] );
                if( !string.IsNullOrEmpty( _GroupByPropIdAttrName ) )
                {
                    if( !string.IsNullOrEmpty( _GroupByPropTypeAttrName ) )
                    {
                        //setGroupByProp( (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), _GroupByPropTypeAttrName, true ),
                        setGroupByProp( (NbtViewPropIdType) _GroupByPropTypeAttrName,
                                        CswConvert.ToInt32( _GroupByPropIdAttrName ),
                                        _GroupByPropNameAttrName );
                    }
                    else
                    {
                        clearGroupBy();
                    }
                }

                if( RelationshipObj[SelectableAttrName] != null )
                {
                    Selectable = CswConvert.ToBoolean( RelationshipObj[SelectableAttrName] );
                }

                if( RelationshipObj[ShowInTreeAttrName] != null )
                {
                    ShowInTree = CswConvert.ToBoolean( RelationshipObj[ShowInTreeAttrName] );
                }

                string _AllowAddChildrenAttrName = CswConvert.ToString( RelationshipObj[AllowAddChildrenAttrName] );
                if( !string.IsNullOrEmpty( _AllowAddChildrenAttrName ) )
                {
                    //AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), _AllowAddChildrenAttrName, true );
                    AddChildren = (NbtViewAddChildrenSetting) _AllowAddChildrenAttrName;
                }

                if( RelationshipObj[AllowViewAttrName] != null )
                {
                    AllowView = CswConvert.ToBoolean( RelationshipObj[AllowViewAttrName] );
                }
                if( RelationshipObj[AllowEditAttrName] != null )
                {
                    AllowEdit = CswConvert.ToBoolean( RelationshipObj[AllowEditAttrName] );
                }
                if( RelationshipObj[AllowDeleteAttrName] != null )
                {
                    AllowDelete = CswConvert.ToBoolean( RelationshipObj[AllowDeleteAttrName] );
                }

                string _NodeIdFilterInAttrName = CswConvert.ToString( RelationshipObj[NodeIdFilterInAttrName] );
                if( !string.IsNullOrEmpty( _NodeIdFilterInAttrName ) )
                {
                    NodeIdsToFilterIn = _commaDelimitedToFilterAttribute( NodeIdFilterInAttrName );
                }

                string _NodeIdFilterOutAttrName = CswConvert.ToString( RelationshipObj[NodeIdFilterOutAttrName] );
                if( !string.IsNullOrEmpty( _NodeIdFilterOutAttrName ) )
                {
                    NodeIdsToFilterOut = _commaDelimitedToFilterAttribute( _NodeIdFilterOutAttrName );
                }

            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                JProperty Children = RelationshipObj.Property( _ChildRelationshipsName );
                if( null != Children )
                {
                    JObject Relationships = (JObject) Children.Value;
                    foreach( CswNbtViewRelationship ChildRelationship in
                        from Relationship
                            in Relationships.Properties()
                        select (JObject) Relationship.Value
                            into ChildRelationObj
                            let NodeName = CswConvert.ToString( ChildRelationObj["nodename"] )
                            where NodeName == NbtViewXmlNodeName.Relationship.ToString().ToLower()
                            select new CswNbtViewRelationship( CswNbtResources, _View, ChildRelationObj ) )
                    {
                        this.addChildRelationship( ChildRelationship );
                    }
                }

                JProperty Properties = RelationshipObj.Property( _PropertiesName );
                if( null != Properties )
                {
                    JObject PropsObj = (JObject) Properties.Value;
                    foreach( CswNbtViewProperty ChildProp in
                        from Property
                            in PropsObj.Properties()
                        select (JObject) Property.Value
                            into PropObj
                            let NodeName = CswConvert.ToString( PropObj["nodename"] )
                            where NodeName == NbtViewXmlNodeName.Property.ToString().ToLower()
                            select new CswNbtViewProperty( CswNbtResources, _View, PropObj ) )
                    {
                        this.addProperty( ChildProp );
                    }
                }

            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid child definition",
                                          ex );
            }
        }

        private void _setDefaultFilters()
        {
            CswNbtMetaDataObjectClass DefaultFilterOC = null;
            if( SecondType == NbtViewRelatedIdType.ObjectClassId )
            {
                DefaultFilterOC = _CswNbtResources.MetaData.getObjectClass( SecondId );
            }
            else if( SecondType == NbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType DefaultFilterNT = _CswNbtResources.MetaData.getNodeType( SecondId );
                if( DefaultFilterNT != null )
                    DefaultFilterOC = DefaultFilterNT.getObjectClass();
            }
            if( DefaultFilterOC != null )
            {
                CswNbtObjClass DefaultFilterObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, DefaultFilterOC );
                DefaultFilterObjClass.addDefaultViewFilters( this );
            }
        }

        #endregion Constructors

        #region Exporters

        public override string ToString()
        {
            return ToDelimitedString().ToString();
        }

        public CswDelimitedString ToDelimitedString()
        {
            CswDelimitedString ret = new CswDelimitedString( CswNbtView.delimiter );
            ret.Add( NbtViewNodeType.CswNbtViewRelationship.ToString() );
            if( PropId != Int32.MinValue )
            {
                ret.Add( PropId.ToString() );
                ret.Add( PropType.ToString() );
                ret.Add( PropName.ToString() );
                ret.Add( PropOwner.ToString() );
            }
            else
            {
                ret.Add( "" );
                ret.Add( "" );
                ret.Add( "" );
                ret.Add( "" );
            }
            if( FirstId != Int32.MinValue )
            {
                ret.Add( FirstId.ToString() );
                ret.Add( FirstType.ToString() );
                ret.Add( FirstName.ToString() );
            }
            else
            {
                ret.Add( "" );
                ret.Add( "" );
                ret.Add( "" );
            }
            if( SecondId != Int32.MinValue )
            {
                ret.Add( SecondId.ToString() );
                ret.Add( SecondType.ToString() );
                ret.Add( SecondName.ToString() );
                ret.Add( SecondIconFileName.ToString() );
            }
            else
            {
                ret.Add( "" );
                ret.Add( "" );
                ret.Add( "" );
                ret.Add( "" );
            }
            ret.Add( Selectable.ToString().ToLower() );
            ret.Add( ArbitraryId.ToString() );
            ret.Add( "" ); // ShowInGrid.ToString().ToLower();
            ret.Add( AddChildren.ToString() );
            ret.Add( AllowDelete.ToString() );

            CswCommaDelimitedString FilterInString = new CswCommaDelimitedString();
            foreach( CswPrimaryKey child in NodeIdsToFilterIn )
                FilterInString.Add( child.ToString() );
            ret.Add( FilterInString.ToString() );

            CswCommaDelimitedString FilterOutString = new CswCommaDelimitedString();
            foreach( CswPrimaryKey child in NodeIdsToFilterOut )
                FilterOutString.Add( child.ToString() );
            ret.Add( FilterOutString.ToString() );

            ret.Add( ShowInTree.ToString().ToLower() );
            if( GroupByPropId != Int32.MinValue )
            {
                ret.Add( GroupByPropType.ToString() );
                ret.Add( GroupByPropId.ToString() );
                ret.Add( GroupByPropName );
            }
            else
            {
                ret.Add( "" );
                ret.Add( "" );
                ret.Add( "" );
            }
            ret.Add( AllowView.ToString() );
            ret.Add( AllowEdit.ToString() );
            return ret;
        }

        public static string PropIdAttrName = "propid";
        public static string PropNameAttrName = "propname";
        public static string PropTypeAttrName = "proptype";
        public static string PropOwnerAttrName = "propowner";
        public static string GroupByPropIdAttrName = "groupbypropid";
        public static string GroupByPropNameAttrName = "groupbypropname";
        public static string GroupByPropTypeAttrName = "groupbyproptype";
        public static string FirstNameAttrName = "firstname";
        public static string FirstTypeAttrName = "firsttype";
        public static string FirstIdAttrName = "firstid";
        public static string SecondNameAttrName = "secondname";
        public static string SecondTypeAttrName = "secondtype";
        public static string SecondIdAttrName = "secondid";
        public static string SecondIconFileNameAttrName = "secondiconfilename";
        public static string SelectableAttrName = "selectable";
        public static string ArbitraryIdAttrName = "arbitraryid";
        //public static string ShowInGridAttrName = "showingrid";
        public static string ShowInTreeAttrName = "showintree";
        public static string AllowAddChildrenAttrName = "allowaddchildren";
        public static string AllowViewAttrName = "allowview";
        public static string AllowEditAttrName = "allowedit";
        public static string AllowDeleteAttrName = "allowdelete";
        public static string NodeIdFilterInAttrName = "nodeidstofilterin";
        public static string NodeIdFilterOutAttrName = "nodeidstofilterout";

        public XmlNode ToXml( XmlDocument XmlDoc )
        {
            XmlNode RelationshipNode = XmlDoc.CreateNode( XmlNodeType.Element, NbtViewXmlNodeName.Relationship.ToString(), "" );

            if( PropId != Int32.MinValue )
            {
                XmlAttribute PropIdAttr = XmlDoc.CreateAttribute( PropIdAttrName );
                PropIdAttr.Value = PropId.ToString();
                RelationshipNode.Attributes.Append( PropIdAttr );

                XmlAttribute PropNameAttr = XmlDoc.CreateAttribute( PropNameAttrName );
                PropNameAttr.Value = PropName.ToString();
                RelationshipNode.Attributes.Append( PropNameAttr );

                XmlAttribute PropTypeAttr = XmlDoc.CreateAttribute( PropTypeAttrName );
                PropTypeAttr.Value = PropType.ToString();
                RelationshipNode.Attributes.Append( PropTypeAttr );

                XmlAttribute PropOwnerAttr = XmlDoc.CreateAttribute( PropOwnerAttrName );
                PropOwnerAttr.Value = PropOwner.ToString();
                RelationshipNode.Attributes.Append( PropOwnerAttr );
            }

            if( GroupByPropId != Int32.MinValue )
            {
                XmlAttribute GroupByPropIdAttr = XmlDoc.CreateAttribute( GroupByPropIdAttrName );
                GroupByPropIdAttr.Value = GroupByPropId.ToString();
                RelationshipNode.Attributes.Append( GroupByPropIdAttr );

                XmlAttribute GroupByPropNameAttr = XmlDoc.CreateAttribute( GroupByPropNameAttrName );
                GroupByPropNameAttr.Value = GroupByPropName.ToString();
                RelationshipNode.Attributes.Append( GroupByPropNameAttr );

                XmlAttribute GroupByPropTypeAttr = XmlDoc.CreateAttribute( GroupByPropTypeAttrName );
                GroupByPropTypeAttr.Value = GroupByPropType.ToString();
                RelationshipNode.Attributes.Append( GroupByPropTypeAttr );
            }

            if( FirstId != Int32.MinValue )
            {
                XmlAttribute FirstNameAttr = XmlDoc.CreateAttribute( FirstNameAttrName );
                FirstNameAttr.Value = FirstName.ToString();
                RelationshipNode.Attributes.Append( FirstNameAttr );

                XmlAttribute FirstTypeAttr = XmlDoc.CreateAttribute( FirstTypeAttrName );
                FirstTypeAttr.Value = FirstType.ToString();
                RelationshipNode.Attributes.Append( FirstTypeAttr );

                XmlAttribute FirstIdAttr = XmlDoc.CreateAttribute( FirstIdAttrName );
                FirstIdAttr.Value = FirstId.ToString();
                RelationshipNode.Attributes.Append( FirstIdAttr );
            }

            if( SecondId != Int32.MinValue )
            {
                XmlAttribute SecondNameAttr = XmlDoc.CreateAttribute( SecondNameAttrName );
                SecondNameAttr.Value = SecondName.ToString();
                RelationshipNode.Attributes.Append( SecondNameAttr );

                XmlAttribute SecondTypeAttr = XmlDoc.CreateAttribute( SecondTypeAttrName );
                SecondTypeAttr.Value = SecondType.ToString();
                RelationshipNode.Attributes.Append( SecondTypeAttr );

                XmlAttribute SecondIdAttr = XmlDoc.CreateAttribute( SecondIdAttrName );
                SecondIdAttr.Value = SecondId.ToString();
                RelationshipNode.Attributes.Append( SecondIdAttr );

                XmlAttribute SecondIconFileNameAttr = XmlDoc.CreateAttribute( SecondIconFileNameAttrName );
                SecondIconFileNameAttr.Value = SecondIconFileName.ToString();
                RelationshipNode.Attributes.Append( SecondIconFileNameAttr );

            }

            XmlAttribute SelectableAttr = XmlDoc.CreateAttribute( SelectableAttrName );
            SelectableAttr.Value = Selectable.ToString().ToLower();
            RelationshipNode.Attributes.Append( SelectableAttr );

            XmlAttribute ArbitraryIdAttr = XmlDoc.CreateAttribute( ArbitraryIdAttrName );
            ArbitraryIdAttr.Value = ArbitraryId.ToString();
            RelationshipNode.Attributes.Append( ArbitraryIdAttr );

            //XmlAttribute ShowInGridAttr = XmlDoc.CreateAttribute( ShowInGridAttrName );
            //ShowInGridAttr.Value = ShowInGrid.ToString().ToLower();
            //RelationshipNode.Attributes.Append( ShowInGridAttr );

            XmlAttribute ShowInTreeAttr = XmlDoc.CreateAttribute( ShowInTreeAttrName );
            ShowInTreeAttr.Value = ShowInTree.ToString().ToLower();
            RelationshipNode.Attributes.Append( ShowInTreeAttr );

            XmlAttribute AllowAddChildrenAttr = XmlDoc.CreateAttribute( AllowAddChildrenAttrName );
            AllowAddChildrenAttr.Value = AddChildren.ToString();
            RelationshipNode.Attributes.Append( AllowAddChildrenAttr );

            XmlAttribute AllowViewAttr = XmlDoc.CreateAttribute( AllowViewAttrName );
            AllowViewAttr.Value = AllowView.ToString();
            RelationshipNode.Attributes.Append( AllowViewAttr );

            XmlAttribute AllowEditAttr = XmlDoc.CreateAttribute( AllowEditAttrName );
            AllowEditAttr.Value = AllowEdit.ToString();
            RelationshipNode.Attributes.Append( AllowEditAttr );

            XmlAttribute AllowDeleteAttr = XmlDoc.CreateAttribute( AllowDeleteAttrName );
            AllowDeleteAttr.Value = AllowDelete.ToString();
            RelationshipNode.Attributes.Append( AllowDeleteAttr );

            XmlAttribute NodeIdFilterInAttribute = XmlDoc.CreateAttribute( NodeIdFilterInAttrName );
            //NodeIdFilterInAttribute.Value = CswTools.IntCollectionToDelimitedString( NodeIdsToFilterIn, ',', false );
            string FilterInString = "";
            bool bFirst = true;
            foreach( CswPrimaryKey child in NodeIdsToFilterIn )
            {
                if( null != child )
                {
                    if( !bFirst ) FilterInString += ','.ToString();
                    FilterInString += child.ToString();
                    bFirst = false;
                }
            }
            NodeIdFilterInAttribute.Value = FilterInString;
            RelationshipNode.Attributes.Append( NodeIdFilterInAttribute );

            XmlAttribute NodeIdFilterOutAttribute = XmlDoc.CreateAttribute( NodeIdFilterOutAttrName );
            //NodeIdFilterOutAttribute.Value = CswTools.IntCollectionToDelimitedString( NodeIdsToFilterOut, ',', false );
            string FilterOutString = "";
            bFirst = true;
            foreach( CswPrimaryKey child in NodeIdsToFilterOut )
            {
                if( !bFirst ) FilterOutString += ','.ToString();
                FilterOutString += child.ToString();
                bFirst = false;
            }
            NodeIdFilterOutAttribute.Value = FilterOutString;
            RelationshipNode.Attributes.Append( NodeIdFilterOutAttribute );

            // Handle props and propfilters
            foreach( CswNbtViewProperty Prop in this.Properties )
            {
                XmlNode PropXmlNode = Prop.ToXml( XmlDoc );
                RelationshipNode.AppendChild( PropXmlNode );
            }

            // Recurse on child ViewNodes
            foreach( CswNbtViewRelationship ChildRelationship in this.ChildRelationships )
            {
                XmlNode ChildXmlNode = ChildRelationship.ToXml( XmlDoc );
                RelationshipNode.AppendChild( ChildXmlNode );
            }

            return RelationshipNode;
        }

        public JProperty ToJson( string RPropName = null, bool FirstLevelOnly = false )
        {
            JObject RelationshipObj = new JObject();

            RelationshipObj["nodename"] = NbtViewXmlNodeName.Relationship.ToString().ToLower();

            string RelationshipId = string.Empty;
            if( PropId != Int32.MinValue )
            {
                RelationshipId = PropId.ToString();
                RelationshipObj[PropIdAttrName] = PropId.ToString();
                RelationshipObj[PropNameAttrName] = PropName.ToString();
                RelationshipObj[PropTypeAttrName] = PropType.ToString();
                RelationshipObj[PropOwnerAttrName] = PropOwner.ToString();
            }

            if( GroupByPropId != Int32.MinValue )
            {
                RelationshipObj[GroupByPropIdAttrName] = GroupByPropId.ToString();
                RelationshipObj[GroupByPropNameAttrName] = GroupByPropName.ToString();
                RelationshipObj[GroupByPropTypeAttrName] = GroupByPropType.ToString();
            }

            if( FirstId != Int32.MinValue )
            {
                if( string.IsNullOrEmpty( RelationshipId ) )
                {
                    RelationshipId = FirstId.ToString();
                }
                RelationshipObj[FirstNameAttrName] = FirstName.ToString();
                RelationshipObj[FirstTypeAttrName] = FirstType.ToString();
                RelationshipObj[FirstIdAttrName] = FirstId.ToString();
            }

            if( SecondId != Int32.MinValue )
            {
                if( string.IsNullOrEmpty( RelationshipId ) )
                {
                    RelationshipId = SecondId.ToString();
                }
                RelationshipObj[SecondNameAttrName] = SecondName.ToString();
                RelationshipObj[SecondTypeAttrName] = SecondType.ToString();
                RelationshipObj[SecondIdAttrName] = SecondId.ToString();
                RelationshipObj[SecondIconFileNameAttrName] = SecondIconFileName.ToString();
            }

            RelationshipObj[SelectableAttrName] = Selectable.ToString().ToLower();
            RelationshipObj[ArbitraryIdAttrName] = ArbitraryId.ToString();
            RelationshipObj[ShowInTreeAttrName] = ShowInTree.ToString().ToLower();
            RelationshipObj[AllowAddChildrenAttrName] = AddChildren.ToString();
            RelationshipObj[AllowViewAttrName] = AllowView.ToString();
            RelationshipObj[AllowEditAttrName] = AllowEdit.ToString();
            RelationshipObj[AllowDeleteAttrName] = AllowDelete.ToString();

            string FilterInString = "";
            bool bFirst = true;
            foreach( CswPrimaryKey Child in NodeIdsToFilterIn.Where( Child => null != Child ) )
            {
                if( !bFirst ) FilterInString += ','.ToString();
                FilterInString += Child.ToString();
                bFirst = false;
            }
            RelationshipObj[NodeIdFilterInAttrName] = FilterInString;

            string FilterOutString = "";
            bFirst = true;
            foreach( CswPrimaryKey Child in NodeIdsToFilterOut )
            {
                if( !bFirst ) FilterOutString += ','.ToString();
                FilterOutString += Child.ToString();
                bFirst = false;
            }
            RelationshipObj[NodeIdFilterOutAttrName] = FilterOutString;

            if( !FirstLevelOnly )
            {
                // Handle props and propfilters
                JObject PropObj = new JObject();
                RelationshipObj[_PropertiesName] = PropObj;
                foreach( CswNbtViewProperty Prop in this.Properties )
                {
                    JProperty PropProperty = Prop.ToJson();
                    if( null == PropObj[PropProperty.Name] )
                    {
                        PropObj.Add( PropProperty );
                    }
                }

                // Recurse on child ViewNodes
                JObject ChildObj = new JObject();
                RelationshipObj[_ChildRelationshipsName] = ChildObj;
                foreach( CswNbtViewRelationship ChildRelationship in this.ChildRelationships )
                {
                    JProperty ChildRelationshipProperty = ChildRelationship.ToJson();
                    if( null == ChildObj[ChildRelationshipProperty.Name] )
                    {
                        ChildObj.Add( ChildRelationshipProperty );
                    }
                }
                if( string.IsNullOrEmpty( RPropName ) )
                {
                    RPropName = NbtViewXmlNodeName.Relationship.ToString() + "_" + RelationshipId; ;
                }
            }
            JProperty RelationshipProp = new JProperty( RPropName, RelationshipObj );
            return RelationshipProp;
        }

        #endregion Exporters

        #region Child relationships and properties

        public void addProperty( CswNbtViewProperty Prop )
        {
            Properties.Add( Prop );
            Prop.Parent = this;
        }
        public void removeProperty( CswNbtViewProperty Prop )
        {
            //Remove matching properties, not necessarily exact
            ArrayList PropsToRemove = new ArrayList();
            foreach( CswNbtViewProperty CurrentProp in Properties )
            {
                if( CurrentProp.CompareTo( Prop ) == 0 )
                {
                    PropsToRemove.Add( CurrentProp );
                }
            }
            foreach( CswNbtViewProperty CurrentProp in PropsToRemove )
            {
                Properties.Remove( CurrentProp );
                CurrentProp.Parent = null;
            }
        }




        public void addChildRelationship( CswNbtViewRelationship ChildRelationship )
        {
            ChildRelationships.Add( ChildRelationship );
            ChildRelationship.Parent = this;
        }
        public void removeChildRelationship( CswNbtViewRelationship ChildRelationship )
        {
            ChildRelationships.Remove( ChildRelationship );
            ChildRelationship.Parent = null;
        }

        #endregion Child relationships and properties

        public override string TextLabel
        {
            get
            {
                string NodeText = SecondName;
                if( PropName != String.Empty )
                {
                    if( PropOwner == NbtViewPropOwnerType.First )
                        NodeText += " (by " + FirstName + "'s " + PropName + ")";
                    else
                        NodeText += " (by " + PropName + ")";
                }
                return NodeText;
            }
        }

        #region IComparable

        public int CompareTo( object obj )
        {
            if( obj is CswNbtViewRelationship )
                return CompareTo( (CswNbtViewRelationship) obj );
            else
                throw new CswDniException( ErrorType.Error, "Illegal comparison", "Can't compare CswNbtViewRelationship to object: " + obj.ToString() );
        }

        public int CompareTo( CswNbtViewRelationship rel )
        {
            int ret = int.MinValue;
            if( this == rel )
                ret = 0;
            else
                ret = this.SecondName.CompareTo( rel );

            return ret;
        }

        #endregion IComparable

        #region IEquatable
        public static bool operator ==( CswNbtViewRelationship r1, CswNbtViewRelationship r2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( r1, r2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) r1 == null ) || ( (object) r2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( r1.FirstType == r2.FirstType &&
                r1.FirstId == r2.FirstId &&
                r1.SecondType == r2.SecondType &&
                r1.SecondId == r2.SecondId &&
                r1.PropId == r2.PropId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtViewRelationship r1, CswNbtViewRelationship r2 )
        {
            return !( r1 == r2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtViewRelationship ) )
                return false;
            return this == (CswNbtViewRelationship) obj;
        }

        public bool Equals( CswNbtViewRelationship obj )
        {
            return this == (CswNbtViewRelationship) obj;
        }

        public override int GetHashCode()
        {
            return SecondId;
        }
        #endregion IEquatable



        private Collection<CswPrimaryKey> _commaDelimitedToFilterAttribute( string CommaDelimitedFilters )
        {
            Collection<CswPrimaryKey> Collection = new Collection<CswPrimaryKey>();
            if( string.Empty != CommaDelimitedFilters )
            {
                //if( !CswTools.IsDelimitedStringAllIntegers( FilterInAttr, ',' ) )
                //    throw ( new CswDniException( "Invalid View", "nodeidstofilterin attribute contains non-numeric value: " + FilterInAttr ) );
                CommaDelimitedFilters = CommaDelimitedFilters.Replace( "\"", "" );
                //NodeIdsToFilterIn = CswTools.DelimitedStringToIntCollection( FilterInAttr, ',' );
                string[] splitinput = CommaDelimitedFilters.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                foreach( string child in splitinput )
                {
                    CswPrimaryKey ThisPk = new CswPrimaryKey();
                    ThisPk.FromString( child );
                    Collection.Add( ThisPk );
                }
            }
            return Collection;
        }

    } // class CswNbtViewRelationship

} // namespace ChemSW.Nbt
