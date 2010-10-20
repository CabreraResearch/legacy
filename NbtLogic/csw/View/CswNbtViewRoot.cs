using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.DB;
using System.Data;

namespace ChemSW.Nbt
{
    ///// <summary>
    ///// Editing setting which constrains which nodes can be edited
    ///// </summary>
    //public enum GridEditMode { Quick, Full, None };

    public class CswNbtViewRoot : CswNbtViewNode
    {
        public override NbtViewNodeType ViewNodeType { get { return NbtViewNodeType.CswNbtViewRoot; } }

        private string _ViewName = string.Empty;
        public string ViewName
        {
            get { return _ViewName; }
            set
            {
                if( _ViewName != value )
                {
                    _ViewName = value;

                    if( ViewId != Int32.MinValue )
                    {
                        // Update ViewPickList properties
                        CswStaticSelect RelatedsQuery = _CswNbtResources.makeCswStaticSelect( "RelatedsQuery", "getViewPickListsForViewId" );
                        RelatedsQuery.S4Parameters.Add( "getviewid", ViewId );
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
                    }


                }
            }
        }
        public NbtViewVisibility Visibility = NbtViewVisibility.Unknown;
        public CswPrimaryKey VisibilityRoleId = null;
        public CswPrimaryKey VisibilityUserId = null;
        public string Category = String.Empty;
        //private NbtViewAddChildrenSetting _AddChildren = NbtViewAddChildrenSetting.InView;
        //public NbtViewAddChildrenSetting AddChildren
        //{
        //    get { return _AddChildren; }
        //    set
        //    {
        //        // Backwards compatibility
        //        if( value == NbtViewAddChildrenSetting.True )
        //            _AddChildren = NbtViewAddChildrenSetting.InView;
        //        else if( value == NbtViewAddChildrenSetting.False )
        //            _AddChildren = NbtViewAddChildrenSetting.None;
        //        else
        //            _AddChildren = value;
        //    }
        //}

        private NbtViewRenderingMode _ViewMode = NbtViewRenderingMode.Tree;
        public NbtViewRenderingMode ViewMode
        {
            get { return _ViewMode; }
            set { _ViewMode = value; }
        }
        private Int32 _ViewId = Int32.MinValue;
        public Int32 ViewId
        {
            get { return _ViewId; }
            set { _ViewId = value; }
        }
        private Int32 _Width = 100;
        public Int32 Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        //private GridEditMode _EditMode = GridEditMode.Full;
        //public GridEditMode EditMode
        //{
        //    get { return _EditMode; }
        //    set { _EditMode = value; }
        //}

        //private string _WelcomeText = string.Empty;
        //public string WelcomeText
        //{
        //    get { return _WelcomeText; }
        //    set { _WelcomeText = value; }
        //}

        //private string _RelatedViewIds = string.Empty;
        //public string RelatedViewIds
        //{
        //    get { return _RelatedViewIds; }
        //    set { _RelatedViewIds = value; }
        //}


        public bool Selectable = true;

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

        //private string _ArbitraryId = "";
        //public override string ArbitraryId
        //{
        //    get { return _ArbitraryId; }
        //    set { _ArbitraryId = value; }
        //}

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

        public CswNbtViewRoot( CswNbtResources CswNbtResources, CswNbtView View )
            : base( CswNbtResources, View )
        {
        }

