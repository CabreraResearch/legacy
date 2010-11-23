using System;
using System.Xml;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.Session;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceUpdateProperties
    {
        private CswNbtWebServiceResources _CswNbtWebServiceResources;
        public CswNbtWebServiceUpdateProperties( CswNbtWebServiceResources CswNbtWebServiceResources )
        {
            _CswNbtWebServiceResources = CswNbtWebServiceResources;
        }

        public string Run( string ParentId, string UpdatedViewXml )
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml( UpdatedViewXml );

            XmlNodeList PropNodes = XmlDoc.SelectNodes( "result/node/subitems/prop[@wasmodified='1']" );
            foreach( XmlNode PropNode in PropNodes )
            {
                string NodePropId = PropNode.Attributes["id"].Value;
                string[] SplitNodePropId = NodePropId.Split( '_' );
                Int32 NodeTypePropId = Convert.ToInt32( SplitNodePropId[1] );
                CswPrimaryKey NodePk = new CswPrimaryKey( SplitNodePropId[3], Convert.ToInt32( SplitNodePropId[4] ) );

                CswNbtNode Node = _CswNbtWebServiceResources.CswNbtResources.Nodes[NodePk];
                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtWebServiceResources.CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );
                Node.postChanges( false );
            }

            // return the refreshed view
            CswNbtWebServiceView ViewService = new CswNbtWebServiceView(_CswNbtWebServiceResources);
            return ViewService.Run( ParentId );

        } // Run()

    } // class CswNbtWebServiceUpdateProperties

} // namespace ChemSW.WebServices

