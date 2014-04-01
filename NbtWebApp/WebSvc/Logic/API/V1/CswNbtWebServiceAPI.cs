using System.Net;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.WebSvc.Logic.API
{
    public abstract class CswNbtWebServiceAPI
    {
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
                ret.addProperty(NewProp);
            }
            return ret;
        }
    }
}