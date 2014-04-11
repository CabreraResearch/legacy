using System;
using System.Net;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
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
                    CswNbtNode NewNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, IsTemp : GenericRequest.Properties.Count > 0, OnAfterMakeNode : delegate( CswNbtNode node )
                        {
                            foreach( CswNbtWcfProperty WcfProp in GenericRequest.Properties )
                            {
                                ReadPropertyData( node, WcfProp );
                            }
                        } );


                    Return.NodeId = NewNode.NodeId;
                    Return.NodeName = NewNode.NodeName;
                    Return.NodeType = NewNode.getNodeType().NodeTypeName;
                    Return.ObjectClass = NewNode.ObjClass.ObjectClass.ObjectClassName;
                    Return.URI = BuildURI( NodeType.NodeTypeName, NewNode.NodeId.PrimaryKey );

                    CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                    Return.PropertyData = ConvertPropertyData( CswConvert.ToJObject( SdTabsAndProps.getProps( NewNode, string.Empty, null, CswEnumNbtLayoutType.Add )["properties"] ) );

                    Return.Status = HttpStatusCode.Created;
                }
                catch( Exception )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                Return.Status = HttpStatusCode.Forbidden;
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