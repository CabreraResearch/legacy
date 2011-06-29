using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMobileUpdateProperties
    {
        private CswNbtResources _CswNbtResources;
        private bool _ForMobile;

        public CswNbtWebServiceMobileUpdateProperties( CswNbtResources CswNbtResources, bool ForMobile )
        {
            _CswNbtResources = CswNbtResources;
            _ForMobile = ForMobile;
        }

        public XElement Run( string ParentId, string UpdatedViewXml )
        {
            string ViewXml = UpdatedViewXml.Replace( @"xmlns=""http://www.w3.org/1999/xhtml""", string.Empty );
            XElement AllProps = XElement.Parse( ViewXml );
            IEnumerable<XElement> Props = ( from Element in AllProps.Descendants().Elements( "prop" )
                                            where ( null != Element.Attribute( "wasmodified" ) &&
                                                   Element.Attribute( "wasmodified" ).Value == "1" )
                                            select Element );

            // post changes once per node, not once per prop            
            Collection<CswNbtNode> NodesToPost = new Collection<CswNbtNode>();

            foreach( XElement Prop in Props )
            {
                if( null != Prop.Attribute( "id" ) )
                {
                    string NodePropId = Prop.Attribute( "id" ).Value;
                    string[] SplitNodePropId = NodePropId.Split( '_' );
                    Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[1] );
                    CswPrimaryKey NodePk = new CswPrimaryKey( SplitNodePropId[3], CswConvert.ToInt32( SplitNodePropId[4] ) );

                    CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
                    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );

                    XmlDocument Doc = new XmlDocument();
                    XmlNode PropNode = Doc.ReadNode( Prop.CreateReader() );

                    Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );

                    if( !NodesToPost.Contains( Node ) )
                        NodesToPost.Add( Node );
                }
            }

            foreach( CswNbtNode Node in NodesToPost )
                Node.postChanges( false );

            // return the refreshed view
            CswNbtWebServiceMobileView ViewService = new CswNbtWebServiceMobileView( _CswNbtResources, _ForMobile );
            return ViewService.getView( ParentId, _CswNbtResources.CurrentNbtUser );

        } // Run()

    } // class CswNbtWebServiceMobileUpdateProperties

} // namespace ChemSW.WebServices

