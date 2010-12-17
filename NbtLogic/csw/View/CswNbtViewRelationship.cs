using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    [Serializable()]
    public class CswNbtViewRelationship : CswNbtViewNode, IEquatable<CswNbtViewRelationship>, IComparable
    {
        public override NbtViewNodeType ViewNodeType { get { return NbtViewNodeType.CswNbtViewRelationship; } }

        public enum RelatedIdType { NodeTypeId, ObjectClassId, Unknown };
        public enum PropIdType { NodeTypePropId, ObjectClassPropId, Unknown };
        public enum PropOwnerType { First, Second, Unknown };

        // For the Relationship
        public bool Selectable = true;
        //public bool ShowInGrid = true;
        public bool ShowInTree = true;
        public bool AllowDelete = true;

        private Int32 _PropId = Int32.MinValue;
        private PropIdType _PropType = PropIdType.NodeTypePropId;
        private PropOwnerType _PropOwner = PropOwnerType.First;
        private string _PropName = "";
        private Int32 _FirstId = Int32.MinValue;
        private string _FirstName = "";
        private RelatedIdType _FirstType = RelatedIdType.NodeTypeId;
        private Int32 _SecondId = Int32.MinValue;
        private string _SecondName = "";
        private RelatedIdType _SecondType = RelatedIdType.NodeTypeId;
        private string _SecondIconFileName = "blank.gif";
        private Int32 _GroupByPropId = Int32.MinValue;
        private PropIdType _GroupByPropType = PropIdType.NodeTypePropId;
        private string _GroupByPropName = "";

        public PropIdType PropType { get { return _PropType; } }
        public PropOwnerType PropOwner { get { return _PropOwner; } }
        public Int32 PropId { get { return _PropId; } }
        public string PropName { get { return _PropName; } }
        public Int32 FirstId { get { return _FirstId; } }
        public string FirstName { get { return _FirstName; } }
        public RelatedIdType FirstType { get { return _FirstType; } }
        public Int32 SecondId { get { return _SecondId; } }
        public string SecondName { get { return _SecondName; } }
        public RelatedIdType SecondType { get { return _SecondType; } }
        public string SecondIconFileName { get { return _SecondIconFileName; } }
        public Int32 GroupByPropId { get { return _GroupByPropId; } }
        public PropIdType GroupByPropType { get { return _GroupByPropType; } }
        public string GroupByPropName { get { return _GroupByPropName; } }


        #region Relationship internals

        public void overrideFirst( CswNbtMetaDataNodeType NodeType )
        {
            if( NodeType != null )
                setFirst( RelatedIdType.NodeTypeId, NodeType.FirstVersionNodeTypeId, NodeType.LatestVersionNodeType.NodeTypeName );
            else
                setFirst( RelatedIdType.Unknown, Int32.MinValue, string.Empty );
        }
        public void overrideFirst( CswNbtMetaDataObjectClass ObjectClass )
        {
            if( ObjectClass != null )
                setFirst( RelatedIdType.ObjectClassId, ObjectClass.ObjectClassId, ObjectClass.ObjectClass.ToString() );
            else
                setFirst( RelatedIdType.Unknown, Int32.MinValue, string.Empty );
        }
        private void setFirst( RelatedIdType InFirstType, Int32 InFirstId, string InFirstName )
        {
            _FirstType = InFirstType;
            _FirstId = InFirstId;
            if( InFirstId > 0 && InFirstType == RelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType FirstNodeType = _CswNbtResources.MetaData.getNodeType( InFirstId );
                if( FirstNodeType != null )
                    _FirstId = FirstNodeType.FirstVersionNodeTypeId;
            }
            _FirstName = InFirstName;
        }

        public void overrideSecond( CswNbtMetaDataNodeType NodeType )
        {
            setSecond( RelatedIdType.NodeTypeId, NodeType.FirstVersionNodeTypeId, NodeType.LatestVersionNodeType.NodeTypeName, NodeType.LatestVersionNodeType.IconFileName );
        }
        public void overrideSecond( CswNbtMetaDataObjectClass ObjectClass )
        {
            setSecond( RelatedIdType.ObjectClassId, ObjectClass.ObjectClassId, ObjectClass.ObjectClass.ToString(), ObjectClass.IconFileName );
        }
        private void setSecond( RelatedIdType InSecondType, Int32 InSecondId, string InSecondName, string InIconFileName )
        {
            _SecondType = InSecondType;
            _SecondId = InSecondId;
            if( InSecondId > 0 && InSecondType == RelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType SecondNodeType = _CswNbtResources.MetaData.getNodeType( InSecondId );
                if( SecondNodeType != null )
                    _SecondId = SecondNodeType.FirstVersionNodeTypeId;
            }
            _SecondName = InSecondName;
            string IconFileNamePrefix = "Images/icons/";
            if( InIconFileName.Length > IconFileNamePrefix.Length &&
                InIconFileName.Substring( 0, IconFileNamePrefix.Length ) == IconFileNamePrefix )
            {
                _SecondIconFileName = InIconFileName;
            }
            else
            {
                _SecondIconFileName = IconFileNamePrefix + InIconFileName;
            }
        }

        public void overrideProp( PropOwnerType InOwnerType, CswNbtMetaDataNodeTypeProp Prop )
        {
            setProp( InOwnerType, Prop );
        }

        private void setProp( PropOwnerType InOwnerType, CswNbtMetaDataNodeTypeProp Prop )
        {
            if( Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Location )
            {
                throw new CswDniException( "Illegal view setting", "Views must be built from Relationship or Location properties" );
            }

            setPropValue( InOwnerType, PropIdType.NodeTypePropId, Prop.FirstPropVersionId, Prop.PropName );

            if( InOwnerType == PropOwnerType.First )
            {
                overrideFirst( Prop.NodeType );
                if( Prop.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                    overrideSecond( _CswNbtResources.MetaData.getNodeType( Prop.FKValue ) );
                else if( Prop.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
                    overrideSecond( _CswNbtResources.MetaData.getObjectClass( Prop.FKValue ) );
            }
            else if( InOwnerType == PropOwnerType.Second )
            {
                if( Prop.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                    overrideFirst( _CswNbtResources.MetaData.getNodeType( Prop.FKValue ) );
                else if( Prop.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
                    overrideFirst( _CswNbtResources.MetaData.getObjectClass( Prop.FKValue ) );
                overrideSecond( Prop.NodeType );
            }
            else
            {
                throw new CswDniException( "Illegal view setting", "setProp() got Unknown owner type" );
            }
        }

        private void setProp( PropOwnerType InOwnerType, CswNbtMetaDataObjectClassProp Prop )
        {
            if( Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Location )
            {
                throw new CswDniException( "Illegal view setting", "Views must be built from Relationship or Location properties" );
            }

            setPropValue( InOwnerType, PropIdType.ObjectClassPropId, Prop.PropId, Prop.PropName );

            if( InOwnerType == PropOwnerType.First )
            {
                overrideFirst( Prop.ObjectClass );
                overrideSecond( _CswNbtResources.MetaData.getObjectClass( Prop.FKValue ) );
            }
            else if( InOwnerType == PropOwnerType.Second )
            {
                overrideFirst( _CswNbtResources.MetaData.getObjectClass( Prop.FKValue ) );
                overrideSecond( Prop.ObjectClass );
            }
            else
            {
                throw new CswDniException( "Illegal view setting", "setProp() got Unknown owner type" );
            }
        }

        private void setPropValue( PropOwnerType InOwnerType, PropIdType InPropType, Int32 InPropId, string InPropName )
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
            setGroupByProp( PropIdType.NodeTypePropId, Prop.FirstPropVersionId, Prop.PropName );
        }
        public void setGroupByProp( CswNbtMetaDataObjectClassProp Prop )
        {
            setGroupByProp( PropIdType.ObjectClassPropId, Prop.PropId, Prop.PropName );
        }
        private void setGroupByProp( PropIdType InPropType, Int32 InPropId, string InPropName )
        {
            _GroupByPropType = InPropType;
            _GroupByPropId = InPropId;
            _GroupByPropName = InPropName;
        }
        public void clearGroupBy()
        {
            _GroupByPropType = PropIdType.Unknown;
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
                    ArbId += Parent.ArbitraryId + "_";
                if( this.SecondType == RelatedIdType.NodeTypeId )
                    ArbId += "NT_";
                else if( this.SecondType == RelatedIdType.ObjectClassId )
                    ArbId += "OC_";
                ArbId += SecondId;
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
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, PropOwnerType InOwnerType, CswNbtMetaDataNodeTypeProp Prop, bool IncludeDefaultFilters )
            : base( CswNbtResources, View )
        {
            setProp( InOwnerType, Prop );

            if( IncludeDefaultFilters )
                _setDefaultFilters();
        }
        /// <summary>
        /// For a relationship below the root level, determined by an object class property
        /// </summary>
        public CswNbtViewRelationship( CswNbtResources CswNbtResources, CswNbtView View, PropOwnerType InOwnerType, CswNbtMetaDataObjectClassProp Prop, bool IncludeDefaultFilters )
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
                        setPropValue( (PropOwnerType) Enum.Parse( typeof( PropOwnerType ), StringRelationship[4], true ),
                                      (PropIdType) Enum.Parse( typeof( PropIdType ), StringRelationship[2], true ),
                                      Convert.ToInt32( StringRelationship[1] ),
                                      StringRelationship[3] );
                    }
                    if( StringRelationship[5] != String.Empty )
                    {
                        setFirst( (RelatedIdType) Enum.Parse( typeof( RelatedIdType ), StringRelationship[6], true ),
                                  Convert.ToInt32( StringRelationship[5] ),
                                  StringRelationship[7] );
                    }
                    if( StringRelationship[8] != String.Empty )
                    {
                        setSecond( (RelatedIdType) Enum.Parse( typeof( RelatedIdType ), StringRelationship[9], true ),
                                   Convert.ToInt32( StringRelationship[8] ),
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
                        AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), StringRelationship[15] );
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
                        setGroupByProp( (PropIdType) Enum.Parse( typeof( PropIdType ), StringRelationship[20], true ),
                                        Convert.ToInt32( StringRelationship[21] ),
                                        StringRelationship[22] );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( "Misconfigured CswNbtViewRelationship",
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
                    setPropValue( (PropOwnerType) Enum.Parse( typeof( PropOwnerType ), RelationshipNode.Attributes[PropOwnerAttrName].Value, true ),
                                  (PropIdType) Enum.Parse( typeof( PropIdType ), RelationshipNode.Attributes[PropTypeAttrName].Value, true ),
                                  Convert.ToInt32( RelationshipNode.Attributes[PropIdAttrName].Value ),
                                  RelationshipNode.Attributes[PropNameAttrName].Value );
                }
                if( RelationshipNode.Attributes[FirstIdAttrName] != null )
                {
                    setFirst( (RelatedIdType) Enum.Parse( typeof( RelatedIdType ), RelationshipNode.Attributes[FirstTypeAttrName].Value, true ),
                              Convert.ToInt32( RelationshipNode.Attributes[FirstIdAttrName].Value ),
                              RelationshipNode.Attributes[FirstNameAttrName].Value );
                }
                if( RelationshipNode.Attributes[SecondIdAttrName] != null )
                {
                    string icon = string.Empty;
                    if( RelationshipNode.Attributes[SecondIconFileNameAttrName] != null )
                        icon = RelationshipNode.Attributes[SecondIconFileNameAttrName].Value;

                    setSecond( (RelatedIdType) Enum.Parse( typeof( RelatedIdType ), RelationshipNode.Attributes[SecondTypeAttrName].Value, true ),
                               Convert.ToInt32( RelationshipNode.Attributes[SecondIdAttrName].Value ),
                               RelationshipNode.Attributes[SecondNameAttrName].Value,
                               icon );
                }
                if( RelationshipNode.Attributes[GroupByPropIdAttrName] != null )
                {
                    setGroupByProp( (PropIdType) Enum.Parse( typeof( PropIdType ), RelationshipNode.Attributes[GroupByPropTypeAttrName].Value, true ),
                                    Convert.ToInt32( RelationshipNode.Attributes[GroupByPropIdAttrName].Value ),
                                    RelationshipNode.Attributes[GroupByPropNameAttrName].Value );
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
                    AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), RelationshipNode.Attributes[AllowAddChildrenAttrName].Value, true );
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
                throw new CswDniException( "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                foreach( XmlNode ChildNode in RelationshipNode.ChildNodes )
                {
                    if( ChildNode.Name == CswNbtViewXmlNodeName.Relationship.ToString() )
                    {
                        CswNbtViewRelationship ChildRelationshipNode = new CswNbtViewRelationship( CswNbtResources, _View, ChildNode );
                        this.addChildRelationship( ChildRelationshipNode );
                    }
                    if( ChildNode.Name == CswNbtViewXmlNodeName.Property.ToString() )
                    {
                        CswNbtViewProperty ChildProp = new CswNbtViewProperty( CswNbtResources, _View, ChildNode );
                        this.addProperty( ChildProp );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( "Misconfigured CswNbtViewRelationship",
                                          "CswNbtViewRelationship.constructor(xmlnode) encountered an invalid child definition",
                                          ex );
            }
        }


        private void _setDefaultFilters()
        {
            CswNbtMetaDataObjectClass DefaultFilterOC = null;
            if( SecondType == RelatedIdType.ObjectClassId )
            {
                DefaultFilterOC = _CswNbtResources.MetaData.getObjectClass( SecondId );
            }
            else if( SecondType == RelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType DefaultFilterNT = _CswNbtResources.MetaData.getNodeType( SecondId );
                if( DefaultFilterNT != null )
                    DefaultFilterOC = DefaultFilterNT.ObjectClass;
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
            ret.Add( "" ); // ArbitraryId.ToString();
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
        public static string AllowDeleteAttrName = "allowdelete";
        public static string NodeIdFilterInAttrName = "nodeidstofilterin";
        public static string NodeIdFilterOutAttrName = "nodeidstofilterout";

        public XmlNode ToXml( XmlDocument XmlDoc )
        {
            XmlNode RelationshipNode = XmlDoc.CreateNode( XmlNodeType.Element, CswNbtViewXmlNodeName.Relationship.ToString(), "" );

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

            XmlAttribute AllowDeleteAttr = XmlDoc.CreateAttribute( AllowDeleteAttrName );
            AllowDeleteAttr.Value = AllowDelete.ToString();
            RelationshipNode.Attributes.Append( AllowDeleteAttr );

            XmlAttribute NodeIdFilterInAttribute = XmlDoc.CreateAttribute( NodeIdFilterInAttrName );
            //NodeIdFilterInAttribute.Value = CswTools.IntCollectionToDelimitedString( NodeIdsToFilterIn, ',', false );
            string FilterInString = "";
            bool bFirst = true;
            foreach( CswPrimaryKey child in NodeIdsToFilterIn )
            {
                if( !bFirst ) FilterInString += ','.ToString();
                FilterInString += child.ToString();
                bFirst = false;
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
                    if( PropOwner == CswNbtViewRelationship.PropOwnerType.First )
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
                throw new CswDniException( "Illegal comparison", "Can't compare CswNbtViewRelationship to object: " + obj.ToString() );
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
