using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    [DataContract]
    public class CswNbtViewRelationship: CswNbtViewNode, IEquatable<CswNbtViewRelationship>, IComparable
    {
        public override CswEnumNbtViewNodeType ViewNodeType
        {
            get { return CswEnumNbtViewNodeType.CswNbtViewRelationship; }
            set { CswEnumNbtViewNodeType DummyVal = value; }
        }

        // For the Relationship
        public bool Selectable = true;
        //public bool ShowInGrid = true;
        public bool ShowInTree = true;

        public bool AllowView = true;
        public bool AllowEdit = true;
        public bool AllowDelete = true;

        /// <summary>
        /// Whether or not the user can add a Node of the NodeType (set using the AddChildren prop, None = false, anything else = true)
        /// </summary>
        public bool AllowAdd
        {
            get
            {
                return ( CswEnumNbtViewAddChildrenSetting.None == AddChildren ) ? false : true; //28663 - If "None" then false, otherwise true
            }
        }

        [DataMember]
        private Int32 _PropId = Int32.MinValue;
        [DataMember]
        private CswEnumNbtViewPropIdType _PropType = CswEnumNbtViewPropIdType.NodeTypePropId;
        [DataMember]
        private CswEnumNbtViewPropOwnerType _PropOwner = CswEnumNbtViewPropOwnerType.First;
        [DataMember]
        private string _PropName = "";
        [DataMember]
        private Int32 _FirstId = Int32.MinValue;
        [DataMember]
        private string _FirstName = "";
        [DataMember]
        private CswEnumNbtViewRelatedIdType _FirstType = CswEnumNbtViewRelatedIdType.NodeTypeId;
        [DataMember]
        private Int32 _SecondId = Int32.MinValue;
        [DataMember]
        private string _SecondName = "";
        [DataMember]
        private CswEnumNbtViewRelatedIdType _SecondType = CswEnumNbtViewRelatedIdType.NodeTypeId;
        [DataMember]
        private string _SecondIconFileName;
        [DataMember]
        private Int32 _GroupByPropId = Int32.MinValue;
        [DataMember]
        private CswEnumNbtViewPropIdType _GroupByPropType = CswEnumNbtViewPropIdType.NodeTypePropId;
        [DataMember]
        private string _GroupByPropName = "";

        [DataMember]
        private const string _ChildRelationshipsName = "childrelationships";
        [DataMember]
        private const string _PropertiesName = "properties";

        [DataMember]
        public CswEnumNbtViewPropIdType PropType { get { return _PropType; } set { CswEnumNbtViewPropIdType DummyVal = value; } }
        [DataMember]
        public CswEnumNbtViewPropOwnerType PropOwner { get { return _PropOwner; } set { CswEnumNbtViewPropOwnerType DummyVal = value; } }
        [DataMember]
        public Int32 PropId { get { return _PropId; } set { int DummyVal = value; } }
        [DataMember]
        public string PropName { get { return _PropName; } set { string DummyVal = value; } }
        [DataMember]
        public Int32 FirstId { get { return _FirstId; } set { int DummyVal = value; } }
        [DataMember]
        public string FirstName { get { return _FirstName; } set { string DummyVal = value; } }
        [DataMember]
        public CswEnumNbtViewRelatedIdType FirstType { get { return _FirstType; } set { CswEnumNbtViewRelatedIdType DummyVal = value; } }
        [DataMember]
        public Int32 SecondId { get { return _SecondId; } set { int DummyVal = value; } }
        [DataMember]
        public string SecondName { get { return _SecondName; } set { string DummyVal = value; } }
        [DataMember]
        public CswEnumNbtViewRelatedIdType SecondType { get { return _SecondType; } set { CswEnumNbtViewRelatedIdType DummyVal = value; } }
        [DataMember]
        public string SecondIconFileName
        {
            get
            {
                if( null == _SecondIconFileName )
                {
                    setSecondIconFile();
                }
                return _SecondIconFileName;
            }
            set { string DummyVal = value; }
        }
        [DataMember]
        public Int32 GroupByPropId { get { return _GroupByPropId; } set { int DummyVal = value; } }
        [DataMember]
        public CswEnumNbtViewPropIdType GroupByPropType { get { return _GroupByPropType; } set { CswEnumNbtViewPropIdType DummyVal = value; } }
        [DataMember]
        public string GroupByPropName { get { return _GroupByPropName; } set { string DummyVal = value; } }

        public ICswNbtMetaDataDefinitionObject SecondMetaDataDefinitionObject()
        {
            ICswNbtMetaDataDefinitionObject Ret = null;
            if( Int32.MinValue != SecondId )
            {
                if( SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {
                    Ret = _CswNbtResources.MetaData.getObjectClass( SecondId );
                }
                else if( SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    Ret = _CswNbtResources.MetaData.getNodeType( SecondId );
                }
                else if( SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                {
                    Ret = _CswNbtResources.MetaData.getPropertySet( SecondId );
                }
            }
            return Ret;
        }

        #region Relationship internals

        public void overrideFirst( CswNbtMetaDataNodeType NodeType )
        {
            if( NodeType != null )
            {
                setFirst( CswEnumNbtViewRelatedIdType.NodeTypeId, NodeType.FirstVersionNodeTypeId, NodeType.getNodeTypeLatestVersion().NodeTypeName );
            }
            else
            {
                setFirst( CswEnumNbtViewRelatedIdType.Unknown, Int32.MinValue, string.Empty );
            }
        }
        public void overrideFirst( CswNbtMetaDataObjectClass ObjectClass )
        {
            if( ObjectClass != null )
            {
                setFirst( CswEnumNbtViewRelatedIdType.ObjectClassId, ObjectClass.ObjectClassId, ObjectClass.ObjectClass.ToString() );
            }
            else
            {
                setFirst( CswEnumNbtViewRelatedIdType.Unknown, Int32.MinValue, string.Empty );
            }
        }
        public void overrideFirst( CswNbtMetaDataPropertySet PropertySet )
        {
            if( PropertySet != null )
            {
                setFirst( CswEnumNbtViewRelatedIdType.PropertySetId, PropertySet.PropertySetId, PropertySet.Name.ToString() );
            }
            else
            {
                setFirst( CswEnumNbtViewRelatedIdType.Unknown, Int32.MinValue, string.Empty );
            }
        }
        private void setFirst( CswEnumNbtViewRelatedIdType InFirstType, Int32 InFirstId, string InFirstName )
        {
            _FirstType = InFirstType;
            _FirstId = InFirstId;
            if( InFirstId > 0 && InFirstType == CswEnumNbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType FirstNodeType = _CswNbtResources.MetaData.getNodeType( InFirstId );
                if( FirstNodeType != null )
                    _FirstId = FirstNodeType.FirstVersionNodeTypeId;
            }
            _FirstName = InFirstName;
        }

        public void overrideSecond( CswEnumNbtViewRelatedIdType InSecondType, Int32 InSecondId )
        {
            if( InSecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
            {
                overrideSecond( _CswNbtResources.MetaData.getPropertySet( InSecondId ) );
            }
            else if( InSecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
            {
                overrideSecond( _CswNbtResources.MetaData.getObjectClass( InSecondId ) );
            }
            else if( InSecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
            {
                overrideSecond( _CswNbtResources.MetaData.getNodeType( InSecondId ) );
            }
        }

        public void overrideSecond( CswNbtMetaDataNodeType NodeType )
        {
            if( null != NodeType ) /* Case 25943 */
            {
                setSecond( CswEnumNbtViewRelatedIdType.NodeTypeId, NodeType.FirstVersionNodeTypeId, NodeType.getNodeTypeLatestVersion().NodeTypeName, NodeType.getNodeTypeLatestVersion().IconFileName );
            }
            else
            {
                setSecond( CswEnumNbtViewRelatedIdType.NodeTypeId, Int32.MinValue, string.Empty, string.Empty );
            }
        }
        public void overrideSecond( CswNbtMetaDataObjectClass ObjectClass )
        {
            if( null != ObjectClass ) /* Case 25943 */
            {
                setSecond( CswEnumNbtViewRelatedIdType.ObjectClassId, ObjectClass.ObjectClassId, ObjectClass.ObjectClass.ToString(), ObjectClass.IconFileName );
            }
            else
            {
                setSecond( CswEnumNbtViewRelatedIdType.ObjectClassId, Int32.MinValue, string.Empty, string.Empty );
            }
        }
        public void overrideSecond( CswNbtMetaDataPropertySet PropertySet )
        {
            if( null != PropertySet ) /* Case 25943 */
            {
                setSecond( CswEnumNbtViewRelatedIdType.PropertySetId, PropertySet.PropertySetId, PropertySet.Name.ToString(), PropertySet.IconFileName );
            }
            else
            {
                setSecond( CswEnumNbtViewRelatedIdType.PropertySetId, Int32.MinValue, string.Empty, string.Empty );
            }
        }
        private void setSecond( CswEnumNbtViewRelatedIdType InSecondType, Int32 InSecondId, string InSecondName, string InIconFileName )
        {
            _SecondType = InSecondType;
            _SecondId = InSecondId;
            if( InSecondId > 0 && InSecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType SecondNodeType = _CswNbtResources.MetaData.getNodeType( InSecondId );
                if( SecondNodeType != null )
                    _SecondId = SecondNodeType.FirstVersionNodeTypeId;
            }
            _SecondName = InSecondName;
            //this might be deprecated
            if( InIconFileName.ToLower().StartsWith( "images/" ) )
            {
                _SecondIconFileName = InIconFileName;
            }
            else if( false == string.IsNullOrEmpty( InIconFileName ) )
            {
                _SecondIconFileName = CswNbtMetaDataObjectClass.IconPrefix16 + InIconFileName;
            }

            setSecondIconFile();
        }

        public string setSecondIconFile()
        {
            if( SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType RootNT = _CswNbtResources.MetaData.getNodeType( SecondId );
                if( RootNT != null )
                {
                    _SecondIconFileName = RootNT.IconFileName;
                }
            }
            else if( SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
            {
                CswNbtMetaDataObjectClass RootOC = _CswNbtResources.MetaData.getObjectClass( SecondId );
                if( RootOC != null )
                {
                    _SecondIconFileName = RootOC.IconFileName;
                }
            }
            else if( SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
            {
                CswNbtMetaDataPropertySet RootPS = _CswNbtResources.MetaData.getPropertySet( SecondId );
                if( RootPS != null )
                {
                    _SecondIconFileName = RootPS.IconFileName;
                }
            }
            return _SecondIconFileName;
        }

        public void overrideProp( CswEnumNbtViewPropOwnerType InOwnerType, CswNbtMetaDataNodeTypeProp Prop )
        {
            setProp( InOwnerType, Prop );
        }

        private void setProp( CswEnumNbtViewPropOwnerType InOwnerType, ICswNbtMetaDataProp Prop )
        {
            CswNbtMetaDataFieldType FieldType = Prop.getFieldType();
            if( FieldType.FieldType != CswEnumNbtFieldType.Relationship &&
                FieldType.FieldType != CswEnumNbtFieldType.Location )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Illegal view setting", "Views must be built from Relationship or Location properties" );
            }

            if( Prop is CswNbtMetaDataNodeTypeProp )
            {
                CswNbtMetaDataNodeTypeProp NodeTypeProp = (CswNbtMetaDataNodeTypeProp) Prop;

                setPropValue( InOwnerType, CswEnumNbtViewPropIdType.NodeTypePropId, Prop.FirstPropVersionId, NodeTypeProp.getNodeTypePropLatestVersion().PropName );

                if( InOwnerType == CswEnumNbtViewPropOwnerType.First )
                {
                    overrideFirst( NodeTypeProp.getNodeType() );
                    if( Prop.FKType == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() )
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Prop.FKValue );
                        if( null != NodeType )
                        {
                            overrideSecond( NodeType );
                        }
                    }
                    else if( Prop.FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                    {
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Prop.FKValue );
                        if( null != ObjectClass )
                        {
                            overrideSecond( ObjectClass );
                        }
                    }
                    else if( Prop.FKType == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                    {
                        CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( Prop.FKValue );
                        if( null != PropertySet )
                        {
                            overrideSecond( PropertySet );
                        }
                    }

                }
                else if( InOwnerType == CswEnumNbtViewPropOwnerType.Second )
                {
                    if( Prop.FKType == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() )
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Prop.FKValue );
                        if( null != NodeType )
                        {
                            overrideFirst( NodeType );
                        }
                    }
                    else if( Prop.FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                    {
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Prop.FKValue );
                        if( null != ObjectClass )
                        {
                            overrideFirst( ObjectClass );
                        }
                    }
                    else if( Prop.FKType == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                    {
                        CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( Prop.FKValue );
                        if( null != PropertySet )
                        {
                            overrideFirst( PropertySet );
                        }
                    }
                    overrideSecond( NodeTypeProp.getNodeType() );
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Illegal view setting", "setProp() got Unknown owner type" );
                }
            }
            else if( Prop is CswNbtMetaDataObjectClassProp )
            {
                CswNbtMetaDataObjectClassProp ObjectClassProp = (CswNbtMetaDataObjectClassProp) Prop;
                setPropValue( InOwnerType, CswEnumNbtViewPropIdType.ObjectClassPropId, Prop.PropId, Prop.PropName );

                if( InOwnerType == CswEnumNbtViewPropOwnerType.First )
                {
                    overrideFirst( ObjectClassProp.getObjectClass() );
                    if( Prop.FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                    {
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Prop.FKValue );
                        if( null != ObjectClass )
                        {
                            overrideSecond( ObjectClass );
                        }
                    }
                    else if( Prop.FKType == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                    {
                        CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( Prop.FKValue );
                        if( null != PropertySet )
                        {
                            overrideSecond( PropertySet );
                        }
                    }
                }
                else if( InOwnerType == CswEnumNbtViewPropOwnerType.Second )
                {
                    overrideSecond( ObjectClassProp.getObjectClass() );
                    if( Prop.FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                    {
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Prop.FKValue );
                        if( null != ObjectClass )
                        {
                            overrideFirst( ObjectClass );
                        }
                    }
                    else if( Prop.FKType == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                    {
                        CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( Prop.FKValue );
                        if( null != PropertySet )
                        {
                            overrideFirst( PropertySet );
                        }
                    }
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Illegal view setting", "setProp() got Unknown owner type" );
                }
            }
        } // setProp()

        private void setPropValue( CswEnumNbtViewPropOwnerType InOwnerType, CswEnumNbtViewPropIdType InPropType, Int32 InPropId, string InPropName )
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
            setGroupByProp( CswEnumNbtViewPropIdType.NodeTypePropId, Prop.FirstPropVersionId, Prop.PropName );
        }
        public void setGroupByProp( CswNbtMetaDataObjectClassProp Prop )
        {
            setGroupByProp( CswEnumNbtViewPropIdType.ObjectClassPropId, Prop.PropId, Prop.PropName );
        }
        private void setGroupByProp( CswEnumNbtViewPropIdType InPropType, Int32 InPropId, string InPropName )
        {
            _GroupByPropType = InPropType;
            _GroupByPropId = InPropId;
            _GroupByPropName = InPropName;
        }
        public void clearGroupBy()
        {
            _GroupByPropType = CswEnumNbtViewPropIdType.Unknown;
            _GroupByPropId = Int32.MinValue;
            _GroupByPropName = string.Empty;
        }

        #endregion Group By


        public Collection<CswPrimaryKey> NodeIdsToFilterIn = new Collection<CswPrimaryKey>();
        public Collection<CswPrimaryKey> NodeIdsToFilterOut = new Collection<CswPrimaryKey>();

        private CswEnumNbtViewAddChildrenSetting _AddChildren = CswEnumNbtViewAddChildrenSetting.InView;
        public CswEnumNbtViewAddChildrenSetting AddChildren
        {
            get { return _AddChildren; }
            set
            {
                // Backwards compatibility
                if( value == CswEnumNbtViewAddChildrenSetting.True )
                    _AddChildren = CswEnumNbtViewAddChildrenSetting.InView;
                else if( value == CswEnumNbtViewAddChildrenSetting.False )
                    _AddChildren = CswEnumNbtViewAddChildrenSetting.None;
                else
                    _AddChildren = value;
            }
        }

        [DataMember]
        public override string IconFileName
        {
            get { return SecondIconFileName; }
            set { string DummyString = value; } //dummy for Wcf
        }

        #region For the View

        //private string _ArbitraryId = "";
        //public override string ArbitraryId
        //{
        //    get { return _ArbitraryId; }
        //    set { _ArbitraryId = value; }
        //}

        [DataMember]
        public override string ArbitraryId
        {
            get
            {
                string ArbId = string.Empty;
                if( Parent != null )
                {
                    ArbId += Parent.ArbitraryId + "_";
                }
                if( this.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    ArbId += "NT_";
                }
                else if( this.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {
                    ArbId += "OC_";
                }
                else if( this.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                {
                    ArbId += "PS_";
                }
                ArbId += SecondId;
                if( Int32.MinValue != this.PropId )
                {
                    ArbId += this.PropId.ToString();
                }
                return ArbId;
            }
            set { string s = value; }
        }

        private CswNbtViewNode _Parent;
        public override CswNbtViewNode Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        private Collection<CswNbtViewRelationship> _ChildRelationships = new Collection<CswNbtViewRelationship>();
        [DataMember]
        public Collection<CswNbtViewRelationship> ChildRelationships
        {
            get { return _ChildRelationships; }
            set { _ChildRelationships = value; }
        }

        private Collection<CswNbtViewProperty> _Properties = new Collection<CswNbtViewProperty>();
        [DataMember]
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
        /// For adding a property set to the root level of the view
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, CswNbtMetaDataPropertySet PropertySet, bool IncludeDefaultFilters )
            : base( CswNbtResources, View )
        {
            overrideSecond( PropertySet );

            if( IncludeDefaultFilters )
                _setDefaultFilters();
        }
        /// <summary>
        /// For a relationship below the root level, determined by a property
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, CswEnumNbtViewPropOwnerType InOwnerType, ICswNbtMetaDataProp Prop, bool IncludeDefaultFilters )
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
                if( StringRelationship[0] == CswEnumNbtViewNodeType.CswNbtViewRelationship.ToString() )
                {
                    if( StringRelationship[1] != String.Empty )
                    {
                        //setPropValue( (NbtViewPropOwnerType) Enum.Parse( typeof( NbtViewPropOwnerType ), StringRelationship[4], true ),
                        //              (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), StringRelationship[2], true ),
                        setPropValue( (CswEnumNbtViewPropOwnerType) StringRelationship[4],
                                      (CswEnumNbtViewPropIdType) StringRelationship[2],
                                      CswConvert.ToInt32( StringRelationship[1] ),
                                      StringRelationship[3] );
                    }
                    if( StringRelationship[5] != String.Empty )
                    {
                        //setFirst( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), StringRelationship[6], true ),
                        setFirst( (CswEnumNbtViewRelatedIdType) StringRelationship[6],
                                  CswConvert.ToInt32( StringRelationship[5] ),
                                  StringRelationship[7] );
                    }
                    if( StringRelationship[8] != String.Empty )
                    {
                        //setSecond( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), StringRelationship[9], true ),
                        setSecond( (CswEnumNbtViewRelatedIdType) StringRelationship[9],
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
                        AddChildren = (CswEnumNbtViewAddChildrenSetting) StringRelationship[15];
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
                        setGroupByProp( (CswEnumNbtViewPropIdType) StringRelationship[20],
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
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured CswNbtViewRelationship",
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
                    setPropValue( (CswEnumNbtViewPropOwnerType) RelationshipNode.Attributes[PropOwnerAttrName].Value,
                                  (CswEnumNbtViewPropIdType) RelationshipNode.Attributes[PropTypeAttrName].Value,
                                  CswConvert.ToInt32( RelationshipNode.Attributes[PropIdAttrName].Value ),
                                  RelationshipNode.Attributes[PropNameAttrName].Value );
                }
                if( RelationshipNode.Attributes[FirstIdAttrName] != null )
                {
                    //setFirst( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), RelationshipNode.Attributes[FirstTypeAttrName].Value, true ),
                    setFirst( (CswEnumNbtViewRelatedIdType) RelationshipNode.Attributes[FirstTypeAttrName].Value,
                              CswConvert.ToInt32( RelationshipNode.Attributes[FirstIdAttrName].Value ),
                              RelationshipNode.Attributes[FirstNameAttrName].Value );
                }
                if( RelationshipNode.Attributes[SecondIdAttrName] != null )
                {
                    string icon = string.Empty;
                    if( RelationshipNode.Attributes[SecondIconFileNameAttrName] != null )
                        icon = RelationshipNode.Attributes[SecondIconFileNameAttrName].Value;

                    //setSecond( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), RelationshipNode.Attributes[SecondTypeAttrName].Value, true ),
                    setSecond( (CswEnumNbtViewRelatedIdType) RelationshipNode.Attributes[SecondTypeAttrName].Value,
                               CswConvert.ToInt32( RelationshipNode.Attributes[SecondIdAttrName].Value ),
                               RelationshipNode.Attributes[SecondNameAttrName].Value,
                               icon );
                }
                if( RelationshipNode.Attributes[GroupByPropIdAttrName] != null )
                {
                    if( RelationshipNode.Attributes[GroupByPropTypeAttrName].Value != string.Empty )
                    {
                        //setGroupByProp( (NbtViewPropIdType) Enum.Parse( typeof( NbtViewPropIdType ), RelationshipNode.Attributes[GroupByPropTypeAttrName].Value, true ),
                        setGroupByProp( (CswEnumNbtViewPropIdType) RelationshipNode.Attributes[GroupByPropTypeAttrName].Value,
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
                    AddChildren = (CswEnumNbtViewAddChildrenSetting) RelationshipNode.Attributes[AllowAddChildrenAttrName].Value;
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
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                foreach( XmlNode ChildNode in RelationshipNode.ChildNodes )
                {
                    if( ChildNode.Name.ToLower() == CswEnumNbtViewXmlNodeName.Relationship.ToString().ToLower() )
                    {
                        CswNbtViewRelationship ChildRelationshipNode = new CswNbtViewRelationship( CswNbtResources, _View, ChildNode );
                        this.addChildRelationship( ChildRelationshipNode );
                    }
                    if( ChildNode.Name.ToLower() == CswEnumNbtViewXmlNodeName.Property.ToString().ToLower() )
                    {
                        CswNbtViewProperty ChildProp = new CswNbtViewProperty( CswNbtResources, _View, ChildNode );
                        this.addProperty( ChildProp );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured CswNbtViewRelationship",
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
                    setPropValue( (CswEnumNbtViewPropOwnerType) _PropOwnerAttrName,
                                  (CswEnumNbtViewPropIdType) _PropTypeAttrName,
                                  CswConvert.ToInt32( _PropIdAttrName ),
                                  _PropNameAttrName );
                }

                string _FirstTypeAttrName = CswConvert.ToString( RelationshipObj[FirstTypeAttrName] );
                string _FirstNameAttrName = CswConvert.ToString( RelationshipObj[FirstNameAttrName] );
                string _FirstIdAttrName = CswConvert.ToString( RelationshipObj[FirstIdAttrName] );
                if( !string.IsNullOrEmpty( _FirstIdAttrName ) )
                {
                    //setFirst( (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), _FirstTypeAttrName, true ),
                    setFirst( (CswEnumNbtViewRelatedIdType) _FirstTypeAttrName,
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
                    setSecond( (CswEnumNbtViewRelatedIdType) _SecondTypeAttrName,
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
                        setGroupByProp( (CswEnumNbtViewPropIdType) _GroupByPropTypeAttrName,
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

                bool _AllowAdd = CswConvert.ToBoolean( RelationshipObj[AllowAddChildrenAttrName] );
                if( _AllowAdd )
                {
                    AddChildren = CswEnumNbtViewAddChildrenSetting.InView;
                }
                else
                {
                    AddChildren = CswEnumNbtViewAddChildrenSetting.None;
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
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured CswNbtViewRelationship",
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
                            where NodeName == CswEnumNbtViewXmlNodeName.Relationship.ToString().ToLower()
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
                            where NodeName == CswEnumNbtViewXmlNodeName.Property.ToString().ToLower()
                            select new CswNbtViewProperty( CswNbtResources, _View, PropObj ) )
                    {
                        this.addProperty( ChildProp );
                    }
                }

            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid child definition",
                                          ex );
            }
        }

        private void _setDefaultFilters()
        {
            IEnumerable<CswNbtMetaDataObjectClass> DefaultFilterOCs = null;
            if( SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
            {
                DefaultFilterOCs = new Collection<CswNbtMetaDataObjectClass>();
                ( (Collection<CswNbtMetaDataObjectClass>) DefaultFilterOCs ).Add( _CswNbtResources.MetaData.getObjectClass( SecondId ) );
            }
            else if( SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType DefaultFilterNT = _CswNbtResources.MetaData.getNodeType( SecondId );
                if( DefaultFilterNT != null )
                {
                    DefaultFilterOCs = new Collection<CswNbtMetaDataObjectClass>();
                    ( (Collection<CswNbtMetaDataObjectClass>) DefaultFilterOCs ).Add( DefaultFilterNT.getObjectClass() );
                }
            }
            else if( SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
            {
                DefaultFilterOCs = _CswNbtResources.MetaData.getObjectClassesByPropertySetId( SecondId );
            }

            if( null != DefaultFilterOCs )
            {
                foreach( CswNbtMetaDataObjectClass DefaultFilterOC in DefaultFilterOCs )
                {
                    CswNbtObjClass DefaultFilterObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, DefaultFilterOC );
                    DefaultFilterObjClass.addDefaultViewFilters( this );
                }
            }
        } // _setDefaultFilters()

        #endregion Constructors

        #region Exporters

        public override string ToString()
        {
            return ToDelimitedString().ToString();
        }

        public CswDelimitedString ToDelimitedString()
        {
            CswDelimitedString ret = new CswDelimitedString( CswNbtView.delimiter );
            ret.Add( CswEnumNbtViewNodeType.CswNbtViewRelationship.ToString() );
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
            ret.Add( AllowAdd.ToString() );
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
            XmlNode RelationshipNode = XmlDoc.CreateNode( XmlNodeType.Element, CswEnumNbtViewXmlNodeName.Relationship.ToString(), "" );

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

            AllowAddChildrenAttr.Value = AllowAdd.ToString();
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

            RelationshipObj["nodename"] = CswEnumNbtViewXmlNodeName.Relationship.ToString().ToLower();

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
            RelationshipObj[AllowAddChildrenAttrName] = AllowAdd.ToString();
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
                    RPropName = CswEnumNbtViewXmlNodeName.Relationship.ToString() + "_" + RelationshipId; ;
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

        [DataMember]
        public override string TextLabel
        {
            get
            {
                string NodeText = SecondName;
                if( PropName != String.Empty )
                {
                    if( PropOwner == CswEnumNbtViewPropOwnerType.First )
                        NodeText += " (by " + FirstName + "'s " + PropName + ")";
                    else
                        NodeText += " (by " + PropName + ")";
                }
                return NodeText;
            }

            set { string s = value; }
        }

        #region Matches

        public bool FirstMatches( ICswNbtMetaDataDefinitionObject Compare, bool IgnoreVersions = false )
        {
            return Matches( _CswNbtResources, FirstType, FirstId, Compare, IgnoreVersions );
        }

        public bool SecondMatches( ICswNbtMetaDataDefinitionObject Compare, bool IgnoreVersions = false )
        {
            return Matches( _CswNbtResources, SecondType, SecondId, Compare, IgnoreVersions );
        }

        /// <summary>
        /// Returns true if the relationship includes the provided NodeType
        /// </summary>
        public static bool Matches( CswNbtResources CswNbtResources, CswEnumNbtViewRelatedIdType Type, Int32 Pk, ICswNbtMetaDataDefinitionObject Compare, bool IgnoreVersions = false )
        {
            bool ret = false;
            if( Compare is CswNbtMetaDataNodeType )
            {
                ret = Matches( CswNbtResources, Type, Pk, (CswNbtMetaDataNodeType) Compare, IgnoreVersions );
            }
            if( Compare is CswNbtMetaDataObjectClass )
            {
                ret = Matches( CswNbtResources, Type, Pk, (CswNbtMetaDataObjectClass) Compare );
            }
            if( Compare is CswNbtMetaDataPropertySet )
            {
                ret = Matches( CswNbtResources, Type, Pk, (CswNbtMetaDataPropertySet) Compare );
            }
            return ret;
        }


        /// <summary>
        /// Returns true if the relationship includes the provided NodeType
        /// </summary>
        public static bool Matches( CswNbtResources CswNbtResources, CswEnumNbtViewRelatedIdType Type, Int32 Pk, CswNbtMetaDataNodeType CompareNT, bool IgnoreVersions = false )
        {
            bool ret = false;
            if( null != CompareNT )
            {
                if( Type == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType SecondNT = CswNbtResources.MetaData.getNodeType( Pk );
                    ret = ( null != SecondNT &&
                            ( IgnoreVersions && SecondNT.FirstVersionNodeTypeId == CompareNT.FirstVersionNodeTypeId ) ||
                            SecondNT == CompareNT );
                }
                else if( Type == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {

                    CswNbtMetaDataObjectClass SecondOC = CswNbtResources.MetaData.getObjectClass( Pk );
                    ret = ( null != SecondOC &&
                            SecondOC.ObjectClassId == CompareNT.ObjectClassId );
                }
                else if( Type == CswEnumNbtViewRelatedIdType.PropertySetId )
                {
                    CswNbtMetaDataPropertySet SecondPS = CswNbtResources.MetaData.getPropertySet( Pk );
                    ret = ( null != SecondPS &&
                            null != CompareNT.getObjectClass().getPropertySet() &&
                            SecondPS == CompareNT.getObjectClass().getPropertySet() );
                }
            }
            return ret;
        }

        /// <summary>
        /// Returns true if the relationship matches the provided ObjectClass
        /// </summary>
        public static bool Matches( CswNbtResources CswNbtResources, CswEnumNbtViewRelatedIdType Type, Int32 Pk, CswNbtMetaDataObjectClass CompareOC )
        {
            bool ret = false;
            if( null != CompareOC )
            {
                if( Type == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType SecondNT = CswNbtResources.MetaData.getNodeType( Pk );
                    ret = ( null != SecondNT &&
                            SecondNT.ObjectClassId == CompareOC.ObjectClassId );
                }
                else if( Type == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {

                    CswNbtMetaDataObjectClass SecondOC = CswNbtResources.MetaData.getObjectClass( Pk );
                    ret = ( null != SecondOC &&
                            SecondOC == CompareOC );
                }
                else if( Type == CswEnumNbtViewRelatedIdType.PropertySetId )
                {
                    CswNbtMetaDataPropertySet SecondPS = CswNbtResources.MetaData.getPropertySet( Pk );
                    ret = ( null != SecondPS &&
                            null != CompareOC.getPropertySet() &&
                            SecondPS == CompareOC.getPropertySet() );
                }
            }
            return ret;
        }

        /// <summary>
        /// Returns true if the relationship matches the provided PropertySet
        /// </summary>
        public static bool Matches( CswNbtResources CswNbtResources, CswEnumNbtViewRelatedIdType Type, Int32 Pk, CswNbtMetaDataPropertySet ComparePS )
        {
            bool ret = false;
            if( null != ComparePS )
            {
                if( Type == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType SecondNT = CswNbtResources.MetaData.getNodeType( Pk );
                    ret = ( null != SecondNT &&
                            null != SecondNT.getObjectClass().getPropertySet() &&
                            SecondNT.getObjectClass().getPropertySet() == ComparePS );
                }
                else if( Type == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {

                    CswNbtMetaDataObjectClass SecondOC = CswNbtResources.MetaData.getObjectClass( Pk );
                    ret = ( null != SecondOC &&
                            null != SecondOC.getPropertySet() &&
                            SecondOC.getPropertySet() == ComparePS );
                }
                else if( Type == CswEnumNbtViewRelatedIdType.PropertySetId )
                {
                    CswNbtMetaDataPropertySet SecondPS = CswNbtResources.MetaData.getPropertySet( Pk );
                    ret = ( null != SecondPS &&
                            SecondPS == ComparePS );
                }
            }
            return ret;
        }

        #endregion Matches


        #region IComparable

        public int CompareTo( object obj )
        {
            if( obj is CswNbtViewRelationship )
                return CompareTo( (CswNbtViewRelationship) obj );
            else
                throw new CswDniException( CswEnumErrorType.Error, "Illegal comparison", "Can't compare CswNbtViewRelationship to object: " + obj.ToString() );
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
