using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02C_Case29935: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29935; }
        }

        public override void update()
        {
            CswNbtNode CisproRequestFulfillerRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Request_Fulfiller" );
            if( null != CisproRequestFulfillerRole )
            {
                LandingPageData PageData = _CswNbtSchemaModTrnsctn.getLandingPageTable().getLandingPageItems( new LandingPageData.Request
                    {
                        RoleId = CisproRequestFulfillerRole.NodeId.ToString(),
                        ActionId = null
                    } );

                Collection<Int32> DoomedIds = new Collection<int>();
                foreach( LandingPageData.LandingPageItem PageItem in PageData.LandingPageItems )
                {
                    if( PageItem.Text == "Pending Requests" )
                    {
                        _CswNbtSchemaModTrnsctn.getLandingPageTable().addLandingPageItem( new LandingPageData.Request
                            {
                                Type = CswEnumNbtLandingPageItemType.Link,
                                RoleId = CisproRequestFulfillerRole.NodeId.ToString(),
                                Text = "Submitted Requests",
                                NodeViewId = PageItem.ViewId,
                                PkValue = PageItem.ViewId,
                                ButtonIcon = "cart.png",
                                ViewType = CswEnumNbtViewType.View.ToString()
                            } );
                        DoomedIds.Add( CswConvert.ToInt32( PageItem.LandingPageId ) );
                    }
                }
                foreach( int DoomedId in DoomedIds )
                {
                    _CswNbtSchemaModTrnsctn.getLandingPageTable().deleteLandingPageItem( new LandingPageData.Request { LandingPageId = DoomedId } );
                }

            }

            foreach( CswEnumNbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( Member );
                foreach( CswNbtMetaDataNodeType NodeType in MemberOc.getLatestVersionNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp CommentsNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Comments );
                    CommentsNtp.updateLayout( CswEnumNbtLayoutType.Add, false );
                }
            }

            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
            foreach( CswNbtMetaDataNodeType RequestNt in RequestOc.getNodeTypes() )
            {
                RequestNt.NameTemplateValue = "";
                RequestNt.addNameTemplateText( CswNbtObjClassRequest.PropertyName.Name );
                RequestNt.addNameTemplateText( CswNbtObjClassRequest.PropertyName.SubmittedDate );

                if( null != CisproRequestFulfillerRole )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestNt.NodeTypeId, true );
                }
            }

        } // update()


    }//class CswUpdateSchema_02C_CaseXXXXX

}//namespace ChemSW.Nbt.Schema