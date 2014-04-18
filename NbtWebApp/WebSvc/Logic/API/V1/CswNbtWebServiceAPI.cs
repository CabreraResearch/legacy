using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Logic.API.DataContracts;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.WebSvc.Logic.API
{
    public abstract class CswNbtWebServiceAPI
    {
        public static string AppPath = string.Empty;

        public static int VersionNo = 1;

        protected CswNbtResources _CswNbtResources;
        protected abstract bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return );

        /// <summary>
        /// Based on the input, verifies if the user has permission to continue
        /// </summary>
        public bool hasPermission( CswNbtResources NbtResources, CswEnumNbtNodeTypePermission Permission, CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            bool ret = false;
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( GenericRequest.MetaDataName );
            if( null != NodeType )
            {
                if( NbtResources.Permit.canAnyTab( Permission, NodeType, User : NbtResources.CurrentNbtUser ) )
                {
                    ret = true;
                }
                else
                {
                    Return.Status = Return.Status = HttpStatusCode.Forbidden; //Permission denied
                }
            }
            else
            {
                Return.Status = Return.Status = HttpStatusCode.NotFound;
            }
            return ret;
        }

        public static string BuildURI( string MetaDataName, int id = Int32.MinValue )
        {
            string appUri = string.Empty;
            //We need to extract the full application URI from the request url
            if( null != HttpContext.Current ) //This is null for unit tests
            {
                appUri = ( HttpContext.Current.Request.Url.IsDefaultPort ) ? HttpContext.Current.Request.Url.Host : HttpContext.Current.Request.Url.Authority;
                appUri = String.Format( "{0}://{1}", HttpContext.Current.Request.Url.Scheme, appUri );
                if( HttpContext.Current.Request.ApplicationPath != "/" )
                {
                    appUri += HttpContext.Current.Request.ApplicationPath;
                }
            }

            string ret = appUri + "/api/v" + VersionNo + "/" + MetaDataName;
            if( Int32.MinValue != id )
            {
                ret += "/" + id;
            }
            return ret;
        }

        /// <summary>
        /// Gets property data for a given Node and returns it in our WCF object format
        /// <returns>All visible non-UI-specific properties on the Node's Edit layout for which the current user has view permissions</returns>
        /// </summary>
        public CswNbtWcfPropCollection GetPropertyData( CswNbtNode Node )
        {
            CswNbtWcfPropCollection PropCollection = new CswNbtWcfPropCollection();
            List<Int32> PropIdsAdded = new List<Int32>();
            CswNbtMetaDataNodeType NodeType = Node.getNodeType();
            foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.getNodeTypeTabs() )
            {
                if( _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.View, NodeType, Tab ) )
                {
                    IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Node.NodeTypeId, Tab.TabId, CswEnumNbtLayoutType.Edit );
                    foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
                    {
                        if( false == PropIdsAdded.Contains( Prop.PropId ) && //Don't grab the same prop more than once
                            Prop.getFieldTypeValue() != CswEnumNbtFieldType.Button && //Exclude these prop types
                            Prop.getFieldTypeValue() != CswEnumNbtFieldType.Password && 
                            false == Prop.Hidden ) //Don't grab hidden props
                        {
                            CswNbtWcfProperty WCFProp = new CswNbtWcfProperty
                                {
                                    PropId = Prop.PropId.ToString(),
                                    PropName = Prop.PropName,
                                    OriginalPropName = Prop.getObjectClassPropName()
                                };
                            JObject PropVals = new JObject();
                            Node.Properties[Prop].ToJSON( PropVals );
                            foreach( JProperty OldPropValue in PropVals["values"].Children() )
                            {
                                WCFProp.values[OldPropValue.Name] = OldPropValue.Value.ToString();
                            }
                            PropCollection.addProperty( WCFProp );
                            PropIdsAdded.Add( Prop.PropId );
                        }
                    }
                }
            }
            return PropCollection;
        }

        public JObject ConvertWcfPropertyData( CswNbtWcfProperty Prop )
        {
            JObject ret = new JObject();
            ret["id"] = Prop.PropId;
            ret["name"] = Prop.PropName;
            ret["ocpname"] = Prop.OriginalPropName;
            JObject values = new JObject();
            foreach( string subFieldStr in Prop.values.Keys )
            {
                string subFieldStrOrig = subFieldStr.Replace( '_', ' ' );
                object subFieldVal = Prop.values[subFieldStr];
                JObject subFieldObj = CswConvert.ToJObject( subFieldVal.ToString() );
                if( subFieldObj.HasValues )
                {
                    //Some out our subfield values are actually JObjects and must be added as such.
                    values.Add( subFieldStrOrig, subFieldObj );   
                }
                else
                {
                    values.Add( subFieldStrOrig, subFieldVal.ToString() );   
                }
            }
            ret["values"] = values;

            return ret;
        }

        public void ReadPropertyData( CswNbtNode Node, CswNbtWcfProperty WcfProp )
        {
            CswNbtMetaDataNodeType NodeType = Node.getNodeType();
            CswNbtNodePropWrapper propWrapper = null;
            if( false == String.IsNullOrEmpty( WcfProp.OriginalPropName ) )
            {
                propWrapper = Node.Properties[WcfProp.OriginalPropName];
            }
            else
            {
                CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( WcfProp.PropName );
                propWrapper = Node.Properties[ntp];
            }

            JObject propData = ConvertWcfPropertyData( WcfProp );
            propWrapper.ReadJSON( propData, null, null );
        }

    }
}