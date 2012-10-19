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
            CswNbtLandingPageTable LandingPageObj = _CswNbtSchemaModTrnsctn.getLandingPageTable();
            string CreateMaterialActionId = _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Create_Material].ActionId.ToString();

            //Create Another Material
            LandingPageData.Request Request = new LandingPageData.Request
            {
                Type = "Link",
                ViewType = "Action",
                PkValue = CreateMaterialActionId,
                NodeTypeId = String.Empty,
                Text = "Create Another Material",
                RoleId = "nodes_1",
                ActionId = CreateMaterialActionId
            };
            LandingPageObj.addLandingPageItem( Request );

            //View this Material
            Request = new LandingPageData.Request
            {
                Type = "Link",
                ViewType = "View",
                PkValue = "0",//this will probably change
                NodeTypeId = String.Empty,
                Text = "View this Material",
                RoleId = "nodes_1",
                ActionId = CreateMaterialActionId
            };
            LandingPageObj.addLandingPageItem( Request );

            //todo - add all default createMaterial landing page items:
            //GHS (tab)
            //Receive this Material (Action)
            //Request This Material (Action)
            //Defines Sizes for this Material (tab? view?)
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