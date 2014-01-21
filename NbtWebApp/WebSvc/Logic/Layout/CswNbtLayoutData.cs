using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Nbt.MetaData;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Layout
{
    [DataContract]
    public class CswNbtLayoutDataReturn: CswWebSvcReturn
    {
        public CswNbtLayoutDataReturn()
        {
            Data = new CswNbtNodeTypeLayout();
        }

        [DataMember]
        public CswNbtNodeTypeLayout Data;
    }

    [DataContract]
    public class CswNbtNodeTypeLayout
    {
        private string _imgLink = string.Empty;
        [DataMember( Name = "imagelink" )]
        public string ImageLink
        {
            get { return _imgLink; }
            set { _imgLink = value; }
        }

        [DataMember( Name = "props" )]
        public Collection<CswNbtLayoutProp> Props = new Collection<CswNbtLayoutProp>();

        private CswEnumNbtLayoutType _layout = null;
        public CswEnumNbtLayoutType Layout
        {
            get
            {
                if( null == _layout )
                {
                    _layout = _layoutStr;
                }
                return _layout;
            }
            set
            {
                _layout = value;
                _layoutStr = _layout.ToString();
            }
        }

        private string _layoutStr = string.Empty;
        [DataMember( Name = "layout" )]
        private string LayoutStr
        {
            get { return _layoutStr; }
            set { _layoutStr = value; }
        }

        private int _nodeTypeId = Int32.MinValue;
        [DataMember( Name = "nodetypeid" )]
        public int NodeTypeId
        {
            get { return _nodeTypeId; }
            set
            {
                _nodeTypeId = value;
            }
        }

        [DataMember( Name = "tabid" )]
        public int TabId { get; set; }
    }

    [DataContract]
    public class CswNbtLayoutProp
    {
        private int _nodeTypePropId = Int32.MinValue;
        [DataMember( Name = "nodetypepropid" )]
        public int NodeTypePropId
        {
            get { return _nodeTypePropId; }
            set
            {
                _nodeTypePropId = value;
            }
        }

        [DataMember( Name = "tabgroup" )]
        public string TabGroup { get; set; }

        private int _DisplayColumn = Int32.MinValue;
        [DataMember( Name = "displaycol" )]
        public int DisplayColumn
        {
            get { return _DisplayColumn; }
            set { _DisplayColumn = value; }
        }

        private int _DisplayRow = Int32.MinValue;
        [DataMember( Name = "displayrow" )]
        public int DisplayRow
        {
            get { return _DisplayRow; }
            set { _DisplayRow = value; }
        }

        private bool _DoMove = true;
        /// <summary>
        /// Determines whether or not the prop should be moved from an existing layout
        /// </summary>
        [DataMember( Name = "domove" )]
        public bool DoMove
        {
            get { return _DoMove; }
            set { _DoMove = value; }
        }
    }
}