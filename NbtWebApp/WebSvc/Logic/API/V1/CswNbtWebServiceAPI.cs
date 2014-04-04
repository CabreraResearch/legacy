using System;
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
                if( NbtResources.Permit.canNodeType( Permission, NodeType, User : NbtResources.CurrentNbtUser ) )
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
        /// Converts property data from a JObject to WCF
        /// See CIS-53051
        /// </summary>
        public CswNbtWcfPropCollection ConvertPropertyData( JObject PropData )
        {
            CswNbtWcfPropCollection ret = new CswNbtWcfPropCollection();

            foreach( JProperty OldProp in PropData.Children() )
            {
                CswNbtWcfProperty NewProp = new CswNbtWcfProperty();
                NewProp.PropId = OldProp.Value["id"].ToString();
                NewProp.PropName = OldProp.Value["name"].ToString();
                NewProp.OriginalPropName = OldProp.Value["ocpname"].ToString();
                foreach( JProperty OldPropValue in OldProp.Value["values"].Children() )
                {
                    NewProp.values[OldPropValue.Name] = OldPropValue.Value.ToString();
                }
                ret.addProperty( NewProp );
            }
            return ret;
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