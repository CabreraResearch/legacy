using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropGrid : CswNbtNodeProp
    {

        public CswNbtNodePropGrid( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
        }//generic

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt

        public CswNbtView View
        {
            get
            {
                //CswNbtView Ret = new CswNbtView(_CswNbtResources);
                CswNbtView Ret = null;
                if( _CswNbtMetaDataNodeTypeProp.ViewId.isSet() )
                    //Ret.LoadXml(_CswNbtMetaDataNodeTypeProp.ViewId);
                    Ret = _CswNbtResources.ViewSelect.restoreView( _CswNbtMetaDataNodeTypeProp.ViewId );
                return Ret;
            }
        }
        public Int32 Width
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaRows = value;
            //}
        }

        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, "viewname", View.ViewName );
            CswXmlDocument.AppendXmlNode( ParentNode, "viewid", View.ViewId.ToString() );
            CswXmlDocument.AppendXmlNode( ParentNode, "width", Width.ToString() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( "viewname", View.ViewName ),
                new XElement( "viewid", View.ViewId.ToString() ),
                new XElement( "width", Width.ToString() ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject["viewname"] = View.ViewName;
            ParentObject["viewid"] = View.ViewId.ToString();
            ParentObject["width"] = Width.ToString();
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            // Nothing to restore
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
