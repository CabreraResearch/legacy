using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Core;

namespace ChemSW.NbtWebControls
{

    public class CswLocationImage : CompositeControl
    {

        protected bool _MoveMode = false;

        protected string ImageUrlBase = "Images/location/";

        public static string KeyPrefix = "lockey_";
        public static string NodePrefix = "nodeid_";
        public static string GridKeyPrefix = "gridcell_";

        //protected string _TitleText = "";
        protected CswPrimaryKey _PropOwnerNodeId = null;
        protected XmlNode _ParentNodeSet;
        protected XmlNode _ParentNode;
        protected CswPrimaryKey _SelectedNodeId = null;
        public Int32 SelectedRow = Int32.MinValue;
        public Int32 SelectedColumn = Int32.MinValue;

        private CswNbtView _View;
        protected CswNbtView View
        {
            get { return _View; }
            set { _View = value; }
        }
        
        protected string OnClientSideLocationImageClick= string.Empty;

        public static CswLocationImage makeCswLocationImage( CswNbtResources CswNbtResources, CswPrimaryKey PropOwnerNodeId, XmlNode ParentNode, CswPrimaryKey SelectedNodeId, bool MoveMode, CswNbtView View, string OnClientSideLocationImageClick )
        {
            CswLocationImage Ret = null;

            CswNbtLocationTree.LocationType Type = CswNbtLocationTree.LocationType.Unknown;
            XmlNode ChildSetNode = ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_ChildSet);
            if (ChildSetNode != null)
            {
                XmlAttribute LocationTypeAttribute = ChildSetNode.Attributes[CswNbtLocationTree.XmlAttrName_LocationType];
                if (LocationTypeAttribute != null)
                {
                    string TypeString = LocationTypeAttribute.Value;
                    try
                    {
                        Type = (CswNbtLocationTree.LocationType)Enum.Parse(typeof(CswNbtLocationTree.LocationType), TypeString, true);
                    }
                    catch (Exception e)
                    {
                        throw new CswDniException("An internal error occurred", "Invalid LocationType: " + Type.ToString(), e);
                    }
                }
            }

            switch (Type)
            {
                case CswNbtLocationTree.LocationType.Horizontal:
                    Ret = new CswLocationImageHorizontal( CswNbtResources, ParentNode, SelectedNodeId );
                    break;
                case CswNbtLocationTree.LocationType.Vertical:
                    Ret = new CswLocationImageVertical( CswNbtResources, ParentNode, SelectedNodeId );
                    break;
                case CswNbtLocationTree.LocationType.Grid:
                    Ret = new CswLocationImageGrid( CswNbtResources, ParentNode, SelectedNodeId );
                    break;
                default:
                    Ret = new CswLocationImageHorizontal( CswNbtResources, ParentNode, SelectedNodeId );
                    break;
                    //throw new CswDniException("Unhandled LocationType: " + Type.ToString());
            }
            Ret._PropOwnerNodeId = PropOwnerNodeId;
            Ret._MoveMode = MoveMode;
            Ret.View = View;
            Ret.OnClientSideLocationImageClick = OnClientSideLocationImageClick;
            return Ret;
        }
        
        protected CswNbtResources _CswNbtResources;

        public CswLocationImage( CswNbtResources CswNbtResources, XmlNode ParentNode, CswPrimaryKey SelectedNodeId )
        {
            _CswNbtResources = CswNbtResources;
            _ParentNode = ParentNode;
            _ParentNodeSet = ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_ChildSet);
            //_TitleText = ParentNode.SelectSingleNode(XmlNodeName_Display).InnerText;
            _SelectedNodeId = SelectedNodeId;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        public delegate void ClickHandler(object sender, CswLocationImageClickEventArgs e);

        public class CswLocationImageClickEventArgs : EventArgs
        {
            public string ClickedId;

            public CswLocationImageClickEventArgs()
            {
            }
            public CswLocationImageClickEventArgs(string Id)
            {
                ClickedId = Id;
            }
        }

        public event ClickHandler OnClick = null;

        protected void Image_Click(object sender, EventArgs e)
        {
            if (sender is Button && OnClick != null)
            {
                OnClick(this, new CswLocationImageClickEventArgs(((Button)sender).Text));
            }
        }
    }

}