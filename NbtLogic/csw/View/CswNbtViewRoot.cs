using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    public class CswNbtViewRoot : CswNbtViewNode
    {
        private CswDelimitedString _RootString;
        private const string _ChildRelationshipsName = "childrelationships";
        #region Properties in _RootString

        // 0 - ViewNodeType
        public override NbtViewNodeType ViewNodeType
        {
            get
            {
                NbtViewNodeType ret;
                if( !Enum.TryParse<NbtViewNodeType>( _RootString[0], out ret ) )
                    ret = NbtViewNodeType.CswNbtViewRoot;
                return ret;
            }
        }

        // 1 - ViewName
        public string ViewName
        {
            get { return _RootString[1]; }
            set
            {
                if( _RootString[1] != value )
                {
                    _RootString[1] = value;

                    if( ViewId.isSet() )
                    {
                        // Update ViewPickList properties
                        CswStaticSelect RelatedsQuery = _CswNbtResources.makeCswStaticSelect( "RelatedsQuery", "getViewPickListsForViewId" );
                        RelatedsQuery.S4Parameters.Add( "getviewid", ViewId.get() );
                        DataTable RelatedsTable = RelatedsQuery.getTable();

                        // Update the jct_nodes_props directly, to avoid having to fetch all the node info for every node referencing this view
                        string PkString = string.Empty;
                        foreach( DataRow RelatedsRow in RelatedsTable.Rows )
                        {
                            if( PkString != string.Empty ) PkString += ",";
                            PkString += RelatedsRow["jctnodepropid"].ToString();
                        }
                        if( PkString != string.Empty )
                        {
                            CswTableUpdate JctNodesPropsUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtViewRoot.JctNodesPropsUpdate", "jct_nodes_props" );
                            DataTable JctNodesPropsTable = JctNodesPropsUpdate.getTable( "where jctnodepropid in (" + PkString + ")" );
                            foreach( DataRow JctNodesPropsRow in JctNodesPropsTable.Rows )
                            {
                                JctNodesPropsRow["pendingupdate"] = "1";
                            }
                            JctNodesPropsUpdate.update( JctNodesPropsTable );
                        }
                    } // if( ViewId != Int32.MinValue )
                } // if( _RootString[1] != value )
            } // set
        } // ViewName

        // 2 - Selectable
        public bool Selectable
        {
            get
            {
                bool ret = true;
                if( _RootString[2] != string.Empty )
                    ret = CswConvert.ToBoolean( _RootString[2] );
                return ret;
            }
            set
            {
                _RootString[2] = value.ToString();
            }
        } // Selectable

        // 3 - NodeIdsToFilterOut (defunct)

        // 4 - ViewMode
        public NbtViewRenderingMode ViewMode
        {
            get
            {
                NbtViewRenderingMode ret;
                if( !Enum.TryParse<NbtViewRenderingMode>( _RootString[4], out ret ) )
                    ret = NbtViewRenderingMode.Tree;
                return ret;
            }
            set
            {
                _RootString[4] = value.ToString();
            }
        }

        // 5 - Width
        public Int32 Width
        {
            get
            {
                Int32 ret = 100;
                if( CswTools.IsInteger( _RootString[5] ) )
                    ret = CswConvert.ToInt32( _RootString[5] );
                if( ret <= 0 )
                    ret = 100;
                return ret;
            }
            set
            {
                _RootString[5] = value.ToString();
            }
        }

        // 6 - EditMode (defunct)

        // 7 - ViewId
        public CswNbtViewId ViewId
        {
            get { return new CswNbtViewId( _RootString[7] ); }
            set { _RootString[7] = value.ToString(); }
        }

        // 8 - Category
        public string Category
        {
            get { return _RootString[8]; }
            set { _RootString[8] = value; }
        }

        // 9 - Visibility
        public NbtViewVisibility Visibility
        {
            get
            {
                NbtViewVisibility ret;
                if( !Enum.TryParse<NbtViewVisibility>( _RootString[9], out ret ) )
                    ret = NbtViewVisibility.Unknown;
                return ret;
            }
            set
            {
                _RootString[9] = value.ToString();
            }
        }

        // 10 - AddChildren (defunct)

        // 11 - VisibilityRoleId
        public CswPrimaryKey VisibilityRoleId
        {
            get
            {
                CswPrimaryKey ret = null;
                if( _RootString[11] != string.Empty )
                {
                    ret = new CswPrimaryKey();
                    ret.FromString( _RootString[11] );
                }
                return ret;
            }
            set
            {
                if( value != null )
                    _RootString[11] = value.ToString();
                else
                    _RootString[11] = string.Empty;
            }
        } // VisibilityRoleId

        // 12 - VisibilityUserId
        public CswPrimaryKey VisibilityUserId
        {
            get
            {
                CswPrimaryKey ret = null;
                if( _RootString[12] != string.Empty )
                {
                    ret = new CswPrimaryKey();
                    ret.FromString( _RootString[12] );
                }
                return ret;
            }
            set
            {
                if( value != null )
                    _RootString[12] = value.ToString();
                else
                    _RootString[12] = string.Empty;
            }
        } // VisibilityUserId

        // 13 - WelcomeText (defunct)
        // 14 - RelatedViewIds (defunct)

        // 15 - ForMobile
        public bool ForMobile
        {
            get
            {
                bool ret = false;
                if( _RootString[15] != string.Empty )
                    ret = CswConvert.ToBoolean( _RootString[15] );
                return ret;
            }
            set
            {
                _RootString[15] = value.ToString();
            }
        } // ForMobile

        private Int32 _PropCount = 16;

        #endregion Properties in _RootString

        #region Properties not in _RootString

        public override string IconFileName
        {
            get
            {
                return "Images/view/view" + ViewMode.ToString().ToLower() + ".gif"; ;
            }
        }

        public override CswNbtViewNode Parent
        {
            get { return null; }
            set { }
        }

        public override string ArbitraryId
        {
            get { return "root"; }
        }

        private Collection<CswNbtViewRelationship> _ChildRelationships = new Collection<CswNbtViewRelationship>();
        public Collection<CswNbtViewRelationship> ChildRelationships
        {
            get { return _ChildRelationships; }
            set { _ChildRelationships = value; }
        }

        public override string TextLabel
        {
            get
            {
                return ViewName;
            }
        }

        #endregion Properties not in _RootString

        #region Constructors

        public CswNbtViewRoot( CswNbtResources CswNbtResources, CswNbtView View )
            : base( CswNbtResources, View )
        {
            _RootString = new CswDelimitedString( CswNbtView.delimiter, _PropCount );
            _RootString.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _RootString_OnChange );
        }

        public CswNbtViewRoot( CswNbtResources CswNbtResources, CswNbtView View, CswDelimitedString RootString )
            : base( CswNbtResources, View )
        {
            _RootString = RootString;
            _RootString.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _RootString_OnChange );
            if( ViewNodeType != NbtViewNodeType.CswNbtViewRoot )
                throw new CswDniException( ErrorType.Error, "Invalid View Root", "CswNbtViewRoot was given an invalid RootString: " + RootString.ToString() );
        }


        public CswNbtViewRoot( CswNbtResources CswNbtResources, CswNbtView View, XmlNode Node )
            : base( CswNbtResources, View )
        {
            try
            {
                _RootString = new CswDelimitedString( CswNbtView.delimiter, _PropCount );
                _RootString.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _RootString_OnChange );

                if( Node.Attributes["viewname"] != null )
                    _RootString[1] = Node.Attributes["viewname"].Value;    // set _RootString[1], not ViewName, because we're not *changing* the name of the view
                //if( Node.Attributes[ "version" ] != null )
                //    _ReadVersion = Node.Attributes[ "version" ].Value;
                if( Node.Attributes["selectable"] != null )
                    Selectable = Convert.ToBoolean( Node.Attributes["selectable"].Value );
                if( Node.Attributes["mode"] != null )
                    ViewMode = (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), Node.Attributes["mode"].Value, true );
                if( Node.Attributes["width"] != null && Node.Attributes["width"].Value != String.Empty )
                    Width = CswConvert.ToInt32( Node.Attributes["width"].Value );
                //if( Node.Attributes[ "editmode" ] != null )
                //    EditMode = ( GridEditMode ) Enum.Parse( typeof( GridEditMode ), Node.Attributes[ "editmode" ].Value, true );
                if( Node.Attributes["viewid"] != null && Node.Attributes["viewid"].Value != String.Empty )
                    ViewId = new CswNbtViewId( CswConvert.ToInt32( Node.Attributes["viewid"].Value ) );
                if( Node.Attributes["category"] != null && Node.Attributes["category"].Value != String.Empty )
                    Category = Node.Attributes["category"].Value;
                if( Node.Attributes["visibility"] != null && Node.Attributes["visibility"].Value != String.Empty )
                    Visibility = (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), Node.Attributes["visibility"].Value, true );
                //if (Node.Attributes["addchildren"] != null && Node.Attributes["addchildren"].Value != String.Empty)
                //    AddChildren = ( NbtViewAddChildrenSetting ) Enum.Parse( typeof( NbtViewAddChildrenSetting ), Node.Attributes[ "addchildren" ].Value, true );
                if( Node.Attributes["visibilityroleid"] != null && Node.Attributes["visibilityroleid"].Value != String.Empty )
                    VisibilityRoleId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Node.Attributes["visibilityroleid"].Value ) );
                if( Node.Attributes["visibilityuserid"] != null && Node.Attributes["visibilityuserid"].Value != String.Empty )
                    VisibilityUserId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Node.Attributes["visibilityuserid"].Value ) );
                //if( Node.Attributes["welcometext"] != null && Node.Attributes["welcometext"].Value != string.Empty )
                //    WelcomeText = Node.Attributes["welcometext"].Value;
                //if( Node.Attributes["relatedviewids"] != null && Node.Attributes["relatedviewids"].Value != string.Empty )
                //    RelatedViewIds = Node.Attributes["relatedviewids"].Value;
                if( Node.Attributes["formobile"] != null )
                    ForMobile = Convert.ToBoolean( Node.Attributes["formobile"].Value );
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewNodeRoot",
                                          "CswNbtViewNodeRoot.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                foreach( XmlNode ChildNode in Node.ChildNodes )
                {
                    if( ChildNode.Name.ToLower() == CswNbtViewXmlNodeName.Relationship.ToString().ToLower() )
                    {
                        CswNbtViewRelationship ChildRelationship = new CswNbtViewRelationship( CswNbtResources, _View, ChildNode );
                        this.addChildRelationship( ChildRelationship );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewNodeRoot",
                                          "CswNbtViewNodeRoot.constructor(xmlnode) encountered an invalid child definition",
                                          ex );
            }
        }

        public CswNbtViewRoot( CswNbtResources CswNbtResources, CswNbtView View, JObject Node )
            : base( CswNbtResources, View )
        {
            try
            {
                _RootString = new CswDelimitedString( CswNbtView.delimiter, _PropCount );
                _RootString.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _RootString_OnChange );

                string _ViewName = CswConvert.ToString( Node["viewname"] );
                if( !string.IsNullOrEmpty( _ViewName ) )
                {
                    _RootString[1] = _ViewName; // set _RootString[1], not ViewName, because we're not *changing* the name of the view
                }

                if( null != Node["selectable"] )
                {
                    bool _Selectable = CswConvert.ToBoolean( Node["selectable"] );
                    Selectable = _Selectable;
                }

                string _Mode = CswConvert.ToString( Node["mode"] );
                if( !string.IsNullOrEmpty( _Mode ) )
                {
                    ViewMode = (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), _Mode, true );
                }

                Int32 _Width = CswConvert.ToInt32( Node["width"] );
                if( Int32.MinValue != _Width )
                {
                    Width = _Width;
                }

                Int32 _ViewId = CswConvert.ToInt32( Node["viewid"] );
                if( Int32.MinValue != _ViewId )
                {
                    ViewId = new CswNbtViewId( _ViewId );
                }

                string _Category = CswConvert.ToString( Node["category"] );
                if( !string.IsNullOrEmpty( _Category ) )
                {
                    Category = _Category;
                }

                string _Visibility = CswConvert.ToString( Node["visibility"] );
                if( !string.IsNullOrEmpty( _Visibility ) )
                {
                    Visibility = (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), _Visibility, true );
                }

                Int32 _VisibilityRoleId = CswConvert.ToInt32( Node["visibilityroleid"] );
                if( Int32.MinValue != _VisibilityRoleId )
                {
                    VisibilityRoleId = new CswPrimaryKey( "nodes", _VisibilityRoleId );
                }

                Int32 _VisibilityUserId = CswConvert.ToInt32( Node["visibilityuserid"] );
                if( Int32.MinValue != _VisibilityUserId )
                {
                    VisibilityUserId = new CswPrimaryKey( "nodes", _VisibilityUserId );
                }

                if( Node["formobile"] != null )
                {
                    bool _ForMobile = CswConvert.ToBoolean( Node["formobile"] );
                    ForMobile = _ForMobile;
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewNodeRoot",
                                          "CswNbtViewNodeRoot.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                JProperty Children = Node.Property( _ChildRelationshipsName );
                if( null != Children )
                {
                    JObject Relationships = (JObject) Children.Value;
                    foreach( CswNbtViewRelationship ChildRelationship in
                        from Relationship
                            in Relationships.Properties()
                        select (JObject) Relationship.Value
                            into RelationshipObj
                            let NodeName = CswConvert.ToString( RelationshipObj["nodename"] )
                            where NodeName == CswNbtViewXmlNodeName.Relationship.ToString().ToLower()
                            select new CswNbtViewRelationship( CswNbtResources, _View, RelationshipObj ) )
                    {
                        this.addChildRelationship( ChildRelationship );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswNbtViewNodeRoot",
                                          "CswNbtViewNodeRoot.constructor(xmlnode) encountered an invalid child definition",
                                          ex );
            }
        }

        #endregion Constructors

        #region Events

        void _RootString_OnChange()
        {

        }

        #endregion Events

        #region Exporters

        public XmlNode ToXml( XmlDocument XmlDoc )
        {
            XmlNode RootXmlNode = XmlDoc.CreateNode( XmlNodeType.Element, CswNbtViewXmlNodeName.TreeView.ToString(), "" );

            XmlAttribute ViewNameAttribute = XmlDoc.CreateAttribute( "viewname" );
            ViewNameAttribute.Value = ViewName;
            RootXmlNode.Attributes.Append( ViewNameAttribute );

            //bz #5157
            XmlAttribute ViewVersionAttribute = XmlDoc.CreateAttribute( "version" );
            ViewVersionAttribute.Value = "1.0";
            RootXmlNode.Attributes.Append( ViewVersionAttribute );

            XmlAttribute IconFileNameAttribute = XmlDoc.CreateAttribute( "iconfilename" );
            IconFileNameAttribute.Value = IconFileName;
            RootXmlNode.Attributes.Append( IconFileNameAttribute );

            XmlAttribute SelectableAttribute = XmlDoc.CreateAttribute( "selectable" );
            SelectableAttribute.Value = Selectable.ToString().ToLower();
            RootXmlNode.Attributes.Append( SelectableAttribute );

            XmlAttribute ModeAttribute = XmlDoc.CreateAttribute( "mode" );
            ModeAttribute.Value = ViewMode.ToString();
            RootXmlNode.Attributes.Append( ModeAttribute );

            XmlAttribute WidthAttribute = XmlDoc.CreateAttribute( "width" );
            if( Width > 0 )
                WidthAttribute.Value = Width.ToString();
            else
                WidthAttribute.Value = "";
            RootXmlNode.Attributes.Append( WidthAttribute );

            //XmlAttribute EditModeAttribute = XmlDoc.CreateAttribute( "editmode" );
            //EditModeAttribute.Value = EditMode.ToString();
            //RootXmlNode.Attributes.Append( EditModeAttribute );

            XmlAttribute ViewIdAttribute = XmlDoc.CreateAttribute( "viewid" );
            if( ViewId.isSet() )
                ViewIdAttribute.Value = ViewId.get().ToString();
            else
                ViewIdAttribute.Value = "";
            RootXmlNode.Attributes.Append( ViewIdAttribute );

            XmlAttribute CategoryAttribute = XmlDoc.CreateAttribute( "category" );
            CategoryAttribute.Value = Category.ToString();
            RootXmlNode.Attributes.Append( CategoryAttribute );

            XmlAttribute VisibilityAttribute = XmlDoc.CreateAttribute( "visibility" );
            VisibilityAttribute.Value = Visibility.ToString();
            RootXmlNode.Attributes.Append( VisibilityAttribute );

            //XmlAttribute AddChildrenAttribute = XmlDoc.CreateAttribute("addchildren");
            //AddChildrenAttribute.Value = AddChildren.ToString();
            //RootXmlNode.Attributes.Append( AddChildrenAttribute );

            XmlAttribute VisibilityRoleIdAttribute = XmlDoc.CreateAttribute( "visibilityroleid" );
            if( VisibilityRoleId != null )
                VisibilityRoleIdAttribute.Value = VisibilityRoleId.PrimaryKey.ToString();
            RootXmlNode.Attributes.Append( VisibilityRoleIdAttribute );

            XmlAttribute VisibilityUserIdAttribute = XmlDoc.CreateAttribute( "visibilityuserid" );
            if( VisibilityUserId != null )
                VisibilityUserIdAttribute.Value = VisibilityUserId.PrimaryKey.ToString();
            RootXmlNode.Attributes.Append( VisibilityUserIdAttribute );

            //XmlAttribute WelcomeTextAttribute = XmlDoc.CreateAttribute( "welcometext" );
            //WelcomeTextAttribute.Value = WelcomeText;
            //RootXmlNode.Attributes.Append( WelcomeTextAttribute );

            //XmlAttribute RelatedViewIdsAttribute = XmlDoc.CreateAttribute( "relatedviewids" );
            //RelatedViewIdsAttribute.Value = RelatedViewIds;
            //RootXmlNode.Attributes.Append( RelatedViewIdsAttribute );

            XmlAttribute ForMobileAttribute = XmlDoc.CreateAttribute( "formobile" );
            ForMobileAttribute.Value = ForMobile.ToString().ToLower();
            RootXmlNode.Attributes.Append( ForMobileAttribute );

            // Recurse on child ViewNodes
            foreach( CswNbtViewRelationship ChildRelationship in this.ChildRelationships )
            {
                XmlNode ChildXmlNode = ChildRelationship.ToXml( XmlDoc );
                RootXmlNode.AppendChild( ChildXmlNode );
            }

            return RootXmlNode;
        }//ToXml()

        public JObject ToJson()
        {
            JObject Ret = new JObject();

            JObject RootPropObj = new JObject();
            JProperty RootProperty = new JProperty( CswNbtViewXmlNodeName.TreeView.ToString(), RootPropObj );

            Ret.Add( RootProperty );

            RootPropObj.Add( new JProperty( "nodename", CswNbtViewXmlNodeName.TreeView.ToString().ToLower() ) );
            RootPropObj.Add( new JProperty( "viewname", ViewName ) );
            RootPropObj.Add( new JProperty( "version", "1.0" ) );
            RootPropObj.Add( new JProperty( "iconfilename", IconFileName ) );
            RootPropObj.Add( new JProperty( "selectable", Selectable.ToString().ToLower() ) );
            RootPropObj.Add( new JProperty( "mode", ViewMode.ToString() ) );
            RootPropObj.Add( new JProperty( "width", ( Width > 0 ) ? Width.ToString() : "" ) );
            RootPropObj.Add( new JProperty( "viewid", ( ViewId.isSet() ) ? ViewId.get().ToString() : "" ) );
            RootPropObj.Add( new JProperty( "category", Category.ToString() ) );
            RootPropObj.Add( new JProperty( "visibility", Visibility.ToString() ) );
            RootPropObj.Add( new JProperty( "visibilityroleid", ( VisibilityRoleId != null ) ? VisibilityRoleId.PrimaryKey.ToString() : "" ) );
            RootPropObj.Add( new JProperty( "visibilityuserid", ( VisibilityUserId != null ) ? VisibilityUserId.PrimaryKey.ToString() : "" ) );
            RootPropObj.Add( new JProperty( "formobile", ForMobile.ToString().ToLower() ) );

            JObject ChildObject = new JObject();
            RootPropObj.Add( new JProperty( _ChildRelationshipsName, ChildObject ) );
            // Recurse on child ViewNodes
            foreach( CswNbtViewRelationship ChildRelationship in this.ChildRelationships )
            {
                ChildObject.Add( ChildRelationship.ToJson() );
            }

            return Ret;
        }//ToXml()

        public override string ToString()
        {
            return _RootString.ToString();
        }

        #endregion Exporters


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



    } // class CswNbtViewNodeRoot

} // namespace ChemSW.Nbt
