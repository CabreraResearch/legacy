using System.Net;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public abstract class CswNbtWebServiceAPI
    {
        protected CswNbtResources _CswNbtResources;
        protected abstract bool hasPermission( CswNbtAPIRequest Request, CswNbtAPIReturn Return );

        /// <summary>
        /// Based on the input, verifies if the user has permission to continue
        /// </summary>
        public bool hasPermission( CswNbtResources NbtResources, CswEnumNbtNodeTypePermission Permission, CswNbtAPIRequest Request, CswNbtAPIReturn Return )
        {
            bool ret = false;
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Request.MetaDataName );
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
    }
}