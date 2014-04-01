using System;
using System.Net;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceCREATE: CswNbtWebServiceAPI
    {
        public const string VERB = "POST";

        #region Non Static

        public CswNbtWebServiceCREATE( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            return base.hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.Create, GenericRequest, Return );
        }

        public void Create( CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            if( hasPermission( GenericRequest, Return ) )
            {
                try
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( GenericRequest.MetaDataName );
                    CswNbtNode NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, IsTemp : true );

                    Return.NodeId = NewNode.NodeId;
                    Return.NodeName = NewNode.NodeName;
                    Return.NodeType = NewNode.getNodeType().NodeTypeName;
                    Return.ObjectClass = NewNode.ObjClass.ObjectClass.ObjectClassName;
                    Return.URI = "api/v1/" + NodeType.NodeTypeName + "/" + NewNode.NodeId.PrimaryKey; //TODO: this URI is incomplete

                    CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                    Return.PropertyData = ConvertPropertyData( CswConvert.ToJObject( SdTabsAndProps.getProps( NewNode, string.Empty, null, CswEnumNbtLayoutType.Add )["properties"] ) );

                    Return.Status = HttpStatusCode.Created;
                }
                catch( Exception )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
        }

        #endregion

        #region Static

        public static void Create( ICswResources CswResources, CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            CswNbtWebServiceCREATE POST = new CswNbtWebServiceCREATE( (CswNbtResources) CswResources );
            POST.Create( Return, GenericRequest );
        }

        #endregion

    }
}