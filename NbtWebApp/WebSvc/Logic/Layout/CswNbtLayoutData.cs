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

        [DataMember( Name = "displaycol" )]
        public int DisplayColumn { get; set; }

        [DataMember( Name = "displayrow" )]
        public int DisplayRow { get; set; }
    }

    [DataContract]
    public class CswNbtTabMoveRequest
    {
        [DataMember]
        public int TabId { get; set; }

        [DataMember]
        public int OldPosition { get; set; }

        [DataMember]
        public int NewPosition { get; set; }
    }
}