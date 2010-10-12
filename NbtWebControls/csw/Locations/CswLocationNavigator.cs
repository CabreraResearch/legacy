using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;
using ChemSW.Core;

namespace ChemSW.NbtWebControls
{
    public class CswLocationNavigator : CompositeControl, INamingContainer, IPostBackDataHandler
    {
        #region Properties

        //private bool _UseUpdatePanel = true;
        public bool MoveMode = false;
        
        private CswNbtLocationTree _CswNbtLocationTree;
        private CswNbtResources _CswNbtResources;

        private CswNbtView _View;
        public CswNbtView View
        {
            get { return _View; }
            set { _View = value; }
        }

        public CswPrimaryKey PropOwnerNodeId
        {
            get
            {
                CswPrimaryKey ret = null;
                if( HF_PropOwnerNodeId.Value != string.Empty )
                {
                    ret = new CswPrimaryKey();
                    ret.FromString( HF_PropOwnerNodeId.Value );
                }
                return ret;
            }
            set
            {   
                if ( value != null )
                    HF_PropOwnerNodeId.Value = value.ToString();
            }
        }

        public CswPrimaryKey ParentNodeId
        {
            get
            {
                if( HF_ParentNodeId.Value != string.Empty )
                    return new CswPrimaryKey( "nodes", Convert.ToInt32( HF_ParentNodeId.Value ) );
                else
                    return null;   // Top
            }
            set
            {
                if( value != null )
                    HF_ParentNodeId.Value = value.ToString();
            }
        }
        public String ParentNodeName
        {
            get
            {
                if (HF_ParentNodeName.Value != string.Empty)
                    return HF_ParentNodeName.Value;
                else
                {
                    if (ParentNodeId == null)
                        return CswNbtLocationTree.TopLevelName;
                    else
                        return "Unknown Node";
                }
            }
            set
            {
                HF_ParentNodeName.Value = value;
            }
        }
        public CswPrimaryKey SelectedNodeId
        {
            get
            {
                if( HF_SelectedNodeId.Value != string.Empty )
                    return new CswPrimaryKey( "nodes", Convert.ToInt32( HF_SelectedNodeId.Value ) );
                else
                    return null;
            }
            set
            {
                if( value != null )
                    HF_SelectedNodeId.Value = value.ToString();
            }
        }
        public Int32 SelectedColumn
        {
            get
            {
                if (HF_SelectedColumn.Value != string.Empty)
                    return Convert.ToInt32(HF_SelectedColumn.Value);
                else
                    return Int32.MinValue;
            }
            set
            {
                HF_SelectedColumn.Value = value.ToString();
            }
        }
        public Int32 SelectedRow
        {
            get
            {
                if (HF_SelectedRow.Value != string.Empty)
                    return Convert.ToInt32(HF_SelectedRow.Value);
                else
                    return Int32.MinValue;
            }
            set
            {
                HF_SelectedRow.Value = value.ToString();
            }
        }
        public string SelectedNodeName
        {
            get
            {
                string ret = String.Empty;
                if (SelectedNodeId != null && _CswNbtLocationTree != null)
                {
                    XmlNode SelectedNode = _CswNbtLocationTree.LocationTreeXml.SelectSingleNode("//" + CswNbtLocationTree.XmlNodeName_Child + "[" + CswNbtLocationTree.XmlNodeName_Key + " = \"" + SelectedNodeId + "\"]/" + CswNbtLocationTree.XmlNodeName_Display);
                    if (SelectedNode != null)
                        ret = SelectedNode.InnerText;
                }
                return ret;
            }
        }

        #endregion Properties

        #region Lifecycle

        public CswLocationNavigator(CswNbtResources CswNbtResources)//, bool UseUpdatePanel)
        {
            _CswNbtResources = CswNbtResources;
            EnsureChildControls();
        }

