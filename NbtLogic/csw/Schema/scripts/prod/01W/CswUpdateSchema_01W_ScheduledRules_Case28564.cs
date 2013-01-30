using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
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
                _CswNbtSchemaModTrnsctn.Permit.can( CswNbtActionName.View_Scheduled_Rules, CswAdmin );
            }
        }

    }//class CswUpdateSchema_01V_CaseXXXXX

}//namespace ChemSW.Nbt.Schema