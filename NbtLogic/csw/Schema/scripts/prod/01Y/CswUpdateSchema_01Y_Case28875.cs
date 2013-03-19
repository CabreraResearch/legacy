using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.LandingPage;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01Y_Case28875 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28875; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RoleMDObjC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RoleClass );

            CswNbtObjClassRole cispro_receiver_role = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Receiver" );
            CswNbtObjClassRole cispro_dispenser_role = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Dispenser" ); ;
            CswNbtObjClassRole cispro_request_fulfiller_role = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Request_Fulfiller" ); ;
            CswNbtObjClassRole cispro_admin_role = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" ); ;


            CswNbtObjClassUser cispro_dispenser_User = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( "cispro_dispenser" );


            if( null != cispro_receiver_role )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( Actions.CswNbtActionName.DispenseContainer, cispro_receiver_role, false );
                _CswNbtSchemaModTrnsctn.Permit.set( Actions.CswNbtActionName.DisposeContainer, cispro_receiver_role, false );
            }//if we have have a cispro_receiver_role


            if( null != cispro_admin_role )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( Actions.CswNbtActionName.KioskMode, cispro_admin_role, true );
            }

            if( null != cispro_request_fulfiller_role )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( Actions.CswNbtActionName.DispenseContainer, cispro_request_fulfiller_role, true );
                _CswNbtSchemaModTrnsctn.Permit.set( Actions.CswNbtActionName.DisposeContainer, cispro_request_fulfiller_role, true );

                CswNbtMetaDataNodeType ContainerNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
                if( null != ContainerNodeType )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( new CswNbtPermit.NodeTypePermission[] { CswNbtPermit.NodeTypePermission.Delete, CswNbtPermit.NodeTypePermission.Edit, CswNbtPermit.NodeTypePermission.View },
                                                       ContainerNodeType,
                                                       cispro_request_fulfiller_role,
                                                       true );
                }

                if( ( null != cispro_dispenser_User ) )
                {
                    cispro_dispenser_User.Node.Properties[CswNbtObjClassUser.PropertyName.Role].AsRelationship.RelatedNodeId = cispro_request_fulfiller_role.NodeId;
                    cispro_dispenser_User.postChanges( true );
                }//if we have the dispenser user and the container nt

            }//if we have the request-fulfiller role

            if( null != cispro_dispenser_role )
            {
                cispro_dispenser_role.Node.delete();
            }

            _CswNbtSchemaModTrnsctn.deleteView( "Dispense Requests: Open", true );


            /*
             * Pending Requests View: 
             * Includes all four request node types
             * Includes Needed By, Description, and Requested For (in that order)
             * Sorted by Needed By, Description
             * Description Width = 90
             * Filtered on Status = Pending
             * Filtered by 
             */
            CswNbtView PendingRequestsView = _CswNbtSchemaModTrnsctn.makeSafeView( "Pending Requests", NbtViewVisibility.Global );
            PendingRequestsView.ViewMode = NbtViewRenderingMode.Grid;
            PendingRequestsView.Category = "Requests";
            PendingRequestsView.Width = 100;

            //do this for all
            CswNbtMetaDataNodeType Rel1SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Container Dispense" );
            if( null != Rel1SecondNT )
            {
                CswNbtViewRelationship Rel1 = PendingRequestsView.AddViewRelationship( Rel1SecondNT, true );

                //do this for all
                CswNbtMetaDataNodeTypeProp Prop2NTP = Rel1SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Status );
                CswNbtViewProperty Prop2 = PendingRequestsView.AddViewProperty( Rel1, Prop2NTP );
                Prop2.ShowInGrid = false;
                CswNbtViewPropertyFilter Filt3 = PendingRequestsView.AddViewPropertyFilter( Prop2,
                                                          CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                          CswNbtPropFilterSql.FilterResultMode.Hide,
                                                          CswNbtSubField.SubFieldName.Value,
                                                          CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                          "Pending",
                                                          false,
                                                          false );

                CswNbtMetaDataNodeTypeProp Prop4NTP = Rel1SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Description );
                CswNbtViewProperty Prop4 = PendingRequestsView.AddViewProperty( Rel1, Prop4NTP );
                Prop4.SortBy = true;
                Prop4.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop4.Order = 2;
                Prop4.Width = 90;
                CswNbtMetaDataNodeTypeProp Prop5NTP = Rel1SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.RequestedFor );
                CswNbtViewProperty Prop5 = PendingRequestsView.AddViewProperty( Rel1, Prop5NTP );
                Prop5.Order = 3;
                CswNbtMetaDataNodeTypeProp Prop6NTP = Rel1SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.NeededBy );
                CswNbtViewProperty Prop6 = PendingRequestsView.AddViewProperty( Rel1, Prop6NTP );
                Prop6.SortBy = true;
                Prop6.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop6.Order = 1;

            }//if we have the request dispense nt


            CswNbtMetaDataNodeType Rel7SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Material Create" );
            if( null != Rel7SecondNT )
            {
                CswNbtViewRelationship Rel7 = PendingRequestsView.AddViewRelationship( Rel7SecondNT, true );

                CswNbtMetaDataNodeTypeProp Prop8NTP = Rel7SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Status );
                CswNbtViewProperty Prop8 = PendingRequestsView.AddViewProperty( Rel7, Prop8NTP );
                Prop8.ShowInGrid = false;
                CswNbtViewPropertyFilter Filt9 = PendingRequestsView.AddViewPropertyFilter( Prop8,
                                                          CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                          CswNbtPropFilterSql.FilterResultMode.Hide,
                                                          CswNbtSubField.SubFieldName.Value,
                                                          CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                          "Pending",
                                                          false,
                                                          false );

                CswNbtMetaDataNodeTypeProp Prop10NTP = Rel7SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Description );
                CswNbtViewProperty Prop10 = PendingRequestsView.AddViewProperty( Rel7, Prop10NTP );
                Prop10.SortBy = true;
                Prop10.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop10.Order = 2;
                Prop10.Width = 90;
                CswNbtMetaDataNodeTypeProp Prop11NTP = Rel7SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.RequestedFor );
                CswNbtViewProperty Prop11 = PendingRequestsView.AddViewProperty( Rel7, Prop11NTP );
                Prop11.Order = 3;
                CswNbtMetaDataNodeTypeProp Prop12NTP = Rel7SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.NeededBy );
                CswNbtViewProperty Prop12 = PendingRequestsView.AddViewProperty( Rel7, Prop12NTP );
                Prop12.SortBy = true;
                Prop12.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop12.Order = 1;

            }//if we have "Request Material Create"


            CswNbtMetaDataNodeType Rel13SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Container Update" );
            if( null != Rel13SecondNT )
            {
                CswNbtViewRelationship Rel13 = PendingRequestsView.AddViewRelationship( Rel13SecondNT, true );

                CswNbtMetaDataNodeTypeProp Prop14NTP = Rel13SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Status );
                CswNbtViewProperty Prop14 = PendingRequestsView.AddViewProperty( Rel13, Prop14NTP );
                Prop14.ShowInGrid = false;
                CswNbtViewPropertyFilter Filt15 = PendingRequestsView.AddViewPropertyFilter( Prop14,
                                                          CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                          CswNbtPropFilterSql.FilterResultMode.Hide,
                                                          CswNbtSubField.SubFieldName.Value,
                                                          CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                          "Pending",
                                                          false,
                                                          false );

                CswNbtMetaDataNodeTypeProp Prop16NTP = Rel13SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Description );
                CswNbtViewProperty Prop16 = PendingRequestsView.AddViewProperty( Rel13, Prop16NTP );
                Prop16.SortBy = true;
                Prop16.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop16.Order = 2;
                Prop16.Width = 90;
                CswNbtMetaDataNodeTypeProp Prop17NTP = Rel13SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.RequestedFor );
                CswNbtViewProperty Prop17 = PendingRequestsView.AddViewProperty( Rel13, Prop17NTP );
                Prop17.Order = 3;
                CswNbtMetaDataNodeTypeProp Prop18NTP = Rel13SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.NeededBy );
                CswNbtViewProperty Prop18 = PendingRequestsView.AddViewProperty( Rel13, Prop18NTP );
                Prop18.SortBy = true;
                Prop18.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop18.Order = 1;

            }//if we have "Request Container Update" nt



            CswNbtMetaDataNodeType Rel19SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Material Dispense" );
            if( null != Rel19SecondNT )
            {
                CswNbtViewRelationship Rel19 = PendingRequestsView.AddViewRelationship( Rel19SecondNT, true );

                CswNbtMetaDataNodeTypeProp Prop20NTP = Rel19SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Status );
                CswNbtViewProperty Prop20 = PendingRequestsView.AddViewProperty( Rel19, Prop20NTP );
                Prop20.ShowInGrid = false;
                CswNbtViewPropertyFilter Filt21 = PendingRequestsView.AddViewPropertyFilter( Prop20,
                                                          CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                          CswNbtPropFilterSql.FilterResultMode.Hide,
                                                          CswNbtSubField.SubFieldName.Value,
                                                          CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                          "Pending",
                                                          false,
                                                          false );

                CswNbtMetaDataNodeTypeProp Prop22NTP = Rel19SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Description );
                CswNbtViewProperty Prop22 = PendingRequestsView.AddViewProperty( Rel19, Prop22NTP );
                Prop22.SortBy = true;
                Prop22.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop22.Order = 2;
                Prop22.Width = 90;
                CswNbtMetaDataNodeTypeProp Prop23NTP = Rel19SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RequestedFor );
                CswNbtViewProperty Prop23 = PendingRequestsView.AddViewProperty( Rel19, Prop23NTP );
                Prop23.Order = 3;
                CswNbtMetaDataNodeTypeProp Prop24NTP = Rel19SecondNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NeededBy );
                CswNbtViewProperty Prop24 = PendingRequestsView.AddViewProperty( Rel19, Prop24NTP );
                Prop24.SortBy = true;
                Prop24.SortMethod = NbtViewPropertySortMethod.Ascending;
                Prop24.Order = 1;

            }//if we have "Request Material Dispense" nt

            PendingRequestsView.save();


            if( null != cispro_request_fulfiller_role )
            {
                LandingPage.LandingPageData.Request RequestFulfillerLandingPageRequest = new LandingPage.LandingPageData.Request
                {
                    Type = CswNbtLandingPageItemType.Link,
                    ViewType = "View",
                    PkValue = PendingRequestsView.ViewId.ToString(),
                    NodeTypeId = string.Empty,
                    Text = PendingRequestsView.ViewName,
                    RoleId = cispro_request_fulfiller_role.NodeId.ToString(),
                    ActionId = string.Empty,
                    NewRow = 1,
                    NewColumn = 2
                };

                _CswNbtSchemaModTrnsctn.getLandingPageTable().addLandingPageItem( RequestFulfillerLandingPageRequest );

            }//if we have fulfiller role


        } //Update()

    }//class CswUpdateSchema_01Y_Case28875

}//namespace ChemSW.Nbt.Schema