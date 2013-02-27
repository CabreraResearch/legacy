using System;
using System.Collections;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
//using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{

    [Serializable()]
    public class CswNbtViewProperty : CswNbtViewNode, IComparable, IEquatable<CswNbtViewProperty>
    {
        public override NbtViewNodeType ViewNodeType { get { return NbtViewNodeType.CswNbtViewProperty; } }

        private const string _FiltersName = "filters";
        private CswNbtViewRelationship _Parent;
        public override CswNbtViewNode Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                if( value == null )
                    _Parent = null;
                else if( value is CswNbtViewRelationship )
                    _Parent = (CswNbtViewRelationship) value;
                else
                    throw new CswDniException( "Illegal parent assignment on CswNbtViewPropertyFilter" );
            }
        }

        private Int32 _NodeTypePropId = Int32.MinValue;
        public Int32 NodeTypePropId
        {
            get
            {
                return _NodeTypePropId;
            }
            set
            {
                if( value != _NodeTypePropId )
                {
                    _NodeTypePropId = value;
                    //CswNbtMetaDataNodeTypeProp thisNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( _NodeTypePropId );
                    //if( thisNodeTypeProp != null )
                    //    FieldType = thisNodeTypeProp.getFieldTypeValue();
                    FieldType = _CswNbtResources.MetaData.getFieldTypeValueForNodeTypePropId( value );
                }
            }
        }
        public CswNbtMetaDataNodeTypeProp NodeTypeProp
        {
            get { return _CswNbtResources.MetaData.getNodeTypeProp( _NodeTypePropId ); }
        }
        public CswNbtMetaDataNodeTypeProp FirstVersionNodeTypeProp
        {
            get { return _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeProp.FirstPropVersionId ); }
        }

        private Int32 _ObjectClassPropId = Int32.MinValue;
        public Int32 ObjectClassPropId
        {
            get
            {
                return _ObjectClassPropId;
            }
            set
            {
                if( value != _ObjectClassPropId )
                {
                    _ObjectClassPropId = value;
                    //CswNbtMetaDataObjectClassProp ThisObjectClassProp = _CswNbtResources.MetaData.getObjectClassProp( _ObjectClassPropId );
                    //// case 20031
                    //if( ThisObjectClassProp != null )
                    //{
                    //    FieldType = _CswNbtResources.MetaData.getObjectClassProp( _ObjectClassPropId ).getFieldTypeValue();
                    //}
                    FieldType = _CswNbtResources.MetaData.getFieldTypeValueForObjectClassPropId( value );
                }
            }
        }

        public ICswNbtMetaDataProp MetaDataProp
        {
            get
            {
                ICswNbtMetaDataProp Ret = null;
                if( Type == NbtViewPropType.NodeTypePropId &&
                    Int32.MinValue != _NodeTypePropId )
                {
                    Ret = NodeTypeProp;
                }
                else if( Type == NbtViewPropType.ObjectClassPropId &&
                         Int32.MinValue != _ObjectClassPropId )
                {
                    Ret = ObjectClassProp;
                }
                return Ret;
            }
        }

        public CswNbtMetaDataObjectClassProp ObjectClassProp
        {
            get { return _CswNbtResources.MetaData.getObjectClassProp( _ObjectClassPropId ); }
        }

        public NbtViewPropType Type = NbtViewPropType.Unknown;
        public string Name = string.Empty;
        public ArrayList Filters = new ArrayList();

        public override string IconFileName
        {
            get { return "Images/view/property.gif"; }
        }

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
                { ArbId += Parent.ArbitraryId + "_"; }
                if( this.NodeTypePropId != Int32.MinValue )
                { ArbId += "NTP_" + NodeTypePropId.ToString(); }
                else if( this.ObjectClassPropId != Int32.MinValue )
                { ArbId += "OCP_" + ObjectClassPropId.ToString(); }
                return ArbId;
            }
        }

        private bool _SortBy = false;
        public bool SortBy
        {
            get { return _SortBy; }
            set { _SortBy = value; }
        }
        private NbtViewPropertySortMethod _SortMethod = NbtViewPropertySortMethod.Ascending;
        public NbtViewPropertySortMethod SortMethod
        {
            get { return _SortMethod; }
            set { _SortMethod = value; }
        }

        //public CswNbtMetaDataFieldType FieldType = null;
        public CswNbtMetaDataFieldType.NbtFieldType FieldType = CswNbtResources.UnknownEnum;
        public Int32 Order = Int32.MinValue;
        public Int32 Width = Int32.MinValue;
        public bool ShowInGrid = true;

        /// <summary>
        /// View property constructor
        /// </summary>
        public CswNbtViewProperty( CswNbtResources CswNbtResources, CswNbtView View, ICswNbtMetaDataProp Prop )
            : base( CswNbtResources, View )
        {
            if( Prop is CswNbtMetaDataNodeTypeProp )
            {
                this.Type = NbtViewPropType.NodeTypePropId;
                this.NodeTypePropId = ( (CswNbtMetaDataNodeTypeProp) Prop ).FirstPropVersionId;
            }
            else if( Prop is CswNbtMetaDataObjectClassProp )
            {
                this.Type = NbtViewPropType.ObjectClassPropId;
                this.ObjectClassPropId = ( (CswNbtMetaDataObjectClassProp) Prop ).PropId;
            }
            this.FieldType = Prop.getFieldTypeValue();
            this.Name = Prop.PropName;
        }

        /// <summary>
        /// For loading from a string (created by ToString())
        /// </summary>
        public CswNbtViewProperty( CswNbtResources CswNbtResources, CswNbtView View, CswDelimitedString PropertyString )
            : base( CswNbtResources, View )
        {
            if( PropertyString[0] == NbtViewNodeType.CswNbtViewProperty.ToString() )
            {
                //while( PropertyString.Count < 11 )
                //{
                //    PropertyString.Add( string.Empty );
                //}

                if( PropertyString[1] != string.Empty )
                {
                    //Type = (CswNbtPropType) Enum.Parse( typeof( CswNbtPropType ), PropertyString[1], true );
                    Type = (NbtViewPropType) PropertyString[1];
                }
                if( PropertyString[2] != string.Empty )
                { NodeTypePropId = CswConvert.ToInt32( PropertyString[2] ); }
                if( PropertyString[3] != string.Empty )
                { Name = PropertyString[3]; }
                //if( Values[4] != string.Empty )
                //    ArbitraryId = Values[4];
                if( PropertyString[5] != string.Empty )
                { SortBy = Convert.ToBoolean( PropertyString[5] ); }
                if( PropertyString[6] != string.Empty )
                {
                    //SortMethod = (PropertySortMethod) Enum.Parse( typeof( PropertySortMethod ), PropertyString[6], true );
                    SortMethod = (NbtViewPropertySortMethod) PropertyString[6];
                }
                if( PropertyString[7] != string.Empty )
                { FieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( PropertyString[7] ); }
                if( PropertyString[8] != string.Empty )
                { Order = CswConvert.ToInt32( PropertyString[8] ); }
                if( PropertyString[9] != string.Empty )
                { Width = CswConvert.ToInt32( PropertyString[9] ); }
                if( PropertyString[10] != string.Empty )
                { ObjectClassPropId = CswConvert.ToInt32( PropertyString[10] ); }
                if( PropertyString[11] != string.Empty )
                { ShowInGrid = CswConvert.ToBoolean( PropertyString[11] ); }
            }
        }

        /// <summary>
        /// For loading from XML
        /// </summary>
        public CswNbtViewProperty( CswNbtResources CswNbtResources, CswNbtView View, XmlNode PropNode )
            : base( CswNbtResources, View )
        {
            try
            {
                if( PropNode.Attributes["type"] != null )
                {
                    //Type = (CswNbtPropType) Enum.Parse( typeof( CswNbtPropType ), PropNode.Attributes["type"].Value, true );
                    Type = (NbtViewPropType) PropNode.Attributes["type"].Value;
                }
                if( PropNode.Attributes["value"] != null )   //backwards compatibility
                {
                    if( Type == NbtViewPropType.NodeTypePropId )
                        NodeTypePropId = CswConvert.ToInt32( PropNode.Attributes["value"].Value );
                    else
                        ObjectClassPropId = CswConvert.ToInt32( PropNode.Attributes["value"].Value );
                }
                if( PropNode.Attributes["nodetypepropid"] != null )
                { NodeTypePropId = CswConvert.ToInt32( PropNode.Attributes["nodetypepropid"].Value ); }
                if( PropNode.Attributes["objectclasspropid"] != null )
                { ObjectClassPropId = CswConvert.ToInt32( PropNode.Attributes["objectclasspropid"].Value ); }
                if( PropNode.Attributes["name"] != null )
                { Name = PropNode.Attributes["name"].Value; }
                //if( PropNode.Attributes["arbitraryid"] != null )
                //    ArbitraryId = PropNode.Attributes["arbitraryid"].Value;
                if( PropNode.Attributes["sortby"] != null )
                { SortBy = Convert.ToBoolean( PropNode.Attributes["sortby"].Value ); }
                if( PropNode.Attributes["sortmethod"] != null )
                {
                    //SortMethod = (PropertySortMethod) Enum.Parse( typeof( PropertySortMethod ), PropNode.Attributes["sortmethod"].Value, true );
                    SortMethod = (NbtViewPropertySortMethod) PropNode.Attributes["sortmethod"].Value;
                }
                if( PropNode.Attributes["fieldtype"] != null && PropNode.Attributes["fieldtype"].Value != string.Empty )
                { FieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( PropNode.Attributes["fieldtype"].Value ); }
                if( PropNode.Attributes["order"] != null && PropNode.Attributes["order"].Value != string.Empty )
                { Order = CswConvert.ToInt32( PropNode.Attributes["order"].Value ); }
                if( PropNode.Attributes["width"] != null && PropNode.Attributes["width"].Value != string.Empty )
                { Width = CswConvert.ToInt32( PropNode.Attributes["width"].Value ); }
                if( PropNode.Attributes["showingrid"] != null && PropNode.Attributes["showingrid"].Value != string.Empty )
                { ShowInGrid = CswConvert.ToBoolean( PropNode.Attributes["showingrid"].Value ); }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswViewProperty",
                                          "CswViewProperty.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                foreach( XmlNode ChildNode in PropNode.ChildNodes )
                {
                    if( ChildNode.Name.ToLower() == NbtViewXmlNodeName.Filter.ToString().ToLower() )
                    {
                        CswNbtViewPropertyFilter Filter = new CswNbtViewPropertyFilter( CswNbtResources, _View, ChildNode );
                        this.addFilter( Filter );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswViewProperty",
                                          "CswViewProperty.constructor(xmlnode) encountered an invalid filter definition",
                                          ex );
            }
        }

        /// <summary>
        /// For loading from JSON
        /// </summary>
        public CswNbtViewProperty( CswNbtResources CswNbtResources, CswNbtView View, JObject PropObj )
            : base( CswNbtResources, View )
        {
            try
            {
                string _Type = CswConvert.ToString( PropObj["type"] );
                if( !string.IsNullOrEmpty( _Type ) )
                {
                    //Type = (CswNbtPropType) Enum.Parse( typeof( CswNbtPropType ), _Type, true );
                    Type = (NbtViewPropType) _Type;
                }

                Int32 _Value = CswConvert.ToInt32( PropObj["value"] );
                if( Int32.MinValue != _Value ) //backwards compatibility
                {
                    if( Type == NbtViewPropType.NodeTypePropId )
                    { NodeTypePropId = _Value; }
                    else
                    { ObjectClassPropId = _Value; }
                }

                Int32 _NtPropId = CswConvert.ToInt32( PropObj["nodetypepropid"] );
                if( Int32.MinValue != _NtPropId )
                {
                    NodeTypePropId = _NtPropId;
                }

                Int32 _OcPropId = CswConvert.ToInt32( PropObj["objectclasspropid"] );
                if( Int32.MinValue != _OcPropId )
                {
                    ObjectClassPropId = _OcPropId;
                }

                string _Name = CswConvert.ToString( PropObj["name"] );
                if( !string.IsNullOrEmpty( _Name ) )
                {
                    Name = _Name;
                }

                if( PropObj["sortby"] != null )
                {
                    bool _Sort = CswConvert.ToBoolean( PropObj["sortby"] );
                    SortBy = _Sort;
                }

                string _SortedMethod = CswConvert.ToString( PropObj["sortmethod"] );
                if( !string.IsNullOrEmpty( _SortedMethod ) )
                {
                    //SortMethod = (PropertySortMethod) Enum.Parse( typeof( PropertySortMethod ), _SortedMethod, true );
                    SortMethod = (NbtViewPropertySortMethod) _SortedMethod;
                }


                string _FieldType = CswConvert.ToString( PropObj["fieldtype"] );
                if( !string.IsNullOrEmpty( _FieldType ) )
                {
                    FieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( _FieldType );
                }

                Int32 _Order = CswConvert.ToInt32( PropObj["order"] );
                if( Int32.MinValue != _Order )
                {
                    Order = _Order;
                }

                Int32 _Width = CswConvert.ToInt32( PropObj["width"] );
                if( Int32.MinValue != _Width )
                {
                    Width = _Width;
                }

                ShowInGrid = CswConvert.ToBoolean( PropObj["showingrid"] );
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswViewProperty",
                                          "CswViewProperty.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                JProperty FiltersProp = PropObj.Property( _FiltersName );
                if( null != FiltersProp )
                {
                    JObject FiltersObj = (JObject) FiltersProp.Value;
                    foreach( CswNbtViewPropertyFilter Filter in
                        from FilterProp
                            in FiltersObj.Properties()
                        select (JObject) FilterProp.Value
                            into FilterObj
                            let NodeName = CswConvert.ToString( FilterObj["nodename"] )
                            where NodeName == NbtViewXmlNodeName.Filter.ToString().ToLower()
                            select new CswNbtViewPropertyFilter( CswNbtResources, _View, FilterObj ) )
                    {
                        this.addFilter( Filter );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswViewProperty",
                                          "CswViewProperty.constructor(xmlnode) encountered an invalid filter definition",
                                          ex );
            }
        }

        public XmlNode ToXml( XmlDocument XmlDoc )
        {
            XmlNode NewPropNode = XmlDoc.CreateNode( XmlNodeType.Element, NbtViewXmlNodeName.Property.ToString(), "" );

            XmlAttribute PropTypeAttribute = XmlDoc.CreateAttribute( "type" );
            PropTypeAttribute.Value = Type.ToString();
            NewPropNode.Attributes.Append( PropTypeAttribute );

            XmlAttribute NodeTypePropIdAttribute = XmlDoc.CreateAttribute( "nodetypepropid" );
            NodeTypePropIdAttribute.Value = NodeTypePropId.ToString();
            NewPropNode.Attributes.Append( NodeTypePropIdAttribute );

            XmlAttribute ObjectClassPropIdAttribute = XmlDoc.CreateAttribute( "objectclasspropid" );
            ObjectClassPropIdAttribute.Value = ObjectClassPropId.ToString();
            NewPropNode.Attributes.Append( ObjectClassPropIdAttribute );

            XmlAttribute PropNameAttribute = XmlDoc.CreateAttribute( "name" );
            PropNameAttribute.Value = Name;
            NewPropNode.Attributes.Append( PropNameAttribute );

            XmlAttribute ArbitraryIdAttribute = XmlDoc.CreateAttribute( "arbitraryid" );
            ArbitraryIdAttribute.Value = ArbitraryId;
            NewPropNode.Attributes.Append( ArbitraryIdAttribute );

            XmlAttribute SortByAttribute = XmlDoc.CreateAttribute( "sortby" );
            SortByAttribute.Value = SortBy.ToString();
            NewPropNode.Attributes.Append( SortByAttribute );

            XmlAttribute SortMethodAttribute = XmlDoc.CreateAttribute( "sortmethod" );
            SortMethodAttribute.Value = SortMethod.ToString();
            NewPropNode.Attributes.Append( SortMethodAttribute );

            XmlAttribute FieldTypeAttribute = XmlDoc.CreateAttribute( "fieldtype" );
            if( FieldType != CswNbtResources.UnknownEnum )
            { FieldTypeAttribute.Value = FieldType.ToString(); }
            else
            { FieldTypeAttribute.Value = string.Empty; }
            NewPropNode.Attributes.Append( FieldTypeAttribute );

            XmlAttribute OrderAttribute = XmlDoc.CreateAttribute( "order" );
            if( Order != Int32.MinValue )
            { OrderAttribute.Value = Order.ToString(); }
            else
            { OrderAttribute.Value = string.Empty; }
            NewPropNode.Attributes.Append( OrderAttribute );

            XmlAttribute WidthAttribute = XmlDoc.CreateAttribute( "width" );
            if( Width != Int32.MinValue )
            { WidthAttribute.Value = Width.ToString(); }
            else
            { WidthAttribute.Value = string.Empty; }
            NewPropNode.Attributes.Append( WidthAttribute );

            XmlAttribute ShowInGridAttribute = XmlDoc.CreateAttribute( "showingrid" );
            ShowInGridAttribute.Value = ShowInGrid.ToString();
            NewPropNode.Attributes.Append( ShowInGridAttribute );

            foreach( CswNbtViewPropertyFilter Filter in this.Filters )
            {
                XmlNode FilterNode = Filter.ToXml( XmlDoc );
                NewPropNode.AppendChild( FilterNode );
            }

            return NewPropNode;
        }

        public JProperty ToJson( string PName = null, bool FirstLevelOnly = false, bool ShowAtRuntimeOnly = false )
        {
            JObject FilterObj = new JObject();
            if( string.IsNullOrEmpty( PName ) )
            {
                PName = NbtViewXmlNodeName.Property.ToString() + "_" + ArbitraryId;
            }

            JProperty PropertyProp = new JProperty( PName,
                                        new JObject(
                                            new JProperty( "nodename", NbtViewXmlNodeName.Property.ToString().ToLower() ),
                                            new JProperty( "type", Type.ToString() ),
                                            new JProperty( "nodetypepropid", NodeTypePropId.ToString() ),
                                            new JProperty( "objectclasspropid", ObjectClassPropId.ToString() ),
                                            new JProperty( "name", Name ),
                                            new JProperty( "arbitraryid", ArbitraryId ),
                                            new JProperty( "sortby", SortBy.ToString() ),
                                            new JProperty( "sortmethod", SortMethod.ToString() ),
                                            new JProperty( "fieldtype", ( FieldType != CswNbtResources.UnknownEnum ) ? FieldType.ToString() : "" ),
                                            new JProperty( "order", ( Order != Int32.MinValue ) ? Order.ToString() : "" ),
                                            new JProperty( "width", ( Width != Int32.MinValue ) ? Width.ToString() : "" ),
                                            new JProperty( "filters", FilterObj ),
                                            new JProperty( "showingrid", ShowInGrid )
                                        )
            );
            if( !FirstLevelOnly )
            {
                foreach( CswNbtViewPropertyFilter Filter in this.Filters )
                {
                    if( false == ShowAtRuntimeOnly || Filter.ShowAtRuntime )
                    {
                        FilterObj.Add( Filter.ToJson() );
                    }
                }
            }
            return PropertyProp;
        }

        public override string ToString()
        {
            return ToDelimitedString().ToString();
        }

        public CswDelimitedString ToDelimitedString()
        {
            CswDelimitedString ret = new CswDelimitedString( CswNbtView.delimiter );
            ret.Add( NbtViewNodeType.CswNbtViewProperty.ToString() );
            ret.Add( Type.ToString() );
            ret.Add( NodeTypePropId.ToString() );
            ret.Add( Name.ToString() );
            ret.Add( ArbitraryId.ToString() );
            ret.Add( SortBy.ToString() );
            ret.Add( SortMethod.ToString() );

            if( FieldType != CswNbtResources.UnknownEnum )
            { ret.Add( FieldType.ToString() ); }
            else
            { ret.Add( "" ); }

            if( Order != Int32.MinValue )
            { ret.Add( Order.ToString() ); }
            else
            { ret.Add( "" ); }

            if( Width != Int32.MinValue )
            { ret.Add( Width.ToString() ); }
            else
            { ret.Add( "" ); }

            ret.Add( ShowInGrid.ToString() );

            ret.Add( ObjectClassPropId.ToString() );
            return ret;
        }


        public void addFilter( CswNbtViewPropertyFilter Filter )
        {
            Filters.Add( Filter );
            Filter.Parent = this;
        }

        public void removeFilter( CswNbtViewPropertyFilter Filter )
        {
            Filters.Remove( Filter );
            Filter.Parent = null;
        }


        public override string TextLabel
        {
            get
            {
                return Name;
            }
        }

        //public void setProp(CswNbtMetaDataNodeTypeProp Prop)
        //{
        //    this.Type = CswNbtPropType.NodeTypePropId;
        //    this.NodeTypePropId = Prop.FirstPropVersionId;
        //    _setProp( Prop );
        //}
        //public void setProp(CswNbtMetaDataObjectClassProp Prop)
        //{
        //    this.Type = CswNbtPropType.ObjectClassPropId;
        //    this.ObjectClassPropId = Prop.PropId;
        //    _setProp( Prop );
        //}
        //private void _setProp( ICswNbtMetaDataProp Prop )
        //{
        //    this.FieldType = Prop.getFieldTypeValue();
        //    this.Name = Prop.PropNameWithQuestionNo;
        //}



        #region IComparable

        public int CompareTo( object obj )
        {
            if( obj is CswNbtViewProperty )
                return CompareTo( (CswNbtViewProperty) obj );
            else
                return this.Name.CompareTo( obj );
        }

        public int CompareTo( CswNbtViewProperty prop )
        {
            int ret = int.MinValue;
            if( prop.Type == NbtViewPropType.NodeTypePropId )
            {
                if( prop.Type == this.Type && prop.NodeTypePropId == this.NodeTypePropId )
                    ret = 0;
            }
            else
            {
                if( prop.Type == this.Type && prop.ObjectClassPropId == this.ObjectClassPropId )
                    ret = 0;
            }

            if( ret != 0 )
            {
                ret = ( this.Name.CompareTo( prop.Name ) );
            }
            return ret;
        }

        #endregion IComparable

        #region IEquatable

        public static bool operator ==( CswNbtViewProperty p1, CswNbtViewProperty p2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( p1, p2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) p1 == null ) || ( (object) p2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( p1.Type == p2.Type &&
                p1.Name == p2.Name &&
                p1.View == p2.View &&
                ( ( p1.Type == NbtViewPropType.NodeTypePropId && p1.NodeTypePropId == p2.NodeTypePropId ) ||
                  ( p1.Type == NbtViewPropType.ObjectClassPropId && p1.ObjectClassPropId == p2.ObjectClassPropId ) ) )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtViewProperty p1, CswNbtViewProperty p2 )
        {
            return !( p1 == p2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtViewProperty ) )
                return false;
            return this == (CswNbtViewProperty) obj;
        }

        public bool Equals( CswNbtViewProperty obj )
        {
            return this == (CswNbtViewProperty) obj;
        }

        public override int GetHashCode()
        {
            if( Type == NbtViewPropType.NodeTypePropId )
                return NodeTypePropId;
            else
                return ObjectClassPropId;
        }

        #endregion IEquatable

    } // class CswViewProperty
}