        public CswNbtViewRoot( CswNbtResources CswNbtResources, CswNbtView View, string RootString )
            : base( CswNbtResources, View )
        {
            string[] Values = RootString.Split( Delimiter );
            if( Values[0] == NbtViewNodeType.CswNbtViewRoot.ToString() )
            {
                if( Values[1] != String.Empty )
                    _ViewName = Values[1];      // set _ViewName, not ViewName, because we're not *changing* the name of the view
                if( Values[2] != String.Empty )
                    Selectable = Convert.ToBoolean( Values[2].ToString() );
                //if( Values[ 3 ] != String.Empty )
                //    NodeIdsToFilterOut = CswTools.CommaSeparatedStringToArrayList( Values[ 3 ] );
                if( Values[4] != String.Empty )
                    ViewMode = (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), Values[4], true );
                if( Values[5] != String.Empty )
                    Width = Convert.ToInt32( Values[5] );
                //if( Values[6] != String.Empty )
                //    EditMode = (GridEditMode) Enum.Parse( typeof( GridEditMode ), Values[6], true );
                if( Values[7] != String.Empty )
                    ViewId = Convert.ToInt32( Values[7] );
                if( Values[8] != String.Empty )
                    Category = Values[8];
                if( Values[9] != String.Empty )
                    Visibility = (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), Values[9], true );
                //if( Values[10] != String.Empty )
                //    AddChildren = (NbtViewAddChildrenSetting) Enum.Parse( typeof( NbtViewAddChildrenSetting ), Values[10], true );
                if( Values[11] != String.Empty )
                    VisibilityRoleId = new CswPrimaryKey( "nodes", Convert.ToInt32( Values[11] ) );
                if( Values[12] != String.Empty )
                    VisibilityUserId = new CswPrimaryKey( "nodes", Convert.ToInt32( Values[12] ) );
                //if( Values[13] != String.Empty )
                //    WelcomeText = Values[13];
                //if( Values[14] != String.Empty )
                //    RelatedViewIds = Values[14];
            }
            else
                throw new CswDniException( "Invalid View Root", "CswNbtViewRoot was given an invalid RootString: " + RootString );
        }


        public CswNbtViewRoot( CswNbtResources CswNbtResources, CswNbtView View, XmlNode Node )
            : base( CswNbtResources, View )
        {
            try
            {
                if( Node.Attributes["viewname"] != null )
                    _ViewName = Node.Attributes["viewname"].Value;    // set _ViewName, not ViewName, because we're not *changing* the name of the view
                //if( Node.Attributes[ "version" ] != null )
                //    _ReadVersion = Node.Attributes[ "version" ].Value;
                if( Node.Attributes["selectable"] != null )
                    Selectable = Convert.ToBoolean( Node.Attributes["selectable"].Value );
                if( Node.Attributes["mode"] != null )
                    ViewMode = (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), Node.Attributes["mode"].Value, true );
                if( Node.Attributes["width"] != null && Node.Attributes["width"].Value != String.Empty )
                    Width = Convert.ToInt32( Node.Attributes["width"].Value );
                //if( Node.Attributes[ "editmode" ] != null )
                //    EditMode = ( GridEditMode ) Enum.Parse( typeof( GridEditMode ), Node.Attributes[ "editmode" ].Value, true );
                if( Node.Attributes["viewid"] != null && Node.Attributes["viewid"].Value != String.Empty )
                    ViewId = Convert.ToInt32( Node.Attributes["viewid"].Value );
                if( Node.Attributes["category"] != null && Node.Attributes["category"].Value != String.Empty )
                    Category = Node.Attributes["category"].Value;
                if( Node.Attributes["visibility"] != null && Node.Attributes["visibility"].Value != String.Empty )
                    Visibility = (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), Node.Attributes["visibility"].Value, true );
                //if (Node.Attributes["addchildren"] != null && Node.Attributes["addchildren"].Value != String.Empty)
                //    AddChildren = ( NbtViewAddChildrenSetting ) Enum.Parse( typeof( NbtViewAddChildrenSetting ), Node.Attributes[ "addchildren" ].Value, true );
                if( Node.Attributes["visibilityroleid"] != null && Node.Attributes["visibilityroleid"].Value != String.Empty )
                    VisibilityRoleId = new CswPrimaryKey( "nodes", Convert.ToInt32( Node.Attributes["visibilityroleid"].Value ) );
                if( Node.Attributes["visibilityuserid"] != null && Node.Attributes["visibilityuserid"].Value != String.Empty )
                    VisibilityUserId = new CswPrimaryKey( "nodes", Convert.ToInt32( Node.Attributes["visibilityuserid"].Value ) );
                //if( Node.Attributes["welcometext"] != null && Node.Attributes["welcometext"].Value != string.Empty )
                //    WelcomeText = Node.Attributes["welcometext"].Value;
                //if( Node.Attributes["relatedviewids"] != null && Node.Attributes["relatedviewids"].Value != string.Empty )
                //    RelatedViewIds = Node.Attributes["relatedviewids"].Value;
            }
            catch( Exception ex )
            {
                throw new CswDniException( "Misconfigured CswNbtViewNodeRoot",
                                          "CswNbtViewNodeRoot.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }
            try
            {
                foreach( XmlNode ChildNode in Node.ChildNodes )
                {
                    if( ChildNode.Name == CswNbtViewXmlNodeName.Relationship.ToString() )
                    {
                        CswNbtViewRelationship ChildRelationship = new CswNbtViewRelationship( CswNbtResources, _View, ChildNode );
                        this.addChildRelationship( ChildRelationship );
                    }
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( "Misconfigured CswNbtViewNodeRoot",
                                          "CswNbtViewNodeRoot.constructor(xmlnode) encountered an invalid child definition",
                                          ex );
            }
        }


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
            if( ViewId > 0 )
                ViewIdAttribute.Value = ViewId.ToString();
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

            // Recurse on child ViewNodes
            foreach( CswNbtViewRelationship ChildRelationship in this.ChildRelationships )
            {
                XmlNode ChildXmlNode = ChildRelationship.ToXml( XmlDoc );
                RootXmlNode.AppendChild( ChildXmlNode );
            }

            return RootXmlNode;
        }//ToXml()


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

        public override string ToString()
        {
            string ret = NbtViewNodeType.CswNbtViewRoot.ToString();
            ret += Delimiter.ToString() + ViewName;
            ret += Delimiter.ToString() + Selectable.ToString();
            ret += Delimiter.ToString();// +CswTools.ArrayListToCommaSeparatedString(NodeIdsToFilterOut);
            ret += Delimiter.ToString() + ViewMode.ToString();
            if( Width > 0 )
                ret += Delimiter.ToString() + Width.ToString();
            else
                ret += Delimiter.ToString();
            ret += Delimiter.ToString();// +EditMode.ToString();
            if( ViewId > 0 )
                ret += Delimiter.ToString() + ViewId.ToString();
            else
                ret += Delimiter.ToString();
            ret += Delimiter.ToString() + Category;
            ret += Delimiter.ToString() + Visibility.ToString();
            ret += Delimiter.ToString(); // + AddChildren.ToString();
            ret += Delimiter.ToString() + VisibilityRoleId.ToString();
            ret += Delimiter.ToString() + VisibilityUserId.ToString();
            ret += Delimiter.ToString(); // + WelcomeText;
            ret += Delimiter.ToString(); // + RelatedViewIds;
            ret += Delimiter.ToString();
            return ret;
        }

        public override string TextLabel
        {
            get
            {
                return ViewName;
            }
        }



    } // class CswNbtViewNodeRoot

} // namespace ChemSW.Nbt
