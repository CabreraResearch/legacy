using System;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;

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

            CswNbtLocationTreeDeprecated.LocationType Type = CswNbtLocationTreeDeprecated.LocationType.Unknown;
            XmlNode ChildSetNode = ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_ChildSet );
            if (ChildSetNode != null)
            {
                XmlAttribute LocationTypeAttribute = ChildSetNode.Attributes[CswNbtLocationTreeDeprecated.XmlAttrName_LocationType];
                if (LocationTypeAttribute != null)
                {
                    string TypeString = LocationTypeAttribute.Value;
                    try
                    {
                        Type = (CswNbtLocationTreeDeprecated.LocationType)Enum.Parse(typeof(CswNbtLocationTreeDeprecated.LocationType), TypeString, true);
                    }
                    catch (Exception e)
                    {
						throw new CswDniException( CswEnumErrorType.Error, "An internal error occurred", "Invalid LocationType: " + Type.ToString(), e );
                    }
                }
            }

            switch (Type)
            {
                case CswNbtLocationTreeDeprecated.LocationType.Horizontal:
                    Ret = new CswLocationImageHorizontal( CswNbtResources, ParentNode, SelectedNodeId );
                    break;
                case CswNbtLocationTreeDeprecated.LocationType.Vertical:
                    Ret = new CswLocationImageVertical( CswNbtResources, ParentNode, SelectedNodeId );
                    break;
                case CswNbtLocationTreeDeprecated.LocationType.Grid:
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
            _ParentNodeSet = ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_ChildSet );
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