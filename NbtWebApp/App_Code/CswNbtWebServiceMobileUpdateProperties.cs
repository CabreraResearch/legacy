using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

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

        public JObject Run( string ParentId, string UpdatedViewJson )
        {
            JObject UpdatedView = JObject.Parse( UpdatedViewJson );
            Collection<JProperty> Props = new Collection<JProperty>();

            foreach( JProperty JProp in UpdatedView.Properties() )
            {
                JObject NodeAttr = (JObject) JProp.Value;
                JObject SubItems = (JObject) NodeAttr.Property( "subitems" ).Value;
                foreach( JProperty NodeProp in SubItems.Properties() )
                {
                    JObject PropAttr = (JObject) NodeProp.Value;
                    if( null != PropAttr.Property( "wasmodified" ) )
                    {
                        Props.Add( NodeProp );
                    }
                }
            }

            // post changes once per node, not once per prop            
            Collection<CswNbtNode> NodesToPost = new Collection<CswNbtNode>();

            foreach( JProperty Prop in Props )
            {
                if( null != Prop.Name )
                {
                    string NodePropId = Prop.Name; // ~ "prop_4019_nodeid_nodes_24709"
                    string[] SplitNodePropId = NodePropId.Split( '_' );
                    Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[1] );
                    CswPrimaryKey NodePk = new CswPrimaryKey( SplitNodePropId[3], CswConvert.ToInt32( SplitNodePropId[4] ) );

                    CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
                    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );

                    JObject PropObj = (JObject) Prop.Value;
                   
                    Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null );

                    if( !NodesToPost.Contains( Node ) )
                    {
                        NodesToPost.Add( Node );
                    }
                }
            }

            foreach( CswNbtNode Node in NodesToPost )
            {
                Node.postChanges( false );
            }

                // return the refreshed view
            CswNbtWebServiceMobileView ViewService = new CswNbtWebServiceMobileView( _CswNbtResources, _ForMobile );
            return ViewService.getView( ParentId, _CswNbtResources.CurrentNbtUser );

        } // Run()

    } // class CswNbtWebServiceMobileUpdateProperties

} // namespace ChemSW.WebServices

