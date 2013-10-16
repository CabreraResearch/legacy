using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case28518A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28518; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "A"; }
        }

        public override string Title
        {
            get { return "02H_Case28518A"; }
        }

        public override void update()
        {
            // Delete all 'Roles and Users' views except 1
            CswNbtObjClassRole ChemSWAdmin = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            foreach( CswNbtView CurrentView in _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Roles and Users" ).Where( CurrentView => CurrentView.VisibilityRoleId != ChemSWAdmin.NodeId ) )
            {
                CurrentView.Delete();
            }
        }// update()
    }

}//namespace ChemSW.Nbt.Schema