using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29680
    /// </summary>
    public class CswUpdateSchema_02C_Case29680_Constituent3 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29680; }
        }

        public override void update()
        {
            // Remove the "Define Sizes for this Material" landing page item for the Create Material wizard
            // Originally added in CswUpdateSchema_01T_Case27881

            CswNbtLandingPageTable LandingPageObj = _CswNbtSchemaModTrnsctn.getLandingPageTable();
            string CreateMaterialActionId = _CswNbtSchemaModTrnsctn.Actions[CswEnumNbtActionName.Create_Material].ActionId.ToString();
            string RoleId = "nodes_1";
            CswNbtObjClassRole AdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( null != AdminRole )
            {
                RoleId = AdminRole.NodeId.ToString();
            }

            LandingPageData Items = LandingPageObj.getLandingPageItems( new LandingPageData.Request()
                {
                    RoleId = RoleId,
                    ActionId = CreateMaterialActionId
                } );
            foreach( LandingPageData.LandingPageItem Item in Items.LandingPageItems )
            {
                if( Item.Text == "Define Sizes for this Material" )
                {
                    LandingPageObj.deleteLandingPageItem( new LandingPageData.Request()
                        {
                            LandingPageId = CswConvert.ToInt32( Item.LandingPageId )
                        } );
                }
            } // foreach
        } // update()

    }//class CswUpdateSchema_02C_Case29680_Constituent3

}//namespace ChemSW.Nbt.Schema