        private PlaceHolder _Holder = null;
        private CswBreadCrumb _Path = null;
        private CswLocationImage _LocationImage = null;
        
        private HiddenField HF_PropOwnerNodeId;
        private HiddenField HF_ParentNodeId;
        private HiddenField HF_ParentNodeName;
        private HiddenField HF_SelectedNodeId;
        private HiddenField HF_SelectedColumn;
        private HiddenField HF_SelectedRow;

        protected override void CreateChildControls()
        {
            try
            {
                Control ParentControl = this;

                _Path = new CswBreadCrumb();
                _Path.ID = "path";
                _Path.OnClick += new CswBreadCrumb.ClickHandler(BreadCrumb_Click);
                _Path.OnError += new CswErrorHandler(HandleError);
                ParentControl.Controls.Add(_Path);

                _Holder = new PlaceHolder();
                _Holder.ID = "holder";
                ParentControl.Controls.Add(_Holder);

                HF_PropOwnerNodeId = new HiddenField();
                HF_PropOwnerNodeId.ID = "HF_PropOwnerNodeId";
                ParentControl.Controls.Add(HF_PropOwnerNodeId);

                HF_ParentNodeId = new HiddenField();
                HF_ParentNodeId.ID = "HF_ParentNodeId";
                ParentControl.Controls.Add(HF_ParentNodeId);

                HF_ParentNodeName = new HiddenField();
                HF_ParentNodeName.ID = "HF_ParentNodeName";
                ParentControl.Controls.Add(HF_ParentNodeName);

                HF_SelectedNodeId = new HiddenField();
                HF_SelectedNodeId.ID = "HF_SelectedNodeId";
                ParentControl.Controls.Add(HF_SelectedNodeId);

                HF_SelectedColumn = new HiddenField();
                HF_SelectedColumn.ID = "HF_SelectedColumn";
                ParentControl.Controls.Add(HF_SelectedColumn);

                HF_SelectedRow = new HiddenField();
                HF_SelectedRow.ID = "HF_SelectedRow";
                ParentControl.Controls.Add(HF_SelectedRow);

                base.CreateChildControls();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        // IPostBackDataHandler
        public bool LoadPostData(String postDataKey, NameValueCollection values)
        {
            try
            {
                initImage();
                //if (!Page.IsPostBack)
                //    InitBreadCrumb();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                EnsureChildControls();
                initImage();
                if (!Page.IsPostBack)
                {
                    InitBreadCrumb();
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            base.OnLoad(e);
        }

        //public override void DataBind()
        //{
        //    try
        //    {
        //        EnsureChildControls();
        //        initImage();
        //        //if (!Page.IsPostBack)
        //        //    InitBreadCrumb();
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleError(ex);
        //    }
        //    base.DataBind();
        //}


        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                Page.RegisterRequiresPostBack(this);

                if( SelectedNodeId != null )
                    HF_SelectedNodeId.Value = SelectedNodeId.PrimaryKey.ToString();
                else
                    HF_SelectedNodeId.Value = String.Empty;
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string ParentNodeIdVal = CswNbtLocationTree.TopLevelName;
            string SelectedNodeIdVal = "Nothing selected";
            if (ParentNodeId != null)
            {
                ParentNodeIdVal = ParentNodeId.ToString();
            }
            if (SelectedNodeId != null)
            {
                SelectedNodeIdVal = SelectedNodeId.ToString();
            }
            writer.Write("ParentNodeId = " + ParentNodeIdVal + "<BR>");
            writer.Write("SelectedNodeId = " + SelectedNodeIdVal + "<BR>");
        }

        #endregion Lifecycle

        #region Events

        public event CswBreadCrumb.ClickHandler OnBreadCrumbClick = null;

        protected void BreadCrumb_Click(object sender, CswBreadCrumb.CswBreadCrumbClickEventArgs e)
        {
            try
            {
                if (e.ClickedKey.Substring(0, CswLocationImage.KeyPrefix.Length) == CswLocationImage.KeyPrefix)
                {
                    CswPrimaryKey NewNodeId = new CswPrimaryKey( "nodes", Convert.ToInt32( e.ClickedKey.Substring( CswLocationImage.KeyPrefix.Length ) ) );
                    ParentNodeId = NewNodeId;
                    ParentNodeName = e.ClickedText;
                    if (!MoveMode)
                        SelectedNodeId = NewNodeId;
                    initImage();
                }
                if (OnBreadCrumbClick != null)
                    OnBreadCrumbClick(sender, e);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Function name for client side function (which should receive the selected nodeid as a single parameter)
        /// </summary>
        public string OnClientSideLocationImageClick = string.Empty;

        public event CswLocationImage.ClickHandler OnLocationImageClick = null;

        protected void LocationImage_Click(object sender, CswLocationImage.CswLocationImageClickEventArgs e)
        {
            try
            {
                if (e.ClickedId.Length > CswLocationImage.KeyPrefix.Length &&
                    e.ClickedId.Substring(0, CswLocationImage.KeyPrefix.Length) == CswLocationImage.KeyPrefix)
                {
                    // Clicked a location node
                    CswPrimaryKey NewNodeId = new CswPrimaryKey( "nodes", Convert.ToInt32( e.ClickedId.Substring( CswLocationImage.KeyPrefix.Length ) ) );

                    if (!MoveMode || ParentNodeId == NewNodeId)
                    {
                        SelectedNodeId = NewNodeId;
                    }

                    if (ParentNodeId != NewNodeId)
                    {
                        ParentNodeId = NewNodeId;
                        ParentNodeName = getParentDisplay();
                        _Path.addLink(CswLocationImage.KeyPrefix + ParentNodeId, ParentNodeName);
                    }
                }
                else if (e.ClickedId.Length > CswLocationImage.GridKeyPrefix.Length &&
                         e.ClickedId.Substring(0, CswLocationImage.GridKeyPrefix.Length) == CswLocationImage.GridKeyPrefix)
                {
                    // Clicked a Grid cell
                    SelectedNodeId = ParentNodeId;
                    string GridCell = e.ClickedId.Substring(CswLocationImage.GridKeyPrefix.Length);
                    SelectedRow = Convert.ToInt32(GridCell.Substring(0, GridCell.LastIndexOf('_')));
                    SelectedColumn = Convert.ToInt32(GridCell.Substring(GridCell.LastIndexOf('_') + 1));
                }
                else if (e.ClickedId.Length > CswLocationImage.NodePrefix.Length &&
                         e.ClickedId.Substring(0, CswLocationImage.NodePrefix.Length) == CswLocationImage.NodePrefix)
                {
                    // Clicked a non-location node
                    SelectedNodeId = new CswPrimaryKey( "nodes", Convert.ToInt32( e.ClickedId.Substring( CswLocationImage.NodePrefix.Length ) ) );
                }

                initImage();

                if (OnLocationImageClick != null)
                    OnLocationImageClick(sender, e);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public event CswErrorHandler OnError;

        void HandleError(Exception ex)
        {
            if (OnError != null)
                OnError(ex);
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }


        #endregion Events

        #region Private Helpers

        private XmlNode getParentXmlNode()
        {
            XmlNode ret = null;
            if (_CswNbtLocationTree != null)
            {
                if (ParentNodeId != null)
                    ret = _CswNbtLocationTree.LocationTreeXml.SelectSingleNode("//" + CswNbtLocationTree.XmlNodeName_Child + "[" + CswNbtLocationTree.XmlNodeName_Key + " = \"" + ParentNodeId + "\"]");
                else
                    ret = _CswNbtLocationTree.LocationTreeXml.DocumentElement;
            }
            return ret;
        }
        public string getParentDisplay()
        {
            XmlNode ParentNode = getParentXmlNode();
            return ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_Display).InnerText;
        }
        private bool ParentNodeHasChildren()
        {
            bool ret = false;
            XmlNode ParentNode = getParentXmlNode();
            if (ParentNode != null)
            {
                XmlNode ChildSet = ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_ChildSet);
                if (ChildSet != null)
                {
                    ret = (ChildSet.ChildNodes.Count > 0);
                }
            }
            return ret;
        }


        private void initImage()
        {
            EnsureChildControls();
            
            string PropOwnerNodeIdString = String.Empty;
            if (PropOwnerNodeId != null )
                PropOwnerNodeIdString = PropOwnerNodeId.PrimaryKey.ToString();
            _CswNbtLocationTree = new CswNbtLocationTree( _CswNbtResources, ParentNodeId, ParentNodeName, 2, PropOwnerNodeIdString );

            XmlNode ParentNode = getParentXmlNode();

            if(_LocationImage != null)
                _Holder.Controls.Remove(_LocationImage);
            _LocationImage = CswLocationImage.makeCswLocationImage( _CswNbtResources, PropOwnerNodeId, ParentNode, SelectedNodeId, this.MoveMode, _View, OnClientSideLocationImageClick );
            _LocationImage.SelectedRow = SelectedRow;
            _LocationImage.SelectedColumn = SelectedColumn;
            _LocationImage.ID = "locationimage";
            _LocationImage.OnClick += new CswLocationImage.ClickHandler(LocationImage_Click);
            _Holder.Controls.Add(_LocationImage);
        }

        public void ClearSelected()
        {
            SelectedNodeId = null;
            SelectedRow = Int32.MinValue;
            SelectedColumn = Int32.MinValue;
            HF_SelectedNodeId.Value = SelectedNodeId.PrimaryKey.ToString();
            initImage();
        }

        protected void InitBreadCrumb()
        {
            _Path.Clear();
            XmlNode ParentNode = getParentXmlNode();
            if (ParentNode != null)
            {
                InitBreadCrumbRecursive(ParentNode);
            }
        }


        protected void InitBreadCrumbRecursive( CswPrimaryKey NodeId )
        {
            CswNbtNode NbtNode = _CswNbtResources.Nodes[NodeId];
            CswNbtPropEnmrtrFiltered PropEnmrtr = NbtNode.Properties[_CswNbtResources.MetaData.getFieldType(CswNbtMetaDataFieldType.NbtFieldType.Location)];
            PropEnmrtr.MoveNext();
            CswNbtNodePropLocation LocationProp = null;
            if (PropEnmrtr.Current != null)
            {
                LocationProp = ((CswNbtNodePropWrapper)PropEnmrtr.Current).AsLocation;
                if (LocationProp.SelectedNodeId != null)
                    InitBreadCrumbRecursive(LocationProp.SelectedNodeId);
                _Path.addLink( CswLocationImage.KeyPrefix + LocationProp.SelectedNodeId, LocationProp.CachedNodeName );
            }
        }

        protected void InitBreadCrumbRecursive(XmlNode Node)
        {
            CswPrimaryKey NodeId = null;
            if( Node.SelectSingleNode( CswNbtLocationTree.XmlNodeName_Key ).InnerText != string.Empty )
                NodeId = new CswPrimaryKey( "nodes", Convert.ToInt32( Node.SelectSingleNode( CswNbtLocationTree.XmlNodeName_Key ).InnerText ) );

            if (Node.ParentNode != null && Node.ParentNode.ParentNode != null)
            {
                InitBreadCrumbRecursive(Node.ParentNode.ParentNode);
            }
            else
            {
                if (NodeId != null)
                    InitBreadCrumbRecursive(NodeId);
            }
            _Path.addLink(CswLocationImage.KeyPrefix + NodeId,
                          Node.SelectSingleNode(CswNbtLocationTree.XmlNodeName_Display).InnerText);
        }

        #endregion Private Helpers
    }

}
