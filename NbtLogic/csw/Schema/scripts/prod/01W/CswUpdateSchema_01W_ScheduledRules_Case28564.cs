using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01W_ScheduledRules_Case28564: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 28564; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction(CswNbtModuleName.NBTManager, CswNbtActionName.View_Scheduled_Rules );
            CswNbtObjClassRole CswAdmin = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            if( null != CswAdmin )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.View_Scheduled_Rules, CswAdmin, value: true );
            }

            CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            Int32 ActionId = _CswNbtSchemaModTrnsctn.getActionId( CswNbtActionName.View_Scheduled_Rules );

            foreach( CswNbtNode RoleNode in RoleOc.getNodes(includeSystemNodes: false, forceReInit: true) )
            {
                if( _CswNbtSchemaModTrnsctn.Permit.can( CswNbtActionName.View_Scheduled_Rules, RoleNode ) )
                {
                    CswNbtLandingPageTable Lp = _CswNbtSchemaModTrnsctn.getLandingPageTable();
                    Lp.addLandingPageItem(new LandingPageData.Request
                        {
                            RoleId = RoleNode.NodeId.ToString(),
                            ActionId = ActionId.ToString(),
                            Text = "View Scheduled Rules",
                            Type = CswNbtLandingPageItemType.Link,
                            ViewType = "Action",
                            PkValue = ActionId.ToString(),
                            NewRow = 1,
                            NewColumn = 2
                        });
                }
            }

        }

    }//class CswUpdateSchema_01V_CaseXXXXX

}//namespace ChemSW.Nbt.Schema