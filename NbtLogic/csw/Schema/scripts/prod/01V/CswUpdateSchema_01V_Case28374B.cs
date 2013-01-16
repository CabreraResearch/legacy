using System.Collections.Generic;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28374B
    /// </summary>
    public class CswUpdateSchema_01V_Case28374B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28374; }
        }

        public override void update()
        {
            //6-8 - update action and view permissions for CIS_Pro roles, and add WelcomePage Items
            CswNbtLandingPageTable LandingPageObj = _CswNbtSchemaModTrnsctn.getLandingPageTable();
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, false ) )
            {
                LandingPageData.Request Request;
                if( RoleNode.Name.Text == "CISPro_Admin" )
                {
                    //Actions - edit view, multi-edit, reconcile, sessions, subscriptions
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Edit_View, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Multi_Edit, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Reconciliation, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Sessions, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Subscriptions, RoleNode, true );
                    
                    //Views (and Welcome Items) - roles and users, reports                    
                    List<CswNbtView> RolesAndUsersViews = _CswNbtSchemaModTrnsctn.restoreViews( "Roles and Users" );
                    CswNbtView RolesAndUsersView = null;
                    foreach( CswNbtView View in RolesAndUsersViews )
                    {
                        if( View.VisibilityRoleId != null || View.ViewVisibility == NbtViewVisibility.Global.ToString() )
                        {
                            RolesAndUsersView = View;
                            if( RolesAndUsersView.VisibilityRoleId == RoleNode.NodeId )
                            {
                                break;
                            }
                        }
                    }                    
                    if( null != RolesAndUsersView )
                    {
                        if( RolesAndUsersView.VisibilityRoleId != RoleNode.NodeId && RolesAndUsersView.ViewVisibility != NbtViewVisibility.Global.ToString() )
                        {
                            RolesAndUsersView = _CswNbtSchemaModTrnsctn.makeNewView(
                                "Roles and Users", RolesAndUsersView.Visibility, RoleNode.NodeId, null, RolesAndUsersView.ViewId.get());
                            RolesAndUsersView.save();
                        }
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = RolesAndUsersView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = RolesAndUsersView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 1,
                            NewColumn = 1
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }

                    List<CswNbtView> ReportsViews = _CswNbtSchemaModTrnsctn.restoreViews( "Reports" );
                    CswNbtView ReportsView = null;
                    foreach( CswNbtView View in ReportsViews )
                    {
                        if( View.VisibilityRoleId != null || View.ViewVisibility == NbtViewVisibility.Global.ToString() )
                        {
                            ReportsView = View;
                            if( ReportsView.VisibilityRoleId == RoleNode.NodeId )
                            {
                                break;
                            }
                        }
                    }     
                    if( null != ReportsView )
                    {
                        if( ReportsView.VisibilityRoleId != RoleNode.NodeId && ReportsView.ViewVisibility != NbtViewVisibility.Global.ToString() )
                        {
                            ReportsView = _CswNbtSchemaModTrnsctn.makeNewView(
                                "Reports", ReportsView.Visibility, RoleNode.NodeId, null, ReportsView.ViewId.get() );
                            ReportsView.save();
                        }
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = ReportsView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = ReportsView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 1,
                            NewColumn = 2
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                    //WelcomeItems - work units, vendors, UOM, Locations, Inventory Groups, Regulatory Listss
                    CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.restoreView( "Locations", NbtViewVisibility.Global );
                    if( null != LocationsView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = LocationsView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = LocationsView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 1,
                            NewColumn = 3
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                    CswNbtView VendorsView = _CswNbtSchemaModTrnsctn.restoreView( "Vendors", NbtViewVisibility.Global );
                    if( null != VendorsView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = VendorsView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = VendorsView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 2,
                            NewColumn = 1
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                    CswNbtView WorkUnitsView = _CswNbtSchemaModTrnsctn.restoreView( "Work Units", NbtViewVisibility.Global );
                    if( null != WorkUnitsView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = WorkUnitsView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = WorkUnitsView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 2,
                            NewColumn = 2
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                    CswNbtView InventoryGroupsView = _CswNbtSchemaModTrnsctn.restoreView( "Inventory Groups", NbtViewVisibility.Global );
                    if( null != InventoryGroupsView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = InventoryGroupsView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = InventoryGroupsView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 2,
                            NewColumn = 3
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                    CswNbtView UoMView = _CswNbtSchemaModTrnsctn.restoreView( "Units of Measurement", NbtViewVisibility.Global );
                    if( null != UoMView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = UoMView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = UoMView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 3,
                            NewColumn = 1
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                    CswNbtView RegListsView = _CswNbtSchemaModTrnsctn.restoreView( "Regulatory Lists", NbtViewVisibility.Role );
                    if( null != RegListsView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = RegListsView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = RegListsView.ViewName,
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 3,
                            NewColumn = 2
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }

                    RoleNode.postChanges( false );
                }
                else if( RoleNode.Name.Text == "CISPro_General" )
                {
                    //Actions - edit view, multi-edit, subscriptions
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Edit_View, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Multi_Edit, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Subscriptions, RoleNode, true );

                    //WelcomeItems - My Containers, My Expired Containers, My Cart
                    CswNbtView MyContainersView = _CswNbtSchemaModTrnsctn.restoreView( "My Containers", NbtViewVisibility.Global );
                    if( null != MyContainersView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = MyContainersView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = "My Containers",
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 1,
                            NewColumn = 1
                        };
                        LandingPageObj.addLandingPageItem(Request);
                    }
                    CswNbtView MyExpiredContainersView = _CswNbtSchemaModTrnsctn.restoreView( "My Expiring Containers", NbtViewVisibility.Global );
                    if( null != MyExpiredContainersView )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "View",
                            PkValue = MyExpiredContainersView.ViewId.ToString(),
                            NodeTypeId = String.Empty,
                            Text = "My Expiring Containers",
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = String.Empty,
                            NewRow = 1,
                            NewColumn = 2
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                    string SubmitRequestActionId = _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Submit_Request].ActionId.ToString();
                    Request = new LandingPageData.Request
                    {
                        Type = CswNbtLandingPageItemType.Link,
                        ViewType = "Action",
                        PkValue = SubmitRequestActionId,
                        NodeTypeId = String.Empty,
                        Text = "My Cart",
                        RoleId = RoleNode.NodeId.ToString(),
                        ActionId = String.Empty,
                        NewRow = 1,
                        NewColumn = 3
                    };
                    LandingPageObj.addLandingPageItem( Request );

                    RoleNode.postChanges( false );
                }
                else if( RoleNode.Name.Text == "CISPro_Receiver" )
                {
                    //Actions - edit view, multi-edit, subscriptions
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Edit_View, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Multi_Edit, RoleNode, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Subscriptions, RoleNode, true );

                    //WelcomeItems - Create Material, search materials (requires restored search - not available yet)>>"Receive"
                    string CreateMaterialActionId = _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Create_Material].ActionId.ToString();
                    Request = new LandingPageData.Request
                    {
                        Type = CswNbtLandingPageItemType.Link,
                        ViewType = "Action",
                        PkValue = CreateMaterialActionId,
                        NodeTypeId = String.Empty,
                        Text = "Create Material",
                        RoleId = RoleNode.NodeId.ToString(),
                        ActionId = String.Empty,
                        NewRow = 1,
                        NewColumn = 1
                    };
                    LandingPageObj.addLandingPageItem( Request );

                    RoleNode.postChanges( false );
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase_01V_28374B

}//namespace ChemSW.Nbt.Schema