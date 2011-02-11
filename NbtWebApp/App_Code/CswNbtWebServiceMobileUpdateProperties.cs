using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ChemSW.NbtWebControls;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMobileUpdateProperties
    {
        private CswNbtWebServiceResources _CswNbtWebServiceResources;
        private bool _ForMobile;

        public CswNbtWebServiceMobileUpdateProperties( CswNbtWebServiceResources CswNbtWebServiceResources, bool ForMobile )
        {
            _CswNbtWebServiceResources = CswNbtWebServiceResources;
            _ForMobile = ForMobile;
        }

        public string Run( string ParentId, string UpdatedViewXml )
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml( UpdatedViewXml );

            XmlNodeList PropNodes = XmlDoc.SelectNodes( "//prop[@wasmodified='1']" );

            // post changes once per node, not once per prop            
            Collection<CswNbtNode> NodesToPost = new Collection<CswNbtNode>();

            foreach( XmlNode PropNode in PropNodes )
            {
                string NodePropId = PropNode.Attributes["id"].Value;
                string[] SplitNodePropId = NodePropId.Split( '_' );
                Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[1] );
                CswPrimaryKey NodePk = new CswPrimaryKey( SplitNodePropId[3], CswConvert.ToInt32( SplitNodePropId[4] ) );

                CswNbtNode Node = _CswNbtWebServiceResources.CswNbtResources.Nodes[NodePk];
                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtWebServiceResources.CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );

                if( !NodesToPost.Contains( Node ) )
                    NodesToPost.Add( Node );
            }

            foreach( CswNbtNode Node in NodesToPost )
                Node.postChanges( false );

            // return the refreshed view
            CswNbtWebServiceMobileView ViewService = new CswNbtWebServiceMobileView( _CswNbtWebServiceResources, _ForMobile );
            return ViewService.Run( ParentId );

        } // Run()

    } // class CswNbtWebServiceMobileUpdateProperties

} // namespace ChemSW.WebServices

