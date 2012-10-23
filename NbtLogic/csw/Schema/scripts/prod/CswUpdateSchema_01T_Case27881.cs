using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.Nbt.LandingPage;
using System.Data;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27881
    /// </summary>
    public class CswUpdateSchema_01T_Case27881 : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region Init

            CswNbtLandingPageTable LandingPageObj = _CswNbtSchemaModTrnsctn.getLandingPageTable();
            string CreateMaterialActionId = _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Create_Material].ActionId.ToString();
            string RoleId = "nodes_1";
            CswNbtObjClassRole AdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( null != AdminRole )
            {
                RoleId = AdminRole.NodeId.ToString();
            }

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            #endregion Init

            #region Create Another Material

            LandingPageData.Request Request = new LandingPageData.Request
            {
                Type = CswNbtLandingPageItemType.Link.ToString(),
                ViewType = "Action",
                PkValue = CreateMaterialActionId,
                NodeTypeId = String.Empty,
                Text = "Create Another Material",
                RoleId = RoleId,
                ActionId = CreateMaterialActionId,
                NewRow = 1,
                NewColumn = 3
            };
            LandingPageObj.addLandingPageItem( Request );

            #endregion Create Another Material

            #region Receive this Material

            CswNbtMetaDataObjectClassProp ReceiveProp =
                _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialOc.ObjectClassId,
                                                                    CswNbtObjClassMaterial.PropertyName.Receive );
            if( null != ReceiveProp )
            {
                Request = new LandingPageData.Request
                {
                    Type = CswNbtLandingPageItemType.Button.ToString(),
                    ViewType = String.Empty,
                    PkValue = ReceiveProp.PropId.ToString(),
                    NodeTypeId = String.Empty,
                    Text = "Receive this Material",
                    RoleId = RoleId,
                    ActionId = CreateMaterialActionId,
                    NewRow = 1,
                    NewColumn = 1
                };
                LandingPageObj.addLandingPageItem( Request );
            }

            #endregion Receive this Material

            #region Request this Material

            CswNbtMetaDataObjectClassProp RequestProp =
                _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp(MaterialOc.ObjectClassId,
                                                                    CswNbtObjClassMaterial.PropertyName.Request);
            if( null != RequestProp )
            {
                Request = new LandingPageData.Request
                {
                    Type = CswNbtLandingPageItemType.Button.ToString(),
                    ViewType = String.Empty,
                    PkValue = RequestProp.PropId.ToString(),
                    NodeTypeId = String.Empty,
                    Text = "Request this Material",
                    RoleId = RoleId,
                    ActionId = CreateMaterialActionId,
                    NewRow = 1,
                    NewColumn = 2
                };
                LandingPageObj.addLandingPageItem( Request );
            }

            #endregion Request this Material

            #region Define Sizes for this Material

            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );
            CswNbtMetaDataNodeType SizeNt = SizeOc.FirstNodeType;
            if( null != SizeNt )
            {
                Request = new LandingPageData.Request
                {
                    Type = CswNbtLandingPageItemType.Add.ToString(),
                    ViewType = String.Empty,
                    PkValue = String.Empty,
                    NodeTypeId = SizeNt.NodeTypeId.ToString(),
                    Text = "Define Sizes for this Material",
                    RoleId = RoleId,
                    ActionId = CreateMaterialActionId,
                    NewRow = 2,
                    NewColumn = 1
                };
                LandingPageObj.addLandingPageItem(Request);
            }

            #endregion Define Sizes for this Material

            #region Enter GHS Data for this Material
            
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                if( MaterialNt.NodeTypeName == "Chemical" )
                {
                    CswNbtMetaDataNodeTypeTab GHSTab = MaterialNt.getNodeTypeTab( "GHS" );
                    if( null != GHSTab )
                    {
                        Request = new LandingPageData.Request
                        {
                            Type = CswNbtLandingPageItemType.Tab.ToString(),
                            ViewType = "View",
                            PkValue = GHSTab.TabId.ToString(),
                            NodeTypeId = MaterialNt.NodeTypeId.ToString(),
                            Text = "Enter GHS Data for this Material",
                            RoleId = RoleId,
                            ActionId = CreateMaterialActionId,
                            NewRow = 2,
                            NewColumn = 2
                        };
                        LandingPageObj.addLandingPageItem( Request );
                    }
                }
            }

            #endregion Enter GHS Data for this Material

        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27881; }
        }

    }//class CswUpdateSchema_01T_Case27881

}//namespace ChemSW.Nbt.Schema