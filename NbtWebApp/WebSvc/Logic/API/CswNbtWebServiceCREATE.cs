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

        protected override bool hasPermission( CswNbtAPIRequest Request, CswNbtAPIReturn Return )
        {
            return base.hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.Create, Request, Return );
        }

        public void Create( CswNbtResourceWithProperties Return, CswNbtAPIRequest Request )
        {
            if( hasPermission( Request, Return ) )
            {
                try
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Request.MetaDataName );
                    CswNbtNode NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, IsTemp : true );

                    Return.NodeId = NewNode.NodeId;
                    Return.NodeName = NewNode.NodeName;
                    Return.NodeType = NewNode.getNodeType().NodeTypeName;
                    Return.ObjectClass = NewNode.ObjClass.ObjectClass.ObjectClassName;
                    Return.URI = "api/v1/" + NodeType.NodeTypeName + "/" + NewNode.NodeId.PrimaryKey; //TODO: this URI is incomplete

                    CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                    //TODO: see CswNbtWebServiceGET for the problems with the line below
                    Return.PropertyData = CswConvert.ToJObject( SdTabsAndProps.getProps( NewNode, string.Empty, null, CswEnumNbtLayoutType.Add )["properties"] );

                    Return.Status = HttpStatusCode.Created;
                }
                catch( Exception ex )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
        }

        #endregion

        #region Static

        public static void Create( ICswResources CswResources, CswNbtResourceWithProperties Return, CswNbtAPIRequest Request )
        {
            CswNbtWebServiceCREATE POST = new CswNbtWebServiceCREATE( (CswNbtResources) CswResources );
            POST.Create( Return, Request );
        }

        #endregion

    }
}