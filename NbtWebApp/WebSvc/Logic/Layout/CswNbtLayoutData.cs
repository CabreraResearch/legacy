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
            Data = new CswNbtLayoutData();
        }

        [DataMember]
        public CswNbtLayoutData Data;
    }

    [DataContract]
    public class CswNbtLayoutDataCollection
    {
        [DataMember]
        public Collection<CswNbtLayoutData> Props = new Collection<CswNbtLayoutData>();
    }

    [DataContract]
    public class CswNbtLayoutData
    {
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

        //private CswNbtMetaDataNodeType _nodeType;
        //public CswNbtMetaDataNodeType NodeType
        //{
        //    get
        //    {
        //        if( null == _nodeType && Int32.MinValue != _nodeTypeId )
        //        {
        //            _nodeType = CswNbtResources.MetaData.getNodeType( _nodeTypeId );
        //        }
        //        return _nodeType;
        //    }
        //    set
        //    {
        //        _nodeType = value;
        //        _nodeTypeId = ( null != value ? value.NodeTypeId : Int32.MinValue );
        //    }
        //}

        //private CswNbtMetaDataNodeTypeProp _nodeTypeProp;
        //public CswNbtMetaDataNodeTypeProp NodeTypeProp
        //{
        //    get
        //    {
        //        if( null == _nodeTypeProp )
        //        {
        //            _nodeTypeProp = CswNbtResources.MetaData.getNodeTypeProp( _nodeTypePropId );
        //        }
        //        return _nodeTypeProp;
        //    }
        //    set
        //    {
        //        _nodeTypeProp = value;
        //        _nodeTypePropId = ( null != value ? value.PropId : Int32.MinValue );
        //    }
        //}

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
                //_nodeType = null;
            }
        }

        private int _nodeTypePropId = Int32.MinValue;
        [DataMember( Name = "nodetypepropid" )]
        public int NodeTypePropId
        {
            get { return _nodeTypePropId; }
            set
            {
                _nodeTypePropId = value;
                //_nodeTypeProp = null;
            }
        }

        [DataMember( Name = "displaycol" )]
        public int DisplayColumn { get; set; }

        [DataMember( Name = "displayrow" )]
        public int DisplayRow { get; set; }

        [DataMember( Name = "tabid" )]
        public int TabId { get; set; }
    }
}